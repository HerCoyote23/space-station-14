namespace Content.Shared.Destructible.Thresholds
{
    [Serializable]
    [DataDefinition]
    public struct MinMax
    {
        [DataField("min")]
        public int Min;

        [DataField("max")]
        public int Max;
    }
}
