namespace Orbor.Operands;

public sealed class IndexOperand : IOperand
{
    public readonly ulong Index;
    public IndexOperand(ulong index)
    {
        Index = index;
    }

    public override string ToString()
    {
        return Index.ToString();
    }
}