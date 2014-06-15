using System;
using ICSharpCode.Decompiler.IL;

namespace ICSharpCode.Decompiler
{
	static class Argument
	{
		public static T NotNull<T>(T value, string paramName)
		{
			if (value == null)
				throw new ArgumentNullException(paramName);
			return value;
		}
	}
}