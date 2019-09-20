// Issue1389.Program
using System;

namespace Issue1389
{
	public class Program
	{
		private static object GetObject()
		{
			throw null;
		}

		private static void UnusedResultOfIsinst()
		{
			_ = (GetObject() is TypeCode);
		}

		private static bool BoolResultOfIsinst()
		{
			return GetObject() is TypeCode;
		}

		private static object EnumResultOfIsinst(object A_0)
		{
			return (A_0 is TypeCode) ? A_0 : null;
		}
	}
}