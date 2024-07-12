using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Blip.Dealer.Desk.Manager.Models.Request;
using Newtonsoft.Json;

namespace Blip.Dealer.Manager.Models.Request;

public sealed class RouterServicesRequest : BotFactoryRequest
{
    [Required]
    [JsonProperty("tenant")]
    public string Tenant { get; set; }

    [Required]
    [JsonProperty("brand")]
    public string Brand { get; set; }

    [JsonProperty("dataSource")]
    public GoogleSheetsRequest DataSource { get; set; }

    [Required]
    [JsonProperty("routerId")]
    public string BotId { get; set; }

    [Required]
    [JsonProperty("authKey")]
    public string AuthKey { get; set; }
}