// Copyright (c) 2018 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.CSharp
{
	public enum LanguageVersion
	{
		CSharp1 = 1,
		CSharp2 = 2,
		CSharp3 = 3,
		CSharp4 = 4,
		CSharp5 = 5,
		CSharp6 = 6,
		CSharp7 = 7,
		CSharp7_1 = 701,
		CSharp7_2 = 702,
		CSharp7_3 = 703,
		CSharp8_0 = 800,
		CSharp9_0 = 900,
		CSharp10_0 = 1000,
		CSharp11_0 = 1100,
		Preview = 1100,
		Latest = 0x7FFFFFFF
	}
}
