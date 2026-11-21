// Copyright (c) 2026 Christoph Wille
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

static class BodyGapExtensions
{
	public static int ExtensionLength(this string? s) => s?.Length ?? 0;

	public static void Add(this BodyGapBag bag, int value) => bag.Items.Add(value);

	public static int TupleExtension(this (int, string) t) => t.Item1;
}

struct BodyGapLiftable
{
	public static bool operator ==(BodyGapLiftable a, BodyGapLiftable b) => true;

	public static bool operator !=(BodyGapLiftable a, BodyGapLiftable b) => false;

	public override bool Equals(object? obj) => false;

	public override int GetHashCode() => 0;
}

class BodyGapBag : System.Collections.IEnumerable
{
	public readonly System.Collections.Generic.List<int> Items = new();

	public System.Collections.IEnumerator GetEnumerator() => Items.GetEnumerator();
}

struct BodyGapValue
{
	public override string ToString() => "value";
}

class BodyGapBase
{
	public int Value() => 1;
}

class BodyGapDerived : BodyGapBase
{
	public new int Value() => 2;
}

record BodyGapRecord(int X, string Y);

struct BodyGapGenericStruct<T>
{
	public T Field;

	public BodyGapGenericStruct(T value) => Field = value;
}

class BodyGapDefaults
{
	public virtual int this[int x, int y = 10] => x + y;

	public virtual int Method(int x, int y = 10) => x + y;
}

class BodyGapRedeclaredDefaults : BodyGapDefaults
{
	public override int this[int x, int y = 20] => x + y + 1;

	public override int Method(int x, int y = 20) => x + y + 1;
}

class BodyGapVirtualBase
{
	public virtual void M()
	{
	}
}

class BodyGapHidingDerived : BodyGapVirtualBase
{
	public new void M()
	{
	}

	public Action BaseVirtual() => ((BodyGapVirtualBase)this).M;
}

class BodyGapNamedIndexer
{
	public int this[int x, int y] {
		get => x;
		set { }
	}
}

/// <summary>
/// Bodies of the calls, casts, arguments and object creations the engine translates itself:
/// each decompiles to the same C# as the managed statement builder writes, without the
/// engine falling back.
/// </summary>
[TestFixture, Parallelizable(ParallelScope.All)]
public class BodyGapTests
{
	static object AnonymousInferred(int a, string b) => new { a, b };

	static object AnonymousNamed(int a, string b) => new { X = a, Y = b + "!" };

	static int ExtensionOnNull() => ((string?)null).ExtensionLength();

	static char DecimalToChar(object o) => (char)(decimal)o;

	static Func<int> HiddenMethodReference(BodyGapDerived d) => ((BodyGapBase)d).Value;

	static object WithInitializer(BodyGapRecord r) => r with { X = 2 };

	static string FormatOfArrayLiteral(int x) => string.Format("value {0}", new object[] { x });

	static void TakesDynamic(dynamic d) { }

	static object DynamicMember(dynamic d) => d.Name;

	static void DynamicSetIndexAndMember(dynamic d, int i)
	{
		d[i, "k"] = d.Value + 1;
		d.Other = -d;
	}

	static int DynamicConvert(dynamic d) => (int)d;

	static string StaticConversionFromDynamic(dynamic d) => (string)(object)d;

	static void ArgumentOfDynamicParameter() => TakesDynamic(1);

	static void DynamicCompound(dynamic d)
	{
		d.X += 1;
		d *= 2;
	}

	static object DynamicInvocations(dynamic d)
	{
		Console.WriteLine(d);
		d.Generic<int>(1, name: "n");
		return new BodyGapRecord(d, "x");
	}

	static bool DynamicConditions(dynamic d)
	{
		if (d)
			return !d;
		return d && d.Flag ? true : false;
	}

	static object QueryWithLet(int[] xs) => from x in xs let y = x * 2 where y > 1 select x + y;

	static int RedeclaredDefaults(BodyGapRedeclaredDefaults d) => d[1, 10] + d.Method(1, 10);

	static void NamedIndexerSetter(BodyGapNamedIndexer o, Func<int, int> f)
	{
		o[y: f(1), x: f(2)] = f(3);
	}

	static object DictionaryInitializer(string key) => new System.Collections.Generic.Dictionary<string, int> { [key] = 1, ["b"] = 2 };

	static object NestedDictionaryInitializer() => new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<int>> { [1] = { 2, 3 } };

	static object ExtensionCollectionInitializer() => new BodyGapBag { 1, 2 };

	static string BoxedValueToString(BodyGapValue v) => ((object)v).ToString()!;

	static bool SwitchOfTuples(int i) => (i switch { 0 => (0, true), 1 => (1, true), _ => (i, false) }).Item2;

	static Func<string?, int> UnboundExtensionMethod() => BodyGapExtensions.ExtensionLength;

	static void Identify<T>(T value) { }

	static T MakeDefault<T>() => default!;

	static void NullOfAnonymousType() => Identify(true ? null : new { X = 0, Inner = new { S = "" } });

	static object TupleTypeArgument() => MakeDefault<(int, string)>();

	static void DynamicForeach(dynamic d)
	{
		foreach (var x in d)
			Console.WriteLine(x);
	}

	static void NonGenericTupleForeach(System.Collections.IEnumerable input)
	{
		foreach ((string, int) item in input)
			Console.WriteLine(item.Item1);
	}

	static object ValueTupleOfOne(ValueTuple<int> t) => new ValueTuple<int>(t.Item1);

	static object LongTuple() => (1, 2, 3, 4, 5, 6, 7, 8, 9).Item9;

	static int GenericFieldElementNames(BodyGapGenericStruct<(int A, int B)> s) => s.Field.A;

	static int FirstOfNamedTuples(System.Collections.Generic.List<(string C, int D)> l) => l.First().D;

	static System.Collections.Generic.Dictionary<(int, string), (string C, int D)> TupleDict { get; } = new();

	static string IndexerOfNamedTuples() => TupleDict[(1, "abc")].C;

	static int ValuesOfNamedTuples() => TupleDict.Values.ToList().First().D;

	static (int, int) Pair() => (1, 2);

	static System.Threading.Tasks.Task VoidLambdaOfTaskRun()
	{
		int a = 0;
		return System.Threading.Tasks.Task.Run(() => {
			a = Pair().Item1;
		});
	}

	static System.Threading.Tasks.Task DeconstructionInVoidLambda()
	{
		int a = 0;
		int b = 0;
		return System.Threading.Tasks.Task.Run(() => {
			(a, b) = Pair();
		});
	}

	[TestCase(nameof(ValuesOfNamedTuples))]
	[TestCase(nameof(VoidLambdaOfTaskRun))]
	[TestCase(nameof(DeconstructionInVoidLambda))]
	[TestCase(nameof(NullOfAnonymousType))]
	[TestCase(nameof(TupleTypeArgument))]
	[TestCase(nameof(DynamicForeach))]
	[TestCase(nameof(NonGenericTupleForeach))]
	[TestCase(nameof(ValueTupleOfOne))]
	[TestCase(nameof(LongTuple))]
	[TestCase(nameof(GenericFieldElementNames))]
	[TestCase(nameof(FirstOfNamedTuples))]
	[TestCase(nameof(IndexerOfNamedTuples))]
	public void EngineServesTupleAndAnonymousBody(string name)
	{
		AssertEngineServes(typeof(BodyGapTests).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!);
	}

	static Func<int> BoundExtensionMethod(string s) => s.ExtensionLength;

	static Func<int> BoundExtensionMethodOnNull() => ((string?)null).ExtensionLength;

	static int ExtensionOnTuple(int i) => (i, "x").TupleExtension();

	static bool LiftedUserDefinedOperator(BodyGapLiftable? a, BodyGapLiftable? b) => a == b;

	[TestCase(nameof(BoundExtensionMethod))]
	[TestCase(nameof(BoundExtensionMethodOnNull))]
	[TestCase(nameof(ExtensionOnTuple))]
	[TestCase(nameof(LiftedUserDefinedOperator))]
	[TestCase(nameof(SwitchOfTuples))]
	[TestCase(nameof(UnboundExtensionMethod))]
	[TestCase(nameof(DictionaryInitializer))]
	[TestCase(nameof(NestedDictionaryInitializer))]
	[TestCase(nameof(ExtensionCollectionInitializer))]
	[TestCase(nameof(BoxedValueToString))]
	[TestCase(nameof(QueryWithLet))]
	[TestCase(nameof(RedeclaredDefaults))]
	[TestCase(nameof(NamedIndexerSetter))]
	[TestCase(nameof(DynamicMember))]
	[TestCase(nameof(DynamicSetIndexAndMember))]
	[TestCase(nameof(DynamicConvert))]
	[TestCase(nameof(StaticConversionFromDynamic))]
	[TestCase(nameof(ArgumentOfDynamicParameter))]
	[TestCase(nameof(DynamicCompound))]
	[TestCase(nameof(DynamicInvocations))]
	[TestCase(nameof(DynamicConditions))]
	[TestCase(nameof(WithInitializer))]
	[TestCase(nameof(FormatOfArrayLiteral))]
	[TestCase(nameof(AnonymousInferred))]
	[TestCase(nameof(AnonymousNamed))]
	[TestCase(nameof(ExtensionOnNull))]
	[TestCase(nameof(DecimalToChar))]
	[TestCase(nameof(HiddenMethodReference))]
	public void EngineServesBody(string name)
	{
		AssertEngineServes(typeof(BodyGapTests).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!);
	}

	[TestCase(typeof(BodyGapHidingDerived), nameof(BodyGapHidingDerived.BaseVirtual))]
	public void EngineServesInstanceBody(Type type, string name)
	{
		AssertEngineServes(type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public)!);
	}

	static void GenericLocalFunctionArgs<T2>() where T2 : System.Collections.Generic.List<T2>
	{
		Outer<T2>(null!);
		static void Nop<T>(T data)
		{
		}
		static void Outer<T3>(T3 t3) where T3 : T2
		{
			Nop<System.Collections.Generic.List<T2>>(t3);
		}
	}

	static bool GenericExtensionOnNull() => ((System.Collections.Generic.IEnumerable<int>)null!).Any();

	static string? ConstrainedCallOnCast<T>(object o) => ((T)o)!.ToString();

	[TestCase(nameof(GenericLocalFunctionArgs))]
	[TestCase(nameof(ConstrainedCallOnCast))]
	[TestCase(nameof(GenericExtensionOnNull))]
	public void EngineServesCallBody(string name)
	{
		AssertEngineServes(typeof(BodyGapTests).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!);
	}

	// Decompiled without the ICSharpCode.Decompiler reference, every member of it these bodies
	// use is a fake member of an unknown type.
	static ICSharpCode.Decompiler.Util.BitSet UnresolvedConstructorAndIndexer()
	{
		var bits = new ICSharpCode.Decompiler.Util.BitSet(8);
		bits[1] = bits[2];
		return bits;
	}

	static ICSharpCode.Decompiler.Util.BitSet UnresolvedIndexInitializer() => new ICSharpCode.Decompiler.Util.BitSet(8) { [0] = true };

	static DecompilerSettings UnresolvedPropertiesAndMethod(DecompilerSettings settings)
	{
		settings.UsingDeclarations = !settings.UsingDeclarations;
		return settings.Clone();
	}

	static DecompilerSettings UnresolvedObjectInitializer() => new DecompilerSettings { UsingDeclarations = false };

	static void UnresolvedEvent(DecompilerSettings settings, System.ComponentModel.PropertyChangedEventHandler handler)
	{
		settings.PropertyChanged += handler;
		settings.PropertyChanged -= handler;
	}

	static ICSharpCode.Decompiler.CSharp.Syntax.TextLocation UnresolvedStaticField() => ICSharpCode.Decompiler.CSharp.Syntax.TextLocation.Empty;

	[TestCase(nameof(UnresolvedConstructorAndIndexer))]
	[TestCase(nameof(UnresolvedIndexInitializer))]
	[TestCase(nameof(UnresolvedPropertiesAndMethod))]
	[TestCase(nameof(UnresolvedObjectInitializer))]
	[TestCase(nameof(UnresolvedEvent))]
	[TestCase(nameof(UnresolvedStaticField))]
	public void EngineServesBodyWithUnresolvedMembers(string name)
	{
		AssertEngineServes(typeof(BodyGapTests).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!,
			unresolvedAssembly: "ICSharpCode.Decompiler");
	}

	static void AssertEngineServes(MethodInfo method, string? unresolvedAssembly = null)
	{
		var handle = (MethodDefinitionHandle)MetadataTokens.EntityHandle(method.MetadataToken);
		string path = typeof(BodyGapTests).Assembly.Location;
		string managed = DecompileMethod(path, handle, engineBodies: false, unresolvedAssembly);
		RustSeam2.LastFallbackReason = null;
		string engine = DecompileMethod(path, handle, engineBodies: true, unresolvedAssembly);
		Assert.That(RustSeam2.LastFallbackReason, Is.Null);
		Assert.That(engine, Is.EqualTo(managed));
	}

	static string DecompileMethod(string path, MethodDefinitionHandle handle, bool engineBodies, string? unresolvedAssembly)
	{
		RustSeam2.Enabled = engineBodies;
		try
		{
			var module = new PEFile(path, new MemoryStream(Corpus.ReadFile(path)));
			IAssemblyResolver resolver = new UniversalAssemblyResolver(path, false, module.DetectTargetFrameworkId());
			if (unresolvedAssembly != null)
				resolver = new ResolverWithout(resolver, unresolvedAssembly);
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			return decompiler.DecompileAsString(handle);
		}
		finally
		{
			RustSeam2.Enabled = true;
		}
	}

	/// <summary>A resolver that cannot find the assembly of the given name.</summary>
	sealed class ResolverWithout(IAssemblyResolver inner, string name) : IAssemblyResolver
	{
		public MetadataFile? Resolve(IAssemblyReference reference)
			=> reference.Name == name ? null : inner.Resolve(reference);

		public MetadataFile? ResolveModule(MetadataFile mainModule, string moduleName)
			=> inner.ResolveModule(mainModule, moduleName);

		public System.Threading.Tasks.Task<MetadataFile?> ResolveAsync(IAssemblyReference reference)
			=> reference.Name == name ? System.Threading.Tasks.Task.FromResult<MetadataFile?>(null) : inner.ResolveAsync(reference);

		public System.Threading.Tasks.Task<MetadataFile?> ResolveModuleAsync(MetadataFile mainModule, string moduleName)
			=> inner.ResolveModuleAsync(mainModule, moduleName);

		public IDisposable? BeginSnapshot() => inner.BeginSnapshot();
	}
}
