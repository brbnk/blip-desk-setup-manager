
using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Facades;

public sealed class FlowFacade(IGoogleSheetsService googleSheetsService,
                               IBotFactoryService botFactoryService,
                               IBlipClientFactory blipClientFactory,
                               IBlipCommandService blipCommandService,
                               ILogger logger) : IFlowFacade
{
    private IEnumerable<Application> _applications = [];

    public async Task PublishFlowAsync(PublishFlowRequest request, Stream file)
    {
        logger.Information("Starting...");

        botFactoryService.SetToken(request.GetBearerToken());

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

        blipCommandService.BlipClient = blipClientFactory.InitBlipClient(request.Tenant);

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

            tasks.Add(() => HandleFlowPublishAsync(application.ShortName, request.FlowStr, file));
        }

        foreach (var task in tasks)
        {
            await task();
            logger.Information("--------------------------------------");
        }

        logger.Information("Flows publishing completed!");
    }

    private async Task HandleFlowPublishAsync(string shortName, string flowStr, Stream file)
    {
        var chatbot = await botFactoryService.GetApplicationAsync(shortName);
        var botAuthKey = chatbot?.GetAuthorizationKey();

        if (chatbot is null || botAuthKey is null)
            return;

        await botFactoryService.PublishFlowAsync(chatbot.ShortName, file);

        await blipCommandService.PublishBusinessBuilderConfigurationAsync(chatbot.ShortName, chatbot.AccessKey, botAuthKey, flowStr);

        await blipCommandService.PublishBuilderWorkingConfigurationAsync(chatbot.ShortName, botAuthKey);

        await blipCommandService.PublishBuilderPublishedConfigurationAsync(chatbot.ShortName, botAuthKey);

        await blipCommandService.PublishPostmasterConfigurationAsync(chatbot.ShortName, botAuthKey);
    }
}