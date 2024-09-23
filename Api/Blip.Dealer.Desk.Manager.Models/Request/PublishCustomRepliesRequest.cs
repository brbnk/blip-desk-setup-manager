using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Blip.Dealer.Desk.Manager.Models.Blip.Replies;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Request;

public sealed class PublishCustomRepliesRequest : BotFactoryRequest
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
  [JsonProperty("items")]
  public IList<Item> Items { get; set; }
}