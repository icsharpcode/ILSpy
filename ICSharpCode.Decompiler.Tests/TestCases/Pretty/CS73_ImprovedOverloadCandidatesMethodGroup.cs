using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	// C# 7.3 "improved overload candidates" for method groups: when the receiver is an
	// instance, static candidates are removed from the set. Here the static overload is the
	// better parameter match, so `this.M` is the only way to bind the method group to the
	// instance overload; a bare `M` re-resolves to the static overload and silently changes
	// which method the delegate invokes. This fixture is the desired output: the decompiler
	// must qualify the method group with `this.` to preserve the original binding.
	public class CS73_ImprovedOverloadCandidatesMethodGroup
	{
		public static string M(string s)
		{
			return "static M(string)";
		}

		public string M(object o)
		{
			return "instance M(object)";
		}

		public Func<string, string> ViaThis()
		{
			return this.M;
		}
	}
}
