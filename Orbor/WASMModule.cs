using Orbor.Enums;
using Orbor.Sections;

namespace Orbor;

public sealed class WASMModule
{
    public uint Version { get; private set; }
    public List<WASMSection> Sections { get; private set; } = new();

    private WASMModule()
    {

    }

    public static WASMModule From(string path) {
        using var stream = new MemoryStream(File.ReadAllBytes(path));
        return From(stream);
    }

    public static WASMModule From(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        return From(reader);

    }

    public static WASMModule From(BinaryReader reader)
    {
        var module = new WASMModule();
        if (!reader.CheckMagic(0x00, 0x61, 0x73, 0x6D))
            throw new Exception("Not a valid wasm binary");

        Console.WriteLine("Magic number check Ok...");

        module.Version = reader.ReadUInt32();

        Console.WriteLine($"Wasm Module Version: {module.Version}");

        while (reader.BaseStream.Position < reader.BaseStream.Length) // Better way to do this?
            module.Sections.Add(module.ReadSection(reader));

        Console.WriteLine("Finished reading sections");

        return module;
    }

    public void Write(BinaryWriter writer)
    {
        writer.Write(new byte[] { 0x00, 0x61, 0x73, 0x6D });
        writer.Write(Version);
        if (Version != 1)
            throw new Exception($"Unsupported version: {Version}. Only Version 1 is supported");
        foreach (var section in Sections)
        {
            writer.Write((byte)section.Type);
            using var memoryStream = new MemoryStream();
            using var tempWriter = new BinaryWriter(memoryStream);
            section.Write(tempWriter);
            tempWriter.Flush();
            var data = memoryStream.ToArray();
            writer.WriteUleb((ulong)data.Length);
            writer.Write(data);
            
        }
    }

    public void Write(string path)
    {
        using var stream = new FileStream(path, FileMode.Create);
        using var writer = new BinaryWriter(stream);
        Write(writer);
    }



    private WASMSection ReadSection(BinaryReader reader)
    {
        var sectionType = (SectionType)reader.ReadByte();
        switch (sectionType)
        {
            case SectionType.Type:
                return TypeSection.From(reader);
            case SectionType.Import:
                return ImportSection.From(reader);
            case SectionType.Func:
                return FuncSection.From(reader);
            case SectionType.Global:
                return GlobalSection.From(reader);
            case SectionType.Data:
                return DataSection.From(reader);
            case SectionType.Memory:
                return MemorySection.From(reader);
            case SectionType.Export:
                return ExportSection.From(reader);
            case SectionType.Code:
                //return CodeSection.From(reader);
            case SectionType.Elem:
            case SectionType.Start:
            case SectionType.Table:
            case SectionType.Custom:
            default:
                return UnknownSection.From(reader, sectionType);
        }
    }

}
