using Blip.Dealer.Desk.Manager.Models.Request;

namespace Blip.Dealer.Desk.Manager.Facades.Interfaces;

public interface IFlowFacade
{
    public Task PublishFlowAsync(PublishFlowRequest request, Stream file);
}