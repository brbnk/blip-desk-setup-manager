using Lime.Protocol;
using RestEase;

namespace Blip.Dealer.Desk.Manager.Services;

public interface IBlipClient
{
    [Post("/commands")]
    Task<Command> SendCommandAsync([Header("Authorization")] string authorizationKey, [Body] Command command, CancellationToken cancellationToken);
}