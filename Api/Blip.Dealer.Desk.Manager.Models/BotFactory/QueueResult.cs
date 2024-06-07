using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.BotFactory;

public sealed class QueueResult
{
    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("results")]
    public IEnumerable<Queue> Results { get; set; }
}

public sealed class Queue
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("uniqueId")]
    public string UniqueId { get; set; }
}