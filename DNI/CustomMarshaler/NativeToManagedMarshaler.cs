using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DNI.Marshaler
{
    public class NativeToManagedMarshaler: ICustomMarshaler
    {
        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            GCHandle handle = GCHandle.FromIntPtr(pNativeData);
            var ret = handle.Target;
            return ret;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            // Managed Object must be a long (i.e. 64bit integer) type.
            return IntPtr.Zero;
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            GCHandle handle = GCHandle.FromIntPtr(pNativeData);
            handle.Free();
        }

        public void CleanUpManagedData(object ManagedObj)
        {
            // Nothing to do
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
                marshaler = new NativeToManagedMarshaler();
            }

            return marshaler;
        }

        static private NativeToManagedMarshaler marshaler;
    }
}
