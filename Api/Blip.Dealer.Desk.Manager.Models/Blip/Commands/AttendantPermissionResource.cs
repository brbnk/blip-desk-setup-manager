using Blip.Dealer.Desk.Manager.Models.Blip.Attendance;
using Lime.Protocol;
using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Commands;

public sealed class AttendantPermissionResource(IList<AttendantPermission> permissions) : Document(MediaType)
{
    public const string MIME_TYPE = "application/vnd.lime.collection+json";

    public static readonly MediaType MediaType = MediaType.Parse(MIME_TYPE);

    [JsonProperty("itemType")]
    public string ItemType { get; set; } = "application/vnd.iris.desk.agentpermission+json";

    [JsonProperty("items")]
    public IList<AttendantPermission> Items { get; set; } = permissions; 
}