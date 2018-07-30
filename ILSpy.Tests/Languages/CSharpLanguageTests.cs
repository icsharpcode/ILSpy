using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.ILSpy;
using NUnit.Framework;

namespace ILSpy.Tests.Languages
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class CSharpLanguageTests
	{
		const string ns = "ICSharpCode.Decompiler.Tests.TypeSystem";

		static PEFile LoadAssembly(string filename)
		{
			return new PEFile(filename, new FileStream(filename, FileMode.Open, FileAccess.Read));
		}

		static readonly Lazy<PEFile> mscorlib = new Lazy<PEFile>(
			delegate {
				return LoadAssembly(typeof(object).Assembly.Location);
			});

		static readonly Lazy<PEFile> systemCore = new Lazy<PEFile>(
			delegate {
				return LoadAssembly(typeof(System.Linq.Enumerable).Assembly.Location);
			});

		static readonly Lazy<PEFile> testAssembly = new Lazy<PEFile>(
			delegate {
				return LoadAssembly(typeof(CSharpLanguageTests).Assembly.Location);
			});

		public static PEFile Mscorlib { get { return mscorlib.Value; } }
		public static PEFile SystemCore { get { return systemCore.Value; } }
		public static PEFile TestAssembly { get { return testAssembly.Value; } }

		[OneTimeSetUp]
		public void FixtureSetUp()
		{
			compilation = new SimpleCompilation(TestAssembly,
				Mscorlib.WithOptions(TypeSystemOptions.Default));
			language = new CSharpLanguage();
		}

		ICompilation compilation;
		CSharpLanguage language;

		ITypeDefinition GetTypeDefinition(Type type)
		{
			return compilation.FindType(type).GetDefinition();
		}

		void TestType(Type t, string ns, string name)
		{
			var type = GetTypeDefinition(t);
			Assert.AreEqual(name, language.TypeToString(type, includeNamespace: false));
			Assert.AreEqual(ns + "." + name, language.TypeToString(type, includeNamespace: true));
		}

		void TestMethod(Type t, Predicate<IMember> filter, string ns, string typeName, string name, string paramListReturnType, string longParamListReturnType = null)
		{
			var type = GetTypeDefinition(t);
			var method = type.GetMembers(filter, GetMemberOptions.IgnoreInheritedMembers).Single() as IMethod;
			if (method == null)
				throw new ArgumentNullException();
			if (longParamListReturnType == null)
				longParamListReturnType = paramListReturnType;
			Assert.AreEqual(name + paramListReturnType, language.MethodToString(method, includeDeclaringTypeName: false, includeNamespace: false, includeNamespaceOfDeclaringTypeName: false));
			Assert.AreEqual(typeName + "." + name + paramListReturnType, language.MethodToString(method, includeDeclaringTypeName: true, includeNamespace: false, includeNamespaceOfDeclaringTypeName: false));
			Assert.AreEqual(name + longParamListReturnType, language.MethodToString(method, includeDeclaringTypeName: false, includeNamespace: true, includeNamespaceOfDeclaringTypeName: false));
			Assert.AreEqual(typeName + "." + name + longParamListReturnType, language.MethodToString(method, includeDeclaringTypeName: true, includeNamespace: true, includeNamespaceOfDeclaringTypeName: false));
			Assert.AreEqual(name + paramListReturnType, language.MethodToString(method, includeDeclaringTypeName: false, includeNamespace: false, includeNamespaceOfDeclaringTypeName: true));
			Assert.AreEqual(ns + "." + typeName + "." + name + paramListReturnType, language.MethodToString(method, includeDeclaringTypeName: true, includeNamespace: false, includeNamespaceOfDeclaringTypeName: true));
			Assert.AreEqual(name + longParamListReturnType, language.MethodToString(method, includeDeclaringTypeName: false, includeNamespace: true, includeNamespaceOfDeclaringTypeName: true));
			Assert.AreEqual(ns + "." + typeName + "." + name + longParamListReturnType, language.MethodToString(method, includeDeclaringTypeName: true, includeNamespace: true, includeNamespaceOfDeclaringTypeName: true));
		}

		[Test]
		public void PrimitiveTypes()
		{
			TestType(typeof(object), "System", "Object");
			TestType(typeof(string), "System", "String");
			TestType(typeof(int), "System", "Int32");
		}

		[Test]
		public void ClassTests()
		{
			TestType(typeof(SimplePublicClass), ns, "SimplePublicClass");
			TestType(typeof(GenericClass<,>), ns, "GenericClass<A,B>");
			TestType(typeof(OuterGeneric<>), ns, "OuterGeneric<X>");
			TestType(typeof(OuterGeneric<>.Inner), ns + ".OuterGeneric<X>", "Inner");
		}

		[Test]
		public void InterfaceTests()
		{
			TestType(typeof(IBase1), ns, "IBase1");
			TestType(typeof(IGenericInterface<>), ns, "IGenericInterface<T>");
		}

		[Test]
		public void EnumTests()
		{
			TestType(typeof(MyEnum), ns, "MyEnum");
			TestType(typeof(GenericClass<,>.NestedEnum), ns + ".GenericClass<A,B>", "NestedEnum");
		}

		[Test]
		public void DelegateTests()
		{
			TestType(typeof(GenericDelegate<,>), ns, "GenericDelegate<T,S>");
		}

		[Test]
		public void MethodTests()
		{
			TestMethod(typeof(IMarshalAsTests), x => x.Name == "QueryApplicationFile", ns, "IMarshalAsTests", "QueryApplicationFile", "(string, out string, out string, out bool, out bool, out object[]) : void");
			TestMethod(typeof(MyClassWithCtor), x => x is IMethod m && m.IsConstructor, ns, "MyClassWithCtor", "MyClassWithCtor", "(int)");
			TestMethod(typeof(OuterGeneric<>), x => x is IMethod m && m.IsConstructor, ns, "OuterGeneric<X>", "OuterGeneric<X>", "()");
		}
	}
}
