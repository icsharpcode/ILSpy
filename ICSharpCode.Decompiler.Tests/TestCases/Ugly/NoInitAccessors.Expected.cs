namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal class NoInitAccessors
	{
		private string name;

		public int AutoInit { get; set/*init*/; }

		public string Name {
			get {
				return name;
			}
			set/*init*/ {
				name = value;
			}
		}
	}
}
