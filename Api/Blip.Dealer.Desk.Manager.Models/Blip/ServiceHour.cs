using Blip.Dealer.Desk.Manager.Models.Blip.Attendance;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip;

public class ServiceHour
{
    [JsonProperty("attendanceHour")]
    public AttendanceHour AttendanceHour { get; set; }

    [JsonProperty("queues")]
    public IList<AttendanceQueue> Queues { get; set; }

    [JsonProperty("attendanceHourScheduleItems")]
    public IList<AttendanceHourItem> AttendanceHourScheduleItems { get; set; }

    [JsonProperty("attendanceHourOffItems")]
    public IList<AttendanceHourItem> AttendanceHourOffItems { get; set; }
}