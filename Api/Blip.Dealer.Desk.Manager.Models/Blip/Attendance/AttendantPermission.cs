using Newtonsoft.Json;

namespace Blip.Dealer.Desk.Manager.Models.Blip.Attendance;

public sealed class AttendantPermissionRequest
{
    public string Email { get; set; }

    public IList<AttendantPermission> Permissions { get; set; }
}

public sealed class AttendantPermission
{
    [JsonProperty("ownerIdentity")]
    public string OwnerIdentity { get; set; }

    [JsonProperty("agentIdentity")]
    public string AgentIdentity { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("isActive")]
    public bool IsActive { get; set; }
}