using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Shared
{
    public static class GuidHelper
    {
        public static Guid ToGuid(this ulong input)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(input).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        public static ulong ToUlong(this Guid input)
        {
            byte[] bytes = input.ToByteArray()[0..8];
            return BitConverter.ToUInt64(bytes, 0);
        }
    }
}