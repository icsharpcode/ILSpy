// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

// compile:
//     csc ShortCircuit.cs /t:Library && ildasm /text ShortCircuit.dll >ShortCircuit.il
//     csc ShortCircuit.cs /t:Library /o /out:ShortCircuit.opt.dll && ildasm /text ShortCircuit.opt.dll >ShortCircuit.opt.il


namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public abstract class ShortCircuit
	{
		public abstract void B(bool b);
		public abstract bool F(int i);
		public abstract int GetInt(int i);
		public abstract void M1();
		public abstract void M2();
		public abstract void E();

		public void ExprAnd()
		{
			B(F(0) && F(1));
		}

		public void ExprOr()
		{
			B(F(0) || F(1));
		}

		public void ExprCond()
		{
			B(F(0) ? F(1) : F(2));
		}

		public void ExprCondAnd()
		{
			B((F(0) && F(1)) ? F(2) : F(3));
		}

		public void ExprMix4A()
		{
			B(((F(0) || F(1)) && F(2)) || F(3));
		}

		public void ExprMix4B()
		{
			B((F(0) || F(1)) && (F(2) || F(3)));
		}

		public void ExprMix4C()
		{
			B((F(0) && F(1)) || (F(2) && F(3)));
		}

		public void StmtAnd2()
		{
			if (F(0) && F(1))
			{
				M1();
			}
			else
			{
				M2();
			}
			E();
		}

		public void StmtOr2A()
		{
			if (F(0) || F(1))
			{
				M1();
			}
		}

		public void StmtOr2B()
		{
			if (F(0) || F(1))
			{
				M1();
			}
			else
			{
				M2();
			}
			E();
		}

		public void StmtAnd3()
		{
			if (F(0) && F(1) && F(2))
			{
				M1();
			}
			else
			{
				M2();
			}
			E();
		}

		public void StmtOr3()
		{
			if (F(0) || F(1) || F(2))
			{
				M1();
			}
			else
			{
				M2();
			}
			E();
		}

		public void StmtOr4()
		{
			if (GetInt(0) != 0 || GetInt(1) != 0)
			{
				M1();
			}
			else
			{
				M2();
			}
			E();
		}

		public void StmtMix3A()
		{
			if ((F(0) || F(1)) && F(2))
			{
				M1();
			}
		}

		public void StmtMix3B()
		{
			if ((F(0) || F(1)) && F(2))
			{
				M1();
			}
			else
			{
				M2();
			}
		}

		public void StmtMix4V1A()
		{
			if (((F(0) || F(1)) && F(2)) || F(3))
			{
				M1();
			}
		}

		public void StmtMix4V1B()
		{
			if (((F(0) || F(1)) && F(2)) || F(3))
			{
				M1();
			}
			else
			{
				M2();
			}
		}

		public void StmtMix4V2A()
		{
			if ((F(0) || F(1)) && (F(2) || F(3)))
			{
				M1();
			}
		}

		public void StmtMix4V2B()
		{
			if ((F(0) || F(1)) && (F(2) || F(3)))
			{
				M1();
			}
			else
			{
				M2();
			}
		}

		public void StmtMix4V3A()
		{
			if ((F(0) && F(1)) || (F(2) && F(3)))
			{
				M1();
			}
		}

		public void StmtMix4V3B()
		{
			if ((F(0) && F(1)) || (F(2) && F(3)))
			{
				M1();
			}
			else
			{
				M2();
			}
		}

		public void StmtComplex()
		{
			if (F(0) && F(1) && !F(2) && (F(3) || F(4)))
			{
				M1();
			}
			else
			{
				M2();
			}
			E();
		}

		public void StmtComplex2(int i)
		{
			if (i > 1000 || (i >= 1 && i <= 8) || i == 42)
			{
				M1();
			}
			else
			{
				M2();
			}
			E();
		}

		public void StmtComplex3(int i)
		{
			if (i > 1000 || (i >= 1 && i <= 8) || (i >= 100 && i <= 200) || i == 42)
			{
				M1();
			}
			else
			{
				M2();
			}
			E();
		}

		public void StmtComplex4(int i)
		{
			if (i > 1000 || (i >= 1 && i <= 8) || i == 42 || i == 23)
			{
				M1();
			}
			else
			{
				M2();
			}
			E();
		}

		public void StmtComplex5()
		{
			if (F(0))
			{
				if (!F(1) && !F(2))
				{
					return;
				}
			}
			else if (!F(3) || !F(4))
			{
				M2();
				return;
			}
			E();
		}

		public int StmtComplex6()
		{
			if (F(0))
			{
				M1();
				if (F(1) || F(2))
				{
					return 1;
				}
			}
			return 2;
		}

		public int InferCorrectOrder()
		{
			if (F(1) || F(2))
			{
				return 1;
			}
			return 2;
		}

#if !OPT
		public void EmptyIf()
		{
			if (F(0))
			{
			}
			if (!F(1))
			{
			}
			if (F(2) && F(3))
			{
			}
			if (F(4) || F(5))
			{
			}
			if (F(0) && F(1) && !F(2) && (F(3) || F(4)))
			{
			}
			E();
		}
#endif

		public void PreferLogicalToBitwise(bool a, bool b, int i, float f)
		{
			B(a && b);
			B(a && i == 1);
			B(i == 1 && a);
			B(i > i - 3 && a);
			B(f < 0.1f && a);
		}
	}
}
