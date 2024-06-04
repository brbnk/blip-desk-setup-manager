using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Request;

public sealed class PublishDealerSetupRequest
{
  [JsonProperty("spreadSheetId")]
  public string SpreadSheetId { get; set; }

  [JsonProperty("sheetName")]
  public string SheetName { get; set; }

  [JsonProperty("range")]
  public string Range { get; set; }
}