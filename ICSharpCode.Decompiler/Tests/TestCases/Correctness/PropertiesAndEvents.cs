using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class PropertiesAndEvents
	{
		public static int Main(string[] args)
		{
			Index i = new Index();
			i.AutoProp = "Name";
			Console.WriteLine("AutoProp set!");
			i[0] = 5;
			i[1] = 2;
			Console.WriteLine("{0} {1}", i[0], i[5]);
			Console.WriteLine("PI² = {0}", i.PISquare);
			return 0;
		}
	}

	class Index
	{
		int thisValue;

		public int this[int i]
		{
			get { Console.WriteLine("get_this({0})", i); return i * i; }
			set { Console.WriteLine("set_this({0}, {1})", i, value);  thisValue = value; }
		}

		public string AutoProp { get; set; }

		public double PISquare
		{
			get { Console.WriteLine("get_PISquare"); return Math.Pow(Math.PI, 2); }
		}
	}
}
