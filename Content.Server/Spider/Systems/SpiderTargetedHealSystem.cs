using Content.Server.Actions;
using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.DoAfter;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.FixedPoint;
using Content.Shared.Mobs.Components;
using Content.Shared.Spider.Components;
using Content.Shared.Mobs;
using Content.Shared.Spider.Systems;
using Content.Shared.Storage;
using Content.Shared.Maps;
using Robust.Server.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Network.Messages;

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
        SubscribeLocalEvent<SpiderTargetedHealComponent, SpiderTargetedHealDoAfterEvent>(OnDoAfter);
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

        args.Handled = TryTargetedHeal(uid, component, args.Target);
    }

    public bool TryTargetedHeal(EntityUid uid, SpiderTargetedHealComponent? component, EntityUid target)
    {
        if (!Resolve(uid, ref component))
        {
            return false;
        }

        var doAfter = new DoAfterArgs(EntityManager, uid, component.HealTime, new SpiderTargetedHealDoAfterEvent(), null, target: target)
        {
            BreakOnDamage = true,
            BreakOnTargetMove = true,
            BreakOnUserMove = true,
            MovementThreshold = 0.2f,
        };

        return _doAfterSystem.TryStartDoAfter(doAfter);
    }

    private void OnDoAfter(EntityUid uid, SpiderTargetedHealComponent component, SpiderTargetedHealDoAfterEvent args)
    {
        
        if (args.Cancelled || args.Handled)
        {
            return;
        }

        var target = args.Target;

        if (target == null)
        {
            return;
        }

        if (target == uid)
        {
            _popup.PopupEntity(Loc.GetString("You cannot heal yourself"), uid, uid);
            return;
        }

        // Heal all bleeding, and restore some blood.
        if (TryComp<BloodstreamComponent>(target, out var bloodstream))
        {
            var bleedAmount = bloodstream.BleedAmount;

            if (bleedAmount > 0)
            {
                _bloodstreamSystem.TryModifyBleedAmount((EntityUid) target, -bleedAmount);
            }

            _bloodstreamSystem.TryModifyBloodLevel((EntityUid) target, component.BloodHealing);
        }

        // I meant to make this heal % max health, but that was very difficult. Healing a % of current damage + a flat damage achieves a similar effect.
        if (TryComp<DamageableComponent>(target, out var damage))
        {
            DamageSpecifier finalHealing = new(damage.Damage);

            int count = 0;

            foreach (var val in finalHealing.DamageDict.Values)
            {
                if (val > 0)
                {
                    count++;
                }
            }

            var totalHealing = (damage.TotalDamage * (component.PercentHealing / 100)) + component.FlatHealing;
            var categoryHealing = totalHealing / count;

            foreach (var (key, value) in finalHealing.DamageDict)
            {
                finalHealing.DamageDict[key] = -categoryHealing;
            }

            _damageable.TryChangeDamage(target, finalHealing, true, origin: uid);
            _popup.PopupEntity(Loc.GetString("Damage Healed"), uid, uid);
        }

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
