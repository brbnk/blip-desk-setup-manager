using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Request;

public sealed class PublishDealerSetupRequest
{
  [JsonProperty("brand")]
  public string Brand { get; set; }

  [JsonProperty("spreadSheetId")]
  public string SpreadSheetId { get; set; }

  [JsonProperty("sheetName")]
  public string SheetName { get; set; }

  [JsonProperty("range")]
  public string Range { get; set; }

  private string _bearerToken;

  public void SetBearerToken(string token) 
  {
    _bearerToken = $"{token}";
  }

  public string GetBearerToken() => _bearerToken;
}