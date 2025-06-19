using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal class NoGenericTypeInstantiation<TOnType> where TOnType : new()
	{
		public static TOnType CreateTOnType()
		{
			return Activator.CreateInstance<TOnType>();
		}

		public static TOnMethod CreateTOnMethod<TOnMethod>() where TOnMethod : new()
		{
			return Activator.CreateInstance<TOnMethod>();
		}
	}
}
