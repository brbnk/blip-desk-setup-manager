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

    public async Task UpdateCustomRepliesAsync(PublishCustomRepliesRequest request)
    {
        logger.Information("Starting to update Custom Replies...");

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

            tasks.Add(() => HandleCustomReplyUpdateAsync(application.ShortName, request.Items));
        }

        foreach (var task in tasks)
        {
            await task();
        }

        logger.Information("Custom replies updating completed!");
    }

    private async Task HandleCustomReplyPublishAsync(string shortName, IList<Item> items)
    {
        var chatbot = await botFactoryService.GetApplicationAsync(shortName);
        var botAuthKey = chatbot?.GetAuthorizationKey();

        if (chatbot is null || botAuthKey is null)
            return;

        await blipCommandService.PublishCustomRepliesAsync(shortName, botAuthKey, items);
    }

    private async Task HandleCustomReplyUpdateAsync(string shortName, IList<Item> items)
    {
        var chatbot = await botFactoryService.GetApplicationAsync(shortName);
        var botAuthKey = chatbot?.GetAuthorizationKey();

        if (chatbot is null || botAuthKey is null)
            return;
        
        var cateogories = await blipCommandService.GetCustomRepliesAsync(shortName, botAuthKey);

        var itemsDict = new Dictionary<string, IList<Item>>();

        foreach (var item in items)
        {
            if (itemsDict.TryGetValue(item.Category, out var _))
            {
                itemsDict[item.Category].Add(item);
            }
            else
            {
                itemsDict.Add(item.Category, [item]);
            }
        }

        foreach (var group in itemsDict)
        {
            var category = cateogories.FirstOrDefault(c => c.Category.Equals(group.Key));

            if (category is null)
                break;
            
            await blipCommandService.PublishCustomRepliesAsync(shortName, botAuthKey, group.Value, category.Id);
        }

    }
}