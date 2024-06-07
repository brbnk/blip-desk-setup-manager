using Blip.Dealer.Desk.Manager.Models.BotFactory;

namespace Blip.Dealer.Desk.Manager.Services.Interfaces;

public interface IBotFactoryService
{
    public Task<Application> GetApplicationAsync(string shortName);

    public Task<IEnumerable<Application>> GetAllApplicationsAsync(string tenantId);

    public Task<bool> CreateChatbotAsync(CreateChatbotRequest request);

    public Task<IEnumerable<Queue>> GetAllQueuesAsync(string chatbotShortName);

    public Task<bool> CreateQueuesAsync(string chatbotShortName, CreateQueuesRequest request);

    public Task<bool> CreateRulesAsync(string chatbotShortName, CreateRulesRequest request);

    public Task<IEnumerable<string>> GetTagsAsync(string chatbotShortName);

    public Task CreateTagsAsync(string chatbotShortName, CreateTagsRequest request);

    public void SetToken(string token);
}