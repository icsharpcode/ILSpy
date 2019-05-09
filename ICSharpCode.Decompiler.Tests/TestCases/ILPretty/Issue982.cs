using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class Issue982
	{
		private string textStr;

		private string textStr2;

		public string Text {
			get {
				return textStr;
			}
			set {
				textStr = value;
			}
		}

		[IndexerName("Text2")]
		public string this[int index] {
			get {
				return textStr2;
			}
			set {
				textStr2 = value;
			}
		}
	}
}
