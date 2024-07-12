using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Blip.Dealer.Manager.Models.Request;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Facades;

public sealed class RouterFacade(ILogger logger,
                                 IBotFactoryService botFactoryService,
                                 IBlipCommandService blipCommandService,
                                 IGoogleSheetsService googleSheetsService,
                                 IBlipClientFactory blipClientFactory) : IRouterFacade
{
    private IEnumerable<Application> _applications = [];

    public async Task<IEnumerable<RouterChild>> GetRouterServicesAsync(RouterServicesRequest request)
    {
        logger.Information("Starting...");

        botFactoryService.SetToken(request.GetBearerToken());

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

        blipCommandService.BlipClient = blipClientFactory.InitBlipClient(request.Tenant);

        var dealers = await googleSheetsService.ReadDealersAsync(request.DataSource.SpreadSheetId,
                                                                 request.DataSource.Name,
                                                                 request.DataSource.Range,
                                                                 request.Brand);
        
        var router = await blipCommandService.GetApplicationAdvancedSettings(request.BotId, request.AuthKey);

        var children = router.Settings.Children.OrderBy(c => c.Id).ToList();

        var lastIndex = children.LastOrDefault()?.Id ?? 0;

        foreach (var dealer in dealers)
        {
            var chatbot = new Chatbot(request.Brand, dealer.FantasyName, request.Tenant, imageUrl: "");

            var application = _applications.FirstOrDefault(a => a.ShortName.Contains(chatbot.ShortName));

            if (application is null)
            {
                logger.Warning("Chatbot does not exist: {Group}", dealer.FantasyName);
                continue;
            }

            if (children.Any(c => c.Service == dealer.Code)) {
                logger.Warning("Service is already on router");
                continue;
            }

            children.Add(new RouterChild() 
            {
                Id = ++lastIndex,
                Service = dealer.Code,
                Identity = $"{application.ShortName}@msging.net",
                LongName = application.Name,
                ShortName = application.ShortName,
                IsOnline = true
            });
        }

        return children;
    }
}
