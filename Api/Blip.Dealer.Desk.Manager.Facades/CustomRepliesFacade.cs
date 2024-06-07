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
                                        ILogger logger) : ICustomRepliesFacade
{
    private IEnumerable<Application> _applications = [];

    private IBlipClient _client;

    public async Task PublishCustomRepliesAsync(PublishCustomRepliesRequest request)
    {
        logger.Information("Starting to create Custom Replies...");

        botFactoryService.SetToken(request.GetBearerToken());

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

        _client = blipClientFactory.InitBlipClient(request.Tenant);

        var groups = await googleSheetsService.ReadAndGroupDealersAsync(request.DataSource.SpreadSheetId,
                                                                        request.DataSource.Name,
                                                                        request.DataSource.Range,
                                                                        request.Brand);
        var tasks = new List<Task>();

        foreach (var group in groups)
        {
            var chatbot = SetupChatbot(request.Brand, group.Key, request.Tenant);

            var application = _applications.FirstOrDefault(a => a.ShortName.Contains(chatbot.ShortName));

            if (application is null)
            {
                logger.Warning("Chatbot does not exist: {Group}", group.Key);
                continue;
            }

            tasks.Add(HandleCustomReplyPublishAsync(application.ShortName, request.Items));
        }

        await Task.WhenAll(tasks.ToArray());

        logger.Information("Custom replies publishing completed!");
    }

    private static Chatbot SetupChatbot(string brand, string dealerGroup, string tenant)
    {
        var name = $"{brand.Trim().ToUpper()} - {dealerGroup}";

        return new Chatbot(name, tenant, imageUrl: "");
    }

    private async Task HandleCustomReplyPublishAsync(string shortName, IList<Item> items)
    {
        var chatbot = await botFactoryService.GetApplicationAsync(shortName);

        if (chatbot is null)
            return;

        var accessKeyDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(chatbot.AccessKey));
        var rawAuthorization = $"{chatbot.ShortName}:{accessKeyDecoded}";
        var botAuthKey = $"Key {Convert.ToBase64String(Encoding.UTF8.GetBytes(rawAuthorization))}";

        var customReplyId = Guid.NewGuid();

        foreach (var item in items)
        {
            item.Id = customReplyId.ToString();
        }

        var command = new Command()
        {
            Uri = $"/replies/{customReplyId}",
            Method = CommandMethod.Set,
            To = "postmaster@desk.msging.net",
            Resource = new CustomReplyResource(new() { Items = items })
        };

        var cts = new CancellationTokenSource();

        var result = await _client.SendCommandAsync(botAuthKey, command, cts.Token);

        if (result.Status == CommandStatus.Success)
            logger.Information("Success to create custom reply for {DealerName}", shortName);
        else
            logger.Error("Error to create custom reply for {DealerName}", shortName);
    }
}