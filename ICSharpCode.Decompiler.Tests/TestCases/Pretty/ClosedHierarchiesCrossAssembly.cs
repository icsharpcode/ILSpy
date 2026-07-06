// #dependency ClosedHierarchiesCrossAssembly.dep.cs
using CrossAssemblyClosed;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public closed record LocalMessage;
	public record LocalTextMessage(string Text) : LocalMessage;
	public record Sedan(int Doors) : Car(Doors);
}
