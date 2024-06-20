using Blip.Dealer.Desk.Manager.Models.Blip.Replies;

namespace Blip.Dealer.Desk.Manager.Models.Blip;

public sealed class CustomReply
{
    public string ItemType { get; set; } = "application/vnd.iris.desk.custom-reply+json";

    public IList<Item> Items { get; set; }
}