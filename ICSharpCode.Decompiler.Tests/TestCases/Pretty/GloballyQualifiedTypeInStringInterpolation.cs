namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public static class GloballyQualifiedTypeInStringInterpolation
	{
		public static string Root => $"Prefix {(global::System.DateTime.Now)} suffix";
		public static string Cast => $"Prefix {((int)global::System.DateTime.Now.Ticks)} suffix";
#if CS100 && NET60
		public static string Lambda1 => $"Prefix {(() => global::System.DateTime.Now)} suffix";
#else
		public static string Lambda1 => $"Prefix {(global::System.Func<global::System.DateTime>)(() => global::System.DateTime.Now)} suffix";
#endif
		public static string Lambda2 => $"Prefix {((global::System.Func<global::System.DateTime>)(() => global::System.DateTime.Now))()} suffix";
		public static string Method1 => $"Prefix {M(global::System.DateTime.Now)} suffix";
		public static string Method2 => $"Prefix {(global::System.DateTime.Now.Ticks)} suffix";
		public static string Method3 => $"Prefix {(global::System.DateTime.Equals(global::System.DateTime.Now, global::System.DateTime.Now))} suffix";
		public static string ConditionalExpression1 => $"Prefix {(Boolean ? global::System.DateTime.Now : global::System.DateTime.UtcNow)} suffix";
		public static string ConditionalExpression2 => $"Prefix {(Boolean ? global::System.DateTime.Now : global::System.DateTime.UtcNow).Ticks} suffix";

		private static bool Boolean => false;
		private static long M(global::System.DateTime time)
		{
			return time.Ticks;
		}
	}
}
