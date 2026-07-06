using System;
#if !OPT
using System.Diagnostics;
#endif
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly;

internal class GenericHolder<T> where T : class
{
	[CompilerGenerated]
#if !OPT
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
	private T Value__BackingField;

	public T Value {
		get {
			return Value__BackingField;
		}
		set {
			if (value != null)
			{
				Value__BackingField = value;
			}
		}
	}
}
internal class NoFieldKeyword
{
	[CompilerGenerated]
#if !OPT
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
	private int Clamped__BackingField;

	[CompilerGenerated]
#if !OPT
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
	private int WithInitializer__BackingField = 5;

	public int Clamped {
		get {
			return Clamped__BackingField;
		}
		set {
			Clamped__BackingField = Math.Max(0, value);
		}
	}

	public int WithInitializer {
		get {
			return WithInitializer__BackingField;
		}
		set {
			WithInitializer__BackingField = value + 1;
		}
	}
}
