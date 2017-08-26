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

using System.Diagnostics;
using ICSharpCode.Decompiler.IL;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using System.Threading;

namespace ICSharpCode.Decompiler.CSharp
{
	class StatementBuilder : ILVisitor<Statement>
	{
		internal readonly ExpressionBuilder exprBuilder;
		readonly ILFunction currentFunction;
		readonly IMethod currentMethod;
		readonly CancellationToken cancellationToken;

		public StatementBuilder(IDecompilerTypeSystem typeSystem, ITypeResolveContext decompilationContext, IMethod currentMethod, ILFunction currentFunction, CancellationToken cancellationToken)
		{
			Debug.Assert(typeSystem != null && decompilationContext != null && currentMethod != null);
			this.exprBuilder = new ExpressionBuilder(typeSystem, decompilationContext, cancellationToken);
			this.currentFunction = currentFunction;
			this.currentMethod = currentMethod;
			this.cancellationToken = cancellationToken;
		}
		
		public Statement Convert(ILInstruction inst)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return inst.AcceptVisitor(this);
		}

		public BlockStatement ConvertAsBlock(ILInstruction inst)
		{
			Statement stmt = Convert(inst);
			return stmt as BlockStatement ?? new BlockStatement { stmt };
		}

		protected override Statement Default(ILInstruction inst)
		{
			return new ExpressionStatement(exprBuilder.Translate(inst));
		}
		
		protected internal override Statement VisitNop(Nop inst)
		{
			var stmt = new EmptyStatement();
			if (inst.Comment != null) {
				stmt.AddChild(new Comment(inst.Comment), Roles.Comment);
			}
			return stmt;
		}
		
		protected internal override Statement VisitIfInstruction(IfInstruction inst)
		{
			var condition = exprBuilder.TranslateCondition(inst.Condition);
			var trueStatement = Convert(inst.TrueInst);
			var falseStatement = inst.FalseInst.OpCode == OpCode.Nop ? null : Convert(inst.FalseInst);
			return new IfElseStatement(condition, trueStatement, falseStatement);
		}

		CaseLabel CreateTypedCaseLabel(long i, IType type)
		{
			object value;
			if (type.IsKnownType(KnownTypeCode.Boolean)) {
				value = i != 0;
			} else if (type.Kind == TypeKind.Enum) {
				var enumType = type.GetDefinition().EnumUnderlyingType;
				value = CSharpPrimitiveCast.Cast(ReflectionHelper.GetTypeCode(enumType), i, false);
			} else {
				value = CSharpPrimitiveCast.Cast(ReflectionHelper.GetTypeCode(type), i, false);
			}
			return new CaseLabel(exprBuilder.ConvertConstantValue(new ConstantResolveResult(type, value)));
		}
		
		protected internal override Statement VisitSwitchInstruction(SwitchInstruction inst)
		{
			var oldBreakTarget = breakTarget;
			breakTarget = null; // 'break' within a switch would only leave the switch
			
			var value = exprBuilder.Translate(inst.Value);
			var stmt = new SwitchStatement() { Expression = value };	
			foreach (var section in inst.Sections) {
				var astSection = new Syntax.SwitchSection();
				astSection.CaseLabels.AddRange(section.Labels.Values.Select(i => CreateTypedCaseLabel(i, value.Type)));
				ConvertSwitchSectionBody(astSection, section.Body);
				stmt.SwitchSections.Add(astSection);
			}
			
			if (inst.DefaultBody.OpCode != OpCode.Nop) {
				var astSection = new Syntax.SwitchSection();
				astSection.CaseLabels.Add(new CaseLabel());
				ConvertSwitchSectionBody(astSection, inst.DefaultBody);
				stmt.SwitchSections.Add(astSection);
			}

			breakTarget = oldBreakTarget;
			return stmt;
		}

		private void ConvertSwitchSectionBody(Syntax.SwitchSection astSection, ILInstruction bodyInst)
		{
			var body = Convert(bodyInst);
			astSection.Statements.Add(body);
			if (!bodyInst.HasFlag(InstructionFlags.EndPointUnreachable)) {
				// we need to insert 'break;'
				BlockStatement block = body as BlockStatement;
				if (block != null) {
					block.Add(new BreakStatement());
				} else {
					astSection.Statements.Add(new BreakStatement());
				}
			}
		}
		
		/// <summary>Target block that a 'continue;' statement would jump to</summary>
		Block continueTarget;
		/// <summary>Number of ContinueStatements that were created for the current continueTarget</summary>
		int continueCount;
		
		protected internal override Statement VisitBranch(Branch inst)
		{
			if (inst.TargetBlock == continueTarget) {
				continueCount++;
				return new ContinueStatement();
			}
			return new GotoStatement(inst.TargetLabel);
		}
		
		/// <summary>Target container that a 'break;' statement would break out of</summary>
		BlockContainer breakTarget;
		/// <summary>Dictionary from BlockContainer to label name for 'goto of_container';</summary>
		readonly Dictionary<BlockContainer, string> endContainerLabels = new Dictionary<BlockContainer, string>();
		
		protected internal override Statement VisitLeave(Leave inst)
		{
			if (inst.TargetContainer == breakTarget)
				return new BreakStatement();
			if (inst.IsLeavingFunction) {
				if (currentFunction.IsIterator)
					return new YieldBreakStatement();
				else
					return new ReturnStatement();
			}
			string label;
			if (!endContainerLabels.TryGetValue(inst.TargetContainer, out label)) {
				label = "end_" + inst.TargetLabel;
				endContainerLabels.Add(inst.TargetContainer, label);
			}
			return new GotoStatement(label);
		}
		
		protected internal override Statement VisitThrow(Throw inst)
		{
			return new ThrowStatement(exprBuilder.Translate(inst.Argument));
		}
		
		protected internal override Statement VisitRethrow(Rethrow inst)
		{
			return new ThrowStatement();
		}
		
		protected internal override Statement VisitReturn(Return inst)
		{
			return new ReturnStatement(exprBuilder.Translate(inst.Value).ConvertTo(currentMethod.ReturnType, exprBuilder));
		}

		protected internal override Statement VisitYieldReturn(YieldReturn inst)
		{
			var elementType = currentMethod.ReturnType.GetElementTypeFromIEnumerable(currentMethod.Compilation, true, out var isGeneric);
			return new YieldReturnStatement {
				Expression = exprBuilder.Translate(inst.Value).ConvertTo(elementType, exprBuilder)
			};
		}

		TryCatchStatement MakeTryCatch(ILInstruction tryBlock)
		{
			var tryBlockConverted = Convert(tryBlock);
			var tryCatch = tryBlockConverted as TryCatchStatement;
			if (tryCatch != null && tryCatch.FinallyBlock.IsNull)
				return tryCatch; // extend existing try-catch
			tryCatch = new TryCatchStatement();
			tryCatch.TryBlock = tryBlockConverted as BlockStatement ?? new BlockStatement { tryBlockConverted };
			return tryCatch;
		}
		
		protected internal override Statement VisitTryCatch(TryCatch inst)
		{
			var tryCatch = new TryCatchStatement();
			tryCatch.TryBlock = ConvertAsBlock(inst.TryBlock);
			foreach (var handler in inst.Handlers) {
				var catchClause = new CatchClause();
				var v = handler.Variable;
				if (v != null) {
					if (v.StoreCount > 1 || v.LoadCount > 0 || v.AddressCount > 0) {
						catchClause.VariableName = v.Name;
						catchClause.Type = exprBuilder.ConvertType(v.Type);
					} else if (!v.Type.IsKnownType(KnownTypeCode.Object)) {
						catchClause.Type = exprBuilder.ConvertType(v.Type);
					}
				}
				if (!handler.Filter.MatchLdcI4(1))
					catchClause.Condition = exprBuilder.TranslateCondition(handler.Filter);
				catchClause.Body = ConvertAsBlock(handler.Body);
				tryCatch.CatchClauses.Add(catchClause);
			}
			return tryCatch;
		}
		
		protected internal override Statement VisitTryFinally(TryFinally inst)
		{
			var tryCatch = MakeTryCatch(inst.TryBlock);
			tryCatch.FinallyBlock = ConvertAsBlock(inst.FinallyBlock);
			return tryCatch;
		}
		
		protected internal override Statement VisitTryFault(TryFault inst)
		{
			var tryCatch = new TryCatchStatement();
			tryCatch.TryBlock = ConvertAsBlock(inst.TryBlock);
			var faultBlock = ConvertAsBlock(inst.FaultBlock);
			faultBlock.InsertChildAfter(null, new Comment("try-fault"), Roles.Comment);
			faultBlock.Add(new ThrowStatement());
			tryCatch.CatchClauses.Add(new CatchClause { Body = faultBlock });
			return tryCatch;
		}
		
		protected internal override Statement VisitPinnedRegion(PinnedRegion inst)
		{
			var fixedStmt = new FixedStatement();
			fixedStmt.Type = exprBuilder.ConvertType(inst.Variable.Type);
			Expression initExpr;
			if (inst.Init.OpCode == OpCode.ArrayToPointer) {
				initExpr = exprBuilder.Translate(((ArrayToPointer)inst.Init).Array);
			} else {
				initExpr = exprBuilder.Translate(inst.Init).ConvertTo(inst.Variable.Type, exprBuilder);
			}
			fixedStmt.Variables.Add(new VariableInitializer(inst.Variable.Name, initExpr).WithILVariable(inst.Variable));
			fixedStmt.EmbeddedStatement = Convert(inst.Body);
			return fixedStmt;
		}

		protected internal override Statement VisitBlock(Block block)
		{
			// Block without container
			BlockStatement blockStatement = new BlockStatement();
			foreach (var inst in block.Instructions) {
				blockStatement.Add(Convert(inst));
			}
			if (block.FinalInstruction.OpCode != OpCode.Nop)
				blockStatement.Add(Convert(block.FinalInstruction));
			return blockStatement;
		}

		protected internal override Statement VisitBlockContainer(BlockContainer container)
		{
			if (container.EntryPoint.IncomingEdgeCount > 1) {
				var oldContinueTarget = continueTarget;
				var oldContinueCount = continueCount;
				var oldBreakTarget = breakTarget;
				var loop = ConvertLoop(container);
				loop.AddAnnotation(container);
				continueTarget = oldContinueTarget;
				continueCount = oldContinueCount;
				breakTarget = oldBreakTarget;
				return loop;
			} else {
				return ConvertBlockContainer(container, false);
			}
		}
		
		Statement ConvertLoop(BlockContainer container)
		{
			DetectedLoop loop = DetectedLoop.DetectLoop(container);
			continueCount = 0;
			breakTarget = container;
			continueTarget = loop.ContinueJumpTarget;
			Expression conditionExpr;
			BlockStatement blockStatement;
			switch (loop.Kind) {
				case LoopKind.DoWhile:
					blockStatement = ConvertBlockContainer(new BlockStatement(), loop.Container, loop.AdditionalBlocks, true);
					if (container.EntryPoint.IncomingEdgeCount == continueCount) {
						// Remove the entrypoint label if all jumps to the label were replaced with 'continue;' statements
						blockStatement.Statements.First().Remove();
					}
					if (blockStatement.LastOrDefault() is ContinueStatement continueStmt)
						continueStmt.Remove();
					return new DoWhileStatement {
						EmbeddedStatement = blockStatement,
						Condition = exprBuilder.TranslateCondition(loop.Condition)
					};
				case LoopKind.For:
					conditionExpr = exprBuilder.TranslateCondition(loop.Condition);
					blockStatement = ConvertAsBlock(loop.Body);
					var forBody = ConvertBlockContainer(blockStatement, container, loop.AdditionalBlocks, true);
					var forStmt = new ForStatement() {
						Condition = conditionExpr,
						EmbeddedStatement = forBody
					};
					for (int i = 0; i < loop.IncrementBlock.Instructions.Count - 1; i++) {
						forStmt.Iterators.Add(Convert(loop.IncrementBlock.Instructions[i]));
					}
					if (loop.IncrementBlock.IncomingEdgeCount > continueCount)
						forBody.Add(new LabelStatement { Label = loop.IncrementBlock.Label });
					if (forBody.LastOrDefault() is ContinueStatement continueStmt2)
						continueStmt2.Remove();
					return forStmt;
				default:
				case LoopKind.While:
					if (loop.Body == null) {
						blockStatement = ConvertBlockContainer(container, true);
					} else {
						blockStatement = ConvertAsBlock(loop.Body);
						if (!loop.Body.HasFlag(InstructionFlags.EndPointUnreachable))
							blockStatement.Add(new BreakStatement());
					}
					if (loop.Condition == null) {
						conditionExpr = new PrimitiveExpression(true);
						Debug.Assert(continueCount < container.EntryPoint.IncomingEdgeCount);
						Debug.Assert(blockStatement.Statements.First() is LabelStatement);
						if (container.EntryPoint.IncomingEdgeCount == continueCount + 1) {
							// Remove the entrypoint label if all jumps to the label were replaced with 'continue;' statements
							blockStatement.Statements.First().Remove();
						}
					} else {
						conditionExpr = exprBuilder.TranslateCondition(loop.Condition);
						blockStatement = ConvertBlockContainer(blockStatement, container, loop.AdditionalBlocks, true);
					}
					if (blockStatement.LastOrDefault() is ContinueStatement stmt)
						stmt.Remove();
					return new WhileStatement(conditionExpr, blockStatement);
			}
		}

		BlockStatement ConvertBlockContainer(BlockContainer container, bool isLoop)
		{
			return ConvertBlockContainer(new BlockStatement(), container, container.Blocks, isLoop);
		}

		BlockStatement ConvertBlockContainer(BlockStatement blockStatement, BlockContainer container, IEnumerable<Block> blocks, bool isLoop)
		{
			foreach (var block in blocks) {
				if (block.IncomingEdgeCount > 1 || block != container.EntryPoint) {
					// If there are any incoming branches to this block, add a label:
					blockStatement.Add(new LabelStatement { Label = block.Label });
				}
				foreach (var inst in block.Instructions) {
					if (!isLoop && inst.OpCode == OpCode.Leave && IsFinalLeave((Leave)inst)) {
						// skip the final 'leave' instruction and just fall out of the BlockStatement
						continue;
					}
					blockStatement.Add(Convert(inst));
				}
				if (block.FinalInstruction.OpCode != OpCode.Nop) {
					blockStatement.Add(Convert(block.FinalInstruction));
				}
			}
			string label;
			if (endContainerLabels.TryGetValue(container, out label)) {
				if (isLoop) {
					blockStatement.Add(new ContinueStatement());
				}
				blockStatement.Add(new LabelStatement { Label = label });
				if (isLoop) {
					blockStatement.Add(new BreakStatement());
				}
			}
			return blockStatement;
		}

		static bool IsFinalLeave(Leave leave)
		{
			Block block = (Block)leave.Parent;
			if (leave.ChildIndex != block.Instructions.Count - 1 || block.FinalInstruction.OpCode != OpCode.Nop)
				return false;
			BlockContainer container = (BlockContainer)block.Parent;
			return block.ChildIndex == container.Blocks.Count - 1
				&& container == leave.TargetContainer;
		}
	}
}
