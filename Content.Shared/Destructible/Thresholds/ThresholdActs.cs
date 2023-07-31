using Robust.Shared.Serialization;

namespace Content.Shared.Destructible.Thresholds
{
    [Flags, FlagsFor(typeof(ActsFlags))]
    [Serializable]
    public enum ThresholdActs
    {
        None = 0,
        Breakage,
        Destruction
    }
}
