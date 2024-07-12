using System.Text.RegularExpressions;
using Blip.Dealer.Desk.Manager.Models;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.Blip.Commands;
using Blip.Dealer.Desk.Manager.Models.Blip.Replies;
using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Lime.Protocol;
using Newtonsoft.Json;
using Serilog;

namespace Blip.Dealer.Desk.Manager.Services;

public sealed class BlipCommandService(ILogger logger) : IBlipCommandService
{
    public IBlipClient BlipClient { get; set; }

    public async Task<RouterConfiguration> GetApplicationAdvancedSettings(string shortName, string botAuthKey)
    {
        var command = new Command
        {
            Uri = $"lime://business.master.hosting@msging.net/configuration",
            Method = CommandMethod.Get,
            To = "postmaster@msging.net"
        };

        var cts = new CancellationTokenSource();

        var result = await BlipClient.SendCommandAsync(botAuthKey, command, cts.Token);

        if (result.Status != CommandStatus.Success)
        {
            logger.Error("Error to get {ShortName} advanced settings: {Reason}", shortName, result.Reason?.Description);
            return new();
        }

        var resource = JsonConvert.SerializeObject(result.Resource);
        var settings = JsonConvert.DeserializeObject<AdvancedSettings>(resource);
        var application = JsonConvert.DeserializeObject<RouterConfiguration>(settings.Application);

        logger.Information("Success to get {ShortName} advanced settings", shortName);

        return application;
    }

    public async Task PublishBuilderPublishedConfigurationAsync(string shortName, string botAuthKey)
    {
        var command = new Command
        {
            Uri = $"/buckets/blip_portal%3Abuilder_published_configuration",
            Method = CommandMethod.Set,
            To = "postmaster@msging.net",
            Resource = new BuilderWorkingConfigurationResource(botAuthKey)
        };

        var cts = new CancellationTokenSource();

        var result = await BlipClient.SendCommandAsync(botAuthKey, command, cts.Token);

        if (result.Status == CommandStatus.Success)
        {
            logger.Information("Success to get {ShortName} advanced settings", shortName);
        }
        else
        {
            logger.Error("Error to get {ShortName} advanced settings: {Reason}", shortName, result.Reason?.Description);
        }
    }

    public async Task PublishBuilderWorkingConfigurationAsync(string shortName, string botAuthKey)
    {
        var command = new Command
        {
            Uri = $"/buckets/blip_portal%3Abuilder_working_configuration",
            Method = CommandMethod.Set,
            To = "postmaster@msging.net",
            Resource = new BuilderWorkingConfigurationResource(botAuthKey)
        };

        var cts = new CancellationTokenSource();

        var result = await BlipClient.SendCommandAsync(botAuthKey, command, cts.Token);

        if (result.Status == CommandStatus.Success)
        {
            logger.Information("Success to configure builder working configurations for {ShortName}", shortName);
        }
        else
        {
            logger.Error("Error to configure builder working configurations for {ShortName}: {Reason}", shortName, result.Reason?.Description);
        }
    }

    public async Task PublishBusinessBuilderConfigurationAsync(string shortName, string accessKey, string botAuthKey, string flow)
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

        var result = await BlipClient.SendCommandAsync(botAuthKey, command, cts.Token);

        if (result.Status == CommandStatus.Success)
        {
            logger.Information("Success to configure business builder configuration for {ShortName}", shortName);
        }
        else
        {
            logger.Error("Error to configure business builder configuration for {ShortName}: {Reason}", shortName,  result.Reason?.Description);
        }
    }

    public async Task PublishCustomRepliesAsync(string shortName, string botAuthKey, IList<Item> items)
    {
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

        var result = await BlipClient.SendCommandAsync(botAuthKey, command, cts.Token);

        if (result.Status == CommandStatus.Success)
        {
            logger.Information("Success to create custom reply for {DealerName}", shortName);
        }
        else
        {
            logger.Error("Error to create custom reply for {DealerName}: {Reason}", shortName, result.Reason?.Description);
        }
    }

    public async Task PublishPostmasterConfigurationAsync(string shortName, string botAuthKey)
    {
        var command = new Command
        {
            Uri = $"lime://postmaster@desk.msging.net/configuration?caller={shortName}@msging.net",
            Method = CommandMethod.Set,
            To = "postmaster@msging.net",
            Resource = new PostmasterConfigurationResource()
        };

        var cts = new CancellationTokenSource();

        var result = await BlipClient.SendCommandAsync(botAuthKey, command, cts.Token);

        if (result.Status == CommandStatus.Success)
        {
            logger.Information("Success to configure postmaster configuration for {ShortName}", shortName);
        }
        else
        {
            logger.Error("Error to configure postmaster configuration for {ShortName}: {Reason}", shortName,  result.Reason?.Description);
        }
    }

    public async Task PublishServiceHoursAsync(string botAuthKey, ServiceHour serviceHour)
    {
        var command = new Command()
        {
            Uri = "/attendance-hour",
            Method = CommandMethod.Set,
            To = "postmaster@desk.msging.net",
            Resource = new ServiceHourDocument(serviceHour) 
        };

        var cts = new CancellationTokenSource();

        var result = await BlipClient.SendCommandAsync(botAuthKey, command, cts.Token);

        if (result.Status == CommandStatus.Success)
        {
            logger.Information("Success to configure service hours for {QueueName}", serviceHour.AttendanceHour.Title);
        }
        else
        {
            logger.Error("Error to configure service hours for {QueueName}: {Reason}", serviceHour.AttendanceHour.Title, result.Reason?.Description);
        }
    }

    public async Task PublishTagsAsync(string shortName, string botAuthKey, IList<Tag> tags)
    {
        var resource = new TagsResource(tags);

        var command = new Command()
        {
            Uri = $"lime://{shortName}@msging.net/buckets/blip%3Adesk%3Atags",
            Method = CommandMethod.Set,
            To = "postmaster@msging.net",
            Resource = new PlainDocument(JsonConvert.SerializeObject(resource), MediaType.TextPlain)
        };

        var cts = new CancellationTokenSource();

        var result = await BlipClient.SendCommandAsync(botAuthKey, command, cts.Token);

        if (result.Status == CommandStatus.Success)
        {
            logger.Information("Success to create tags for {DealerName}", shortName);
        }
        else
        {
            logger.Error("Error to create tags for {DealerName}: {Reason}", shortName, result.Reason?.Description);
        }
    }
}