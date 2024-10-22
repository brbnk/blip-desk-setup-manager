using Blip.Dealer.Desk.Manager.Models.Request;

namespace Blip.Dealer.Desk.Manager.Facades.Interfaces;

public interface IAttendantsFacade
{
    public Task PublishAttendantsAsync(PublishAttendantsRequest request);

    public Task CheckAttendantsAsync(PublishAttendantsRequest request);
}