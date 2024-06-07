using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.BotFactory;

public sealed class TagResult
{
    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("results")]
    public IEnumerable<string> Results { get; set; }
}