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
using System;
using System.Threading;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;

namespace ICSharpCode.Decompiler.CSharp
{
	class StatementBuilder : ILVisitor<Statement>
	{
		internal readonly ExpressionBuilder exprBuilder;
		readonly ILFunction currentFunction;
		readonly IMethod currentMethod;
		readonly DecompilerSettings settings;
		readonly CancellationToken cancellationToken;

		public StatementBuilder(IDecompilerTypeSystem typeSystem, ITypeResolveContext decompilationContext, IMethod currentMethod, ILFunction currentFunction, DecompilerSettings settings, CancellationToken cancellationToken)
		{
			Debug.Assert(typeSystem != null && decompilationContext != null && currentMethod != null);
			this.exprBuilder = new ExpressionBuilder(typeSystem, decompilationContext, settings, cancellationToken);
			this.currentFunction = currentFunction;
			this.currentMethod = currentMethod;
			this.settings = settings;
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

		ConstantResolveResult CreateTypedCaseLabel(long i, IType type, string[] map = null)
		{
			object value;
			if (type.IsKnownType(KnownTypeCode.Boolean)) {
				value = i != 0;
			} else if (type.IsKnownType(KnownTypeCode.String) && map != null) {
				value = map[i];
			} else if (type.Kind == TypeKind.Enum) {
				var enumType = type.GetDefinition().EnumUnderlyingType;
				value = CSharpPrimitiveCast.Cast(ReflectionHelper.GetTypeCode(enumType), i, false);
			} else if (type.IsKnownType(KnownTypeCode.NullableOfT)) {
				var nullableType = NullableType.GetUnderlyingType(type);
				value = CSharpPrimitiveCast.Cast(ReflectionHelper.GetTypeCode(nullableType), i, false);
			} else {
				value = CSharpPrimitiveCast.Cast(ReflectionHelper.GetTypeCode(type), i, false);
			}
			return new ConstantResolveResult(type, value);
		}

		protected internal override Statement VisitSwitchInstruction(SwitchInstruction inst)
		{
			return TranslateSwitch(null, inst);
		}

		SwitchStatement TranslateSwitch(BlockContainer switchContainer, SwitchInstruction inst)
		{
			Debug.Assert(switchContainer.EntryPoint.IncomingEdgeCount == 1);
			var oldBreakTarget = breakTarget;
			breakTarget = switchContainer; // 'break' within a switch would only leave the switch
			var oldCaseLabelMapping = caseLabelMapping;
			caseLabelMapping = new Dictionary<Block, ConstantResolveResult>();

			TranslatedExpression value;
			var strToInt = inst.Value as StringToInt;
			if (strToInt != null) {
				value = exprBuilder.Translate(strToInt.Argument);
			} else {
				value = exprBuilder.Translate(inst.Value);
			}

			// Pick the section with the most labels as default section.
			IL.SwitchSection defaultSection = inst.Sections.First();
			foreach (var section in inst.Sections) {
				if (section.Labels.Count() > defaultSection.Labels.Count()) {
					defaultSection = section;
				}
			}

			var stmt = new SwitchStatement() { Expression = value };
			Dictionary<IL.SwitchSection, Syntax.SwitchSection> translationDictionary = new Dictionary<IL.SwitchSection, Syntax.SwitchSection>();
			// initialize C# switch sections.
			foreach (var section in inst.Sections) {
				// This is used in the block-label mapping.
				ConstantResolveResult firstValueResolveResult;
				var astSection = new Syntax.SwitchSection();
				// Create case labels:
				if (section == defaultSection) {
					astSection.CaseLabels.Add(new CaseLabel());
					firstValueResolveResult = null;
				} else {
					var values = section.Labels.Values.Select(i => CreateTypedCaseLabel(i, value.Type, strToInt?.Map)).ToArray();
					if (section.HasNullLabel) {
						astSection.CaseLabels.Add(new CaseLabel(new NullReferenceExpression()));
						firstValueResolveResult = new ConstantResolveResult(SpecialType.NullType, null);
					} else {
						Debug.Assert(values.Length > 0);
						firstValueResolveResult = values[0];
					}
					astSection.CaseLabels.AddRange(values.Select(label => new CaseLabel(exprBuilder.ConvertConstantValue(label, allowImplicitConversion: true))));
				}
				switch (section.Body) {
					case Branch br:
						// we can only inline the block, if all branches are in the switchContainer.
						if (br.TargetBlock.Parent == switchContainer && switchContainer.Descendants.OfType<Branch>().Where(b => b.TargetBlock == br.TargetBlock).All(b => BlockContainer.FindClosestSwitchContainer(b) == switchContainer))
							caseLabelMapping.Add(br.TargetBlock, firstValueResolveResult);
						break;
					default:
						break;
				}
				translationDictionary.Add(section, astSection);
				stmt.SwitchSections.Add(astSection);
			}
			foreach (var section in inst.Sections) {
				var astSection = translationDictionary[section];
				switch (section.Body) {
					case Branch br:
						// we can only inline the block, if all branches are in the switchContainer.
						if (br.TargetBlock.Parent == switchContainer && switchContainer.Descendants.OfType<Branch>().Where(b => b.TargetBlock == br.TargetBlock).All(b => BlockContainer.FindClosestSwitchContainer(b) == switchContainer))
							ConvertSwitchSectionBody(astSection, br.TargetBlock);
						else
							ConvertSwitchSectionBody(astSection, section.Body);
						break;
					default:
						ConvertSwitchSectionBody(astSection, section.Body);
						break;
				}
			}
			if (switchContainer != null) {
				// Translate any remaining blocks:
				var lastSectionStatements = stmt.SwitchSections.Last().Statements;
				foreach (var block in switchContainer.Blocks.Skip(1)) {
					if (caseLabelMapping.ContainsKey(block)) continue;
					lastSectionStatements.Add(new LabelStatement { Label = block.Label });
					foreach (var nestedInst in block.Instructions) {
						var nestedStmt = Convert(nestedInst);
						if (nestedStmt is BlockStatement b) {
							foreach (var nested in b.Statements)
								lastSectionStatements.Add(nested.Detach());
						} else {
							lastSectionStatements.Add(nestedStmt);
						}
					}
					Debug.Assert(block.FinalInstruction.OpCode == OpCode.Nop);
				}
				if (endContainerLabels.TryGetValue(switchContainer, out string label)) {
					lastSectionStatements.Add(new LabelStatement { Label = label });
					lastSectionStatements.Add(new BreakStatement());
				}
			}

			breakTarget = oldBreakTarget;
			caseLabelMapping = oldCaseLabelMapping;
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
		/// <summary>Maps blocks to cases.</summary>
		Dictionary<Block, ConstantResolveResult> caseLabelMapping;
		
		protected internal override Statement VisitBranch(Branch inst)
		{
			if (inst.TargetBlock == continueTarget) {
				continueCount++;
				return new ContinueStatement();
			}
			if (caseLabelMapping != null && caseLabelMapping.TryGetValue(inst.TargetBlock, out var label)) {
				if (label == null)
					return new GotoDefaultStatement();
				return new GotoCaseStatement() { LabelExpression = exprBuilder.ConvertConstantValue(label, allowImplicitConversion: true) };
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
				else if (!inst.Value.MatchNop()) {
					IType targetType = currentFunction.IsAsync ? currentFunction.AsyncReturnType : currentMethod.ReturnType;
					return new ReturnStatement(exprBuilder.Translate(inst.Value).ConvertTo(targetType, exprBuilder, allowImplicitConversion: true));
				} else
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
				catchClause.AddAnnotation(new ILVariableResolveResult(v, v.Type));
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

		protected internal override Statement VisitLockInstruction(LockInstruction inst)
		{
			return new LockStatement {
				Expression = exprBuilder.Translate(inst.OnExpression),
				EmbeddedStatement = ConvertAsBlock(inst.Body)
			};
		}

		#region foreach construction
		static readonly InvocationExpression getEnumeratorPattern = new InvocationExpression(new MemberReferenceExpression(new AnyNode("collection").ToExpression(), "GetEnumerator"));
		static readonly InvocationExpression moveNextConditionPattern = new InvocationExpression(new MemberReferenceExpression(new NamedNode("enumerator", new IdentifierExpression(Pattern.AnyString)), "MoveNext"));

		protected internal override Statement VisitUsingInstruction(UsingInstruction inst)
		{
			var transformed = TransformToForeach(inst, out var resource);
			if (transformed != null)
				return transformed;
			AstNode usingInit = resource;
			var var = inst.Variable;
			if (!inst.ResourceExpression.MatchLdNull() && !NullableType.GetUnderlyingType(var.Type).GetAllBaseTypes().Any(b => b.IsKnownType(KnownTypeCode.IDisposable))) {
				var.Kind = VariableKind.Local;
				var disposeType = exprBuilder.compilation.FindType(KnownTypeCode.IDisposable);
				var disposeVariable = currentFunction.RegisterVariable(
					VariableKind.Local, disposeType,
					AssignVariableNames.GenerateVariableName(currentFunction, disposeType)
				);
				return new BlockStatement {
					new ExpressionStatement(new AssignmentExpression(exprBuilder.ConvertVariable(var).Expression, resource.Detach())),
					new TryCatchStatement {
						TryBlock = ConvertAsBlock(inst.Body),
						FinallyBlock = new BlockStatement() {
							new ExpressionStatement(new AssignmentExpression(exprBuilder.ConvertVariable(disposeVariable).Expression, new AsExpression(exprBuilder.ConvertVariable(var).Expression, exprBuilder.ConvertType(disposeType)))),
							new IfElseStatement {
								Condition = new BinaryOperatorExpression(exprBuilder.ConvertVariable(disposeVariable), BinaryOperatorType.InEquality, new NullReferenceExpression()),
								TrueStatement = new ExpressionStatement(new InvocationExpression(new MemberReferenceExpression(exprBuilder.ConvertVariable(disposeVariable).Expression, "Dispose")))
							}
						}
					},
				};
			} else {
				if (var.LoadCount > 0 || var.AddressCount > 0) {
					var type = settings.AnonymousTypes && var.Type.ContainsAnonymousType() ? new SimpleType("var") : exprBuilder.ConvertType(var.Type);
					var vds = new VariableDeclarationStatement(type, var.Name, resource);
					vds.Variables.Single().AddAnnotation(new ILVariableResolveResult(var, var.Type));
					usingInit = vds;
				}
				return new UsingStatement {
					ResourceAcquisition = usingInit,
					EmbeddedStatement = ConvertAsBlock(inst.Body)
				};
			}
		}

		Statement TransformToForeach(UsingInstruction inst, out Expression resource)
		{
			// Check if the using resource matches the GetEnumerator pattern.
			resource = exprBuilder.Translate(inst.ResourceExpression);
			var m = getEnumeratorPattern.Match(resource);
			// The using body must be a BlockContainer.
			if (!(inst.Body is BlockContainer container) || !m.Success)
				return null;
			// The using-variable is the enumerator.
			var enumeratorVar = inst.Variable;
			// If there's another BlockContainer nested in this container and it only has one child block, unwrap it.
			// If there's an extra leave inside the block, extract it into optionalReturnAfterLoop.
			var loopContainer = UnwrapNestedContainerIfPossible(container, out var optionalReturnAfterLoop);
			// Detect whether we're dealing with a while loop with multiple embedded statements.
			var loop = DetectedLoop.DetectLoop(loopContainer);
			if (loop.Kind != LoopKind.While || !(loop.Body is Block body))
				return null;
			// The loop condition must be a call to enumerator.MoveNext()
			var condition = exprBuilder.TranslateCondition(loop.Conditions.Single());
			var m2 = moveNextConditionPattern.Match(condition.Expression);
			if (!m2.Success)
				return null;
			// Check enumerator variable references.
			var enumeratorVar2 = m2.Get<IdentifierExpression>("enumerator").Single().GetILVariable();
			if (enumeratorVar2 != enumeratorVar)
				return null;
			// Detect which foreach-variable transformation is necessary/possible.
			var transformation = DetectGetCurrentTransformation(container, body, enumeratorVar, condition.ILInstructions.Single(),
																out var singleGetter, out var foreachVariable);
			if (transformation == RequiredGetCurrentTransformation.NoForeach)
				return null;
			// The existing foreach variable, if found, can only be used in the loop container.
			if (foreachVariable != null && !(foreachVariable.CaptureScope == null || foreachVariable.CaptureScope == loopContainer))
				return null;
			// Extract in-expression
			var collectionExpr = m.Get<Expression>("collection").Single();
			// Special case: foreach (var item in this) is decompiled as foreach (var item in base)
			// but a base reference is not valid in this context.
			if (collectionExpr is BaseReferenceExpression) {
				collectionExpr = new ThisReferenceExpression().CopyAnnotationsFrom(collectionExpr);
			}
			// Handle explicit casts:
			// This is the case if an explicit type different from the collection-item-type was used.
			// For example: foreach (ClassA item in nonGenericEnumerable)
			var type = singleGetter.Method.ReturnType;
			ILInstruction instToReplace = singleGetter;
			switch (instToReplace.Parent) {
				case CastClass cc:
					type = cc.Type;
					instToReplace = cc;
					break;
				case UnboxAny ua:
					type = ua.Type;
					instToReplace = ua;
					break;
			}
			// Handle the required foreach-variable transformation:
			switch (transformation) {
				case RequiredGetCurrentTransformation.UseExistingVariable:
					foreachVariable.Type = type;
					foreachVariable.Kind = VariableKind.ForeachLocal;
					foreachVariable.Name = AssignVariableNames.GenerateForeachVariableName(currentFunction, collectionExpr.Annotation<ILInstruction>(), foreachVariable);
					break;
				case RequiredGetCurrentTransformation.UninlineAndUseExistingVariable:
					// Unwrap stloc chain.
					var nestedStores = new Stack<ILVariable>();
					var currentInst = instToReplace; // instToReplace is the innermost value of the stloc chain.
					while (currentInst.Parent is StLoc stloc) {
						// Exclude nested stores to foreachVariable
						// we'll insert one store at the beginning of the block.
						if (stloc.Variable != foreachVariable && stloc.Parent is StLoc)
							nestedStores.Push(stloc.Variable);
						currentInst = stloc;
					}
					// Rebuild the nested store instructions:
					ILInstruction reorderedStores = new LdLoc(foreachVariable);
					while (nestedStores.Count > 0) {
						reorderedStores = new StLoc(nestedStores.Pop(), reorderedStores);
					}
					currentInst.ReplaceWith(reorderedStores);
					body.Instructions.Insert(0, new StLoc(foreachVariable, instToReplace));
					// Adjust variable type, kind and name.
					goto case RequiredGetCurrentTransformation.UseExistingVariable;
				case RequiredGetCurrentTransformation.IntroduceNewVariable:
					foreachVariable = currentFunction.RegisterVariable(
						VariableKind.ForeachLocal, type,
						AssignVariableNames.GenerateForeachVariableName(currentFunction, collectionExpr.Annotation<ILInstruction>())
					);
					instToReplace.ReplaceWith(new LdLoc(foreachVariable));
					body.Instructions.Insert(0, new StLoc(foreachVariable, instToReplace));
					break;
			}
			// Convert the modified body to C# AST:
			var whileLoop = (WhileStatement)ConvertAsBlock(container).First();
			BlockStatement foreachBody = (BlockStatement)whileLoop.EmbeddedStatement.Detach();
			// Remove the first statement, as it is the foreachVariable = enumerator.Current; statement.
			foreachBody.Statements.First().Detach();
			// Construct the foreach loop.
			var foreachStmt = new ForeachStatement {
				VariableType = settings.AnonymousTypes && foreachVariable.Type.ContainsAnonymousType() ? new SimpleType("var") : exprBuilder.ConvertType(foreachVariable.Type),
				VariableName = foreachVariable.Name,
				InExpression = collectionExpr.Detach(),
				EmbeddedStatement = foreachBody
			};
			// Add the variable annotation for highlighting (TokenTextWriter expects it directly on the ForeachStatement).
			foreachStmt.AddAnnotation(new ILVariableResolveResult(foreachVariable, foreachVariable.Type));
			// If there was an optional return statement, return it as well.
			if (optionalReturnAfterLoop != null) {
				return new BlockStatement {
					Statements = {
						foreachStmt,
						optionalReturnAfterLoop.AcceptVisitor(this)
					}
				};
			}
			return foreachStmt;
		}

		/// <summary>
		/// Unwraps a nested BlockContainer, if container contains only a single block,
		/// and that single block contains only a BlockContainer followed by a Leave instruction.
		/// If the leave instruction is a return that carries a value, the container is unwrapped only
		/// if the value has no side-effects.
		/// Otherwise returns the unmodified container.
		/// </summary>
		/// <param name="optionalReturnInst">If the leave is a return and has no side-effects, we can move the return out of the using-block and put it after the loop, otherwise returns null.</param>
		BlockContainer UnwrapNestedContainerIfPossible(BlockContainer container, out Leave optionalReturnInst)
		{
			optionalReturnInst = null;
			// Check block structure:
			if (container.Blocks.Count != 1)
				return container;
			var nestedBlock = container.Blocks[0];
			if (nestedBlock.Instructions.Count != 2 ||
				!(nestedBlock.Instructions[0] is BlockContainer nestedContainer) ||
				!(nestedBlock.Instructions[1] is Leave leave))
				return container;
			// If the leave has no value, just unwrap the BlockContainer.
			if (leave.MatchLeave(container))
				return nestedContainer;
			// If the leave is a return, we can move the return out of the using-block and put it after the loop
			// (but only if the value doesn't have side-effects)
			if (leave.IsLeavingFunction && SemanticHelper.IsPure(leave.Value.Flags)) {
				optionalReturnInst = leave;
				return nestedContainer;
			}
			return container;
		}

		enum RequiredGetCurrentTransformation
		{
			/// <summary>
			/// Foreach transformation not possible.
			/// </summary>
			NoForeach,
			/// <summary>
			/// Uninline the stloc foreachVar(call get_Current()) and insert it as first statement in the loop body.
			/// <code>
			///	... (stloc foreachVar(call get_Current()) ...
			///	=>
			///	stloc foreachVar(call get_Current())
			///	... (ldloc foreachVar) ...
			/// </code>
			/// </summary>
			UseExistingVariable,
			/// <summary>
			/// Uninline (and possibly reorder) multiple stloc instructions and insert stloc foreachVar(call get_Current()) as first statement in the loop body.
			/// <code>
			///	... (stloc foreachVar(stloc otherVar(call get_Current())) ...
			///	=>
			///	stloc foreachVar(call get_Current())
			///	... (stloc otherVar(ldloc foreachVar)) ...
			/// </code>
			/// </summary>
			UninlineAndUseExistingVariable,
			/// <summary>
			/// No store was found, thus create a new variable and use it as foreach variable.
			/// <code>
			///	... (call get_Current()) ...
			///	=>
			///	stloc foreachVar(call get_Current())
			///	... (ldloc foreachVar) ...
			/// </code>
			/// </summary>
			IntroduceNewVariable
		}

		/// <summary>
		/// Determines whether <paramref name="enumerator"/> is only used once inside <paramref name="loopBody"/> for accessing the Current property.
		/// </summary>
		/// <param name="usingContainer">The using body container. This is only used for variable usage checks.</param>
		/// <param name="loopBody">The loop body. The first statement of this block is analyzed.</param>
		/// <param name="enumerator">The current enumerator.</param>
		/// <param name="moveNextUsage">The call MoveNext(ldloc enumerator) pattern.</param>
		/// <param name="singleGetter">Returns the call instruction invoking Current's getter.</param>
		/// <param name="foreachVariable">Returns the the foreach variable, if a suitable was found. This variable is only assigned once and its assignment is the first statement in <paramref name="loopBody"/>.</param>
		/// <returns><see cref="RequiredGetCurrentTransformation"/> for details.</returns>
		RequiredGetCurrentTransformation DetectGetCurrentTransformation(BlockContainer usingContainer, Block loopBody, ILVariable enumerator, ILInstruction moveNextUsage, out CallInstruction singleGetter, out ILVariable foreachVariable)
		{
			singleGetter = null;
			foreachVariable = null;
			var loads = (enumerator.LoadInstructions.OfType<ILInstruction>().Concat(enumerator.AddressInstructions.OfType<ILInstruction>())).Where(ld => !ld.IsDescendantOf(moveNextUsage)).ToArray();
			// enumerator is used in multiple locations or not in conjunction with get_Current
			// => no foreach
			if (loads.Length != 1 || !ParentIsCurrentGetter(loads[0]))
				return RequiredGetCurrentTransformation.NoForeach;
			singleGetter = (CallInstruction)loads[0].Parent;
			// singleGetter is not part of the first instruction in body or cannot be uninlined
			// => no foreach
			if (!(singleGetter.IsDescendantOf(loopBody.Instructions[0]) && ILInlining.CanUninline(singleGetter, loopBody.Instructions[0])))
				return RequiredGetCurrentTransformation.NoForeach;
			ILInstruction inst = singleGetter;
			// in some cases, i.e. foreach variable with explicit type different from the collection-item-type,
			// the result of call get_Current is casted.
			while (inst.Parent is UnboxAny || inst.Parent is CastClass)
				inst = inst.Parent;
			// Gather all nested assignments to determine the foreach variable.
			List<StLoc> nestedStores = new List<StLoc>();
			while (inst.Parent is StLoc stloc) {
				nestedStores.Add(stloc);
				inst = stloc;
			}
			// No variable was found: we need a new one.
			if (nestedStores.Count == 0)
				return RequiredGetCurrentTransformation.IntroduceNewVariable;
			// One variable was found.
			if (nestedStores.Count == 1) {
				// Must be a plain assignment expression and variable must only be used in 'body' + only assigned once.
				if (nestedStores[0].Parent == loopBody && VariableIsOnlyUsedInBlock(nestedStores[0], usingContainer)) {
					foreachVariable = nestedStores[0].Variable;
					return RequiredGetCurrentTransformation.UseExistingVariable;
				}
			} else {
				// Check if any of the variables is usable as foreach variable.
				foreach (var store in nestedStores) {
					if (VariableIsOnlyUsedInBlock(store, usingContainer)) {
						foreachVariable = store.Variable;
						return RequiredGetCurrentTransformation.UninlineAndUseExistingVariable;
					}
				}
			}
			// No suitable variable found.
			return RequiredGetCurrentTransformation.IntroduceNewVariable;
		}

		/// <summary>
		/// Determines whether storeInst.Variable is only assigned once and used only inside <paramref name="usingContainer"/>.
		/// Loads by reference (ldloca) are only allowed in the context of this pointer in call instructions.
		/// (This only applies to value types.)
		/// </summary>
		bool VariableIsOnlyUsedInBlock(StLoc storeInst, BlockContainer usingContainer)
		{
			if (storeInst.Variable.LoadInstructions.Any(ld => !ld.IsDescendantOf(usingContainer)))
				return false;
			if (storeInst.Variable.AddressInstructions.Any(la => !la.IsDescendantOf(usingContainer) || !ILInlining.IsUsedAsThisPointerInCall(la)))
				return false;
			if (storeInst.Variable.StoreInstructions.OfType<ILInstruction>().Any(st => st != storeInst))
				return false;
			return true;
		}

		bool ParentIsCurrentGetter(ILInstruction inst)
		{
			return inst.Parent is CallInstruction cv && cv.Method.IsAccessor &&
				cv.Method.AccessorOwner is IProperty p && p.Getter.Equals(cv.Method);
		}
		#endregion

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
			} else if (container.EntryPoint.Instructions.Count == 1 && container.EntryPoint.Instructions[0] is SwitchInstruction switchInst) {
				return TranslateSwitch(container, switchInst);
			} else {
				var blockStmt = ConvertBlockContainer(container, false);
				blockStmt.AddAnnotation(container);
				return blockStmt;
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
						Condition = exprBuilder.TranslateCondition(CombineConditions(loop.Conditions))
					};
				case LoopKind.For:
					conditionExpr = exprBuilder.TranslateCondition(loop.Conditions[0]);
					blockStatement = ConvertAsBlock(loop.Body);
					if (!loop.Body.HasFlag(InstructionFlags.EndPointUnreachable))
						blockStatement.Add(new BreakStatement());
					Statement iterator = null;
					if (loop.IncrementBlock == null) {
						// increment check is done by DetectLoop
						var statement = blockStatement.Last();
						while (statement is ContinueStatement)
							statement = (Statement)statement.PrevSibling;
						iterator = statement.Detach();
					}
					var forBody = ConvertBlockContainer(blockStatement, container, loop.AdditionalBlocks, true);
					var forStmt = new ForStatement() {
						Condition = conditionExpr,
						EmbeddedStatement = forBody
					};
					if (forBody.LastOrDefault() is ContinueStatement continueStmt2)
						continueStmt2.Remove();
					if (loop.IncrementBlock != null) {
						for (int i = 0; i < loop.IncrementBlock.Instructions.Count - 1; i++) {
							forStmt.Iterators.Add(Convert(loop.IncrementBlock.Instructions[i]));
						}
						if (loop.IncrementBlock.IncomingEdgeCount > continueCount)
							forBody.Add(new LabelStatement { Label = loop.IncrementBlock.Label });
					} else if (iterator != null) {
						forStmt.Iterators.Add(iterator.Detach());
					}
					return forStmt;
				case LoopKind.While:
					if (loop.Body == null) {
						blockStatement = ConvertBlockContainer(container, true);
					} else {
						blockStatement = ConvertAsBlock(loop.Body);
						if (!loop.Body.HasFlag(InstructionFlags.EndPointUnreachable))
							blockStatement.Add(new BreakStatement());
					}
					if (loop.Conditions == null) {
						conditionExpr = new PrimitiveExpression(true);
						Debug.Assert(continueCount < container.EntryPoint.IncomingEdgeCount);
						Debug.Assert(blockStatement.Statements.First() is LabelStatement);
						if (container.EntryPoint.IncomingEdgeCount == continueCount + 1) {
							// Remove the entrypoint label if all jumps to the label were replaced with 'continue;' statements
							blockStatement.Statements.First().Remove();
						}
					} else {
						conditionExpr = exprBuilder.TranslateCondition(loop.Conditions[0]);
						blockStatement = ConvertBlockContainer(blockStatement, container, loop.AdditionalBlocks, true);
					}
					if (blockStatement.LastOrDefault() is ContinueStatement stmt)
						stmt.Remove();
					return new WhileStatement(conditionExpr, blockStatement);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private ILInstruction CombineConditions(ILInstruction[] conditions)
		{
			ILInstruction condition = null;
			foreach (var c in conditions) {
				if (condition == null)
					condition = Comp.LogicNot(c);
				else
					condition = IfInstruction.LogicAnd(Comp.LogicNot(c), condition);
			}
			return condition;
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
					var stmt = Convert(inst);
					if (stmt is BlockStatement b) {
						foreach (var nested in b.Statements)
							blockStatement.Add(nested.Detach());
					} else {
						blockStatement.Add(stmt);
					}
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
			if (!leave.Value.MatchNop())
				return false;
			Block block = (Block)leave.Parent;
			if (leave.ChildIndex != block.Instructions.Count - 1 || block.FinalInstruction.OpCode != OpCode.Nop)
				return false;
			BlockContainer container = (BlockContainer)block.Parent;
			return block.ChildIndex == container.Blocks.Count - 1
				&& container == leave.TargetContainer;
		}

		protected internal override Statement VisitInitblk(Initblk inst)
		{
			var stmt = new ExpressionStatement(new InvocationExpression {
				Target = new IdentifierExpression("memset"),
				Arguments = {
					exprBuilder.Translate(inst.Address),
					exprBuilder.Translate(inst.Value),
					exprBuilder.Translate(inst.Size)
				}
			});
			stmt.AddChild(new Comment(" IL initblk instruction"), Roles.Comment);
			return stmt;
		}

		protected internal override Statement VisitCpblk(Cpblk inst)
		{
			var stmt = new ExpressionStatement(new InvocationExpression {
				Target = new IdentifierExpression("memcpy"),
				Arguments = {
					exprBuilder.Translate(inst.DestAddress),
					exprBuilder.Translate(inst.SourceAddress),
					exprBuilder.Translate(inst.Size)
				}
			});
			stmt.AddChild(new Comment(" IL cpblk instruction"), Roles.Comment);
			return stmt;
		}
	}
}
