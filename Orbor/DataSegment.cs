using Orbor.Enums;
namespace Orbor;

public sealed class DataSegment
{
    public Byte[] Data { get; set; } = [];

    public DataSegmentType Type { get; set; }

    //public bool IsActiveSegment => Flags.IsBitSet(1);
    //public bool IsPassiveSegment => Flags.IsBitSet(0);

    public ulong? MemoryIndex { get; set; }

    public List<Instruction>? InitExpression { get; set; }
    public DataSegment(DataSegmentType type, Byte[] data, ulong? memoryIndex = null, List<Instruction>? initInstructions = null)
    {
        Type = type;
        Data = data;
        MemoryIndex = memoryIndex;
        InitExpression = initInstructions;
    }
}
