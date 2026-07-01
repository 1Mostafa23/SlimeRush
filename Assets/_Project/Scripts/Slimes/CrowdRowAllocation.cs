public readonly struct CrowdRowAllocation
{
    public CrowdRowAllocation(int count, int baseWidth, int layerCount)
    {
        Count = count;
        BaseWidth = baseWidth;
        LayerCount = layerCount;
    }

    public int Count { get; }
    public int BaseWidth { get; }
    public int LayerCount { get; }
}
