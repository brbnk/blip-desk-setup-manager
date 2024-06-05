using Blip.Dealer.Desk.Manager.Models;
using Blip.Dealer.Desk.Manager.Models.Request;

namespace Blip.Dealer.Desk.Manager.Facades.Interfaces;

public interface IDeskManagerFacade
{
    public Task<IList<ReportSheet>> PublishDealerSetupAsync(PublishDealerSetupRequest request);
}