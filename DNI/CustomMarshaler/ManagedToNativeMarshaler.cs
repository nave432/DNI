using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DNI.Marshaler
{
    public class ManagedToNativeMarshaler : ICustomMarshaler
    {
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return null;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            if(ManagedObj == null)
                return IntPtr.Zero;
            _handle = GCHandle.Alloc(ManagedObj, GCHandleType.Pinned);
            return _handle.AddrOfPinnedObject();
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
           
        }

        public void CleanUpManagedData(object ManagedObj)
        {
            _handle.Free();
        }

        public int GetNativeDataSize()
        {
            return -1;
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            // Always return the same instance
            if (marshaler == null)
            {
                marshaler = new ManagedToNativeMarshaler();
            }

            return marshaler;
        }

        GCHandle _handle;
        static private ManagedToNativeMarshaler marshaler;
    }
}

