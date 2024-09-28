using Orbor.Types;

namespace Orbor;

public sealed class Global
{
    public GlobalType GlobalType;
    public List<Instruction> InitExpression;
    public Global(GlobalType globalType, List<Instruction> instructions)
    {
        GlobalType = globalType;
        InitExpression = instructions;
    }
}
