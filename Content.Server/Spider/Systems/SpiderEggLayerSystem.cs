using Content.Server.Actions;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Spider.Components;
using Content.Shared.Spider.Systems;
using Content.Shared.Storage;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Network;

using Content.Shared.Animals.Components;

namespace Content.Server.Spider.Systems;

public sealed class SpiderEggLayerSystem : EntitySystem
{
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly SharedActionsSystem _action = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly PopupSystem _popup = default!;

    [Dependency] private readonly INetManager _net = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpiderEggLayerComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SpiderEggLayerComponent, SpiderEggLayInstantActionEvent>(OnSpiderEggLayAction);
        SubscribeLocalEvent<SpiderEggLayerComponent, SpiderEggLayDoAfterEvent>(OnDoAfter);
    }

    private void OnComponentInit(EntityUid uid, SpiderEggLayerComponent component, ComponentInit args)
    {
        if (_net.IsClient)
            return;

        _action.AddAction(uid, Spawn(component.SpiderEggLayAction), null);
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

        var doAfter = new DoAfterArgs(EntityManager, uid, component.EggLayTime, new SpiderEggLayDoAfterEvent(), uid)
        {
            BreakOnDamage = true,
            BreakOnUserMove = true,
            MovementThreshold = 0.1f,
        };

        return (_doAfterSystem.TryStartDoAfter(doAfter));

    }

    private void OnDoAfter(EntityUid uid, SpiderEggLayerComponent component, SpiderEggLayDoAfterEvent args) {

        if (args.Cancelled || args.Handled)
        {
            return;
        }

        Spawn(component.EggSpawn, Transform(uid).Coordinates);

        // Sound + popups
        _audio.PlayPvs(component.EggLaySound, uid);
        _popup.PopupEntity(Loc.GetString("action-popup-lay-egg-user"), uid, uid);
        _popup.PopupEntity(Loc.GetString("action-popup-lay-egg-others", ("entity", uid)), uid, Filter.PvsExcept(uid), true);

        args.Handled = true;
    }
}
