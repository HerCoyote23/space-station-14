namespace Content.Shared.Spider.Components;

[RegisterComponent]
public sealed partial class SpiderEggComponent : Component
{
    /// <summary>
    ///     Whether the egg spawns regular or rare spiders.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("enriched")]
    public bool Enriched = false;

    /// <summary>
    ///     Time taken to hatch.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("hatchTime")]
    public TimeSpan HatchTime = TimeSpan.FromSeconds(60);
}
/*
[Serializable, NetSerializable]
public sealed partial class GhostRoleTakenEvent : SimpleDoAfterEvent { }*/
