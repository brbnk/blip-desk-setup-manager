using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Facade;

public sealed class AttendantsFacade(IBotFactoryService botFactoryService,
                                     IGoogleSheetsService googleSheetsService,
                                     ILogger logger) : IAttendantsFacade
{
    private IEnumerable<Application> _applications = [];

    public async Task PublishAttendantsAsync(PublishAttendantsRequest request)
    {
        logger.Information("Starting to create Attendants...");

        botFactoryService.SetToken(request.GetBearerToken());

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

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

            Dictionary<string, IList<string>> attendantsMap = [];

            var attendants = dealer.Attendants.Split(";");

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

            tasks.Add(HandlePublishAttendantsAsync(application.ShortName, attendantsMap));
        }

        await Task.WhenAll(tasks);

        logger.Information("Attendants publishing completed!");
    }

    private async Task HandlePublishAttendantsAsync(string shortName, Dictionary<string, IList<string>> attendantsMap)
    {

        var request = new CreateAttendantsRequest()
        {
            Attendants = attendantsMap.Select(item => new Attendant()
            {
                Email = item.Key,
                Teams = [ "Default", "Alteração de Dados" ]
            })
            .ToList()
        };

        await botFactoryService.CreateAttendantsAsync(shortName, request);
    }
}