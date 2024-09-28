using Orbor.Enums;
namespace Orbor.Types;

public sealed class GlobalType
{
    public readonly NumberType NumberType;
    public readonly MutType MutType;
    public GlobalType(NumberType numberType, MutType mutType)
    {
        NumberType = numberType;
        MutType = mutType;
    }

    public override string ToString()
    {
        return $"NumberType: {NumberType}, MutType: {MutType}";
    }
}
