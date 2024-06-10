using System.Text;
using System.Text.RegularExpressions;
using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.Blip.Commands;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Lime.Protocol;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Facades;

public sealed class FlowFacade(IGoogleSheetsService googleSheetsService,
                               IBotFactoryService botFactoryService,
                               IBlipClientFactory blipClientFactory,
                               ILogger logger) : IFlowFacade
{
    private IEnumerable<Application> _applications = [];
    private IBlipClient _client;

    public async Task PublishFlowAsync(PublishFlowRequest request, Stream file)
    {
        logger.Information("Starting...");

        botFactoryService.SetToken(request.GetBearerToken());

        _applications = await botFactoryService.GetAllApplicationsAsync(request.Tenant);

        if (_applications is null)
        {
            logger.Error("It was not possible to find application");
            throw new Exception("Error to get all applications");
        }

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

            tasks.Add(HandleFlowPublishAsync(application.ShortName, request.FlowStr, file));
        }

        await Task.WhenAll(tasks.ToArray());

        logger.Information("Tags publishing completed!");
    }

    private static Chatbot SetupChatbot(string brand, string dealerGroup, string tenant)
    {
        var name = $"{brand.Trim().ToUpper()} - {dealerGroup}";

        return new Chatbot(name, tenant, imageUrl: "");
    }

    private async Task HandleFlowPublishAsync(string shortName, string flowStr, Stream file)
    {
        // Get AccessKey
        var chatbot = await botFactoryService.GetApplicationAsync(shortName);

        if (chatbot is null)
            return;

        // Build Authorization key
        var accessKeyDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(chatbot.AccessKey));
        var rawAuthorization = $"{chatbot.ShortName}:{accessKeyDecoded}";
        var botAuthKey = $"Key {Convert.ToBase64String(Encoding.UTF8.GetBytes(rawAuthorization))}";

        await botFactoryService.PublishFlowAsync(chatbot.ShortName, file);

        await PublishBusinessBuilderConfigurationAsync(chatbot.ShortName, chatbot.AccessKey, botAuthKey, flowStr);

        await PublishBuilderWorkingConfigurationAsync(chatbot.ShortName, botAuthKey);

        await PublishBuilderPublishedConfigurationAsync(chatbot.ShortName, botAuthKey);
    }

    private async Task PublishBusinessBuilderConfigurationAsync(string shortName, string accessKey, string botAuthKey, string flow)
    {
        var withShortName = Regex.Replace(flow, "<SHORTNAME>", shortName);
        var withAccessKey = Regex.Replace(withShortName, "<ACCESSKEY>", accessKey);
        var withAuthKey = Regex.Replace(withAccessKey, "<AUTHORIZATIONKEY>", botAuthKey);

        var command = new Command
        {
            Uri = $"lime://business.builder.hosting@msging.net/configuration?caller={shortName}@msging.net",
            Method = CommandMethod.Set,
            To = "postmaster@msging.net",
            Resource = new BusinessConfigurationResource(withAuthKey)
        };

        var cts = new CancellationTokenSource();

        var result = await _client.SendCommandAsync(botAuthKey, command, cts.Token);

        if (result.Status == CommandStatus.Success)
        {
            logger.Information("Success to configure business builder configuration for {ShortName}", shortName);
        }
        else
        {
            logger.Error("Error to configure business builder configuration for {ShortName}", shortName);
        }
    }

    private async Task PublishBuilderWorkingConfigurationAsync(string shortName, string botAuthKey)
    {
        var command = new Command
        {
            Uri = $"/buckets/blip_portal%3Abuilder_working_configuration",
            Method = CommandMethod.Set,
            To = "postmaster@msging.net",
            Resource = new BuilderWorkingConfigurationResource(botAuthKey)
        };

        var cts = new CancellationTokenSource();

        var result = await _client.SendCommandAsync(botAuthKey, command, cts.Token);

        if (result.Status == CommandStatus.Success)
        {
            logger.Information("Success to configure builder working configurations for {ShortName}", shortName);
        }
        else
        {
            logger.Error("Error to configure builder working configurations for {ShortName}", shortName);
        }
    }

    private async Task PublishBuilderPublishedConfigurationAsync(string shortName, string botAuthKey)
    {
        var command = new Command
        {
            Uri = $"/buckets/blip_portal%3Abuilder_published_configuration",
            Method = CommandMethod.Set,
            To = "postmaster@msging.net",
            Resource = new BuilderWorkingConfigurationResource(botAuthKey)
        };

        var cts = new CancellationTokenSource();

        var result = await _client.SendCommandAsync(botAuthKey, command, cts.Token);

        if (result.Status == CommandStatus.Success)
        {
            logger.Information("Success to configure builder published configurations for {ShortName}", shortName);
        }
        else
        {
            logger.Error("Error to configure builder published configurations for {ShortName}", shortName);
        }
    }
}