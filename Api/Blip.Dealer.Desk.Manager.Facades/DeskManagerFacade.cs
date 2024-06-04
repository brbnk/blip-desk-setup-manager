using Blip.Dealer.Desk.Manager.Facades.Interfaces;
using Blip.Dealer.Desk.Manager.Models.Blip;
using Blip.Dealer.Desk.Manager.Models.Enums;
using Blip.Dealer.Desk.Manager.Models.Google;
using Blip.Dealer.Desk.Manager.Models.Request;
using Blip.Dealer.Desk.Manager.Services;
using Blip.Dealer.Desk.Manager.Services.Interfaces;

namespace Blip.Dealer.Desk.Manager.Facades;

public sealed class DeskManagerFacade(IGoogleSheetsService googleSheetsService,
                                      IBotFactoryService botFactoryService) : IDeskManagerFacade
{
    public async Task<IEnumerable<DealerSetupSheet>> PublishDealerSetupAsync(PublishDealerSetupRequest request)
    {
        var dealers = await googleSheetsService.SetSpreadSheetId(request.SpreadSheetId)
                                               .ReadAsync<DealerSetupSheet>(request.SheetName, request.Range);

        var groups = dealers.Where(d => !string.IsNullOrWhiteSpace(d.Code) && request.Brand.Equals(d.Brand))
                            .GroupBy(d => d.Group);

        var token = request.GetBearerToken();

        foreach (var group in groups)
        {
            var chatbot = SetupChatbot(request.Brand, group.Key);

            await HandleChatbotCreation(token, chatbot.Id);
        }

        return dealers;
    }

    private static Chatbot SetupChatbot(string brand, string dealerGroup)
    {
        var name =  $"{brand.Trim().ToUpper()} - {dealerGroup}";
        
        return new Chatbot(name);
    }

    private async Task HandleChatbotCreation(string token, string shortName)
    {
        var state = await botFactoryService.CheckChatbotStateAsync(token, shortName);

        if (state.Equals(ChatbotState.UNKNOWN))
        {
            // Log error to get Chatbot data
        }
        else
        {
            if (state.Equals(ChatbotState.NEW)) 
            {
                // Create Chatbot Request
            }
            else
            {
                // Log Chatbot already exists
            }

            await HandleQueuesCreation();
        }
    }

    private async Task HandleQueuesCreation()
    {
        await HandleRulesCreation();
    }

    private async Task HandleRulesCreation()
    {

    }
}