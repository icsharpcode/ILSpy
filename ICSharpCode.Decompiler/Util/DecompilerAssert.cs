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

#nullable enable

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;

namespace ICSharpCode.Decompiler.Util
{
	static class DecompilerAssert
	{
		[Conditional("DEBUG")]
		public static void That([DoesNotReturnIf(false)] bool condition)
		{
#if DEBUG
			if (!condition)
				Debug.Fail("Assertion failed.");
#endif
		}

		[Conditional("DEBUG")]
		public static void That([DoesNotReturnIf(false)] bool condition,
			[InterpolatedStringHandlerArgument(nameof(condition))] ref AssertInterpolatedStringHandler message)
		{
#if DEBUG
			if (!condition)
				Debug.Fail(message.ToString());
#endif
		}
	}

	[InterpolatedStringHandler]
	ref struct AssertInterpolatedStringHandler
	{
		readonly bool enabled;
		StringBuilder? builder;

		public AssertInterpolatedStringHandler(int literalLength, int formattedCount, bool condition, out bool shouldAppend)
		{
			enabled = !condition;
			shouldAppend = enabled;
			builder = enabled ? new StringBuilder(literalLength) : null;
		}

		public void AppendLiteral(string value)
		{
			if (enabled)
				builder!.Append(value);
		}

		public void AppendFormatted<T>(T value)
		{
			if (enabled)
				builder!.Append(value);
		}

		public override string ToString()
		{
			return builder?.ToString() ?? string.Empty;
		}
	}
}

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	sealed class InterpolatedStringHandlerAttribute : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	sealed class InterpolatedStringHandlerArgumentAttribute : Attribute
	{
		public InterpolatedStringHandlerArgumentAttribute(string argument)
		{
			Arguments = new[] { argument };
		}

		public InterpolatedStringHandlerArgumentAttribute(params string[] arguments)
		{
			Arguments = arguments;
		}

		public string[] Arguments { get; }
	}
}
