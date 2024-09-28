using Orbor.Enums;
using Orbor.Imports;

namespace Orbor.Sections;

public sealed class ImportSection : WASMSection
{
    public override SectionType Type => SectionType.Import;

    public List<BaseImport> Imports { get; set; } = [];

    private ImportSection() { }

    public static WASMSection From(BinaryReader reader)
    {
        var importSection = new ImportSection();
        var size = reader.ReadUleb();
        var range = reader.ReadUleb();
        for (ulong i = 0; i < range; i++)
        {
            importSection.Imports.Add(ReadImport(reader));
        }
        return importSection;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.WriteUleb((ulong)Imports.Count);
        foreach (var import in Imports)
        {
            writer.WriteName(import.ModuleName);
            writer.WriteName(import.Name);
            writer.Write((byte)import.Type);
            switch (import)
            {
                case TypeImport typeImport:
                    writer.WriteUleb(typeImport.Index);
                    break;
                case TableImport tableImport:
                    writer.WriteTableType(tableImport.TableType);
                    break;
                case MemoryImport memoryImport:
                    writer.WriteMemoryType(memoryImport.MemoryType);
                    break;
                case GlobalImport globalImport:
                    writer.WriteGlobalType(globalImport.GlobalType);
                    break;
                default:
                    throw new Exception($"Unknown import type: {import.Type}");
            }
        }
    }

    private static BaseImport ReadImport(BinaryReader binaryReader)
    {
        var moduleName = binaryReader.ReadName();
        var name = binaryReader.ReadName();
        var importType = (ImportType)binaryReader.ReadByte();
        switch (importType)
        {
            case ImportType.Type:
                return new TypeImport(moduleName, name, binaryReader.ReadUleb());
            case ImportType.Table:
                return new TableImport(moduleName, name, binaryReader.ReadTableType());
            case ImportType.Memory:
                return new MemoryImport(moduleName, name, binaryReader.ReadMemoryType());
            case ImportType.Global:
                return new GlobalImport(moduleName, name, binaryReader.ReadGlobalType());
            default:
                throw new Exception($"Unknown import type: {importType}");
        }
    }
}
