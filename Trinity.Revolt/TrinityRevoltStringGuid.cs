using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Shared;

namespace Trinity.Revolt
{
    public class TrinityRevoltStringGuid : TrinityGuid
    {
        public TrinityRevoltStringGuid(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public override Guid ToGuid()
        {
            byte[] bytes = Encoding.UTF8.GetBytes(Value);
            if (bytes.Length != 16)
            {
                if (bytes.Length > 16)
                {
                    bytes = bytes.Take(16).ToArray();
                }
                else
                {
                    List<byte> b = new(bytes);
                    while (b.Count != 16)
                    {
                        b.Add(69);
                    }
                    Debug.WriteLine("RevoltStringGuid.ToGuid: String was too short, had to pad for " + (b.Count - bytes.Length) + " bytes");
                    bytes = b.ToArray();
                }
            }
            return new(bytes);
        }
    }
}