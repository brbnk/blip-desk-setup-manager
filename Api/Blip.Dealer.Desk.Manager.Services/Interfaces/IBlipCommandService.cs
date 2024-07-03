using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.Blip.Replies;
using Blip.Dealer.Desk.Manager.Models.BotFactory;

namespace Blip.Dealer.Desk.Manager.Services.Interfaces;

public interface IBlipCommandService
{
    public IBlipClient BlipClient { get; set; }

    Task PublishTagsAsync(string shortName, string botAuthKey, IList<Tag> tags);

    public Task PublishBusinessBuilderConfigurationAsync(string shortName, string accessKey, string botAuthKey, string flow);

    public Task PublishBuilderWorkingConfigurationAsync(string shortName, string botAuthKey);

    public Task PublishBuilderPublishedConfigurationAsync(string shortName, string botAuthKey);

    public Task PublishCustomRepliesAsync(string shortName, string botAuthKey, IList<Item> items);

    public Task PublishServiceHoursAsync(string botAuthKey, ServiceHour serviceHour);

    public Task PublishPostmasterConfigurationAsync(string shortName, string botAuthKey);
}