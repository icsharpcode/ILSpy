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
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Rust.Interop;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// Operators the engine translates in method bodies (seam 2): each method below decompiles
/// with seam 2 strict, so a body the engine cannot serve fails, to the managed output.
/// Non-parallelizable because strictness is the process-wide <c>ILSPY_RUST_STRICT_SEAM2</c>.
/// </summary>
[TestFixture, NonParallelizable]
public class Seam2OperatorTests
{
	public struct Money
	{
		public int Value;

		public static Money operator +(Money a, int b) => new Money { Value = a.Value + b };
		public static Money operator ++(Money a) => new Money { Value = a.Value + 1 };
	}

	public struct Flag
	{
		public bool Value;

		public static Flag operator &(Flag a, Flag b) => new Flag { Value = a.Value && b.Value };
		public static Flag operator |(Flag a, Flag b) => new Flag { Value = a.Value || b.Value };
		public static bool operator true(Flag a) => a.Value;
		public static bool operator false(Flag a) => !a.Value;
	}

	public enum Color
	{
		Red,
		Green
	}

	static Money money;
	static int? nullableNumber;
	static string text = "";
	static int number;
	static byte small;
	static Action? handler;

	static void UserDefinedCompoundAssign(int n)
	{
		money += n;
		money++;
		++money;
		text += "x";
	}

	static void CompoundShift(int n)
	{
		number <<= n;
		number >>= 2;
		number >>>= 3;
		small >>= 1;
	}

	static void TakeShort(short s, sbyte b, ushort u)
	{
	}

	static void SmallIntegerMinMax()
	{
		TakeShort(short.MaxValue, sbyte.MinValue, ushort.MaxValue);
	}

	static bool DelegateCalls(Action other)
	{
		handler = (Action?)Delegate.Combine(handler, other);
		handler = (Action?)Delegate.Remove(handler, other);
		return handler == other;
	}

	static long Negation(int x, long y, short s)
	{
		return -x - y + -s;
	}

	static int SwitchExpressionOnString(string s) => s switch {
		"a" => 1,
		"b" => 2,
		"c" => 3,
		"d" => 4,
		"e" => 5,
		"f" => 6,
		"g" => 7,
		"h" => 8,
		_ => 0
	};

	static int LiftedSwitch(int? i)
	{
		switch (i)
		{
			case null:
				return 0;
			case 1:
				return 10;
			case 2:
				return 20;
			case 3:
				return 30;
			default:
				return 40;
		}
	}

	static void DefaultDecimal(out decimal d)
	{
		d = default;
	}

	static bool LiftedOperations(int? a, long? b, int c, bool? x, bool? y)
	{
		long? sum = a + b * c;
		nullableNumber += c;
		nullableNumber <<= c;
		int? inverted = ~a;
		uint? converted = (uint?)a;
		bool? both = x & y;
		return a < c && sum > b && (x | y) == both && converted != unchecked((uint?)-1) && (a << c) == inverted;
	}

	static Flag ShortCircuit(Flag a, Flag b) => a && b || a;

	static int EnumArithmetic(Color c) => (int)c * 3;

	// The compiler folds `~` of a constant, so the expression tree (which the decompiler turns
	// back into a lambda) is the way to get `~` of a constant into the ILAst unoptimized.
	static Expression<Func<bool>> ConstantBitNot() => Expression.Lambda<Func<bool>>(
		Expression.AndAlso(
			Expression.Equal(Expression.Not(Expression.Constant(0L, typeof(long))), Expression.Constant(-1L, typeof(long))),
			Expression.Equal(
				Expression.Convert(Expression.Not(Expression.Constant(0, typeof(int))), typeof(ushort)),
				Expression.Constant(ushort.MaxValue, typeof(ushort)))),
		Array.Empty<ParameterExpression>());

	[TestCase(nameof(UserDefinedCompoundAssign))]
	[TestCase(nameof(ConstantBitNot))]
	[TestCase(nameof(LiftedOperations))]
	[TestCase(nameof(ShortCircuit))]
	[TestCase(nameof(EnumArithmetic))]
	[TestCase(nameof(CompoundShift))]
	[TestCase(nameof(SmallIntegerMinMax))]
	[TestCase(nameof(DelegateCalls))]
	[TestCase(nameof(Negation))]
	[TestCase(nameof(SwitchExpressionOnString))]
	[TestCase(nameof(LiftedSwitch))]
	[TestCase(nameof(DefaultDecimal))]
	public void EngineServesBody(string name)
	{
		var method = typeof(Seam2OperatorTests).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!;
		var handle = (MethodDefinitionHandle)MetadataTokens.EntityHandle(method.MetadataToken);
		string managed = DecompileMethod(handle, engineBodies: false);
		string? previous = Environment.GetEnvironmentVariable("ILSPY_RUST_STRICT_SEAM2");
		Environment.SetEnvironmentVariable("ILSPY_RUST_STRICT_SEAM2", "1");
		string engine;
		try
		{
			engine = DecompileMethod(handle, engineBodies: true);
		}
		finally
		{
			Environment.SetEnvironmentVariable("ILSPY_RUST_STRICT_SEAM2", previous);
		}
		Assert.That(engine, Is.EqualTo(managed));
	}

	static string DecompileMethod(MethodDefinitionHandle handle, bool engineBodies)
	{
		string path = typeof(Seam2OperatorTests).Assembly.Location;
		RustSeam2.Enabled = engineBodies;
		try
		{
			var module = new PEFile(path, new MemoryStream(Corpus.ReadFile(path)));
			var resolver = new UniversalAssemblyResolver(path, false, module.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			return decompiler.DecompileAsString(handle);
		}
		finally
		{
			RustSeam2.Enabled = true;
		}
	}
}
