using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Request;

public sealed class PublishFlowRequest : BotFactoryRequest
{
  [Required]
  [JsonProperty("tenant")]
  public string Tenant { get; set; }

  [Required]
  [JsonProperty("brand")]
  public string Brand { get; set; }

  [JsonProperty("dataSource")]
  public GoogleSheetsRequest DataSource { get; set; }
}