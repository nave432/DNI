# DNI - DotNet Native Interface

A donet core to c++ header interoperability library

The DNI library provides a DNI instance object which has few function pointers(delegates) to manipulate DotNet objects. The DNI object can be passed to C++ layer using which DotNet object can be converted to c++ and vice versa. The concept is similar to Java Native Interface.

Examples
## 1. Passing a amanged object to c/c++
A simple example is passing an dotnet int array to c function as parameter and the C function returns a dotnet int array.

C# side

### Declaring intArrayFunction function.
The first parameter to the C++ function is DNI instance which has the function pointers that can be used from C++ side.
```        [DllImport("Cpp_Dll.dll")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(DNI.Marshaler.NativeToManagedMarshaler))]
        public static extern int[] intArrayFunction(DNI.DNI pDni,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(DNI.Marshaler.ManagedToNativeMarshaler))]
            int[] c);
```

### Calling C function.
DNIHelper.DNIInstance is of type DNI
```
            using (DNIHelper dNIHelper = new DNIHelper())
            {
                int[] inputArray = new int[] { 5,6,7,8 };
                int[] retArray = intArrayFunction(dNIHelper.DNIInstance, inputArray);
            }
```

### C++ side
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
## 2. Using C++ class in DotNet.
In the example in test project, there is "Zoo" and "Animal" class in C++ and for those classes, there is corresponding proxy classes in dotnetside. The only member variable in the ProxyClass is a IntPtr which is a point to the actuall pointer to the class in C++.

### C# side
```
    internal class Zoo : BaseProxyClass
    {

        [DllImport("Cpp_Dll.dll")]
        private static extern IntPtr ClassTest_Zoo_Constructor(DNI.DNI dniInstance);

        . . . . 

        [DllImport("Cpp_Dll.dll")]
        private static extern IntPtr ClassTest_Zoo_CreateAnimal(DNI.DNI pDni, IntPtr pClassPtr, int type, string name);

        public override IntPtr ProxyHandle { get; protected set; }

        public Zoo()
        {
            using (DNIHelper helper = new DNIHelper())
            {
                ProxyHandle = ClassTest_Zoo_Constructor(helper.DNIInstance);
            }
        }

        public Animal CreateAnimal(AnimalTypes type, string name)
        {
            using (DNIHelper helper = new DNIHelper())
            {
                return new Animal(ClassTest_Zoo_CreateAnimal(helper.DNIInstance, ProxyHandle, (int)type, name));
            }
        }
    }
```
### C++ side

The class definition
```
class Zoo
{
public:
	int  GetAnimalCount();
	bool AddAnimal(Animal* pAnimals);
	bool RemoveAnimal(const std::string& name);
        Animal* CreateAnimal(const AnimalTypes type, const std::string& name);
       . . . . .
}
```

C wrapper functions which can be called from dotnet
```
extern "C" _declspec(dllexport) DNINativeObject ClassTest_Zoo_Constructor(DNI::DNI * pDni)
{
	return new Zoo();
}

extern "C" _declspec(dllexport) DNINativeObject ClassTest_Zoo_CreateAnimal(DNI::DNI * pDni, DNINativeObject pClassPtr, const DNIEnum type, const char* name)
{
	return ((Zoo*)pClassPtr)->CreateAnimal(
		DNI::convertTo<AnimalTypes>(pDni, type),
		DNI::convertTo<std::string>(pDni, name));
}
```
