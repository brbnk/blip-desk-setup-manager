using Blip.Dealer.Desk.Manager.Models.Google;
using Blip.Dealer.Desk.Manager.Models.Request;

namespace Blip.Dealer.Desk.Manager.Facades.Interfaces;

public interface IDeskManagerFacade
{
    public Task<IEnumerable<DealerSetupSheet>> ReadGoogleSheetAsync(PublishDealerSetupRequest request);
}