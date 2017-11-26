namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class Issue982
	{
		private string textStr;

		private string textStr2;

		public string Text {
			get {
				return this.textStr;
			}
			set {
				this.textStr = value;
			}
		}

		public string this[int index] {
			get {
				return this.textStr2;
			}
			set {
				this.textStr2 = value;
			}
		}
	}
}
