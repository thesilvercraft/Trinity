using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Shared
{
    public abstract class TrinityGuid
    {
        public virtual Guid ToGuid()
        {
            return Guid.Empty;
        }
    }

    public class TrinityUlongGuid : TrinityGuid
    {
        public TrinityUlongGuid(ulong value)
        {
            Value = value;
        }

        public ulong Value { get; set; }

        public override Guid ToGuid()
        {
            return Value.ToGuid();
        }
    }
}