using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DNI;
namespace DNITests.ProxyClasses
{
    enum AnimalTypes
    {
        Dog,
		Cat
    };

    internal class Zoo : BaseProxyClass
    {
        [DllImport("Cpp_Dll.dll")]
        private static extern IntPtr ClassTest_Zoo_Constructor(DNI.DNI dniInstance);

        [DllImport("Cpp_Dll.dll")]
        private static extern IntPtr ClassTest_Zoo_Destructor(DNI.DNI dniInstance, IntPtr proxyHandle);

        [DllImport("Cpp_Dll.dll")]
        private static extern int ClassTest_Zoo_GetAnimalCount(DNI.DNI pDni, IntPtr pClassPtr);

        [DllImport("Cpp_Dll.dll")]
        private static extern bool ClassTest_Zoo_AddAnimal(DNI.DNI pDni, IntPtr pClassPtr, IntPtr pAnimal);

        [DllImport("Cpp_Dll.dll")]
        [return:MarshalAs(UnmanagedType.Bool)]
        private static extern bool ClassTest_Zoo_RemoveAnimal(DNI.DNI pDni, IntPtr pClassPtr, string name);

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

        ~Zoo()
        {
            Dispose();
        }

        public override void Dispose()
        {
            deleteNativeObject();
        }

        private void deleteNativeObject()
        {
            if (ProxyHandle != IntPtr.Zero)
            {
                using (DNIHelper helper = new DNIHelper())
                {
                    ProxyHandle = ClassTest_Zoo_Destructor(helper.DNIInstance, ProxyHandle);
                }
                ProxyHandle = IntPtr.Zero;
            }

        }

        // all functions
        public int GetAnimalCount()
        {
            using (DNIHelper helper = new DNIHelper())
            {
                return ClassTest_Zoo_GetAnimalCount(helper.DNIInstance, ProxyHandle);
            }
        }
        public bool AddAnimal(Animal pAnimal)
        {
            using (DNIHelper helper = new DNIHelper())
            {
                return ClassTest_Zoo_AddAnimal(helper.DNIInstance, ProxyHandle, pAnimal.ProxyHandle);
            }
        }
        public bool RemoveAnimal(string name)
        {
            using (DNIHelper helper = new DNIHelper())
            {
                return ClassTest_Zoo_RemoveAnimal(helper.DNIInstance, ProxyHandle, name);
            }
        }

        // ideally it should be a factory, but for now will do it here
        public Animal CreateAnimal(AnimalTypes type, string name)
        {
            using (DNIHelper helper = new DNIHelper())
            {
                return new Animal(ClassTest_Zoo_CreateAnimal(helper.DNIInstance, ProxyHandle, (int)type, name));
            }
        }

    }
}
