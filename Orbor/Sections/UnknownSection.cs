using Orbor.Enums;
using System.Drawing;
namespace Orbor.Sections;

public sealed class UnknownSection : WASMSection
{
    public override SectionType Type => sectionType;

    private SectionType sectionType;

    public Byte[] Data { get; set; } = [];

    private UnknownSection() { }


    public static WASMSection From(BinaryReader reader, SectionType type)
    {
        var unknownSection = new UnknownSection();
        unknownSection.sectionType = type;
        var size = reader.ReadUleb();
        unknownSection.Data = reader.ReadBytes((int)size);
        return unknownSection;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.Write(Data);
    }
}
