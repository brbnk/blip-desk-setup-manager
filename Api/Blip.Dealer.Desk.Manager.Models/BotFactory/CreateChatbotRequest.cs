using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.BotFactory;

public sealed class CreateChatbotRequest(string tenant, 
                                         string shortName,
                                         string fullName,
                                         string imageUrl = "")
{
    [JsonProperty("tenant")]
    public string Tenant { get; set; } = tenant;

    [JsonProperty("shortName")]
    public string ShortName { get; set; } = shortName;

    [JsonProperty("fullName")]
    public string FullName { get; set; } = fullName;

    [JsonProperty("imageUrl")]
    public string ImageUrl { get; set; } = imageUrl;

    [JsonProperty("shouldBuildUsingBuilder")]
    public bool ShouldBuildUsingBuilder { get; set; } = true;
}