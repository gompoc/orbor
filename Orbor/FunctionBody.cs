namespace Orbor;

public sealed class FunctionBody
{
    public List<Local> Locals { get; set; } = new();
    public List<Instruction> Instructions { get; set; } = new();

    public FunctionBody(List<Local> locals, List<Instruction> instructions)
    {
        Locals = locals;
        Instructions = instructions;
    }

}
