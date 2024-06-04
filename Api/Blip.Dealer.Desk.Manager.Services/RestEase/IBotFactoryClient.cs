using Blip.Dealer.Desk.Manager.Models.BotFactory;
using RestEase;

namespace Blip.Dealer.Desk.Manager.Services.RestEase;

[BasePath("api")]
public interface IBotFactoryClient
{
    [Get("application")]
    public Task<Application> GetApplicationAsync([Header("X-Blip-User-Access-Token")] string bearerToken, string shortName);
}