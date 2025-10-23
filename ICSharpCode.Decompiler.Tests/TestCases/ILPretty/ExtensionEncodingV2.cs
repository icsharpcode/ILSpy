using System.Collections.Generic;
using System.Linq;
namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal static class ExtensionPropertiesV2
	{
		extension<T>(ICollection<T> collection) where T : notnull
		{
			public bool IsEmpty => collection.Count == 0;
			public int Test {
				get {
					return 42;
				}
				set {
				}
			}
			public void AddIfNotNull(T item)
			{
				if (item != null)
				{
					collection.Add(item);
				}
			}
			public T2 Cast<T2>(int index) where T2 : T
			{
				return (T2)(object)collection.ElementAt(index);
			}
			public static void StaticExtension()
			{
			}
		}
	}
}