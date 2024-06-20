namespace Blip.Dealer.Desk.Manager.Services;

public interface IBlipClientFactory
{
    public IBlipClient InitBlipClient(string tenantId);
}