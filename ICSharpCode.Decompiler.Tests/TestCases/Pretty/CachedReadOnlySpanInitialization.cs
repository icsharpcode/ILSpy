using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class CachedReadOnlySpanInitialization
	{
		// On target frameworks without RuntimeHelpers.CreateSpan (before .NET 7) Roslyn emits a
		// compiler-generated lazy cache in <PrivateImplementationDetails> for a ReadOnlySpan<char>
		// created from a multi-byte array literal. The CachedReadOnlySpanInitialization transform
		// collapses that cache back to the explicit ReadOnlySpan constructor.
#if NET70
		public static ReadOnlySpan<char> NewLine => new char[2] { '\r', '\n' };
#else
		public static ReadOnlySpan<char> NewLine => new ReadOnlySpan<char>(new char[2] { '\r', '\n' });
#endif
	}
}
