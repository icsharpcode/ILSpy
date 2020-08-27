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
				return LoadAssembly(typeof(object).Assembly.Location);
			});

		static readonly Lazy<PEFile> systemCore = new Lazy<PEFile>(
			delegate {
				return LoadAssembly(typeof(System.Linq.Enumerable).Assembly.Location);
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
			Assert.AreEqual(typeof(SimplePublicClass).Name, c.Name);
			Assert.AreEqual(typeof(SimplePublicClass).FullName, c.FullName);
			Assert.AreEqual(typeof(SimplePublicClass).Namespace, c.Namespace);
			Assert.AreEqual(typeof(SimplePublicClass).FullName, c.ReflectionName);

			Assert.AreEqual(Accessibility.Public, c.Accessibility);
			Assert.IsFalse(c.IsAbstract);
			Assert.IsFalse(c.IsSealed);
			Assert.IsFalse(c.IsStatic);
			//Assert.IsFalse(c.IsShadowing);
		}

		[Test]
		public void SimplePublicClassMethodTest()
		{
			ITypeDefinition c = GetTypeDefinition(typeof(SimplePublicClass));

			IMethod method = c.Methods.Single(m => m.Name == "Method");
			Assert.AreEqual(typeof(SimplePublicClass).FullName + ".Method", method.FullName);
			Assert.AreSame(c, method.DeclaringType);
			Assert.AreEqual(Accessibility.Public, method.Accessibility);
			Assert.AreEqual(SymbolKind.Method, method.SymbolKind);
			Assert.IsFalse(method.IsVirtual);
			Assert.IsFalse(method.IsStatic);
			Assert.AreEqual(0, method.Parameters.Count);
			Assert.AreEqual(0, method.GetAttributes().Count());
			Assert.IsTrue(method.HasBody);
			Assert.IsNull(method.AccessorOwner);
		}

		[Test]
		public void SimplePublicClassCtorTest()
		{
			ITypeDefinition c = GetTypeDefinition(typeof(SimplePublicClass));

			IMethod method = c.Methods.Single(m => m.IsConstructor);
			Assert.AreEqual(typeof(SimplePublicClass).FullName + "..ctor", method.FullName);
			Assert.AreSame(c, method.DeclaringType);
			Assert.AreEqual(Accessibility.Public, method.Accessibility);
			Assert.AreEqual(SymbolKind.Constructor, method.SymbolKind);
			Assert.IsFalse(method.IsVirtual);
			Assert.IsFalse(method.IsStatic);
			Assert.AreEqual(0, method.Parameters.Count);
			Assert.AreEqual(0, method.GetAttributes().Count());
			Assert.IsTrue(method.HasBody);
			Assert.IsNull(method.AccessorOwner);
		}

		[Test]
		public void SimplePublicClassDtorTest()
		{
			ITypeDefinition c = GetTypeDefinition(typeof(SimplePublicClass));

			IMethod method = c.Methods.Single(m => m.IsDestructor);
			Assert.AreEqual(typeof(SimplePublicClass).FullName + ".Finalize", method.FullName);
			Assert.AreSame(c, method.DeclaringType);
			Assert.AreEqual(Accessibility.Protected, method.Accessibility);
			Assert.AreEqual(SymbolKind.Destructor, method.SymbolKind);
			Assert.IsFalse(method.IsVirtual);
			Assert.IsFalse(method.IsStatic);
			Assert.AreEqual(0, method.Parameters.Count);
			Assert.AreEqual(1, method.GetAttributes().Count());
			Assert.IsTrue(method.HasBody);
			Assert.IsNull(method.AccessorOwner);
		}

		[Test]
		public void DynamicType()
		{
			ITypeDefinition testClass = GetTypeDefinition(typeof(DynamicTest));
			Assert.AreEqual(SpecialType.Dynamic, testClass.Fields.Single(f => f.Name == "DynamicField").ReturnType);
			Assert.AreEqual(SpecialType.Dynamic, testClass.Properties.Single().ReturnType);
			Assert.AreEqual(0, testClass.Properties.Single().GetAttributes().Count());
		}

		[Test]
		public void DynamicTypeInGenerics()
		{
			ITypeDefinition testClass = GetTypeDefinition(typeof(DynamicTest));

			IMethod m1 = testClass.Methods.Single(me => me.Name == "DynamicGenerics1");
			Assert.AreEqual("System.Collections.Generic.List`1[[dynamic]]", m1.ReturnType.ReflectionName);
			Assert.AreEqual("System.Action`3[[System.Object],[dynamic[]],[System.Object]]", m1.Parameters[0].Type.ReflectionName);

			IMethod m2 = testClass.Methods.Single(me => me.Name == "DynamicGenerics2");
			Assert.AreEqual("System.Action`3[[System.Object],[dynamic],[System.Object]]", m2.Parameters[0].Type.ReflectionName);

			IMethod m3 = testClass.Methods.Single(me => me.Name == "DynamicGenerics3");
			Assert.AreEqual("System.Action`3[[System.Int32],[dynamic],[System.Object]]", m3.Parameters[0].Type.ReflectionName);

			IMethod m4 = testClass.Methods.Single(me => me.Name == "DynamicGenerics4");
			Assert.AreEqual("System.Action`3[[System.Int32[]],[dynamic],[System.Object]]", m4.Parameters[0].Type.ReflectionName);

			IMethod m5 = testClass.Methods.Single(me => me.Name == "DynamicGenerics5");
			Assert.AreEqual("System.Action`3[[System.Int32*[]],[dynamic],[System.Object]]", m5.Parameters[0].Type.ReflectionName);

			IMethod m6 = testClass.Methods.Single(me => me.Name == "DynamicGenerics6");
			Assert.AreEqual("System.Action`3[[System.Object],[dynamic],[System.Object]]&", m6.Parameters[0].Type.ReflectionName);

			IMethod m7 = testClass.Methods.Single(me => me.Name == "DynamicGenerics7");
			Assert.AreEqual("System.Action`3[[System.Int32[][,]],[dynamic],[System.Object]]", m7.Parameters[0].Type.ReflectionName);
		}

		[Test]
		public void DynamicParameterHasNoAttributes()
		{
			ITypeDefinition testClass = GetTypeDefinition(typeof(DynamicTest));
			IMethod m1 = testClass.Methods.Single(me => me.Name == "DynamicGenerics1");
			Assert.AreEqual(0, m1.Parameters[0].GetAttributes().Count());
		}

		[Test]
		public void AssemblyAttribute()
		{
			var attributes = compilation.MainModule.GetAssemblyAttributes().ToList();
			var typeTest = attributes.Single(a => a.AttributeType.FullName == typeof(TypeTestAttribute).FullName);
			Assert.AreEqual(3, typeTest.FixedArguments.Length);
			// first argument is (int)42
			Assert.AreEqual(42, (int)typeTest.FixedArguments[0].Value);
			// second argument is typeof(System.Action<>)
			var ty = (IType)typeTest.FixedArguments[1].Value;
			Assert.IsFalse(ty is ParameterizedType); // rt must not be constructed - it's just an unbound type
			Assert.AreEqual("System.Action", ty.FullName);
			Assert.AreEqual(1, ty.TypeParameterCount);
			// third argument is typeof(IDictionary<string, IList<TestAttribute>>)
			var crt = (ParameterizedType)typeTest.FixedArguments[2].Value;
			Assert.AreEqual("System.Collections.Generic.IDictionary", crt.FullName);
			Assert.AreEqual("System.String", crt.TypeArguments[0].FullName);
			// we know the name for TestAttribute, but not necessarily the namespace, as NUnit is not in the compilation
			Assert.AreEqual("System.Collections.Generic.IList", crt.TypeArguments[1].FullName);
			var testAttributeType = ((ParameterizedType)crt.TypeArguments[1]).TypeArguments.Single();
			Assert.AreEqual("TestAttribute", testAttributeType.Name);
			Assert.AreEqual(TypeKind.Unknown, testAttributeType.Kind);
			// (more accurately, we know the namespace and reflection name if the type was loaded by cecil,
			// but not if we parsed it from C#)
		}

		[Test]
		public void TypeForwardedTo_Attribute()
		{
			var attributes = compilation.MainModule.GetAssemblyAttributes().ToList();
			var forwardAttribute = attributes.Single(a => a.AttributeType.FullName == typeof(TypeForwardedToAttribute).FullName);
			Assert.AreEqual(1, forwardAttribute.FixedArguments.Length);
			var rt = (IType)forwardAttribute.FixedArguments[0].Value;
			Assert.AreEqual("System.Func`2", rt.ReflectionName);
		}

		[Test]
		public void TestClassTypeParameters()
		{
			var testClass = GetTypeDefinition(typeof(GenericClass<,>));
			Assert.AreEqual(SymbolKind.TypeDefinition, testClass.TypeParameters[0].OwnerType);
			Assert.AreEqual(SymbolKind.TypeDefinition, testClass.TypeParameters[1].OwnerType);
			Assert.AreSame(testClass.TypeParameters[1], testClass.TypeParameters[0].DirectBaseTypes.First());
		}

		[Test]
		public void TestMethod()
		{
			var testClass = GetTypeDefinition(typeof(GenericClass<,>));

			IMethod m = testClass.Methods.Single(me => me.Name == "TestMethod");
			Assert.AreEqual("K", m.TypeParameters[0].Name);
			Assert.AreEqual("V", m.TypeParameters[1].Name);
			Assert.AreEqual(SymbolKind.Method, m.TypeParameters[0].OwnerType);
			Assert.AreEqual(SymbolKind.Method, m.TypeParameters[1].OwnerType);

			Assert.AreEqual("System.IComparable`1[[``1]]", m.TypeParameters[0].DirectBaseTypes.First().ReflectionName);
			Assert.AreSame(m.TypeParameters[0], m.TypeParameters[1].DirectBaseTypes.First());
		}

		[Test]
		public void GetIndex()
		{
			var testClass = GetTypeDefinition(typeof(GenericClass<,>));

			IMethod m = testClass.Methods.Single(me => me.Name == "GetIndex");
			Assert.AreEqual("T", m.TypeParameters[0].Name);
			Assert.AreEqual(SymbolKind.Method, m.TypeParameters[0].OwnerType);
			Assert.AreSame(m, m.TypeParameters[0].Owner);

			ParameterizedType constraint = (ParameterizedType)m.TypeParameters[0].DirectBaseTypes.First();
			Assert.AreEqual("IEquatable", constraint.Name);
			Assert.AreEqual(1, constraint.TypeParameterCount);
			Assert.AreEqual(1, constraint.TypeArguments.Count);
			Assert.AreSame(m.TypeParameters[0], constraint.TypeArguments[0]);
			Assert.AreSame(m.TypeParameters[0], m.Parameters[0].Type);
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

			Assert.AreEqual("T", m.TypeParameters[0].Name);
			Assert.AreEqual(SymbolKind.Method, m.TypeParameters[0].OwnerType);
			Assert.AreSame(m, m.TypeParameters[0].Owner);

			ParameterizedType constraint = (ParameterizedType)m.TypeParameters[0].DirectBaseTypes.First();
			Assert.AreEqual("IEquatable", constraint.Name);
			Assert.AreEqual(1, constraint.TypeParameterCount);
			Assert.AreEqual(1, constraint.TypeArguments.Count);
			Assert.AreSame(m.TypeParameters[0], constraint.TypeArguments[0]);
			Assert.AreSame(m.TypeParameters[0], m.Parameters[0].Type);
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
			Assert.AreEqual(m12, m2);
		}

		[Test]
		public void SpecializedMethod_AccessorOwner()
		{
			// NRefactory bug #143 - Accessor Owner throws null reference exception in some cases now
			var method = compilation.FindType(typeof(GenericClass<string, object>)).GetMethods(m => m.Name == "GetIndex").Single();
			Assert.IsNull(method.AccessorOwner);
		}

		[Test]
		public void Specialized_GetIndex_ToMemberReference()
		{
			var method = compilation.FindType(typeof(GenericClass<string, object>)).GetMethods(m => m.Name == "GetIndex").Single();
			Assert.AreSame(method.TypeParameters[0], method.Parameters[0].Type);
			Assert.AreSame(method, method.TypeParameters[0].Owner);
			Assert.IsInstanceOf<SpecializedMethod>(method);
			//Assert.IsFalse(method.IsParameterized); // the method itself is not specialized
			Assert.AreEqual(method.TypeParameters, method.TypeArguments);
		}

		[Test]
		public void Specialized_GetIndex_SpecializeWithIdentityHasNoEffect()
		{
			var genericClass = compilation.FindType(typeof(GenericClass<string, object>));
			IType[] methodTypeArguments = { DummyTypeParameter.GetMethodTypeParameter(0) };
			var method = genericClass.GetMethods(methodTypeArguments, m => m.Name == "GetIndex").Single();
			// GenericClass<string,object>.GetIndex<!!0>()
			Assert.AreSame(method, method.TypeParameters[0].Owner);
			Assert.AreNotEqual(method.TypeParameters[0], method.TypeArguments[0]);
			Assert.IsNull(((ITypeParameter)method.TypeArguments[0]).Owner);
			// Now apply identity substitution:
			var method2 = method.Specialize(TypeParameterSubstitution.Identity);
			Assert.AreSame(method2, method2.TypeParameters[0].Owner);
			Assert.AreNotEqual(method2.TypeParameters[0], method2.TypeArguments[0]);
			Assert.IsNull(((ITypeParameter)method2.TypeArguments[0]).Owner);

			Assert.AreEqual(method, method2);
		}

		[Test]
		public void GenericEnum()
		{
			var testClass = GetTypeDefinition(typeof(GenericClass<,>.NestedEnum));
			Assert.AreEqual(2, testClass.TypeParameterCount);
		}

		[Test]
		public void FieldInGenericClassWithNestedEnumType()
		{
			var testClass = GetTypeDefinition(typeof(GenericClass<,>));
			var enumClass = GetTypeDefinition(typeof(GenericClass<,>.NestedEnum));
			var field = testClass.Fields.Single(f => f.Name == "EnumField");
			Assert.AreEqual(new ParameterizedType(enumClass, testClass.TypeParameters), field.ReturnType);
		}

		[Test]
		public void GenericEnumMemberReturnType()
		{
			var enumClass = GetTypeDefinition(typeof(GenericClass<,>.NestedEnum));
			var field = enumClass.Fields.Single(f => f.Name == "EnumMember");
			Assert.AreEqual(new ParameterizedType(enumClass, enumClass.TypeParameters), field.ReturnType);
		}

		[Test]
		public void PropertyWithProtectedSetter()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.Name == "PropertyWithProtectedSetter");
			Assert.IsTrue(p.CanGet);
			Assert.IsTrue(p.CanSet);
			Assert.AreEqual(Accessibility.Public, p.Accessibility);
			Assert.AreEqual(Accessibility.Public, p.Getter.Accessibility);
			Assert.AreEqual(Accessibility.Protected, p.Setter.Accessibility);
		}

		[Test]
		public void PropertyWithPrivateSetter()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.Name == "PropertyWithPrivateSetter");
			Assert.IsTrue(p.CanGet);
			Assert.IsTrue(p.CanSet);
			Assert.AreEqual(Accessibility.Public, p.Accessibility);
			Assert.AreEqual(Accessibility.Public, p.Getter.Accessibility);
			Assert.AreEqual(Accessibility.Private, p.Setter.Accessibility);
			Assert.IsTrue(p.Getter.HasBody);
		}

		[Test]
		public void PropertyWithPrivateGetter()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.Name == "PropertyWithPrivateGetter");
			Assert.IsTrue(p.CanGet);
			Assert.IsTrue(p.CanSet);
			Assert.AreEqual(Accessibility.Public, p.Accessibility);
			Assert.AreEqual(Accessibility.Private, p.Getter.Accessibility);
			Assert.AreEqual(Accessibility.Public, p.Setter.Accessibility);
			Assert.IsTrue(p.Getter.HasBody);
		}

		[Test]
		public void PropertyWithoutSetter()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.Name == "PropertyWithoutSetter");
			Assert.IsTrue(p.CanGet);
			Assert.IsFalse(p.CanSet);
			Assert.AreEqual(Accessibility.Public, p.Accessibility);
			Assert.AreEqual(Accessibility.Public, p.Getter.Accessibility);
			Assert.IsNull(p.Setter);
		}

		[Test]
		public void Indexer()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.IsIndexer);
			Assert.AreEqual("Item", p.Name);
			Assert.AreEqual(new[] { "index" }, p.Parameters.Select(x => x.Name).ToArray());
		}

		[Test]
		public void IndexerGetter()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.IsIndexer);
			Assert.IsTrue(p.CanGet);
			Assert.AreEqual(SymbolKind.Accessor, p.Getter.SymbolKind);
			Assert.AreEqual("get_Item", p.Getter.Name);
			Assert.AreEqual(Accessibility.Public, p.Getter.Accessibility);
			Assert.AreEqual(new[] { "index" }, p.Getter.Parameters.Select(x => x.Name).ToArray());
			Assert.AreEqual("System.String", p.Getter.ReturnType.ReflectionName);
			Assert.AreEqual(p, p.Getter.AccessorOwner);
		}

		[Test]
		public void IndexerSetter()
		{
			var testClass = GetTypeDefinition(typeof(PropertyTest));
			IProperty p = testClass.Properties.Single(pr => pr.IsIndexer);
			Assert.IsTrue(p.CanSet);
			Assert.AreEqual(SymbolKind.Accessor, p.Setter.SymbolKind);
			Assert.AreEqual("set_Item", p.Setter.Name);
			Assert.AreEqual(Accessibility.Public, p.Setter.Accessibility);
			Assert.AreEqual(new[] { "index", "value" }, p.Setter.Parameters.Select(x => x.Name).ToArray());
			Assert.AreEqual(TypeKind.Void, p.Setter.ReturnType.Kind);
		}

		[Test]
		public void GenericPropertyGetter()
		{
			var type = compilation.FindType(typeof(GenericClass<string, object>));
			var prop = type.GetProperties(p => p.Name == "Property").Single();
			Assert.AreEqual("System.String", prop.Getter.ReturnType.ReflectionName);
			Assert.IsTrue(prop.Getter.IsAccessor);
			Assert.AreEqual(prop, prop.Getter.AccessorOwner);
		}

		[Test]
		public void EnumTest()
		{
			var e = GetTypeDefinition(typeof(MyEnum));
			Assert.AreEqual(TypeKind.Enum, e.Kind);
			Assert.AreEqual(false, e.IsReferenceType);
			Assert.AreEqual("System.Int16", e.EnumUnderlyingType.ReflectionName);
			Assert.AreEqual(new[] { "System.Enum" }, e.DirectBaseTypes.Select(t => t.ReflectionName).ToArray());
		}

		[Test]
		public void EnumFieldsTest()
		{
			var e = GetTypeDefinition(typeof(MyEnum));
			IField valueField = e.Fields.First();
			IField[] fields = e.Fields.Skip(1).ToArray();
			Assert.AreEqual(5, fields.Length);

			Assert.AreEqual("value__", valueField.Name);
			Assert.AreEqual(GetTypeDefinition(typeof(short)), valueField.Type);
			Assert.AreEqual(Accessibility.Public, valueField.Accessibility);
			Assert.AreEqual(null, valueField.GetConstantValue());
			Assert.IsFalse(valueField.IsConst);
			Assert.IsFalse(valueField.IsStatic);

			foreach (IField f in fields)
			{
				Assert.IsTrue(f.IsStatic);
				Assert.IsTrue(f.IsConst);
				Assert.AreEqual(Accessibility.Public, f.Accessibility);
				Assert.AreSame(e, f.Type);
				Assert.AreEqual(typeof(short), f.GetConstantValue().GetType());
			}

			Assert.AreEqual("First", fields[0].Name);
			Assert.AreEqual(0, fields[0].GetConstantValue());

			Assert.AreEqual("Second", fields[1].Name);
			Assert.AreSame(e, fields[1].Type);
			Assert.AreEqual(1, fields[1].GetConstantValue());

			Assert.AreEqual("Flag1", fields[2].Name);
			Assert.AreEqual(0x10, fields[2].GetConstantValue());

			Assert.AreEqual("Flag2", fields[3].Name);
			Assert.AreEqual(0x20, fields[3].GetConstantValue());

			Assert.AreEqual("CombinedFlags", fields[4].Name);
			Assert.AreEqual(0x30, fields[4].GetConstantValue());
		}

		[Test]
		public void GetNestedTypesFromBaseClassTest()
		{
			ITypeDefinition d = GetTypeDefinition(typeof(Derived<,>));

			IType pBase = d.DirectBaseTypes.Single();
			Assert.AreEqual(typeof(Base<>).FullName + "[[`1]]", pBase.ReflectionName);
			// Base[`1].GetNestedTypes() = { Base`1+Nested`1[`1, unbound] }
			Assert.AreEqual(new[] { typeof(Base<>.Nested<>).FullName + "[[`1],[]]" },
							pBase.GetNestedTypes().Select(n => n.ReflectionName).ToArray());

			// Derived.GetNestedTypes() = { Base`1+Nested`1[`1, unbound] }
			Assert.AreEqual(new[] { typeof(Base<>.Nested<>).FullName + "[[`1],[]]" },
							d.GetNestedTypes().Select(n => n.ReflectionName).ToArray());
			// This is 'leaking' the type parameter from B as is usual when retrieving any members from an unbound type.
		}

		[Test]
		public void ParameterizedTypeGetNestedTypesFromBaseClassTest()
		{
			// Derived[string,int].GetNestedTypes() = { Base`1+Nested`1[int, unbound] }
			var d = compilation.FindType(typeof(Derived<string, int>));
			Assert.AreEqual(new[] { typeof(Base<>.Nested<>).FullName + "[[System.Int32],[]]" },
							d.GetNestedTypes().Select(n => n.ReflectionName).ToArray());
		}

		[Test]
		public void ConstraintsOnOverrideAreInherited()
		{
			ITypeDefinition d = GetTypeDefinition(typeof(Derived<,>));
			ITypeParameter tp = d.Methods.Single(m => m.Name == "GenericMethodWithConstraints").TypeParameters.Single();
			Assert.AreEqual("Y", tp.Name);
			Assert.IsFalse(tp.HasValueTypeConstraint);
			Assert.IsFalse(tp.HasReferenceTypeConstraint);
			Assert.IsTrue(tp.HasDefaultConstructorConstraint);
			Assert.AreEqual(new string[] { "System.Collections.Generic.IComparer`1[[`1]]", "System.Object" },
							tp.DirectBaseTypes.Select(t => t.ReflectionName).ToArray());
		}

		[Test]
		public void DtorInDerivedClass()
		{
			ITypeDefinition c = GetTypeDefinition(typeof(Derived<,>));
			IMethod method = c.Methods.Single(m => m.IsDestructor);
			Assert.AreEqual(c.FullName + ".Finalize", method.FullName);
			Assert.AreSame(c, method.DeclaringType);
			Assert.AreEqual(Accessibility.Protected, method.Accessibility);
			Assert.AreEqual(SymbolKind.Destructor, method.SymbolKind);
			Assert.IsFalse(method.IsVirtual);
			Assert.IsFalse(method.IsStatic);
			Assert.AreEqual(0, method.Parameters.Count);
			Assert.AreEqual(0, method.GetAttributes().Count());
			Assert.IsTrue(method.HasBody);
			Assert.IsNull(method.AccessorOwner);
		}

		[Test]
		public void PrivateFinalizeMethodIsNotADtor()
		{
			ITypeDefinition c = GetTypeDefinition(typeof(TypeTestAttribute));
			IMethod method = c.Methods.Single(m => m.Name == "Finalize");
			Assert.AreEqual(c.FullName + ".Finalize", method.FullName);
			Assert.AreSame(c, method.DeclaringType);
			Assert.AreEqual(Accessibility.Private, method.Accessibility);
			Assert.AreEqual(SymbolKind.Method, method.SymbolKind);
			Assert.IsFalse(method.IsVirtual);
			Assert.IsFalse(method.IsStatic);
			Assert.AreEqual(0, method.Parameters.Count);
			Assert.AreEqual(0, method.GetAttributes().Count());
			Assert.IsTrue(method.HasBody);
			Assert.IsNull(method.AccessorOwner);
		}

		[Test]
		public void DefaultConstructorAddedToStruct()
		{
			var ctors = compilation.FindType(typeof(MyStructWithCtor)).GetConstructors();
			Assert.AreEqual(2, ctors.Count());
			Assert.IsFalse(ctors.Any(c => c.IsStatic));
			Assert.IsTrue(ctors.All(c => c.ReturnType.Kind == TypeKind.Void));
			Assert.IsTrue(ctors.All(c => c.Accessibility == Accessibility.Public));
		}

		[Test]
		public void NoDefaultConstructorAddedToClass()
		{
			var ctors = compilation.FindType(typeof(MyClassWithCtor)).GetConstructors();
			Assert.AreEqual(Accessibility.Private, ctors.Single().Accessibility);
			Assert.AreEqual(1, ctors.Single().Parameters.Count);
		}

		[Test]
		public void DefaultConstructorOnAbstractClassIsProtected()
		{
			var ctors = compilation.FindType(typeof(AbstractClass)).GetConstructors();
			Assert.AreEqual(0, ctors.Single().Parameters.Count);
			Assert.AreEqual(Accessibility.Protected, ctors.Single().Accessibility);
		}

		[Test]
		public void SerializableAttribute()
		{
			IAttribute attr = GetTypeDefinition(typeof(NonCustomAttributes)).GetAttributes().Single();
			Assert.AreEqual("System.SerializableAttribute", attr.AttributeType.FullName);
		}

		[Test]
		public void NonSerializedAttribute()
		{
			IField field = GetTypeDefinition(typeof(NonCustomAttributes)).Fields.Single(f => f.Name == "NonSerializedField");
			Assert.AreEqual("System.NonSerializedAttribute", field.GetAttributes().Single().AttributeType.FullName);
		}

		[Test]
		public void ExplicitStructLayoutAttribute()
		{
			IAttribute attr = GetTypeDefinition(typeof(ExplicitFieldLayoutStruct)).GetAttributes().Single();
			Assert.AreEqual("System.Runtime.InteropServices.StructLayoutAttribute", attr.AttributeType.FullName);
			var arg1 = attr.FixedArguments.Single();
			Assert.AreEqual("System.Runtime.InteropServices.LayoutKind", arg1.Type.FullName);
			Assert.AreEqual((int)LayoutKind.Explicit, arg1.Value);

			var arg2 = attr.NamedArguments[0];
			Assert.AreEqual("CharSet", arg2.Name);
			Assert.AreEqual("System.Runtime.InteropServices.CharSet", arg2.Type.FullName);
			Assert.AreEqual((int)CharSet.Unicode, arg2.Value);

			var arg3 = attr.NamedArguments[1];
			Assert.AreEqual("Pack", arg3.Name);
			Assert.AreEqual("System.Int32", arg3.Type.FullName);
			Assert.AreEqual(8, arg3.Value);
		}

		[Test]
		public void FieldOffsetAttribute()
		{
			IField field = GetTypeDefinition(typeof(ExplicitFieldLayoutStruct)).Fields.Single(f => f.Name == "Field0");
			Assert.AreEqual("System.Runtime.InteropServices.FieldOffsetAttribute", field.GetAttributes().Single().AttributeType.FullName);
			var arg = field.GetAttributes().Single().FixedArguments.Single();
			Assert.AreEqual("System.Int32", arg.Type.FullName);
			Assert.AreEqual(0, arg.Value);

			field = GetTypeDefinition(typeof(ExplicitFieldLayoutStruct)).Fields.Single(f => f.Name == "Field100");
			Assert.AreEqual("System.Runtime.InteropServices.FieldOffsetAttribute", field.GetAttributes().Single().AttributeType.FullName);
			arg = field.GetAttributes().Single().FixedArguments.Single();
			Assert.AreEqual("System.Int32", arg.Type.FullName);
			Assert.AreEqual(100, arg.Value);
		}

		[Test]
		public void DllImportAttribute()
		{
			IMethod method = GetTypeDefinition(typeof(NonCustomAttributes)).Methods.Single(m => m.Name == "DllMethod");
			IAttribute dllImport = method.GetAttributes().Single();
			Assert.AreEqual("System.Runtime.InteropServices.DllImportAttribute", dllImport.AttributeType.FullName);
			Assert.AreEqual("unmanaged.dll", dllImport.FixedArguments[0].Value);
			Assert.AreEqual((int)CharSet.Unicode, dllImport.NamedArguments.Single().Value);
		}

		[Test]
		public void DllImportAttributeWithPreserveSigFalse()
		{
			IMethod method = GetTypeDefinition(typeof(NonCustomAttributes)).Methods.Single(m => m.Name == "DoNotPreserveSig");
			IAttribute dllImport = method.GetAttributes().Single();
			Assert.AreEqual("System.Runtime.InteropServices.DllImportAttribute", dllImport.AttributeType.FullName);
			Assert.AreEqual("unmanaged.dll", dllImport.FixedArguments[0].Value);
			Assert.AreEqual(false, dllImport.NamedArguments.Single().Value);
		}

		[Test]
		public void PreserveSigAttribute()
		{
			IMethod method = GetTypeDefinition(typeof(NonCustomAttributes)).Methods.Single(m => m.Name == "PreserveSigAsAttribute");
			IAttribute preserveSig = method.GetAttributes().Single();
			Assert.AreEqual("System.Runtime.InteropServices.PreserveSigAttribute", preserveSig.AttributeType.FullName);
			Assert.IsTrue(preserveSig.FixedArguments.Length == 0);
			Assert.IsTrue(preserveSig.NamedArguments.Length == 0);
		}

		[Test]
		public void InOutParametersOnRefMethod()
		{
			IParameter p = GetTypeDefinition(typeof(NonCustomAttributes)).Methods.Single(m => m.Name == "DllMethod").Parameters.Single();
			Assert.AreEqual(ReferenceKind.Ref, p.ReferenceKind);
			var attr = p.GetAttributes().ToList();
			Assert.AreEqual(2, attr.Count);
			Assert.AreEqual("System.Runtime.InteropServices.InAttribute", attr[0].AttributeType.FullName);
			Assert.AreEqual("System.Runtime.InteropServices.OutAttribute", attr[1].AttributeType.FullName);
		}

		[Test]
		public void MarshalAsAttributeOnMethod()
		{
			IMethod method = GetTypeDefinition(typeof(NonCustomAttributes)).Methods.Single(m => m.Name == "DllMethod");
			IAttribute marshalAs = method.GetReturnTypeAttributes().Single();
			Assert.AreEqual((int)UnmanagedType.Bool, marshalAs.FixedArguments.Single().Value);
		}

		[Test]
		public void MethodWithOutParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithOutParameter").Parameters.Single();
			Assert.IsFalse(p.IsOptional);
			Assert.AreEqual(ReferenceKind.Out, p.ReferenceKind);
			Assert.AreEqual(0, p.GetAttributes().Count());
			Assert.IsTrue(p.Type.Kind == TypeKind.ByReference);
		}

		[Test]
		public void MethodWithRefParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithRefParameter").Parameters.Single();
			Assert.IsFalse(p.IsOptional);
			Assert.AreEqual(ReferenceKind.Ref, p.ReferenceKind);
			Assert.AreEqual(0, p.GetAttributes().Count());
			Assert.IsTrue(p.Type.Kind == TypeKind.ByReference);
		}

		[Test]
		public void MethodWithInParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithInParameter").Parameters.Single();
			Assert.IsFalse(p.IsOptional);
			Assert.AreEqual(ReferenceKind.In, p.ReferenceKind);
			Assert.AreEqual(0, p.GetAttributes().Count());
			Assert.IsTrue(p.Type.Kind == TypeKind.ByReference);
		}

		[Test]
		public void MethodWithParamsArray()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithParamsArray").Parameters.Single();
			Assert.IsFalse(p.IsOptional);
			Assert.AreEqual(ReferenceKind.None, p.ReferenceKind);
			Assert.IsTrue(p.IsParams);
			Assert.AreEqual(0, p.GetAttributes().Count());
			Assert.IsTrue(p.Type.Kind == TypeKind.Array);
		}

		[Test]
		public void MethodWithOptionalParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithOptionalParameter").Parameters.Single();
			Assert.IsTrue(p.IsOptional);
			Assert.AreEqual(ReferenceKind.None, p.ReferenceKind);
			Assert.IsFalse(p.IsParams);
			Assert.IsTrue(p.HasConstantValueInSignature);
			Assert.AreEqual(0, p.GetAttributes().Count());
			Assert.AreEqual(4, p.GetConstantValue());
		}

		[Test]
		public void MethodWithExplicitOptionalParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithExplicitOptionalParameter").Parameters.Single();
			Assert.IsTrue(p.IsOptional);
			Assert.AreEqual(ReferenceKind.None, p.ReferenceKind);
			Assert.IsFalse(p.IsParams);
			Assert.IsFalse(p.HasConstantValueInSignature);
			// explicit optional parameter appears in type system if it's read from C#, but not when read from IL
			//Assert.AreEqual(1, p.GetAttributes().Count());
		}

		[Test]
		public void MethodWithEnumOptionalParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithEnumOptionalParameter").Parameters.Single();
			Assert.IsTrue(p.IsOptional);
			Assert.AreEqual(ReferenceKind.None, p.ReferenceKind);
			Assert.IsFalse(p.IsParams);
			Assert.IsTrue(p.HasConstantValueInSignature);
			Assert.AreEqual(0, p.GetAttributes().Count());
			Assert.AreEqual((int)StringComparison.OrdinalIgnoreCase, p.GetConstantValue());
		}

		[Test]
		public void MethodWithOptionalNullableParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithOptionalNullableParameter").Parameters.Single();
			Assert.IsTrue(p.IsOptional);
			Assert.AreEqual(ReferenceKind.None, p.ReferenceKind);
			Assert.IsFalse(p.IsParams);
			Assert.IsTrue(p.HasConstantValueInSignature);
			Assert.AreEqual(0, p.GetAttributes().Count());
			Assert.IsNull(p.GetConstantValue());
		}

		[Test]
		public void MethodWithOptionalLongParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithOptionalLongParameter").Parameters.Single();
			Assert.IsTrue(p.IsOptional);
			Assert.AreEqual(ReferenceKind.None, p.ReferenceKind);
			Assert.IsFalse(p.IsParams);
			Assert.IsTrue(p.HasConstantValueInSignature);
			Assert.AreEqual(1L, p.GetConstantValue());
			Assert.AreEqual(typeof(long), p.GetConstantValue().GetType());
		}

		[Test]
		public void MethodWithOptionalNullableLongParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithOptionalNullableLongParameter").Parameters.Single();
			Assert.IsTrue(p.IsOptional);
			Assert.AreEqual(ReferenceKind.None, p.ReferenceKind);
			Assert.IsFalse(p.IsParams);
			Assert.IsTrue(p.HasConstantValueInSignature);
			Assert.AreEqual(1L, p.GetConstantValue());
			Assert.AreEqual(typeof(long), p.GetConstantValue().GetType());
		}

		[Test]
		public void MethodWithOptionalDecimalParameter()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "MethodWithOptionalDecimalParameter").Parameters.Single();
			Assert.IsTrue(p.IsOptional);
			Assert.AreEqual(ReferenceKind.None, p.ReferenceKind);
			Assert.IsFalse(p.IsParams);
			Assert.IsTrue(p.HasConstantValueInSignature);
			Assert.AreEqual(1M, p.GetConstantValue());
			Assert.AreEqual(typeof(decimal), p.GetConstantValue().GetType());
		}

		[Test]
		public void VarArgsMethod()
		{
			IParameter p = GetTypeDefinition(typeof(ParameterTests)).Methods.Single(m => m.Name == "VarArgsMethod").Parameters.Single();
			Assert.IsFalse(p.IsOptional);
			Assert.AreEqual(ReferenceKind.None, p.ReferenceKind);
			Assert.IsFalse(p.IsParams);
			Assert.AreEqual(TypeKind.ArgList, p.Type.Kind);
			Assert.AreEqual("", p.Name);
		}

		[Test]
		public void VarArgsCtor()
		{
			IParameter p = GetTypeDefinition(typeof(VarArgsCtor)).Methods.Single(m => m.IsConstructor).Parameters.Single();
			Assert.IsFalse(p.IsOptional);
			Assert.AreEqual(ReferenceKind.None, p.ReferenceKind);
			Assert.IsFalse(p.IsParams);
			Assert.AreEqual(TypeKind.ArgList, p.Type.Kind);
			Assert.AreEqual("", p.Name);
		}

		[Test]
		public void GenericDelegate_Variance()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(GenericDelegate<,>));
			Assert.AreEqual(VarianceModifier.Contravariant, type.TypeParameters[0].Variance);
			Assert.AreEqual(VarianceModifier.Covariant, type.TypeParameters[1].Variance);

			Assert.AreSame(type.TypeParameters[1], type.TypeParameters[0].DirectBaseTypes.FirstOrDefault());
		}

		[Test]
		public void GenericDelegate_ReferenceTypeConstraints()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(GenericDelegate<,>));
			Assert.IsFalse(type.TypeParameters[0].HasReferenceTypeConstraint);
			Assert.IsTrue(type.TypeParameters[1].HasReferenceTypeConstraint);

			Assert.IsNull(type.TypeParameters[0].IsReferenceType);
			Assert.AreEqual(true, type.TypeParameters[1].IsReferenceType);
		}

		[Test]
		public void GenericDelegate_GetInvokeMethod()
		{
			IType type = compilation.FindType(typeof(GenericDelegate<string, object>));
			IMethod m = type.GetDelegateInvokeMethod();
			Assert.AreEqual("Invoke", m.Name);
			Assert.AreEqual("System.Object", m.ReturnType.FullName);
			Assert.AreEqual("System.String", m.Parameters[0].Type.FullName);
		}

		[Test]
		public void ComInterfaceTest()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(IAssemblyEnum));
			// [ComImport]
			Assert.AreEqual(1, type.GetAttributes().Count(a => a.AttributeType.FullName == typeof(ComImportAttribute).FullName));

			IMethod m = type.Methods.Single();
			Assert.AreEqual("GetNextAssembly", m.Name);
			Assert.AreEqual(Accessibility.Public, m.Accessibility);
			Assert.IsTrue(m.IsAbstract);
			Assert.IsFalse(m.IsVirtual);
			Assert.IsFalse(m.IsSealed);
		}

		[Test]
		public void InnerClassInGenericClassIsReferencedUsingParameterizedType()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(OuterGeneric<>));
			IField field1 = type.Fields.Single(f => f.Name == "Field1");
			IField field2 = type.Fields.Single(f => f.Name == "Field2");
			IField field3 = type.Fields.Single(f => f.Name == "Field3");

			// types must be self-parameterized
			Assert.AreEqual("ICSharpCode.Decompiler.Tests.TypeSystem.OuterGeneric`1+Inner[[`0]]", field1.Type.ReflectionName);
			Assert.AreEqual("ICSharpCode.Decompiler.Tests.TypeSystem.OuterGeneric`1+Inner[[`0]]", field2.Type.ReflectionName);
			Assert.AreEqual("ICSharpCode.Decompiler.Tests.TypeSystem.OuterGeneric`1+Inner[[ICSharpCode.Decompiler.Tests.TypeSystem.OuterGeneric`1+Inner[[`0]]]]", field3.Type.ReflectionName);
		}

		[Test]
		public void FlagsOnInterfaceMembersAreCorrect()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(IInterfaceWithProperty));
			var p = type.Properties.Single();
			Assert.AreEqual(false, p.IsIndexer);
			Assert.AreEqual(true, p.IsAbstract);
			Assert.AreEqual(true, p.IsOverridable);
			Assert.AreEqual(false, p.IsOverride);
			Assert.AreEqual(Accessibility.Public, p.Accessibility);
			Assert.AreEqual(true, p.Getter.IsAbstract);
			Assert.AreEqual(true, p.Getter.IsOverridable);
			Assert.AreEqual(false, p.Getter.IsOverride);
			Assert.AreEqual(Accessibility.Public, p.Getter.Accessibility);
			Assert.AreEqual(false, p.Getter.HasBody);
			Assert.AreEqual(true, p.Setter.IsAbstract);
			Assert.AreEqual(true, p.Setter.IsOverridable);
			Assert.AreEqual(false, p.Setter.IsOverride);
			Assert.AreEqual(Accessibility.Public, p.Setter.Accessibility);
			Assert.AreEqual(false, p.Setter.HasBody);

			type = GetTypeDefinition(typeof(IInterfaceWithIndexers));
			p = type.Properties.Single(x => x.Parameters.Count == 2);
			Assert.AreEqual(true, p.IsIndexer);
			Assert.AreEqual(true, p.IsAbstract);
			Assert.AreEqual(true, p.IsOverridable);
			Assert.AreEqual(false, p.IsOverride);
			Assert.AreEqual(Accessibility.Public, p.Accessibility);
			Assert.AreEqual(true, p.Getter.IsAbstract);
			Assert.AreEqual(true, p.Getter.IsOverridable);
			Assert.AreEqual(false, p.Getter.IsOverride);
			Assert.AreEqual(Accessibility.Public, p.Getter.Accessibility);
			Assert.AreEqual(true, p.Setter.IsAbstract);
			Assert.AreEqual(true, p.Setter.IsOverridable);
			Assert.AreEqual(false, p.Setter.IsOverride);
			Assert.AreEqual(Accessibility.Public, p.Setter.Accessibility);

			type = GetTypeDefinition(typeof(IHasEvent));
			var e = type.Events.Single();
			Assert.AreEqual(true, e.IsAbstract);
			Assert.AreEqual(true, e.IsOverridable);
			Assert.AreEqual(false, e.IsOverride);
			Assert.AreEqual(Accessibility.Public, e.Accessibility);
			Assert.AreEqual(true, e.AddAccessor.IsAbstract);
			Assert.AreEqual(true, e.AddAccessor.IsOverridable);
			Assert.AreEqual(false, e.AddAccessor.IsOverride);
			Assert.AreEqual(Accessibility.Public, e.AddAccessor.Accessibility);
			Assert.AreEqual(true, e.RemoveAccessor.IsAbstract);
			Assert.AreEqual(true, e.RemoveAccessor.IsOverridable);
			Assert.AreEqual(false, e.RemoveAccessor.IsOverride);
			Assert.AreEqual(Accessibility.Public, e.RemoveAccessor.Accessibility);

			type = GetTypeDefinition(typeof(IDisposable));
			var m = type.Methods.Single();
			Assert.AreEqual(true, m.IsAbstract);
			Assert.AreEqual(true, m.IsOverridable);
			Assert.AreEqual(false, m.IsOverride);
			Assert.AreEqual(Accessibility.Public, m.Accessibility);
		}

		[Test]
		public void InnerClassInGenericClass_TypeParameterOwner()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(OuterGeneric<>.Inner));
			Assert.AreSame(type.DeclaringTypeDefinition.TypeParameters[0], type.TypeParameters[0]);
			Assert.AreSame(type.DeclaringTypeDefinition, type.TypeParameters[0].Owner);
		}

		[Test]
		public void InnerClassInGenericClass_ReferencesTheOuterClass_Field()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(OuterGeneric<>.Inner));
			IField f = type.Fields.Single();
			Assert.AreEqual("ICSharpCode.Decompiler.Tests.TypeSystem.OuterGeneric`1[[`0]]", f.Type.ReflectionName);
		}

		[Test]
		public void InnerClassInGenericClass_ReferencesTheOuterClass_Parameter()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(OuterGeneric<>.Inner));
			IParameter p = type.Methods.Single(m => m.IsConstructor).Parameters.Single();
			Assert.AreEqual("ICSharpCode.Decompiler.Tests.TypeSystem.OuterGeneric`1[[`0]]", p.Type.ReflectionName);
		}

		CustomAttributeTypedArgument<IType> GetParamsAttributeArgument(int index)
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ParamsAttribute));
			var arr = (AttributeArray)type.GetAttributes().Single().FixedArguments.Single().Value;
			Assert.AreEqual(5, arr.Length);
			return arr[index];
		}

		[Test]
		public void ParamsAttribute_Integer()
		{
			var arg = GetParamsAttributeArgument(0);
			Assert.AreEqual("System.Int32", arg.Type.FullName);
			Assert.AreEqual(1, arg.Value);
		}

		[Test]
		public void ParamsAttribute_Enum()
		{
			var arg = GetParamsAttributeArgument(1);
			Assert.AreEqual("System.StringComparison", arg.Type.FullName);
			Assert.AreEqual((int)StringComparison.CurrentCulture, arg.Value);
		}

		[Test]
		public void ParamsAttribute_NullReference()
		{
			var arg = GetParamsAttributeArgument(2);
			//Assert.AreEqual("System.Object", arg.Type.FullName);
			Assert.IsNull(arg.Value);
		}

		[Test]
		public void ParamsAttribute_Double()
		{
			var arg = GetParamsAttributeArgument(3);
			Assert.AreEqual("System.Double", arg.Type.FullName);
			Assert.AreEqual(4.0, arg.Value);
		}

		[Test]
		public void ParamsAttribute_String()
		{
			var arg = GetParamsAttributeArgument(4);
			Assert.AreEqual("System.String", arg.Type.FullName);
			Assert.AreEqual("Test", arg.Value);
		}

		[Test]
		public void ParamsAttribute_Property()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ParamsAttribute));
			IProperty prop = type.Properties.Single(p => p.Name == "Property");
			var attr = prop.GetAttributes().Single();
			Assert.AreEqual(type, attr.AttributeType);

			var elements = (AttributeArray)attr.FixedArguments.Single().Value;
			Assert.AreEqual(0, elements.Length);

			var namedArg = attr.NamedArguments.Single();
			Assert.AreEqual(prop.Name, namedArg.Name);
			var arrayElements = (AttributeArray)namedArg.Value;
			Assert.AreEqual(2, arrayElements.Length);
		}

		[Test]
		public void ParamsAttribute_Getter_ReturnType()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ParamsAttribute));
			IProperty prop = type.Properties.Single(p => p.Name == "Property");
			Assert.AreEqual(0, prop.Getter.GetAttributes().Count());
			Assert.AreEqual(1, prop.Getter.GetReturnTypeAttributes().Count());
		}

		[Test]
		public void DoubleAttribute_ImplicitNumericConversion()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(DoubleAttribute));
			var arg = type.GetAttributes().Single().FixedArguments.Single();
			Assert.AreEqual("System.Double", arg.Type.ReflectionName);
			Assert.AreEqual(1.0, arg.Value);
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
			Assert.IsTrue(evt1.IsStatic);
			Assert.IsTrue(evt1.AddAccessor.IsStatic);
			Assert.IsTrue(evt1.RemoveAccessor.IsStatic);

			var evt2 = type.Events.Single(e => e.Name == "Event2");
			Assert.IsFalse(evt2.IsStatic);
			Assert.IsFalse(evt2.AddAccessor.IsStatic);
			Assert.IsFalse(evt2.RemoveAccessor.IsStatic);

			var evt3 = type.Events.Single(e => e.Name == "Event3");
			Assert.IsTrue(evt3.IsStatic);
			Assert.IsTrue(evt3.AddAccessor.IsStatic);
			Assert.IsTrue(evt3.RemoveAccessor.IsStatic);

			var evt4 = type.Events.Single(e => e.Name == "Event4");
			Assert.IsFalse(evt4.IsStatic);
			Assert.IsFalse(evt4.AddAccessor.IsStatic);
			Assert.IsFalse(evt4.RemoveAccessor.IsStatic);
		}

		[Test]
		public void StaticityOfPropertyAccessors()
		{
			// https://github.com/icsharpcode/NRefactory/issues/20
			ITypeDefinition type = GetTypeDefinition(typeof(ClassWithStaticAndNonStaticMembers));
			var prop1 = type.Properties.Single(e => e.Name == "Prop1");
			Assert.IsTrue(prop1.IsStatic);
			Assert.IsTrue(prop1.Getter.IsStatic);
			Assert.IsTrue(prop1.Setter.IsStatic);

			var prop2 = type.Properties.Single(e => e.Name == "Prop2");
			Assert.IsFalse(prop2.IsStatic);
			Assert.IsFalse(prop2.Getter.IsStatic);
			Assert.IsFalse(prop2.Setter.IsStatic);

			var prop3 = type.Properties.Single(e => e.Name == "Prop3");
			Assert.IsTrue(prop3.IsStatic);
			Assert.IsTrue(prop3.Getter.IsStatic);
			Assert.IsTrue(prop3.Setter.IsStatic);

			var prop4 = type.Properties.Single(e => e.Name == "Prop4");
			Assert.IsFalse(prop4.IsStatic);
			Assert.IsFalse(prop4.Getter.IsStatic);
			Assert.IsFalse(prop4.Setter.IsStatic);
		}

		[Test]
		public void PropertyAccessorsHaveBody()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassWithStaticAndNonStaticMembers));
			foreach (var prop in type.Properties)
			{
				Assert.IsTrue(prop.Getter.HasBody, prop.Getter.Name);
				Assert.IsTrue(prop.Setter.HasBody, prop.Setter.Name);
			}
		}

		[Test]
		public void EventAccessorNames()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassWithStaticAndNonStaticMembers));
			var customEvent = type.Events.Single(e => e.Name == "Event1");
			Assert.AreEqual("add_Event1", customEvent.AddAccessor.Name);
			Assert.AreEqual("remove_Event1", customEvent.RemoveAccessor.Name);

			var normalEvent = type.Events.Single(e => e.Name == "Event3");
			Assert.AreEqual("add_Event3", normalEvent.AddAccessor.Name);
			Assert.AreEqual("remove_Event3", normalEvent.RemoveAccessor.Name);
		}

		[Test]
		public void EventAccessorHaveBody()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassWithStaticAndNonStaticMembers));
			foreach (var ev in type.Events)
			{
				Assert.IsTrue(ev.AddAccessor.HasBody, ev.AddAccessor.Name);
				Assert.IsTrue(ev.RemoveAccessor.HasBody, ev.RemoveAccessor.Name);
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
			Assert.AreEqual(Accessibility.Public, prop.Accessibility);
			Assert.AreEqual(Accessibility.Public, prop.Getter.Accessibility);
		}

		[Test]
		public void ClassThatOverridesSetterOnly()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatOverridesSetterOnly));
			var prop = type.Properties.Single(p => p.Name == "Prop");
			Assert.AreEqual(Accessibility.Public, prop.Accessibility);
			Assert.AreEqual(Accessibility.Protected, prop.Setter.Accessibility);
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
			Assert.IsFalse(prop.IsVirtual);
			Assert.IsFalse(prop.IsOverridable);
			Assert.IsFalse(prop.IsSealed);
		}

		[Test]
		public void Property_SealedOverride()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassThatOverridesAndSealsVirtualProperty));
			var prop = type.Properties.Single(p => p.Name == "Prop");
			Assert.IsFalse(prop.IsVirtual);
			Assert.IsTrue(prop.IsOverride);
			Assert.IsTrue(prop.IsSealed);
			Assert.IsFalse(prop.IsOverridable);
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
			Assert.IsTrue(method.IsExplicitInterfaceImplementation);
			Assert.AreEqual("System.IDisposable.Dispose", method.ExplicitlyImplementedInterfaceMembers.Single().FullName);
		}

		[Test]
		public void ExplicitImplementationOfUnifiedMethods()
		{
			IType type = compilation.FindType(typeof(ExplicitGenericInterfaceImplementationWithUnifiableMethods<int, int>));
			Assert.AreEqual(2, type.GetMethods(m => m.IsExplicitInterfaceImplementation).Count());
			foreach (IMethod method in type.GetMethods(m => m.IsExplicitInterfaceImplementation))
			{
				Assert.AreEqual(1, method.ExplicitlyImplementedInterfaceMembers.Count(), method.ToString());
				Assert.AreEqual("System.Int32", method.Parameters.Single().Type.ReflectionName);
				IMethod interfaceMethod = (IMethod)method.ExplicitlyImplementedInterfaceMembers.Single();
				Assert.AreEqual("System.Int32", interfaceMethod.Parameters.Single().Type.ReflectionName);
				var genericParamType = ((IMethod)method.MemberDefinition).Parameters.Single().Type;
				var interfaceGenericParamType = ((IMethod)interfaceMethod.MemberDefinition).Parameters.Single().Type;
				Assert.AreEqual(TypeKind.TypeParameter, genericParamType.Kind);
				Assert.AreEqual(TypeKind.TypeParameter, interfaceGenericParamType.Kind);
				Assert.AreEqual(genericParamType.ReflectionName, interfaceGenericParamType.ReflectionName);
			}
		}

		[Test]
		public void ExplicitGenericInterfaceImplementation()
		{
			ITypeDefinition impl = GetTypeDefinition(typeof(ExplicitGenericInterfaceImplementation));
			IType genericInterfaceOfString = compilation.FindType(typeof(IGenericInterface<string>));
			IMethod implMethod1 = impl.Methods.Single(m => !m.IsConstructor && !m.Parameters[1].IsRef);
			IMethod implMethod2 = impl.Methods.Single(m => !m.IsConstructor && m.Parameters[1].IsRef);
			Assert.IsTrue(implMethod1.IsExplicitInterfaceImplementation);
			Assert.IsTrue(implMethod2.IsExplicitInterfaceImplementation);

			IMethod interfaceMethod1 = (IMethod)implMethod1.ExplicitlyImplementedInterfaceMembers.Single();
			Assert.AreEqual(genericInterfaceOfString, interfaceMethod1.DeclaringType);
			Assert.IsTrue(!interfaceMethod1.Parameters[1].IsRef);

			IMethod interfaceMethod2 = (IMethod)implMethod2.ExplicitlyImplementedInterfaceMembers.Single();
			Assert.AreEqual(genericInterfaceOfString, interfaceMethod2.DeclaringType);
			Assert.IsTrue(interfaceMethod2.Parameters[1].IsRef);
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
			Assert.IsTrue(prop.IsExplicitInterfaceImplementation);
			Assert.IsTrue(prop.Getter.IsExplicitInterfaceImplementation);
			Assert.IsTrue(prop.Setter.IsExplicitInterfaceImplementation);
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
			Assert.IsTrue(type.IsAbstract);
			Assert.IsTrue(type.IsSealed);
			Assert.IsTrue(type.IsStatic);
		}

		[Test]
		public void ExtensionMethodTest()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(StaticClass));
			var method = type.Methods.Single(m => m.Name == "Extension");

			Assert.IsTrue(method.IsStatic);
			Assert.IsTrue(method.IsExtensionMethod);
			Assert.IsNull(method.ReducedFrom);

			Assert.IsTrue(type.HasExtensionMethods);
		}

		[Test]
		public void NoDefaultConstructorOnStaticClassTest()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(StaticClass));
			Assert.AreEqual(0, type.GetConstructors().Count());
		}

		[Test]
		public void IndexerNonDefaultName()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(IndexerNonDefaultName));
			var indexer = type.GetProperties(p => p.IsIndexer).Single();
			Assert.AreEqual("Foo", indexer.Name);
		}

		[Test]
		public void TestNullableDefaultParameter()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ClassWithMethodThatHasNullableDefaultParameter));
			var method = type.GetMethods().Single(m => m.Name == "Foo");
			Assert.AreEqual(42, method.Parameters.Single().GetConstantValue());
		}

		[Test]
		public void AccessibilityTests()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(AccessibilityTest));
			Assert.AreEqual(Accessibility.Public, type.Methods.Single(m => m.Name == "Public").Accessibility);
			Assert.AreEqual(Accessibility.Internal, type.Methods.Single(m => m.Name == "Internal").Accessibility);
			Assert.AreEqual(Accessibility.ProtectedOrInternal, type.Methods.Single(m => m.Name == "ProtectedInternal").Accessibility);
			Assert.AreEqual(Accessibility.ProtectedOrInternal, type.Methods.Single(m => m.Name == "InternalProtected").Accessibility);
			Assert.AreEqual(Accessibility.Protected, type.Methods.Single(m => m.Name == "Protected").Accessibility);
			Assert.AreEqual(Accessibility.Private, type.Methods.Single(m => m.Name == "Private").Accessibility);
			Assert.AreEqual(Accessibility.Private, type.Methods.Single(m => m.Name == "None").Accessibility);
		}

		private void AssertConstantField<T>(ITypeDefinition type, string name, T expected)
		{
			var f = type.GetFields().Single(x => x.Name == name);
			Assert.IsTrue(f.IsConst);
			Assert.AreEqual(expected, f.GetConstantValue());
			Assert.AreEqual(0, f.GetAttributes().Count());
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
			Assert.IsTrue(field.IsConst);
			Assert.AreEqual((short)MyEnum.Second, field.GetConstantValue());
		}

		[Test]
		public void ConstantEnumFromAnotherAssembly()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ConstantFieldTest));
			IField field = type.Fields.Single(f => f.Name == "EnumFromAnotherAssembly");
			Assert.IsTrue(field.IsConst);
			Assert.AreEqual((int)StringComparison.OrdinalIgnoreCase, field.GetConstantValue());
		}

		[Test]
		public void DefaultOfEnum()
		{
			ITypeDefinition type = GetTypeDefinition(typeof(ConstantFieldTest));
			IField field = type.Fields.Single(f => f.Name == "DefaultOfEnum");
			Assert.IsTrue(field.IsConst);
			Assert.AreEqual((short)default(MyEnum), field.GetConstantValue());
		}

		[Test]
		public void ExplicitImplementation()
		{
			var type = GetTypeDefinition(typeof(ExplicitImplementationTests));
			var itype = GetTypeDefinition(typeof(IExplicitImplementationTests));

			var methods = type.GetMethods(m => m.Name == "M" || m.Name.EndsWith(".M")).ToList();
			var imethod = itype.GetMethods(m => m.Name == "M").Single();
			Assert.That(methods.Select(m => m.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.AreEqual(methods.SelectMany(m => m.ExplicitlyImplementedInterfaceMembers).Single(), imethod);

			var properties = type.GetProperties(p => p.Name == "P" || p.Name.EndsWith(".P")).ToList();
			var iproperty = itype.GetProperties(m => m.Name == "P").Single();
			Assert.That(properties.Select(p => p.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.AreEqual(properties.SelectMany(p => p.ExplicitlyImplementedInterfaceMembers).Single(), iproperty);
			Assert.That(properties.Select(p => p.Getter.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.AreEqual(properties.SelectMany(p => p.Getter.ExplicitlyImplementedInterfaceMembers).Single(), iproperty.Getter);
			Assert.That(properties.Select(p => p.Setter.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.AreEqual(properties.SelectMany(p => p.Setter.ExplicitlyImplementedInterfaceMembers).Single(), iproperty.Setter);

			var indexers = type.GetProperties(p => p.Name == "Item" || p.Name.EndsWith(".Item")).ToList();
			var iindexer = itype.GetProperties(m => m.Name == "Item").Single();
			Assert.That(indexers.Select(p => p.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.AreEqual(indexers.SelectMany(p => p.ExplicitlyImplementedInterfaceMembers).Single(), iindexer);
			Assert.That(indexers.Select(p => p.Getter.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.AreEqual(indexers.SelectMany(p => p.Getter.ExplicitlyImplementedInterfaceMembers).Single(), iindexer.Getter);
			Assert.That(indexers.Select(p => p.Setter.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.AreEqual(indexers.SelectMany(p => p.Setter.ExplicitlyImplementedInterfaceMembers).Single(), iindexer.Setter);

			var events = type.GetEvents(e => e.Name == "E" || e.Name.EndsWith(".E")).ToList();
			var ievent = itype.GetEvents(m => m.Name == "E").Single();
			Assert.That(events.Select(e => e.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.AreEqual(events.SelectMany(e => e.ExplicitlyImplementedInterfaceMembers).Single(), ievent);
			Assert.That(events.Select(e => e.AddAccessor.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.AreEqual(events.SelectMany(e => e.AddAccessor.ExplicitlyImplementedInterfaceMembers).Single(), ievent.AddAccessor);
			Assert.That(events.Select(e => e.RemoveAccessor.ExplicitlyImplementedInterfaceMembers.Count()).ToList(), Is.EquivalentTo(new[] { 0, 1 }));
			Assert.AreEqual(events.SelectMany(e => e.RemoveAccessor.ExplicitlyImplementedInterfaceMembers).Single(), ievent.RemoveAccessor);
		}

		[Test]
		public void MarshalTests()
		{
			ITypeDefinition c = compilation.FindType(typeof(IMarshalAsTests)).GetDefinition();
			Assert.AreEqual(1, c.GetMethods(m => m.Name == "GetCollectionByQuery2").Count());
		}


		[Test]
		public void AttributesUsingNestedMembers()
		{
			var type = GetTypeDefinition(typeof(ClassWithAttributesUsingNestedMembers));
			var inner = type.GetNestedTypes().Single(t => t.Name == "Inner");
			var myAttribute = type.GetNestedTypes().Single(t => t.Name == "MyAttribute");
			var typeTypeTestAttr = type.GetAttributes().Single(a => a.AttributeType.Name == "TypeTestAttribute");
			Assert.AreEqual(42, typeTypeTestAttr.FixedArguments[0].Value);
			Assert.AreEqual(inner, typeTypeTestAttr.FixedArguments[1].Value);
			var typeMyAttr = type.GetAttributes().Single(a => a.AttributeType.Name == "MyAttribute");
			Assert.AreEqual(myAttribute, typeMyAttr.AttributeType);

			var prop = type.GetProperties().Single(p => p.Name == "P");
			var propTypeTestAttr = prop.GetAttributes().Single(a => a.AttributeType.Name == "TypeTestAttribute");
			Assert.AreEqual(42, propTypeTestAttr.FixedArguments[0].Value);
			Assert.AreEqual(inner, propTypeTestAttr.FixedArguments[1].Value);
			var propMyAttr = prop.GetAttributes().Single(a => a.AttributeType.Name == "MyAttribute");
			Assert.AreEqual(myAttribute, propMyAttr.AttributeType);

			var attributedInner = (ITypeDefinition)type.GetNestedTypes().Single(t => t.Name == "AttributedInner");
			var innerTypeTestAttr = attributedInner.GetAttributes().Single(a => a.AttributeType.Name == "TypeTestAttribute");
			Assert.AreEqual(42, innerTypeTestAttr.FixedArguments[0].Value);
			Assert.AreEqual(inner, innerTypeTestAttr.FixedArguments[1].Value);
			var innerMyAttr = attributedInner.GetAttributes().Single(a => a.AttributeType.Name == "MyAttribute");
			Assert.AreEqual(myAttribute, innerMyAttr.AttributeType);

			var attributedInner2 = (ITypeDefinition)type.GetNestedTypes().Single(t => t.Name == "AttributedInner2");
			var inner2 = attributedInner2.GetNestedTypes().Single(t => t.Name == "Inner");
			var myAttribute2 = attributedInner2.GetNestedTypes().Single(t => t.Name == "MyAttribute");
			var inner2TypeTestAttr = attributedInner2.GetAttributes().Single(a => a.AttributeType.Name == "TypeTestAttribute");
			Assert.AreEqual(43, inner2TypeTestAttr.FixedArguments[0].Value);
			Assert.AreEqual(inner2, inner2TypeTestAttr.FixedArguments[1].Value);
			var inner2MyAttr = attributedInner2.GetAttributes().Single(a => a.AttributeType.Name == "MyAttribute");
			Assert.AreEqual(myAttribute2, inner2MyAttr.AttributeType);
		}

		[Test]
		public void ClassWithAttributeOnTypeParameter()
		{
			var tp = GetTypeDefinition(typeof(ClassWithAttributeOnTypeParameter<>)).TypeParameters.Single();
			var attr = tp.GetAttributes().Single();
			Assert.AreEqual("DoubleAttribute", attr.AttributeType.Name);
		}

		[Test]
		public void InheritanceTest()
		{
			ITypeDefinition c = compilation.FindType(typeof(SystemException)).GetDefinition();
			ITypeDefinition c2 = compilation.FindType(typeof(Exception)).GetDefinition();
			Assert.IsNotNull(c, "c is null");
			Assert.IsNotNull(c2, "c2 is null");
			//Assert.AreEqual(3, c.BaseTypes.Count); // Inherited interfaces are not reported by Cecil
			// which matches the behaviour of our C#/VB parsers
			Assert.AreEqual("System.Exception", c.DirectBaseTypes.First().FullName);
			Assert.AreSame(c2, c.DirectBaseTypes.First());

			string[] superTypes = c.GetAllBaseTypes().Select(t => t.ReflectionName).ToArray();
			Assert.AreEqual(new string[] {
								"System.Object",
								"System.Runtime.Serialization.ISerializable", "System.Runtime.InteropServices._Exception",
								"System.Exception", "System.SystemException"
							}, superTypes);
		}

		[Test]
		public void GenericPropertyTest()
		{
			ITypeDefinition c = compilation.FindType(typeof(Comparer<>)).GetDefinition();
			IProperty def = c.Properties.Single(p => p.Name == "Default");
			ParameterizedType pt = (ParameterizedType)def.ReturnType;
			Assert.AreEqual("System.Collections.Generic.Comparer", pt.FullName);
			Assert.AreEqual(c.TypeParameters[0], pt.TypeArguments[0]);
		}

		[Test]
		public void PointerTypeTest()
		{
			ITypeDefinition c = compilation.FindType(typeof(IntPtr)).GetDefinition();
			IMethod toPointer = c.Methods.Single(p => p.Name == "ToPointer");
			Assert.AreEqual("System.Void*", toPointer.ReturnType.ReflectionName);
			Assert.IsTrue(toPointer.ReturnType is PointerType);
			Assert.AreEqual("System.Void", ((PointerType)toPointer.ReturnType).ElementType.FullName);
		}

		[Test]
		public void DateTimeDefaultConstructor()
		{
			ITypeDefinition c = compilation.FindType(typeof(DateTime)).GetDefinition();
			Assert.AreEqual(1, c.Methods.Count(m => m.IsConstructor && !m.IsStatic && m.Parameters.Count == 0));
			Assert.AreEqual(1, c.GetConstructors().Count(m => m.Parameters.Count == 0));
		}

		[Test]
		public void NoEncodingInfoDefaultConstructor()
		{
			ITypeDefinition c = compilation.FindType(typeof(EncodingInfo)).GetDefinition();
			// EncodingInfo only has an internal constructor
			Assert.IsFalse(c.Methods.Any(m => m.IsConstructor));
			// and no implicit ctor should be added:
			Assert.AreEqual(0, c.GetConstructors().Count());
		}

		[Test]
		public void StaticModifierTest()
		{
			ITypeDefinition c = compilation.FindType(typeof(Environment)).GetDefinition();
			Assert.IsNotNull(c, "System.Environment not found");
			Assert.IsTrue(c.IsAbstract, "class should be abstract");
			Assert.IsTrue(c.IsSealed, "class should be sealed");
			Assert.IsTrue(c.IsStatic, "class should be static");
		}

		[Test]
		public void InnerClassReferenceTest()
		{
			ITypeDefinition c = compilation.FindType(typeof(Environment)).GetDefinition();
			Assert.IsNotNull(c, "System.Environment not found");
			IType rt = c.Methods.First(m => m.Name == "GetFolderPath").Parameters[0].Type;
			Assert.AreSame(c.NestedTypes.Single(ic => ic.Name == "SpecialFolder"), rt);
		}

		[Test]
		public void NestedTypesTest()
		{
			ITypeDefinition c = compilation.FindType(typeof(Environment.SpecialFolder)).GetDefinition();
			Assert.IsNotNull(c, "c is null");
			Assert.AreEqual("System.Environment.SpecialFolder", c.FullName);
			Assert.AreEqual("System.Environment+SpecialFolder", c.ReflectionName);
		}

		[Test]
		public void VoidHasNoMembers()
		{
			ITypeDefinition c = compilation.FindType(typeof(void)).GetDefinition();
			Assert.IsNotNull(c, "System.Void not found");
			Assert.AreEqual(TypeKind.Void, c.Kind);
			Assert.AreEqual(0, c.GetMethods().Count());
			Assert.AreEqual(0, c.GetProperties().Count());
			Assert.AreEqual(0, c.GetEvents().Count());
			Assert.AreEqual(0, c.GetFields().Count());
		}

		[Test]
		public void Void_SerializableAttribute()
		{
			ITypeDefinition c = compilation.FindType(typeof(void)).GetDefinition();
			var attr = c.GetAttributes().Single(a => a.AttributeType.FullName == "System.SerializableAttribute");
			Assert.AreEqual(0, attr.Constructor.Parameters.Count);
			Assert.AreEqual(0, attr.FixedArguments.Length);
			Assert.AreEqual(0, attr.NamedArguments.Length);
		}

		[Test]
		public void Void_StructLayoutAttribute()
		{
			ITypeDefinition c = compilation.FindType(typeof(void)).GetDefinition();
			var attr = c.GetAttributes().Single(a => a.AttributeType.FullName == "System.Runtime.InteropServices.StructLayoutAttribute");
			Assert.AreEqual(1, attr.Constructor.Parameters.Count);
			Assert.AreEqual(1, attr.FixedArguments.Length);
			Assert.AreEqual(0, attr.FixedArguments[0].Value);
			Assert.AreEqual(1, attr.NamedArguments.Length);
			Assert.AreEqual("Size", attr.NamedArguments[0].Name);
			Assert.AreEqual(1, attr.NamedArguments[0].Value);
		}

		[Test]
		public void Void_ComVisibleAttribute()
		{
			ITypeDefinition c = compilation.FindType(typeof(void)).GetDefinition();
			var attr = c.GetAttributes().Single(a => a.AttributeType.FullName == "System.Runtime.InteropServices.ComVisibleAttribute");
			Assert.AreEqual(1, attr.Constructor.Parameters.Count);
			Assert.AreEqual(1, attr.FixedArguments.Length);
			Assert.AreEqual(true, attr.FixedArguments[0].Value);
			Assert.AreEqual(0, attr.NamedArguments.Length);
		}

		[Test]
		public void NestedClassInGenericClassTest()
		{
			ITypeDefinition dictionary = compilation.FindType(typeof(Dictionary<,>)).GetDefinition();
			Assert.IsNotNull(dictionary);
			ITypeDefinition valueCollection = compilation.FindType(typeof(Dictionary<,>.ValueCollection)).GetDefinition();
			Assert.IsNotNull(valueCollection);
			var dictionaryRT = new ParameterizedType(dictionary, new[] { compilation.FindType(typeof(string)).GetDefinition(), compilation.FindType(typeof(int)).GetDefinition() });
			IProperty valueProperty = dictionaryRT.GetProperties(p => p.Name == "Values").Single();
			IType parameterizedValueCollection = valueProperty.ReturnType;
			Assert.AreEqual("System.Collections.Generic.Dictionary`2+ValueCollection[[System.String],[System.Int32]]", parameterizedValueCollection.ReflectionName);
			Assert.AreSame(valueCollection, parameterizedValueCollection.GetDefinition());
		}

		[Test]
		public void ValueCollectionCountModifiers()
		{
			ITypeDefinition valueCollection = compilation.FindType(typeof(Dictionary<,>.ValueCollection)).GetDefinition();
			Assert.AreEqual(Accessibility.Public, valueCollection.Accessibility);
			Assert.IsTrue(valueCollection.IsSealed);
			Assert.IsFalse(valueCollection.IsAbstract);
			Assert.IsFalse(valueCollection.IsStatic);

			IProperty count = valueCollection.Properties.Single(p => p.Name == "Count");
			Assert.AreEqual(Accessibility.Public, count.Accessibility);
			// It's sealed on the IL level; but in C# it's just a normal non-virtual method that happens to implement an interface
			Assert.IsFalse(count.IsSealed);
			Assert.IsFalse(count.IsVirtual);
			Assert.IsFalse(count.IsAbstract);
		}

		[Test]
		public void MathAcosModifiers()
		{
			ITypeDefinition math = compilation.FindType(typeof(Math)).GetDefinition();
			Assert.AreEqual(Accessibility.Public, math.Accessibility);
			Assert.IsTrue(math.IsSealed);
			Assert.IsTrue(math.IsAbstract);
			Assert.IsTrue(math.IsStatic);

			IMethod acos = math.Methods.Single(p => p.Name == "Acos");
			Assert.AreEqual(Accessibility.Public, acos.Accessibility);
			Assert.IsTrue(acos.IsStatic);
			Assert.IsFalse(acos.IsAbstract);
			Assert.IsFalse(acos.IsSealed);
			Assert.IsFalse(acos.IsVirtual);
			Assert.IsFalse(acos.IsOverride);
		}

		[Test]
		public void EncodingModifiers()
		{
			ITypeDefinition encoding = compilation.FindType(typeof(Encoding)).GetDefinition();
			Assert.AreEqual(Accessibility.Public, encoding.Accessibility);
			Assert.IsFalse(encoding.IsSealed);
			Assert.IsTrue(encoding.IsAbstract);

			IMethod getDecoder = encoding.Methods.Single(p => p.Name == "GetDecoder");
			Assert.AreEqual(Accessibility.Public, getDecoder.Accessibility);
			Assert.IsFalse(getDecoder.IsStatic);
			Assert.IsFalse(getDecoder.IsAbstract);
			Assert.IsFalse(getDecoder.IsSealed);
			Assert.IsTrue(getDecoder.IsVirtual);
			Assert.IsFalse(getDecoder.IsOverride);

			IMethod getMaxByteCount = encoding.Methods.Single(p => p.Name == "GetMaxByteCount");
			Assert.AreEqual(Accessibility.Public, getMaxByteCount.Accessibility);
			Assert.IsFalse(getMaxByteCount.IsStatic);
			Assert.IsTrue(getMaxByteCount.IsAbstract);
			Assert.IsFalse(getMaxByteCount.IsSealed);
			Assert.IsFalse(getMaxByteCount.IsVirtual);
			Assert.IsFalse(getMaxByteCount.IsOverride);

			IProperty encoderFallback = encoding.Properties.Single(p => p.Name == "EncoderFallback");
			Assert.AreEqual(Accessibility.Public, encoderFallback.Accessibility);
			Assert.IsFalse(encoderFallback.IsStatic);
			Assert.IsFalse(encoderFallback.IsAbstract);
			Assert.IsFalse(encoderFallback.IsSealed);
			Assert.IsFalse(encoderFallback.IsVirtual);
			Assert.IsFalse(encoderFallback.IsOverride);
		}

		[Test]
		public void UnicodeEncodingModifiers()
		{
			ITypeDefinition encoding = compilation.FindType(typeof(UnicodeEncoding)).GetDefinition();
			Assert.AreEqual(Accessibility.Public, encoding.Accessibility);
			Assert.IsFalse(encoding.IsSealed);
			Assert.IsFalse(encoding.IsAbstract);

			IMethod getDecoder = encoding.Methods.Single(p => p.Name == "GetDecoder");
			Assert.AreEqual(Accessibility.Public, getDecoder.Accessibility);
			Assert.IsFalse(getDecoder.IsStatic);
			Assert.IsFalse(getDecoder.IsAbstract);
			Assert.IsFalse(getDecoder.IsSealed);
			Assert.IsFalse(getDecoder.IsVirtual);
			Assert.IsTrue(getDecoder.IsOverride);
		}

		[Test]
		public void UTF32EncodingModifiers()
		{
			ITypeDefinition encoding = compilation.FindType(typeof(UTF32Encoding)).GetDefinition();
			Assert.AreEqual(Accessibility.Public, encoding.Accessibility);
			Assert.IsTrue(encoding.IsSealed);
			Assert.IsFalse(encoding.IsAbstract);

			IMethod getDecoder = encoding.Methods.Single(p => p.Name == "GetDecoder");
			Assert.AreEqual(Accessibility.Public, getDecoder.Accessibility);
			Assert.IsFalse(getDecoder.IsStatic);
			Assert.IsFalse(getDecoder.IsAbstract);
			Assert.IsFalse(getDecoder.IsSealed);
			Assert.IsFalse(getDecoder.IsVirtual);
			Assert.IsTrue(getDecoder.IsOverride);
		}

		[Test]
		public void FindRedirectedType()
		{
			var compilationWithSystemCore = new SimpleCompilation(SystemCore, Mscorlib);

			var typeRef = ReflectionHelper.ParseReflectionName("System.Func`2, System.Core");
			ITypeDefinition c = typeRef.Resolve(new SimpleTypeResolveContext(compilationWithSystemCore)).GetDefinition();
			Assert.IsNotNull(c, "System.Func<,> not found");
			Assert.AreEqual("mscorlib", c.ParentModule.AssemblyName);
		}

		public void DelegateIsClass()
		{
			var @delegate = compilation.FindType(KnownTypeCode.Delegate).GetDefinition();
			Assert.AreEqual(TypeKind.Class, @delegate);
			Assert.IsFalse(@delegate.IsSealed);
		}

		public void MulticastDelegateIsClass()
		{
			var multicastDelegate = compilation.FindType(KnownTypeCode.MulticastDelegate).GetDefinition();
			Assert.AreEqual(TypeKind.Class, multicastDelegate);
			Assert.IsFalse(multicastDelegate.IsSealed);
		}
	}
}
