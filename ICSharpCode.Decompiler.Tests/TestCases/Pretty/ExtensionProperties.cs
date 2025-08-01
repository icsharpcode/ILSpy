using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	static class ExtensionProperties
	{
		extension<T>(ICollection<T> collection)
		{
			public bool IsEmpty => collection.Count == 0;
	}
}
}
