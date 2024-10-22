using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.Blip.Attendance;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Facade;

public sealed class AttendantsFacade(IBotFactoryService botFactoryService,
                                     IGoogleSheetsService googleSheetsService,
                                     IBlipCommandService blipCommandService,
                                     IBlipClientFactory blipClientFactory,
                                     ILogger logger) : IAttendantsFacade
{
    private IEnumerable<Application> _applications = [];

    public async Task CheckAttendantsAsync(PublishAttendantsRequest request)
    {
        logger.Information("Starting to check Attendants registration...");

        botFactoryService.SetToken(request.GetBearerToken());

        blipCommandService.BlipClient = blipClientFactory.InitBlipClient(request.Tenant);

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

        var dealers = await googleSheetsService.ReadDealersAsync(request.DataSource.SpreadSheetId,
                                                                 request.DataSource.Name,
                                                                 request.DataSource.Range,
                                                                 request.Brand);

        var tasks = new List<Func<Task>>();

          foreach (var dealer in dealers)
        {
            var chatbot = new Chatbot(request.Brand, dealer.FantasyName, request.Tenant, imageUrl: "");

            var application = _applications.FirstOrDefault(a => a.ShortName.Contains(chatbot.ShortName));

            if (application is null)
            {
                logger.Warning("Chatbot does not exist: {Group}", dealer.FantasyName);
                continue;
            }

            tasks.Add(() => CheckIfAttendantsExist(application));
        }

        foreach (var task in tasks)
        {
            await task();
            Thread.Sleep(1000);
        }

        logger.Information("Attendants check completed!");
    }

    public async Task PublishAttendantsAsync(PublishAttendantsRequest request)
    {
        logger.Information("Starting to create Attendants...");

        botFactoryService.SetToken(request.GetBearerToken());

        blipCommandService.BlipClient = blipClientFactory.InitBlipClient(request.Tenant);

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

        var dealers = await googleSheetsService.ReadDealersAsync(request.DataSource.SpreadSheetId,
                                                                 request.DataSource.Name,
                                                                 request.DataSource.Range,
                                                                 request.Brand);

        var tasks = new List<Func<Task>>();

        foreach (var dealer in dealers)
        {
            var chatbot = new Chatbot(request.Brand, dealer.FantasyName, request.Tenant, imageUrl: "");

            var application = _applications.FirstOrDefault(a => a.ShortName.Contains(chatbot.ShortName));

            if (application is null)
            {
                logger.Warning("Chatbot does not exist: {Group}", dealer.FantasyName);
                continue;
            }

            Dictionary<string, IList<string>> attendantsMap = [];

            var attendants = dealer.Attendants?.Split(";");

            if (attendants is null || attendants.Length == 0) 
            {
                logger.Warning("There is no attendants listed for {Dealer}", dealer.FantasyName);
                continue;
            }

            foreach (var email in attendants)
            {
                if (attendantsMap.TryGetValue(email, out var teams))
                {
                    teams.Add(dealer.FantasyName);
                }
                else
                {
                    attendantsMap.Add(email, [dealer.FantasyName]);
                }
            }

            tasks.Add(() => HandlePublishAttendantsAsync(application.ShortName, attendantsMap));
        }

        foreach (var task in tasks)
        {
            await task();
        }

        logger.Information("Attendants publishing completed!");
    }

    private async Task HandlePublishAttendantsAsync(string shortName, Dictionary<string, IList<string>> attendantsMap)
    {

        var request = new CreateAttendantsRequest()
        {
            Attendants = attendantsMap.Select(item => new Attendant()
            {
                Email = item.Key,
                Teams = [ "Varejo", "Lead em Atendimento", "Alteração de Dados", "Venda em Andamento" ]
            })
            .ToList()
        };

        await botFactoryService.CreateAttendantsAsync(shortName, request);

        var permissions = attendantsMap.Select(item => {
            var email = $"{item.Key.Replace("@", "%2540")}%40blip.ai";

            return new AttendantPermissionRequest() 
            {
                Email = email,
                Permissions = 
                [
                    new() 
                    {
                        OwnerIdentity = $"{shortName}@msging.net",
                        AgentIdentity = email,
                        Name = "canSendActiveMessage",
                        IsActive = true
                    },
                    new() 
                    {
                        OwnerIdentity = $"{shortName}@msging.net",
                        AgentIdentity = email,
                        Name = "canViewAndCreatePaymentLink",
                        IsActive = true
                    }
                ]
            };
        });

        var chatbot = await botFactoryService.GetApplicationAsync(shortName);
        var botAuthKey = chatbot?.GetAuthorizationKey();

        if (chatbot is null || botAuthKey is null)
            return;

        foreach (var permission in permissions)
        {
            await blipCommandService.SetAttendantPermissionAsync(shortName, botAuthKey, permission);
        }
    }

    private async Task CheckIfAttendantsExist(Application application)
    {
        var hasAttendant = await botFactoryService.HasAttendantsAsync(application.ShortName);

        if (!hasAttendant) {
            var chatbot = await botFactoryService.GetApplicationAsync(application.ShortName);
            var botAuthKey = chatbot?.GetAuthorizationKey();
            var ticket = await blipCommandService.GetTicketsAsync(botAuthKey);

            if (ticket is null) 
            {
                logger.Information("No attendants: {Dealer},{Id},0", application.Name, application.ShortName);
            }
            else
            {
                logger.Information("No attendants: {Dealer},{Id},{Count}", application.Name, application.ShortName, ticket.Resource.Count());
            }

        }
    }
}