using System;
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly;

internal class NoAutomaticProperties
{
	public int Plain {
		[CompilerGenerated]
		get {
			return field;
		}
		[CompilerGenerated]
		set {
			field = value;
		}
	}

	public int WithInitializer {
		[CompilerGenerated]
		get {
			return field;
		}
		[CompilerGenerated]
		set {
			field = value;
		}
	} = 5;

	public int SemiAuto {
		get {
			return field;
		}
		set {
			field = Math.Max(0, value);
		}
	}
}
