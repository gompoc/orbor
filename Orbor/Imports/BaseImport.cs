using Orbor.Enums;

namespace Orbor.Imports;

public abstract class BaseImport
{
    public readonly string ModuleName;
    public readonly string Name;
    public BaseImport(string moduleName, string name)
    {
        ModuleName = moduleName;
        Name = name;
    }

    public abstract ImportType Type { get; }
}
