using Orbor.Enums;
using System.IO;

namespace Orbor.Sections;

public sealed class GlobalSection : WASMSection
{
    public override SectionType Type => SectionType.Global;

    public List<Global> Globals { get; set; } = new List<Global>();

    private GlobalSection() { }

    public static GlobalSection From(BinaryReader reader)
    {
        var globalSection = new GlobalSection();
        var size = reader.ReadUleb();
        var range = reader.ReadUleb();
        for (ulong i = 0; i < range; i++)
        {
            var globalType = reader.ReadGlobalType();
            var initExpression = Instruction.Disassemble(reader);
            globalSection.Globals.Add(new Global(globalType, initExpression));
        }
        return globalSection;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.WriteUleb((ulong)Globals.Count);
        foreach (var global in Globals)
        {
            writer.WriteGlobalType(global.GlobalType);
            writer.Write(Instruction.Assemble(global.InitExpression));
        }
    }
}
