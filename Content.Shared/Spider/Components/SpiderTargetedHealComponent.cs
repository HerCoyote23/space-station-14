using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Storage;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Spider.Components;

/// <summary>
///     This component adds the action to heal OTHER spiders, not yourself.
/// </summary>
[RegisterComponent]
public sealed partial class SpiderTargetedHealComponent : Component
{
    [DataField("targetedHealAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SpiderTargetedHealAction = "ActionSpiderTargetedHeal";

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("percentHealing")]
    public double PercentHealing = 20;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("flatHealing")]
    public int FlatHealing = 10;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("bloodHealing")]
    public int BloodHealing = 25;

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("healTime")]
    public int HealTime = 10;

    [DataField("healingSound")]
    public SoundSpecifier HealingSound = new SoundPathSpecifier("/Audio/Effects/pop.ogg");
}

public sealed partial class SpiderTargetedHealEntityTargetActionEvent : EntityTargetActionEvent { }

[Serializable, NetSerializable]
public sealed partial class SpiderTargetedHealDoAfterEvent : SimpleDoAfterEvent { }

