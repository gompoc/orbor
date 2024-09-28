using Orbor.Enums;

namespace Orbor.Exports;

public sealed class Export
{
    public ulong Index;
    public string Name;
    public ExportType Type;
    public Export(string name, ulong index, ExportType type)
    {
        Name = name;
        Index = index;
        Type = type;
    }

}