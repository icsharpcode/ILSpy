using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NUnit.Framework;
using ICSharpCode.Decompiler.Tests.Helpers;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture]
	public class StackToVariablesTests
	{
		ILVariable StackSlot<T>(int index)
		{
			return new ILVariable(VariableKind.StackSlot, TypeSystem.FromReflection(typeof(T)), index) {
				Name = "S_" + index
			};
		}
		
		[Test]
		public void Test1()
		{
			Block input = new Block {
				Instructions = {
					new LdcI4(1),
					new LdcI4(2),
					new LdcI4(3),
					new LdcI4(4),
					new Call(TypeSystem.Action<int, int, int, int>()) {
						Arguments = {
							new Pop(StackType.I4),
							new Block { FinalInstruction = new Pop(StackType.I4) },
							new Block { FinalInstruction = new Pop(StackType.I4) },
							new Pop(StackType.I4)
						}
					}
				}
			};
			// F(3, 2, 1, 4)
			Block expected = new Block {
				Instructions = {
					new Void(new StLoc(new LdcI4(1), StackSlot<int>(0))),
					new Void(new StLoc(new LdcI4(2), StackSlot<int>(1))),
					new Void(new StLoc(new LdcI4(3), StackSlot<int>(2))),
					new Void(new StLoc(new LdcI4(4), StackSlot<int>(3))),
					new Call(TypeSystem.Action<int, int, int, int>()) {
						Arguments = {
							new LdLoc(StackSlot<int>(2)),
							new Block { FinalInstruction = new LdLoc(StackSlot<int>(1)) },
							new Block { FinalInstruction = new LdLoc(StackSlot<int>(0)) },
							new LdLoc(StackSlot<int>(3))
						}
					}
				}
			};
			TestStackIntoVariablesTransform(input, expected);
		}
		
		[Test]
		public void Test2()
		{
			Block input = new Block {
				Instructions = {
					new LdcI4(1),
					new LdcI4(2),
					new Add(new Pop(StackType.I4), new Pop(StackType.I4), false, Sign.Signed)
				}
			};
			Block expected = new Block {
				Instructions = {
					new Void(new StLoc(new LdcI4(1), StackSlot<int>(0))),
					new Void(new StLoc(new LdcI4(2), StackSlot<int>(1))),
					new Void(new StLoc(new Add(new LdLoc(StackSlot<int>(0)), new LdLoc(StackSlot<int>(1)), false, Sign.Signed), StackSlot<int>(2)))
				}
			};
			TestStackIntoVariablesTransform(input, expected);
		}

		void TestStackIntoVariablesTransform(Block input, Block expected)
		{
			input.AddRef();
			ILFunction function = new ILFunction(null, input);
			var context = new ILTransformContext { TypeSystem = TypeSystem.Instance };
			new TransformStackIntoVariables().Run(function, context);
			Assert.AreEqual(expected.ToString(), input.ToString());
		}
	}
}
