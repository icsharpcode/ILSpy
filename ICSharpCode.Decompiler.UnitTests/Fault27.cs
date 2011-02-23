using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ICSharpCode.Decompiler
{
    /// <summary>
    ///   passing null to a method/constructor that has multiple signatures 
    /// </summary>
    /// <remarks>
    ///   When the method has multiple signatures; such as foo(MyObject o)  and foo(string s), then code calling for with a null argument must have a cast.  
    ///   Currently ILSpy will display foo(null) and it should be foo((string) null)  or foo((MyObject) null).
    ///   
    ///  I found this while looking at the .ctor for System.Xml.Resolvers.XmlPreloadedResolver.
    /// </remarks>
    [TestClass]
    public class Fault27 : DecompilerTest
    {
        public class Sample
        {
            public void Foo(Sample o) { }
            public void Foo(string s) { }
            public void Foo(int v, string s) { }
            public void Foo(float v, string s) { }
            public void Bar(Sample o) { }

            void Call1() { Foo((Sample)null); }
            void Call2() { Foo((string)null); }
            void Call3() { Foo(1, null); }
            void Call4() { Foo(1.0f, null); }
            void Call5() { Bar(null); }
        }

        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual("private void Call1(){this.Foo((Fault27.Sample)null);}", DecompileMethod(typeof(Sample), "Call1"));
            Assert.AreEqual("private void Call2(){this.Foo((string)null);}", DecompileMethod(typeof(Sample), "Call2"));
            Assert.AreEqual("private void Call3(){this.Foo(1, null);}", DecompileMethod(typeof(Sample), "Call3"));
            Assert.AreEqual("private void Call4(){this.Foo(1f, null);}", DecompileMethod(typeof(Sample), "Call4"));
            Assert.AreEqual("private void Call5(){this.Bar(null);}", DecompileMethod(typeof(Sample), "Call5"));
        }
    }
}
