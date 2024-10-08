using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.Blip.Attendance;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Facades;

public sealed class ServiceHourFacade(IGoogleSheetsService googleSheetsService,
                                      IBotFactoryService botFactoryService,
                                      IBlipClientFactory blipClientFactory,
                                      IBlipCommandService blipCommandService,
                                      ILogger logger) : IServiceHourFacade
{
    private IEnumerable<Application> _applications = [];
    
    public async Task PublishDealersServiceHoursAsync(PublishServiceHoursRequest request)
    {
        logger.Information("Starting to setup Service Hours ...");

        botFactoryService.SetToken(request.GetBearerToken());

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

        blipCommandService.BlipClient = blipClientFactory.InitBlipClient(request.Tenant);

        var dealers = await googleSheetsService.ReadDealersAsync(request.DataSource.SpreadSheetId, 
                                                                request.DataSource.Name,
                                                                request.DataSource.Range,
                                                                request.Brand);

        var tasks = new List<Task>();

        foreach (var dealer in dealers)
        {
            var chatbot = new Chatbot(request.Brand, dealer.FantasyName, request.Tenant, imageUrl: "");

            var application = _applications.FirstOrDefault(a => a.ShortName.Contains(chatbot.ShortName));

            if (application is null) 
            {
                logger.Warning("Chatbot does not exist: {Group}", dealer.FantasyName);
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

            var defaultServiceHour = CreateDefaultServiceHour(workingHours);

            await blipCommandService.PublishServiceHoursAsync(botAuthKey, defaultServiceHour, application.Name);
        }
        else 
        {
            logger.Warning("Skiping service hours creation for {Dealer} because its empty", application.ShortName);
        }
    }

    private static ServiceHour CreateDefaultServiceHour(IList<AttendanceHourItem> workingHours)
    {
        var attendanceHour = new AttendanceHour()
        {
            Title = "Regular Hours",
            Description = "Default attendance hour",
            IsMain = true
        };

         return new ServiceHour()
        {
            AttendanceHour = attendanceHour,
            Queues = [],
            AttendanceHourScheduleItems = workingHours,
            AttendanceHourOffItems = []
        };
    }
}