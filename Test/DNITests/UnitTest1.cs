using DNI;
using DNITests.ProxyClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DNITests
{
    [TestClass]
    public class UnitTest1
    {
        [DllImport("Cpp_Dll.dll")]
        public static extern void voidFunction();

        [DllImport("Cpp_Dll.dll")]
        public static extern int intFunction(int i);

        [DllImport("Cpp_Dll.dll")]
        public static extern double doubleFunction(double d);

        [DllImport("Cpp_Dll.dll")]
        public static extern char charFunction(char c);

        [DllImport("Cpp_Dll.dll")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(DNI.Marshaler.NativeToManagedMarshaler))]
        public static extern int[] intArrayFunction(DNI.DNI pDni,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(DNI.Marshaler.ManagedToNativeMarshaler))]
            int[] c);

        [DllImport("Cpp_Dll.dll", CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(DNI.Marshaler.NativeToManagedMarshaler))]
        public static extern string stringFunction(DNI.DNI pDni, string str);


        [DllImport("Cpp_Dll.dll")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(DNI.Marshaler.NativeToManagedMarshaler))]
        public static extern Dictionary<string,string> functionTakingDictionary(DNI.DNI pDni,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(DNI.Marshaler.ManagedToNativeMarshaler))]
            Dictionary<string,int> input);


        [TestMethod]
        public void callVoidFunction()
        {
            voidFunction();
        }

        [TestMethod]
        public void callPODTypeFunction()
        {
            Assert.AreEqual(10, intFunction(10));
            Assert.AreEqual(105, (int)(doubleFunction(10.5) * 10));
            Assert.AreEqual('x', charFunction('x'));
        }

        //[TestMethod]
        //public void callStringFunction()
        //{
        //    using (DNIHelper dNIHelper = new DNIHelper())
        //    {
        //        string str = stringFunction(dNIHelper.DNIInstance, "abcd");
        //        Assert.AreEqual(str, "abcd");
        //    }

        //}

        [TestMethod]
        public void callIntArrayFunction()
        {
            using (DNIHelper dNIHelper = new DNIHelper())
            {
                int[] inputArray = new int[] { 5,6,7,8 };
                int[] retArray = intArrayFunction(dNIHelper.DNIInstance, inputArray);
                for (int i = 0; i < inputArray.Length; ++i)
                {
                    Assert.AreEqual(inputArray[i], retArray[i]);
                }
            }

        }

        [TestMethod]
        public void TestPassingDictionary()
        {
            using (DNIHelper dNIHelper = new DNIHelper())
            {
                Dictionary<string, int> dict = new Dictionary<string, int>()
                {
                    { "person1", 1},
                    { "person2", 2},
                    { "person3", 3},
                    { "person4", 4}
                };

                var ret = functionTakingDictionary(dNIHelper.DNIInstance, dict);
                foreach (var item in ret)
                {
                    Assert.IsTrue(dict.TryGetValue(item.Key, out int value));
                    Assert.AreEqual(value.ToString(), item.Value);
                }
            }

        }

        [TestMethod]
        public void TestClassConstructionAndDestruction()
        {
            using (Zoo zoo = new Zoo())
            {
                Assert.AreNotEqual( zoo.ProxyHandle, IntPtr.Zero );
                Assert.AreEqual(zoo.GetAnimalCount(), 0 );
                Animal animal1 = zoo.CreateAnimal(AnimalTypes.Cat, "meow");
                zoo.AddAnimal(animal1);
                Assert.AreEqual(zoo.GetAnimalCount(), 1);

                Animal animal2 = zoo.CreateAnimal(AnimalTypes.Dog, "bowbow");
                zoo.AddAnimal(animal2);
                Assert.AreEqual(zoo.GetAnimalCount(), 2);

                zoo.RemoveAnimal("meow");
                Assert.AreEqual(zoo.GetAnimalCount(), 1);

                zoo.RemoveAnimal("bowbow");
                Assert.AreEqual(zoo.GetAnimalCount(), 0);
            }
        }
    }
}