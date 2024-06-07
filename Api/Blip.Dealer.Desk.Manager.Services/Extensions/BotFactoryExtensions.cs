using System.Net.Http.Headers;
using Blip.Dealer.Desk.Manager.Services.RestEase;
using RestEase;
using RestEase.Implementation;

namespace Blip.Dealer.Desk.Manager.Services.Extensions;

public static class BotFactoryExtensions
{
    public static Task PublishFlowAsync(this IBotFactoryClient client, Stream stream, string token, string shortName)
    {
        var file = new MultipartFormDataContent();

        using var ms = new MemoryStream();

        stream.CopyTo(ms);

        var content = new ByteArrayContent(ms.ToArray());

        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        file.Add(content, "file", "flow.json");

        var requestInfo = new RequestInfo(HttpMethod.Post, "api/flow/publish")
        {
            BaseAddress = "https://mcmh01bt-55598.brs.devtunnels.ms"
        };

        requestInfo.SetBodyParameterInfo(BodySerializationMethod.Serialized, file);

        requestInfo.AddHeaderParameter("X-Blip-User-Access-Token", token);
        requestInfo.AddQueryParameter(QuerySerializationMethod.ToString, "shortName", shortName);

        return client.Requester.RequestVoidAsync(requestInfo);
    }
}