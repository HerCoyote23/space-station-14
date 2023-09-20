using Content.Shared.DoAfter;
using Content.Shared.Storage;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Animals.Components;

/// <summary>
///     This component handles animals which lay eggs (or some other item) on a timer, using up hunger to do so.
///     It also grants an action to players who are controlling these entities, allowing them to do it manually.
/// </summary>
[RegisterComponent]
public sealed partial class EggLayerComponent : Component
{
    [DataField("eggLayAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string EggLayAction = "ActionAnimalLayEgg";

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("hungerUsage")]
    public float HungerUsage = 60f;

    /// <summary>
    ///     Time taken to lay an egg
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("eggLayTime")]
    public TimeSpan EggLayTime = TimeSpan.FromSeconds(1);

    /// <summary>
    ///     Minimum cooldown used for the automatic egg laying.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("eggLayCooldownMin")]
    public float EggLayCooldownMin = 60f;

    /// <summary>
    ///     Maximum cooldown used for the automatic egg laying.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("eggLayCooldownMax")]
    public float EggLayCooldownMax = 120f;

    /// <summary>
    ///     Set during component init.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float CurrentEggLayCooldown;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("eggSpawn", required: true)]
    public List<EntitySpawnEntry> EggSpawn = default!;

    [DataField("eggLaySound")]
    public SoundSpecifier EggLaySound = new SoundPathSpecifier("/Audio/Effects/pop.ogg");

    [DataField("accumulatedFrametime")]
    public float AccumulatedFrametime;
}

//public sealed partial class EggLayInstantActionEvent : InstantActionEvent {}

//[Serializable, NetSerializable]
public sealed partial class EggLayDoAfterEvent : SimpleDoAfterEvent {}

