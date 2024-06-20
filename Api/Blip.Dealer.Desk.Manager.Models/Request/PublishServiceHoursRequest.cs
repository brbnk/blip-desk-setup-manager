using System.ComponentModel.DataAnnotations;
using Blip.Dealer.Desk.Manager.Models.Blip.Attendance;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Request;

public class PublishServiceHoursRequest : BotFactoryRequest
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
  [JsonProperty("workingHours")]
  public IList<AttendanceHourItem> WorkingHours { get; set; }
}