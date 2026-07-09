public readonly struct EnemyAddress
{
    public static readonly EnemyAddress LaneJumper = new("Enemies/LaneJumper");

    public EnemyAddress(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
