using Blip.Dealer.Desk.Manager.Models.Enums;
using Blip.Dealer.Desk.Manager.Services.Interfaces;
using Blip.Dealer.Desk.Manager.Services.RestEase;

namespace Blip.Dealer.Desk.Manager.Services;

public sealed class BotFactoryService(IBotFactoryClient client) : IBotFactoryService
{
    public async Task<ChatbotState> CheckChatbotStateAsync(string bearerToken, string shortName)
    {
        try 
        {
            var chatbot = await client.GetApplicationAsync(bearerToken, shortName);
            
            return chatbot is not null ? ChatbotState.CREATED : ChatbotState.CREATED;
        }
        catch(Exception ex)
        {
            return ChatbotState.UNKNOWN; 
        }
    }
}