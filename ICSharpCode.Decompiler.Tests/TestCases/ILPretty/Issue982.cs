namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class Issue982
	{
		private string textStr2;

		public string Text { get; set; }

		public string this[int index] {
			get => this.textStr2;
			set => this.textStr2 = value;
		}
	}
}
