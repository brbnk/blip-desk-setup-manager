using Blip.Dealer.Desk.Manager.Models.Google;

namespace Blip.Dealer.Desk.Manager.Models;

public sealed record ReportSheet : GoogleSheet
{
    public string Group { get; private set; }

    public string Dealer { get; private set; }

    public string BotId { get; set; }

    public string ChatbotStepStatus { get; private set; } = "FAILED";

    public string QueuesStepStatus { get; private set; } = "FAILED";

    public string RulesStepStatus { get; private set; } = "FAILED";

    public void SetGroup(string group) => Group = group;

    public void SetBotId(string botId) => BotId = botId;

    public void SetDealer(string dealer) => Dealer = dealer;

    public void SetChatbotStepStatus(bool success) => ChatbotStepStatus = success ? "OK" : "FAILED";

    public void SetQueuesStepStatus(bool success) => QueuesStepStatus = success ? "OK" : "FAILED"; 

    public void SetRulesStepStatus(bool success) => RulesStepStatus = success ? "OK" : "FAILED";
}