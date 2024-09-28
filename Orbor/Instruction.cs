using System.Text;
using Orbor.Enums;
using Orbor.Operands;

namespace Orbor;

public sealed class Instruction
{
    public readonly OpCode OpCode;
    public int Size { get; private set; }
    public readonly IOperand[] Operands;

    private Instruction(OpCode opCode)
    {
        OpCode = opCode;
        Size = 0;
        Operands = new IOperand[OperandTypes().Count];
    }

    public static Instruction Create(OpCode opCode, params IOperand[] operands)
    {
        var instruction = new Instruction(opCode);
        var operandTypes = instruction.OperandTypes();
        if (operands.Length != operandTypes.Count)
            throw new ArgumentException("Invalid number of operands");
        for (int i = 0; i < operands.Length; i++)
        {
            if (operandTypes[i] != operands[i].GetType())
                throw new ArgumentException("Invalid operand type");
            instruction.Operands[i] = operands[i];
        }
        return instruction;
    }

    private List<Type> OperandTypes()
    {
        switch (OpCode)
        {
            case OpCode.Block:
            case OpCode.If:
            case OpCode.Loop:
                return BlockOperandList;
            case OpCode.LocalGet:
            case OpCode.LocalSet:
            case OpCode.LocalTee:
            case OpCode.GlobalGet:
            case OpCode.GlobalSet:
                return LocalList;
            case OpCode.CallIndirect:
                return CallIndirectList;
            case OpCode.Br:
            case OpCode.BrIf:
            case OpCode.Call:
                return ControlFlowList;
            case OpCode.BrTable:
                return BranchTableList;
            case OpCode.I32Load:
            case OpCode.I64Load:
            case OpCode.F32Load:
            case OpCode.F64Load:
            case OpCode.I32Load8S:
            case OpCode.I32Load8U:
            case OpCode.I32Load16S:
            case OpCode.I32Load16U:
            case OpCode.I64Load8S:
            case OpCode.I64Load8U:
            case OpCode.I64Load16S:
            case OpCode.I64Load16U:
            case OpCode.I64Load32S:
            case OpCode.I64Load32U:
            case OpCode.I32Store:
            case OpCode.I64Store:
            case OpCode.F32Store:
            case OpCode.F64Store:
            case OpCode.I32Store8:
            case OpCode.I32Store16:
            case OpCode.I64Store8:
            case OpCode.I64Store16:
            case OpCode.I64Store32:
                return LoadStoreList;
            case OpCode.MemorySize:
            case OpCode.MemoryGrow:
                return MemoryList;
            case OpCode.I32Const:
                return I32List;
            case OpCode.I64Const:
                return I64List;
            case OpCode.F32Const:
                return F32List;
            case OpCode.F64Const:
                return F64List;
            default:
                return EmptyList;
        }
    }

    private static readonly List<Type> BlockOperandList = new() { typeof(BlockTypeOperand) };
    private static readonly List<Type> LocalList = new() { typeof(IndexOperand) };
    private static readonly List<Type> CallIndirectList = new() { typeof(IndexOperand), typeof(ZeroOperand) };
    private static readonly List<Type> ControlFlowList = new() { typeof(IndexOperand) };
    private static readonly List<Type> BranchTableList = new() { typeof(IndexVectorOperand), typeof(IndexOperand) };
    private static readonly List<Type> LoadStoreList = new() { typeof(MemArgOperand) };
    private static readonly List<Type> MemoryList = new() { typeof(ZeroOperand) };
    private static readonly List<Type> I32List = new() { typeof(I32Operand) };
    private static readonly List<Type> I64List = new() { typeof(I64Operand) };
    private static readonly List<Type> F32List = new() { typeof(F32Operand) };
    private static readonly List<Type> F64List = new() { typeof(F64Operand) };
    private static readonly List<Type> EmptyList = new();


    public string OpName()
    {
        return OpCode.ToString().ToLower().Replace("_", ".");
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(OpName());
        for (int i = 0; i < Operands.Length; i++)
        {
            if (i > 1)
                stringBuilder.Append(',');
            var opStr = Operands[i].ToString();
            if (!string.IsNullOrEmpty(opStr))
                stringBuilder.Append($" {opStr}");
        }

        return stringBuilder.ToString();
    }


    public static List<Instruction> Disassemble(byte[] data)
    {
        int offset = 0;
        List<Instruction> instructions = new();
        while (offset < data.Length)
        {
            var instruction = DisassembleInstruction(data, offset);
            instructions.Add(instruction);
            offset += instruction.Size;
        }
        return instructions;
    }

    internal static List<Instruction> Disassemble(BinaryReader binaryReader, int length)
    {
        int offset = 0;
        List<Instruction> instructions = new();
        while (offset < length)
        {
            var instruction = DisassembleInstruction(binaryReader, offset);
            instructions.Add(instruction);
            offset += instruction.Size;
        }
        return instructions;
    }

    public static byte[] Assemble(List<Instruction> instructions)
    {
        using var memoryStream = new MemoryStream();
        using var binaryWriter = new BinaryWriter(memoryStream);

        foreach (var instruction in instructions)
        {
            binaryWriter.Write((byte)instruction.OpCode);
            var operands = instruction.OperandTypes();

            for (int i = 0; i < operands.Count; i++)
            {
                var operand = operands[i];

                if (operand == typeof(IndexOperand))
                    binaryWriter.WriteUleb(((IndexOperand)instruction.Operands[i]).Index);
                else if (operand == typeof(IndexVectorOperand))
                {
                    var ivo = (IndexVectorOperand)instruction.Operands[i];
                    binaryWriter.WriteUleb((ulong)ivo.Indices.Count);
                    foreach (var index in ivo.Indices)
                        binaryWriter.WriteUleb(index);
                }
                else if (operand == typeof(BlockTypeOperand))
                {
                    var bto = (BlockTypeOperand)instruction.Operands[i];
                    binaryWriter.WriteUleb(bto.ValueTypeEnum.HasValue ? (ulong)bto.ValueTypeEnum.Value : 0x40);
                }
                else if (operand == typeof(MemArgOperand))
                {
                    var mao = (MemArgOperand)instruction.Operands[i];
                    binaryWriter.WriteUleb(mao.Align);
                    binaryWriter.WriteUleb(mao.Offset);
                }
                else if (operand == typeof(I32Operand))
                    binaryWriter.WriteSleb((long)(int)((I32Operand)instruction.Operands[i]).Value);
                else if (operand == typeof(I64Operand))
                    binaryWriter.WriteSleb((long)((I64Operand)instruction.Operands[i]).Value);
                else if (operand == typeof(F32Operand))
                    binaryWriter.Write((float)((F32Operand)instruction.Operands[i]).Value);
                else if (operand == typeof(F64Operand))
                    binaryWriter.Write((double)((F64Operand)instruction.Operands[i]).Value);
            }
        }
        binaryWriter.Flush();
        return memoryStream.ToArray();

    }

    public static List<Instruction> Disassemble(BinaryReader binaryReader)
    {
        List<Instruction> instructions = new();

        bool done = false;
        while (!done)
        {
            var instruction = DisassembleInstruction(binaryReader, 0);
            if (instruction.OpCode == OpCode.End)
                done = true;
            instructions.Add(instruction);
        }
        return instructions;
    }

    private static Instruction DisassembleInstruction(BinaryReader binaryReader, int offset)
    {

        //binaryReader.BaseStream.Position += offset;
        var opCode = (OpCode)binaryReader.ReadByte();
       
        var instruction = new Instruction(opCode);

        var operands = instruction.OperandTypes();

        for (int i = 0; i < operands.Count; i++)
        {
            var operand = operands[i];

            if (operand == typeof(IndexOperand))
                instruction.SetOperand(i, new IndexOperand(binaryReader.ReadUleb()));
            else if (operand == typeof(IndexVectorOperand))
            {
                var ivo = new IndexVectorOperand();
                var range = binaryReader.ReadUleb();
                for (ulong j = 0; j < range; j++)
                    ivo.Add(binaryReader.ReadUleb());
                instruction.SetOperand(i, ivo);
            }
            else if (operand == typeof(BlockTypeOperand))
            {
                var resultType = binaryReader.ReadUleb();
                instruction.SetOperand(i, resultType != 0x40 ? new BlockTypeOperand((NumberType)resultType) : new BlockTypeOperand());
            }
            else if (operand == typeof(MemArgOperand))
                instruction.SetOperand(i, new MemArgOperand(binaryReader.ReadUleb(), binaryReader.ReadUleb()));
            else if (operand == typeof(I32Operand))
                instruction.SetOperand(i, new I32Operand((int)binaryReader.ReadSleb()));
            else if (operand == typeof(I64Operand))
                instruction.SetOperand(i, new I64Operand(binaryReader.ReadSleb()));
            else if (operand == typeof(F32Operand))
                instruction.SetOperand(i, new F32Operand(binaryReader.ReadSingle()));
            else if (operand == typeof(F64Operand))
                instruction.SetOperand(i, new F64Operand(binaryReader.ReadDouble()));
        }

        instruction.Size = (int)binaryReader.BaseStream.Position - offset;
        return instruction;
    }

    private void SetOperand(int index, IOperand operand) => Operands[index] = operand;

    private static Instruction DisassembleInstruction(byte[] data, int offset)
    {
        using var memoryStream = new MemoryStream(data);
        using var binaryReader = new BinaryReader(memoryStream);

        return DisassembleInstruction(binaryReader, offset);
    }
}
