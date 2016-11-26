using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.CustomAttributes
{
	[TestFixture]
	public class CustomAttributeTests : DecompilerTestBase
	{
		[Test]
		[Ignore("Needs event pattern detection, which waits for improved control flow detection (loop conditions)")]
		public void CustomAttributeSamples()
		{
			ValidateFileRoundtrip(@"CustomAttributes/S_CustomAttributeSamples.cs");
		}

		[Test]
		public void CustomAttributesMultiTest()
		{
			ValidateFileRoundtrip(@"CustomAttributes/S_CustomAttributes.cs");
		}

		[Test]
		public void AssemblyCustomAttributesMultiTest()
		{
			ValidateFileRoundtrip(@"CustomAttributes/S_AssemblyCustomAttribute.cs");
		}
	}
}
