using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.BotFactory;

public sealed class AttedantsResult
{
    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("results")]
    public IEnumerable<Attendant> Results { get; set; }
}