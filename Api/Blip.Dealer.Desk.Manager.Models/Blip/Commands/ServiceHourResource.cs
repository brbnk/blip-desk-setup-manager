using Blip.Dealer.Desk.Manager.Models.Blip.Attendance;
using Lime.Protocol;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Commands;

public class ServiceHourDocument : Document
{
    public const string MIME_TYPE = "application/vnd.iris.desk.attendance-hour-container+json";

    public static readonly MediaType MediaType = MediaType.Parse(MIME_TYPE);

    public ServiceHourDocument(ServiceHour serviceHour) : base(MediaType)
    {
        AttendanceHour = serviceHour.AttendanceHour;

        Queues = serviceHour.AttendanceHour.IsMain ? [] : serviceHour.Queues;

        AttendanceHourScheduleItems = serviceHour.AttendanceHourScheduleItems;

        AttendanceHourOffItems = serviceHour.AttendanceHourOffItems;
    }

    [JsonProperty("attendanceHour")]
    public AttendanceHour AttendanceHour { get; set; }

    [JsonProperty("queues")]
    public IList<AttendanceQueue> Queues { get; set; }

    [JsonProperty("attendanceHourScheduleItems")]
    public IList<AttendanceHourItem> AttendanceHourScheduleItems { get; set; }

    public IList<AttendanceHourItem> AttendanceHourOffItems { get; set; }
}