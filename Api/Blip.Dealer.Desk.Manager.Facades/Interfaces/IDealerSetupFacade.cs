using Blip.Dealer.Desk.Manager.Models;
using Blip.Dealer.Desk.Manager.Models.Request;

namespace Blip.Dealer.Desk.Manager.Facades.Interfaces;

public interface IDealerSetupFacade
{
    public Task PublishDealerSetupAsync(PublishDealerSetupRequest request);
}