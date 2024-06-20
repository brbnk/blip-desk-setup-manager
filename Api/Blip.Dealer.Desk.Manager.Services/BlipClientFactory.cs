using Lime.Protocol.Serialization.Newtonsoft;
using RestEase;

namespace Blip.Dealer.Desk.Manager.Services;

public class BlipClientFactory(EnvelopeSerializer envelopeSerializer) : IBlipClientFactory
{
    private static readonly Dictionary<string, IBlipClient> _clientPool = [];

    public IBlipClient InitBlipClient(string tenantId)
    {
        if (_clientPool.TryGetValue(tenantId, out var clientFromPool))
        {
            return clientFromPool;
        }

        var url = $"https://{tenantId}.http.msging.net";

        var client = new RestClient(url)
        {
            JsonSerializerSettings = envelopeSerializer.Settings
        }.For<IBlipClient>();

        _clientPool.Add(tenantId, client);

        return client;
    }
}