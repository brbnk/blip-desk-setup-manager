using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Request; 

public class GoogleSheetsRequest
{
  [Required]
  [JsonProperty("spreadSheetId")]
  public string SpreadSheetId { get; set; }

  [Required]
  [JsonProperty("name")]
  public string Name { get; set; }

  [JsonProperty("range")]
  public string Range { get; set; } = "A:Z";
}