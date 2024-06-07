using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Attendance;

public class AttendanceQueue
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("description")]
    public string Description { get; set; }
}