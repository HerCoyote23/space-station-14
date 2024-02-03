using Content.Shared.Actions;
using Content.Shared.Spider.Components;
using Robust.Shared.Containers;
using Robust.Shared.Network;
using Robust.Shared.Random;
using Robust.Shared.Serialization;

namespace Content.Shared.Spider.Systems;

public abstract class SharedSpiderEggLayerSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _action = default!;
    [Dependency] private readonly INetManager _net = default!;
    [Dependency] private readonly IRobustRandom _robustRandom = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] protected readonly SharedContainerSystem ContainerSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpiderEggLayerComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SpiderWebObjectComponent, ComponentStartup>(OnCocoonStartup);
    }

    private void OnComponentInit(EntityUid uid, SpiderEggLayerComponent component, ComponentInit args)
    {
        if (_net.IsClient)
            return;

        _action.AddAction(uid, ref component.Action, component.SpiderEggLayAction);
        _action.AddAction(uid, component.SpiderCocoonAction);
    }
    
    private void OnCocoonStartup(EntityUid uid, SpiderWebObjectComponent component, ComponentStartup args)
    {
        // TODO dont use this. use some general random appearance system
        _appearance.SetData(uid, SpiderWebVisuals.Variant, _robustRandom.Next(1, 3));
    }
}
