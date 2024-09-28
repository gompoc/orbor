using Orbor.Enums;
using System.IO;

namespace Orbor.Sections;

public sealed class CodeSection : WASMSection
{
    public override SectionType Type => SectionType.Code;
    public List<FunctionBody> FunctionBodies { get; set; } = new();

    private CodeSection() { }

    public static CodeSection From(BinaryReader reader)
    {
        var codeSection = new CodeSection();
        var size = reader.ReadUleb();
        var range = reader.ReadUleb();
        for (ulong i = 0; i < range; i++)
        {
            
            var codeSize = reader.ReadUleb();

            var code = reader.ReadBytes((int)codeSize);
            using var memoryStream = new MemoryStream(code);
            using var binaryReader = new BinaryReader(memoryStream);

            var totalLocals = binaryReader.ReadUleb();
            List<Local> locals = new();
            for (ulong j = 0; j < totalLocals; j++)
            {
                var localCount = binaryReader.ReadUleb();
                var localType = (NumberType)binaryReader.ReadByte();
                for (ulong k = 0; k < localCount; k++)
                    locals.Add(new Local(localType));
            }
            var instructions = Instruction.Disassemble(binaryReader, (int)codeSize - (int)binaryReader.BaseStream.Position);

            instructions.AddRange(Instruction.Disassemble(binaryReader, 1));
            
            codeSection.FunctionBodies.Add(new FunctionBody(locals, instructions));
        }
        return codeSection;
    }

    public override void Write(BinaryWriter writer)
    {
        writer.WriteUleb((ulong)FunctionBodies.Count);
        foreach (var body in FunctionBodies)
        {
            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.WriteUleb((ulong)body.Locals.Count);
            foreach (var local in body.Locals)
            {
                binaryWriter.WriteUleb(1);
                binaryWriter.Write((byte)local.NumberType);
            }
            binaryWriter.Write(Instruction.Assemble(body.Instructions));
            writer.WriteUleb((ulong)memoryStream.Length);
            writer.Write(memoryStream.ToArray());
        }
    }
}
