using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Request;

public sealed class PublishDealerSetupRequest
{
  [Required]
  [JsonProperty("tenant")]
  public string Tenant { get; set; }

  [Required]
  [JsonProperty("brand")]
  public string Brand { get; set; }

  [Required]
  [JsonProperty("spreadSheetId")]
  public string SpreadSheetId { get; set; }

  [Required]
  [JsonProperty("sheetName")]
  public string SheetName { get; set; }

  [JsonProperty("range")]
  public string Range { get; set; } = "A:Z";

  [JsonProperty("imageUrl")]
  public string ImageUrl { get; set; }

  private string _bearerToken;

  public void SetBearerToken(string token) 
  {
    _bearerToken = $"{token}";
  }

  public string GetBearerToken() => _bearerToken;
}