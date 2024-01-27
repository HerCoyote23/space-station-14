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

        /*SubscribeLocalEvent<SpiderEggComponent, GhostRoleTakenEvent(OnGhostTake);
        SubscribeLocalEvent<SpiderEggComponent, SpiderEggLayInstantActionEvent>(OnSpiderEggLayAction);
        SubscribeLocalEvent<SpiderEggComponent, SpiderEggLayDoAfterEvent>(OnDoAfter);*/
    }

    private void OnComponentInit(EntityUid uid, SpiderEggComponent component, ComponentInit args)
    {
        if (_net.IsClient)
            return;
    }
}

