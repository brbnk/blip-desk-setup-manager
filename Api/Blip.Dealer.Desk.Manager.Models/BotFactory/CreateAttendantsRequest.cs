using Blip.Dealer.Desk.Manager.Models.Request;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.BotFactory;

public sealed class CreateAttendantsRequest
{
    [JsonProperty("attendants")]
    public IList<Attendant> Attendants { get; set; }
}

public sealed class Attendant
{
    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("teams")]
    public IList<string> Teams { get; set; }
}