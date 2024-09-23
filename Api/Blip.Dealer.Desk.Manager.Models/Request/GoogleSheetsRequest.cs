using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Request; 

public class GoogleSheetsRequest
{
  [Required]
  [DefaultValue(Constants.SPREADSHEET_ID)]
  [JsonProperty("spreadSheetId")]
  public string SpreadSheetId { get; set; }

  [Required]
  [DefaultValue(Constants.SHEET_NAME)]
  [JsonProperty("name")]
  public string Name { get; set; }

  [JsonProperty("range")]
  [DefaultValue("A:Z")]
  public string Range { get; set; } = "A:Z";
}