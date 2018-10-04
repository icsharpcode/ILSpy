namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1281
{
	internal class Issue1281
	{
		internal void MethodWithParams(params object[] args)
		{
			object[] array = new object[0];
			MethodWithParams(array);
			MethodWithParams(new object[1] {
				array
			});
		}

		internal void MethodWithIntAndParams(int arg, params object[] args)
		{
			object[] array = new object[0];
			MethodWithIntAndParams(10, array);
			MethodWithIntAndParams(10, new object[1] {
				array
			});
		}
	}
}
