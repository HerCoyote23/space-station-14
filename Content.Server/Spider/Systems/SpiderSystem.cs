using System.Linq;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.DoAfter;
using Content.Shared.Spider.Components;
using Content.Shared.Spider.Systems;
using Content.Shared.Maps;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Map;
using Robust.Shared.Network;

namespace Content.Server.Spider.Systems;

public sealed class SpiderSystem : SharedSpiderSystem
{
    [Dependency] private readonly DoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SpiderComponent, SpiderWebInstantActionEvent>(OnSpiderWebPlaceAction);
        SubscribeLocalEvent<SpiderComponent, SpiderWebPlaceDoAfterEvent>(OnDoAfter);
    }

    private void OnSpiderWebPlaceAction(EntityUid uid, SpiderComponent component, SpiderWebInstantActionEvent args)
    {
        args.Handled = TryPlaceWeb(uid, component);
    }

    public bool TryPlaceWeb(EntityUid uid, SpiderComponent? component)
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

        if (!IsTileBlockedByWeb(coords))
        {
            var doAfter = new DoAfterArgs(EntityManager, uid, component.WebPlaceTime, new SpiderWebPlaceDoAfterEvent(), uid)
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

    private void OnDoAfter(EntityUid uid, SpiderComponent component, SpiderWebPlaceDoAfterEvent args)
    {

        if (args.Cancelled || args.Handled)
        {
            return;
        }

        Spawn(component.WebPrototype, Transform(uid).Coordinates);

        args.Handled = true;
    }

    //Checks if there's already a web on the tile
    private bool IsTileBlockedByWeb(EntityCoordinates coords)
    {
        foreach (var entity in _lookup.GetEntitiesIntersecting(coords))//coords.GetEntitiesInTile())  //use _lookup
        {
            if (HasComp<SpiderWebObjectComponent>(entity))
                return true;
        }
        return false;
    }
}



/*
    private void OnSpawnNet(EntityUid uid, SpiderComponent component, SpiderWebActionEvent args)
    {
        if (args.Handled)
            return;

        var transform = Transform(uid);

        if (transform.GridUid == null)
        {
            _popup.PopupEntity(Loc.GetString("spider-web-action-nogrid"), args.Performer, args.Performer);
            return;
        }

        var coords = transform.Coordinates;

        // TODO generic way to get certain coordinates

        var result = false;
        // Spawn web in center
        if (!IsTileBlockedByWeb(coords))
        {
            Spawn(component.WebPrototype, coords);
            result = true;
        }

        // Spawn web in other directions
        for (var i = 0; i < 4; i++)
        {
            var direction = (DirectionFlag) (1 << i);
            coords = transform.Coordinates.Offset(direction.AsDir().ToVec());

            if (!IsTileBlockedByWeb(coords))
            {
                Spawn(component.WebPrototype, coords);
                result = true;
            }
        }

        if (result)
        {
            _popup.PopupEntity(Loc.GetString("spider-web-action-success"), args.Performer, args.Performer);
            args.Handled = true;
        }
        else
            _popup.PopupEntity(Loc.GetString("spider-web-action-fail"), args.Performer, args.Performer);
    }

    private bool IsTileBlockedByWeb(EntityCoordinates coords)
    {
        foreach (var entity in coords.GetEntitiesInTile())
        {
            if (HasComp<SpiderWebObjectComponent>(entity))
                return true;
        }
        return false;
    }
    */


