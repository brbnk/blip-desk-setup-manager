using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Commands;

public sealed class TagsResource(IList<Tag> tags)
{
    [JsonProperty("tags")]
    public IList<Tag> Tags { get; set; } = tags;

    [JsonProperty("hasTags")]
    public bool HasTags { get; private set; } = tags.Any();

    [JsonProperty("isTagsRequired")]
    public bool IsTagsRequired { get; set; } = false;
}