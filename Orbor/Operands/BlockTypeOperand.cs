using Orbor.Enums;

namespace Orbor.Operands;

public sealed class BlockTypeOperand : IOperand
{
    public readonly NumberType? ValueTypeEnum;

    public BlockTypeOperand(NumberType? valueTypeEnum = null)
    {
        ValueTypeEnum = valueTypeEnum;
    }

    public override string ToString()
    {
        if (ValueTypeEnum is not null)
            return $"(result {ValueTypeEnum})";
        return "";
    }
}