using Content.Shared.Destructible.Thresholds;

namespace Content.Shared.Destructible
{
    /// <summary>
    ///     When attached to an <see cref="Robust.Shared.GameObjects.EntityUid"/>, allows it to take damage
    ///     and triggers thresholds when reached.
    /// </summary>
    [RegisterComponent]
    public sealed class DestructibleComponent : Component
    {
        [DataField("thresholds")]
        public List<DamageThreshold> Thresholds = new();

    }
}
