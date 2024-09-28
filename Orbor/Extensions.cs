using Orbor.Enums;
using System.Text;
using Orbor.Types;
using Orbor.Exports;

namespace Orbor;

internal static class Extensions
{
    public static bool CheckMagic(this BinaryReader binaryReader, params byte[] bytes)
    {
        foreach (var b in bytes)
            if (binaryReader.ReadByte() != b)
                return false;

        return true;
    }

    public static ulong ReadUleb(this BinaryReader binaryReader)
    {
        return binaryReader.BaseStream.ReadLEB128Unsigned();
    }

    public static long ReadSleb(this BinaryReader binaryReader)
    {
        return binaryReader.BaseStream.ReadLEB128Signed();
    }

    public static void WriteUleb(this BinaryWriter binaryWriter, ulong value)
    {
        binaryWriter.BaseStream.WriteLEB128Unsigned(value);
    }

    public static void WriteSleb(this BinaryWriter binaryWriter, long value)
    {
        binaryWriter.BaseStream.WriteLEB128Signed(value);
    }



    public static List<NumberType> ReadListNumberType(this BinaryReader binaryReader)
    {
        var count = binaryReader.ReadUleb();
        List<NumberType> returnList = new();
        for (ulong i = 0; i < count; i++)
            returnList.Add((NumberType)binaryReader.ReadByte());
        return returnList;
    }

    public static void WriteListNumberType(this BinaryWriter binaryWriter, List<NumberType> list)
    {
        binaryWriter.WriteUleb((ulong)list.Count);
        foreach (var item in list)
            binaryWriter.Write((byte)item);
    }

    public static string ReadName(this BinaryReader binaryReader)
    {
        var size = binaryReader.ReadUleb();
        byte[] buffer = new byte[size];
        binaryReader.Read(buffer, 0, (int)size);
        return Encoding.Default.GetString(buffer);
    }

    public static void WriteName(this BinaryWriter binaryWriter, string name)
    {
        var bytes = Encoding.Default.GetBytes(name);
        binaryWriter.WriteUleb((ulong)bytes.Length);
        binaryWriter.Write(bytes);
    }

    public static GlobalType ReadGlobalType(this BinaryReader binaryReader)
    {
        var vt = (NumberType)binaryReader.ReadByte();
        var mut = (MutType)binaryReader.ReadByte();
        return new GlobalType(vt, mut);
    }

    public static void WriteGlobalType(this BinaryWriter binaryWriter, GlobalType globalType)
    {
        binaryWriter.Write((byte)globalType.NumberType);
        binaryWriter.Write((byte)globalType.MutType);
    }

    public static LimitType ReadLimits(this BinaryReader binaryReader)
    {
        var limitType = binaryReader.ReadByte();
        var min = binaryReader.ReadUleb();
        ulong? max = null;
        if (limitType == 0x1)
            max = binaryReader.ReadUleb();
        return new LimitType(min, max);
    }

    public static void WriteLimits(this BinaryWriter binaryWriter, LimitType limitType)
    {
        binaryWriter.Write(limitType.HasMax ? (byte)0x1 : (byte)0x0);
        binaryWriter.WriteUleb(limitType.Minimum);
        if (limitType.HasMax)
            binaryWriter.WriteUleb(limitType.Maximum!.Value);
    }

    public static MemoryType ReadMemoryType(this BinaryReader binaryReader)
    {
        return new MemoryType(binaryReader.ReadLimits());
    }

    public static void WriteMemoryType(this BinaryWriter binaryWriter, MemoryType memoryType)
    {
        binaryWriter.WriteLimits(memoryType.LimitType);
    }

    public static TableType ReadTableType(this BinaryReader binaryReader)
    {
        var elemType = (ElemType)binaryReader.ReadByte();
        return new TableType(elemType, binaryReader.ReadLimits());
    }

    public static void WriteTableType(this BinaryWriter binaryWriter, TableType tableType)
    {
        binaryWriter.Write((byte)tableType.ElemType);
        binaryWriter.WriteLimits(tableType.LimitType);
    }

    /*
    public static byte[] ReadUntil(this BinaryReader binaryReader, byte b)
    {
        List<byte> bytes = new();
        byte c;
        if (binaryReader.PeekChar() != 0x0B)
            c = binaryReader.ReadByte();
        else
            return bytes.ToArray();
        while (c != 0x0B)
        {
            bytes.Add(c);
            c = binaryReader.ReadByte();
        }

        return bytes.ToArray();
    }
    */

    public static Export ReadExport(this BinaryReader binaryReader)
    {
        var name = binaryReader.ReadName();
        var exportType = (ExportType)binaryReader.ReadByte();
        var index = binaryReader.ReadUleb();

        return new Export(name, index, exportType);
    }

    public static void WriteExport(this BinaryWriter binaryWriter, Export export)
    {
        binaryWriter.WriteName(export.Name);
        binaryWriter.Write((byte)export.Type);
        binaryWriter.WriteUleb(export.Index);
    }

    public static byte[] ReadRestOfBytes(this BinaryReader reader)
    {
        const int bufferSize = 4096;
        using var ms = new MemoryStream();
        byte[] buffer = new byte[bufferSize];
        int count;
        while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
            ms.Write(buffer, 0, count);
        return ms.ToArray();
    }

    public static bool IsBitSet(this byte b, int pos)
    {
        return (b & (1 << pos)) != 0;
    }

    public static bool IsBitSet(this ulong ul, int pos)
    {
       return (ul & (1UL << pos)) != 0;
    }  


}
