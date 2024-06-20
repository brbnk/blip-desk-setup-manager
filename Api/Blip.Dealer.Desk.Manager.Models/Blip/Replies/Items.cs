using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Replies;

public sealed class Item
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("category")]
    public string Category { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("document")]
    public string Document { get; set; }

    [JsonProperty("isDynamicContent")]
    public bool IsDynamicContent { get; set; } =  false;
}