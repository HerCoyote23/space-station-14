using Content.Shared.Actions;
using Content.Shared.Spider.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Spider.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(SharedSpiderSystem))]
public sealed partial class SpiderComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("webPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string WebPrototype = "SpiderWeb";

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("webMakingTime")]
    public TimeSpan WebMakingTime = TimeSpan.FromSeconds(10);

    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("webAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string WebAction = "ActionSpiderWeb";

<<<<<<< HEAD:Content.Shared/Spider/Components/SpiderComponent.cs

=======
    [DataField] public EntityUid? Action;
>>>>>>> 98420b5735fe288284ee39f2aff11a28d3550f0b:Content.Shared/Spider/SpiderComponent.cs
}

public sealed partial class SpiderWebActionEvent : InstantActionEvent { }
