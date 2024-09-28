using Orbor.Enums;

namespace Orbor.Types
{
    public sealed class TableType
    {
        public readonly ElemType ElemType;
        public readonly LimitType LimitType;
        public TableType(ElemType elemType, LimitType limitType)
        {
            ElemType = elemType;
            LimitType = limitType;
        }
    }
}
