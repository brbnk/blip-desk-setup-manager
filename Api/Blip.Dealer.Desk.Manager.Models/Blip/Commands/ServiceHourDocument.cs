using Blip.Dealer.Desk.Manager.Models.Blip.Attendance;
using Lime.Protocol;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Commands;

public class ServiceHourDocument(ServiceHour serviceHour) : Document(MediaType)
{
    public const string MIME_TYPE = "application/vnd.iris.desk.attendance-hour-container+json";

    public static readonly MediaType MediaType = MediaType.Parse(MIME_TYPE);

    [JsonProperty("attendanceHour")]
    public AttendanceHour AttendanceHour { get; set; } = serviceHour.AttendanceHour;

    [JsonProperty("queues")]
    public IList<AttendanceQueue> Queues { get; set; } = serviceHour.AttendanceHour.IsMain ? [] : serviceHour.Queues;

    [JsonProperty("attendanceHourScheduleItems")]
    public IList<AttendanceHourItem> AttendanceHourScheduleItems { get; set; } = serviceHour.AttendanceHourScheduleItems;

    [JsonProperty("attendanceHourOffItems")]
    public IList<AttendanceHourItem> AttendanceHourOffItems { get; set; } = serviceHour.AttendanceHourOffItems;
}