using Blip.Dealer.Desk.Manager.Models;
using Blip.Dealer.Manager.Models.Request;

namespace Blip.Dealer.Desk.Manager.Facades.Interfaces;

public interface IRouterFacade 
{
    public Task<IEnumerable<RouterChild>> GetRouterServicesAsync(RouterServicesRequest request);
}