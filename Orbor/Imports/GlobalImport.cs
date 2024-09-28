using Orbor.Types;
using Orbor.Enums;

namespace Orbor.Imports;

public sealed class GlobalImport : BaseImport
{
    public readonly GlobalType GlobalType;
    public GlobalImport(string moduleName, string name, GlobalType globalType) : base(moduleName, name)
    {
        GlobalType = globalType;
    }

    public override ImportType Type => ImportType.Global;
}