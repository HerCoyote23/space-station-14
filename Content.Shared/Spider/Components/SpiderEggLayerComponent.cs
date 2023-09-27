using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Storage;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Spider.Components;

/// <summary>
///     This component is an adaptation of EggLayerComponent, it handles spider egglaying, which doesn't require hunger, and can use a special resource.
/// </summary>
[RegisterComponent]
public sealed partial class SpiderEggLayerComponent : Component
{
    [DataField("eggLayAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SpiderEggLayAction = "ActionSpiderLayEgg";

    /// <summary>
    ///     Time taken to lay an egg
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("eggLayTime")]
    public TimeSpan EggLayTime = TimeSpan.FromSeconds(10);

    /// <summary>
    ///     Special points for laying Enriched Eggs
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("enrichment")]
    public int Enrichment = 0;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("eggSpawn")]
    public string EggSpawn = "SpiderEgg";

    [DataField("eggLaySound")]
    public SoundSpecifier EggLaySound = new SoundPathSpecifier("/Audio/Effects/pop.ogg");
}

public sealed partial class SpiderEggLayInstantActionEvent : InstantActionEvent {}


[Serializable, NetSerializable]
public sealed partial class SpiderEggLayDoAfterEvent : SimpleDoAfterEvent {}

