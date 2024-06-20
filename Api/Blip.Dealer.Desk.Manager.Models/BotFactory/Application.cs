using System.Text;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.BotFactory;

public sealed class Application
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("shortName")]
    public string ShortName { get; set; }

    [JsonProperty("accessKey")]
    public string AccessKey { get; set; }

    public string GetAuthorizationKey()
    {
        if (string.IsNullOrWhiteSpace(ShortName) || string.IsNullOrWhiteSpace(AccessKey))
            return null;

        var accessKeyDecoded = Encoding.UTF8.GetString(Convert.FromBase64String(AccessKey));
        var rawAuthorization = $"{ShortName}:{accessKeyDecoded}";

        return $"Key {Convert.ToBase64String(Encoding.UTF8.GetBytes(rawAuthorization))}";
    }
}