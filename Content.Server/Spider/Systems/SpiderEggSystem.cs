using Content.Server.Actions;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Spider.Components;
using Content.Shared.Spider.Systems;
using Content.Shared.Storage;
using Content.Shared.Maps;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Map;
using Robust.Shared.Network;

namespace Content.Server.Spider.Systems;

public sealed class SpiderEggSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _action = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly INetManager _net = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpiderEggComponent, ComponentInit>(OnComponentInit);/*
        SubscribeLocalEvent<SpiderEggComponent, SpiderEggLayInstantActionEvent>(OnSpiderEggLayAction);
        SubscribeLocalEvent<SpiderEggComponent, SpiderEggLayDoAfterEvent>(OnDoAfter);*/
    }

    private void OnComponentInit(EntityUid uid, SpiderEggComponent component, ComponentInit args)
    {
        if (_net.IsClient)
            return;
    }

    private void OnSpiderEggLayAction(EntityUid uid, SpiderEggLayerComponent component, SpiderEggLayInstantActionEvent args)
    {
        args.Handled = TryLayEgg(uid, component);
    }

    public bool TryLayEgg(EntityUid uid, SpiderEggLayerComponent? component)
    {
        if (!Resolve(uid, ref component))
        {
            return false;
        }

        var transform = Transform(uid);

        if (transform.GridUid == null)
        {
            //_popup.PopupEntity(Loc.GetString("spider-web-action-nogrid"), args.Performer, args.Performer);
            return false;
        }

        var coords = transform.Coordinates;

        return false;
    }

    private void OnDoAfter(EntityUid uid, SpiderEggLayerComponent component, SpiderEggLayDoAfterEvent args) {

        if (args.Cancelled || args.Handled)
        {
            return;
        }

        Spawn(component.EggSpawn, Transform(uid).Coordinates);

        args.Handled = true;
    }
}

