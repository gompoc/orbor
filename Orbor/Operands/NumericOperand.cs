namespace Orbor.Operands;

public abstract class NumericOperand : IOperand
{
    public readonly int Width;
    public readonly IConvertible Value;

    public NumericOperand(int width, IConvertible value)
    {
        Width = width;
        Value = value;
    }

    public override string ToString()
    {
        return Value.ToString() ?? "";
    }
}