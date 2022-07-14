using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNITests.ProxyClasses
{
    internal class Animal : BaseProxyClass
    {
        public override IntPtr ProxyHandle { get;protected set ; }

        public Animal(IntPtr proxyHandle)
        {
            ProxyHandle = proxyHandle;
        }

        public override void Dispose()
        {
            //life time
        }
    }
}
