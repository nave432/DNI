using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNITests.ProxyClasses
{
    public abstract class BaseProxyClass : IDisposable
    {
        public abstract IntPtr ProxyHandle { get; protected set; }

        public BaseProxyClass()
        {
        }

        public abstract void Dispose();
    }
}
