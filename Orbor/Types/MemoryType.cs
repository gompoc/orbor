namespace Orbor.Types;

public sealed class MemoryType
{
    public readonly LimitType LimitType;
    public MemoryType(LimitType limitType)
    {
        LimitType = limitType;
    }
}