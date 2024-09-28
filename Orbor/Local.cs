using Orbor.Enums;

namespace Orbor;

public sealed class Local
{
    public readonly NumberType NumberType;
    public Local(NumberType numberType)
    {
        NumberType = numberType;
    }
}
