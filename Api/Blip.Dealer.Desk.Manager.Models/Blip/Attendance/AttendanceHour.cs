using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Attendance;

public class AttendanceHour
{
    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("isMain")]
    public bool IsMain { get; set; }
}