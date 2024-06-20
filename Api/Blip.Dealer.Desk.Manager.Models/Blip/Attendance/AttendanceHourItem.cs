using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Attendance;

public class AttendanceHourItem
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("startTime")]
    public string StartTime { get; set; }

    [JsonProperty("endTime")]
    public string EndTime { get; set; }

    [JsonProperty("dayOfWeek")]
    public string DayOfWeek { get; set; }

    [JsonProperty("danger")]
    public bool Danger { get; set; } = false;

    [JsonProperty("errorMessage")]
    public string ErrorMessage { get; set; } = string.Empty;

    [JsonProperty("scheduleId")]
    public int ScheduledId { get; set; } = 1;
}