using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class AnonymousMethodEdgeCases
	{
		public Func<int, int> AssignmentIsTheLambdaBody()
		{
			return (int x) => {
				int num;
				return num = x;
			};
		}

		public Action<object> UsedParameterWithInvalidName()
		{
			return (object value) => {
				Console.WriteLine(value);
			};
		}
	}
}
