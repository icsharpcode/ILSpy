// Copyright (c) 2026 Siegfried Pammer
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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

using DecompilerSymbolKind = ICSharpCode.Decompiler.TypeSystem.SymbolKind;

namespace ICSharpCode.Decompiler.Tests.Documentation
{
	[TestFixture]
	public class IdStringProviderTests
	{
		// ----------------------------------------------------------------
		// Test assembly source.
		// Mirrors the C# spec sections D.3 and D.5 examples and adds edge cases.
		// ----------------------------------------------------------------

		private const string testSource = """
#line 45
using System;

// Top-level enum (no namespace)
enum Color { Red, Blue, Green }

namespace Acme
{
    interface IProcess { }

    struct ValueType
    {
        private int total;
        public void M(int i) { }
    }

    class Widget : IProcess
    {
        public class NestedClass
        {
            private int value;
            public void M(int i) { }
        }

        public interface IMenuItem { }
        public delegate void Del(int i);
        public enum Direction { North, South, East, West }

        // Fields
        private string message;
        private static Color defaultColor;
        private const double PI = 3.14159;
        protected readonly double monthlyAverage;
        private long[] array1;
        private Widget[,] array2;
        private unsafe int* pCount;
        private unsafe float** ppValues;
        private nint nativeInt;

        // Constructors
        static Widget() { }
        public Widget() { }
        public Widget(string s) { }

        // Finalizer
        ~Widget() { }

        // Methods
        public static void M0() { }
        public void M1(char c, out float f, ref ValueType v, in int i) { f = 0; }
        public void M2(short[] x1, int[,] x2, long[][] x3) { }
        public void M3(long[][] x3, Widget[][,,] x4) { }
        public unsafe void M4(char* pc, Color** pf) { }
        public unsafe void M5(void* pv, double*[][,] pd) { }
        public void M6(int i, params object[] args) { }
		public void M7(nint x, nuint y) { }

        // Properties & indexers
        public int Width { get; set; }
        public int this[int i] { get { return 0; } set { } }
        public int this[string s, int i] { get { return 0; } set { } }

        // Event
        public event Del AnEvent;

        // Operators
        public static Widget operator +(Widget x) { return x; }
        public static Widget operator +(Widget x1, Widget x2) { return x1; }
        public static explicit operator int(Widget x) { return 0; }
        public static implicit operator long(Widget x) { return 0; }
    }

    class MyList<T>
    {
        class Helper<U, V> { }
        public void Test(T t) { }
    }

    class UseList
    {
        public void Process(MyList<int> list) { }
        public MyList<T> GetValues<T>(T value) { return null; }
    }
}

namespace Graphics
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point() : this(0, 0) { }
        public Point(int xPosition, int yPosition)
        {
            X = xPosition;
            Y = yPosition;
        }

        public void Move(int xPosition, int yPosition)
        {
            X = xPosition;
            Y = yPosition;
        }

        public void Translate(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }

        public override bool Equals(object o) => false;
        public override int GetHashCode() => X + (Y >> 4);
        public override string ToString() => $"({X},{Y})";

        public static bool operator ==(Point p1, Point p2) => false;
        public static bool operator !=(Point p1, Point p2) => true;
    }
}

namespace ExplicitImpl
{
    interface IFoo
    {
        void Bar();
        int Baz { get; }
    }

    interface IFoo<T>
    {
        void Generic(T t);
    }

    class Impl : IFoo, IFoo<int>
    {
        void IFoo.Bar() { }
        int IFoo.Baz { get { return 0; } }
        void IFoo<int>.Generic(int t) { }
    }
}

namespace Tuples
{
    class TupleTests
    {
        // ValueTuple in field types
        private (int, string) tupleField;
        private (int x, string y) namedTupleField;

        // ValueTuple in method signatures
        public (int, string) GetTuple() { return (1, "a"); }
        public void TakesTuple((int a, string b) t) { }
        public (int, (string, bool)) NestedTuple() { return (1, ("a", true)); }

        // Tuple as generic argument
        public System.Collections.Generic.List<(int, string)> TupleInGeneric() { return null; }
    }
}

namespace NullableTests
{
    class NullableValueTypes
    {
        private int? nullableField;
        public void TakesNullable(int? x, double? y) { }
        public int? ReturnsNullable() { return null; }
        public System.Collections.Generic.List<int?> NullableInGeneric() { return null; }
    }
}

namespace RefReturns
{
    class RefReturnTests
    {
        private int[] data = new int[10];
        public ref int RefReturn() { return ref data[0]; }
        public ref readonly int RefReadonlyReturn() { return ref data[0]; }
    }
}

namespace DynamicTests
{
    class DynamicMethods
    {
        // dynamic becomes System.Object in metadata ID strings
        public void TakesDynamic(dynamic d) { }
        public dynamic ReturnsDynamic() { return null; }
    }
}

namespace DefaultInterfaceMethods
{
    interface IWithDefault
    {
        void Required();
        void WithDefault() { }  // default interface method
        static void StaticMethod() { }
    }

    interface IStaticAbstract<T> where T : IStaticAbstract<T>
    {
        static abstract T Create();
        static virtual T CreateDefault() { return default; }
    }
}

namespace RecordTests
{
    // Record class - generates Equals, GetHashCode, ToString, PrintMembers,
    // Deconstruct, op_Equality, op_Inequality, Clone, copy ctor
    record RecordClass(int X, string Y);

    // Record struct
    record struct RecordStruct(int A, double B);

    // Record with explicit members
    record RecordWithCustom(int Value)
    {
        public int ComputedProp => Value * 2;
        public void CustomMethod() { }
    }
}

namespace DeepNesting
{
    class Level1
    {
        public class Level2
        {
            public class Level3
            {
                public class Level4
                {
                    public void DeepMethod(int x) { }
                    public int DeepProp { get; set; }
                }
            }
        }
    }

    class GenericLevel1<T>
    {
        public class GenericLevel2<U>
        {
            public class GenericLevel3<V>
            {
                public void MixedMethod(T t, U u, V v) { }
                public System.Collections.Generic.Dictionary<T, System.Collections.Generic.List<V>> ComplexReturn() { return null; }
            }
        }
    }
}

namespace GenericEdgeCases
{
    class GenericOperators<T>
    {
        public static GenericOperators<T> operator +(GenericOperators<T> a, GenericOperators<T> b) { return a; }
        public static explicit operator int(GenericOperators<T> x) { return 0; }
    }

    // Method using both class and method type params in complex ways
    class MixedGenerics<T>
    {
        public System.Collections.Generic.Dictionary<T, U> Mix<U>(T t, U u, System.Collections.Generic.List<T> list) { return null; }
        public void NestedGenericParam<U>(System.Collections.Generic.Dictionary<System.Collections.Generic.List<T>, U> complex) { }
        // Generic method returning array of generic type
        public T[] ArrayOfT(T input) { return null; }
        // Multi-dim array of generic type
        public T[,] MultiDimOfT() { return null; }
    }

    // Explicit interface impl with multiple generic type args
    interface IMultiGeneric<T, U>
    {
        void Process(T t, U u);
    }

    class MultiGenericImpl : IMultiGeneric<int, string>
    {
        void IMultiGeneric<int, string>.Process(int t, string u) { }
    }

    // Self-referencing generic constraint
    class Comparable<T> where T : System.IComparable<T>
    {
        public void Compare(T a, T b) { }
    }
}

namespace ArrayEdgeCases
{
    class ArrayMethods
    {
        // Multi-dim arrays as generic type arguments
        public System.Collections.Generic.List<int[,]> MultiDimInGeneric() { return null; }

        // Array of arrays of different dimensions
        public int[][,,][] WeirdArrays() { return null; }

        // Params with multi-dim
        public void ParamsMultiDim(params int[][] args) { }

        // Jagged array of generic type
        public System.Collections.Generic.List<int>[][] JaggedGenericArray() { return null; }
    }
}

namespace InitOnlyAndRequired
{
    class InitOnlyProps
    {
        public int InitProp { get; init; }
        public required string RequiredProp { get; set; }
    }

    // Required + init on a record
    record InitRecord
    {
        public required int Id { get; init; }
    }
}

namespace RefStructTests
{
    ref struct MyRefStruct
    {
        public int Value;
        public void DoSomething(int x) { }
    }

    class UsesRefStruct
    {
        public void TakesSpan(System.Span<int> span) { }
        public void TakesReadOnlySpan(System.ReadOnlySpan<int> span) { }
    }
}

namespace Overloads
{
    class OverloadResolution
    {
        public void M(int x) { }
        public void M(string x) { }
        public void M(int x, string y) { }
        public void M<T>(T x) { }
        public void M<T, U>(T x, U y) { }
        // Overload differing only by ref-ness
        public void ByRef(ref int x) { }
        public void ByRef(int x) { }
    }
}

namespace SpecialNames
{
    class Operators
    {
        // All remaining unary operators
        public static Operators operator -(Operators x) { return x; }
        public static bool operator !(Operators x) { return false; }
        public static Operators operator ~(Operators x) { return x; }
        public static Operators operator ++(Operators x) { return x; }
        public static Operators operator --(Operators x) { return x; }
        public static bool operator true(Operators x) { return true; }
        public static bool operator false(Operators x) { return false; }

        // All remaining binary operators
        public static Operators operator -(Operators a, Operators b) { return a; }
        public static Operators operator *(Operators a, Operators b) { return a; }
        public static Operators operator /(Operators a, Operators b) { return a; }
        public static Operators operator %(Operators a, Operators b) { return a; }
        public static Operators operator &(Operators a, Operators b) { return a; }
        public static Operators operator |(Operators a, Operators b) { return a; }
        public static Operators operator ^(Operators a, Operators b) { return a; }
        public static Operators operator <<(Operators a, int b) { return a; }
        public static Operators operator >>(Operators a, int b) { return a; }
        public static bool operator ==(Operators a, Operators b) { return true; }
        public static bool operator !=(Operators a, Operators b) { return false; }
        public static bool operator <(Operators a, Operators b) { return false; }
        public static bool operator <=(Operators a, Operators b) { return false; }
        public static bool operator >(Operators a, Operators b) { return false; }
        public static bool operator >=(Operators a, Operators b) { return false; }

        public override bool Equals(object o) { return false; }
        public override int GetHashCode() { return 0; }
    }
}

namespace ByRefLikeParams
{
    class ScopedTests
    {
        // scoped doesn't affect the ID string, but good to verify
        public void TakesScopedSpan(scoped System.Span<int> span) { }
        public void TakesScopedReadOnlySpan(scoped System.ReadOnlySpan<int> span) { }
    }
}

namespace FnPtrs
{
    class FnPtrParameters
    {
        public unsafe void TakesFnPtr(delegate*<int, string> fnptr) { }
        public unsafe void TakesFnPtr(delegate*<int, int> fnptr) { }
    }
}

namespace NestedGenericInstantiations
{
    public class Outer<T>
    {
        public class Inner { }
        public class Inner2<U> { }
    }

    public class Consumer
    {
        public void TakesInner(Outer<int>.Inner x) { }
        public void TakesInner2(Outer<int>.Inner2<string> x) { }
        public void TakesDeep(Outer<Outer<int>.Inner>.Inner2<Outer<string>.Inner> x) { }
        internal void TakesThreeLevels(DeepNesting.GenericLevel1<int>.GenericLevel2<string>.GenericLevel3<bool> x) { }
        public Outer<int>.Inner ReturnsInner() { return null; }
    }
}

namespace CheckedOperators
{
    public class Money
    {
        public static explicit operator int(Money m) { return 0; }
        public static explicit operator checked int(Money m) { return 0; }
    }
}

namespace ModreqParams
{
    public interface IWithIn
    {
        // 'in' parameters of interface/virtual methods carry modreq(InAttribute).
        void TakesIn(in int x);
    }
}
""";

		private static CSharpCompilation roslynCompilation;
		private static DecompilerTypeSystem decompilerTypeSystem;
		private static string tempDllPath;

		/// <summary>
		/// Maps Roslyn documentation comment IDs to ISymbol, for every symbol in the compilation.
		/// </summary>
		private static Dictionary<string, Microsoft.CodeAnalysis.ISymbol> roslynIdMap;

		[OneTimeSetUp]
		public void SetUp()
		{
			// ----------------------------------------------------------
			// 1. Build a Roslyn compilation (in-memory) to get the
			//    authoritative ID strings via DocumentationCommentId.
			// ----------------------------------------------------------
			var syntaxTree = CSharpSyntaxTree.ParseText(testSource);
			roslynCompilation = CSharpCompilation.Create(
				"IdStringTestAssembly",
				new[] { syntaxTree },
				Tester.CoreDefaultReferences.Select(r => MetadataReference.CreateFromFile(Path.Combine(Tester.RefAssembliesToolset.GetPath(Tester.CurrentNetCoreAppVersion), r))),
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true));

			// Verify the compilation has no errors (warnings are OK)
			var diagnostics = roslynCompilation.GetDiagnostics()
				.Where(d => d.Severity == DiagnosticSeverity.Error)
				.ToList();
			Assert.That(diagnostics, Is.Empty,
				"Test source has compilation errors:\n" +
				string.Join("\n", diagnostics.Select(d => d.ToString())));

			// Build the Roslyn ID -> symbol map
			roslynIdMap = new Dictionary<string, Microsoft.CodeAnalysis.ISymbol>();
			CollectSymbols(roslynCompilation.GlobalNamespace);

			// ----------------------------------------------------------
			// 2. Emit to a temp DLL and load it into DecompilerTypeSystem
			//    so we can test the original IdStringProvider.
			// ----------------------------------------------------------
			tempDllPath = Path.Combine(Path.GetTempPath(),
				"IdStringTestAssembly_" + Guid.NewGuid().ToString("N") + ".dll");
			var emitResult = roslynCompilation.Emit(tempDllPath);
			Assert.That(emitResult.Success, Is.True,
				"Emit failed:\n" + string.Join("\n", emitResult.Diagnostics.Select(d => d.ToString())));

			var module = new PEFile(tempDllPath);
			decompilerTypeSystem = new DecompilerTypeSystem(module, new UniversalAssemblyResolver(
				tempDllPath, false, module.DetectTargetFrameworkId()));
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			decompilerTypeSystem = null;
			roslynCompilation = null;
			roslynIdMap = null;
			if (tempDllPath != null && File.Exists(tempDllPath))
			{
				try
				{ File.Delete(tempDllPath); }
				catch { /* best effort cleanup */ }
			}
		}

		// ------------------------------------------------------------------
		// Recursively collect all symbols and their Roslyn-generated IDs
		// ------------------------------------------------------------------

		private static void CollectSymbols(INamespaceOrTypeSymbol symbol)
		{
			foreach (var member in symbol.GetMembers())
			{
				string id = member.GetDocumentationCommentId();
				if (id != null && !roslynIdMap.ContainsKey(id))
				{
					roslynIdMap[id] = member;
				}

				if (member is INamespaceOrTypeSymbol nsOrType)
				{
					CollectSymbols(nsOrType);
				}
			}
		}

		// ------------------------------------------------------------------
		// Lookup helpers for the decompiler type system
		// ------------------------------------------------------------------

		private ITypeDefinition FindType(string fullName)
		{
			var type = decompilerTypeSystem.FindType(new FullTypeName(fullName)).GetDefinition();
			Assert.That(type, Is.Not.Null, $"Type '{fullName}' not found in decompiler type system");
			return type;
		}

		private IMethod FindMethod(string typeName, string methodName,
			int paramCount = -1, int typeParamCount = 0)
		{
			var type = FindType(typeName);
			var methods = type.Methods
				.Where(m => m.Name == methodName)
				.Where(m => typeParamCount == 0 || m.TypeParameters.Count == typeParamCount);
			if (paramCount >= 0)
				methods = methods.Where(m => m.Parameters.Count == paramCount);
			var method = methods.FirstOrDefault();
			Assert.That(method, Is.Not.Null,
				$"Method '{methodName}' (params={paramCount}, tparams={typeParamCount}) not found on '{typeName}'");
			return method;
		}

		private IField FindField(string typeName, string fieldName)
		{
			var type = FindType(typeName);
			var field = type.Fields.FirstOrDefault(f => f.Name == fieldName);
			Assert.That(field, Is.Not.Null, $"Field '{fieldName}' not found on '{typeName}'");
			return field;
		}

		private IProperty FindProperty(string typeName, string propertyName, int paramCount = -1)
		{
			var type = FindType(typeName);
			var props = type.Properties.Where(p => p.Name == propertyName);
			if (paramCount >= 0)
				props = props.Where(p => p.Parameters.Count == paramCount);
			var prop = props.FirstOrDefault();
			Assert.That(prop, Is.Not.Null, $"Property '{propertyName}' not found on '{typeName}'");
			return prop;
		}

		private IEvent FindEvent(string typeName, string eventName)
		{
			var type = FindType(typeName);
			var evt = type.Events.FirstOrDefault(e => e.Name == eventName);
			Assert.That(evt, Is.Not.Null, $"Event '{eventName}' not found on '{typeName}'");
			return evt;
		}

		// ------------------------------------------------------------------
		// Assertion helpers
		// ------------------------------------------------------------------

		/// <summary>
		/// Assert that the decompiler's GetIdString matches Roslyn's
		/// DocumentationCommentId for the given entity (no hardcoded expected value).
		/// </summary>
		private void AssertMatchesRoslyn(IEntity entity)
		{
			string decompilerId = IdStringProvider.GetIdString(entity.ParentModule.MetadataFile, entity.MetadataToken);
			Assert.That(roslynIdMap.ContainsKey(decompilerId), Is.True,
				$"Decompiler produced ID '{decompilerId}' which is not in the Roslyn ID map.\n" +
				$"Roslyn IDs containing similar text:\n" +
				string.Join("\n", roslynIdMap.Keys
					.Where(k => k.Contains(entity.Name))
					.Take(10)));
			AssertIdentifiesEntity(entity, decompilerId);
		}

		/// <summary>
		/// Assert that the decompiler's GetIdString matches a specific expected ID,
		/// AND that Roslyn also produces that same ID (three-way consistency).
		/// </summary>
		private void AssertIdString(IEntity entity, string expectedId)
		{
			Assert.That(roslynIdMap.ContainsKey(expectedId), Is.True,
				$"Expected ID '{expectedId}' not found in Roslyn ID map - " +
				$"is the expected string correct?");
			string decompilerId = IdStringProvider.GetIdString(entity.ParentModule.MetadataFile, entity.MetadataToken);
			Assert.That(decompilerId, Is.EqualTo(expectedId), "Decompiler ID mismatch");
			AssertIdentifiesEntity(entity, decompilerId);
		}

		/// <summary>
		/// Assert that <paramref name="idString"/> names <paramref name="entity"/> and not some
		/// other member. The Roslyn ID map spans every referenced assembly, so mere membership
		/// in it also accepts the ID of an unrelated member that happens to exist - an ID naming
		/// the wrong overload, the wrong arity or a member of a different type would pass. Both
		/// directions are checked: the symbol Roslyn files under the ID has to be of the
		/// entity's kind, and resolving the ID back through FindEntity has to land on exactly
		/// this entity's metadata token - or, where the format cannot tell two members apart,
		/// on a member that carries the very same ID.
		/// </summary>
		private void AssertIdentifiesEntity(IEntity entity, string idString)
		{
			var module = entity.ParentModule.MetadataFile;
			var roslynSymbol = roslynIdMap[idString];
			Assert.That(roslynSymbol.Kind, Is.EqualTo(ExpectedRoslynKind(entity.SymbolKind)),
				$"ID '{idString}' is filed by Roslyn under a {roslynSymbol.Kind} " +
				$"('{roslynSymbol.ToDisplayString()}'), but it was generated for the " +
				$"{entity.SymbolKind} '{entity.FullName}'.");

			var (resolvedModule, resolvedHandle) = IdStringProvider.FindEntity(idString, new[] { module });
			Assert.That(resolvedHandle.IsNil, Is.False,
				$"ID '{idString}' generated for '{entity.FullName}' does not resolve back to any member.");
			Assert.That(resolvedModule, Is.SameAs(module));
			if (resolvedHandle.Equals(entity.MetadataToken))
				return;

			// Some IDs cannot name a single member because the format cannot express the
			// signature: Roslyn renders a function-pointer parameter as nothing at all, so
			// TakesFnPtr(delegate*<int, string>) and TakesFnPtr(delegate*<int, int>) both come
			// out as 'M:C.TakesFnPtr()'. Resolution can then only return the first member
			// carrying the key, so require that the member it returned really does carry it;
			// a resolution that picked a member with a different ID is still a failure.
			string resolvedId = IdStringProvider.GetIdString(module, resolvedHandle);
			Assert.That(resolvedId, Is.EqualTo(idString),
				$"ID '{idString}' was generated for '{entity.FullName}' " +
				$"(token {MetadataTokens.GetToken(entity.MetadataToken):X8}) but names " +
				$"'{DescribeHandle(module, resolvedHandle)}' " +
				$"(token {MetadataTokens.GetToken(resolvedHandle):X8}), whose own ID is " +
				$"'{resolvedId}'.");
		}

		/// <summary>The Roslyn symbol kind an entity of the given kind must be filed under.</summary>
		private static Microsoft.CodeAnalysis.SymbolKind ExpectedRoslynKind(DecompilerSymbolKind kind) => kind switch {
			DecompilerSymbolKind.TypeDefinition => Microsoft.CodeAnalysis.SymbolKind.NamedType,
			DecompilerSymbolKind.Field => Microsoft.CodeAnalysis.SymbolKind.Field,
			DecompilerSymbolKind.Property or DecompilerSymbolKind.Indexer => Microsoft.CodeAnalysis.SymbolKind.Property,
			DecompilerSymbolKind.Event => Microsoft.CodeAnalysis.SymbolKind.Event,
			_ => Microsoft.CodeAnalysis.SymbolKind.Method,
		};

		/// <summary>Names the member a handle points at, for assertion messages.</summary>
		private static string DescribeHandle(MetadataFile module, EntityHandle handle)
		{
			var metadata = module.Metadata;
			return handle.Kind switch {
				HandleKind.TypeDefinition => metadata.GetString(metadata.GetTypeDefinition((TypeDefinitionHandle)handle).Name),
				HandleKind.MethodDefinition => metadata.GetString(metadata.GetMethodDefinition((MethodDefinitionHandle)handle).Name),
				HandleKind.FieldDefinition => metadata.GetString(metadata.GetFieldDefinition((FieldDefinitionHandle)handle).Name),
				HandleKind.PropertyDefinition => metadata.GetString(metadata.GetPropertyDefinition((PropertyDefinitionHandle)handle).Name),
				HandleKind.EventDefinition => metadata.GetString(metadata.GetEventDefinition((EventDefinitionHandle)handle).Name),
				_ => handle.Kind.ToString(),
			};
		}

		#region Types

		[Test]
		public void Type_TopLevelEnum()
		{
			AssertIdString(FindType("Color"), "T:Color");
		}

		[Test]
		public void Type_Interface()
		{
			AssertIdString(FindType("Acme.IProcess"), "T:Acme.IProcess");
		}

		[Test]
		public void Type_Struct()
		{
			AssertIdString(FindType("Acme.ValueType"), "T:Acme.ValueType");
		}

		[Test]
		public void Type_Class()
		{
			AssertIdString(FindType("Acme.Widget"), "T:Acme.Widget");
		}

		[Test]
		public void Type_NestedClass()
		{
			AssertIdString(FindType("Acme.Widget+NestedClass"), "T:Acme.Widget.NestedClass");
		}

		[Test]
		public void Type_NestedInterface()
		{
			AssertIdString(FindType("Acme.Widget+IMenuItem"), "T:Acme.Widget.IMenuItem");
		}

		[Test]
		public void Type_NestedDelegate()
		{
			AssertIdString(FindType("Acme.Widget+Del"), "T:Acme.Widget.Del");
		}

		[Test]
		public void Type_NestedEnum()
		{
			AssertIdString(FindType("Acme.Widget+Direction"), "T:Acme.Widget.Direction");
		}

		[Test]
		public void Type_GenericClass()
		{
			AssertIdString(FindType("Acme.MyList`1"), "T:Acme.MyList`1");
		}

		[Test]
		public void Type_NestedGenericClass()
		{
			AssertIdString(FindType("Acme.MyList`1+Helper`2"), "T:Acme.MyList`1.Helper`2");
		}

		#endregion

		#region Fields

		[Test]
		public void Field_StructPrivate()
		{
			AssertIdString(FindField("Acme.ValueType", "total"), "F:Acme.ValueType.total");
		}

		[Test]
		public void Field_NestedClass()
		{
			AssertIdString(FindField("Acme.Widget+NestedClass", "value"),
				"F:Acme.Widget.NestedClass.value");
		}

		[Test]
		public void Field_String()
		{
			AssertIdString(FindField("Acme.Widget", "message"), "F:Acme.Widget.message");
		}

		[Test]
		public void Field_Static()
		{
			AssertIdString(FindField("Acme.Widget", "defaultColor"), "F:Acme.Widget.defaultColor");
		}

		[Test]
		public void Field_Const()
		{
			AssertIdString(FindField("Acme.Widget", "PI"), "F:Acme.Widget.PI");
		}

		[Test]
		public void Field_Readonly()
		{
			AssertIdString(FindField("Acme.Widget", "monthlyAverage"),
				"F:Acme.Widget.monthlyAverage");
		}

		[Test]
		public void Field_Array()
		{
			AssertIdString(FindField("Acme.Widget", "array1"), "F:Acme.Widget.array1");
		}

		[Test]
		public void Field_MultiDimArray()
		{
			AssertIdString(FindField("Acme.Widget", "array2"), "F:Acme.Widget.array2");
		}

		[Test]
		public void Field_Pointer()
		{
			AssertIdString(FindField("Acme.Widget", "pCount"), "F:Acme.Widget.pCount");
		}

		[Test]
		public void Field_NativeInt()
		{
			AssertIdString(FindField("Acme.Widget", "nativeInt"), "F:Acme.Widget.nativeInt");
		}

		[Test]
		public void Field_PointerToPointer()
		{
			AssertIdString(FindField("Acme.Widget", "ppValues"), "F:Acme.Widget.ppValues");
		}

		#endregion

		#region Constructors

		[Test]
		public void Ctor_Static()
		{
			AssertIdString(FindMethod("Acme.Widget", ".cctor", paramCount: 0),
				"M:Acme.Widget.#cctor");
		}

		[Test]
		public void Ctor_Default()
		{
			AssertIdString(FindMethod("Acme.Widget", ".ctor", paramCount: 0),
				"M:Acme.Widget.#ctor");
		}

		[Test]
		public void Ctor_Parameterized()
		{
			AssertIdString(FindMethod("Acme.Widget", ".ctor", paramCount: 1),
				"M:Acme.Widget.#ctor(System.String)");
		}

		#endregion

		#region Finalizer

		[Test]
		public void Finalizer()
		{
			AssertIdString(FindMethod("Acme.Widget", "Finalize", paramCount: 0),
				"M:Acme.Widget.Finalize");
		}

		#endregion

		#region Methods

		[Test]
		public void Method_StructMethod()
		{
			AssertIdString(FindMethod("Acme.ValueType", "M"),
				"M:Acme.ValueType.M(System.Int32)");
		}

		[Test]
		public void Method_NestedClass()
		{
			AssertIdString(FindMethod("Acme.Widget+NestedClass", "M"),
				"M:Acme.Widget.NestedClass.M(System.Int32)");
		}

		[Test]
		public void Method_NoParams()
		{
			AssertIdString(FindMethod("Acme.Widget", "M0"), "M:Acme.Widget.M0");
		}

		[Test]
		public void Method_OutRefIn()
		{
			AssertIdString(FindMethod("Acme.Widget", "M1"),
				"M:Acme.Widget.M1(System.Char,System.Single@,Acme.ValueType@,System.Int32@)");
		}

		[Test]
		public void Method_ArrayParams()
		{
			AssertIdString(FindMethod("Acme.Widget", "M2"),
				"M:Acme.Widget.M2(System.Int16[],System.Int32[0:,0:],System.Int64[][])");
		}

		[Test]
		public void Method_JaggedMultiDim()
		{
			AssertIdString(FindMethod("Acme.Widget", "M3"),
				"M:Acme.Widget.M3(System.Int64[][],Acme.Widget[0:,0:,0:][])");
		}

		[Test]
		public void Method_Pointers()
		{
			AssertIdString(FindMethod("Acme.Widget", "M4"),
				"M:Acme.Widget.M4(System.Char*,Color**)");
		}

		[Test]
		public void Method_VoidPointerAndPointerArray()
		{
			AssertIdString(FindMethod("Acme.Widget", "M5"),
				"M:Acme.Widget.M5(System.Void*,System.Double*[0:,0:][])");
		}

		[Test]
		public void Method_ParamsArray()
		{
			AssertIdString(FindMethod("Acme.Widget", "M6"),
				"M:Acme.Widget.M6(System.Int32,System.Object[])");
		}

		[Test]
		public void Method_NintNuint()
		{
			AssertIdString(FindMethod("Acme.Widget", "M7"),
				"M:Acme.Widget.M7(System.IntPtr,System.UIntPtr)");
		}

		[Test]
		public void Method_GenericClassParam()
		{
			AssertIdString(FindMethod("Acme.MyList`1", "Test"),
				"M:Acme.MyList`1.Test(`0)");
		}

		[Test]
		public void Method_ConcreteGenericArg()
		{
			AssertIdString(FindMethod("Acme.UseList", "Process"),
				"M:Acme.UseList.Process(Acme.MyList{System.Int32})");
		}

		[Test]
		public void Method_GenericMethod()
		{
			AssertIdString(FindMethod("Acme.UseList", "GetValues", typeParamCount: 1),
				"M:Acme.UseList.GetValues``1(``0)");
		}

		#endregion

		#region Properties

		[Test]
		public void Property_Simple()
		{
			AssertIdString(FindProperty("Acme.Widget", "Width"), "P:Acme.Widget.Width");
		}

		[Test]
		public void Property_IndexerOneParam()
		{
			AssertIdString(FindProperty("Acme.Widget", "Item", paramCount: 1),
				"P:Acme.Widget.Item(System.Int32)");
		}

		[Test]
		public void Property_IndexerTwoParams()
		{
			AssertIdString(FindProperty("Acme.Widget", "Item", paramCount: 2),
				"P:Acme.Widget.Item(System.String,System.Int32)");
		}

		#endregion

		#region Events

		[Test]
		public void Event_Simple()
		{
			AssertIdString(FindEvent("Acme.Widget", "AnEvent"), "E:Acme.Widget.AnEvent");
		}

		#endregion

		#region Operators

		[Test]
		public void Operator_Unary()
		{
			AssertIdString(FindMethod("Acme.Widget", "op_UnaryPlus"),
				"M:Acme.Widget.op_UnaryPlus(Acme.Widget)");
		}

		[Test]
		public void Operator_Binary()
		{
			AssertIdString(FindMethod("Acme.Widget", "op_Addition"),
				"M:Acme.Widget.op_Addition(Acme.Widget,Acme.Widget)");
		}

		[Test]
		public void Operator_ExplicitConversion()
		{
			AssertIdString(FindMethod("Acme.Widget", "op_Explicit"),
				"M:Acme.Widget.op_Explicit(Acme.Widget)~System.Int32");
		}

		[Test]
		public void Operator_ImplicitConversion()
		{
			AssertIdString(FindMethod("Acme.Widget", "op_Implicit"),
				"M:Acme.Widget.op_Implicit(Acme.Widget)~System.Int64");
		}

		#endregion

		#region Graphics.Point

		[Test]
		public void Point_Type()
		{
			AssertIdString(FindType("Graphics.Point"), "T:Graphics.Point");
		}

		[Test]
		public void Point_PropertyX()
		{
			AssertIdString(FindProperty("Graphics.Point", "X"), "P:Graphics.Point.X");
		}

		[Test]
		public void Point_PropertyY()
		{
			AssertIdString(FindProperty("Graphics.Point", "Y"), "P:Graphics.Point.Y");
		}

		[Test]
		public void Point_DefaultCtor()
		{
			AssertIdString(FindMethod("Graphics.Point", ".ctor", paramCount: 0),
				"M:Graphics.Point.#ctor");
		}

		[Test]
		public void Point_ParameterizedCtor()
		{
			AssertIdString(FindMethod("Graphics.Point", ".ctor", paramCount: 2),
				"M:Graphics.Point.#ctor(System.Int32,System.Int32)");
		}

		[Test]
		public void Point_Move()
		{
			AssertIdString(FindMethod("Graphics.Point", "Move"),
				"M:Graphics.Point.Move(System.Int32,System.Int32)");
		}

		[Test]
		public void Point_Translate()
		{
			AssertIdString(FindMethod("Graphics.Point", "Translate"),
				"M:Graphics.Point.Translate(System.Int32,System.Int32)");
		}

		[Test]
		public void Point_Equals()
		{
			AssertIdString(FindMethod("Graphics.Point", "Equals"),
				"M:Graphics.Point.Equals(System.Object)");
		}

		[Test]
		public void Point_GetHashCode()
		{
			AssertIdString(FindMethod("Graphics.Point", "GetHashCode", paramCount: 0),
				"M:Graphics.Point.GetHashCode");
		}

		[Test]
		public void Point_ToString()
		{
			AssertIdString(FindMethod("Graphics.Point", "ToString", paramCount: 0),
				"M:Graphics.Point.ToString");
		}

		[Test]
		public void Point_EqualityOp()
		{
			AssertIdString(FindMethod("Graphics.Point", "op_Equality"),
				"M:Graphics.Point.op_Equality(Graphics.Point,Graphics.Point)");
		}

		[Test]
		public void Point_InequalityOp()
		{
			AssertIdString(FindMethod("Graphics.Point", "op_Inequality"),
				"M:Graphics.Point.op_Inequality(Graphics.Point,Graphics.Point)");
		}

		#endregion

		#region Explicit Interface Implementations

		[Test]
		public void ExplicitImpl_Method()
		{
			var type = FindType("ExplicitImpl.Impl");
			var method = type.Methods.FirstOrDefault(m =>
				m.IsExplicitInterfaceImplementation &&
				m.ExplicitlyImplementedInterfaceMembers.Any(em => em.Name == "Bar"));
			Assert.That(method, Is.Not.Null, "Explicit impl of IFoo.Bar not found");
			AssertMatchesRoslyn(method);
		}

		[Test]
		public void ExplicitImpl_Property()
		{
			var type = FindType("ExplicitImpl.Impl");
			var prop = type.Properties.FirstOrDefault(p =>
				p.IsExplicitInterfaceImplementation &&
				p.ExplicitlyImplementedInterfaceMembers.Any(em => em.Name == "Baz"));
			Assert.That(prop, Is.Not.Null, "Explicit impl of IFoo.Baz not found");
			AssertMatchesRoslyn(prop);
		}

		[Test]
		public void ExplicitImpl_GenericInterface()
		{
			var type = FindType("ExplicitImpl.Impl");
			var method = type.Methods.FirstOrDefault(m =>
				m.IsExplicitInterfaceImplementation &&
				m.ExplicitlyImplementedInterfaceMembers.Any(em => em.Name == "Generic"));
			Assert.That(method, Is.Not.Null, "Explicit impl of IFoo<int>.Generic not found");
			AssertMatchesRoslyn(method);
		}

		#endregion

		#region FindEntity round-trip

		[TestCase("T:Color")]
		[TestCase("T:Acme.Widget")]
		[TestCase("T:Acme.Widget.NestedClass")]
		[TestCase("T:Acme.MyList`1")]
		[TestCase("T:Acme.MyList`1.Helper`2")]
		[TestCase("F:Acme.Widget.message")]
		[TestCase("F:Acme.Widget.PI")]
		[TestCase("M:Acme.Widget.#ctor")]
		[TestCase("M:Acme.Widget.#ctor(System.String)")]
		[TestCase("M:Acme.Widget.#cctor")]
		[TestCase("M:Acme.Widget.Finalize")]
		[TestCase("M:Acme.Widget.M0")]
		[TestCase("M:Acme.Widget.M1(System.Char,System.Single@,Acme.ValueType@,System.Int32@)")]
		[TestCase("M:Acme.Widget.M2(System.Int16[],System.Int32[0:,0:],System.Int64[][])")]
		[TestCase("M:Acme.Widget.M3(System.Int64[][],Acme.Widget[0:,0:,0:][])")]
		[TestCase("M:Acme.Widget.M6(System.Int32,System.Object[])")]
		[TestCase("M:Acme.MyList`1.Test(`0)")]
		[TestCase("M:Acme.UseList.Process(Acme.MyList{System.Int32})")]
		[TestCase("M:Acme.UseList.GetValues``1(``0)")]
		[TestCase("P:Acme.Widget.Width")]
		[TestCase("P:Acme.Widget.Item(System.Int32)")]
		[TestCase("P:Acme.Widget.Item(System.String,System.Int32)")]
		[TestCase("E:Acme.Widget.AnEvent")]
		[TestCase("M:Acme.Widget.op_UnaryPlus(Acme.Widget)")]
		[TestCase("M:Acme.Widget.op_Addition(Acme.Widget,Acme.Widget)")]
		[TestCase("M:Acme.Widget.op_Explicit(Acme.Widget)~System.Int32")]
		[TestCase("M:Acme.Widget.op_Implicit(Acme.Widget)~System.Int64")]
		[TestCase("M:NestedGenericInstantiations.Consumer.TakesInner(NestedGenericInstantiations.Outer{System.Int32}.Inner)")]
		[TestCase("M:NestedGenericInstantiations.Consumer.TakesInner2(NestedGenericInstantiations.Outer{System.Int32}.Inner2{System.String})")]
		[TestCase("M:CheckedOperators.Money.op_CheckedExplicit(CheckedOperators.Money)~System.Int32")]
		public void FindEntity_RoundTrip(string idString)
		{
			var (_, handle) = IdStringProvider.FindEntity(idString, new[] { decompilerTypeSystem.MainModule.MetadataFile });
			Assert.That(handle.IsNil, Is.False, $"FindEntity returned null for '{idString}'");
			Assert.That(IdStringProvider.GetIdString(decompilerTypeSystem.MainModule.MetadataFile, handle), Is.EqualTo(idString),
				"GetIdString on found entity does not match the input ID string");
		}

		[TestCase("T:Acme.MyList{")]
		[TestCase("T:Acme.MyList{System.Int32")]
		[TestCase("T:Acme.MyList{Acme.MyList{System.Int32}")]
		[TestCase("M:Acme.MyList{.Test")]
		public void FindEntity_UnbalancedBraces_Throws(string idString)
		{
			Assert.Throws<ReflectionNameParseException>(
				() => IdStringProvider.FindEntity(idString, new[] { decompilerTypeSystem.MainModule.MetadataFile }));
		}

		#endregion

		#region Exhaustive Roslyn cross-check

		[Test]
		public void AllTypes_MatchRoslyn()
		{
			foreach (var type in decompilerTypeSystem.MainModule.TypeDefinitions)
			{
				if (type.Name == "<Module>")
					continue;
				AssertMatchesRoslyn(type);
			}
		}

		[Test]
		public void AllMethods_MatchRoslyn()
		{
			foreach (var type in decompilerTypeSystem.MainModule.TypeDefinitions)
			{
				if (type.Name == "<Module>")
					continue;

				foreach (var method in type.Methods)
				{
					// Skip methods without a metadata token
					if (method.MetadataToken.IsNil)
						continue;
					AssertMatchesRoslyn(method);
				}
			}
		}

		[Test]
		public void FunctionPointerParameters_ShareOneIdString()
		{
			// Roslyn renders a function-pointer parameter as nothing at all, so overloads that
			// differ only in one collapse onto a single key with an empty parameter list. The
			// generator reproduces that instead of inventing a distinguishable key: this is the
			// key the C# compiler writes into the documentation file, so it is the one a lookup
			// has to produce and a cref has to resolve against.
			var overloads = FindType("FnPtrs.FnPtrParameters").Methods
				.Where(m => m.Name == "TakesFnPtr")
				.ToList();
			Assert.That(overloads, Has.Count.EqualTo(2),
				"the fixture declares two overloads differing only in their function-pointer parameter");
			foreach (var overload in overloads)
				AssertIdString(overload, "M:FnPtrs.FnPtrParameters.TakesFnPtr()");
		}

		[Test]
		public void AllFields_MatchRoslyn()
		{
			foreach (var type in decompilerTypeSystem.MainModule.TypeDefinitions)
			{
				if (type.Name == "<Module>")
					continue;
				foreach (var field in type.Fields)
				{
					if (field.DeclaringType.Kind == Decompiler.TypeSystem.TypeKind.Enum && field.Name == "value__")
						continue;
					if (field.IsCompilerGenerated())
						continue;
					AssertMatchesRoslyn(field);
				}
			}
		}

		[Test]
		public void AllProperties_MatchRoslyn()
		{
			foreach (var type in decompilerTypeSystem.MainModule.TypeDefinitions)
			{
				if (type.Name == "<Module>")
					continue;
				foreach (var prop in type.Properties)
					AssertMatchesRoslyn(prop);
			}
		}

		[Test]
		public void AllEvents_MatchRoslyn()
		{
			foreach (var type in decompilerTypeSystem.MainModule.TypeDefinitions)
			{
				if (type.Name == "<Module>")
					continue;
				foreach (var evt in type.Events)
					AssertMatchesRoslyn(evt);
			}
		}

		#endregion

		#region Tuples

		[Test]
		public void Tuple_Field()
		{
			// (int, string) becomes System.ValueTuple`2 in metadata
			AssertMatchesRoslyn(FindField("Tuples.TupleTests", "tupleField"));
		}

		[Test]
		public void Tuple_NamedField()
		{
			// Named tuples have identical metadata representation
			AssertMatchesRoslyn(FindField("Tuples.TupleTests", "namedTupleField"));
		}

		[Test]
		public void Tuple_ReturnType()
		{
			AssertMatchesRoslyn(FindMethod("Tuples.TupleTests", "GetTuple"));
		}

		[Test]
		public void Tuple_Parameter()
		{
			AssertMatchesRoslyn(FindMethod("Tuples.TupleTests", "TakesTuple"));
		}

		[Test]
		public void Tuple_Nested()
		{
			AssertMatchesRoslyn(FindMethod("Tuples.TupleTests", "NestedTuple"));
		}

		[Test]
		public void Tuple_InsideGeneric()
		{
			AssertMatchesRoslyn(FindMethod("Tuples.TupleTests", "TupleInGeneric"));
		}

		#endregion

		#region Nullable value types

		[Test]
		public void Nullable_Field()
		{
			AssertMatchesRoslyn(FindField("NullableTests.NullableValueTypes", "nullableField"));
		}

		[Test]
		public void Nullable_Parameters()
		{
			AssertMatchesRoslyn(FindMethod("NullableTests.NullableValueTypes", "TakesNullable"));
		}

		[Test]
		public void Nullable_Return()
		{
			AssertMatchesRoslyn(FindMethod("NullableTests.NullableValueTypes", "ReturnsNullable"));
		}

		[Test]
		public void Nullable_InsideGeneric()
		{
			AssertMatchesRoslyn(FindMethod("NullableTests.NullableValueTypes", "NullableInGeneric"));
		}

		#endregion

		#region Ref returns

		[Test]
		public void RefReturn_Method()
		{
			AssertMatchesRoslyn(FindMethod("RefReturns.RefReturnTests", "RefReturn"));
		}

		[Test]
		public void RefReadonlyReturn_Method()
		{
			AssertMatchesRoslyn(FindMethod("RefReturns.RefReturnTests", "RefReadonlyReturn"));
		}

		#endregion

		#region Dynamic

		[Test]
		public void Dynamic_Parameter()
		{
			// dynamic -> System.Object in ID strings
			var method = FindMethod("DynamicTests.DynamicMethods", "TakesDynamic");
			AssertIdString(method, "M:DynamicTests.DynamicMethods.TakesDynamic(System.Object)");
		}

		[Test]
		public void Dynamic_Return()
		{
			AssertMatchesRoslyn(FindMethod("DynamicTests.DynamicMethods", "ReturnsDynamic"));
		}

		#endregion

		#region Default interface methods and static abstract/virtual

		[Test]
		public void DefaultInterfaceMethod_Required()
		{
			AssertMatchesRoslyn(FindMethod("DefaultInterfaceMethods.IWithDefault", "Required"));
		}

		[Test]
		public void DefaultInterfaceMethod_WithDefault()
		{
			AssertMatchesRoslyn(FindMethod("DefaultInterfaceMethods.IWithDefault", "WithDefault"));
		}

		[Test]
		public void DefaultInterfaceMethod_Static()
		{
			AssertMatchesRoslyn(FindMethod("DefaultInterfaceMethods.IWithDefault", "StaticMethod"));
		}

		[Test]
		public void StaticAbstract_Create()
		{
			AssertMatchesRoslyn(FindMethod("DefaultInterfaceMethods.IStaticAbstract`1", "Create"));
		}

		[Test]
		public void StaticVirtual_CreateDefault()
		{
			AssertMatchesRoslyn(FindMethod("DefaultInterfaceMethods.IStaticAbstract`1", "CreateDefault"));
		}

		#endregion

		#region Records

		[Test]
		public void Record_Type()
		{
			AssertMatchesRoslyn(FindType("RecordTests.RecordClass"));
		}

		[Test]
		public void Record_PrimaryCtorParams_BecomeProperties()
		{
			AssertMatchesRoslyn(FindProperty("RecordTests.RecordClass", "X"));
			AssertMatchesRoslyn(FindProperty("RecordTests.RecordClass", "Y"));
		}

		[Test]
		public void Record_SynthesizedEquals()
		{
			AssertMatchesRoslyn(FindMethod("RecordTests.RecordClass", "Equals", paramCount: 1));
		}

		[Test]
		public void Record_SynthesizedGetHashCode()
		{
			AssertMatchesRoslyn(FindMethod("RecordTests.RecordClass", "GetHashCode", paramCount: 0));
		}

		[Test]
		public void Record_SynthesizedToString()
		{
			AssertMatchesRoslyn(FindMethod("RecordTests.RecordClass", "ToString", paramCount: 0));
		}

		[Test]
		public void Record_Deconstruct()
		{
			AssertMatchesRoslyn(FindMethod("RecordTests.RecordClass", "Deconstruct"));
		}

		[Test]
		public void Record_EqualityOp()
		{
			AssertMatchesRoslyn(FindMethod("RecordTests.RecordClass", "op_Equality"));
		}

		[Test]
		public void Record_InequalityOp()
		{
			AssertMatchesRoslyn(FindMethod("RecordTests.RecordClass", "op_Inequality"));
		}

		[Test]
		public void RecordStruct_Type()
		{
			AssertMatchesRoslyn(FindType("RecordTests.RecordStruct"));
		}

		[Test]
		public void RecordStruct_Properties()
		{
			AssertMatchesRoslyn(FindProperty("RecordTests.RecordStruct", "A"));
			AssertMatchesRoslyn(FindProperty("RecordTests.RecordStruct", "B"));
		}

		[Test]
		public void Record_CustomMembers()
		{
			AssertMatchesRoslyn(FindProperty("RecordTests.RecordWithCustom", "ComputedProp"));
			AssertMatchesRoslyn(FindMethod("RecordTests.RecordWithCustom", "CustomMethod"));
		}

		#endregion

		#region Deep nesting

		[Test]
		public void DeepNesting_FourLevels_Type()
		{
			AssertIdString(
				FindType("DeepNesting.Level1+Level2+Level3+Level4"),
				"T:DeepNesting.Level1.Level2.Level3.Level4");
		}

		[Test]
		public void DeepNesting_FourLevels_Method()
		{
			AssertIdString(
				FindMethod("DeepNesting.Level1+Level2+Level3+Level4", "DeepMethod"),
				"M:DeepNesting.Level1.Level2.Level3.Level4.DeepMethod(System.Int32)");
		}

		[Test]
		public void DeepNesting_FourLevels_Property()
		{
			AssertIdString(
				FindProperty("DeepNesting.Level1+Level2+Level3+Level4", "DeepProp"),
				"P:DeepNesting.Level1.Level2.Level3.Level4.DeepProp");
		}

		[Test]
		public void DeepNesting_ThreeLevelGeneric_Type()
		{
			AssertMatchesRoslyn(FindType("DeepNesting.GenericLevel1`1+GenericLevel2`1+GenericLevel3`1"));
		}

		[Test]
		public void DeepNesting_ThreeLevelGeneric_MixedMethod()
		{
			AssertMatchesRoslyn(
				FindMethod("DeepNesting.GenericLevel1`1+GenericLevel2`1+GenericLevel3`1", "MixedMethod"));
		}

		[Test]
		public void DeepNesting_ThreeLevelGeneric_ComplexReturn()
		{
			AssertMatchesRoslyn(
				FindMethod("DeepNesting.GenericLevel1`1+GenericLevel2`1+GenericLevel3`1", "ComplexReturn"));
		}

		#endregion

		#region Generic edge cases

		[Test]
		public void GenericOperator_Addition()
		{
			AssertMatchesRoslyn(FindMethod("GenericEdgeCases.GenericOperators`1", "op_Addition"));
		}

		[Test]
		public void GenericOperator_ExplicitConversion()
		{
			AssertMatchesRoslyn(FindMethod("GenericEdgeCases.GenericOperators`1", "op_Explicit"));
		}

		[Test]
		public void MixedGenerics_DictionaryReturn()
		{
			AssertMatchesRoslyn(FindMethod("GenericEdgeCases.MixedGenerics`1", "Mix", typeParamCount: 1));
		}

		[Test]
		public void MixedGenerics_NestedGenericParam()
		{
			AssertMatchesRoslyn(
				FindMethod("GenericEdgeCases.MixedGenerics`1", "NestedGenericParam", typeParamCount: 1));
		}

		[Test]
		public void MixedGenerics_ArrayOfT()
		{
			AssertMatchesRoslyn(FindMethod("GenericEdgeCases.MixedGenerics`1", "ArrayOfT"));
		}

		[Test]
		public void MixedGenerics_MultiDimOfT()
		{
			AssertMatchesRoslyn(FindMethod("GenericEdgeCases.MixedGenerics`1", "MultiDimOfT"));
		}

		[Test]
		public void ExplicitImpl_MultiGeneric()
		{
			var type = FindType("GenericEdgeCases.MultiGenericImpl");
			var method = type.Methods.FirstOrDefault(m =>
				m.IsExplicitInterfaceImplementation &&
				m.ExplicitlyImplementedInterfaceMembers.Any(em => em.Name == "Process"));
			Assert.That(method, Is.Not.Null, "Explicit impl of IMultiGeneric<int,string>.Process not found");
			AssertMatchesRoslyn(method);
		}

		[Test]
		public void SelfReferencingGeneric_Method()
		{
			AssertMatchesRoslyn(FindMethod("GenericEdgeCases.Comparable`1", "Compare"));
		}

		#endregion

		#region Array edge cases

		[Test]
		public void Array_MultiDimInsideGeneric()
		{
			AssertMatchesRoslyn(FindMethod("ArrayEdgeCases.ArrayMethods", "MultiDimInGeneric"));
		}

		[Test]
		public void Array_WeirdDimensions()
		{
			AssertMatchesRoslyn(FindMethod("ArrayEdgeCases.ArrayMethods", "WeirdArrays"));
		}

		[Test]
		public void Array_ParamsMultiDim()
		{
			AssertMatchesRoslyn(FindMethod("ArrayEdgeCases.ArrayMethods", "ParamsMultiDim"));
		}

		[Test]
		public void Array_JaggedGeneric()
		{
			AssertMatchesRoslyn(FindMethod("ArrayEdgeCases.ArrayMethods", "JaggedGenericArray"));
		}

		#endregion

		#region Init-only and required

		[Test]
		public void InitOnly_Property()
		{
			AssertMatchesRoslyn(FindProperty("InitOnlyAndRequired.InitOnlyProps", "InitProp"));
		}

		[Test]
		public void Required_Property()
		{
			AssertMatchesRoslyn(FindProperty("InitOnlyAndRequired.InitOnlyProps", "RequiredProp"));
		}

		[Test]
		public void Required_InitRecord()
		{
			AssertMatchesRoslyn(FindProperty("InitOnlyAndRequired.InitRecord", "Id"));
		}

		#endregion

		#region Ref struct and Span

		[Test]
		public void RefStruct_Type()
		{
			AssertMatchesRoslyn(FindType("RefStructTests.MyRefStruct"));
		}

		[Test]
		public void RefStruct_Field()
		{
			AssertMatchesRoslyn(FindField("RefStructTests.MyRefStruct", "Value"));
		}

		[Test]
		public void RefStruct_Method()
		{
			AssertMatchesRoslyn(FindMethod("RefStructTests.MyRefStruct", "DoSomething"));
		}

		[Test]
		public void Span_Parameter()
		{
			AssertMatchesRoslyn(FindMethod("RefStructTests.UsesRefStruct", "TakesSpan"));
		}

		[Test]
		public void ReadOnlySpan_Parameter()
		{
			AssertMatchesRoslyn(FindMethod("RefStructTests.UsesRefStruct", "TakesReadOnlySpan"));
		}

		#endregion

		#region Overload resolution

		[Test]
		public void Overload_IntParam()
		{
			AssertIdString(
				FindMethod("Overloads.OverloadResolution", "M", paramCount: 1, typeParamCount: 0),
				"M:Overloads.OverloadResolution.M(System.Int32)");
		}

		[Test]
		public void Overload_StringParam()
		{
			var type = FindType("Overloads.OverloadResolution");
			var method = type.Methods.First(m =>
				m.Name == "M" && m.Parameters.Count == 1 &&
				m.TypeParameters.Count == 0 &&
				m.Parameters[0].Type.FullName == "System.String");
			AssertIdString(method, "M:Overloads.OverloadResolution.M(System.String)");
		}

		[Test]
		public void Overload_TwoParams()
		{
			AssertIdString(
				FindMethod("Overloads.OverloadResolution", "M", paramCount: 2, typeParamCount: 0),
				"M:Overloads.OverloadResolution.M(System.Int32,System.String)");
		}

		[Test]
		public void Overload_OneTypeParam()
		{
			AssertMatchesRoslyn(
				FindMethod("Overloads.OverloadResolution", "M", paramCount: 1, typeParamCount: 1));
		}

		[Test]
		public void Overload_TwoTypeParams()
		{
			AssertMatchesRoslyn(
				FindMethod("Overloads.OverloadResolution", "M", paramCount: 2, typeParamCount: 2));
		}

		[Test]
		public void Overload_ByRef()
		{
			// ref int and out int both produce System.Int32@ - but they're different methods
			// The ID string includes the @, making ref/out/in look the same in the ID.
			// Each overload of ByRef that takes ref/out int should still be distinguishable
			// from the one that takes plain int.
			var type = FindType("Overloads.OverloadResolution");
			foreach (var method in type.Methods.Where(m => m.Name == "ByRef"))
			{
				AssertMatchesRoslyn(method);
			}
		}

		#endregion

		#region All operator names

		[Test]
		public void Operator_UnaryNegation()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_UnaryNegation"));
		}

		[Test]
		public void Operator_LogicalNot()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_LogicalNot"));
		}

		[Test]
		public void Operator_OnesComplement()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_OnesComplement"));
		}

		[Test]
		public void Operator_Increment()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_Increment"));
		}

		[Test]
		public void Operator_Decrement()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_Decrement"));
		}

		[Test]
		public void Operator_True()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_True"));
		}

		[Test]
		public void Operator_False()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_False"));
		}

		[Test]
		public void Operator_Subtraction()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_Subtraction"));
		}

		[Test]
		public void Operator_Multiply()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_Multiply"));
		}

		[Test]
		public void Operator_Division()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_Division"));
		}

		[Test]
		public void Operator_Modulus()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_Modulus"));
		}

		[Test]
		public void Operator_BitwiseAnd()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_BitwiseAnd"));
		}

		[Test]
		public void Operator_BitwiseOr()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_BitwiseOr"));
		}

		[Test]
		public void Operator_ExclusiveOr()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_ExclusiveOr"));
		}

		[Test]
		public void Operator_LeftShift()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_LeftShift"));
		}

		[Test]
		public void Operator_RightShift()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_RightShift"));
		}

		[Test]
		public void Operator_Equality()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_Equality"));
		}

		[Test]
		public void Operator_Inequality()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_Inequality"));
		}

		[Test]
		public void Operator_LessThan()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_LessThan"));
		}

		[Test]
		public void Operator_LessThanOrEqual()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_LessThanOrEqual"));
		}

		[Test]
		public void Operator_GreaterThan()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_GreaterThan"));
		}

		[Test]
		public void Operator_GreaterThanOrEqual()
		{
			AssertMatchesRoslyn(FindMethod("SpecialNames.Operators", "op_GreaterThanOrEqual"));
		}

		#endregion

		#region Scoped parameters

		[Test]
		public void Scoped_Span()
		{
			// scoped doesn't affect the ID string
			AssertMatchesRoslyn(FindMethod("ByRefLikeParams.ScopedTests", "TakesScopedSpan"));
		}

		[Test]
		public void Scoped_ReadOnlySpan()
		{
			AssertMatchesRoslyn(FindMethod("ByRefLikeParams.ScopedTests", "TakesScopedReadOnlySpan"));
		}

		#endregion

		#region Nested generic instantiations

		[Test]
		public void NestedGenericInstantiation_NonGenericInner()
		{
			// Generic arguments must be distributed to their nesting level:
			// Outer{System.Int32}.Inner, not Outer`1.Inner{System.Int32}.
			AssertIdString(
				FindMethod("NestedGenericInstantiations.Consumer", "TakesInner"),
				"M:NestedGenericInstantiations.Consumer.TakesInner(NestedGenericInstantiations.Outer{System.Int32}.Inner)");
		}

		[Test]
		public void NestedGenericInstantiation_GenericInner()
		{
			AssertIdString(
				FindMethod("NestedGenericInstantiations.Consumer", "TakesInner2"),
				"M:NestedGenericInstantiations.Consumer.TakesInner2(NestedGenericInstantiations.Outer{System.Int32}.Inner2{System.String})");
		}

		[Test]
		public void NestedGenericInstantiation_NestedArgs()
		{
			AssertMatchesRoslyn(FindMethod("NestedGenericInstantiations.Consumer", "TakesDeep"));
		}

		[Test]
		public void NestedGenericInstantiation_ThreeLevels()
		{
			AssertMatchesRoslyn(FindMethod("NestedGenericInstantiations.Consumer", "TakesThreeLevels"));
		}

		#endregion

		#region Checked operators

		[Test]
		public void Operator_CheckedExplicitConversion()
		{
			AssertIdString(
				FindMethod("CheckedOperators.Money", "op_CheckedExplicit"),
				"M:CheckedOperators.Money.op_CheckedExplicit(CheckedOperators.Money)~System.Int32");
		}

		#endregion

		#region Required modifiers vs Roslyn

		[Test]
		public void InParameter_OnInterfaceMethod()
		{
			// The parameter type is int32& modreq(InAttribute); Roslyn renders it as
			// System.Int32@, ignoring the modifier. Pins that required modifiers are
			// omitted from ID strings.
			AssertIdString(
				FindMethod("ModreqParams.IWithIn", "TakesIn"),
				"M:ModreqParams.IWithIn.TakesIn(System.Int32@)");
		}

		#endregion

		#region Hand-built metadata

		[Test]
		public void Array_ExplicitBoundsAndSizes()
		{
			// C# cannot express arrays with non-zero lower bounds or fixed sizes in a
			// signature, so this exercises the spec's "lowerbound:size" notation with a
			// hand-built module: M(int[1..5, 3..]) => System.Int32[1:5,3:]
			var pe = BuildAssemblyWithMethodSignature((metadata, parameter) => parameter.Type().Array(
				elementType => elementType.Int32(),
				shape => shape.Shape(rank: 2, sizes: [5], lowerBounds: [1, 3])));
			string idString = pe.GetIdString(MetadataTokens.MethodDefinitionHandle(1));
			Assert.That(idString, Is.EqualTo("M:Host.M(System.Int32[1:5,3:])"));
		}

		[Test]
		public void Modifier_Optional()
		{
			// The C#/Roslyn form ignores custom modifiers; the MSVC C++/CLI form renders
			// modopt as '!' + modifier following the modified type, as C++/CLI 'const int'
			// parameters show (see https://github.com/icsharpcode/ILSpy/issues/2728).
			var pe = BuildAssemblyWithMethodSignature((metadata, parameter) => {
				parameter.CustomModifiers().AddModifier(
					AddCompilerServicesTypeRef(metadata, "IsConst"), isOptional: true);
				parameter.Type().Int32();
			});
			Assert.That(pe.GetIdString(MetadataTokens.MethodDefinitionHandle(1)),
				Is.EqualTo("M:Host.M(System.Int32)"));
			Assert.That(pe.GetIdStringCandidates(MetadataTokens.MethodDefinitionHandle(1)),
				Is.EqualTo(new[] {
					"M:Host.M(System.Int32!System.Runtime.CompilerServices.IsConst)",
					"M:Host.M(System.Int32)",
				}));
		}

		[Test]
		public void Modifier_Required()
		{
			// The C#/Roslyn form ignores modreq (e.g. modreq(InAttribute) on virtual 'in'
			// parameters is documented as T@); the MSVC form renders modreq(IsVolatile)
			// with '|' as observed in MSVC-generated xml doc files.
			var pe = BuildAssemblyWithMethodSignature((metadata, parameter) => {
				parameter.CustomModifiers().AddModifier(
					AddCompilerServicesTypeRef(metadata, "IsVolatile"), isOptional: false);
				parameter.Type().Int32();
			});
			Assert.That(pe.GetIdString(MetadataTokens.MethodDefinitionHandle(1)),
				Is.EqualTo("M:Host.M(System.Int32)"));
			Assert.That(pe.GetIdStringCandidates(MetadataTokens.MethodDefinitionHandle(1)),
				Is.EqualTo(new[] {
					"M:Host.M(System.Int32|System.Runtime.CompilerServices.IsVolatile)",
					"M:Host.M(System.Int32)",
				}));
		}

		[Test]
		public void Modifier_OptionalUnderPointer()
		{
			// C++/CLI 'char*' emits int8 modopt(IsSignUnspecifiedByte)*, rendered by MSVC as
			// System.SByte!System.Runtime.CompilerServices.IsSignUnspecifiedByte*
			var pe = BuildAssemblyWithMethodSignature((metadata, parameter) => {
				var pointee = parameter.Type().Pointer();
				pointee.CustomModifiers().AddModifier(
					AddCompilerServicesTypeRef(metadata, "IsSignUnspecifiedByte"), isOptional: true);
				pointee.SByte();
			});
			Assert.That(pe.GetIdString(MetadataTokens.MethodDefinitionHandle(1)),
				Is.EqualTo("M:Host.M(System.SByte*)"));
			Assert.That(pe.GetIdStringCandidates(MetadataTokens.MethodDefinitionHandle(1)),
				Does.Contain("M:Host.M(System.SByte!System.Runtime.CompilerServices.IsSignUnspecifiedByte*)"));
		}

		[Test]
		public void Pinned_Suffix()
		{
			// ELEMENT_TYPE_PINNED is represented as '^' following the modified type per the
			// MSVC xml doc format. It cannot occur in a valid method signature (only in
			// local variable signatures), so it is written as a raw prefix byte here.
			var pe = BuildAssemblyWithMethodSignature((metadata, parameter) => {
				parameter.Builder.WriteByte(0x45); // ELEMENT_TYPE_PINNED
				parameter.Type().Int32();
			});
			string idString = pe.GetIdString(MetadataTokens.MethodDefinitionHandle(1));
			Assert.That(idString, Is.EqualTo("M:Host.M(System.Int32^)"));
		}

		[Test]
		public void FindEntity_ModifiedSignature()
		{
			var pe = BuildAssemblyWithMethodSignature((metadata, parameter) => {
				parameter.CustomModifiers().AddModifier(
					AddCompilerServicesTypeRef(metadata, "IsConst"), isOptional: true);
				parameter.Type().Int32();
			});
			var (module, handle) = IdStringProvider.FindEntity(
				"M:Host.M(System.Int32!System.Runtime.CompilerServices.IsConst)", new MetadataFile[] { pe });
			Assert.That(module, Is.SameAs(pe));
			Assert.That(handle, Is.EqualTo((EntityHandle)MetadataTokens.MethodDefinitionHandle(1)));
		}

		[Test]
		public void FindEntity_PrefersDialectConsistentMatch()
		{
			// Overloads that differ only in a custom modifier (C++/CLI 'char*' vs
			// 'signed char*'): the C#/Roslyn form of the modified overload collapses to
			// the unmodified overload's ID, so a naive first-match lookup would resolve
			// "M:Host.M(System.SByte*)" to the modified overload declared first. The
			// dialect-consistent two-pass match must pick the unmodified overload.
			var pe = BuildAssemblyWithMethods(
				(metadata, parameter) => {
					var pointee = parameter.Type().Pointer();
					pointee.CustomModifiers().AddModifier(
						AddCompilerServicesTypeRef(metadata, "IsSignUnspecifiedByte"), isOptional: true);
					pointee.SByte();
				},
				(metadata, parameter) => parameter.Type().Pointer().SByte());

			var (_, plainHandle) = IdStringProvider.FindEntity(
				"M:Host.M(System.SByte*)", new MetadataFile[] { pe });
			Assert.That(plainHandle, Is.EqualTo((EntityHandle)MetadataTokens.MethodDefinitionHandle(2)));

			var (_, modifiedHandle) = IdStringProvider.FindEntity(
				"M:Host.M(System.SByte!System.Runtime.CompilerServices.IsSignUnspecifiedByte*)", new MetadataFile[] { pe });
			Assert.That(modifiedHandle, Is.EqualTo((EntityHandle)MetadataTokens.MethodDefinitionHandle(1)));
		}

		[Test]
		public void DocumentationLookup_DoesNotFallBackToSiblingKey()
		{
			// The Roslyn form of the modified overload equals the only key of the
			// unmodified overload. Such an assembly cannot come from the C# compiler, so
			// its xml file uses the C++/CLI dialect, where that key documents the
			// unmodified overload: the modified overload's candidates must omit it, and
			// its documentation lookup must miss instead of showing the sibling's text.
			var pe = BuildAssemblyWithMethods(
				(metadata, parameter) => {
					var pointee = parameter.Type().Pointer();
					pointee.CustomModifiers().AddModifier(
						AddCompilerServicesTypeRef(metadata, "IsSignUnspecifiedByte"), isOptional: true);
					pointee.SByte();
				},
				(metadata, parameter) => parameter.Type().Pointer().SByte());

			Assert.That(pe.GetIdStringCandidates(MetadataTokens.MethodDefinitionHandle(1)),
				Is.EqualTo(new[] { "M:Host.M(System.SByte!System.Runtime.CompilerServices.IsSignUnspecifiedByte*)" }));
			Assert.That(pe.GetIdStringCandidates(MetadataTokens.MethodDefinitionHandle(2)),
				Is.EqualTo(new[] { "M:Host.M(System.SByte*)" }));

			string xmlPath = Path.Combine(Path.GetTempPath(),
				"IdStringSiblingGuard_" + Guid.NewGuid().ToString("N") + ".xml");
			File.WriteAllText(xmlPath, """
				<?xml version="1.0"?>
				<doc>
					<assembly><name>test</name></assembly>
					<members>
						<member name="M:Host.M(System.SByte*)">
							<summary>plain overload</summary>
						</member>
					</members>
				</doc>
				""");
			try
			{
				var provider = new XmlDocumentationProvider(xmlPath);
				var compilation = new SimpleCompilation(pe, MinimalCorlib.Instance);
				var host = compilation.MainModule.TopLevelTypeDefinitions.Single(t => t.Name == "Host");
				var modified = host.Methods.Single(
					m => m.MetadataToken == (EntityHandle)MetadataTokens.MethodDefinitionHandle(1));
				var plain = host.Methods.Single(
					m => m.MetadataToken == (EntityHandle)MetadataTokens.MethodDefinitionHandle(2));

				Assert.That(provider.GetDocumentation(plain), Does.Contain("plain overload"));
				Assert.That(provider.GetDocumentation(modified), Is.Null);
			}
			finally
			{
				File.Delete(xmlPath);
			}
		}

		#endregion

		#region MSVC C++/CLI dialect fixture

		// IdStringProbe.il is the trimmed disassembly of an MSVC-compiled C++/CLI assembly
		// and IdStringProbe.xml the unmodified xml doc file MSVC generated for it; every
		// member key MSVC wrote must be reachable through the ID string candidates.

		static async Task<PEFile> AssembleIdStringProbe()
		{
			string dir = Path.Combine(Tester.TesterPath, "../../../../Documentation");
			string dll = await Tester.AssembleIL(Path.Combine(dir, "IdStringProbe.il"), AssemblerOptions.Library);
			return new PEFile(dll);
		}

		static HashSet<string> CollectIdStringCandidates(PEFile pe)
		{
			var md = pe.Metadata;
			var candidates = new HashSet<string>();
			foreach (var th in md.TypeDefinitions)
			{
				var td = md.GetTypeDefinition(th);
				if (md.GetString(td.Name) == "<Module>")
					continue;
				candidates.UnionWith(pe.GetIdStringCandidates(th));
				foreach (var h in td.GetMethods())
					candidates.UnionWith(pe.GetIdStringCandidates(h));
				foreach (var h in td.GetProperties())
					candidates.UnionWith(pe.GetIdStringCandidates(h));
				foreach (var h in td.GetEvents())
					candidates.UnionWith(pe.GetIdStringCandidates(h));
				foreach (var h in td.GetFields())
					candidates.UnionWith(pe.GetIdStringCandidates(h));
			}
			return candidates;
		}

		[Test]
		public async Task MsvcCppCliXml_AllMemberIdsCovered()
		{
			var pe = await AssembleIdStringProbe();
			var candidates = CollectIdStringCandidates(pe);
			string xmlPath = Path.Combine(Tester.TesterPath, "../../../../Documentation/IdStringProbe.xml");
			Assert.Multiple(() => {
				foreach (var member in System.Xml.Linq.XDocument.Load(xmlPath).Descendants("member"))
				{
					string name = member.Attribute("name").Value;
					Assert.That(candidates, Does.Contain(name), name);
				}
			});
		}

		[Test]
		public async Task MsvcCppCliXml_FindEntityResolvesDefaultIndexer()
		{
			// The 'default' key does not equal the property's metadata name (Item), so the
			// member-name narrowing must fall back to the unfiltered scan.
			var pe = await AssembleIdStringProbe();
			var (module, handle) = IdStringProvider.FindEntity(
				"P:IdProbe.default(System.Int32!System.Runtime.CompilerServices.IsLong)",
				new MetadataFile[] { pe });
			Assert.That(module, Is.SameAs((MetadataFile)pe));
			Assert.That(handle.Kind, Is.EqualTo(HandleKind.PropertyDefinition));
			var propertyName = pe.Metadata.GetPropertyDefinition((PropertyDefinitionHandle)handle).Name;
			Assert.That(pe.Metadata.GetString(propertyName), Is.EqualTo("Item"));
		}

		[Test]
		public async Task MsvcCppCliXml_DocumentationLookup()
		{
			var pe = await AssembleIdStringProbe();
			string xmlPath = Path.Combine(Tester.TesterPath, "../../../../Documentation/IdStringProbe.xml");
			var provider = new XmlDocumentationProvider(xmlPath);
			var compilation = new SimpleCompilation(pe, MinimalCorlib.Instance);
			var probeType = compilation.MainModule.TopLevelTypeDefinitions.Single(t => t.Name == "IdProbe");

			var volatileMethod = probeType.Methods.Single(m => m.Name == "TakesVolatilePtr");
			Assert.That(provider.GetDocumentation(volatileMethod), Does.Contain("volatile int pointer parameter"));

			var indexer = probeType.Properties.Single(p => p.Name == "Item");
			Assert.That(provider.GetDocumentation(indexer), Does.Contain("indexed property with a long parameter"));

			var enumeratorMethod = probeType.Methods.Single(m => m.Name == "TakesEnumerator");
			Assert.That(provider.GetDocumentation(enumeratorMethod), Does.Contain("nested type of a generic instantiation"));
		}

		#endregion

		#region Hand-built metadata helpers

		static TypeReferenceHandle AddCompilerServicesTypeRef(MetadataBuilder metadata, string name)
		{
			var mscorlib = metadata.AddAssemblyReference(metadata.GetOrAddString("mscorlib"),
				new Version(4, 0, 0, 0), default, default, 0, default);
			return metadata.AddTypeReference(mscorlib,
				metadata.GetOrAddString("System.Runtime.CompilerServices"), metadata.GetOrAddString(name));
		}

		static PEFile BuildAssemblyWithMethodSignature(Action<MetadataBuilder, ParameterTypeEncoder> encodeParameter)
		{
			return BuildAssemblyWithMethods(encodeParameter);
		}

		/// <summary>
		/// Builds a minimal in-memory assembly containing a single type "Host" with one
		/// static method "M" per element of <paramref name="encodeParameters"/>, each
		/// taking one parameter whose type is produced by that element.
		/// </summary>
		static PEFile BuildAssemblyWithMethods(params Action<MetadataBuilder, ParameterTypeEncoder>[] encodeParameters)
		{
			var metadata = new MetadataBuilder();
			metadata.AddModule(0, metadata.GetOrAddString("test.dll"),
				metadata.GetOrAddGuid(Guid.NewGuid()), default, default);
			metadata.AddAssembly(metadata.GetOrAddString("test"), new Version(1, 0, 0, 0),
				default, default, 0, AssemblyHashAlgorithm.None);

			MethodDefinitionHandle firstMethod = default;
			foreach (var encodeParameter in encodeParameters)
			{
				var signature = new BlobBuilder();
				new BlobEncoder(signature).MethodSignature().Parameters(1,
					returnType => returnType.Void(),
					parameters => encodeParameter(metadata, parameters.AddParameter()));
				var method = metadata.AddMethodDefinition(
					MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.Abstract,
					MethodImplAttributes.IL, metadata.GetOrAddString("M"),
					metadata.GetOrAddBlob(signature), -1, parameterList: MetadataTokens.ParameterHandle(1));
				if (firstMethod.IsNil)
					firstMethod = method;
			}

			metadata.AddTypeDefinition(default, default,
				metadata.GetOrAddString("<Module>"), baseType: default,
				fieldList: MetadataTokens.FieldDefinitionHandle(1), methodList: firstMethod);
			metadata.AddTypeDefinition(
				TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Sealed,
				default, metadata.GetOrAddString("Host"), baseType: default,
				fieldList: MetadataTokens.FieldDefinitionHandle(1), methodList: firstMethod);

			var peBlob = new BlobBuilder();
			new ManagedPEBuilder(PEHeaderBuilder.CreateLibraryHeader(),
				new MetadataRootBuilder(metadata), ilStream: new BlobBuilder()).Serialize(peBlob);
			return new PEFile("test.dll", new MemoryStream(peBlob.ToArray()));
		}

		#endregion

	}
}