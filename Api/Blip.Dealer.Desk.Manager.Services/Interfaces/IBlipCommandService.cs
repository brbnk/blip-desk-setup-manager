using Blip.Dealer.Desk.Manager.Models;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.Blip.Attendance;
using Blip.Dealer.Desk.Manager.Models.Blip.Leads;
using Blip.Dealer.Desk.Manager.Models.Blip.Replies;
using Blip.Dealer.Desk.Manager.Models.BotFactory;

namespace Blip.Dealer.Desk.Manager.Services.Interfaces;

public interface IBlipCommandService
{
    public IBlipClient BlipClient { get; set; }

    public Task<RouterConfiguration> GetApplicationAdvancedSettings(string shortName, string botAuthKey);

    public Task PublishTagsAsync(string shortName, string botAuthKey, IList<Tag> tags);

    public Task PublishBusinessBuilderConfigurationAsync(string shortName, string accessKey, string botAuthKey, string flow);

    public Task PublishBuilderWorkingConfigurationAsync(string shortName, string botAuthKey);

    public Task PublishBuilderPublishedConfigurationAsync(string shortName, string botAuthKey);

    public Task PublishCustomRepliesAsync(string shortName, string botAuthKey, IList<Item> items);

    public Task PublishServiceHoursAsync(string botAuthKey, ServiceHour serviceHour);

    public Task PublishPostmasterConfigurationAsync(string shortName, string botAuthKey);

    public Task SetAttendantPermissionAsync(string shortName, string botAuthKey, AttendantPermissionRequest attendantPermissions);

    public Task<Contact> GetContactAsync(string phone, string botAuthKey);

    public Task<T> GetContextAsync<T>(string phone, string context, string botAuthKey);
}