#if !OPT
using System.Diagnostics;
#endif
using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly;

internal class NoGetterOnlyAutomaticProperties
{
	[CompilerGenerated]
#if !OPT
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
	private readonly int GetOnly__BackingField;

	public int GetOnly {
		[CompilerGenerated]
		get {
			return GetOnly__BackingField;
		}
	}

	public int WithSetter { get; set; }

	public NoGetterOnlyAutomaticProperties()
	{
		GetOnly__BackingField = 5;
	}
}
