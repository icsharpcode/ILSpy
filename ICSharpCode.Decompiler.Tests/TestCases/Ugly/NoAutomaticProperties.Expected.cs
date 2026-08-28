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

	public int SemiAuto {
		get {
			return field;
		}
		set {
			field = Math.Max(0, value);
		}
	}
}
