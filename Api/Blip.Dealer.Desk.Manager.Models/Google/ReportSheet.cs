using Blip.Dealer.Desk.Manager.Models.Google;

namespace Blip.Dealer.Desk.Manager.Models;

public sealed record ReportSheet() : GoogleSheet
{
    public string BotId { get; set; }

    public string ChatbotStepStatus { get; private set; } = "FAILED";

    public string QueuesStepStatus { get; private set; } = "FAILED";

    public string RulesStepStatus { get; private set; } = "FAILED";

    public void SetBotId(string botId) => BotId = botId;

    public void SetChatbotStepStatus(bool success) => ChatbotStepStatus = success ? "OK" : "FAILED";

    public void SetQueuesStepStatus(bool success) => QueuesStepStatus = success ? "OK" : "FAILED"; 

    public void SetRulesStepStatus(bool success) => RulesStepStatus = success ? "OK" : "FAILED";

    public override string ToString()
    {
        return base.ToString();
    }
}