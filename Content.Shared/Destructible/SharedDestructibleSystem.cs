namespace Content.Shared.Destructible;

using Content.Shared.FixedPoint;
using Content.Shared.Damage.Prototypes;
using Content.Shared.Destructible.Thresholds.Behaviors;
using Content.Shared.Destructible.Thresholds.Triggers;

public abstract class SharedDestructibleSystem : EntitySystem
{
    /// <summary>
    ///     Force entity to be destroyed and deleted.
    /// </summary>
    public void DestroyEntity(EntityUid owner)
    {
        var eventArgs = new DestructionEventArgs();

        RaiseLocalEvent(owner, eventArgs);
        QueueDel(owner);
    }

    /// <summary>
    ///     Force entity to broke.
    /// </summary>
    public void BreakEntity(EntityUid owner)
    {
        var eventArgs = new BreakageEventArgs();
        RaiseLocalEvent(owner, eventArgs);
    }

    public FixedPoint2 DestroyedAt(EntityUid uid, DestructibleComponent? destructible = null)
    {
        if (!Resolve(uid, ref destructible, logMissing: false))
            return FixedPoint2.MaxValue;

        // We have nested for loops here, but the vast majority of components only have one threshold with 1-3 behaviors.
        // Really, this should probably just be a property of the damageable component.
        var damageNeeded = FixedPoint2.MaxValue;
        foreach (var threshold in destructible.Thresholds)
        {
            if (threshold.Trigger is not DamageTrigger trigger)
                continue;

            foreach (var behavior in threshold.Behaviors)
            {
                if (behavior is DoActsBehavior actBehavior &&
                    actBehavior.HasAct(ThresholdActs.Destruction | ThresholdActs.Breakage))
                {
                    damageNeeded = Math.Min(damageNeeded.Float(), trigger.Damage);
                }
            }
        }
        return damageNeeded;
    }
}

/// <summary>
///     Raised when entity is destroyed and about to be deleted.
/// </summary>
public sealed class DestructionEventArgs : EntityEventArgs
{

}

/// <summary>
///     Raised when entity was heavy damage and about to break.
/// </summary>
public sealed class BreakageEventArgs : EntityEventArgs
{

}
