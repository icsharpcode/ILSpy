using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
#if !EXPECTED_OUTPUT
	public struct MissingMemory<T>
	{
	}
#endif

	[CompilerGenerated]
	public sealed class CompilerGeneratedAutoProperty
	{
		public string Name { get; }

		public CompilerGeneratedAutoProperty(string name)
		{
			Name = name;
		}
	}

	public class UnresolvedGenericAutoProperty<T>
	{
		public MissingMemory<T> Data { get; set; }
	}
}
