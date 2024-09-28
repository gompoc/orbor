namespace Orbor.Types;

public sealed class LimitType
{
    public ulong Minimum;
    public ulong? Maximum;

    public bool HasMax => Maximum.HasValue;

    public LimitType(ulong minimum, ulong? maximum = null)
    {
        Minimum = minimum;
        Maximum = maximum;
    }
}
