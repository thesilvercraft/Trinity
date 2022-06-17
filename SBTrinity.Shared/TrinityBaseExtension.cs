using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Shared
{
    public abstract class TrinityBaseExtension
    {
        public List<IPlatformProvider> Providers = new();

        public TrinityBaseExtension(params IPlatformProvider[] platform)
        {
            Providers.AddRange(platform);
        }

        public virtual Task Remove(IPlatformProvider platform)
        {
            Providers.Remove(platform);
            return Task.CompletedTask;
        }

        public abstract void Setup(IPlatformProvider platform);
    }
}