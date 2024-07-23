// Copyright (c) 2010-2018 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.TypeSystem
{
	using AttributeArray = ImmutableArray<CustomAttributeTypedArgument<IType>>;

	[TestFixture]
	public class TypeSystemLoaderTests
	{
		static PEFile LoadAssembly(string filename)
		{
			return new PEFile(filename, new FileStream(filename, FileMode.Open, FileAccess.Read));
		}

		static readonly Lazy<PEFile> mscorlib = new Lazy<PEFile>(
			delegate {
				return LoadAssembly(Path.Combine(Helpers.Tester.RefAsmPath, "mscorlib.dll"));
			});

		static readonly Lazy<PEFile> systemCore = new Lazy<PEFile>(
			delegate {
				return LoadAssembly(Path.Combine(Helpers.Tester.RefAsmPath, "System.Core.dll"));
			});

		static readonly Lazy<PEFile> testAssembly = new Lazy<PEFile>(
			delegate {
				return LoadAssembly(typeof(SimplePublicClass).Assembly.Location);
			});

		public static PEFile Mscorlib { get { return mscorlib.Value; } }
		public static PEFile SystemCore { get { return systemCore.Value; } }
		public static PEFile TestAssembly { get { return testAssembly.Value; } }

		[OneTimeSetUp]
		public void FixtureSetUp()
		{
			compilation = new SimpleCompilation(TestAssembly,
				Mscorlib.WithOptions(TypeSystemOptions.Default | TypeSystemOptions.OnlyPublicAPI));
		}

		protected ICompilation compilation;

		protected ITypeDefinition GetTypeDefinition(Type type)
		{
			return compilation.FindType(type).GetDefinition();
		}

		[Test]
		public void SimplePublicClassTest()
		{
			ITypeDefinition c = GetTypeDefinition(typeof(SimplePublicClass));
			Assert.That(c.Name, Is.EqualTo(typeof(SimplePublicClass).Name));
			Assert.That(c.FullName, Is.EqualTo(typeof(SimplePublicClass).FullName));
			Assert.That(c.Namespace, Is.EqualTo(typeof(SimplePublicClass).Namespace));
			Assert.That(c.ReflectionName, Is.EqualTo(typeof(SimplePublicClass).FullName));

			Assert.That(c.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(!c.IsAbstract);
			Assert.That(!c.IsSealed);
			Assert.That(!c.IsStatic);
			Assert.That(!c.HasAttribute(KnownAttribute.SpecialName));
		}

		[Test]
		public void SimplePublicClassMethodTest()
		{
			ITypeDefinition c = GetTypeDefinition(typeof(SimplePublicClass));

			IMethod method = c.Methods.Single(m => m.Name == "Method");
			Assert.That(method.FullName, Is.EqualTo(typeof(SimplePublicClass).FullName + ".Method"));
			Assert.That(method.DeclaringType, Is.SameAs(c));
			Assert.That(method.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(method.SymbolKind, Is.EqualTo(SymbolKind.Method));
			Assert.That(!method.IsVirtual);
			Assert.That(!method.IsStatic);
			Assert.That(method.Parameters.Count, Is.EqualTo(0));
			Assert.That(method.GetAttributes().Count(), Is.EqualTo(0));
			Assert.That(method.HasBody);
			Assert.That(method.AccessorOwner, Is.Null);
			Assert.That(!method.HasAttribute(KnownAttribute.SpecialName));
		}

		[Test]
		public void SimplePublicClassCtorTest()
		{
			ITypeDefinition c = GetTypeDefinition(typeof(SimplePublicClass));

			IMethod method = c.Methods.Single(m => m.IsConstructor);
			Assert.That(method.FullName, Is.EqualTo(typeof(SimplePublicClass).FullName + "..ctor"));
			Assert.That(method.DeclaringType, Is.SameAs(c));
			Assert.That(method.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(method.SymbolKind, Is.EqualTo(SymbolKind.Constructor));
			Assert.That(!method.IsVirtual);
			Assert.That(!method.IsStatic);
			Assert.That(method.Parameters.Count, Is.EqualTo(0));
			Assert.That(method.GetAttributes().Count(), Is.EqualTo(0));
			Assert.That(method.HasBody);
			Assert.That(method.AccessorOwner, Is.Null);
			Assert.That(!method.HasAttribute(KnownAttribute.SpecialName));
		}

		[Test]
		public void SimplePublicClassDtorTest()
		{
			ITypeDefinition c = GetTypeDefinition(typeof(SimplePublicClass));

			IMethod method = c.Methods.Single(m => m.IsDestructor);
			Assert.That(method.FullName, Is.EqualTo(typeof(SimplePublicClass).FullName + ".Finalize"));
			Assert.That(method.DeclaringType, Is.SameAs(c));
			Assert.That(method.Accessibility, Is.EqualTo(Accessibility.Protected));
			Assert.That(method.SymbolKind, Is.EqualTo(SymbolKind.Destructor));
			Assert.That(!method.IsVirtual);
			Assert.That(!method.IsStatic);
			Assert.That(method.Parameters.Count, Is.EqualTo(0));
			Assert.That(method.GetAttributes().Count(), Is.EqualTo(1));
			Assert.That(method.HasBody);
			Assert.That(method.AccessorOwner, Is.Null);
			Assert.That(!method.HasAttribute(KnownAttribute.SpecialName));
		}

		[Test]
		public void DynamicType()
		{
			ITypeDefinition testClass = GetTypeDefinition(typeof(DynamicTest));
			Assert.That(testClass.Fields.Single(f => f.Name == "DynamicField").ReturnType, Is.EqualTo(SpecialType.Dynamic));
			Assert.That(testClass.Properties.Single().ReturnType, Is.EqualTo(SpecialType.Dynamic));
			Assert.That(testClass.Properties.Single().GetAttributes().Count(), Is.EqualTo(0));
		}

		[Test]
		public void DynamicTypeInGenerics()
		{
			ITypeDefinition testClass = GetTypeDefinition(typeof(DynamicTest));

			IMethod m1 = testClass.Methods.Single(me => me.Name == "DynamicGenerics1");
			Assert.That(m1.ReturnType.ReflectionName, Is.EqualTo("System.Collections.Generic.List`1[[dynamic]]"));
			Assert.That(m1.Parameters[0].Type.ReflectionName, Is.EqualTo("System.Action`3[[System.Object],[dynamic[]],[System.Object]]"));

			IMethod m2 = testClass.Methods.Single(me => me.Name == "DynamicGenerics2");
			Assert.That(m2.Parameters[0].Type.ReflectionName, Is.EqualTo("System.Action`3[[System.Object],[dynamic],[System.Object]]"));

			IMethod m3 = testClass.Methods.Single(me => me.Name == "DynamicGenerics3");
			Assert.That(m3.Parameters[0].Type.ReflectionName, Is.EqualTo("System.Action`3[[System.Int32],[dynamic],[System.Object]]"));

			IMethod m4 = testClass.Methods.Single(me => me.Name == "DynamicGenerics4");
			Assert.That(m4.Parameters[0].Type.ReflectionName, Is.EqualTo("System.Action`3[[System.Int32[]],[dynamic],[System.Object]]"));

			IMethod m5 = testClass.Methods.Single(me => me.Name == "DynamicGenerics5");
			Assert.That(m5.Parameters[0].Type.ReflectionName, Is.EqualTo("System.Action`3[[System.Int32*[]],[dynamic],[System.Object]]"));

			IMethod m6 = testClass.Methods.Single(me => me.Name == "DynamicGenerics6");
			Assert.That(m6.Parameters[0].Type.ReflectionName, Is.EqualTo("System.Action`3[[System.Object],[dynamic],[System.Object]]&"));

			IMethod m7 = testClass.Methods.Single(me => me.Name == "DynamicGenerics7");
			Assert.That(m7.Parameters[0].Type.ReflectionName, Is.EqualTo("System.Action`3[[System.Int32[][,]],[dynamic],[System.Object]]"));
		}

		[Test]
		public void DynamicParameterHasNoAttributes()
		{
			ITypeDefinition testClass = GetTypeDefinition(typeof(DynamicTest));
			IMethod m1 = testClass.Methods.Single(me => me.Name == "DynamicGenerics1");
			Assert.That(m1.Parameters[0].GetAttributes().Count(), Is.EqualTo(0));
		}

		[Test]
		public void AssemblyAttribute()
		{
			var attributes = compilation.MainModule.GetAssemblyAttributes().ToList();
			var typeTest = attributes.Single(a => a.AttributeType.FullName == typeof(TypeTestAttribute).FullName);
			Assert.That(typeTest.FixedArguments.Length, Is.EqualTo(3));
			// first argument is (int)42
			Assert.That((int)typeTest.FixedArguments[0].Value, Is.EqualTo(42));
			// second argument is typeof(System.Action<>)
			var ty = (IType)typeTest.FixedArguments[1].Value;
			Assert.That(ty is ParameterizedType, Is.False); // rt must not be constructed - it's just an unbound type
			Assert.That(ty.FullName, Is.EqualTo("System.Action"));
			Assert.That(ty.TypeParameterCount, Is.EqualTo(1));
			// third argument is typeof(IDictionary<string, IList<TestAttribute>>)
			var crt = (ParameterizedType)typeTest.FixedArguments[2].Value;
			Assert.That(crt.FullName, Is.EqualTo("System.Collections.Generic.IDictionary"));
			Assert.That(crt.TypeArguments[0].FullName, Is.EqualTo("System.String"));
			// we know the name for TestAttribute, but not necessarily the namespace, as NUnit is not in the compilation
			Assert.That(crt.TypeArguments[1].FullName, Is.EqualTo("System.Collections.Generic.IList"));
			var testAttributeType = ((ParameterizedType)crt.TypeArguments[1]).TypeArguments.Single();
			Assert.That(testAttributeType.Name, Is.EqualTo("TestAttribute"));
			Assert.That(testAttributeType.Kind, Is.EqualTo(TypeKind.Unknown));
			// (more accurately, we know the namespace and reflection name if the type was loaded by cecil,
			// but not if we parsed it from C#)
		}

		[Test]
		public void TypeForwardedTo_Attribute()
		{
			var attributes = compilation.MainModule.GetAssemblyAttributes().ToList();
			var forwardAttribute = attributes.Single(a => a.AttributeType.FullName == typeof(TypeForwardedToAttribute).FullName);
			Assert.That(forwardAttribute.FixedArguments.Length, Is.EqualTo(1));
			var rt = (IType)forwardAttribute.FixedArguments[0].Value;
			Assert.That(rt.ReflectionName, Is.EqualTo("System.Func`2"));
		}

		[Test]
		public void TestClassTypeParameters()
		{
			var testClass = GetTypeDefinition(typeof(GenericClass<,>));
			Assert.That(testClass.TypeParameters[0].OwnerType, Is.EqualTo(SymbolKind.TypeDefinition));
			Assert.That(testClass.TypeParameters[1].OwnerType, Is.EqualTo(SymbolKind.TypeDefinition));
			Assert.That(testClass.TypeParameters[0].DirectBaseTypes.First(), Is.SameAs(testClass.TypeParameters[1]));
		}

		[Test]
		public void TestMethod()
		{
			var testClass = GetTypeDefinition(typeof(GenericClass<,>));

			IMethod m = testClass.Methods.Single(me => me.Name == "TestMethod");
			Assert.That(m.TypeParameters[0].Name, Is.EqualTo("K"));
			Assert.That(m.TypeParameters[1].Name, Is.EqualTo("V"));
			Assert.That(m.TypeParameters[0].OwnerType, Is.EqualTo(SymbolKind.Method));
			Assert.That(m.TypeParameters[1].OwnerType, Is.EqualTo(SymbolKind.Method));

			Assert.That(m.TypeParameters[0].DirectBaseTypes.First().ReflectionName, Is.EqualTo("System.IComparable`1[[``1]]"));
			Assert.That(m.TypeParameters[1].DirectBaseTypes.First(), Is.SameAs(m.TypeParameters[0]));
		}

		[Test]
		public void GetIndex()
		{
			var testClass = GetTypeDefinition(typeof(GenericClass<,>));

			IMethod m = testClass.Methods.Single(me => me.Name == "GetIndex");
			Assert.That(m.TypeParameters[0].Name, Is.EqualTo("T"));
			Assert.That(m.TypeParameters[0].OwnerType, Is.EqualTo(SymbolKind.Method));
			Assert.That(m.TypeParameters[0].Owner, Is.SameAs(m));

			ParameterizedType constraint = (ParameterizedType)m.TypeParameters[0].DirectBaseTypes.First();
			Assert.That(constraint.Name, Is.EqualTo("IEquatable"));
			Assert.That(constraint.TypeParameterCount, Is.EqualTo(1));
			Assert.That(constraint.TypeArguments.Count, Is.EqualTo(1));
			Assert.That(constraint.TypeArguments[0], Is.SameAs(m.TypeParameters[0]));
			Assert.That(m.Parameters[0].Type, Is.SameAs(m.TypeParameters[0]));
		}

		[Test]
		public void GetIndexSpecializedTypeParameter()
		{
			var testClass = GetTypeDefinition(typeof(GenericClass<,>));
			var methodDef = testClass.Methods.Single(me => me.Name == "GetIndex");
			var m = methodDef.Specialize(new TypeParameterSubstitution(
				new[] { compilation.FindType(KnownTypeCode.Int16), compilation.FindType(KnownTypeCode.Int32) },
				null
			));

			Assert.That(m.TypeParameters[0].Name, Is.EqualTo("T"));
			Assert.That(m.TypeParameters[0].OwnerType, Is.EqualTo(SymbolKind.Method));
			Assert.That(m.TypeParameters[0].Owner, Is.SameAs(m));

			ParameterizedType constraint = (ParameterizedType)m.TypeParameters[0].DirectBaseTypes.First();
			Assert.That(constraint.Name, Is.EqualTo("IEquatable"));
			Assert.That(constraint.TypeParameterCount, Is.EqualTo(1));
			Assert.That(constraint.TypeArguments.Count, Is.EqualTo(1));
			Assert.That(constraint.TypeArguments[0], Is.SameAs(m.TypeParameters[0]));
			Assert.That(m.Parameters[0].Type, Is.SameAs(m.TypeParameters[0]));
		}

		[Test]
		public void GetIndexDoubleSpecialization()
		{
			var testClass = GetTypeDefinition(typeof(GenericClass<,>));
			// GenericClass<A, B>.GetIndex<T>
			var methodDef = testClass.Methods.Single(me => me.Name == "GetIndex");

			// GenericClass<B, A>.GetIndex<A>
			var m1 = methodDef.Specialize(new TypeParameterSubstitution(
				new[] { testClass.TypeParameters[1], testClass.TypeParameters[0] },
				new[] { testClass.TypeParameters[0] }
			));
			// GenericClass<string, int>.GetIndex<int>
			var m2 = m1.Specialize(new TypeParameterSubstitution(
				new[] { compilation.FindType(KnownTypeCode.Int32), compilation.FindType(KnownTypeCode.String) },
				null
			));

			// GenericClass<string, int>.GetIndex<int>
			var m12 = methodDef.Specialize(new TypeParameterSubstitution(
				new[] { compilation.FindType(KnownTypeCode.String), compilation.FindType(KnownTypeCode.Int32) },
				new[] { compilation.FindType(KnownTypeCode.Int32) }
			));
			Assert.That(m2, Is.EqualTo(m12));
		}

		[Test]
		public void SpecializedMethod_AccessorOwner()
		{
			// NRefactory bug #143 - Accessor Owner throws null reference exception in some cases now
			var method = compilation.FindType(typeof(GenericClass<string, object>)).GetMethods(m => m.Name == "GetIndex").Single();
			Assert.That(method.AccessorOwner, Is.Null);
		}

		[Test]
		public void Specialized_GetIndex_ToMemberReference()
		{
			var method = compilation.FindType(typeof(GenericClass<string, object>)).GetMethods(m => m.Name == "GetIndex").Single();
			Assert.That(method.Parameters[0].Type, Is.SameAs(method.TypeParameters[0]));
			Assert.That(method.TypeParameters[0].Owner, Is.SameAs(method));
			Assert.That(method, Is.InstanceOf<SpecializedMethod>());
			//Assert.That(!method.IsParameterized); // the method itself is not specialized
			Assert.That(method.TypeArguments, Is.EqualTo(method.TypeParameters));
		}

		[Test]
		public void Specialized_GetIndex_SpecializeWithIdentityHasNoEffect()
		{
			var genericClass = compilation.FindType(typeof(GenericClass<string, object>));
			IType[] methodTypeArguments = { DummyTypeParameter.GetMethodTypeParameter(0) };
			var method = genericClass.GetMethods(methodTypeArguments, m => m.Name == "GetIndex").Single();
			// GenericClass<string,object>.GetIndex<!!0>()
			Assert.That(method.TypeParameters[0].Owner, Is.SameAs(method));
			Assert.That(method.TypeArguments[0], Is.Not.EqualTo(method.TypeParameters[0]));
			Assert.That(((ITypeParameter)method.TypeArguments[0]).Owner, Is.Null);
			// Now apply identity substitution:
			var method2 = method.Specialize(TypeParameterSubstitution.Identity);
			Assert.That(method2.TypeParameters[0].Owner, Is.SameAs(method2));
			Assert.That(method2.TypeArguments[0], Is.Not.EqualTo(method2.TypeParameters[0]));
			Assert.That(((ITypeParameter)method2.TypeArguments[0]).Owner, Is.Null);

			Assert.That(method2, Is.EqualTo(method));
		}

		[Test]
		public void GenericEnum()
		{
			var testClass = GetTypeDefinition(typeof(GenericClass<,>.NestedEnum));
			Assert.That(testClass.TypeParameterCount, Is.EqualTo(2));
		}

		[Test]
		public void FieldInGenericClassWithNestedEnumType()
		{
			var testClass = GetTypeDefinition(typeof(GenericClass<,>));
			var enumClass = GetTypeDefinition(typeof(GenericClass<,>.NestedEnum));
			var field = testClass.Fields.Single(f => f.Name == "EnumField");
			Assert.That(field.ReturnType, Is.EqualTo(new ParameterizedType(enumClass, testClass.TypeParameters)));
		}

		[Test]
		public void GenericEnumMemberReturnType()
		{
			var enumClass = GetTypeDefinition(typeof(GenericClass<,>.NestedEnum));
			var field = enumClass.Fields.Single(f => f.Name == "EnumMember");
			Assert.That(field.ReturnType, Is.EqualTo(new ParameterizedType(enumClass, enumClass.TypeParameters)));
		}

		[Test]
		public void PropertyWithProtectedSetter()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.Name == "PropertyWithProtectedSetter");
			Assert.That(p.CanGet);
			Assert.That(p.CanSet);
			Assert.That(p.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Getter.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Setter.Accessibility, Is.EqualTo(Accessibility.Protected));
		}

		[Test]
		public void PropertyWithPrivateSetter()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.Name == "PropertyWithPrivateSetter");
			Assert.That(p.CanGet);
			Assert.That(p.CanSet);
			Assert.That(p.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Getter.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Setter.Accessibility, Is.EqualTo(Accessibility.Private));
			Assert.That(p.Getter.HasBody);
			Assert.That(!p.HasAttribute(KnownAttribute.SpecialName));
			Assert.That(!p.Getter.HasAttribute(KnownAttribute.SpecialName));
			Assert.That(!p.Setter.HasAttribute(KnownAttribute.SpecialName));
		}

		[Test]
		public void PropertyWithPrivateGetter()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.Name == "PropertyWithPrivateGetter");
			Assert.That(p.CanGet);
			Assert.That(p.CanSet);
			Assert.That(p.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Getter.Accessibility, Is.EqualTo(Accessibility.Private));
			Assert.That(p.Setter.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Getter.HasBody);
		}

		[Test]
		public void PropertyWithoutSetter()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.Name == "PropertyWithoutSetter");
			Assert.That(p.CanGet);
			Assert.That(!p.CanSet);
			Assert.That(p.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Getter.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Setter, Is.Null);
		}

		[Test]
		public void Indexer()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.IsIndexer);
			Assert.That(p.Name, Is.EqualTo("Item"));
			Assert.That(p.Parameters.Select(x => x.Name).ToArray(), Is.EqualTo(new[] { "index" }));
		}

		[Test]
		public void IndexerGetter()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.IsIndexer);
			Assert.That(p.CanGet);
			Assert.That(p.Getter.SymbolKind, Is.EqualTo(SymbolKind.Accessor));
			Assert.That(p.Getter.Name, Is.EqualTo("get_Item"));
			Assert.That(p.Getter.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Getter.Parameters.Select(x => x.Name).ToArray(), Is.EqualTo(new[] { "index" }));
			Assert.That(p.Getter.ReturnType.ReflectionName, Is.EqualTo("System.String"));
			Assert.That(p.Getter.AccessorOwner, Is.EqualTo(p));
		}

		[Test]
		public void IndexerSetter()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.IsIndexer);
			Assert.That(p.CanSet);
			Assert.That(p.Setter.SymbolKind, Is.EqualTo(SymbolKind.Accessor));
			Assert.That(p.Setter.Name, Is.EqualTo("set_Item"));
			Assert.That(p.Setter.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Setter.Parameters.Select(x => x.Name).ToArray(), Is.EqualTo(new[] { "index", "value" }));
			Assert.That(p.Setter.ReturnType.Kind, Is.EqualTo(TypeKind.Void));
		}

		[Test]
		public void GenericPropertyGetter()
		{
			var type = compilation.FindType(typeof(GenericClass<string, object>));
			var prop = type.GetProperties(p => p.Name == "Property").Single();
			Assert.That(prop.Getter.ReturnType.ReflectionName, Is.EqualTo("System.String"));
			Assert.That(prop.Getter.IsAccessor);
			Assert.That(prop.Getter.AccessorOwner, Is.EqualTo(prop));
		}

		[Test]
		public void EnumTest()
		{
			var e = GetTypeDefinition(typeof(MyEnum));
			Assert.That(e.Kind, Is.EqualTo(TypeKind.Enum));
			Assert.That(e.IsReferenceType, Is.EqualTo(false));
			Assert.That(e.EnumUnderlyingType.ReflectionName, Is.EqualTo("System.Int16"));
			Assert.That(e.DirectBaseTypes.Select(t => t.ReflectionName).ToArray(), Is.EqualTo(new[] { "System.Enum" }));
		}

		[Test]
		public void EnumFieldsTest()
		{
			var e = GetTypeDefinition(typeof(MyEnum));
			IField valueField = e.Fields.First();
			IField[] fields = e.Fields.Skip(1).ToArray();
			Assert.That(fields.Length, Is.EqualTo(5));

			Assert.That(valueField.Name, Is.EqualTo("value__"));
			Assert.That(valueField.Type, Is.EqualTo(GetTypeDefinition(typeof(short))));
			Assert.That(valueField.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(valueField.GetConstantValue(), Is.EqualTo(null));
			Assert.That(!valueField.IsConst);
			Assert.That(!valueField.IsStatic);

			foreach (IField f in fields)
			{
				Assert.That(f.IsStatic);
				Assert.That(f.IsConst);
				Assert.That(f.Accessibility, Is.EqualTo(Accessibility.Public));
				Assert.That(f.Type, Is.SameAs(e));
				Assert.That(f.GetConstantValue().GetType(), Is.EqualTo(typeof(short)));
			}

			Assert.That(fields[0].Name, Is.EqualTo("First"));
			Assert.That(fields[0].GetConstantValue(), Is.EqualTo(0));

			Assert.That(fields[1].Name, Is.EqualTo("Second"));
			Assert.That(fields[1].Type, Is.SameAs(e));
			Assert.That(fields[1].GetConstantValue(), Is.EqualTo(1));

			Assert.That(fields[2].Name, Is.EqualTo("Flag1"));
			Assert.That(fields[2].GetConstantValue(), Is.EqualTo(0x10));

			Assert.That(fields[3].Name, Is.EqualTo("Flag2"));
			Assert.That(fields[3].GetConstantValue(), Is.EqualTo(0x20));

			Assert.That(fields[4].Name, Is.EqualTo("CombinedFlags"));
			Assert.That(fields[4].GetConstantValue(), Is.EqualTo(0x30));
		}

		[Test]
		public void GetNestedTypesFromBaseClassTest()
		{
			ITypeDefinition d = GetTypeDefinition(typeof(Derived<,>));

			IType pBase = d.DirectBaseTypes.Single();
			Assert.That(pBase.ReflectionName, Is.EqualTo(typeof(Base<>).FullName + "[[`1]]"));
			// Base[`1].GetNestedTypes() = { Base`1+Nested`1[`1, unbound] }
			Assert.That(pBase.GetNestedTypes().Select(n => n.ReflectionName).ToArray(), Is.EqualTo(new[] { typeof(Base<>.Nested<>).FullName + "[[`1],[]]" }));

			// Derived.GetNestedTypes() = { Base`1+Nested`1[`1, unbound] }
			Assert.That(d.GetNestedTypes().Select(n => n.ReflectionName).ToArray(), Is.EqualTo(new[] { typeof(Base<>.Nested<>).FullName + "[[`1],[]]" }));
			// This is 'leaking' the type parameter from B as is usual when retrieving any members from an unbound type.
		}

		[Test]
		public void ParameterizedTypeGetNestedTypesFromBaseClassTest()
		{
			// Derived[string,int].GetNestedTypes() = { Base`1+Nested`1[int, unbound] }
			var d = compilation.FindType(typeof(Derived<string, int>));
			Assert.That(d.GetNestedTypes().Select(n => n.ReflectionName).ToArray(), Is.EqualTo(new[] { typeof(Base<>.Nested<>).FullName + "[[System.Int32],[]]" }));
		}

		[Test]
		public void ConstraintsOnOverrideAreInherited()
		{
			ITypeDefinition d = GetTypeDefinition(typeof(Derived<,>));
			ITypeParameter tp = d.Methods.Single(m => m.Name == "GenericMethodWithConstraints").TypeParameters.Single();
			Assert.That(tp.Name, Is.EqualTo("Y"));
			Assert.That(!tp.HasValueTypeConstraint);
			Assert.That(!tp.HasReferenceTypeConstraint);
			Assert.That(tp.HasDefaultConstructorConstraint);
			Assert.That(tp.DirectBaseTypes.Select(t => t.ReflectionName).ToArray(), Is.EqualTo(new string[] { "System.Collections.Generic.IComparer`1[[`1]]", "System.Object" }));
		}

		[Test]
		public void DtorInDerivedClass()
		{
			ITypeDefinition c = GetTypeDefinition(typeof(Derived<,>));
			IMethod method = c.Methods.Single(m => m.IsDestructor);
			Assert.That(method.FullName, Is.EqualTo(c.FullName + ".Finalize"));
			Assert.That(method.DeclaringType, Is.SameAs(c));
			Assert.That(method.Accessibility, Is.EqualTo(Accessibility.Protected));
			Assert.That(method.SymbolKind, Is.EqualTo(SymbolKind.Destructor));
			Assert.That(!method.IsVirtual);
			Assert.That(!method.IsStatic);
			Assert.That(method.Parameters.Count, Is.EqualTo(0));
			Assert.That(method.GetAttributes().Count(), Is.EqualTo(0));
			Assert.That(method.HasBody);
			Assert.That(method.AccessorOwner, Is.Null);
		}

		[Test]
		public void PrivateFinalizeMethodIsNotADtor()
		{
			ITypeDefinition c = GetTypeDefinition(typeof(TypeTestAttribute));
			IMethod method = c.Methods.Single(m => m.Name == "Finalize");
			Assert.That(method.FullName, Is.EqualTo(c.FullName + ".Finalize"));
			Assert.That(method.DeclaringType, Is.SameAs(c));
			Assert.That(method.Accessibility, Is.EqualTo(Accessibility.Private));
			Assert.That(method.SymbolKind, Is.EqualTo(SymbolKind.Method));
			Assert.That(!method.IsVirtual);
			Assert.That(!method.IsStatic);
			Assert.That(method.Parameters.Count, Is.EqualTo(0));
			Assert.That(method.GetAttributes().Count(), Is.EqualTo(0));
			Assert.That(method.HasBody);
			Assert.That(method.AccessorOwner, Is.Null);
		}

		[Test]
		public void DefaultConstructorAddedToStruct()
		{
			var ctors = compilation.FindType(typeof(MyStructWithCtor)).GetConstructors();
			Assert.That(ctors.Count(), Is.EqualTo(2));
			Assert.That(!ctors.Any(c => c.IsStatic));
			Assert.That(ctors.All(c => c.ReturnType.Kind == TypeKind.Void));
			Assert.That(ctors.All(c => c.Accessibility == Accessibility.Public));
		}

		[Test]
		public void NoDefaultConstructorAddedToStruct()
		{
			var ctors = compilation.FindType(typeof(MyStructWithDefaultCtor)).GetConstructors();
			Assert.That(ctors.Count(), Is.EqualTo(1));
			Assert.That(!ctors.Any(c => c.IsStatic));
			Assert.That(ctors.All(c => c.ReturnType.Kind == TypeKind.Void));
			Assert.That(ctors.All(c => c.Accessibility == Accessibility.Public));
		}

		[Test]
		public void NoDefaultConstructorAddedToClass()
		{
			var ctors = compilation.FindType(typeof(MyClassWithCtor)).GetConstructors();
			Assert.That(ctors.Single().Accessibility, Is.EqualTo(Accessibility.Private));
			Assert.That(ctors.Single().Parameters.Count, Is.EqualTo(1));
		}

		[Test]
		public void DefaultConstructorOnAbstractClassIsProtected()
		{
			var ctors = compilation.FindType(typeof(AbstractClass)).GetConstructors();
			Assert.That(ctors.Single().Parameters.Count, Is.EqualTo(0));
			Assert.That(ctors.Single().Accessibility, Is.EqualTo(Accessibility.Protected));
		}

		[Test]
		public void SerializableAttribute()
		{
			IAttribute attr = GetTypeDefinition(typeof(NonCustomAttributes)).GetAttributes().Single();
			Assert.That(attr.AttributeType.FullName, Is.EqualTo("System.SerializableAttribute"));
		}

		[Test]
		public void NonSerializedAttribute()
		{
			IField field = GetTypeDefinition(typeof(NonCustomAttributes)).Fields.Single(f => f.Name == "NonSerializedField");
			Assert.That(field.GetAttributes().Single().AttributeType.FullName, Is.EqualTo("System.NonSerializedAttribute"));
		}

		[Test]
		public void ExplicitStructLayoutAttribute()
		{
			IAttribute attr = GetTypeDefinition(typeof(ExplicitFieldLayoutStruct)).GetAttributes().Single();
			Assert.That(attr.AttributeType.FullName, Is.EqualTo("System.Runtime.InteropServices.StructLayoutAttribute"));
			var arg1 = attr.FixedArguments.Single();
			Assert.That(arg1.Type.FullName, Is.EqualTo("System.Runtime.InteropServices.LayoutKind"));
			Assert.That(arg1.Value, Is.EqualTo((int)LayoutKind.Explicit));

			var arg2 = attr.NamedArguments[0];
			Assert.That(arg2.Name, Is.EqualTo("CharSet"));
			Assert.That(arg2.Type.FullName, Is.EqualTo("System.Runtime.InteropServices.CharSet"));
			Assert.That(arg2.Value, Is.EqualTo((int)CharSet.Unicode));

			var arg3 = attr.NamedArguments[1];
			Assert.That(arg3.Name, Is.EqualTo("Pack"));
			Assert.That(arg3.Type.FullName, Is.EqualTo("System.Int32"));
			Assert.That(arg3.Value, Is.EqualTo(8));
		}

		[Test]
		public void FieldOffsetAttribute()
		{
			IField field = GetTypeDefinition(typeof(ExplicitFieldLayoutStruct)).Fields.Single(f => f.Name == "Field0");
			Assert.That(field.GetAttributes().Single().AttributeType.FullName, Is.EqualTo("System.Runtime.InteropServices.FieldOffsetAttribute"));
			var arg = field.GetAttributes().Single().FixedArguments.Single();
			Assert.That(arg.Type.FullName, Is.EqualTo("System.Int32"));
			Assert.That(arg.Value, Is.EqualTo(0));

			field = GetTypeDefinition(typeof(ExplicitFieldLayoutStruct)).Fields.Single(f => f.Name == "Field100");
			Assert.That(field.GetAttributes().Single().AttributeType.FullName, Is.EqualTo("System.Runtime.InteropServices.FieldOffsetAttribute"));
			arg = field.GetAttributes().Single().FixedArguments.Single();
			Assert.That(arg.Type.FullName, Is.EqualTo("System.Int32"));
			Assert.That(arg.Value, Is.EqualTo(100));
		}

		[Test]
		public void DllImportAttribute()
		{
			IMethod method = GetTypeDefinition(typeof(NonCustomAttributes)).Methods.Single(m => m.Name == "DllMethod");
			IAttribute dllImport = method.GetAttributes().Single();
			Assert.That(dllImport.AttributeType.FullName, Is.EqualTo("System.Runtime.InteropServices.DllImportAttribute"));
			Assert.That(dllImport.FixedArguments[0].Value, Is.EqualTo("unmanaged.dll"));
			Assert.That(dllImport.NamedArguments.Single().Value, Is.EqualTo((int)CharSet.Unicode));
		}

		[Test]
		public void DllImportAttributeWithPreserveSigFalse()
		{
			IMethod method = GetTypeDefinition(typeof(NonCustomAttributes)).Methods.Single(m => m.Name == "DoNotPreserveSig");
			IAttribute dllImport = method.GetAttributes().Single();
			Assert.That(dllImport.AttributeType.FullName, Is.EqualTo("System.Runtime.InteropServices.DllImportAttribute"));
			Assert.That(dllImport.FixedArguments[0].Value, Is.EqualTo("unmanaged.dll"));
			Assert.That(dllImport.NamedArguments.Single().Value, Is.EqualTo(false));
		}

		[Test]
		public void PreserveSigAttribute()
		{
			IMethod method = GetTypeDefinition(typeof(NonCustomAttributes)).Methods.Single(m => m.Name == "PreserveSigAsAttribute");
			IAttribute preserveSig = method.GetAttributes().Single();
			Assert.That(preserveSig.AttributeType.FullName, Is.EqualTo("System.Runtime.InteropServices.PreserveSigAttribute"));
			Assert.That(preserveSig.FixedArguments.Length == 0);
			Assert.That(preserveSig.NamedArguments.Length == 0);
		}

		[Test]
		public void InOutParametersOnRefMethod()
		{
			IParameter p = GetTypeDefinition(typeof(NonCustomAttributes)).Methods.Single(m => m.Name == "DllMethod").Parameters.Single();
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.Ref));
			var attr = p.GetAttributes().ToList();
			Assert.That(attr.Count, Is.EqualTo(2));
			Assert.That(attr[0].AttributeType.FullName, Is.EqualTo("System.Runtime.InteropServices.InAttribute"));
			Assert.That(attr[1].AttributeType.FullName, Is.EqualTo("System.Runtime.InteropServices.OutAttribute"));
		}

		[Test]
		public void MarshalAsAttributeOnMethod()
		{
			IMethod method = GetTypeDefinition(typeof(NonCustomAttributes)).Methods.Single(m => m.Name == "DllMethod");
			IAttribute marshalAs = method.GetReturnTypeAttributes().Single();
			Assert.That(marshalAs.FixedArguments.Single().Value, Is.EqualTo((int)UnmanagedType.Bool));
		}

		[Test]
		public void MethodWithOutParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithOutParameter").Parameters.Single();
			Assert.That(!p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.Out));
			Assert.That(p.GetAttributes().Count(), Is.EqualTo(0));
			Assert.That(p.Type.Kind == TypeKind.ByReference);
		}

		[Test]
		public void MethodWithRefParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithRefParameter").Parameters.Single();
			Assert.That(!p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.Ref));
			Assert.That(p.GetAttributes().Count(), Is.EqualTo(0));
			Assert.That(p.Type.Kind == TypeKind.ByReference);
		}

		[Test]
		public void MethodWithInParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithInParameter").Parameters.Single();
			Assert.That(!p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.In));
			Assert.That(p.GetAttributes().Count(), Is.EqualTo(0));
			Assert.That(p.Type.Kind == TypeKind.ByReference);
		}

		[Test]
		public void MethodWithParamsArray()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithParamsArray").Parameters.Single();
			Assert.That(!p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.None));
			Assert.That(p.IsParams);
			Assert.That(p.GetAttributes().Count(), Is.EqualTo(0));
			Assert.That(p.Type.Kind == TypeKind.Array);
		}

		[Test]
		public void MethodWithOptionalParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithOptionalParameter").Parameters.Single();
			Assert.That(p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.None));
			Assert.That(!p.IsParams);
			Assert.That(p.HasConstantValueInSignature);
			Assert.That(p.GetAttributes().Count(), Is.EqualTo(0));
			Assert.That(p.GetConstantValue(), Is.EqualTo(4));
		}

		[Test]
		public void MethodWithExplicitOptionalParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithExplicitOptionalParameter").Parameters.Single();
			Assert.That(p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.None));
			Assert.That(!p.IsParams);
			Assert.That(!p.HasConstantValueInSignature);
			// explicit optional parameter appears in type system if it's read from C#, but not when read from IL
			//Assert.AreEqual(1, p.GetAttributes().Count());
		}

		[Test]
		public void MethodWithEnumOptionalParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithEnumOptionalParameter").Parameters.Single();
			Assert.That(p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.None));
			Assert.That(!p.IsParams);
			Assert.That(p.HasConstantValueInSignature);
			Assert.That(p.GetAttributes().Count(), Is.EqualTo(0));
			Assert.That(p.GetConstantValue(), Is.EqualTo((int)StringComparison.OrdinalIgnoreCase));
		}

		[Test]
		public void MethodWithOptionalNullableParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithOptionalNullableParameter").Parameters.Single();
			Assert.That(p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.None));
			Assert.That(!p.IsParams);
			Assert.That(p.HasConstantValueInSignature);
			Assert.That(p.GetAttributes().Count(), Is.EqualTo(0));
			Assert.That(p.GetConstantValue(), Is.Null);
		}

		[Test]
		public void MethodWithOptionalLongParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithOptionalLongParameter").Parameters.Single();
			Assert.That(p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.None));
			Assert.That(!p.IsParams);
			Assert.That(p.HasConstantValueInSignature);
			Assert.That(p.GetConstantValue(), Is.EqualTo(1L));
			Assert.That(p.GetConstantValue().GetType(), Is.EqualTo(typeof(long)));
		}

		[Test]
		public void MethodWithOptionalNullableLongParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithOptionalNullableLongParameter").Parameters.Single();
			Assert.That(p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.None));
			Assert.That(!p.IsParams);
			Assert.That(p.HasConstantValueInSignature);
			Assert.That(p.GetConstantValue(), Is.EqualTo(1L));
			Assert.That(p.GetConstantValue().GetType(), Is.EqualTo(typeof(long)));
		}

		[Test]
		public void MethodWithOptionalDecimalParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithOptionalDecimalParameter").Parameters.Single();
			Assert.That(p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.None));
			Assert.That(!p.IsParams);
			Assert.That(p.HasConstantValueInSignature);
			Assert.That(p.GetConstantValue(), Is.EqualTo(1M));
			Assert.That(p.GetConstantValue().GetType(), Is.EqualTo(typeof(decimal)));
		}

		[Test]
		public void VarArgsMethod()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "VarArgsMethod").Parameters.Single();
			Assert.That(!p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.None));
			Assert.That(!p.IsParams);
			Assert.That(p.Type.Kind, Is.EqualTo(TypeKind.ArgList));
			Assert.That(p.Name, Is.EqualTo(""));
		}

		[Test]
		public void VarArgsCtor()
		{
			IParameter p = GetTypeDefinition(typeof(VarArgsCtor)).Methods.Single(m => m.IsConstructor).Parameters.Single();
			Assert.That(!p.IsOptional);
			Assert.That(p.ReferenceKind, Is.EqualTo(ReferenceKind.None));
			Assert.That(!p.IsParams);
			Assert.That(p.Type.Kind, Is.EqualTo(TypeKind.ArgList));
			Assert.That(p.Name, Is.EqualTo(""));
		}

		[Test]
		public void GenericDelegate_Variance()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(GenericDelegate<,>));
			Assert.That(type.TypeParameters[0].Variance, Is.EqualTo(VarianceModifier.Contravariant));
			Assert.That(type.TypeParameters[1].Variance, Is.EqualTo(VarianceModifier.Covariant));

			Assert.That(type.TypeParameters[0].DirectBaseTypes.FirstOrDefault(), Is.SameAs(type.TypeParameters[1]));
		}

		[Test]
		public void GenericDelegate_ReferenceTypeConstraints()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(GenericDelegate<,>));
			Assert.That(!type.TypeParameters[0].HasReferenceTypeConstraint);
			Assert.That(type.TypeParameters[1].HasReferenceTypeConstraint);

			Assert.That(type.TypeParameters[0].IsReferenceType, Is.Null);
			Assert.That(type.TypeParameters[1].IsReferenceType, Is.EqualTo(true));
		}

		[Test]
		public void GenericDelegate_GetInvokeMethod()
		{
			IType type = compilation.FindType(typeof(GenericDelegate<string, object>));
			IMethod m = type.GetDelegateInvokeMethod();
			Assert.That(m.Name, Is.EqualTo("Invoke"));
			Assert.That(m.ReturnType.FullName, Is.EqualTo("System.Object"));
			Assert.That(m.Parameters[0].Type.FullName, Is.EqualTo("System.String"));
		}

		[Test]
		public void ComInterfaceTest()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(IAssemblyEnum));
			// [ComImport]
			Assert.That(type.GetAttributes().Count(a => a.AttributeType.FullName == typeof(ComImportAttribute).FullName), Is.EqualTo(1));

			IMethod m = type.Methods.Single();
			Assert.That(m.Name, Is.EqualTo("GetNextAssembly"));
			Assert.That(m.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(m.IsAbstract);
			Assert.That(!m.IsVirtual);
			Assert.That(!m.IsSealed);
		}

		[Test]
		public void InnerClassInGenericClassIsReferencedUsingParameterizedType()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(OuterGeneric<>));
			IField field1 = type.Fields.Single(f => f.Name == "Field1");
			IField field2 = type.Fields.Single(f => f.Name == "Field2");
			IField field3 = type.Fields.Single(f => f.Name == "Field3");

			// types must be self-parameterized
			Assert.That(field1.Type.ReflectionName, Is.EqualTo("ICSharpCode.Decompiler.Tests.TypeSystem.OuterGeneric`1+Inner[[`0]]"));
			Assert.That(field2.Type.ReflectionName, Is.EqualTo("ICSharpCode.Decompiler.Tests.TypeSystem.OuterGeneric`1+Inner[[`0]]"));
			Assert.That(field3.Type.ReflectionName, Is.EqualTo("ICSharpCode.Decompiler.Tests.TypeSystem.OuterGeneric`1+Inner[[ICSharpCode.Decompiler.Tests.TypeSystem.OuterGeneric`1+Inner[[`0]]]]"));
		}

		[Test]
		public void FlagsOnInterfaceMembersAreCorrect()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(IInterfaceWithProperty));
			var p = type.Properties.Single();
			Assert.That(p.IsIndexer, Is.EqualTo(false));
			Assert.That(p.IsAbstract, Is.EqualTo(true));
			Assert.That(p.IsOverridable, Is.EqualTo(true));
			Assert.That(p.IsOverride, Is.EqualTo(false));
			Assert.That(p.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Getter.IsAbstract, Is.EqualTo(true));
			Assert.That(p.Getter.IsOverridable, Is.EqualTo(true));
			Assert.That(p.Getter.IsOverride, Is.EqualTo(false));
			Assert.That(p.Getter.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Getter.HasBody, Is.EqualTo(false));
			Assert.That(p.Setter.IsAbstract, Is.EqualTo(true));
			Assert.That(p.Setter.IsOverridable, Is.EqualTo(true));
			Assert.That(p.Setter.IsOverride, Is.EqualTo(false));
			Assert.That(p.Setter.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Setter.HasBody, Is.EqualTo(false));

			type = GetTypeDefinition(typeof(IInterfaceWithIndexers));
			p = type.Properties.Single(x => x.Parameters.Count == 2);
			Assert.That(p.IsIndexer, Is.EqualTo(true));
			Assert.That(p.IsAbstract, Is.EqualTo(true));
			Assert.That(p.IsOverridable, Is.EqualTo(true));
			Assert.That(p.IsOverride, Is.EqualTo(false));
			Assert.That(p.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Getter.IsAbstract, Is.EqualTo(true));
			Assert.That(p.Getter.IsOverridable, Is.EqualTo(true));
			Assert.That(p.Getter.IsOverride, Is.EqualTo(false));
			Assert.That(p.Getter.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(p.Setter.IsAbstract, Is.EqualTo(true));
			Assert.That(p.Setter.IsOverridable, Is.EqualTo(true));
			Assert.That(p.Setter.IsOverride, Is.EqualTo(false));
			Assert.That(p.Setter.Accessibility, Is.EqualTo(Accessibility.Public));

			type = GetTypeDefinition(typeof(IHasEvent));
			var e = type.Events.Single();
			Assert.That(e.IsAbstract, Is.EqualTo(true));
			Assert.That(e.IsOverridable, Is.EqualTo(true));
			Assert.That(e.IsOverride, Is.EqualTo(false));
			Assert.That(e.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(e.AddAccessor.IsAbstract, Is.EqualTo(true));
			Assert.That(e.AddAccessor.IsOverridable, Is.EqualTo(true));
			Assert.That(e.AddAccessor.IsOverride, Is.EqualTo(false));
			Assert.That(e.AddAccessor.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(e.RemoveAccessor.IsAbstract, Is.EqualTo(true));
			Assert.That(e.RemoveAccessor.IsOverridable, Is.EqualTo(true));
			Assert.That(e.RemoveAccessor.IsOverride, Is.EqualTo(false));
			Assert.That(e.RemoveAccessor.Accessibility, Is.EqualTo(Accessibility.Public));

			type = GetTypeDefinition(typeof(IDisposable));
			var m = type.Methods.Single();
			Assert.That(m.IsAbstract, Is.EqualTo(true));
			Assert.That(m.IsOverridable, Is.EqualTo(true));
			Assert.That(m.IsOverride, Is.EqualTo(false));
			Assert.That(m.Accessibility, Is.EqualTo(Accessibility.Public));
		}

		[Test]
		public void InnerClassInGenericClass_TypeParameterOwner()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(OuterGeneric<>.Inner));
			Assert.That(type.TypeParameters[0], Is.SameAs(type.DeclaringTypeDefinition.TypeParameters[0]));
			Assert.That(type.TypeParameters[0].Owner, Is.SameAs(type.DeclaringTypeDefinition));
		}

		[Test]
		public void InnerClassInGenericClass_ReferencesTheOuterClass_Field()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(OuterGeneric<>.Inner));
			IField f = type.Fields.Single();
			Assert.That(f.Type.ReflectionName, Is.EqualTo("ICSharpCode.Decompiler.Tests.TypeSystem.OuterGeneric`1[[`0]]"));
		}

		[Test]
		public void InnerClassInGenericClass_ReferencesTheOuterClass_Parameter()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(OuterGeneric<>.Inner));
			IParameter p = type.Methods.Single(m => m.IsConstructor).Parameters.Single();
			Assert.That(p.Type.ReflectionName, Is.EqualTo("ICSharpCode.Decompiler.Tests.TypeSystem.OuterGeneric`1[[`0]]"));
		}

		CustomAttributeTypedArgument<IType> GetParamsAttributeArgument(int index)
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ParamsAttribute));
			var arr = (AttributeArray)type.GetAttributes().Single().FixedArguments.Single().Value;
			Assert.That(arr.Length, Is.EqualTo(5));
			return arr[index];
		}

		[Test]
		public void ParamsAttribute_Integer()
		{
			var arg = GetParamsAttributeArgument(0);
			Assert.That(arg.Type.FullName, Is.EqualTo("System.Int32"));
			Assert.That(arg.Value, Is.EqualTo(1));
		}

		[Test]
		public void ParamsAttribute_Enum()
		{
			var arg = GetParamsAttributeArgument(1);
			Assert.That(arg.Type.FullName, Is.EqualTo("System.StringComparison"));
			Assert.That(arg.Value, Is.EqualTo((int)StringComparison.CurrentCulture));
		}

		[Test]
		public void ParamsAttribute_NullReference()
		{
			var arg = GetParamsAttributeArgument(2);
			//Assert.AreEqual("System.Object", arg.Type.FullName);
			Assert.That(arg.Value, Is.Null);
		}

		[Test]
		public void ParamsAttribute_Double()
		{
			var arg = GetParamsAttributeArgument(3);
			Assert.That(arg.Type.FullName, Is.EqualTo("System.Double"));
			Assert.That(arg.Value, Is.EqualTo(4.0));
		}

		[Test]
		public void ParamsAttribute_String()
		{
			var arg = GetParamsAttributeArgument(4);
			Assert.That(arg.Type.FullName, Is.EqualTo("System.String"));
			Assert.That(arg.Value, Is.EqualTo("Test"));
		}

		[Test]
		public void ParamsAttribute_Property()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ParamsAttribute));
			IProperty prop = type.Properties.Single(p => p.Name == "Property");
			var attr = prop.GetAttributes().Single();
			Assert.That(attr.AttributeType, Is.EqualTo(type));

			var elements = (AttributeArray)attr.FixedArguments.Single().Value;
			Assert.That(elements.Length, Is.EqualTo(0));

			var namedArg = attr.NamedArguments.Single();
			Assert.That(namedArg.Name, Is.EqualTo(prop.Name));
			var arrayElements = (AttributeArray)namedArg.Value;
			Assert.That(arrayElements.Length, Is.EqualTo(2));
		}

		[Test]
		public void ParamsAttribute_Getter_ReturnType()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ParamsAttribute));
			IProperty prop = type.Properties.Single(p => p.Name == "Property");
			Assert.That(prop.Getter.GetAttributes().Count(), Is.EqualTo(0));
			Assert.That(prop.Getter.GetReturnTypeAttributes().Count(), Is.EqualTo(1));
		}

		[Test]
		public void DoubleAttribute_ImplicitNumericConversion()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(DoubleAttribute));
			var arg = type.GetAttributes().Single().FixedArguments.Single();
			Assert.That(arg.Type.ReflectionName, Is.EqualTo("System.Double"));
			Assert.That(arg.Value, Is.EqualTo(1.0));
		}

		/* TS no longer provides implicitly implemented interface members.
		[Test]
		public void ImplicitImplementationOfUnifiedMethods()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ImplementationOfUnifiedMethods));
			IMethod test = type.Methods.Single(m => m.Name == "Test");
			Assert.AreEqual(2, test.ImplementedInterfaceMembers.Count);
			Assert.AreEqual("Int32", ((IMethod)test.ImplementedInterfaceMembers[0]).Parameters.Single().Type.Name);
			Assert.AreEqual("Int32", ((IMethod)test.ImplementedInterfaceMembers[1]).Parameters.Single().Type.Name);
			Assert.AreEqual("T", ((IMethod)test.ImplementedInterfaceMembers[0].MemberDefinition).Parameters.Single().Type.Name);
			Assert.AreEqual("S", ((IMethod)test.ImplementedInterfaceMembers[1].MemberDefinition).Parameters.Single().Type.Name);
		}
		*/

		[Test]
		public void StaticityOfEventAccessors()
		{
			// https://github.com/icsharpcode/NRefactory/issues/20
			ITypeDefinition type = GetTypeDefinition(typeof(ClassWithStaticAndNonStaticMembers));
			var evt1 = type.Events.Single(e => e.Name == "Event1");
			Assert.That(evt1.IsStatic);
			Assert.That(evt1.AddAccessor.IsStatic);
			Assert.That(evt1.RemoveAccessor.IsStatic);

			var evt2 = type.Events.Single(e => e.Name == "Event2");
			Assert.That(!evt2.IsStatic);
			Assert.That(!evt2.AddAccessor.IsStatic);
			Assert.That(!evt2.RemoveAccessor.IsStatic);

			var evt3 = type.Events.Single(e => e.Name == "Event3");
			Assert.That(evt3.IsStatic);
			Assert.That(evt3.AddAccessor.IsStatic);
			Assert.That(evt3.RemoveAccessor.IsStatic);

			var evt4 = type.Events.Single(e => e.Name == "Event4");
			Assert.That(!evt4.IsStatic);
			Assert.That(!evt4.AddAccessor.IsStatic);
			Assert.That(!evt4.RemoveAccessor.IsStatic);
		}

		[Test]
		public void StaticityOfPropertyAccessors()
		{
			// https://github.com/icsharpcode/NRefactory/issues/20
			ITypeDefinition type = GetTypeDefinition(typeof(ClassWithStaticAndNonStaticMembers));
			var prop1 = type.Properties.Single(e => e.Name == "Prop1");
			Assert.That(prop1.IsStatic);
			Assert.That(prop1.Getter.IsStatic);
			Assert.That(prop1.Setter.IsStatic);

			var prop2 = type.Properties.Single(e => e.Name == "Prop2");
			Assert.That(!prop2.IsStatic);
			Assert.That(!prop2.Getter.IsStatic);
			Assert.That(!prop2.Setter.IsStatic);

			var prop3 = type.Properties.Single(e => e.Name == "Prop3");
			Assert.That(prop3.IsStatic);
			Assert.That(prop3.Getter.IsStatic);
			Assert.That(prop3.Setter.IsStatic);

			var prop4 = type.Properties.Single(e => e.Name == "Prop4");
			Assert.That(!prop4.IsStatic);
			Assert.That(!prop4.Getter.IsStatic);
			Assert.That(!prop4.Setter.IsStatic);
		}

		[Test]
		public void PropertyAccessorsHaveBody()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassWithStaticAndNonStaticMembers));
			foreach (var prop in type.Properties)
			{
				Assert.That(prop.Getter.HasBody, prop.Getter.Name);
				Assert.That(prop.Setter.HasBody, prop.Setter.Name);
			}
		}

		[Test]
		public void EventAccessorNames()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassWithStaticAndNonStaticMembers));
			var customEvent = type.Events.Single(e => e.Name == "Event1");
			Assert.That(customEvent.AddAccessor.Name, Is.EqualTo("add_Event1"));
			Assert.That(customEvent.RemoveAccessor.Name, Is.EqualTo("remove_Event1"));

			var normalEvent = type.Events.Single(e => e.Name == "Event3");
			Assert.That(normalEvent.AddAccessor.Name, Is.EqualTo("add_Event3"));
			Assert.That(normalEvent.RemoveAccessor.Name, Is.EqualTo("remove_Event3"));
		}

		[Test]
		public void EventAccessorHaveBody()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassWithStaticAndNonStaticMembers));
			foreach (var ev in type.Events)
			{
				Assert.That(ev.AddAccessor.HasBody, ev.AddAccessor.Name);
				Assert.That(ev.RemoveAccessor.HasBody, ev.RemoveAccessor.Name);
			}
		}

		[Test]
		public void InterfacePropertyAccessorsShouldNotBeOverrides()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(IInterfaceWithProperty));
			var prop = type.Properties.Single(p => p.Name == "Prop");
			Assert.That(prop.Getter.IsOverride, Is.False);
			Assert.That(prop.Getter.IsOverridable, Is.True);
			Assert.That(prop.Setter.IsOverride, Is.False);
			Assert.That(prop.Setter.IsOverridable, Is.True);
		}

		[Test]
		public void InterfaceShouldDeriveFromObject()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(IInterfaceWithProperty));
			Assert.That(type.DirectBaseTypes.Count() == 1, "Should have exactly one direct base type");
			Assert.That(type.DirectBaseTypes.First().IsKnownType(KnownTypeCode.Object), "Base type should be object");
		}

		[Test]
		public void InterfaceShouldDeriveFromObject2()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(IShadowTestDerived));
			Assert.That(type.DirectBaseTypes.Count() == 2, "Should have exactly two direct base types");
			Assert.That(type.DirectBaseTypes.Skip(1).First() == GetTypeDefinition(typeof(IShadowTestBase)), "Base type should be IShadowTestBase");
			Assert.That(type.DirectBaseTypes.First().IsKnownType(KnownTypeCode.Object), "Base type should be object");
		}

		[Test]
		public void CheckInterfaceDirectBaseTypes()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(IDerived));
			Assert.That(type.DirectBaseTypes.Count() == 3, "Should have exactly three direct base types");
			Assert.That(type.DirectBaseTypes.First().IsKnownType(KnownTypeCode.Object), "Base type should be object");
			Assert.That(type.DirectBaseTypes.Skip(1).First() == GetTypeDefinition(typeof(IBase1)), "Base type should be IBase1");
			Assert.That(type.DirectBaseTypes.Skip(2).First() == GetTypeDefinition(typeof(IBase2)), "Base type should be IBase2");
		}

		[Test]
		public void VirtualPropertyAccessorsShouldNotBeOverrides()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassWithVirtualProperty));
			var prop = type.Properties.Single(p => p.Name == "Prop");
			Assert.That(prop.Getter.IsOverride, Is.False);
			Assert.That(prop.Getter.IsOverridable, Is.True);
			Assert.That(prop.Setter.IsOverride, Is.False);
			Assert.That(prop.Setter.IsOverridable, Is.True);
		}

		[Test]
		public void ClassThatOverridesGetterOnly()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatOverridesGetterOnly));
			var prop = type.Properties.Single(p => p.Name == "Prop");
			Assert.That(prop.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(prop.Getter.Accessibility, Is.EqualTo(Accessibility.Public));
		}

		[Test]
		public void ClassThatOverridesSetterOnly()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatOverridesSetterOnly));
			var prop = type.Properties.Single(p => p.Name == "Prop");
			Assert.That(prop.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(prop.Setter.Accessibility, Is.EqualTo(Accessibility.Protected));
		}

		/* TS no longer provides implicit interface impls
		[Test]
		public void PropertyAccessorsShouldBeReportedAsImplementingInterfaceAccessors()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatImplementsProperty));
			var prop = type.Properties.Single(p => p.Name == "Prop");
			Assert.That(prop.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithProperty.Prop" }));
			Assert.That(prop.Getter.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithProperty.get_Prop" }));
			Assert.That(prop.Setter.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithProperty.set_Prop" }));
		}
		*/

		[Test]
		public void PropertyThatImplementsInterfaceIsNotVirtual()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatImplementsProperty));
			var prop = type.Properties.Single(p => p.Name == "Prop");
			Assert.That(!prop.IsVirtual);
			Assert.That(!prop.IsOverridable);
			Assert.That(!prop.IsSealed);
		}

		[Test]
		public void Property_SealedOverride()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatOverridesAndSealsVirtualProperty));
			var prop = type.Properties.Single(p => p.Name == "Prop");
			Assert.That(!prop.IsVirtual);
			Assert.That(prop.IsOverride);
			Assert.That(prop.IsSealed);
			Assert.That(!prop.IsOverridable);
		}

		/* The TS no longer provides implicit interface impls.
		[Test]
		public void IndexerAccessorsShouldBeReportedAsImplementingTheCorrectInterfaceAccessors()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatImplementsIndexers));
			var ix1 = type.Properties.Single(p => p.Parameters.Count == 1 && p.Parameters[0].Type.GetDefinition().KnownTypeCode == KnownTypeCode.Int32);
			var ix2 = type.Properties.Single(p => p.Parameters.Count == 1 && p.Parameters[0].Type.GetDefinition().KnownTypeCode == KnownTypeCode.String);
			var ix3 = type.Properties.Single(p => p.Parameters.Count == 2);

			Assert.That(ix1.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EquivalentTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithIndexers.Item", "ICSharpCode.Decompiler.Tests.TypeSystem.IGenericInterfaceWithIndexer`1.Item" }));
			Assert.That(ix1.ImplementedInterfaceMembers.All(p => ((IProperty)p).Parameters.Select(x => x.Type.GetDefinition().KnownTypeCode).SequenceEqual(new[] { KnownTypeCode.Int32 })));
			Assert.That(ix1.Getter.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EquivalentTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithIndexers.get_Item", "ICSharpCode.Decompiler.Tests.TypeSystem.IGenericInterfaceWithIndexer`1.get_Item" }));
			Assert.That(ix1.Getter.ImplementedInterfaceMembers.All(m => ((IMethod)m).Parameters.Select(p => p.Type.GetDefinition().KnownTypeCode).SequenceEqual(new[] { KnownTypeCode.Int32 })));
			Assert.That(ix1.Setter.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EquivalentTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithIndexers.set_Item", "ICSharpCode.Decompiler.Tests.TypeSystem.IGenericInterfaceWithIndexer`1.set_Item" }));
			Assert.That(ix1.Setter.ImplementedInterfaceMembers.All(m => ((IMethod)m).Parameters.Select(p => p.Type.GetDefinition().KnownTypeCode).SequenceEqual(new[] { KnownTypeCode.Int32, KnownTypeCode.Int32 })));

			Assert.That(ix2.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithIndexers.Item" }));
			Assert.That(ix2.ImplementedInterfaceMembers.All(p => ((IProperty)p).Parameters.Select(x => x.Type.GetDefinition().KnownTypeCode).SequenceEqual(new[] { KnownTypeCode.String })));
			Assert.That(ix2.Getter.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithIndexers.get_Item" }));
			Assert.That(ix2.Getter.ImplementedInterfaceMembers.All(m => ((IMethod)m).Parameters.Select(p => p.Type.GetDefinition().KnownTypeCode).SequenceEqual(new[] { KnownTypeCode.String })));
			Assert.That(ix2.Setter.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithIndexers.set_Item" }));
			Assert.That(ix2.Setter.ImplementedInterfaceMembers.All(m => ((IMethod)m).Parameters.Select(p => p.Type.GetDefinition().KnownTypeCode).SequenceEqual(new[] { KnownTypeCode.String, KnownTypeCode.Int32 })));

			Assert.That(ix3.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithIndexers.Item" }));
			Assert.That(ix3.ImplementedInterfaceMembers.All(p => ((IProperty)p).Parameters.Select(x => x.Type.GetDefinition().KnownTypeCode).SequenceEqual(new[] { KnownTypeCode.Int32, KnownTypeCode.Int32 })));
			Assert.That(ix3.Getter.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithIndexers.get_Item" }));
			Assert.That(ix3.Getter.ImplementedInterfaceMembers.All(m => ((IMethod)m).Parameters.Select(p => p.Type.GetDefinition().KnownTypeCode).SequenceEqual(new[] { KnownTypeCode.Int32, KnownTypeCode.Int32 })));
			Assert.That(ix3.Setter.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithIndexers.set_Item" }));
			Assert.That(ix3.Setter.ImplementedInterfaceMembers.All(m => ((IMethod)m).Parameters.Select(p => p.Type.GetDefinition().KnownTypeCode).SequenceEqual(new[] { KnownTypeCode.Int32, KnownTypeCode.Int32, KnownTypeCode.Int32 })));
		}
		*/

		[Test]
		public void ExplicitIndexerImplementationReturnsTheCorrectMembers()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatImplementsIndexersExplicitly));

			Assert.That(type.Properties.All(p => p.SymbolKind == SymbolKind.Indexer));

			Assert.That(type.Properties.All(p => p.ExplicitlyImplementedInterfaceMembers.Count() == 1));
			Assert.That(type.Properties.All(p => p.Getter.ExplicitlyImplementedInterfaceMembers.Count() == 1));
			Assert.That(type.Properties.All(p => p.Setter.ExplicitlyImplementedInterfaceMembers.Count() == 1));
		}

		[Test]
		public void ExplicitDisposableImplementation()
		{
			ITypeDefinition disposable = GetTypeDefinition(typeof(ExplicitDisposableImplementation));
			IMethod method = disposable.Methods.Single(m => !m.IsConstructor);
			Assert.That(method.IsExplicitInterfaceImplementation);
			Assert.That(method.ExplicitlyImplementedInterfaceMembers.Single().FullName, Is.EqualTo("System.IDisposable.Dispose"));
		}

		[Test]
		public void ExplicitImplementationOfUnifiedMethods()
		{
			IType type = compilation.FindType(typeof(ExplicitGenericInterfaceImplementationWithUnifiableMethods<int, int>));
			Assert.That(type.GetMethods(m => m.IsExplicitInterfaceImplementation).Count(), Is.EqualTo(2));
			foreach (IMethod method in type.GetMethods(m => m.IsExplicitInterfaceImplementation))
			{
				Assert.That(method.ExplicitlyImplementedInterfaceMembers.Count(), Is.EqualTo(1), method.ToString());
				Assert.That(method.Parameters.Single().Type.ReflectionName, Is.EqualTo("System.Int32"));
				IMethod interfaceMethod = (IMethod)method.ExplicitlyImplementedInterfaceMembers.Single();
				Assert.That(interfaceMethod.Parameters.Single().Type.ReflectionName, Is.EqualTo("System.Int32"));
				var genericParamType = ((IMethod)method.MemberDefinition).Parameters.Single().Type;
				var interfaceGenericParamType = ((IMethod)interfaceMethod.MemberDefinition).Parameters.Single().Type;
				Assert.That(genericParamType.Kind, Is.EqualTo(TypeKind.TypeParameter));
				Assert.That(interfaceGenericParamType.Kind, Is.EqualTo(TypeKind.TypeParameter));
				Assert.That(interfaceGenericParamType.ReflectionName, Is.EqualTo(genericParamType.ReflectionName));
			}
		}

		[Test]
		public void ExplicitGenericInterfaceImplementation()
		{
			ITypeDefinition impl = GetTypeDefinition(typeof(ExplicitGenericInterfaceImplementation));
			IType genericInterfaceOfString = compilation.FindType(typeof(IGenericInterface<string>));
			IMethod implMethod1 = impl.Methods.Single(m => !m.IsConstructor && m.Parameters[1].ReferenceKind == ReferenceKind.None);
			IMethod implMethod2 = impl.Methods.Single(m => !m.IsConstructor && m.Parameters[1].ReferenceKind == ReferenceKind.Ref);
			Assert.That(implMethod1.IsExplicitInterfaceImplementation);
			Assert.That(implMethod2.IsExplicitInterfaceImplementation);

			IMethod interfaceMethod1 = (IMethod)implMethod1.ExplicitlyImplementedInterfaceMembers.Single();
			Assert.That(interfaceMethod1.DeclaringType, Is.EqualTo(genericInterfaceOfString));
			Assert.That(interfaceMethod1.Parameters[1].ReferenceKind == ReferenceKind.None);

			IMethod interfaceMethod2 = (IMethod)implMethod2.ExplicitlyImplementedInterfaceMembers.Single();
			Assert.That(interfaceMethod2.DeclaringType, Is.EqualTo(genericInterfaceOfString));
			Assert.That(interfaceMethod2.Parameters[1].ReferenceKind == ReferenceKind.Ref);
		}

		[Test]
		public void ExplicitlyImplementedPropertiesShouldBeReportedAsBeingImplemented()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatImplementsPropertyExplicitly));
			var prop = type.Properties.Single();
			Assert.That(prop.ExplicitlyImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithProperty.Prop" }));
			Assert.That(prop.Getter.ExplicitlyImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithProperty.get_Prop" }));
			Assert.That(prop.Setter.ExplicitlyImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IInterfaceWithProperty.set_Prop" }));
		}

		[Test]
		public void ExplicitlyImplementedPropertiesShouldHaveExplicitlyImplementedAccessors()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatImplementsPropertyExplicitly));
			var prop = type.Properties.Single();
			Assert.That(prop.IsExplicitInterfaceImplementation);
			Assert.That(prop.Getter.IsExplicitInterfaceImplementation);
			Assert.That(prop.Setter.IsExplicitInterfaceImplementation);
		}

		/* The TS no longer provides implicit interface impls.
		[Test]
		public void EventAccessorsShouldBeReportedAsImplementingInterfaceAccessors()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatImplementsEvent));
			var evt = type.Events.Single(p => p.Name == "Event");
			Assert.That(evt.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IHasEvent.Event" }));
			Assert.That(evt.AddAccessor.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IHasEvent.add_Event" }));
			Assert.That(evt.RemoveAccessor.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IHasEvent.remove_Event" }));
		}

		[Test]
		public void EventAccessorsShouldBeReportedAsImplementingInterfaceAccessorsWhenCustomAccessorMethodsAreUsed()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatImplementsEventWithCustomAccessors));
			var evt = type.Events.Single(p => p.Name == "Event");
			Assert.That(evt.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IHasEvent.Event" }));
			Assert.That(evt.AddAccessor.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IHasEvent.add_Event" }));
			Assert.That(evt.RemoveAccessor.ImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IHasEvent.remove_Event" }));
		}
		*/

		[Test]
		public void ExplicitlyImplementedEventsShouldBeReportedAsBeingImplemented()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatImplementsEventExplicitly));
			var evt = type.Events.Single();
			Assert.That(evt.ExplicitlyImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IHasEvent.Event" }));
			Assert.That(evt.AddAccessor.ExplicitlyImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IHasEvent.add_Event" }));
			Assert.That(evt.RemoveAccessor.ExplicitlyImplementedInterfaceMembers.Select(p => p.ReflectionName).ToList(), Is.EqualTo(new[] { "ICSharpCode.Decompiler.Tests.TypeSystem.IHasEvent.remove_Event" }));
		}

		[Test]
		public void MembersDeclaredInDerivedInterfacesDoNotImplementBaseMembers()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(IShadowTestDerived));
			var method = type.Methods.Single(m => m.Name == "Method");
			var indexer = type.Properties.Single(p => p.IsIndexer);
			var prop = type.Properties.Single(p => p.Name == "Prop");
			var evt = type.Events.Single(e => e.Name == "Evt");

			Assert.That(method.ExplicitlyImplementedInterfaceMembers, Is.Empty);
			Assert.That(indexer.ExplicitlyImplementedInterfaceMembers, Is.Empty);
			Assert.That(indexer.Getter.ExplicitlyImplementedInterfaceMembers, Is.Empty);
			Assert.That(indexer.Setter.ExplicitlyImplementedInterfaceMembers, Is.Empty);
			Assert.That(prop.ExplicitlyImplementedInterfaceMembers, Is.Empty);
			Assert.That(prop.Getter.ExplicitlyImplementedInterfaceMembers, Is.Empty);
			Assert.That(prop.Setter.ExplicitlyImplementedInterfaceMembers, Is.Empty);
			Assert.That(evt.ExplicitlyImplementedInterfaceMembers, Is.Empty);
			Assert.That(evt.AddAccessor.ExplicitlyImplementedInterfaceMembers, Is.Empty);
			Assert.That(evt.RemoveAccessor.ExplicitlyImplementedInterfaceMembers, Is.Empty);
		}

		[Test]
		public void StaticClassTest()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(StaticClass));
			Assert.That(type.IsAbstract);
			Assert.That(type.IsSealed);
			Assert.That(type.IsStatic);
		}

		[Test]
		public void ExtensionMethodTest()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(StaticClass));
			var method = type.Methods.Single(m => m.Name == "Extension");

			Assert.That(method.IsStatic);
			Assert.That(method.IsExtensionMethod);
			Assert.That(method.ReducedFrom, Is.Null);

			Assert.That(type.HasExtensionMethods);
		}

		[Test]
		public void NoDefaultConstructorOnStaticClassTest()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(StaticClass));
			Assert.That(type.GetConstructors().Count(), Is.EqualTo(0));
		}

		[Test]
		public void IndexerNonDefaultName()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(IndexerNonDefaultName));
			var indexer = type.GetProperties(p => p.IsIndexer).Single();
			Assert.That(indexer.Name, Is.EqualTo("Foo"));
		}

		[Test]
		public void TestNullableDefaultParameter()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassWithMethodThatHasNullableDefaultParameter));
			var method = type.GetMethods().Single(m => m.Name == "Foo");
			Assert.That(method.Parameters.Single().GetConstantValue(), Is.EqualTo(42));
		}

		[Test]
		public void AccessibilityTests()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(AccessibilityTest));
			Assert.That(type.Methods.Single(m => m.Name == "Public").Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(type.Methods.Single(m => m.Name == "Internal").Accessibility, Is.EqualTo(Accessibility.Internal));
			Assert.That(type.Methods.Single(m => m.Name == "ProtectedInternal").Accessibility, Is.EqualTo(Accessibility.ProtectedOrInternal));
			Assert.That(type.Methods.Single(m => m.Name == "InternalProtected").Accessibility, Is.EqualTo(Accessibility.ProtectedOrInternal));
			Assert.That(type.Methods.Single(m => m.Name == "Protected").Accessibility, Is.EqualTo(Accessibility.Protected));
			Assert.That(type.Methods.Single(m => m.Name == "Private").Accessibility, Is.EqualTo(Accessibility.Private));
			Assert.That(type.Methods.Single(m => m.Name == "None").Accessibility, Is.EqualTo(Accessibility.Private));
		}

		private void AssertConstantField<T>(ITypeDefinition type, string name, T expected)
		{
			var f = type.GetFields().Single(x => x.Name == name);
			Assert.That(f.IsConst);
			Assert.That(f.GetConstantValue(), Is.EqualTo(expected));
			Assert.That(f.GetAttributes().Count(), Is.EqualTo(0));
		}

		[Test]
		public unsafe void ConstantFieldsCreatedWithNew()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ConstantFieldTest));
			AssertConstantField<byte>(type, "CNewb", new byte());
			AssertConstantField<sbyte>(type, "CNewsb", new sbyte());
			AssertConstantField<char>(type, "CNewc", new char());
			AssertConstantField<short>(type, "CNews", new short());
			AssertConstantField<ushort>(type, "CNewus", new ushort());
			AssertConstantField<int>(type, "CNewi", new int());
			AssertConstantField<uint>(type, "CNewui", new uint());
			AssertConstantField<long>(type, "CNewl", new long());
			AssertConstantField<ulong>(type, "CNewul", new ulong());
			AssertConstantField<double>(type, "CNewd", new double());
			AssertConstantField<float>(type, "CNewf", new float());
			AssertConstantField<decimal>(type, "CNewm", new decimal());
		}


		[Test]
		public void ConstantFieldsSizeOf()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ConstantFieldTest));
			AssertConstantField<int>(type, "SOsb", sizeof(sbyte));
			AssertConstantField<int>(type, "SOb", sizeof(byte));
			AssertConstantField<int>(type, "SOs", sizeof(short));
			AssertConstantField<int>(type, "SOus", sizeof(ushort));
			AssertConstantField<int>(type, "SOi", sizeof(int));
			AssertConstantField<int>(type, "SOui", sizeof(uint));
			AssertConstantField<int>(type, "SOl", sizeof(long));
			AssertConstantField<int>(type, "SOul", sizeof(ulong));
			AssertConstantField<int>(type, "SOc", sizeof(char));
			AssertConstantField<int>(type, "SOf", sizeof(float));
			AssertConstantField<int>(type, "SOd", sizeof(double));
			AssertConstantField<int>(type, "SObl", sizeof(bool));
			AssertConstantField<int>(type, "SOe", sizeof(MyEnum));
		}

		[Test]
		public void ConstantEnumFromThisAssembly()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ConstantFieldTest));
			IField field = type.Fields.Single(f => f.Name == "EnumFromThisAssembly");
			Assert.That(field.IsConst);
			Assert.That(field.GetConstantValue(), Is.EqualTo((short)MyEnum.Second));
		}

		[Test]
		public void ConstantEnumFromAnotherAssembly()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ConstantFieldTest));
			IField field = type.Fields.Single(f => f.Name == "EnumFromAnotherAssembly");
			Assert.That(field.IsConst);
			Assert.That(field.GetConstantValue(), Is.EqualTo((int)StringComparison.OrdinalIgnoreCase));
		}

		[Test]
		public void DefaultOfEnum()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ConstantFieldTest));
			IField field = type.Fields.Single(f => f.Name == "DefaultOfEnum");
			Assert.That(field.IsConst);
			Assert.That(field.GetConstantValue(), Is.EqualTo((short)default(MyEnum)));
		}

		[Test]
		public void ExplicitImplementation()
		{
			var type = GetTypeDefinition(typeof(ExplicitImplementationTests));
			var itype = GetTypeDefinition(typeof(IExplicitImplementationTests));

			var methods = type.GetMethods(m => m.Name == "M" || m.Name.EndsWith(".M")).ToList();
			var imethod = itype.GetMethods(m => m.Name == "M").Single();
			Assert.That(methods.Select(m => m.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.That(imethod, Is.EqualTo(methods.SelectMany(m => m.ExplicitlyImplementedInterfaceMembers).Single()));

			var properties = type.GetProperties(p => p.Name == "P" || p.Name.EndsWith(".P")).ToList();
			var iproperty = itype.GetProperties(m => m.Name == "P").Single();
			Assert.That(properties.Select(p => p.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.That(iproperty, Is.EqualTo(properties.SelectMany(p => p.ExplicitlyImplementedInterfaceMembers).Single()));
			Assert.That(properties.Select(p => p.Getter.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.That(iproperty.Getter, Is.EqualTo(properties.SelectMany(p => p.Getter.ExplicitlyImplementedInterfaceMembers).Single()));
			Assert.That(properties.Select(p => p.Setter.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.That(iproperty.Setter, Is.EqualTo(properties.SelectMany(p => p.Setter.ExplicitlyImplementedInterfaceMembers).Single()));

			var indexers = type.GetProperties(p => p.Name == "Item" || p.Name.EndsWith(".Item")).ToList();
			var iindexer = itype.GetProperties(m => m.Name == "Item").Single();
			Assert.That(indexers.Select(p => p.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.That(iindexer, Is.EqualTo(indexers.SelectMany(p => p.ExplicitlyImplementedInterfaceMembers).Single()));
			Assert.That(indexers.Select(p => p.Getter.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.That(iindexer.Getter, Is.EqualTo(indexers.SelectMany(p => p.Getter.ExplicitlyImplementedInterfaceMembers).Single()));
			Assert.That(indexers.Select(p => p.Setter.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.That(iindexer.Setter, Is.EqualTo(indexers.SelectMany(p => p.Setter.ExplicitlyImplementedInterfaceMembers).Single()));

			var events = type.GetEvents(e => e.Name == "E" || e.Name.EndsWith(".E")).ToList();
			var ievent = itype.GetEvents(m => m.Name == "E").Single();
			Assert.That(events.Select(e => e.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.That(ievent, Is.EqualTo(events.SelectMany(e => e.ExplicitlyImplementedInterfaceMembers).Single()));
			Assert.That(events.Select(e => e.AddAccessor.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.That(ievent.AddAccessor, Is.EqualTo(events.SelectMany(e => e.AddAccessor.ExplicitlyImplementedInterfaceMembers).Single()));
			Assert.That(events.Select(e => e.RemoveAccessor.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.That(ievent.RemoveAccessor, Is.EqualTo(events.SelectMany(e => e.RemoveAccessor.ExplicitlyImplementedInterfaceMembers).Single()));
		}

		[Test]
		public void MarshalTests()
		{
			ITypeDefinition c = compilation.FindType(typeof(IMarshalAsTests)).GetDefinition();
			Assert.That(c.GetMethods(m => m.Name == "GetCollectionByQuery2").Count(), Is.EqualTo(1));
		}


		[Test]
		public void AttributesUsingNestedMembers()
		{
			var type = GetTypeDefinition(typeof(ClassWithAttributesUsingNestedMembers));
			var inner = type.GetNestedTypes().Single(t => t.Name == "Inner");
			var myAttribute = type.GetNestedTypes().Single(t => t.Name == "MyAttribute");
			var typeTypeTestAttr = type.GetAttributes().Single(a => a.AttributeType.Name == "TypeTestAttribute");
			Assert.That(typeTypeTestAttr.FixedArguments[0].Value, Is.EqualTo(42));
			Assert.That(typeTypeTestAttr.FixedArguments[1].Value, Is.EqualTo(inner));
			var typeMyAttr = type.GetAttributes().Single(a => a.AttributeType.Name == "MyAttribute");
			Assert.That(typeMyAttr.AttributeType, Is.EqualTo(myAttribute));

			var prop = type.GetProperties().Single(p => p.Name == "P");
			var propTypeTestAttr = prop.GetAttributes().Single(a => a.AttributeType.Name == "TypeTestAttribute");
			Assert.That(propTypeTestAttr.FixedArguments[0].Value, Is.EqualTo(42));
			Assert.That(propTypeTestAttr.FixedArguments[1].Value, Is.EqualTo(inner));
			var propMyAttr = prop.GetAttributes().Single(a => a.AttributeType.Name == "MyAttribute");
			Assert.That(propMyAttr.AttributeType, Is.EqualTo(myAttribute));

			var attributedInner = (ITypeDefinition)type.GetNestedTypes().Single(t => t.Name == "AttributedInner");
			var innerTypeTestAttr = attributedInner.GetAttributes().Single(a => a.AttributeType.Name == "TypeTestAttribute");
			Assert.That(innerTypeTestAttr.FixedArguments[0].Value, Is.EqualTo(42));
			Assert.That(innerTypeTestAttr.FixedArguments[1].Value, Is.EqualTo(inner));
			var innerMyAttr = attributedInner.GetAttributes().Single(a => a.AttributeType.Name == "MyAttribute");
			Assert.That(innerMyAttr.AttributeType, Is.EqualTo(myAttribute));

			var attributedInner2 = (ITypeDefinition)type.GetNestedTypes().Single(t => t.Name == "AttributedInner2");
			var inner2 = attributedInner2.GetNestedTypes().Single(t => t.Name == "Inner");
			var myAttribute2 = attributedInner2.GetNestedTypes().Single(t => t.Name == "MyAttribute");
			var inner2TypeTestAttr = attributedInner2.GetAttributes().Single(a => a.AttributeType.Name == "TypeTestAttribute");
			Assert.That(inner2TypeTestAttr.FixedArguments[0].Value, Is.EqualTo(43));
			Assert.That(inner2TypeTestAttr.FixedArguments[1].Value, Is.EqualTo(inner2));
			var inner2MyAttr = attributedInner2.GetAttributes().Single(a => a.AttributeType.Name == "MyAttribute");
			Assert.That(inner2MyAttr.AttributeType, Is.EqualTo(myAttribute2));
		}

		[Test]
		public void ClassWithAttributeOnTypeParameter()
		{
			var tp = GetTypeDefinition(typeof(ClassWithAttributeOnTypeParameter<>)).TypeParameters.Single();
			var attr = tp.GetAttributes().Single();
			Assert.That(attr.AttributeType.Name, Is.EqualTo("DoubleAttribute"));
		}

		[Test]
		public void InheritanceTest()
		{
			ITypeDefinition c = compilation.FindType(typeof(SystemException)).GetDefinition();
			ITypeDefinition c2 = compilation.FindType(typeof(Exception)).GetDefinition();
			Assert.That(c, Is.Not.Null, "c is null");
			Assert.That(c2, Is.Not.Null, "c2 is null");
			//Assert.AreEqual(3, c.BaseTypes.Count); // Inherited interfaces are not reported by Cecil
			// which matches the behaviour of our C#/VB parsers
			Assert.That(c.DirectBaseTypes.First().FullName, Is.EqualTo("System.Exception"));
			Assert.That(c.DirectBaseTypes.First(), Is.SameAs(c2));

			string[] superTypes = c.GetAllBaseTypes().Select(t => t.ReflectionName).ToArray();
			Assert.That(superTypes, Is.EqualTo(new string[] {
								"System.Object",
								"System.Runtime.Serialization.ISerializable", "System.Runtime.InteropServices._Exception",
								"System.Exception", "System.SystemException"
							}));
		}

		[Test]
		public void GenericPropertyTest()
		{
			ITypeDefinition c = compilation.FindType(typeof(Comparer<>)).GetDefinition();
			IProperty def = c.Properties.Single(p => p.Name == "Default");
			ParameterizedType pt = (ParameterizedType)def.ReturnType;
			Assert.That(pt.FullName, Is.EqualTo("System.Collections.Generic.Comparer"));
			Assert.That(pt.TypeArguments[0], Is.EqualTo(c.TypeParameters[0]));
		}

		[Test]
		public void PointerTypeTest()
		{
			ITypeDefinition c = compilation.FindType(typeof(IntPtr)).GetDefinition();
			IMethod toPointer = c.Methods.Single(p => p.Name == "ToPointer");
			Assert.That(toPointer.ReturnType.ReflectionName, Is.EqualTo("System.Void*"));
			Assert.That(toPointer.ReturnType is PointerType);
			Assert.That(((PointerType)toPointer.ReturnType).ElementType.FullName, Is.EqualTo("System.Void"));
		}

		[Test]
		public void DateTimeDefaultConstructor()
		{
			ITypeDefinition c = compilation.FindType(typeof(DateTime)).GetDefinition();
			Assert.That(c.Methods.Count(m => m.IsConstructor && !m.IsStatic && m.Parameters.Count == 0), Is.EqualTo(1));
			Assert.That(c.GetConstructors().Count(m => m.Parameters.Count == 0), Is.EqualTo(1));
		}

		[Test]
		public void NoEncodingInfoDefaultConstructor()
		{
			ITypeDefinition c = compilation.FindType(typeof(EncodingInfo)).GetDefinition();
			// EncodingInfo only has an internal constructor
			Assert.That(!c.Methods.Any(m => m.IsConstructor));
			// and no implicit ctor should be added:
			Assert.That(c.GetConstructors().Count(), Is.EqualTo(0));
		}

		[Test]
		public void StaticModifierTest()
		{
			ITypeDefinition c = compilation.FindType(typeof(Environment)).GetDefinition();
			Assert.That(c, Is.Not.Null, "System.Environment not found");
			Assert.That(c.IsAbstract, "class should be abstract");
			Assert.That(c.IsSealed, "class should be sealed");
			Assert.That(c.IsStatic, "class should be static");
		}

		[Test]
		public void InnerClassReferenceTest()
		{
			ITypeDefinition c = compilation.FindType(typeof(Environment)).GetDefinition();
			Assert.That(c, Is.Not.Null, "System.Environment not found");
			IType rt = c.Methods.First(m => m.Name == "GetFolderPath").Parameters[0].Type;
			Assert.That(rt, Is.SameAs(c.NestedTypes.Single(ic => ic.Name == "SpecialFolder")));
		}

		[Test]
		public void NestedTypesTest()
		{
			ITypeDefinition c = compilation.FindType(typeof(Environment.SpecialFolder)).GetDefinition();
			Assert.That(c, Is.Not.Null, "c is null");
			Assert.That(c.FullName, Is.EqualTo("System.Environment.SpecialFolder"));
			Assert.That(c.ReflectionName, Is.EqualTo("System.Environment+SpecialFolder"));
		}

		[Test]
		public void VoidHasNoMembers()
		{
			ITypeDefinition c = compilation.FindType(typeof(void)).GetDefinition();
			Assert.That(c, Is.Not.Null, "System.Void not found");
			Assert.That(c.Kind, Is.EqualTo(TypeKind.Void));
			Assert.That(c.GetMethods().Count(), Is.EqualTo(0));
			Assert.That(c.GetProperties().Count(), Is.EqualTo(0));
			Assert.That(c.GetEvents().Count(), Is.EqualTo(0));
			Assert.That(c.GetFields().Count(), Is.EqualTo(0));
		}

		[Test]
		public void Void_SerializableAttribute()
		{
			ITypeDefinition c = compilation.FindType(typeof(void)).GetDefinition();
			var attr = c.GetAttributes().Single(a => a.AttributeType.FullName == "System.SerializableAttribute");
			Assert.That(attr.Constructor.Parameters.Count, Is.EqualTo(0));
			Assert.That(attr.FixedArguments.Length, Is.EqualTo(0));
			Assert.That(attr.NamedArguments.Length, Is.EqualTo(0));
		}

		[Test]
		public void Void_StructLayoutAttribute()
		{
			ITypeDefinition c = compilation.FindType(typeof(void)).GetDefinition();
			var attr = c.GetAttributes().Single(a => a.AttributeType.FullName == "System.Runtime.InteropServices.StructLayoutAttribute");
			Assert.That(attr.Constructor.Parameters.Count, Is.EqualTo(1));
			Assert.That(attr.FixedArguments.Length, Is.EqualTo(1));
			Assert.That(attr.FixedArguments[0].Value, Is.EqualTo(0));
			Assert.That(attr.NamedArguments.Length, Is.EqualTo(1));
			Assert.That(attr.NamedArguments[0].Name, Is.EqualTo("Size"));
			Assert.That(attr.NamedArguments[0].Value, Is.EqualTo(1));
		}

		[Test]
		public void Void_ComVisibleAttribute()
		{
			ITypeDefinition c = compilation.FindType(typeof(void)).GetDefinition();
			var attr = c.GetAttributes().Single(a => a.AttributeType.FullName == "System.Runtime.InteropServices.ComVisibleAttribute");
			Assert.That(attr.Constructor.Parameters.Count, Is.EqualTo(1));
			Assert.That(attr.FixedArguments.Length, Is.EqualTo(1));
			Assert.That(attr.FixedArguments[0].Value, Is.EqualTo(true));
			Assert.That(attr.NamedArguments.Length, Is.EqualTo(0));
		}

		[Test]
		public void NestedClassInGenericClassTest()
		{
			ITypeDefinition dictionary = compilation.FindType(typeof(Dictionary<,>)).GetDefinition();
			Assert.That(dictionary, Is.Not.Null);
			ITypeDefinition valueCollection = compilation.FindType(typeof(Dictionary<,>.ValueCollection)).GetDefinition();
			Assert.That(valueCollection, Is.Not.Null);
			var dictionaryRT = new ParameterizedType(dictionary, new[] { compilation.FindType(typeof(string)).GetDefinition(), compilation.FindType(typeof(int)).GetDefinition() });
			IProperty valueProperty = dictionaryRT.GetProperties(p => p.Name == "Values").Single();
			IType parameterizedValueCollection = valueProperty.ReturnType;
			Assert.That(parameterizedValueCollection.ReflectionName, Is.EqualTo("System.Collections.Generic.Dictionary`2+ValueCollection[[System.String],[System.Int32]]"));
			Assert.That(parameterizedValueCollection.GetDefinition(), Is.SameAs(valueCollection));
		}

		[Test]
		public void ValueCollectionCountModifiers()
		{
			ITypeDefinition valueCollection = compilation.FindType(typeof(Dictionary<,>.ValueCollection)).GetDefinition();
			Assert.That(valueCollection.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(valueCollection.IsSealed);
			Assert.That(!valueCollection.IsAbstract);
			Assert.That(!valueCollection.IsStatic);

			IProperty count = valueCollection.Properties.Single(p => p.Name == "Count");
			Assert.That(count.Accessibility, Is.EqualTo(Accessibility.Public));
			// It's sealed on the IL level; but in C# it's just a normal non-virtual method that happens to implement an interface
			Assert.That(!count.IsSealed);
			Assert.That(!count.IsVirtual);
			Assert.That(!count.IsAbstract);
		}

		[Test]
		public void MathAcosModifiers()
		{
			ITypeDefinition math = compilation.FindType(typeof(Math)).GetDefinition();
			Assert.That(math.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(math.IsSealed);
			Assert.That(math.IsAbstract);
			Assert.That(math.IsStatic);

			IMethod acos = math.Methods.Single(p => p.Name == "Acos");
			Assert.That(acos.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(acos.IsStatic);
			Assert.That(!acos.IsAbstract);
			Assert.That(!acos.IsSealed);
			Assert.That(!acos.IsVirtual);
			Assert.That(!acos.IsOverride);
		}

		[Test]
		public void EncodingModifiers()
		{
			ITypeDefinition encoding = compilation.FindType(typeof(Encoding)).GetDefinition();
			Assert.That(encoding.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(!encoding.IsSealed);
			Assert.That(encoding.IsAbstract);

			IMethod getDecoder = encoding.Methods.Single(p => p.Name == "GetDecoder");
			Assert.That(getDecoder.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(!getDecoder.IsStatic);
			Assert.That(!getDecoder.IsAbstract);
			Assert.That(!getDecoder.IsSealed);
			Assert.That(getDecoder.IsVirtual);
			Assert.That(!getDecoder.IsOverride);

			IMethod getMaxByteCount = encoding.Methods.Single(p => p.Name == "GetMaxByteCount");
			Assert.That(getMaxByteCount.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(!getMaxByteCount.IsStatic);
			Assert.That(getMaxByteCount.IsAbstract);
			Assert.That(!getMaxByteCount.IsSealed);
			Assert.That(!getMaxByteCount.IsVirtual);
			Assert.That(!getMaxByteCount.IsOverride);

			IProperty encoderFallback = encoding.Properties.Single(p => p.Name == "EncoderFallback");
			Assert.That(encoderFallback.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(!encoderFallback.IsStatic);
			Assert.That(!encoderFallback.IsAbstract);
			Assert.That(!encoderFallback.IsSealed);
			Assert.That(!encoderFallback.IsVirtual);
			Assert.That(!encoderFallback.IsOverride);
		}

		[Test]
		public void UnicodeEncodingModifiers()
		{
			ITypeDefinition encoding = compilation.FindType(typeof(UnicodeEncoding)).GetDefinition();
			Assert.That(encoding.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(!encoding.IsSealed);
			Assert.That(!encoding.IsAbstract);

			IMethod getDecoder = encoding.Methods.Single(p => p.Name == "GetDecoder");
			Assert.That(getDecoder.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(!getDecoder.IsStatic);
			Assert.That(!getDecoder.IsAbstract);
			Assert.That(!getDecoder.IsSealed);
			Assert.That(!getDecoder.IsVirtual);
			Assert.That(getDecoder.IsOverride);
		}

		[Test]
		public void UTF32EncodingModifiers()
		{
			ITypeDefinition encoding = compilation.FindType(typeof(UTF32Encoding)).GetDefinition();
			Assert.That(encoding.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(encoding.IsSealed);
			Assert.That(!encoding.IsAbstract);

			IMethod getDecoder = encoding.Methods.Single(p => p.Name == "GetDecoder");
			Assert.That(getDecoder.Accessibility, Is.EqualTo(Accessibility.Public));
			Assert.That(!getDecoder.IsStatic);
			Assert.That(!getDecoder.IsAbstract);
			Assert.That(!getDecoder.IsSealed);
			Assert.That(!getDecoder.IsVirtual);
			Assert.That(getDecoder.IsOverride);
		}

		[Test]
		public void FindRedirectedType()
		{
			var compilationWithSystemCore = new SimpleCompilation(SystemCore, Mscorlib);

			var typeRef = ReflectionHelper.ParseReflectionName("System.Func`2, System.Core");
			ITypeDefinition c = typeRef.Resolve(new SimpleTypeResolveContext(compilationWithSystemCore)).GetDefinition();
			Assert.That(c, Is.Not.Null, "System.Func<,> not found");
			Assert.That(c.ParentModule.AssemblyName, Is.EqualTo("mscorlib"));
		}

		public void DelegateIsClass()
		{
			var @delegate = compilation.FindType(KnownTypeCode.Delegate).GetDefinition();
			Assert.That(@delegate, Is.EqualTo(TypeKind.Class));
			Assert.That(!@delegate.IsSealed);
		}

		public void MulticastDelegateIsClass()
		{
			var multicastDelegate = compilation.FindType(KnownTypeCode.MulticastDelegate).GetDefinition();
			Assert.That(multicastDelegate, Is.EqualTo(TypeKind.Class));
			Assert.That(!multicastDelegate.IsSealed);
		}

		[Test]
		public void HasSpecialName()
		{
			var nonCustomAttributes = compilation.FindType(typeof(NonCustomAttributes)).GetDefinition();

			var method = nonCustomAttributes.GetMethods(m => m.Name == "SpecialNameMethod").Single();
			var property = nonCustomAttributes.GetProperties(p => p.Name == "SpecialNameProperty").Single();
			var @event = nonCustomAttributes.GetEvents(e => e.Name == "SpecialNameEvent").Single();
			var field = nonCustomAttributes.GetFields(f => f.Name == "SpecialNameField").Single();

			var @class = nonCustomAttributes.GetNestedTypes(t => t.Name == "SpecialNameClass").Single().GetDefinition();
			var @struct = nonCustomAttributes.GetNestedTypes(t => t.Name == "SpecialNameStruct").Single().GetDefinition();

			Assert.That(method.HasAttribute(KnownAttribute.SpecialName));
			Assert.That(property.HasAttribute(KnownAttribute.SpecialName));
			Assert.That(@event.HasAttribute(KnownAttribute.SpecialName));
			Assert.That(field.HasAttribute(KnownAttribute.SpecialName));

			Assert.That(@class.HasAttribute(KnownAttribute.SpecialName));
			Assert.That(@struct.HasAttribute(KnownAttribute.SpecialName));
		}
	}
}
