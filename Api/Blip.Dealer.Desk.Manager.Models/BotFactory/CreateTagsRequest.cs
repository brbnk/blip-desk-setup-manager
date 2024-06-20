using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.BotFactory;

public sealed class CreateTagsRequest
{
    [JsonProperty("tags")]
    public IList<Tag> Tags { get; set; }
}

public class Tag
{
    [JsonProperty("text")]
    public string Text { get; set; }
}