using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.EnableEditAndContinue | DebuggableAttribute.DebuggingModes.DisableOptimizations)]
[assembly: My(/*Could not decode attribute arguments.*/)]
[assembly: AssemblyVersion("0.0.0.0")]

public class MyAttribute : Attribute
{
	[NullableContext(1)]
	public MyAttribute(string x, Type t)
	{
	}
}

public class ABCD`1<string>
{
}
