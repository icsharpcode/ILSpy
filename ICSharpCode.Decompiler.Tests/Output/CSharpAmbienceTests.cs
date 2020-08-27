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

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.Tests.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

using static ICSharpCode.Decompiler.Output.ConversionFlags;

namespace ICSharpCode.Decompiler.Tests.Output
{
	[TestFixture]
	public class CSharpAmbienceTests
	{
		ICompilation compilation;
		CSharpAmbience ambience;

		[OneTimeSetUp]
		public void FixtureSetUp()
		{
			ambience = new CSharpAmbience();

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
			Assert.IsNotNull(foundType);
			return foundType;
		}

		const ConversionFlags ILSpyMainTreeViewTypeFlags = ShowTypeParameterList | PlaceReturnTypeAfterParameterList;
		const ConversionFlags ILSpyMainTreeViewMemberFlags = ILSpyMainTreeViewTypeFlags | ShowParameterList | ShowReturnType | ShowParameterModifiers;

		#region ITypeDefinition tests
		[TestCase(None, "Dictionary")]
		[TestCase(ShowDefinitionKeyword, "class Dictionary")]
		[TestCase(ShowAccessibility, "public Dictionary")]
		[TestCase(ShowDefinitionKeyword | ShowAccessibility, "public class Dictionary")]
		[TestCase(ShowTypeParameterList, "Dictionary<TKey,TValue>")]
		[TestCase(ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, "public class Dictionary<TKey,TValue>")]
		[TestCase(UseFullyQualifiedEntityNames | ShowTypeParameterList, "System.Collections.Generic.Dictionary<TKey,TValue>")]
		[TestCase(UseFullyQualifiedEntityNames | ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, "public class System.Collections.Generic.Dictionary<TKey,TValue>")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "Dictionary<TKey,TValue>")]
		public void GenericType(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(Dictionary<,>));
			ambience.ConversionFlags = flags;
			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(typeDef));
		}

		[TestCase(None, "Object")]
		[TestCase(ShowDefinitionKeyword, "class Object")]
		[TestCase(ShowAccessibility, "public Object")]
		[TestCase(ShowDefinitionKeyword | ShowAccessibility, "public class Object")]
		[TestCase(ShowTypeParameterList, "Object")]
		[TestCase(ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, "public class Object")]
		[TestCase(UseFullyQualifiedEntityNames | ShowTypeParameterList, "System.Object")]
		[TestCase(All, "public class System.Object")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "Object")]
		public void SimpleType(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(object));
			ambience.ConversionFlags = flags;
			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(typeDef));
		}

		[TestCase(None, "IEnumerable")]
		[TestCase(ShowTypeParameterList, "IEnumerable<T>")]
		[TestCase(ShowTypeParameterList | ShowTypeParameterVarianceModifier, "IEnumerable<out T>")]
		[TestCase(All, "public interface System.Collections.Generic.IEnumerable<out T>")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "IEnumerable<T>")]
		public void GenericInterface(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(IEnumerable<>));
			ambience.ConversionFlags = flags;
			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(typeDef));
		}

		[TestCase(None, "Enumerator")]
		[TestCase(ShowDefinitionKeyword, "struct Enumerator")]
		[TestCase(ShowAccessibility, "public Enumerator")]
		[TestCase(ShowDefinitionKeyword | ShowAccessibility, "public struct Enumerator")]
		[TestCase(ShowTypeParameterList, "Enumerator")]
		[TestCase(ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, "public struct Enumerator")]
		[TestCase(UseFullyQualifiedEntityNames | ShowTypeParameterList, "System.Collections.Generic.List<T>.Enumerator")]
		[TestCase(ShowDeclaringType | ShowTypeParameterList, "List<T>.Enumerator")]
		[TestCase(All, "public struct System.Collections.Generic.List<T>.Enumerator")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "Enumerator")]
		public void GenericTypeWithNested(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(List<>.Enumerator));
			ambience.ConversionFlags = flags;
			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(typeDef));
		}

		[TestCase(None, "StaticClass")]
		[TestCase(ShowDefinitionKeyword, "class StaticClass")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword, "static class StaticClass")]
		[TestCase(ShowModifiers | ShowAccessibility, "private static StaticClass")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword | ShowAccessibility, "private static class StaticClass")]
		[TestCase(ShowModifiers | ShowTypeParameterList, "static StaticClass")]
		[TestCase(ShowModifiers | ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, "private static class StaticClass")]
		[TestCase(All, "private static class ICSharpCode.Decompiler.Tests.Output.CSharpAmbienceTests.StaticClass")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "StaticClass")]
		public void StaticClassTest(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(StaticClass));
			ambience.ConversionFlags = flags;
			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(typeDef));
		}

		[TestCase(None, "SealedClass")]
		[TestCase(ShowDefinitionKeyword, "class SealedClass")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword, "sealed class SealedClass")]
		[TestCase(ShowModifiers | ShowAccessibility, "private sealed SealedClass")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword | ShowAccessibility, "private sealed class SealedClass")]
		[TestCase(ShowModifiers | ShowTypeParameterList, "sealed SealedClass")]
		[TestCase(ShowModifiers | ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, "private sealed class SealedClass")]
		[TestCase(All, "private sealed class ICSharpCode.Decompiler.Tests.Output.CSharpAmbienceTests.SealedClass")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "SealedClass")]
		public void SealedClassTest(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(SealedClass));
			ambience.ConversionFlags = flags;
			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(typeDef));
		}

		[TestCase(None, "RefStruct")]
		[TestCase(ShowDefinitionKeyword, "struct RefStruct")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword, "ref struct RefStruct")]
		[TestCase(ShowModifiers | ShowAccessibility, "private ref RefStruct")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword | ShowAccessibility, "private ref struct RefStruct")]
		[TestCase(ShowModifiers | ShowTypeParameterList, "ref RefStruct")]
		[TestCase(ShowModifiers | ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, "private ref struct RefStruct")]
		[TestCase(All, "private ref struct ICSharpCode.Decompiler.Tests.Output.CSharpAmbienceTests.RefStruct")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "RefStruct")]
		public void RefStructTest(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(RefStruct));
			ambience.ConversionFlags = flags;
			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(typeDef));
		}

		[TestCase(None, "ReadonlyStruct")]
		[TestCase(ShowDefinitionKeyword, "struct ReadonlyStruct")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword, "readonly struct ReadonlyStruct")]
		[TestCase(ShowModifiers | ShowAccessibility, "private readonly ReadonlyStruct")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword | ShowAccessibility, "private readonly struct ReadonlyStruct")]
		[TestCase(ShowModifiers | ShowTypeParameterList, "readonly ReadonlyStruct")]
		[TestCase(ShowModifiers | ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, "private readonly struct ReadonlyStruct")]
		[TestCase(All, "private readonly struct ICSharpCode.Decompiler.Tests.Output.CSharpAmbienceTests.ReadonlyStruct")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "ReadonlyStruct")]
		public void ReadonlyStructTest(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(ReadonlyStruct));
			ambience.ConversionFlags = flags;
			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(typeDef));
		}

		[TestCase(None, "ReadonlyRefStruct")]
		[TestCase(ShowDefinitionKeyword, "struct ReadonlyRefStruct")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword, "readonly ref struct ReadonlyRefStruct")]
		[TestCase(ShowModifiers | ShowAccessibility, "private readonly ref ReadonlyRefStruct")]
		[TestCase(ShowModifiers | ShowDefinitionKeyword | ShowAccessibility, "private readonly ref struct ReadonlyRefStruct")]
		[TestCase(ShowModifiers | ShowTypeParameterList, "readonly ref ReadonlyRefStruct")]
		[TestCase(ShowModifiers | ShowTypeParameterList | ShowDefinitionKeyword | ShowAccessibility, "private readonly ref struct ReadonlyRefStruct")]
		[TestCase(All, "private readonly ref struct ICSharpCode.Decompiler.Tests.Output.CSharpAmbienceTests.ReadonlyRefStruct")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "ReadonlyRefStruct")]
		public void ReadonlyRefStructTest(ConversionFlags flags, string expectedOutput)
		{
			var typeDef = GetDefinition(typeof(ReadonlyRefStruct));
			ambience.ConversionFlags = flags;
			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(typeDef));
		}
		#endregion

		#region Delegate tests
		[TestCase(None, "Func")]
		[TestCase(ShowTypeParameterList, "Func<T,TResult>")]
		[TestCase(ShowTypeParameterList | ShowTypeParameterVarianceModifier, "Func<in T,out TResult>")]
		[TestCase(ShowTypeParameterList | ShowReturnType | ShowTypeParameterVarianceModifier, "TResult Func<in T,out TResult>")]
		[TestCase(ShowTypeParameterList | ShowParameterList | ShowTypeParameterVarianceModifier, "Func<in T,out TResult>(T)")]
		[TestCase(ShowTypeParameterList | ShowParameterList | ShowReturnType | ShowTypeParameterVarianceModifier, "TResult Func<in T,out TResult>(T)")]
		[TestCase(All & ~PlaceReturnTypeAfterParameterList, "public delegate TResult System.Func<in T,out TResult>(T arg);")]
		[TestCase(All, "public delegate System.Func<in T,out TResult>(T arg) : TResult;")]
		[TestCase(ILSpyMainTreeViewTypeFlags, "Func<T,TResult>")]
		public void FuncDelegate(ConversionFlags flags, string expectedOutput)
		{
			var func = GetDefinition(typeof(Func<,>));
			ambience.ConversionFlags = flags;
			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(func));
		}
		#endregion

		#region IField tests
		[TestCase(All & ~PlaceReturnTypeAfterParameterList, "private int ICSharpCode.Decompiler.Tests.Output.CSharpAmbienceTests.Program.test;")]
		[TestCase(ILSpyMainTreeViewMemberFlags, "test : int")]
		[TestCase(ConversionFlags.All & ~(ConversionFlags.ShowDeclaringType | ConversionFlags.ShowModifiers | ConversionFlags.ShowAccessibility | ConversionFlags.PlaceReturnTypeAfterParameterList), "int test;")]
		public void SimpleField(ConversionFlags flags, string expectedOutput)
		{
			var field = GetDefinition(typeof(CSharpAmbienceTests.Program)).GetFields(f => f.Name == "test").Single();
			ambience.ConversionFlags = flags;

			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(field));
		}

		[TestCase(All & ~PlaceReturnTypeAfterParameterList, "private const int ICSharpCode.Decompiler.Tests.Output.CSharpAmbienceTests.Program.TEST2;")]
		[TestCase(ILSpyMainTreeViewMemberFlags, "TEST2 : int")]
		public void SimpleConstField(ConversionFlags flags, string expectedOutput)
		{
			var field = compilation.FindType(typeof(CSharpAmbienceTests.Program)).GetFields(f => f.Name == "TEST2").Single();
			ambience.ConversionFlags = flags;

			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(field));
		}
		#endregion

		#region IEvent tests
		[Test]
		public void EventWithDeclaringType()
		{
			var ev = compilation.FindType(typeof(CSharpAmbienceTests.Program)).GetEvents(f => f.Name == "ProgramChanged").Single();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags | ConversionFlags.ShowDeclaringType;
			string result = ambience.ConvertSymbol(ev);

			Assert.AreEqual("public event EventHandler Program.ProgramChanged;", result);
		}

		[Test]
		public void CustomEvent()
		{
			var ev = compilation.FindType(typeof(CSharpAmbienceTests.Program)).GetEvents(f => f.Name == "SomeEvent").Single();
			ambience.ConversionFlags = ConversionFlags.StandardConversionFlags;
			string result = ambience.ConvertSymbol(ev);

			Assert.AreEqual("public event EventHandler SomeEvent;", result);
		}
		#endregion

		#region Property tests
		[TestCase(StandardConversionFlags, "public int Test { get; set; }")]
		[TestCase(ILSpyMainTreeViewMemberFlags, "Test : int")]
		public void AutomaticProperty(ConversionFlags flags, string expectedOutput)
		{
			var prop = compilation.FindType(typeof(CSharpAmbienceTests.Program)).GetProperties(p => p.Name == "Test").Single();
			ambience.ConversionFlags = flags;

			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(prop));
		}

		[TestCase(StandardConversionFlags, "public int this[int index] { get; }")]
		[TestCase(ILSpyMainTreeViewMemberFlags, "this[int] : int")]
		public void Indexer(ConversionFlags flags, string expectedOutput)
		{
			var prop = compilation.FindType(typeof(CSharpAmbienceTests.Program)).GetProperties(p => p.IsIndexer).Single();
			ambience.ConversionFlags = flags;

			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(prop));
		}
		#endregion

		#region IMethod tests
		[TestCase(StandardConversionFlags, "public Program(int x);")]
		[TestCase(ILSpyMainTreeViewMemberFlags, "Program(int)")]
		public void ConstructorTests(ConversionFlags flags, string expectedOutput)
		{
			var prop = compilation.FindType(typeof(CSharpAmbienceTests.Program)).GetConstructors().Single();
			ambience.ConversionFlags = flags;

			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(prop));
		}

		[TestCase(StandardConversionFlags, "~Program();")]
		[TestCase(ILSpyMainTreeViewMemberFlags, "~Program()")]
		public void DestructorTests(ConversionFlags flags, string expectedOutput)
		{
			var dtor = compilation.FindType(typeof(CSharpAmbienceTests.Program))
				.GetMembers(m => m.SymbolKind == SymbolKind.Destructor, GetMemberOptions.IgnoreInheritedMembers).Single();
			ambience.ConversionFlags = flags;

			Assert.AreEqual(expectedOutput, ambience.ConvertSymbol(dtor));
		}
		#endregion

		#region Test types
#pragma warning disable 169, 67

		class Test { }
		static class StaticClass { }
		sealed class SealedClass { }
		ref struct RefStruct { }
		readonly struct ReadonlyStruct { }
		readonly ref struct ReadonlyRefStruct { }

		class Program
		{
			int test;
			const int TEST2 = 2;

			public int Test { get; set; }

			public int this[int index] {
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
}
