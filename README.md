# DNI - DotNet Native Interface

A donet core to c++ header interoperability library

The DNI library provides a DNI instance object which has few function pointers(delegates) to manipulate DotNet objects. The DNI object can be passed to C++ layer using which DotNet object can be converted to c++ and vice versa. The concept is similar to Java Native Interface.

Examples
1. A simple example is passing an int array to c function as parameter and the C function returns an int array.

C# side

## Declaring intArrayFunction function.
The first parameter to the C++ function is DNI instance which has the function pointers that can be used from C++ side.
```        [DllImport("Cpp_Dll.dll")]
        // NativeToManagedMarshaler class converts a IntPtr to the return type. in this case int[]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(DNI.Marshaler.NativeToManagedMarshaler))]
        public static extern int[] intArrayFunction(DNI.DNI pDni,
        // ManagedToNativeMarshaler class converts a managed type to Inptr. In this case int[] to IntPtr
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(DNI.Marshaler.ManagedToNativeMarshaler))]
            int[] c);
```

## Calling C function.
DNIHelper.DNIInstance is of type DNI
```
            using (DNIHelper dNIHelper = new DNIHelper())
            {
                int[] inputArray = new int[] { 5,6,7,8 };
                int[] retArray = intArrayFunction(dNIHelper.DNIInstance, inputArray);
            }
```

## C++ side
All managed objects to the C functions is passed as void* ( DNIIntArray is just a pointer type to make it more type safe )
```
	_declspec(dllexport) DNI::Types::DNIIntArray intArrayFunction(DNI::DNI* pDni, DNI::Types::DNIIntArray pArray)
	{
		unsigned int length = pDni->GetArraySize(pArray);
		int* nativeIntArray = new int[length];
		int copied = pDni->GetIntArrayElements(pArray, nativeIntArray, 0, length);
		DNI::Types::DNIIntArray pArrayOut = pDni->NewIntArray(length);
		pDni->SetIntArrayElements(pArrayOut, nativeIntArray, 0, length);
		delete[] nativeIntArray;
		return pArrayOut;
	}
```
The above example show raw code on manipulating dotnet object via DNI. There few helper functions written in DNINative project (converters.h), which can do the above things in 2 lines of code.

```
	_declspec(dllexport) DNI::Types::DNIIntArray intArrayFunction(DNI::DNI* pDni, DNI::Types::DNIIntArray pArray)
	{
		// converting DNIIntArray to vector
		const std::vector<int> arrayItems = DNI::convertTo<std::vector<int> >(pDni, pArray);

		//converting vector to DNIIntArray
		return DNI::convertTo<DNI::Types::DNIIntArray>(pDni, arrayItems);
	}
```