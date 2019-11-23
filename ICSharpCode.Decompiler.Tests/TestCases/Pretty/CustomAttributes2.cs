using System;
namespace CustomAttributes2
{
	public static class CustomAttributes
	{
		[Flags]
		public enum EnumWithFlag
		{
			All = 0xF,
			None = 0x0,
			Item1 = 0x1,
			Item2 = 0x2,
			Item3 = 0x4,
			Item4 = 0x8
		}
		[AttributeUsage(AttributeTargets.All)]
		public class MyAttribute : Attribute
		{
			public MyAttribute(EnumWithFlag en)
			{
			}
		}
		[My(EnumWithFlag.Item1 | EnumWithFlag.Item2)]
		private static int field;
		[My(EnumWithFlag.All)]
#if ROSLYN
		public static string Property => "aa";
#else
		public static string Property {
			get {
				return "aa";
			}
		}
#endif
		public static string GetterOnlyPropertyWithAttributeOnGetter {
			[My(EnumWithFlag.Item1)]
			get {
				return "aa";
			}
		}
		[My(EnumWithFlag.All)]
		public static string GetterOnlyPropertyWithAttributeOnGetter2 {
			[My(EnumWithFlag.Item1)]
			get {
				return "aa";
			}
		}
		[Obsolete("some message")]
		public static void ObsoletedMethod()
		{
			Console.WriteLine("{0} $$$ {1}", AttributeTargets.Interface, AttributeTargets.Property | AttributeTargets.Field);
			AttributeTargets attributeTargets = AttributeTargets.Property | AttributeTargets.Field;
			Console.WriteLine("{0} $$$ {1}", AttributeTargets.Interface, attributeTargets);
		}
	}
}
