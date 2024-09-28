namespace Orbor.Operands;

public sealed class I64Operand : NumericOperand
{
    public I64Operand(long value) : base(8, value)
    {
    }
}