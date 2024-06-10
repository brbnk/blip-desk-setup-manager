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
                                      IBlipCommandService blipCommandService,
                                      ILogger logger) : IServiceHourFacade
{
    private IEnumerable<Application> _applications = [];

    private IBlipClient _client;
    
    public async Task PublishDealersServiceHoursAsync(PublishServiceHoursRequest request)
    {
        logger.Information("Starting to setup Service Hours ...");

        botFactoryService.SetToken(request.GetBearerToken());

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

        blipCommandService.BlipClient = blipClientFactory.InitBlipClient(request.Tenant);

        var groups = await googleSheetsService.ReadAndGroupDealersAsync(request.DataSource.SpreadSheetId, 
                                                                        request.DataSource.Name, 
                                                                        request.DataSource.Range, 
                                                                        request.Brand);

        var tasks = new List<Task>();

        foreach (var group in groups)
        {
            var chatbot = new Chatbot(request.Brand, group.Key, request.Tenant, imageUrl: "");

            var application = _applications.FirstOrDefault(a => a.ShortName.Contains(chatbot.ShortName));

            if (application is null) 
            {
                logger.Warning("Chatbot does not exist: {Group}", group.Key);
                continue;
            }

            tasks.Add(HandleServiceHoursPublishAsync(application, request.WorkingHours));
        }   

        await Task.WhenAll(tasks.ToArray());

        logger.Information("Service hours publishing completed!");
    }

    private async Task HandleServiceHoursPublishAsync(Application application, IList<AttendanceHourItem> workingHours)
    {
        if (workingHours.Any())
        {
            logger.Information("Starting to create service hours for {Dealer}",  application.Name);

            // Get AccessKey
            var chatbot = await botFactoryService.GetApplicationAsync(application.ShortName);
            var botAuthKey = chatbot?.GetAuthorizationKey();

            if (chatbot is null || botAuthKey is null)
                return;

            // Request queues
            var queues = await botFactoryService.GetAllQueuesAsync(chatbot.ShortName);

            if (queues is not null)
            {
                // Publish service hour for each queue
                foreach (var queue in queues)
                {
                    var serviceHour = CreateServiceHour(queue, workingHours);

                    await blipCommandService.PublishServiceHoursAsync(botAuthKey, serviceHour);
                }
            }
        }
        else 
        {
            logger.Warning("Skiping service hours creation for {Dealer} because its empty", application.ShortName);
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