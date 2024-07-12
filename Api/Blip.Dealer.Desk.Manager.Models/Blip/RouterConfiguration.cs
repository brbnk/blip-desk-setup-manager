using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models;

public sealed class RouterConfiguration
{
    [JsonProperty("identifier")]
    public string Identifier { get; set; }

    [JsonProperty("messageReceivers")]
    public IEnumerable<MessageReceiver> MessageReceivers { get; set; }

    [JsonProperty("notificationReceivers")]
    public IEnumerable<NotificationReceiver> NotificationReceivers { get; set; }

    [JsonProperty("startupType")]
    public string StartupType { get; set; }

    [JsonProperty("serviceProviderType")]
    public string ServiceProviderType { get; set; }

    [JsonProperty("disableNotify")]
    public bool DisableNotify { get; set; }

    [JsonProperty("receiptEvents")]
    public IEnumerable<string> ReceiptEvents { get; set; }

    [JsonProperty("settings")]
    public RouterSettings Settings { get; set; }

    [JsonProperty("settingsType")]
    public string SettingsType { get; set; }
}

public sealed class MessageReceiver
{
    [JsonProperty("priority")]
    public int Priority { get; set; }

    [JsonProperty("mediaType")]
    public string MediaType { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("sender")]
    public string Sender { get; set; }
}

public sealed class NotificationReceiver
{
    [JsonProperty("priority")]
    public int Priority { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("sender")]
    public string Sender { get; set; }
}

public sealed class RouterSettings
{
    [JsonProperty("children")]
    public IList<RouterChild> Children { get; set; }
}

public sealed class RouterChild
{
    [JsonProperty("$id")]
    public int Id { get; set; }

    [JsonProperty("isDefault")]
    public bool IsDefault { get; set; } = false;

    [JsonProperty("service")]
    public string Service { get; set; }

    [JsonProperty("identity")]
    public string Identity { get; set; }

    [JsonProperty("longName")]
    public string LongName { get; set; }

    [JsonProperty("shortName")]
    public string ShortName { get; set; }

    [JsonProperty("isPersistent")]
    public bool IsPersistent { get; set; }

    [JsonProperty("isOnline")]
    public bool IsOnline { get; set; }
}