using Blip.Dealer.Desk.Manager.Models.Request;

namespace Blip.Dealer.Desk.Manager.Facades.Interfaces;

public interface ICustomRepliesFacade
{
    public Task PublishCustomRepliesAsync(PublishCustomRepliesRequest request);
}