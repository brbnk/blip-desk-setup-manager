using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Request;

public sealed class PublishFlowRequest : BotFactoryRequest
{
  [Required]
  [DefaultValue(Constants.TENANT_ID)]
  [JsonProperty("tenant")]
  public string Tenant { get; set; }

  [Required]
  [DefaultValue(Constants.BRAND)]
  [JsonProperty("brand")]
  public string Brand { get; set; }

  [JsonProperty("dataSource")]
  public GoogleSheetsRequest DataSource { get; set; }

  [Required]
  [JsonProperty("flowStr")]
  public string FlowStr { get; set; }
}