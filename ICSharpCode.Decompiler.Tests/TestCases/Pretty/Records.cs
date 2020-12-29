namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public record Empty
	{
	}

	public record Fields
	{
		public int A;
		public double B = 1.0;
		public object C;
		public dynamic D;
		public string S = "abc";
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
