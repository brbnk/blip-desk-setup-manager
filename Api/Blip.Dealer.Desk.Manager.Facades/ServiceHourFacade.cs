using System.Text;
using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.Blip.Attendance;
using Blip.Dealer.Desk.Manager.Models.Blip.Commands;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Lime.Protocol;
using Serilog;
using Queue = Blip.Dealer.Desk.Manager.Models.BotFactory.Queue;

namespace Blip.Dealer.Desk.Manager.Facades;

public sealed class ServiceHourFacade(IGoogleSheetsService googleSheetsService,
                                      IBotFactoryService botFactoryService,
                                      IBlipClientFactory blipClientFactory,
                                      ILogger logger) : IServiceHourFacade
{
    private IEnumerable<Application> _applications = [];

    private IBlipClient _client;
    
    public async Task PublishDealersServiceHoursAsync(PublishServiceHoursRequest request)
    {
        logger.Information("Starting to setup Service Hours ...");

        botFactoryService.SetToken(request.GetBearerToken());

        _client = blipClientFactory.InitBlipClient(request.Tenant);

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

        var groups = await googleSheetsService.ReadAndGroupDealersAsync(request.DataSource.SpreadSheetId, 
                                                                        request.DataSource.Name, 
                                                                        request.DataSource.Range, 
                                                                        request.Brand);

        var tasks = new List<Func<Task>>();

        foreach (var group in groups)
        {
            var chatbot = SetupChatbot(request.Brand, group.Key, request.Tenant);

            var application = _applications.FirstOrDefault(a => a.ShortName.Contains(chatbot.ShortName));

            if (application is null) 
            {
                logger.Warning("Chatbot does not exist: {Group}", group.Key);
                continue;
            }

            tasks.Add(() => HandleServiceHoursPublish(application, request.WorkingHours));
        }   

        foreach (var task in tasks)
        {
            await task();
        }
    }

    private static Chatbot SetupChatbot(string brand, string dealerGroup, string tenant)
    {
        var name =  $"{brand.Trim().ToUpper()} - {dealerGroup}";
        
        return new Chatbot(name, tenant, imageUrl: "");
    }

    private async Task HandleServiceHoursPublish(Application application, IList<AttendanceHourItem> workingHours)
    {
        logger.Information("Starting to create service hours for {Dealer}",  application.Name);

        // Get AccessKey
        var chatbot = await botFactoryService.GetApplicationAsync(application.ShortName);

        if (chatbot is null)
            return;

        // Build Authorization key
        var accessKeyDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(chatbot.AccessKey));
        var rawAuthorization = $"{chatbot.ShortName}:{accessKeyDecoded}";
        var botAuthKey = $"Key {Convert.ToBase64String(Encoding.UTF8.GetBytes(rawAuthorization))}";

        // Request queues
        var queues = await botFactoryService.GetAllQueuesAsync(chatbot.ShortName);

        if (queues is null)
            return;

        var command = new Command()
        {
            Uri = "/attendance-hour",
            Method = CommandMethod.Set,
            To = "postmaster@desk.msging.net"
        };

        // Publish service hour for each queue
        foreach (var queue in queues)
        {
            var serviceHour = CreateServiceHour(queue, workingHours);

            command.Resource = new ServiceHourDocument(serviceHour);

            var cts = new CancellationTokenSource();

            var result = await _client.SendCommandAsync(botAuthKey, command, cts.Token);

            if (result.Status == CommandStatus.Success)
            {
                logger.Information("Success to configure service hours for {QueueName}", queue.Name);
            }
            else 
            {
                logger.Error("Error to configure service hours for {QueueName}", queue.Name);
            }
        }
    }

    private static ServiceHour CreateServiceHour(Queue queue, IList<AttendanceHourItem> workingHours)
    {
        var attendanceHour = new AttendanceHour()
        {
            Title = queue.Name,
            Description = $"Working hours for {queue.Name}",
            IsMain = false
        };

        var queues = new List<AttendanceQueue>()
        { 
            new() { 
                Description = queue.Name,
                Id = queue.UniqueId 
            }
        };

        return new ServiceHour()
        {
            AttendanceHour = attendanceHour,
            Queues = queues,
            AttendanceHourScheduleItems = workingHours,
            AttendanceHourOffItems = []
        };
    }
}