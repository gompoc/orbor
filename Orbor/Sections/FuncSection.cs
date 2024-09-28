using Orbor.Enums;

namespace Orbor.Sections;

public sealed class FuncSection : WASMSection
{
    public override SectionType Type => SectionType.Func;

    public List<Function> Functions { get; set; } = [];

    private FuncSection() { }


    public static FuncSection From(BinaryReader reader)
    {
        var funcSection = new FuncSection();
        var size = reader.ReadUleb();
        var range = reader.ReadUleb();
        Console.WriteLine($"Total functions: {range}");
        for (ulong i = 0; i < range; i++)
        {
            var typeIndex = reader.ReadUleb();
            funcSection.Functions.Add(new Function(typeIndex));
        }

        return funcSection;
    }

    public override void Write(BinaryWriter writer)
    { 
        writer.WriteUleb((ulong)Functions.Count);
        foreach (var func in Functions)
        {
            writer.WriteUleb(func.TypeIndex);
        }
    }
}
