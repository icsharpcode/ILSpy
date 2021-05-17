// #dependency Issue3684.dep.cs
using CrossAssemblyDep;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class Issue3684
	{
		public interface IInterface
		{
			string Name { get; set; }
			T Convert<T>(T input);
		}

		public class DerivedClass : BaseClass, IInterface
		{
			T IInterface.Convert<T>(T input)
			{
				return ((BaseClass)this).Convert<T>(input);
			}
		}
	}
}
