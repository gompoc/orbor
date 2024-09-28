using Orbor.Types;
using Orbor.Enums;

namespace Orbor.Imports;

public class TableImport : BaseImport
{
    public readonly TableType TableType;
    public TableImport(string moduleName, string name, TableType tableType) : base(moduleName, name)
    {
        TableType = tableType;
    }

    public override ImportType Type => ImportType.Table;
}
