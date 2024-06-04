using Blip.Dealer.Desk.Manager.Models.Enums;

namespace Blip.Dealer.Desk.Manager.Services.Interfaces;

public interface IBotFactoryService
{
    public Task<ChatbotState> CheckChatbotStateAsync(string bearerToken, string shortName);
}