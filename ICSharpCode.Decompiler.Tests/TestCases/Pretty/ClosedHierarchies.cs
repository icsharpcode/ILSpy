namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class ClosedHierarchies
	{
		public closed class Animal
		{
			public string Name { get; }

			protected Animal()
			{
				Name = "unnamed";
			}

			protected Animal(string name)
			{
				Name = name;
			}
		}

		public class Cat : Animal
		{
		}

		public sealed class Dog : Animal
		{
			public Dog()
				: base("Dog")
			{
			}
		}

		public closed class Job
		{
			public required string Title { get; set; }
		}

		public sealed class CompileJob : Job
		{
		}

		public closed class Tree<T>
		{
		}

		public sealed class Leaf<U> : Tree<U>
		{
		}

		public sealed class ArrayLeaf<V> : Tree<V[]>
		{
		}
	}
	public closed record JobStatus;
	internal record JobStatusCanceled : JobStatus;
	public record JobStatusQueued : JobStatus;
	public record JobStatusRunning(int PercentComplete) : JobStatus;
	internal closed class State
	{
	}
	internal sealed class StateActive : State
	{
	}
}
#if !EXPECTED_OUTPUT
// The .NET 11 preview 5 BCL does not ship ClosedAttribute yet; the compiler requires
// every assembly using the 'closed' modifier to declare it (matched by full name).
namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	internal sealed class ClosedAttribute : Attribute
	{
	}
}
#endif
