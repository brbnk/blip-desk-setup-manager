using Blip.Dealer.Desk.Manager.Models.Blip.Replies;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Commands;

public sealed class CustomReplyResponse
{
    [JsonProperty("total")]
    public int total { get; set; }

    [JsonProperty("itemType")]
    public string ItemType { get; set; }

    [JsonProperty("items")]
    public IList<Item> Items { get; set; }
}