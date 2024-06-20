namespace Blip.Dealer.Desk.Manager.Models.Google;

using Newtonsoft.Json;

public record DealerSetupSheet : GoogleSheet
{
  [JsonProperty("A")]
  public string Brand { get; set; }

  [JsonProperty("I")]
  public string Group { get; set; }

  [JsonProperty("K")]
  public string Code { get; set; }

  [JsonProperty("L")]
  public string FantasyName { get; set; }

  [JsonProperty("Z")]
  public string Attendants { get; set; }
}