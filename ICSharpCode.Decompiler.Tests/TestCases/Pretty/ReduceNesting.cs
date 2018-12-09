using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public abstract class ReduceNesting
	{
		public abstract bool B(int i);
		public abstract int I(int i);

		public void IfIf()
		{
			if (B(0)) {
				Console.WriteLine(0);
				return;
			}

			if (B(1)) {
				Console.WriteLine(1);
			}

			Console.WriteLine("end");
		}

		public void IfSwitch()
		{
			if (B(0)) {
				Console.WriteLine(0);
				return;
			}

			Console.WriteLine("switch");
			switch (I(0)) {
				case 0:
					Console.WriteLine("case 0");
					break;
				case 1:
					Console.WriteLine("case 1");
					break;
				default:
					Console.WriteLine("end");
					break;
			}
		}

		public void IfSwitchSwitch()
		{
			if (B(0)) {
				Console.WriteLine(0);
				return;
			}

			Console.WriteLine("switch 0");
			switch (I(1)) {
				case 0:
					Console.WriteLine("case 0");
					return;
				case 1:
					Console.WriteLine("case 1");
					return;
			}

			Console.WriteLine("switch 1");
			switch (I(1)) {
				case 0:
					Console.WriteLine("case 0");
					break;
				case 1:
					Console.WriteLine("case 1");
					break;
				default:
					Console.WriteLine("end");
					break;
			}
		}

		public void IfLoop()
		{
			if (B(0)) {
				Console.WriteLine(0);
				return;
			}

			for (int i = 0; i < 10; i++) {
				Console.WriteLine(i);
			}

			Console.WriteLine("end");
		}

		public void LoopContinue()
		{
			for (int i = 0; i < 10; i++) {
				Console.WriteLine(i);
				if (B(0)) {
					Console.WriteLine(0);
					continue;
				}
				
				if (B(1)) {
					Console.WriteLine(1);
				}
				Console.WriteLine("loop-tail");
			}
		}

		public void LoopBreak()
		{
			for (int i = 0; i < 10; i++) {
				Console.WriteLine(i);
				if (B(0)) {
					Console.WriteLine(0);
					continue;
				}
				
				if (B(1)) {
					Console.WriteLine(1);
					break;
				}

				if (B(2)) {
					Console.WriteLine(2);
				}

				Console.WriteLine("break");
				break;
			}
			Console.WriteLine("end");
		}

		public void LoopBreakElseIf()
		{
			for (int i = 0; i < 10; i++) {
				Console.WriteLine(i);
				if (B(0)) {
					Console.WriteLine(0);
					continue;
				}
				
				if (B(1)) {
					Console.WriteLine(1);
				} else if (B(2)) {
					Console.WriteLine(2);
				}
				break;
			}
			Console.WriteLine("end");
		}

		public void SwitchIf()
		{
			switch (I(0)) {
				case 0:
					Console.WriteLine("case 0");
					return;
				case 1:
					Console.WriteLine("case 1");
					return;
			}
			
			if (B(0)) {
				Console.WriteLine(0);
			}
			Console.WriteLine("end");
		}

		public void NestedSwitchIf()
		{
			if (B(0)) {
				switch (I(0)) {
					case 0:
						Console.WriteLine("case 0");
						return;
					case 1:
						Console.WriteLine("case 1");
						return;
				}

				if (B(1)) {
					Console.WriteLine(1);
				}
			} else {
				Console.WriteLine("else");
			}
		}

		// nesting should not be reduced as maximum nesting level is 1
		public void EarlyExit1()
		{
			if (!B(0)) {
				for (int i = 0; i < 10; i++) {
					Console.WriteLine(i);
				}
				Console.WriteLine("end");
			}
		}
		
		// nesting should not be reduced as maximum nesting level is 2
		public void EarlyExit2()
		{
			if (B(0)) {
				return;
			}

			for (int i = 0; i < 10; i++) {
				Console.WriteLine(i);
				if (i % 2 == 0) {
					Console.WriteLine("even");
				}
			}

			Console.WriteLine("end");
		}
		
		// nesting should not be reduced as maximum nesting level is 1 and the else block has no more instructions than any other block
		public void BalancedIf()
		{
			if (B(0)) {
				Console.WriteLine("true");
				if (B(1)) {
					Console.WriteLine(1);
				}
			} else {
				if (B(2)) {
					Console.WriteLine(2);
				}
				Console.WriteLine("false");
			}
		}
		
		public string ComplexCase1(string s)
		{
			if (B(0)) {
				return s;
			}

			for (int i = 0; i < s.Length; i++) {
				if (B(1)) {
					Console.WriteLine(1);
				} else if (B(2)) {
					switch (i) {
						case 1:
							if (B(3)) {
								Console.WriteLine(3);
								break;
							}
							
							Console.WriteLine("case1");
							if (B(4)) {
								Console.WriteLine(4);
							}
							break;
						case 2:
						case 3:
							Console.WriteLine("case23");
							break;
					}
					Console.WriteLine(2);
				} else if (B(5)) {
					Console.WriteLine(5);
				} else {
					if (B(6)) {
						Console.WriteLine(6);
					}
					Console.WriteLine("else");
				}
			}
			return s;
		}
	}
}
