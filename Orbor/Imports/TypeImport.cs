using Orbor.Enums;

namespace Orbor.Imports;

public class TypeImport : BaseImport
{
    public readonly ulong Index;
    public TypeImport(string moduleName, string name, ulong index) : base(moduleName, name)
    {
        Index = index;
    }

    public override ImportType Type => ImportType.Type;
}
