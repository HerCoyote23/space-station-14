using Content.Server.Actions;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.Damage;
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

public sealed class SpiderTargetedHealSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _action = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly PopupSystem _popup = default!;

    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly INetManager _net = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SpiderTargetedHealComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<SpiderTargetedHealComponent, SpiderTargetedHealEntityTargetActionEvent>(OnSpiderTargetedHealAction);
        //SubscribeLocalEvent<SpiderTargetedHealComponent, SpiderEggLayDoAfterEvent>(OnDoAfter);
    }

    private void OnComponentInit(EntityUid uid, SpiderTargetedHealComponent component, ComponentInit args)
    {
        if (_net.IsClient)
            return;

        _action.AddAction(uid, Spawn(component.SpiderTargetedHealAction), null);
    }

    private void OnSpiderTargetedHealAction(EntityUid uid, SpiderTargetedHealComponent component, SpiderTargetedHealEntityTargetActionEvent args)
    {
        if (args.Handled || !(HasComp<SpiderComponent>(args.Target)))
            return;

        args.Handled = true;
        var target = args.Target;

        // Heal all bleeding.
        if (TryComp<BloodstreamComponent>(target, out var bloodstream))
        {
            var bleedAmount = bloodstream.BleedAmount;
            if (bleedAmount > 0)
            {
                _bloodstreamSystem.TryModifyBleedAmount(target, -bleedAmount);
                _popup.PopupEntity(Loc.GetString("medical-item-stop-bleeding"), uid, uid);
            }
        }

    }

    public bool TryTargetedHeal(EntityUid uid, SpiderTargetedHealComponent? component)
    {
        if (!Resolve(uid, ref component))
        {
            return false;
        }
        return false;
    }

    private void OnDoAfter(EntityUid uid, SpiderEggLayerComponent component, SpiderEggLayDoAfterEvent args)
    {

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

    //Checks if there's already an egg on the tile
    private bool IsTileBlockedByEgg(EntityCoordinates coords)
    {
        foreach (var entity in coords.GetEntitiesInTile())  //use _lookup
        {
            if (HasComp<SpiderEggComponent>(entity))
                return true;
        }
        return false;
    }
}
