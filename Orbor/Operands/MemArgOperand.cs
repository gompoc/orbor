using System.Collections.Generic;

namespace Orbor.Operands;

public sealed class MemArgOperand : IOperand
{
    public readonly ulong Align;
    public readonly ulong Offset;
    public MemArgOperand(ulong align, ulong offset)
    {
        Align = align;
        Offset = offset;
    }

    public override string ToString()
    {
        List<string> list = new List<string>();
            list.Add($"offset={Offset}");
            list.Add($"align={Align}");
        return string.Join(" ", list);
    }
}