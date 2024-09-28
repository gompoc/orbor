using Orbor.Enums;
using Orbor.Types;
namespace Orbor.Imports;

public class MemoryImport : BaseImport
{
    public readonly MemoryType MemoryType;
    public MemoryImport(string moduleName, string name, MemoryType memoryType) : base(moduleName, name)
    {
        MemoryType = memoryType;
    }

    public override ImportType Type => ImportType.Memory;
}