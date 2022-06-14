using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Shared
{
    public delegate Task AsyncEvent<in TSender, in TArgs>(TSender sender, TArgs e) where TArgs : TrinityEventArgs;

    public class TrinityEventArgs
    {
        public bool Handled { get; set; }
    }
}