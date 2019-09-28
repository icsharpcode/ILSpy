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

using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class InterfaceTests
	{
		public interface IA
		{
			int Property1 {
				get;
			}
			int Property2 {
				set;
			}
			int Property3 {
				get;
				set;
			}

			event EventHandler MyEvent;
			void Method();
		}
		public interface IA2 : IA
		{
		}
		public interface IB
		{
		}
		public class C : IA2, IA, IB
		{
			int IA.Property1 {
				get {
					throw new NotImplementedException();
				}
			}
			int IA.Property2 {
				set {
					throw new NotImplementedException();
				}
			}
			int IA.Property3 {
				get {
					throw new NotImplementedException();
				}
				set {
					throw new NotImplementedException();
				}
			}

			event EventHandler IA.MyEvent {
				add {
				}
				remove {
				}
			}
			void IA.Method()
			{
				throw new NotImplementedException();
			}
		}
	}
}
