using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.CustomAttributes
{
	[TestFixture]
	public class CustomAttributeTests : DecompilerTestBase
	{
		[Test]
		public void CustomAttributeSamples()
		{
			ValidateFileRoundtrip(@"CustomAttributes/S_CustomAttributeSamples.cs");
		}

		[Test]
		public void CustomAttributes()
		{
			ValidateFileRoundtrip(@"CustomAttributes/S_CustomAttributes.cs");
		}

		[Test]
		public void AssemblyCustomAttribute()
		{
			ValidateFileRoundtrip(@"CustomAttributes/S_AssemblyCustomAttribute.cs");
		}
	}
}
