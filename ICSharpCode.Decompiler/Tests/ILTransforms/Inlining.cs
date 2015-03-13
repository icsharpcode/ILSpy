// Copyright (c) 2014 Daniel Grunwald
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
using System.Diagnostics;
using ICSharpCode.Decompiler.IL;
using NUnit.Framework;
using ICSharpCode.Decompiler.Tests.Helpers;

namespace ICSharpCode.Decompiler.Tests.ILTransforms
{
	/// <summary>
	/// IL Inlining unit tests.
	/// </summary>
	public class Inlining
	{
		static ILFunction MakeFunction(ILInstruction body)
		{
			var f = new ILFunction(null, body);
			f.AddRef();
			return f;
		}
		
		[Test]
		public void Simple()
		{
			var f = MakeFunction(
				new Block {
					Instructions = {
						new LdcI4(1),
						new LdcI4(2),
						new Add(new Pop(StackType.I4), new Pop(StackType.I4), false, Sign.Signed)
					}
				});
			f.AcceptVisitor(new TransformingVisitor());
			Assert.AreEqual(
				new Block {
					Instructions = {
						new Add(new LdcI4(1), new LdcI4(2), false, Sign.Signed)
					}
				}.ToString(),
				f.Body.ToString()
			);
		}
		
		[Test]
		public void SkipInlineBlocks()
		{
			var f = MakeFunction(
				new Block {
					Instructions = {
						new LdcI4(1),
						new LdcI4(2),
						new LdcI4(3),
						new Call(TypeSystem.Action<int, int, int>()) {
							Arguments = {
								new Pop(StackType.I4),
								new Block { FinalInstruction = new Pop(StackType.I4) },
								new Pop(StackType.I4),
							}
						}
					}
				});
			f.AcceptVisitor(new TransformingVisitor());
			Assert.AreEqual(
				new Block {
					Instructions = {
						new LdcI4(1),
						new Call(TypeSystem.Action<int, int, int>()) {
							Arguments = {
								new LdcI4(2),
								new Block { FinalInstruction = new Pop(StackType.I4) },
								new LdcI4(3)
							}
						}
					}
				}.ToString(),
				f.Body.ToString()
			);
		}
		
		[Test]
		public void BuildInlineBlock()
		{
			var f = MakeFunction(
				new Block {
					Instructions = {
						new LdcI4(1),
						new LdcI4(2),
						new Call(TypeSystem.Action<int>()) { Arguments = { new Peek(StackType.I4) } },
						new Call(TypeSystem.Action<int>()) { Arguments = { new Peek(StackType.I4) } },
						new Call(TypeSystem.Action<int, int>()) { Arguments = { new Pop(StackType.I4), new Pop(StackType.I4) } }
					}
				});
			f.AcceptVisitor(new TransformingVisitor());
			Debug.WriteLine(f.ToString());
			Assert.AreEqual(
				new Call(TypeSystem.Action<int, int>()) {
					Arguments = {
						new LdcI4(1),
						new Block {
							Instructions = {
								new LdcI4(2),
								new Call(TypeSystem.Action<int>()) { Arguments = { new Peek(StackType.I4) } },
								new Call(TypeSystem.Action<int>()) { Arguments = { new Peek(StackType.I4) } },
							},
							FinalInstruction = new Pop(StackType.I4)
						}
					}
				}.ToString(),
				f.Body.ToString()
			);
		}
	}
}
