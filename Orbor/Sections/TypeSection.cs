using Orbor.Enums;
using Orbor.Types;

namespace Orbor.Sections;

public sealed class TypeSection : WASMSection
{
    private TypeSection() { }
    public override SectionType Type => SectionType.Type;
    
    public List<FunctionSignature> FunctionSignatures { get; set; } = [];

    private const byte Magic = 0x60;

    public static WASMSection From(BinaryReader reader)
    {
        var typeSection = new TypeSection();
        var size = reader.ReadUleb();
        var range = reader.ReadUleb();
        for (ulong i = 0; i < range; i++)
        {
            if (!reader.CheckMagic(Magic))
                throw new Exception("Corrupted type section found");
            var parameters = reader.ReadListNumberType();
            var results = reader.ReadListNumberType();
            typeSection.FunctionSignatures.Add(new FunctionSignature(parameters, results));
        }
        return typeSection;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.WriteUleb((ulong)FunctionSignatures.Count);

        foreach (var signature in FunctionSignatures)
        {
            writer.Write(Magic);
            writer.WriteListNumberType(signature.Parameters);
            writer.WriteListNumberType(signature.Results);
        }
    }
}
