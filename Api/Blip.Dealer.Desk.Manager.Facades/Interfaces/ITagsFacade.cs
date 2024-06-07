using Blip.Dealer.Desk.Manager.Models.Request;

namespace Blip.Dealer.Desk.Manager.Facades;

public interface ITagsFacade
{
    public Task PublishTagsAsync(PublishTagsRequest request);
}