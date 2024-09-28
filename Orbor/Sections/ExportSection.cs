using Orbor.Enums;
using Orbor.Exports;
using System.IO;

namespace Orbor.Sections;

public sealed class ExportSection : WASMSection
{
    public override SectionType Type => SectionType.Export;

    public List<Export> Exports { get; set; } = [];


    private ExportSection() { }

    public static ExportSection From(BinaryReader reader)
    {
        var exportSection = new ExportSection();
        var size = reader.ReadUleb();
        var range = reader.ReadUleb();
        for (ulong i = 0; i < range; i++)
        {
            exportSection.Exports.Add(reader.ReadExport());
        }
        return exportSection;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.WriteUleb((ulong)Exports.Count);
        foreach (var export in Exports)
        {
            writer.WriteExport(export);
        }
    }
}
