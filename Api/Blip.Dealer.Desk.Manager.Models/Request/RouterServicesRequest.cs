using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Blip.Dealer.Desk.Manager.Models;
using Blip.Dealer.Desk.Manager.Models.Request;
using Newtonsoft.Json;

namespace Blip.Dealer.Manager.Models.Request;

public sealed class RouterServicesRequest : BotFactoryRequest
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
    [JsonProperty("botId")]
    public string BotId { get; set; }

    [Required]
    [JsonProperty("authKey")]
    public string AuthKey { get; set; }
}