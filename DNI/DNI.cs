using System.Reflection;
using System.Runtime.InteropServices;

namespace DNI
{
    public delegate IntPtr PtrCreateManagedStringFromCharPtr(IntPtr ptr, int len);
    public delegate int    PtrGetArraySizePtr(IntPtr ptr);
    
    public delegate int PtrGetIntArrayElementsPtr(IntPtr managedArrayObject, IntPtr destNativeArrayPtr, int startIndex, int size);
    public delegate int PtrSetIntArrayElementsPtr(IntPtr managedArrayObject, IntPtr srcNativeArrayPtr,  int startIndex, int size);
    public delegate IntPtr PtrNewIntArrayPtr(int size);

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public class DNI
    {
        public PtrCreateManagedStringFromCharPtr CreateManagedStringFromCharPtr = null;
        public PtrGetArraySizePtr                GetArraySizePtr = null;
        public PtrGetIntArrayElementsPtr         GetIntArrayElementsPtr = null;
        public PtrSetIntArrayElementsPtr         SetIntArrayElementsPtr = null;
        public PtrNewIntArrayPtr                 NewIntArrayPtr = null;
    }

    public class DNIHelper : IDisposable
    {
        private List<GCHandle> gCHandles = new List<GCHandle>();

        public DNI DNIInstance { get; set; } = new DNI();

        public DNIHelper()
        {
            DNIInstance.CreateManagedStringFromCharPtr = CreateManagedStringFromCharPtrImpl;
            DNIInstance.GetArraySizePtr = GetArraySizePtrImpl;
            DNIInstance.GetIntArrayElementsPtr = GetIntArrayElementsImpl;
            DNIInstance.SetIntArrayElementsPtr = SetIntArrayElementsImpl;
            DNIInstance.NewIntArrayPtr = NewIntArrayPtrImpl;
        }

        public IntPtr CreateManagedStringFromCharPtrImpl(IntPtr ptr, int len)
        {
            string s = Marshal.PtrToStringAnsi(ptr, len);
            GCHandle handle = GCHandle.Alloc(s);
            gCHandles.Add(handle);
            return GCHandle.ToIntPtr(handle);
        }

        public int GetArraySizePtrImpl(IntPtr ptr)
        {
            GCHandle handle = GCHandle.FromIntPtr(ptr);
            object obj = handle.Target;
            Type t = obj.GetType();
            PropertyInfo props = t.GetProperty("Length");
            if (props == null)
                return -1;
            object ret = props.GetValue(obj);
            return (int)ret;
        }

        int GetIntArrayElementsImpl(IntPtr managedArrayObject, IntPtr destNativeArrayPtr, int startIndex, int length)
        {
            GCHandle handle = GCHandle.FromIntPtr(managedArrayObject);
            object obj = handle.Target;
            int[] arr = obj as int[];
            if (arr == null || startIndex >= arr.Length )
                return -1;
            int remaining = arr.Length - startIndex;
            int sizeToCopy = Math.Min(remaining, length);
            Marshal.Copy(arr, startIndex, destNativeArrayPtr, sizeToCopy);
            return sizeToCopy;

        }

        int SetIntArrayElementsImpl(IntPtr managedArrayObject, IntPtr srcNativeArrayPtr, int startIndex, int length)
        {
            GCHandle handle = GCHandle.FromIntPtr(managedArrayObject);
            object obj = handle.Target;
            int[] arr = obj as int[];
            if (arr == null || startIndex >= arr.Length)
                return -1;
            int remaining = arr.Length - startIndex;
            int sizeToCopy = Math.Min(remaining, length);
            Marshal.Copy(srcNativeArrayPtr, arr, startIndex, sizeToCopy);
            return sizeToCopy;

        }

        public static T GetObjectFromPtr<T>(ref IntPtr ptr) where T : class
        {
            GCHandle handle = GCHandle.FromIntPtr(ptr);
            T ret = handle.Target as T;            
            ptr = IntPtr.Zero;
            return ret;
        }

        IntPtr NewIntArrayPtrImpl(int size)
        {
            int[] arr = new int[size];
            GCHandle handle = GCHandle.Alloc(arr);
            gCHandles.Add(handle);
            return GCHandle.ToIntPtr(handle);
        }

        public void Dispose()
        {
            foreach(GCHandle handle in gCHandles)
            {
                handle.Free();
            }
        }
    }
}