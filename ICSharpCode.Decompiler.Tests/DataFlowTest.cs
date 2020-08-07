// Copyright (c) 2016 Daniel Grunwald
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.FlowAnalysis;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.TypeSystem;
using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture]
	class DataFlowTest
	{
		class RDTest : ReachingDefinitionsVisitor
		{
			ILVariable v;

			public RDTest(ILFunction f, ILVariable v) : base(f, _ => true, CancellationToken.None)
			{
				this.v = v;
			}

			protected internal override void VisitTryFinally(TryFinally inst)
			{
				Assert.IsTrue(IsPotentiallyUninitialized(state, v));
				base.VisitTryFinally(inst);
				Assert.IsTrue(state.IsReachable);
				Assert.AreEqual(1, GetStores(state, v).Count());
				Assert.IsFalse(IsPotentiallyUninitialized(state, v));
			}
		}

		[Test]
		public void TryFinallyWithAssignmentInFinally()
		{
			ILVariable v = new ILVariable(VariableKind.Local, SpecialType.UnknownType, 0);
			ILFunction f = new ILFunction(
				returnType: SpecialType.UnknownType,
				parameters: new IParameter[0],
				genericContext: new GenericContext(),
				body: new TryFinally(
					new Nop(),
					new StLoc(v, new LdcI4(0))
				));
			f.AddRef();
			f.Variables.Add(v);
			f.Body.AcceptVisitor(new RDTest(f, v));
		}
	}
}
