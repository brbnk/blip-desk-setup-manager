using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.BotFactory;

public sealed class CreateQueuesRequest
{
    [JsonProperty("queues")]
    public IList<string> Queues { get; set; } = [];
}