using ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA;
using ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB;
using ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceC;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080
{
	internal static class ExtensionsTest
	{
		private static void Dummy(ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type2 intf)
		{
		}

		private static void Test(object obj)
		{
			ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.Type2 type = obj as ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.Type2;
			if (type != null)
			{
				ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceC.Extensions.Extension(type);
			}
		}
	}
}

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA
{
	internal interface Type2 : ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type2, Type1
	{
	}
}

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB
{
	internal static class Extensions
	{
		public static void Extension(this Type1 obj)
		{
		}
	}
	internal interface Type1
	{
	}
	internal interface Type2 : Type1
	{
	}
}

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceC
{
	internal static class Extensions
	{
		public static void Extension(this Type1 obj)
		{
		}
	}
}
