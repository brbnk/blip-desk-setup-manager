using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.BotFactory;

public sealed class ApplicationResult
{
    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("results")]
    public IEnumerable<Application> Results { get; set; }
}

public sealed class Application
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("shortName")]
    public string ShortName { get; set; }

    [JsonProperty("accessKey")]
    public string AccessKey { get; set; }
}