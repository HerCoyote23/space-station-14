using Content.Server.Actions;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Humanoid;
using Content.Shared.Spider.Components;
using Content.Shared.Spider.Systems;
using Content.Shared.Storage;
using Content.Shared.Maps;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Content.Shared.Humanoid;

namespace Content.Server.Spider.Systems;

public sealed class SpiderEggLayerSystem : SharedSpiderEggLayerSystem
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

        SubscribeLocalEvent<SpiderEggLayerComponent, SpiderCocoonEntityTargetActionEvent>(OnSpiderCocoonAction);
        SubscribeLocalEvent<SpiderEggLayerComponent, SpiderCocoonDoAfterEvent>(OnCocoonDoAfter);
        SubscribeLocalEvent<SpiderEggLayerComponent, SpiderEggLayInstantActionEvent>(OnSpiderEggLayAction);
        SubscribeLocalEvent<SpiderEggLayerComponent, SpiderEggLayDoAfterEvent>(OnEggDoAfter);
    }

    private void OnSpiderCocoonAction(EntityUid uid, SpiderEggLayerComponent component, SpiderCocoonEntityTargetActionEvent args)
    {
        if (args.Handled || !(HasComp<HumanoidAppearanceComponent>(args.Target)))
        {
            args.Handled = true;
            return;
        }

        args.Handled = TryCocoon(uid, component, args.Target);
    }

    public bool TryCocoon(EntityUid uid, SpiderEggLayerComponent? component, EntityUid target)
    {
        if (!Resolve(uid, ref component))
        {
            return false;
        }

        var doAfter = new DoAfterArgs(EntityManager, uid, component.CocoonTime, new SpiderCocoonDoAfterEvent(), uid, target)
        {
            BreakOnDamage = true,
            BreakOnTargetMove = true,
            BreakOnUserMove = true,
            MovementThreshold = 0.2f,
        };

        return _doAfterSystem.TryStartDoAfter(doAfter);
    }

    private void OnCocoonDoAfter(EntityUid uid, SpiderEggLayerComponent component, SpiderCocoonDoAfterEvent args)
    {

        if (args.Cancelled || args.Handled || (args.Target == null))
        {
            return;
        }

        component.Enrichment += 1; // TODO: Make this only give enrichment if the target has not been cocooned once already this death
        EntityUid cocoon = Spawn("SpiderCocoon", Transform((EntityUid) args.Target).Coordinates);

        ContainerSystem.Insert(args.Target, cocoon);

        // Sound + popups
        _audio.PlayPvs(component.EggLaySound, uid);
        _popup.PopupEntity(Loc.GetString("action-popup-lay-egg-user"), uid, uid);
        _popup.PopupEntity(Loc.GetString("action-popup-lay-egg-others", ("entity", uid)), uid, Filter.PvsExcept(uid), true);

        args.Handled = true;
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
            //_popup.PopupEntity(Loc.GetString("spider-egg-action-nogrid"), args.Performer, args.Performer);
            return false;
        }

        var coords = transform.Coordinates;

        if (!IsTileBlockedByEgg(coords))
        {
            var doAfter = new DoAfterArgs(EntityManager, uid, component.EggLayTime, new SpiderEggLayDoAfterEvent(), uid)
            {
                BreakOnDamage = true,
                BreakOnUserMove = true,
                MovementThreshold = 0.2f,
            };

            return (_doAfterSystem.TryStartDoAfter(doAfter));
        }
        else
        {
            _popup.PopupEntity(Loc.GetString("spider-egg-action-tilefull"), uid, uid);
        }

        return false;
    }

    private void OnEggDoAfter(EntityUid uid, SpiderEggLayerComponent component, SpiderEggLayDoAfterEvent args) {

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
        foreach (var entity in _lookup.GetEntitiesIntersecting(coords))//coords.GetEntitiesInTile())  //use _lookup
        {
            if (HasComp<SpiderEggComponent>(entity))
                return true;
        }
        return false;
    }
}
