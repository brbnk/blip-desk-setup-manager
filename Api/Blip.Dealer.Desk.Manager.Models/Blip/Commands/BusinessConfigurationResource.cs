using Lime.Protocol;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Commands;

public sealed class BusinessConfigurationResource(string applicationFlow) : Document(MediaType)
{
    public const string MIME_TYPE = "application/json";

    public static readonly MediaType MediaType = MediaType.Parse(MIME_TYPE);

    [JsonProperty("Template")]
    public string Template { get; set; } = "builder";

    [JsonProperty("Application")]
    public string Application { get; set; } = applicationFlow;
}