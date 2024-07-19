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

    [JsonProperty("CanSendAudioRecording")]
    public bool CanSendAudioRecording { get; set; } = true;

    [JsonProperty("Extensions")]
    public string Extensions { get; set; } = "{ \"extension1\": { \"name\":\"Selling Online (SOL)\", \"url\":\"https://blip-fca-sol-desk-plugin.hmg-cs.blip.ai?tenant=takeblip-bruno-nakayabu&key=ZGluYW1pY2FzdGVsbGFudGlzcm91dGVyMjpJZzFTWUlnZjhpMDk5TXh3ZW91aA==\", \"view\":\"ticket\" } }";

    #region ACTIVATE DESK NA FEATURE

    [JsonProperty("ActiveMessageSearchSource")]
    public string ActiveMessageSearchSource { get; set; } = "router";

    [JsonProperty("ActiveMessageEnabled")]
    public bool ActiveMessageEnabled { get; set; } = true;

    [JsonProperty("ActiveMessageTicketPriorityEnabled")]
    public bool ActiveMessageTicketPriorityEnabled { get; set; } = false;

    [JsonProperty("RouterIdentityActiveCampaign")]
    public string RouterIdentityActiveCampaign { get; set; } = "testestellantis2@msging.net";

    #endregion

    #region CONFIGURE AUTOMATIC CLOSING OF TICKET

    [JsonProperty("AutomaticClosedTicketIsEnabled")]
    public bool AutomaticClosedTicketIsEnabled { get; set; } = true;

    [JsonProperty("MaxClientMinutesDowntime")]
    public string MaxClientMinutesDowntime { get; set; } = "1380";

    [JsonProperty("ExpectInteractionAgent")]
    public bool ExpectInteractionAgent { get; set; } = true;
    
    [JsonProperty("SetTicketClosedByClientAsClosed")]
    public bool SetTicketClosedByClientAsClosed { get; set; } = true;
    
    [JsonProperty("UseInactivityMessage")]
    public bool UseInactivityMessage { get; set; } = true;
    
    [JsonProperty("UseInactivityTags")]
    public bool UseInactivityTags { get; set; } = true;
    
    [JsonProperty("ResetClientInactivityTimerOnAgentInteraction")]
    public bool ResetClientInactivityTimerOnAgentInteraction { get; set; } = true;
    
    [JsonProperty("ResetClientInactivityTimerOnFirstAgentInteraction")]
    public bool ResetClientInactivityTimerOnFirstAgentInteraction { get; set; } = true;
    
    [JsonProperty("InactivityMessageDowntime")]
    public string InactivityMessageDowntime { get; set; } = "1320";
    
    [JsonProperty("InactivityMessage")]
    public string InactivityMessage { get; set; } = "Olá, você está aí ainda?";

    [JsonProperty("InactivityTags")]
    public string InactivityTags { get; set; } = "[\"Inatividade\"]";

    #endregion
}