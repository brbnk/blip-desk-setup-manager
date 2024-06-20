using Lime.Protocol;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Commands;

public sealed class PostmasterConfigurationResource() : Document(MediaType)
{
    public const string MIME_TYPE = "application/json";

    public static readonly MediaType MediaType = MediaType.Parse(MIME_TYPE);

    [JsonProperty("AutomaticClosedTicketIsAllowed")]
    public bool AutomaticClosedTicketIsAllowed { get; set; } = true;

    [JsonProperty("IgnoreCreateTicketOnUserMessage")]
    public bool IgnoreCreateTicketOnUserMessage { get; set; } = true;

    [JsonProperty("AttendanceSatisfactionSurveyEnabled")]
    public bool AttendanceSatisfactionSurveyEnabled { get; set; } = false;
}