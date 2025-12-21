// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;

using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.Tests.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

using static ICSharpCode.Decompiler.Output.ConversionFlags;

namespace ICSharpCode.Decompiler.Tests.Output
{
	[TestFixture]
	public class ILAmbienceTests
	{
		ICompilation compilation;
		ILAmbience ambience;

		[OneTimeSetUp]
		public void FixtureSetUp()
		{
			ambience = new ILAmbience();

			compilation = new SimpleCompilation(TypeSystemLoaderTests.TestAssembly,
				TypeSystemLoaderTests.Mscorlib.WithOptions(TypeSystemOptions.Default));
		}

		ITypeDefinition GetDefinition(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}

			var foundType = compilation.FindType(type).GetDefinition();
			Assert.That(foundType, Is.Not.Null);
			return foundType;
		}

		const ConversionFlags ILSpyMainTreeViewTypeFlags = ShowTypeParameterList | PlaceReturnTypeAfterParameterList;
		const ConversionFlags ILSpyMainTreeViewMemberFlags = ILSpyMainTreeViewTypeFlags | ShowParameterList | ShowReturnType | ShowParameterModifiers;

		#region ITypeDefinition tests
		[TestCase(None, "Dictionary`2")]
		[TestCase(ShowDefinitionKeyword, ".class Dictionary`2")]
		[TestCase(ShowAccessibility, "public Dictionary`2")]
		[TestCase(ShowDefinitionKeyword | ShowAccessibility, ".class public Dictionary`2")]
		[TestCase(ShowTypeParameterList, "Dictionary`2<TKey,TValue>")]
		[TestCase(ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, ".class public Dictionary`2<TKey,TValue>")]
		[TestCase(UseFullyQualifiedEntityNames | ShowTypeParameterList, "System.Collections.Generic.Dictionary`2<TKey,TValue>")]
		[TestCase(UseFullyQualifiedEntityNames | ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, ".class public System.Collections.Generic.Dictionary`2<TKey,TValue>")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "Dictionary`2<TKey,TValue>")]
		public void GenericType(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(Dictionary<,>));
			ambience.ConversionFlags = flags;
			Assert.That(ambience.ConvertSymbol(typeDef), Is.EqualTo(expectedOutput));
		}

		[TestCase(None, "Object")]
		[TestCase(ShowDefinitionKeyword, ".class Object")]
		[TestCase(ShowAccessibility, "public Object")]
		[TestCase(ShowDefinitionKeyword | ShowAccessibility, ".class public Object")]
		[TestCase(ShowTypeParameterList, "Object")]
		[TestCase(ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, ".class public Object")]
		[TestCase(UseFullyQualifiedEntityNames | ShowTypeParameterList, "System.Object")]
		[TestCase(All, ".class public serializable beforefieldinit System.Object")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "Object")]
		public void SimpleType(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(object));
			ambience.ConversionFlags = flags;
			Assert.That(ambience.ConvertSymbol(typeDef), Is.EqualTo(expectedOutput));
		}

		[TestCase(None, "IEnumerable`1")]
		[TestCase(ShowTypeParameterList, "IEnumerable`1<T>")]
		[TestCase(ShowTypeParameterList | ShowTypeParameterVarianceModifier, "IEnumerable`1<+T>")]
		[TestCase(All, ".class interface public abstract System.Collections.Generic.IEnumerable`1<+T>")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "IEnumerable`1<T>")]
		public void GenericInterface(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(IEnumerable<>));
			ambience.ConversionFlags = flags;
			Assert.That(ambience.ConvertSymbol(typeDef), Is.EqualTo(expectedOutput));
		}

		[TestCase(None, "Enumerator")]
		[TestCase(ShowDefinitionKeyword, ".class Enumerator")]
		[TestCase(ShowAccessibility, "nested public Enumerator")]
		[TestCase(ShowDefinitionKeyword | ShowAccessibility, ".class nested public Enumerator")]
		[TestCase(ShowTypeParameterList, "Enumerator<T>")]
		[TestCase(ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, ".class nested public Enumerator<T>")]
		[TestCase(UseFullyQualifiedEntityNames | ShowTypeParameterList, "System.Collections.Generic.List`1<T>.Enumerator<T>")]
		[TestCase(ShowDeclaringType | ShowTypeParameterList, "List`1<T>.Enumerator<T>")]
		[TestCase(All, ".class nested public sealed serializable beforefieldinit System.Collections.Generic.List`1<T>.Enumerator<T>")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "Enumerator<T>")]
		public void GenericTypeWithNested(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(List<>.Enumerator));
			ambience.ConversionFlags = flags;
			Assert.That(ambience.ConvertSymbol(typeDef), Is.EqualTo(expectedOutput));
		}

		[TestCase(None, "StaticClass")]
		[TestCase(ShowDefinitionKeyword, ".class StaticClass")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword, ".class abstract sealed beforefieldinit StaticClass")]
		[TestCase(ShowModifiers | ShowAccessibility, "private abstract sealed beforefieldinit StaticClass")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword | ShowAccessibility, ".class private abstract sealed beforefieldinit StaticClass")]
		[TestCase(ShowModifiers | ShowTypeParameterList, "abstract sealed beforefieldinit StaticClass")]
		[TestCase(ShowModifiers | ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, ".class private abstract sealed beforefieldinit StaticClass")]
		[TestCase(All, ".class private abstract sealed beforefieldinit ICSharpCode.Decompiler.Tests.Output.StaticClass")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "StaticClass")]
		public void StaticClassTest(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(StaticClass));
			ambience.ConversionFlags = flags;
			Assert.That(ambience.ConvertSymbol(typeDef), Is.EqualTo(expectedOutput));
		}

		[TestCase(None, "SealedClass")]
		[TestCase(ShowDefinitionKeyword, ".class SealedClass")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword, ".class sealed beforefieldinit SealedClass")]
		[TestCase(ShowModifiers | ShowAccessibility, "private sealed beforefieldinit SealedClass")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword | ShowAccessibility, ".class private sealed beforefieldinit SealedClass")]
		[TestCase(ShowModifiers | ShowTypeParameterList, "sealed beforefieldinit SealedClass")]
		[TestCase(ShowModifiers | ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, ".class private sealed beforefieldinit SealedClass")]
		[TestCase(All, ".class private sealed beforefieldinit ICSharpCode.Decompiler.Tests.Output.SealedClass")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "SealedClass")]
		public void SealedClassTest(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(SealedClass));
			ambience.ConversionFlags = flags;
			Assert.That(ambience.ConvertSymbol(typeDef), Is.EqualTo(expectedOutput));
		}

		[TestCase(None, "RefStruct")]
		[TestCase(ShowDefinitionKeyword, ".class RefStruct")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword, ".class sealed beforefieldinit RefStruct")]
		[TestCase(ShowModifiers | ShowAccessibility, "private sealed beforefieldinit RefStruct")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword | ShowAccessibility, ".class private sealed beforefieldinit RefStruct")]
		[TestCase(ShowModifiers | ShowTypeParameterList, "sealed beforefieldinit RefStruct")]
		[TestCase(ShowModifiers | ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, ".class private sealed beforefieldinit RefStruct")]
		[TestCase(All, ".class private sealed beforefieldinit ICSharpCode.Decompiler.Tests.Output.RefStruct")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "RefStruct")]
		public void RefStructTest(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(RefStruct));
			ambience.ConversionFlags = flags;
			Assert.That(ambience.ConvertSymbol(typeDef), Is.EqualTo(expectedOutput));
		}

		[TestCase(None, "ReadonlyStruct")]
		[TestCase(ShowDefinitionKeyword, ".class ReadonlyStruct")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword, ".class sealed beforefieldinit ReadonlyStruct")]
		[TestCase(ShowModifiers | ShowAccessibility, "private sealed beforefieldinit ReadonlyStruct")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword | ShowAccessibility, ".class private sealed beforefieldinit ReadonlyStruct")]
		[TestCase(ShowModifiers | ShowTypeParameterList, "sealed beforefieldinit ReadonlyStruct")]
		[TestCase(ShowModifiers | ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, ".class private sealed beforefieldinit ReadonlyStruct")]
		[TestCase(All, ".class private sealed beforefieldinit ICSharpCode.Decompiler.Tests.Output.ReadonlyStruct")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "ReadonlyStruct")]
		public void ReadonlyStructTest(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(ReadonlyStruct));
			ambience.ConversionFlags = flags;
			Assert.That(ambience.ConvertSymbol(typeDef), Is.EqualTo(expectedOutput));
		}

		[TestCase(None, "ReadonlyRefStruct")]
		[TestCase(ShowDefinitionKeyword, ".class ReadonlyRefStruct")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword, ".class sealed beforefieldinit ReadonlyRefStruct")]
		[TestCase(ShowModifiers | ShowAccessibility, "private sealed beforefieldinit ReadonlyRefStruct")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword | ShowAccessibility, ".class private sealed beforefieldinit ReadonlyRefStruct")]
		[TestCase(ShowModifiers | ShowTypeParameterList, "sealed beforefieldinit ReadonlyRefStruct")]
		[TestCase(ShowModifiers | ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, ".class private sealed beforefieldinit ReadonlyRefStruct")]
		[TestCase(All, ".class private sealed beforefieldinit ICSharpCode.Decompiler.Tests.Output.ReadonlyRefStruct")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "ReadonlyRefStruct")]
		public void ReadonlyRefStructTest(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(ReadonlyRefStruct));
			ambience.ConversionFlags = flags;
			Assert.That(ambience.ConvertSymbol(typeDef), Is.EqualTo(expectedOutput));
		}
		#endregion

		#region Delegate tests
		[TestCase(None, "Func`2")]
		[TestCase(ShowTypeParameterList, "Func`2<T,TResult>")]
		[TestCase(ShowTypeParameterList | ShowTypeParameterVarianceModifier, "Func`2<-T,+TResult>")]
		[TestCase(All, ".class public sealed System.Func`2<-T,+TResult>")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "Func`2<T,TResult>")]
		public void FuncDelegate(ConversionFlags flags, string expectedOutput)
		{
			var func = GetDefinition(typeof(Func<,>));
			ambience.ConversionFlags = flags;
			Assert.That(ambience.ConvertSymbol(func), Is.EqualTo(expectedOutput));
		}
		#endregion

		#region IField tests
		[TestCase(All & ~PlaceReturnTypeAfterParameterList, ".field private instance int32 ICSharpCode.Decompiler.Tests.Output.Program::test")]
		[TestCase(ILSpyMainTreeViewMemberFlags, "test : int32")]
		[TestCase(All & ~(ShowDeclaringType | ShowModifiers | ShowAccessibility | PlaceReturnTypeAfterParameterList), ".field int32 ICSharpCode.Decompiler.Tests.Output.Program::test")]
		public void SimpleField(ConversionFlags flags, string expectedOutput)
		{
			var field = GetDefinition(typeof(Program)).GetFields(f => f.Name == "test").Single();
			ambience.ConversionFlags = flags;

			Assert.That(ambience.ConvertSymbol(field), Is.EqualTo(expectedOutput));
		}

		[TestCase(All & ~PlaceReturnTypeAfterParameterList, ".field private static literal int32 ICSharpCode.Decompiler.Tests.Output.Program::TEST2")]
		[TestCase(ILSpyMainTreeViewMemberFlags, "TEST2 : int32")]
		public void SimpleConstField(ConversionFlags flags, string expectedOutput)
		{
			var field = compilation.FindType(typeof(Program)).GetFields(f => f.Name == "TEST2").Single();
			ambience.ConversionFlags = flags;

			Assert.That(ambience.ConvertSymbol(field), Is.EqualTo(expectedOutput));
		}
		#endregion

		#region IEvent tests
		[Test]
		public void EventWithDeclaringType()
		{
			var ev = compilation.FindType(typeof(Program)).GetEvents(f => f.Name == "ProgramChanged").Single();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.ShowDeclaringType;
			string result = ambience.ConvertSymbol(ev);

			Assert.That(result, Is.EqualTo(".event instance EventHandler Program::ProgramChanged"));
		}

		[Test]
		public void CustomEvent()
		{
			var ev = compilation.FindType(typeof(Program)).GetEvents(f => f.Name == "SomeEvent").Single();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
			string result = ambience.ConvertSymbol(ev);

			Assert.That(result, Is.EqualTo(".event instance EventHandler SomeEvent"));
		}
		#endregion

		#region Property tests
		[TestCase(StandardConversionFlags, ".property instance int32 Test")]
		[TestCase(ILSpyMainTreeViewMemberFlags, "Test : int32")]
		public void AutomaticProperty(ConversionFlags flags, string expectedOutput)
		{
			var prop = compilation.FindType(typeof(Program)).GetProperties(p => p.Name == "Test").Single();
			ambience.ConversionFlags = flags;

			Assert.That(ambience.ConvertSymbol(prop), Is.EqualTo(expectedOutput));
		}

		[TestCase(StandardConversionFlags, ".property instance int32 Item(int32 index)")]
		[TestCase(ILSpyMainTreeViewMemberFlags, "Item(int32) : int32")]
		public void Indexer(ConversionFlags flags, string expectedOutput)
		{
			var prop = compilation.FindType(typeof(Program)).GetProperties(p => p.IsIndexer && !p.IsExplicitInterfaceImplementation).Single();
			ambience.ConversionFlags = flags;

			Assert.That(ambience.ConvertSymbol(prop), Is.EqualTo(expectedOutput));
		}

		[TestCase(StandardConversionFlags, ".property instance int32 ICSharpCode.Decompiler.Tests.Output.Interface.Item(int32 index)")]
		[TestCase(ILSpyMainTreeViewMemberFlags, "ICSharpCode.Decompiler.Tests.Output.Interface.Item(int32) : int32")]
		public void ExplicitIndexer(ConversionFlags flags, string expectedOutput)
		{
			var prop = compilation.FindType(typeof(Program)).GetProperties(p => p.IsIndexer && p.IsExplicitInterfaceImplementation).Single();
			ambience.ConversionFlags = flags;

			Assert.That(ambience.ConvertSymbol(prop), Is.EqualTo(expectedOutput));
		}
		#endregion

		#region IMethod tests
		[TestCase(StandardConversionFlags, ".method public hidebysig specialname rtspecialname instance .ctor(int32 x)")]
		[TestCase(ILSpyMainTreeViewMemberFlags, ".ctor(int32)")]
		public void ConstructorTests(ConversionFlags flags, string expectedOutput)
		{
			var prop = compilation.FindType(typeof(Program)).GetConstructors().Single();
			ambience.ConversionFlags = flags;

			Assert.That(ambience.ConvertSymbol(prop), Is.EqualTo(expectedOutput));
		}

		[TestCase(StandardConversionFlags, ".method family hidebysig virtual instance void Finalize()")]
		[TestCase(ILSpyMainTreeViewMemberFlags, "Finalize() : void")]
		public void DestructorTests(ConversionFlags flags, string expectedOutput)
		{
			var dtor = compilation.FindType(typeof(Program))
				.GetMembers(m => m.SymbolKind == SymbolKind.Destructor, GetMemberOptions.IgnoreInheritedMembers).Single();
			ambience.ConversionFlags = flags;

			Assert.That(ambience.ConvertSymbol(dtor), Is.EqualTo(expectedOutput));
		}
		#endregion
	}

	#region Test types
#pragma warning disable 169, 67

	class Test { }
	static class StaticClass { }
	sealed class SealedClass { }
	ref struct RefStruct { }
	readonly struct ReadonlyStruct { }
	readonly ref struct ReadonlyRefStruct { }

	interface Interface
	{
		int this[int x] { get; }
	}

	class Program : Interface
	{
		int test;
		const int TEST2 = 2;

		public int Test { get; set; }

		public int this[int index] {
			get {
				return index;
			}
		}

		int Interface.this[int index] {
			get {
				return index;
			}
		}

		public event EventHandler ProgramChanged;

		public event EventHandler SomeEvent {
			add { }
			remove { }
		}

		public static bool operator +(Program lhs, Program rhs)
		{
			throw new NotImplementedException();
		}

		public static implicit operator Test(Program lhs)
		{
			throw new NotImplementedException();
		}

		public static explicit operator int(Program lhs)
		{
			throw new NotImplementedException();
		}

		public Program(int x)
		{

		}

		~Program()
		{

		}

		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}

		public static void InParameter(in int a)
		{

		}
	}
	#endregion
}
