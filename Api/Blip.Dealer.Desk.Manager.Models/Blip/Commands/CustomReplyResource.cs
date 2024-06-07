using Blip.Dealer.Desk.Manager.Models.Blip.Replies;
using Lime.Protocol;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Commands;

public sealed class CustomReplyResource(CustomReply cr) : Document(MediaType)
{
    public const string MIME_TYPE = "application/vnd.lime.collection+json";

    public static readonly MediaType MediaType = MediaType.Parse(MIME_TYPE);

    [JsonProperty("itemType")]
    public string ItemType { get; set; } = cr.ItemType;

    [JsonProperty("items")]
    public IList<Item> Items { get; set; } = cr.Items;
}