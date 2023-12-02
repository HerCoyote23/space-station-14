using Content.Server.Ghost.Roles;

namespace Content.Server.Ghost.Roles.Components;

/// <summary>
/// This is used for a ghost role which has multiple options for what to spawn. Example use is spider eggs.
/// </summary>
[RegisterComponent, Access(typeof(GhostRoleSystem))]
public sealed partial class GhostRoleSubmenuComponent : Component
{

    /// <summary>
    /// Options to choose from the submenu.
    /// </summary>
    [DataField("submenuContents")]
    public List<String> Options = new();


}
