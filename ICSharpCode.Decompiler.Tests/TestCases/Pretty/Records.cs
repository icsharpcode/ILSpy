namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public record Empty
	{
	}

	public record Properties
	{
		public int A { 
			get; 
			set;
		}
		public int B {
			get;
		}
		public int C => 43;
		public object O { 
			get; 
			set;
		}
		public string S { 
			get;
			set;
		}
		public dynamic D {
			get;
			set;
		}

		public Properties()
		{
			B = 42;
		}
	}
}
