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
		public abstract void M1();
		public abstract void M2();
		public abstract void E();
		
		public void ExprAnd()
		{
			this.B(this.F(0) && this.F(1));
		}
		
		public void ExprOr()
		{
			this.B(this.F(0) || this.F(1));
		}
		
		public void ExprCond()
		{
			this.B(this.F(0) ? this.F(1) : this.F(2));
		}
		
		public void ExprCondAnd()
		{
			this.B((this.F(0) && this.F(1)) ? this.F(2) : this.F(3));
		}

		public void StmtAnd2()
		{
			if (this.F(0) && this.F(1)) {
				this.M1();
			} else {
				this.M2();
			}
			this.E();
		}
		
		public void StmtOr2()
		{
			if (this.F(0) || this.F(1)) {
				this.M1();
			} else {
				this.M2();
			}
			this.E();
		}

		public void StmtAnd3()
		{
			if (this.F(0) && this.F(1) && this.F(2)) {
				this.M1();
			} else {
				this.M2();
			}
			this.E();
		}

		public void StmtOr3()
		{
			if (this.F(0) || this.F(1) || this.F(3)) {
				this.M1();
			} else {
				this.M2();
			}
			this.E();
		}

		public void StmtComplex()
		{
			if (this.F(0) && this.F(1) && !this.F(2) && (this.F(3) || this.F(4))) {
				this.M1();
			} else {
				this.M2();
			}
			this.E();
		}

		public void StmtComplex2(int i)
		{
			if (i > 1000 || (i >= 1 && i <= 8) || i == 42) {
				this.M1();
			} else {
				this.M2();
			}
			this.E();
		}

		public void StmtComplex3(int i)
		{
			if (i > 1000 || (i >= 1 && i <= 8) || (i >= 100 && i <= 200) || i == 42) {
				this.M1();
			} else {
				this.M2();
			}
			this.E();
		}

		public void StmtComplex4(int i)
		{
			if (i > 1000 || (i >= 1 && i <= 8) || i == 42 || i == 23) {
				this.M1();
			} else {
				this.M2();
			}
			this.E();
		}
	}
}
