using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.BotFactory;

public sealed class ApplicationResult
{
    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("results")]
    public IEnumerable<Application> Results { get; set; }
}