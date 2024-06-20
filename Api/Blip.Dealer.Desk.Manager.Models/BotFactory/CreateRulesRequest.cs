using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.BotFactory;

public sealed class CreateRulesRequest
{
    [JsonProperty("rules")]
    public IList<Rule> Rules { get; set; } = [];
}

public sealed class Rule(string team, string dealerId)
{   
    [JsonProperty("team")]
    public string Team { get; set; } = team;

    [JsonProperty("title")]
    public string Title { get; set; } = $"Rule for {team}";

    [JsonProperty("operator")]
    public string Operator { get; set; } = "And";

    [JsonProperty("conditions")]
    public IEnumerable<Conditions> Conditions { get; set; } = [new Conditions(dealerId)];
}

public sealed class Conditions(string dealerId)
{
    [JsonProperty("property")]
    public string Property { get; set; } = "Contact.Extras.dealerId";

    [JsonProperty("relation")]
    public string Relation { get; set; } = "Equals";

    [JsonProperty("values")]
    public IEnumerable<string> Values { get; set; } = [dealerId];
}