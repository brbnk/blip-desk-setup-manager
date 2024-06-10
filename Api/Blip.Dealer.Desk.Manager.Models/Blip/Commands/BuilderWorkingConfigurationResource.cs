using Lime.Protocol;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Commands;

public sealed class BuilderWorkingConfigurationResource(string authorizationKey) : Document(MediaType)
{
    public const string MIME_TYPE = "application/json";

    public static readonly MediaType MediaType = MediaType.Parse(MIME_TYPE);

    [JsonProperty("builder:minimumIntentScore")]
    public string MinIntentScore { get; set; } = "0.5";

    [JsonProperty("builder:stateTrack")]
    public string StateTrack { get; set; } = "true";

    [JsonProperty("builder:useTunnelOwnerContext")]
    public string UseRouterContext { get; set; } = "true";

    [JsonProperty("builder:#localTimeZone")]
    public string LocalTime { get; set; } = "E. South America Standard Time";

    [JsonProperty("authorizationKey")]
    public string AuthorizationKey { get; set; } =  authorizationKey;

    [JsonProperty("gmt")]
    public string Gmt { get; set; } = "-3";
}