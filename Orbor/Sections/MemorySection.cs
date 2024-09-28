using Orbor.Enums;
using Orbor.Types;
using System.IO;

namespace Orbor.Sections;

public sealed class MemorySection : WASMSection
{
    public override SectionType Type => SectionType.Memory;

    public List<MemoryType> MemoryTypes { get; set; } = new();

    public static MemorySection From(BinaryReader reader)
    {
        var memorySection = new MemorySection();
        var size = reader.ReadUleb();
        var range = reader.ReadUleb();
        for (ulong i = 0; i < range; i++)
        {
            memorySection.MemoryTypes.Add(reader.ReadMemoryType());
        }
        return memorySection;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.WriteUleb((ulong)MemoryTypes.Count);
        foreach (var memoryType in MemoryTypes)
        {
            writer.WriteMemoryType(memoryType);
        }
    }
}
