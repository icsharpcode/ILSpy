namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	// Two field initializers that each discard an out argument of the same type share a single
	// compiler-generated temporary local in the constructor (or static constructor) prologue.
	// Splitting that temporary per initializer is required to emit a discard in each one.
	public class CS73_DiscardsInFieldInitializers
	{
		public static string S = "42";

		public static bool StaticOk = int.TryParse(S, out var _);

		public static bool StaticOk2 = int.TryParse(S, out var _);

		public bool Ok = int.TryParse(S, out var _);

		public bool Ok2 = int.TryParse(S, out var _);
	}
}
