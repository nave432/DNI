using DNI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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

        [TestMethod]
        public void callStringFunction()
        {
            using (DNIHelper dNIHelper = new DNIHelper())
            {
                string str = stringFunction(dNIHelper.DNIInstance, "abcd");
                Assert.AreEqual(str, "abcd");
            }

        }

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
    }
}