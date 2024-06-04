using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.BotFactory;

public sealed class Application
{
    [JsonProperty("emailOwner")]
    public string EmailOwner { get; set; }

    [JsonProperty("shortName")]
    public string ShortName { get; set; }
}