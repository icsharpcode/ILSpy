using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class Issue2104
	{
		[CompilerGenerated]
		private readonly string text;
		public string Text {
			[CompilerGenerated]
			get {
				return text;
			}
		}
		public Issue2104(string text)
		{
			this.text = text;
		}
	}
}
