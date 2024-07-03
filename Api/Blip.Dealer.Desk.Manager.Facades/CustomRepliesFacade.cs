using System.Text;
using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.Blip.Commands;
using Blip.Dealer.Desk.Manager.Models.Blip.Replies;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Lime.Protocol;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Facades;

public sealed class CustomRepliesFacade(IBotFactoryService botFactoryService,
                                        IGoogleSheetsService googleSheetsService,
                                        IBlipClientFactory blipClientFactory,
                                        IBlipCommandService blipCommandService,
                                        ILogger logger) : ICustomRepliesFacade
{
    private IEnumerable<Application> _applications = [];

    public async Task PublishCustomRepliesAsync(PublishCustomRepliesRequest request)
    {
        logger.Information("Starting to create Custom Replies...");

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

            tasks.Add(HandleCustomReplyPublishAsync(application.ShortName, request.Items));
        }

        await Task.WhenAll(tasks.ToArray());

        logger.Information("Custom replies publishing completed!");
    }

    private async Task HandleCustomReplyPublishAsync(string shortName, IList<Item> items)
    {
        var chatbot = await botFactoryService.GetApplicationAsync(shortName);
        var botAuthKey = chatbot?.GetAuthorizationKey();

        if (chatbot is null || botAuthKey is null)
            return;

        await blipCommandService.PublishCustomRepliesAsync(shortName, botAuthKey, items);
    }
}