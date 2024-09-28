using Orbor.Enums;

namespace Orbor.Types;
public readonly struct FunctionSignature
{
    public readonly List<NumberType> Parameters;
    public readonly List<NumberType> Results;
    public FunctionSignature(List<NumberType> parameters, List<NumberType> results)
    {
        Parameters = parameters;
        Results = results;
    }

    public override string ToString()
    {
        return $"(parameters {string.Join(" ", Parameters)}) (result {string.Join(" ", Results)})";
    }
}