using System.Reflection;
using System.Runtime.InteropServices;

namespace DNI
{
    public delegate byte   PtrGetBoolFromObject(IntPtr ptr);
    public delegate int    PtrGetIntFromObject(IntPtr ptr);
    public delegate double PtrGetDoubleFromObject(IntPtr ptr);

    public delegate IntPtr PtrBoolToObject(byte boolVal);
    public delegate IntPtr PtrIntToObject(int intVal);
    public delegate IntPtr PtrDoubleToObject(double dbl);


    #region Array function
    public delegate IntPtr  PtrNewObjectArray(IntPtr strObjectTypeName, int size);
    public delegate int     PtrGetArraySize(IntPtr ptr);
    public delegate int     PtrSetArrayElements(IntPtr managedArrayObject, IntPtr srcNativeArrayPtr, int startIndex, int size);
    
    public delegate int     PtrGetIntArrayElements(IntPtr managedArrayObject, IntPtr destNativeArrayPtr, int startIndex, int size);
    public delegate int     PtrSetIntArrayElements(IntPtr managedArrayObject, IntPtr srcNativeArrayPtr,  int startIndex, int size);
    public delegate IntPtr  PtrNewIntArray(int size);
    #endregion

    #region reflections
    public delegate IntPtr  PtrGetMethod(IntPtr managedObject,    IntPtr methodName, IntPtr signature);
    public delegate IntPtr  PtrInvokeMethod(IntPtr managedObject, IntPtr methodPtr,  IntPtr parameters);
    public delegate IntPtr  PtrGetProperty(IntPtr managedObject,  IntPtr propertyName);
    public delegate IntPtr  PtrGetGenericType(IntPtr typeName, IntPtr parameters);
    public delegate IntPtr  PtrCreateInstance(IntPtr type, IntPtr parameters);
    #endregion

    #region string function
    public delegate IntPtr  PtrCreateManagedStringFromChar(IntPtr ptr, int len);
    public delegate int     PtrGetStringLength(IntPtr stringPtr);
    public delegate IntPtr  PtrStringToANSIString(IntPtr stringPtr);
    public delegate void    PtrFreeCoTaskMem(IntPtr nativeCharPtr);
    #endregion

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public class DNI
    {
        
        public PtrGetBoolFromObject          GetBoolFromObject    = null;
        public PtrGetIntFromObject           GetIntFromObject     = null;
        public PtrGetDoubleFromObject        GetDoubleFromObject  = null;

        public PtrBoolToObject               BoolToObject     = null;
        public PtrIntToObject                IntToObject      = null;
        public PtrDoubleToObject             DoubleToObject   = null;

        // array fuctions
        public PtrNewObjectArray             NewObjectArray = null;
        public PtrGetArraySize               GetArraySize = null;
        public PtrSetArrayElements           SetArrayElements = null;
        // 1. int functions (TODO add all other native types)
        public PtrGetIntArrayElements        GetIntArrayElements = null;
        public PtrSetIntArrayElements        SetIntArrayElements = null;
        public PtrNewIntArray                NewIntArray = null;
        
        // Reflection function
        public PtrGetMethod                  GetMethod    = null;
        public PtrInvokeMethod               InvokeMethod = null;
        public PtrGetProperty                GetProperty  = null;
        public PtrGetGenericType             GetGenericType = null;
        public PtrCreateInstance             CreateInstance = null;

        // string functions
        public PtrCreateManagedStringFromChar CreateManagedStringFromChar = null;
        public PtrGetStringLength             GetStringLength = null;
        public PtrStringToANSIString          StringToANSIString = null;
        public PtrFreeCoTaskMem               FreeCoTaskMem = null;
    }

    public class DNIHelper : IDisposable
    {
        private List<GCHandle> gCHandles = new List<GCHandle>();

        public DNI DNIInstance { get; set; } = new DNI();

        public DNIHelper()
        {
            DNIInstance.GetBoolFromObject            = GetBoolFromObjectImpl;
            DNIInstance.GetIntFromObject             = GetIntFromObjectImpl;
            DNIInstance.GetDoubleFromObject          = GetDoubleFromObjectImpl;

            DNIInstance.BoolToObject                 = BoolToObjectImpl;
            DNIInstance.IntToObject                  = IntToObjectImpl;
            DNIInstance.DoubleToObject               = DoubleToObjectImpl;

            // array functions
            DNIInstance.NewObjectArray               = NewObjectArrayImpl;
            DNIInstance.GetArraySize                 = GetArraySizeImpl;
            DNIInstance.SetArrayElements             = SetArrayElementsImpl;
            DNIInstance.GetIntArrayElements          = GetIntArrayElementsImpl;
            DNIInstance.SetIntArrayElements          = SetIntArrayElementsImpl;
            DNIInstance.NewIntArray                  = NewIntArrayImpl;
            
            // reflection function
            DNIInstance.GetMethod                    = GetMethodImpl;
            DNIInstance.InvokeMethod                 = InvokeMethodImpl;
            DNIInstance.GetProperty                  = GetPropertyImpl;
            DNIInstance.GetGenericType               = GetGenericTypeImpl;
            DNIInstance.CreateInstance               = CreateInstanceImpl;

            #region string function
            DNIInstance.CreateManagedStringFromChar  = CreateManagedStringFromCharImpl;
            DNIInstance.GetStringLength              = GetStringLengthImpl;
            DNIInstance.StringToANSIString           = StringToANSIStringImpl;
            DNIInstance.FreeCoTaskMem                = FreeCoTaskMemImpl;
            #endregion
        }

        public IntPtr CreateManagedStringFromCharImpl(IntPtr ptr, int len)
        {
            string s = Marshal.PtrToStringAnsi(ptr, len);
            return IntPtrFromObject(s);
        }

        public byte GetBoolFromObjectImpl(IntPtr ptr)
        {
            object temp = GCHandle.FromIntPtr(ptr).Target;
            if(temp == null )
                return 0;
            byte bTrue = 1;
            byte bFalse = 0;
            return ((bool)temp)? bTrue: bFalse;
        }

        public int GetIntFromObjectImpl(IntPtr ptr)
        {
            object temp = GCHandle.FromIntPtr(ptr).Target;
            if (temp == null)
                return 0;
            return (int)temp;
        }

        public double GetDoubleFromObjectImpl(IntPtr ptr)
        {
            object temp = GCHandle.FromIntPtr(ptr).Target;
            if (temp == null)
                return 0;
            return (double)temp;
        }

        #region array functions
        IntPtr NewObjectArrayImpl(IntPtr strObjectTypeName, int size)
        {
            string ObjectTypeName = StringFrom(strObjectTypeName);
            Type t = Type.GetType(ObjectTypeName);
            var arr = Array.CreateInstance(t, size);
            return IntPtrFromObject(arr);
        }

        public int GetArraySizeImpl(IntPtr ptr)
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

        int SetArrayElementsImpl(IntPtr managedArrayObject, IntPtr srcNativeArrayPtr, int startIndex, int length)
        {
            IntPtr[] destination = new IntPtr[length];
            Marshal.Copy(srcNativeArrayPtr, destination, startIndex, length);
            List<object> objList = new List<object>();
            foreach (IntPtr ptr in destination)
            {
                objList.Add(GetObjectFromIntPtr<object>(ptr));
            }

            object[] arr = GetObjectFromIntPtr<object[]>(managedArrayObject);
            if (arr == null || startIndex >= arr.Length)
                return -1;
            int remaining = arr.Length - startIndex;
            int sizeToCopy = Math.Min(remaining, length);
            for ( int i = startIndex; i < sizeToCopy; i++ )
            {
                arr[i] = objList[i];
            }            
            return sizeToCopy;
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

        IntPtr NewIntArrayImpl(int size)
        {
            int[] arr = new int[size];
            return IntPtrFromObject(arr);
        }

        #endregion

        #region Reflection functions

        private IntPtr GetMethodImpl(IntPtr managedObject, IntPtr methodNamePtr, IntPtr signaturePtr)
        {
            string methodName = StringFrom(methodNamePtr);
            string signature = StringFrom(signaturePtr);

            GCHandle handle = GCHandle.FromIntPtr(managedObject);
            object obj = handle.Target;
            Type objType = obj.GetType();

            List<Type> types = ParseTypes(signature);
            if(types == null)
                return IntPtr.Zero;
            MethodInfo methodInfo = objType.GetMethod(methodName, types.ToArray());
            if (methodInfo == null)
                return IntPtr.Zero;
            return IntPtrFromObject(methodInfo);
        }

        IntPtr InvokeMethodImpl(IntPtr managedObject, IntPtr methodPtr, IntPtr parametersPtr)
        {
            Object obj = GCHandle.FromIntPtr(managedObject).Target;
            MethodInfo method = GCHandle.FromIntPtr(methodPtr).Target as MethodInfo;
            if (method == null) 
                return IntPtr.Zero;
            object[] parameter = new object[0];
            if(parametersPtr != IntPtr.Zero)
                parameter = GCHandle.FromIntPtr(parametersPtr).Target as object[];
            Object ret = method.Invoke(obj, parameter);
            if( ret == null )
                return IntPtr.Zero;
            return IntPtrFromObject(ret);
        }

        IntPtr GetPropertyImpl(IntPtr managedObject, IntPtr propertyNamePtr)
        {
            Object obj = GCHandle.FromIntPtr(managedObject).Target;
            string propName = StringFrom(propertyNamePtr);
            Type t = obj.GetType();
            PropertyInfo props = t.GetProperty(propName);
            if (props == null)
                return IntPtr.Zero;
            object ret = props.GetValue(obj);
            return IntPtrFromObject(ret);
        }

        IntPtr GetGenericTypeImpl(IntPtr typeName, IntPtr parameters)
        {
            string strType = StringFrom(typeName);
            Type openType = Type.GetType(strType);

            if (parameters == IntPtr.Zero)
            {
                return IntPtrFromObject(openType);
            }
            string signature = StringFrom(parameters);
            // To create a List<string>
            List<Type> tArgs = ParseTypes(signature);
            if (tArgs == null)
                return IntPtr.Zero;
            Type target = openType.MakeGenericType(tArgs.ToArray());
            return IntPtrFromObject(target);
        }

        IntPtr CreateInstanceImpl(IntPtr typeName, IntPtr parameters)
        {
            //TODO use parameters if not empty
            Type typeObj = GCHandle.FromIntPtr(typeName).Target as Type;
            object ret = Activator.CreateInstance(typeObj);
            return IntPtrFromObject(ret);
        }

        #endregion 

        IntPtr BoolToObjectImpl(byte boolVal)
        {
            Object obj = boolVal;
            return IntPtrFromObject(obj);
        }
        
        IntPtr IntToObjectImpl(int intVal)
        {
            Object obj = intVal;
            return IntPtrFromObject(obj);
        }
        
        IntPtr DoubleToObjectImpl(double dbl)
        {
            Object obj = dbl;
            return IntPtrFromObject(obj);
        }


        #region string functions
        int GetStringLengthImpl(IntPtr stringPtr)
        {
            string str = StringFrom(stringPtr);
            return str.Length;
        }

        IntPtr StringToANSIStringImpl(IntPtr stringPtr)
        {
            string str = StringFrom(stringPtr);
            return Marshal.StringToCoTaskMemAnsi(str);
        }

        void FreeCoTaskMemImpl(IntPtr nativeStringPtr)
        {
            Marshal.FreeCoTaskMem(nativeStringPtr);
        }
        #endregion

        private List<Type> ParseTypes(string signature)
        {
            List<Type> types = new List<Type>();
            if (!string.IsNullOrEmpty(signature))
            {
                // TODO, parsing need to more intelligent
                string[] strTypes = signature.Split(",");
                foreach (var strType in strTypes)
                {
                    Type t = Type.GetType(strType);
                    if (t == null)
                        return null;
                    types.Add(t);
                }
            }
            return types;
        }

        //private function
        private IntPtr IntPtrFromObject(object obj)
        {
            GCHandle handle = GCHandle.Alloc(obj);
            gCHandles.Add(handle);
            return GCHandle.ToIntPtr(handle);
        }

        private string StringFrom(IntPtr strPtr)
        {
            if (strPtr == IntPtr.Zero)
                return string.Empty;
            GCHandle handle = GCHandle.FromIntPtr(strPtr);
            return handle.Target as string;
        }

        private T GetObjectFromIntPtr<T>(IntPtr objPtr) where T : class
        {
            if (objPtr == IntPtr.Zero)
                return default(T);
            GCHandle handle = GCHandle.FromIntPtr(objPtr);
            return handle.Target as T;
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