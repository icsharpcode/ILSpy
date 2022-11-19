using System;
using System.Collections;
using System.Runtime.CompilerServices;

public class VBNonGenericForEach
{
	public static void M()
	{
		ArrayList arrayList = new ArrayList();
		foreach (object item in arrayList)
		{
#if ROSLYN && OPT
			Console.WriteLine(RuntimeHelpers.GetObjectValue(RuntimeHelpers.GetObjectValue(item)));
#else
			object objectValue = RuntimeHelpers.GetObjectValue(item);
			Console.WriteLine(RuntimeHelpers.GetObjectValue(objectValue));
#endif
		}
	}
}
