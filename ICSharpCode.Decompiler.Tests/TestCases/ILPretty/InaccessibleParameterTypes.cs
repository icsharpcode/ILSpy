using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class InaccessibleParameterTypes
	{
		private class Hidden
		{
		}

		public delegate void Handler(Hidden h);

		public static void Register(Action<Hidden> callback)
		{
		}
	}
	public class InaccessibleParameterTypesConsumer
	{
		public InaccessibleParameterTypes.Handler Create()
		{
			return delegate {
			};
		}

		public void Run()
		{
			InaccessibleParameterTypes.Register(delegate {
			});
		}
	}
}
