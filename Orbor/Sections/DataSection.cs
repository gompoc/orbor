using Orbor.Enums;

namespace Orbor.Sections;

public sealed class DataSection : WASMSection
{
    public override SectionType Type => SectionType.Data;

    public List<DataSegment> Segments { get; set; } = [];

    private DataSection() { }


    public static WASMSection From(BinaryReader reader)
    {
        var dataSection = new DataSection();
        var size = reader.ReadUleb();
        var range = reader.ReadUleb();

        for (ulong i = 0; i < range; i++)
        {
            var dataSegmentType = (DataSegmentType)reader.ReadUleb();
            ulong? memoryIndex = null;
            List<Instruction>? instructions = null;
            if (dataSegmentType == DataSegmentType.Active)
            {
                memoryIndex = 0;
                instructions = Instruction.Disassemble(reader);
            } else if (dataSegmentType == DataSegmentType.ActiveWithMemoryIndex) // TODO: change this
            {
                memoryIndex = reader.ReadUleb();
                instructions = Instruction.Disassemble(reader);
            }
            var dataSize = reader.ReadUleb();
            var data = reader.ReadBytes((int)dataSize);
            dataSection.Segments.Add(new DataSegment(dataSegmentType, data, memoryIndex, instructions));
        }

        return dataSection;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.WriteUleb((ulong)Segments.Count);
        foreach (var segment in Segments)
        {
            writer.WriteUleb((ulong)segment.Type);
            if (segment.Type == DataSegmentType.Active)
            {
                writer.Write(Instruction.Assemble(segment.InitExpression!));
            }
            else if (segment.Type == DataSegmentType.ActiveWithMemoryIndex)
            {
                writer.WriteUleb(segment.MemoryIndex!.Value);
                writer.Write(Instruction.Assemble(segment.InitExpression!));
            }
            writer.WriteUleb((ulong)segment.Data.Length);
            writer.Write(segment.Data);
        }
    }
}
