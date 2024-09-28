using System.Collections.Generic;

namespace Orbor.Operands;

public sealed class IndexVectorOperand : IOperand
{
    public readonly List<ulong> Indices;
    public IndexVectorOperand()
    {
        Indices = new List<ulong>();
    }

    public override string ToString()
    {
        return $"[{string.Join(",", Indices)}]";
    }

    public void Add(ulong index) => Indices.Add(index);
}