using Blip.Dealer.Desk.Manager.Models.BotFactory;
using Blip.Dealer.Desk.Manager.Models.Enums;

namespace Blip.Dealer.Desk.Manager.Services.Interfaces;

public interface IBotFactoryService
{
    public Task<IEnumerable<Application>> GetAllApplicationsAsync(string tenantId);

    public Task<bool> CreateChatbotAsync(CreateChatbotRequest request);

    public Task<IEnumerable<Queue>> GetAllQueuesAsync(string chatbotShortName);

    public Task<bool> CreateQueuesAsync(string chatbotShortName, CreateQueuesRequest request);

    public Task<bool> CreateRulesAsync(string chatbotShortName, CreateRulesRequest request);

    public void SetToken(string token);
}