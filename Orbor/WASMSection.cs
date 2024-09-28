using Orbor.Enums;

namespace Orbor;

public abstract class WASMSection
{

    protected WASMSection() { }
    public abstract SectionType Type { get; }

    public abstract void Write(BinaryWriter writer);
}
