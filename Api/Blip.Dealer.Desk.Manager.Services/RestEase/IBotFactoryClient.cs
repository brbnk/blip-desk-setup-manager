using Blip.Dealer.Desk.Manager.Models.BotFactory;
using RestEase;

namespace Blip.Dealer.Desk.Manager.Services.RestEase;

[BasePath("api")]
public interface IBotFactoryClient
{
    private const string ACCESS_TOKEN_HEADER = "X-Blip-User-Access-Token";

    [Get("application")]
    public Task<Application> GetAplicationAsync([Header(ACCESS_TOKEN_HEADER)] string token, string shortName);

    [Get("application/all")]
    public Task<ApplicationResult> GetAllAplicationsAsync([Header(ACCESS_TOKEN_HEADER)] string token, [Query("tenant-id")] string tenantId);

    [Post("application/chatbot")]
    public Task CreateChatbotAsync([Header(ACCESS_TOKEN_HEADER)] string token, [Body] CreateChatbotRequest request);

    [Get("desk/queues")]
    public Task<QueueResult> GetAllQueuesAsync([Header(ACCESS_TOKEN_HEADER)] string token, string shortName);

    [Post("desk/queues")]
    public Task CreateQueuesAsync([Header(ACCESS_TOKEN_HEADER)] string token, string shortName, [Body] CreateQueuesRequest request);

    [Post("desk/rules")]
    public Task CreateRulesAsync([Header(ACCESS_TOKEN_HEADER)] string token, string shortName, [Body] CreateRulesRequest request);
}