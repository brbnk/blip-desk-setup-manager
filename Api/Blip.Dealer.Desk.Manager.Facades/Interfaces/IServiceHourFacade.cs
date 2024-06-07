using Blip.Dealer.Desk.Manager.Models.Request;

namespace Blip.Dealer.Desk.Manager.Facades.Interfaces;

public interface  IServiceHourFacade
{
    public Task PublishDealersServiceHoursAsync(PublishServiceHoursRequest request); 
}