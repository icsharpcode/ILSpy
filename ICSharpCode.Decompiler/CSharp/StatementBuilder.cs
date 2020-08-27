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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp
{
	sealed class StatementBuilder : ILVisitor<TranslatedStatement>
	{
		internal readonly ExpressionBuilder exprBuilder;
		readonly ILFunction currentFunction;
		readonly IDecompilerTypeSystem typeSystem;
		readonly DecompilerSettings settings;
		readonly CancellationToken cancellationToken;

		internal BlockContainer currentReturnContainer;
		internal IType currentResultType;
		internal bool currentIsIterator;

		public StatementBuilder(IDecompilerTypeSystem typeSystem, ITypeResolveContext decompilationContext, ILFunction currentFunction, DecompilerSettings settings, CancellationToken cancellationToken)
		{
			Debug.Assert(typeSystem != null && decompilationContext != null);
			this.exprBuilder = new ExpressionBuilder(this, typeSystem, decompilationContext, currentFunction, settings, cancellationToken);
			this.currentFunction = currentFunction;
			this.currentReturnContainer = (BlockContainer)currentFunction.Body;
			this.currentIsIterator = currentFunction.IsIterator;
			this.currentResultType = currentFunction.IsAsync ? currentFunction.AsyncReturnType : currentFunction.ReturnType;
			this.typeSystem = typeSystem;
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
			Statement stmt = Convert(inst).WithILInstruction(inst);
			return stmt as BlockStatement ?? new BlockStatement { stmt };
		}

		protected override TranslatedStatement Default(ILInstruction inst)
		{
			return new ExpressionStatement(exprBuilder.Translate(inst))
				.WithILInstruction(inst);
		}

		protected internal override TranslatedStatement VisitIsInst(IsInst inst)
		{
			// isinst on top-level (unused result) can be translated in general
			// (even for value types) by using "is" instead of "as"
			// This can happen when the result of "expr is T" is unused
			// and the C# compiler optimizes away the null check portion of the "is" operator.
			var arg = exprBuilder.Translate(inst.Argument);
			arg = ExpressionBuilder.UnwrapBoxingConversion(arg);
			return new ExpressionStatement(
				new IsExpression(
					arg,
					exprBuilder.ConvertType(inst.Type)
				)
				.WithRR(new ResolveResult(exprBuilder.compilation.FindType(KnownTypeCode.Boolean)))
				.WithILInstruction(inst)
			)
			.WithILInstruction(inst);
		}

		protected internal override TranslatedStatement VisitStLoc(StLoc inst)
		{
			var expr = exprBuilder.Translate(inst);
			// strip top-level ref on ref re-assignment
			if (expr.Expression is DirectionExpression dirExpr)
			{
				expr = expr.UnwrapChild(dirExpr.Expression);
			}
			return new ExpressionStatement(expr).WithILInstruction(inst);
		}

		protected internal override TranslatedStatement VisitNop(Nop inst)
		{
			var stmt = new EmptyStatement();
			if (inst.Comment != null)
			{
				stmt.AddChild(new Comment(inst.Comment), Roles.Comment);
			}
			return stmt.WithILInstruction(inst);
		}

		protected internal override TranslatedStatement VisitIfInstruction(IfInstruction inst)
		{
			var condition = exprBuilder.TranslateCondition(inst.Condition);
			var trueStatement = Convert(inst.TrueInst);
			var falseStatement = inst.FalseInst.OpCode == OpCode.Nop ? null : Convert(inst.FalseInst);
			return new IfElseStatement(condition, trueStatement, falseStatement).WithILInstruction(inst);
		}

		internal IEnumerable<ConstantResolveResult> CreateTypedCaseLabel(long i, IType type, List<(string Key, int Value)> map = null)
		{
			object value;
			// unpack nullable type, if necessary:
			// we need to do this in all cases, because there are nullable bools and enum types as well.
			type = NullableType.GetUnderlyingType(type);
			if (type.IsKnownType(KnownTypeCode.Boolean))
			{
				value = i != 0;
			}
			else if (type.IsKnownType(KnownTypeCode.String) && map != null)
			{
				var keys = map.Where(entry => entry.Value == i).Select(entry => entry.Key);
				foreach (var key in keys)
					yield return new ConstantResolveResult(type, key);
				yield break;
			}
			else if (type.Kind == TypeKind.Enum)
			{
				var enumType = type.GetDefinition().EnumUnderlyingType;
				TypeCode typeCode = ReflectionHelper.GetTypeCode(enumType);
				if (typeCode != TypeCode.Empty)
				{
					value = CSharpPrimitiveCast.Cast(typeCode, i, false);
				}
				else
				{
					value = i;
				}
			}
			else
			{
				TypeCode typeCode = ReflectionHelper.GetTypeCode(type);
				if (typeCode != TypeCode.Empty)
				{
					value = CSharpPrimitiveCast.Cast(typeCode, i, false);
				}
				else
				{
					value = i;
				}
			}
			yield return new ConstantResolveResult(type, value);
		}

		protected internal override TranslatedStatement VisitSwitchInstruction(SwitchInstruction inst)
		{
			return TranslateSwitch(null, inst).WithILInstruction(inst);
		}

		SwitchStatement TranslateSwitch(BlockContainer switchContainer, SwitchInstruction inst)
		{
			var oldBreakTarget = breakTarget;
			breakTarget = switchContainer; // 'break' within a switch would only leave the switch
			var oldCaseLabelMapping = caseLabelMapping;
			caseLabelMapping = new Dictionary<Block, ConstantResolveResult>();

			TranslatedExpression value;
			if (inst.Value is StringToInt strToInt)
			{
				value = exprBuilder.Translate(strToInt.Argument);
			}
			else
			{
				strToInt = null;
				value = exprBuilder.Translate(inst.Value);
			}

			IL.SwitchSection defaultSection = inst.GetDefaultSection();

			var stmt = new SwitchStatement() { Expression = value };
			Dictionary<IL.SwitchSection, Syntax.SwitchSection> translationDictionary = new Dictionary<IL.SwitchSection, Syntax.SwitchSection>();
			// initialize C# switch sections.
			foreach (var section in inst.Sections)
			{
				// This is used in the block-label mapping.
				ConstantResolveResult firstValueResolveResult;
				var astSection = new Syntax.SwitchSection();
				// Create case labels:
				if (section == defaultSection)
				{
					astSection.CaseLabels.Add(new CaseLabel());
					firstValueResolveResult = null;
				}
				else
				{
					var values = section.Labels.Values.SelectMany(i => CreateTypedCaseLabel(i, value.Type, strToInt?.Map)).ToArray();
					if (section.HasNullLabel)
					{
						astSection.CaseLabels.Add(new CaseLabel(new NullReferenceExpression()));
						firstValueResolveResult = new ConstantResolveResult(SpecialType.NullType, null);
					}
					else
					{
						Debug.Assert(values.Length > 0);
						firstValueResolveResult = values[0];
					}
					astSection.CaseLabels.AddRange(values.Select(label => new CaseLabel(exprBuilder.ConvertConstantValue(label, allowImplicitConversion: true))));
				}
				switch (section.Body)
				{
					case Branch br:
						// we can only inline the block, if all branches are in the switchContainer.
						if (br.TargetContainer == switchContainer && switchContainer.Descendants.OfType<Branch>().Where(b => b.TargetBlock == br.TargetBlock).All(b => BlockContainer.FindClosestSwitchContainer(b) == switchContainer))
							caseLabelMapping.Add(br.TargetBlock, firstValueResolveResult);
						break;
					default:
						break;
				}
				translationDictionary.Add(section, astSection);
				stmt.SwitchSections.Add(astSection);
			}
			foreach (var section in inst.Sections)
			{
				var astSection = translationDictionary[section];
				switch (section.Body)
				{
					case Branch br:
						// we can only inline the block, if all branches are in the switchContainer.
						if (br.TargetContainer == switchContainer && switchContainer.Descendants.OfType<Branch>().Where(b => b.TargetBlock == br.TargetBlock).All(b => BlockContainer.FindClosestSwitchContainer(b) == switchContainer))
							ConvertSwitchSectionBody(astSection, br.TargetBlock);
						else
							ConvertSwitchSectionBody(astSection, section.Body);
						break;
					case Leave leave:
						if (astSection.CaseLabels.Count == 1 && astSection.CaseLabels.First().Expression.IsNull && leave.TargetContainer == switchContainer)
						{
							stmt.SwitchSections.Remove(astSection);
							break;
						}
						goto default;
					default:
						ConvertSwitchSectionBody(astSection, section.Body);
						break;
				}
			}
			if (switchContainer != null && stmt.SwitchSections.Count > 0)
			{
				// Translate any remaining blocks:
				var lastSectionStatements = stmt.SwitchSections.Last().Statements;
				foreach (var block in switchContainer.Blocks.Skip(1))
				{
					if (caseLabelMapping.ContainsKey(block))
						continue;
					lastSectionStatements.Add(new LabelStatement { Label = block.Label });
					foreach (var nestedInst in block.Instructions)
					{
						var nestedStmt = Convert(nestedInst);
						if (nestedStmt is BlockStatement b)
						{
							foreach (var nested in b.Statements)
								lastSectionStatements.Add(nested.Detach());
						}
						else
						{
							lastSectionStatements.Add(nestedStmt);
						}
					}
					Debug.Assert(block.FinalInstruction.OpCode == OpCode.Nop);
				}
				if (endContainerLabels.TryGetValue(switchContainer, out string label))
				{
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
			if (!bodyInst.HasFlag(InstructionFlags.EndPointUnreachable))
			{
				// we need to insert 'break;'
				BlockStatement block = body as BlockStatement;
				if (block != null)
				{
					block.Add(new BreakStatement());
				}
				else
				{
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

		protected internal override TranslatedStatement VisitBranch(Branch inst)
		{
			if (inst.TargetBlock == continueTarget)
			{
				continueCount++;
				return new ContinueStatement().WithILInstruction(inst);
			}
			if (caseLabelMapping != null && caseLabelMapping.TryGetValue(inst.TargetBlock, out var label))
			{
				if (label == null)
					return new GotoDefaultStatement().WithILInstruction(inst);
				return new GotoCaseStatement() { LabelExpression = exprBuilder.ConvertConstantValue(label, allowImplicitConversion: true) }
					.WithILInstruction(inst);
			}
			return new GotoStatement(inst.TargetLabel).WithILInstruction(inst);
		}

		/// <summary>Target container that a 'break;' statement would break out of</summary>
		BlockContainer breakTarget;
		/// <summary>Dictionary from BlockContainer to label name for 'goto of_container';</summary>
		readonly Dictionary<BlockContainer, string> endContainerLabels = new Dictionary<BlockContainer, string>();

		protected internal override TranslatedStatement VisitLeave(Leave inst)
		{
			if (inst.TargetContainer == breakTarget)
				return new BreakStatement().WithILInstruction(inst);
			if (inst.TargetContainer == currentReturnContainer)
			{
				if (currentIsIterator)
					return new YieldBreakStatement().WithILInstruction(inst);
				else if (!inst.Value.MatchNop())
				{
					var expr = exprBuilder.Translate(inst.Value, typeHint: currentResultType)
						.ConvertTo(currentResultType, exprBuilder, allowImplicitConversion: true);
					return new ReturnStatement(expr).WithILInstruction(inst);
				}
				else
					return new ReturnStatement().WithILInstruction(inst);
			}
			if (!endContainerLabels.TryGetValue(inst.TargetContainer, out string label))
			{
				label = "end_" + inst.TargetLabel;
				endContainerLabels.Add(inst.TargetContainer, label);
			}
			return new GotoStatement(label).WithILInstruction(inst);
		}

		protected internal override TranslatedStatement VisitThrow(Throw inst)
		{
			return new ThrowStatement(exprBuilder.Translate(inst.Argument)).WithILInstruction(inst);
		}

		protected internal override TranslatedStatement VisitRethrow(Rethrow inst)
		{
			return new ThrowStatement().WithILInstruction(inst);
		}

		protected internal override TranslatedStatement VisitYieldReturn(YieldReturn inst)
		{
			var elementType = currentFunction.ReturnType.GetElementTypeFromIEnumerable(typeSystem, true, out var isGeneric);
			var expr = exprBuilder.Translate(inst.Value, typeHint: elementType)
				.ConvertTo(elementType, exprBuilder, allowImplicitConversion: true);
			return new YieldReturnStatement {
				Expression = expr
			}.WithILInstruction(inst);
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

		protected internal override TranslatedStatement VisitTryCatch(TryCatch inst)
		{
			var tryCatch = new TryCatchStatement();
			tryCatch.TryBlock = ConvertAsBlock(inst.TryBlock);
			foreach (var handler in inst.Handlers)
			{
				var catchClause = new CatchClause();
				catchClause.AddAnnotation(handler);
				var v = handler.Variable;
				if (v != null)
				{
					catchClause.AddAnnotation(new ILVariableResolveResult(v, v.Type));
					if (v.StoreCount > 1 || v.LoadCount > 0 || v.AddressCount > 0)
					{
						catchClause.VariableName = v.Name;
						catchClause.Type = exprBuilder.ConvertType(v.Type);
					}
					else if (!v.Type.IsKnownType(KnownTypeCode.Object))
					{
						catchClause.Type = exprBuilder.ConvertType(v.Type);
					}
				}
				if (!handler.Filter.MatchLdcI4(1))
					catchClause.Condition = exprBuilder.TranslateCondition(handler.Filter);
				catchClause.Body = ConvertAsBlock(handler.Body);
				tryCatch.CatchClauses.Add(catchClause);
			}
			return tryCatch.WithILInstruction(inst);
		}

		protected internal override TranslatedStatement VisitTryFinally(TryFinally inst)
		{
			var tryCatch = MakeTryCatch(inst.TryBlock);
			tryCatch.FinallyBlock = ConvertAsBlock(inst.FinallyBlock);
			return tryCatch.WithILInstruction(inst);
		}

		protected internal override TranslatedStatement VisitTryFault(TryFault inst)
		{
			var tryCatch = new TryCatchStatement();
			tryCatch.TryBlock = ConvertAsBlock(inst.TryBlock);
			var faultBlock = ConvertAsBlock(inst.FaultBlock);
			faultBlock.InsertChildAfter(null, new Comment("try-fault"), Roles.Comment);
			faultBlock.Add(new ThrowStatement());
			tryCatch.CatchClauses.Add(new CatchClause { Body = faultBlock });
			return tryCatch.WithILInstruction(inst);
		}

		protected internal override TranslatedStatement VisitLockInstruction(LockInstruction inst)
		{
			return new LockStatement {
				Expression = exprBuilder.Translate(inst.OnExpression),
				EmbeddedStatement = ConvertAsBlock(inst.Body)
			}.WithILInstruction(inst);
		}

		#region foreach construction
		static readonly InvocationExpression getEnumeratorPattern = new InvocationExpression(
			new Choice {
				new MemberReferenceExpression(new AnyNode("collection").ToExpression(), "GetEnumerator"),
				new MemberReferenceExpression(new AnyNode("collection").ToExpression(), "GetAsyncEnumerator")
			}
		);
		static readonly Expression moveNextConditionPattern = new Choice {
			new InvocationExpression(new MemberReferenceExpression(new NamedNode("enumerator", new IdentifierExpression(Pattern.AnyString)), "MoveNext")),
			new UnaryOperatorExpression(UnaryOperatorType.Await, new InvocationExpression(new MemberReferenceExpression(new NamedNode("enumerator", new IdentifierExpression(Pattern.AnyString)), "MoveNextAsync")))
		};

		protected internal override TranslatedStatement VisitUsingInstruction(UsingInstruction inst)
		{
			var resource = exprBuilder.Translate(inst.ResourceExpression).Expression;
			var transformed = TransformToForeach(inst, resource);
			if (transformed != null)
				return transformed.WithILInstruction(inst);
			AstNode usingInit = resource;
			var var = inst.Variable;
			KnownTypeCode knownTypeCode;
			IType disposeType;
			string disposeTypeMethodName;
			if (inst.IsAsync)
			{
				knownTypeCode = KnownTypeCode.IAsyncDisposable;
				disposeType = exprBuilder.compilation.FindType(KnownTypeCode.IAsyncDisposable);
				disposeTypeMethodName = "DisposeAsync";
			}
			else
			{
				knownTypeCode = KnownTypeCode.IDisposable;
				disposeType = exprBuilder.compilation.FindType(KnownTypeCode.IDisposable);
				disposeTypeMethodName = "Dispose";
			}
			if (!IsValidInCSharp(inst, knownTypeCode))
			{
				Debug.Assert(var.Kind == VariableKind.UsingLocal);
				var.Kind = VariableKind.Local;
				var disposeVariable = currentFunction.RegisterVariable(
					VariableKind.Local, disposeType,
					AssignVariableNames.GenerateVariableName(currentFunction, disposeType)
				);
				Expression disposeInvocation = new InvocationExpression(new MemberReferenceExpression(exprBuilder.ConvertVariable(disposeVariable).Expression, disposeTypeMethodName));
				if (inst.IsAsync)
				{
					disposeInvocation = new UnaryOperatorExpression { Expression = disposeInvocation, Operator = UnaryOperatorType.Await };
				}
				return new BlockStatement {
					new ExpressionStatement(new AssignmentExpression(exprBuilder.ConvertVariable(var).Expression, resource.Detach())),
					new TryCatchStatement {
						TryBlock = ConvertAsBlock(inst.Body),
						FinallyBlock = new BlockStatement() {
							new ExpressionStatement(new AssignmentExpression(exprBuilder.ConvertVariable(disposeVariable).Expression, new AsExpression(exprBuilder.ConvertVariable(var).Expression, exprBuilder.ConvertType(disposeType)))),
							new IfElseStatement {
								Condition = new BinaryOperatorExpression(exprBuilder.ConvertVariable(disposeVariable), BinaryOperatorType.InEquality, new NullReferenceExpression()),
								TrueStatement = new ExpressionStatement(disposeInvocation)
							}
						}
					},
				}.WithILInstruction(inst);
			}
			else
			{
				if (var.LoadCount > 0 || var.AddressCount > 0)
				{
					var type = settings.AnonymousTypes && var.Type.ContainsAnonymousType() ? new SimpleType("var") : exprBuilder.ConvertType(var.Type);
					var vds = new VariableDeclarationStatement(type, var.Name, resource);
					vds.Variables.Single().AddAnnotation(new ILVariableResolveResult(var, var.Type));
					usingInit = vds;
				}
				return new UsingStatement {
					ResourceAcquisition = usingInit,
					IsAsync = inst.IsAsync,
					EmbeddedStatement = ConvertAsBlock(inst.Body)
				}.WithILInstruction(inst);
			}

			bool IsValidInCSharp(UsingInstruction inst, KnownTypeCode code)
			{
				if (inst.ResourceExpression.MatchLdNull())
					return true;
				if (inst.IsRefStruct)
					return true;
				return NullableType.GetUnderlyingType(var.Type).GetAllBaseTypes().Any(b => b.IsKnownType(code));
			}
		}

		Statement TransformToForeach(UsingInstruction inst, Expression resource)
		{
			if (!settings.ForEachStatement)
			{
				return null;
			}
			// Check if the using resource matches the GetEnumerator pattern.
			var m = getEnumeratorPattern.Match(resource);
			// The using body must be a BlockContainer.
			if (!(inst.Body is BlockContainer container) || !m.Success)
				return null;
			bool isAsync = ((MemberReferenceExpression)((InvocationExpression)resource).Target).MemberName == "GetAsyncEnumerator";
			if (isAsync != inst.IsAsync)
				return null;
			// The using-variable is the enumerator.
			var enumeratorVar = inst.Variable;
			// If there's another BlockContainer nested in this container and it only has one child block, unwrap it.
			// If there's an extra leave inside the block, extract it into optionalReturnAfterLoop.
			var loopContainer = UnwrapNestedContainerIfPossible(container, out var optionalReturnAfterLoop);
			// Detect whether we're dealing with a while loop with multiple embedded statements.
			if (loopContainer.Kind != ContainerKind.While)
				return null;
			if (!loopContainer.MatchConditionBlock(loopContainer.EntryPoint, out var conditionInst, out var body))
				return null;
			// The loop condition must be a call to enumerator.MoveNext()
			var condition = exprBuilder.TranslateCondition(conditionInst);
			var m2 = moveNextConditionPattern.Match(condition.Expression);
			if (!m2.Success)
				return null;
			if (condition.Expression is UnaryOperatorExpression { Operator: UnaryOperatorType.Await } != isAsync)
				return null;
			// Check enumerator variable references.
			var enumeratorVar2 = m2.Get<IdentifierExpression>("enumerator").Single().GetILVariable();
			if (enumeratorVar2 != enumeratorVar)
				return null;
			// Detect which foreach-variable transformation is necessary/possible.
			var transformation = DetectGetCurrentTransformation(container, body, loopContainer, enumeratorVar, conditionInst, out var singleGetter, out var foreachVariable);
			if (transformation == RequiredGetCurrentTransformation.NoForeach)
				return null;
			// Extract in-expression
			var collectionExpr = m.Get<Expression>("collection").Single();
			// Special case: foreach (var item in this) is decompiled as foreach (var item in base)
			// but a base reference is not valid in this context.
			if (collectionExpr is BaseReferenceExpression)
			{
				collectionExpr = new ThisReferenceExpression().CopyAnnotationsFrom(collectionExpr);
			}
			else if (IsDynamicCastToIEnumerable(collectionExpr, out var dynamicExpr))
			{
				collectionExpr = dynamicExpr.Detach();
			}
			// Handle explicit casts:
			// This is the case if an explicit type different from the collection-item-type was used.
			// For example: foreach (ClassA item in nonGenericEnumerable)
			var type = singleGetter.Method.ReturnType;
			ILInstruction instToReplace = singleGetter;
			bool useVar = false;
			switch (instToReplace.Parent)
			{
				case CastClass cc:
					type = cc.Type;
					instToReplace = cc;
					break;
				case UnboxAny ua:
					type = ua.Type;
					instToReplace = ua;
					break;
				default:
					if (TupleType.IsTupleCompatible(type, out _))
					{
						// foreach with get_Current returning a tuple type, let's check which type "var" would infer:
						var foreachRR = exprBuilder.resolver.ResolveForeach(collectionExpr.GetResolveResult());
						if (EqualErasedType(type, foreachRR.ElementType))
						{
							type = foreachRR.ElementType;
							useVar = true;
						}
					}
					break;
			}

			VariableDesignation designation = null;

			// Handle the required foreach-variable transformation:
			switch (transformation)
			{
				case RequiredGetCurrentTransformation.UseExistingVariable:
					if (foreachVariable.Type.Kind != TypeKind.Dynamic)
						foreachVariable.Type = type;
					foreachVariable.Kind = VariableKind.ForeachLocal;
					foreachVariable.Name = AssignVariableNames.GenerateForeachVariableName(currentFunction, collectionExpr.Annotation<ILInstruction>(), foreachVariable);
					break;
				case RequiredGetCurrentTransformation.IntroduceNewVariable:
					foreachVariable = currentFunction.RegisterVariable(
						VariableKind.ForeachLocal, type,
						AssignVariableNames.GenerateForeachVariableName(currentFunction, collectionExpr.Annotation<ILInstruction>())
					);
					instToReplace.ReplaceWith(new LdLoc(foreachVariable));
					body.Instructions.Insert(0, new StLoc(foreachVariable, instToReplace));
					break;
				case RequiredGetCurrentTransformation.IntroduceNewVariableAndLocalCopy:
					foreachVariable = currentFunction.RegisterVariable(
						VariableKind.ForeachLocal, type,
						AssignVariableNames.GenerateForeachVariableName(currentFunction, collectionExpr.Annotation<ILInstruction>())
					);
					var localCopyVariable = currentFunction.RegisterVariable(
						VariableKind.Local, type,
						AssignVariableNames.GenerateVariableName(currentFunction, type)
					);
					instToReplace.Parent.ReplaceWith(new LdLoca(localCopyVariable));
					body.Instructions.Insert(0, new StLoc(localCopyVariable, new LdLoc(foreachVariable)));
					body.Instructions.Insert(0, new StLoc(foreachVariable, instToReplace));
					break;
				case RequiredGetCurrentTransformation.Deconstruction:
					useVar = true;
					designation = TranslateDeconstructionDesignation((DeconstructInstruction)body.Instructions[0], isForeach: true);
					break;
			}

			if (designation == null)
			{
				designation = new SingleVariableDesignation { Identifier = foreachVariable.Name };
				// Add the variable annotation for highlighting
				designation.AddAnnotation(new ILVariableResolveResult(foreachVariable, foreachVariable.Type));
			}

			// Convert the modified body to C# AST:
			var whileLoop = (WhileStatement)ConvertAsBlock(container).First();
			BlockStatement foreachBody = (BlockStatement)whileLoop.EmbeddedStatement.Detach();

			// Remove the first statement, as it is the foreachVariable = enumerator.Current; statement.
			Statement firstStatement = foreachBody.Statements.First();
			if (firstStatement is LabelStatement)
			{
				// skip the entry-point label, if any
				firstStatement = firstStatement.GetNextStatement();
			}
			Debug.Assert(firstStatement is ExpressionStatement);
			firstStatement.Remove();

			if (settings.AnonymousTypes && type.ContainsAnonymousType())
				useVar = true;

			// Construct the foreach loop.
			var foreachStmt = new ForeachStatement {
				IsAsync = isAsync,
				VariableType = useVar ? new SimpleType("var") : exprBuilder.ConvertType(foreachVariable.Type),
				VariableDesignation = designation,
				InExpression = collectionExpr.Detach(),
				EmbeddedStatement = foreachBody
			};
			foreachStmt.AddAnnotation(new ForeachAnnotation(inst.ResourceExpression, conditionInst, singleGetter));
			foreachStmt.CopyAnnotationsFrom(whileLoop);
			// If there was an optional return statement, return it as well.
			if (optionalReturnAfterLoop != null)
			{
				return new BlockStatement {
					Statements = {
						foreachStmt,
						optionalReturnAfterLoop.AcceptVisitor(this)
					}
				};
			}
			return foreachStmt;
		}

		internal static VariableDesignation TranslateDeconstructionDesignation(DeconstructInstruction inst, bool isForeach)
		{
			var assignments = inst.Assignments.Instructions;
			int assignmentPos = 0;

			return ConstructDesignation(inst.Pattern);

			VariableDesignation ConstructDesignation(MatchInstruction matchInstruction)
			{
				var designations = new ParenthesizedVariableDesignation();
				foreach (var subPattern in matchInstruction.SubPatterns.Cast<MatchInstruction>())
				{
					if (subPattern.IsVar)
					{
						var designation = new SingleVariableDesignation();
						if (subPattern.HasDesignator)
						{
							ILVariable v = ((StLoc)assignments[assignmentPos]).Variable;
							if (isForeach)
								v.Kind = VariableKind.ForeachLocal;
							designation.Identifier = v.Name;
							designation.AddAnnotation(new ILVariableResolveResult(v));
							assignmentPos++;
						}
						else
						{
							designation.Identifier = "_";
						}
						designations.VariableDesignations.Add(designation);
					}
					else
					{
						designations.VariableDesignations.Add(ConstructDesignation(subPattern));
					}
				}
				return designations;
			}
		}

		static bool EqualErasedType(IType a, IType b)
		{
			return NormalizeTypeVisitor.TypeErasure.EquivalentTypes(a, b);
		}

		private bool IsDynamicCastToIEnumerable(Expression expr, out Expression dynamicExpr)
		{
			if (!(expr is CastExpression cast))
			{
				dynamicExpr = null;
				return false;
			}
			dynamicExpr = cast.Expression;
			if (!(expr.GetResolveResult() is ConversionResolveResult crr))
				return false;
			if (!crr.Type.IsKnownType(KnownTypeCode.IEnumerable))
				return false;
			return crr.Input.Type.Kind == TypeKind.Dynamic;
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
			if (leave.IsLeavingFunction && SemanticHelper.IsPure(leave.Value.Flags))
			{
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
			/// No store was found, thus create a new variable and use it as foreach variable.
			/// <code>
			///	... (call get_Current()) ...
			///	=>
			///	stloc foreachVar(call get_Current())
			///	... (ldloc foreachVar) ...
			/// </code>
			/// </summary>
			IntroduceNewVariable,
			/// <summary>
			/// No store was found, thus create a new variable and use it as foreach variable.
			/// Additionally it is necessary to copy the value of the foreach variable to another local
			/// to allow safe modification of its value.
			/// <code>
			///	... addressof(call get_Current()) ...
			///	=>
			///	stloc foreachVar(call get_Current())
			///	stloc copy(ldloc foreachVar)
			///	... (ldloca copy) ...
			/// </code>
			/// </summary>
			IntroduceNewVariableAndLocalCopy,
			/// <summary>
			/// call get_Current() is the tested operand of a deconstruct instruction.
			/// and the deconstruct instruction is the first statement in the loop body.
			/// </summary>
			Deconstruction,
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
		RequiredGetCurrentTransformation DetectGetCurrentTransformation(BlockContainer usingContainer, Block loopBody, BlockContainer loopContainer, ILVariable enumerator, ILInstruction moveNextUsage, out CallInstruction singleGetter, out ILVariable foreachVariable)
		{
			singleGetter = null;
			foreachVariable = null;
			var loads = enumerator.LoadInstructions.OfType<ILInstruction>()
				.Concat(enumerator.AddressInstructions.OfType<ILInstruction>())
				.Where(ld => !ld.IsDescendantOf(moveNextUsage))
				.ToArray();
			// enumerator is used in multiple locations or not in conjunction with get_Current
			// => no foreach
			if (loads.Length != 1 || !ParentIsCurrentGetter(loads[0]))
				return RequiredGetCurrentTransformation.NoForeach;
			singleGetter = (CallInstruction)loads[0].Parent;
			// singleGetter is not part of the first instruction in body or cannot be uninlined
			// => no foreach
			if (!(singleGetter.IsDescendantOf(loopBody.Instructions[0])
				&& ILInlining.CanUninline(singleGetter, loopBody.Instructions[0])))
			{
				return RequiredGetCurrentTransformation.NoForeach;
			}
			if (loopBody.Instructions[0] is DeconstructInstruction deconstruction
				&& singleGetter == deconstruction.Pattern.TestedOperand
				&& CanBeDeconstructedInForeach(deconstruction, usingContainer, loopContainer))
			{
				return RequiredGetCurrentTransformation.Deconstruction;
			}
			ILInstruction inst = singleGetter;
			// in some cases, i.e. foreach variable with explicit type different from the collection-item-type,
			// the result of call get_Current is casted.
			while (inst.Parent is UnboxAny || inst.Parent is CastClass)
				inst = inst.Parent;
			// One variable was found.
			if (inst.Parent is StLoc stloc && (stloc.Variable.Kind == VariableKind.Local || stloc.Variable.Kind == VariableKind.StackSlot))
			{
				// Must be a plain assignment expression and variable must only be used in 'body' + only assigned once.
				if (stloc.Parent == loopBody && VariableIsOnlyUsedInBlock(stloc, usingContainer, loopContainer))
				{
					foreachVariable = stloc.Variable;
					return RequiredGetCurrentTransformation.UseExistingVariable;
				}
			}
			// In optimized Roslyn code it can happen that the foreach variable is referenced via addressof
			// We only do this unwrapping if where dealing with a custom struct type.
			if (CurrentIsStructSetterTarget(inst, singleGetter))
			{
				return RequiredGetCurrentTransformation.IntroduceNewVariableAndLocalCopy;
			}
			// No suitable variable was found: we need a new one.
			return RequiredGetCurrentTransformation.IntroduceNewVariable;
		}

		bool CanBeDeconstructedInForeach(DeconstructInstruction deconstruction, BlockContainer usingContainer, BlockContainer loopContainer)
		{
			if (deconstruction.Init.Count > 0)
				return false;
			if (deconstruction.Conversions.Instructions.Count > 0)
				return false;
			var operandType = deconstruction.Pattern.TestedOperand.InferType(this.typeSystem);
			var expectedType = deconstruction.Pattern.Variable.Type;
			if (!NormalizeTypeVisitor.TypeErasure.EquivalentTypes(operandType, expectedType))
				return false;
			var usedVariables = new HashSet<ILVariable>(ILVariableEqualityComparer.Instance);
			foreach (var item in deconstruction.Assignments.Instructions)
			{
				if (!item.MatchStLoc(out var v, out var value))
					return false;
				expectedType = ((LdLoc)value).Variable.Type;
				if (!NormalizeTypeVisitor.TypeErasure.EquivalentTypes(v.Type, expectedType))
					return false;
				if (!(v.Kind == VariableKind.StackSlot || v.Kind == VariableKind.Local))
					return false;
				if (!VariableIsOnlyUsedInBlock((StLoc)item, usingContainer, loopContainer))
					return false;
				if (!(v.CaptureScope == null || v.CaptureScope == usingContainer))
					return false;
				if (!usedVariables.Add(v))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Determines whether storeInst.Variable is only assigned once and used only inside <paramref name="usingContainer"/>.
		/// Loads by reference (ldloca) are only allowed in the context of this pointer in call instructions,
		/// or as target of ldobj.
		/// (This only applies to value types.)
		/// </summary>
		bool VariableIsOnlyUsedInBlock(StLoc storeInst, BlockContainer usingContainer, BlockContainer loopContainer)
		{
			if (storeInst.Variable.LoadInstructions.Any(ld => !ld.IsDescendantOf(usingContainer)))
				return false;
			if (storeInst.Variable.AddressInstructions.Any(inst => !AddressUseAllowed(inst)))
				return false;
			if (storeInst.Variable.StoreInstructions.OfType<ILInstruction>().Any(st => st != storeInst))
				return false;
			if (!(storeInst.Variable.CaptureScope == null || storeInst.Variable.CaptureScope == loopContainer))
				return false;
			return true;

			bool AddressUseAllowed(LdLoca la)
			{
				if (!la.IsDescendantOf(usingContainer))
					return false;
				if (ILInlining.IsUsedAsThisPointerInCall(la) && !IsTargetOfSetterCall(la, la.Variable.Type))
					return true;
				var current = la.Parent;
				while (current is LdFlda next)
				{
					current = next.Parent;
				}
				return current is LdObj;
			}
		}

		/// <summary>
		/// Returns true if singleGetter is a value type and its address is used as setter target.
		/// </summary>
		bool CurrentIsStructSetterTarget(ILInstruction inst, CallInstruction singleGetter)
		{
			if (!(inst.Parent is AddressOf addr))
				return false;
			return IsTargetOfSetterCall(addr, singleGetter.Method.ReturnType);
		}

		bool IsTargetOfSetterCall(ILInstruction inst, IType targetType)
		{
			if (inst.ChildIndex != 0)
				return false;
			if (targetType.IsReferenceType ?? false)
				return false;
			switch (inst.Parent.OpCode)
			{
				case OpCode.Call:
				case OpCode.CallVirt:
					var targetMethod = ((CallInstruction)inst.Parent).Method;
					if (!targetMethod.IsAccessor || targetMethod.IsStatic)
						return false;
					switch (targetMethod.AccessorOwner)
					{
						case IProperty p:
							return targetMethod.AccessorKind == System.Reflection.MethodSemanticsAttributes.Setter;
						default:
							return true;
					}
				default:
					return false;
			}
		}

		bool ParentIsCurrentGetter(ILInstruction inst)
		{
			return inst.Parent is CallInstruction cv && cv.Method.IsAccessor &&
				cv.Method.AccessorKind == System.Reflection.MethodSemanticsAttributes.Getter;
		}
		#endregion

		protected internal override TranslatedStatement VisitPinnedRegion(PinnedRegion inst)
		{
			var fixedStmt = new FixedStatement();
			fixedStmt.Type = exprBuilder.ConvertType(inst.Variable.Type);
			Expression initExpr;
			if (inst.Init is GetPinnableReference gpr)
			{
				if (gpr.Method != null)
				{
					IType expectedType = gpr.Method.IsStatic ? gpr.Method.Parameters[0].Type : gpr.Method.DeclaringType;
					initExpr = exprBuilder.Translate(gpr.Argument, typeHint: expectedType).ConvertTo(expectedType, exprBuilder);
				}
				else
				{
					initExpr = exprBuilder.Translate(gpr.Argument);
				}
			}
			else
			{
				IType refType = inst.Variable.Type;
				if (refType is PointerType pointerType)
				{
					refType = new ByReferenceType(pointerType.ElementType);
				}
				initExpr = exprBuilder.Translate(inst.Init, typeHint: refType).ConvertTo(refType, exprBuilder);
				if (initExpr is DirectionExpression dirExpr)
				{
					if (dirExpr.Expression is UnaryOperatorExpression uoe && uoe.Operator == UnaryOperatorType.Dereference)
					{
						initExpr = uoe.Expression.Detach();
					}
					else
					{
						initExpr = new UnaryOperatorExpression(UnaryOperatorType.AddressOf, dirExpr.Expression.Detach())
							.WithRR(new ResolveResult(inst.Variable.Type));
					}
				}
			}
			fixedStmt.Variables.Add(new VariableInitializer(inst.Variable.Name, initExpr).WithILVariable(inst.Variable));
			fixedStmt.EmbeddedStatement = Convert(inst.Body);
			return fixedStmt.WithILInstruction(inst);
		}

		protected internal override TranslatedStatement VisitBlock(Block block)
		{
			if (block.Kind != BlockKind.ControlFlow)
				return Default(block);
			// Block without container
			BlockStatement blockStatement = new BlockStatement();
			foreach (var inst in block.Instructions)
			{
				blockStatement.Add(Convert(inst));
			}
			if (block.FinalInstruction.OpCode != OpCode.Nop)
				blockStatement.Add(Convert(block.FinalInstruction));
			return blockStatement.WithILInstruction(block);
		}

		protected internal override TranslatedStatement VisitBlockContainer(BlockContainer container)
		{
			if (container.Kind != ContainerKind.Normal && container.EntryPoint.IncomingEdgeCount > 1)
			{
				var oldContinueTarget = continueTarget;
				var oldContinueCount = continueCount;
				var oldBreakTarget = breakTarget;
				var loop = ConvertLoop(container);
				loop.AddAnnotation(container);
				continueTarget = oldContinueTarget;
				continueCount = oldContinueCount;
				breakTarget = oldBreakTarget;
				return loop.WithILInstruction(container);
			}
			else if (container.EntryPoint.Instructions.Count == 1 && container.EntryPoint.Instructions[0] is SwitchInstruction switchInst)
			{
				return TranslateSwitch(container, switchInst).WithILInstruction(container);
			}
			else
			{
				var blockStmt = ConvertBlockContainer(container, false);
				return blockStmt.WithILInstruction(container);
			}
		}

		Statement ConvertLoop(BlockContainer container)
		{
			ILInstruction condition;
			Block loopBody;
			BlockStatement blockStatement;
			continueCount = 0;
			breakTarget = container;
			switch (container.Kind)
			{
				case ContainerKind.Loop:
					continueTarget = container.EntryPoint;
					blockStatement = ConvertBlockContainer(container, true);
					Debug.Assert(continueCount < container.EntryPoint.IncomingEdgeCount);
					Debug.Assert(blockStatement.Statements.First() is LabelStatement);
					if (container.EntryPoint.IncomingEdgeCount == continueCount + 1)
					{
						// Remove the entrypoint label if all jumps to the label were replaced with 'continue;' statements
						blockStatement.Statements.First().Remove();
					}

					if (blockStatement.LastOrDefault() is ContinueStatement continueStmt)
						continueStmt.Remove();
					DeclareLocalFunctions(currentFunction, container, blockStatement);
					return new WhileStatement(new PrimitiveExpression(true), blockStatement);
				case ContainerKind.While:
					continueTarget = container.EntryPoint;
					if (!container.MatchConditionBlock(continueTarget, out condition, out loopBody))
						throw new NotSupportedException("Invalid condition block in while loop.");
					blockStatement = ConvertAsBlock(loopBody);
					if (!loopBody.HasFlag(InstructionFlags.EndPointUnreachable))
						blockStatement.Add(new BreakStatement());
					blockStatement = ConvertBlockContainer(blockStatement, container, container.Blocks.Skip(1).Except(new[] { loopBody }), true);
					Debug.Assert(continueCount < container.EntryPoint.IncomingEdgeCount);
					if (continueCount + 1 < container.EntryPoint.IncomingEdgeCount)
					{
						// There's an incoming edge to the entry point (=while condition) that wasn't represented as "continue;"
						// -> emit a real label
						// We'll also remove any "continue;" in front of the label, as it's redundant.
						if (blockStatement.LastOrDefault() is ContinueStatement)
							blockStatement.Last().Remove();
						blockStatement.Add(new LabelStatement { Label = container.EntryPoint.Label });
					}

					if (blockStatement.LastOrDefault() is ContinueStatement continueStmt2)
						continueStmt2.Remove();
					DeclareLocalFunctions(currentFunction, container, blockStatement);
					return new WhileStatement(exprBuilder.TranslateCondition(condition), blockStatement);
				case ContainerKind.DoWhile:
					continueTarget = container.Blocks.Last();
					if (!container.MatchConditionBlock(continueTarget, out condition, out _))
						throw new NotSupportedException("Invalid condition block in do-while loop.");
					blockStatement = ConvertBlockContainer(new BlockStatement(), container, container.Blocks.SkipLast(1), true);
					if (container.EntryPoint.IncomingEdgeCount == 2)
					{
						// Remove the entry-point label, if there are only two jumps to the entry-point:
						// from outside the loop and from the condition-block.
						blockStatement.Statements.First().Remove();
					}
					if (blockStatement.LastOrDefault() is ContinueStatement continueStmt3)
						continueStmt3.Remove();
					if (continueTarget.IncomingEdgeCount > continueCount)
					{
						// if there are branches to the condition block, that were not converted
						// to continue statements, we have to introduce an extra label.
						blockStatement.Add(new LabelStatement { Label = continueTarget.Label });
					}
					DeclareLocalFunctions(currentFunction, container, blockStatement);
					if (blockStatement.Statements.Count == 0)
					{
						return new WhileStatement {
							Condition = exprBuilder.TranslateCondition(condition),
							EmbeddedStatement = blockStatement
						};
					}
					return new DoWhileStatement {
						EmbeddedStatement = blockStatement,
						Condition = exprBuilder.TranslateCondition(condition)
					};
				case ContainerKind.For:
					continueTarget = container.Blocks.Last();
					if (!container.MatchConditionBlock(container.EntryPoint, out condition, out loopBody))
						throw new NotSupportedException("Invalid condition block in for loop.");
					blockStatement = ConvertAsBlock(loopBody);
					if (!loopBody.HasFlag(InstructionFlags.EndPointUnreachable))
						blockStatement.Add(new BreakStatement());
					if (!container.MatchIncrementBlock(continueTarget))
						throw new NotSupportedException("Invalid increment block in for loop.");
					blockStatement = ConvertBlockContainer(blockStatement, container, container.Blocks.SkipLast(1).Skip(1).Except(new[] { loopBody }), true);
					var forStmt = new ForStatement() {
						Condition = exprBuilder.TranslateCondition(condition),
						EmbeddedStatement = blockStatement
					};
					if (blockStatement.LastOrDefault() is ContinueStatement continueStmt4)
						continueStmt4.Remove();
					for (int i = 0; i < continueTarget.Instructions.Count - 1; i++)
					{
						forStmt.Iterators.Add(Convert(continueTarget.Instructions[i]));
					}
					if (continueTarget.IncomingEdgeCount > continueCount)
						blockStatement.Add(new LabelStatement { Label = continueTarget.Label });
					DeclareLocalFunctions(currentFunction, container, blockStatement);
					return forStmt;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		BlockStatement ConvertBlockContainer(BlockContainer container, bool isLoop)
		{
			var blockStatement = ConvertBlockContainer(new BlockStatement(), container, container.Blocks, isLoop);
			DeclareLocalFunctions(currentFunction, container, blockStatement);
			return blockStatement;
		}

		void DeclareLocalFunctions(ILFunction currentFunction, BlockContainer container, BlockStatement blockStatement)
		{
			foreach (var localFunction in currentFunction.LocalFunctions.OrderBy(f => f.Name))
			{
				if (localFunction.DeclarationScope != container)
					continue;
				blockStatement.Add(TranslateFunction(localFunction));
			}

			LocalFunctionDeclarationStatement TranslateFunction(ILFunction function)
			{
				var nestedBuilder = new StatementBuilder(typeSystem, exprBuilder.decompilationContext, function, settings, cancellationToken);
				var astBuilder = exprBuilder.astBuilder;
				var method = (MethodDeclaration)astBuilder.ConvertEntity(function.ReducedMethod);
				method.Body = nestedBuilder.ConvertAsBlock(function.Body);

				Comment prev = null;
				foreach (string warning in function.Warnings)
				{
					method.Body.InsertChildAfter(prev, prev = new Comment(warning), Roles.Comment);
				}

				CSharpDecompiler.CleanUpMethodDeclaration(method, method.Body, function);
				CSharpDecompiler.RemoveAttribute(method, KnownAttribute.CompilerGenerated);
				var stmt = new LocalFunctionDeclarationStatement(method);
				stmt.AddAnnotation(new MemberResolveResult(null, function.ReducedMethod));
				stmt.WithILInstruction(function);
				return stmt;
			}
		}

		BlockStatement ConvertBlockContainer(BlockStatement blockStatement, BlockContainer container, IEnumerable<Block> blocks, bool isLoop)
		{
			foreach (var block in blocks)
			{
				if (block.IncomingEdgeCount > 1 || block != container.EntryPoint)
				{
					// If there are any incoming branches to this block, add a label:
					blockStatement.Add(new LabelStatement { Label = block.Label });
				}
				foreach (var inst in block.Instructions)
				{
					if (!isLoop && inst is Leave leave && IsFinalLeave(leave))
					{
						// skip the final 'leave' instruction and just fall out of the BlockStatement
						blockStatement.AddAnnotation(new ImplicitReturnAnnotation(leave));
						continue;
					}
					var stmt = Convert(inst);
					if (stmt is BlockStatement b)
					{
						foreach (var nested in b.Statements)
							blockStatement.Add(nested.Detach());
					}
					else
					{
						blockStatement.Add(stmt);
					}
				}
				if (block.FinalInstruction.OpCode != OpCode.Nop)
				{
					blockStatement.Add(Convert(block.FinalInstruction));
				}
			}
			if (endContainerLabels.TryGetValue(container, out string label))
			{
				if (isLoop && !(blockStatement.LastOrDefault() is ContinueStatement))
				{
					blockStatement.Add(new ContinueStatement());
				}
				blockStatement.Add(new LabelStatement { Label = label });
				if (isLoop)
				{
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

		protected internal override TranslatedStatement VisitInitblk(Initblk inst)
		{
			var stmt = new ExpressionStatement(
				exprBuilder.CallUnsafeIntrinsic(
					inst.UnalignedPrefix != 0 ? "InitBlockUnaligned" : "InitBlock",
					new Expression[] {
						exprBuilder.Translate(inst.Address),
						exprBuilder.Translate(inst.Value),
						exprBuilder.Translate(inst.Size)
					},
					exprBuilder.compilation.FindType(KnownTypeCode.Void),
					inst
				)
			);
			stmt.InsertChildAfter(null, new Comment(" IL initblk instruction"), Roles.Comment);
			return stmt.WithILInstruction(inst);
		}

		protected internal override TranslatedStatement VisitCpblk(Cpblk inst)
		{
			var stmt = new ExpressionStatement(
				exprBuilder.CallUnsafeIntrinsic(
					inst.UnalignedPrefix != 0 ? "CopyBlockUnaligned" : "CopyBlock",
					new Expression[] {
						exprBuilder.Translate(inst.DestAddress),
						exprBuilder.Translate(inst.SourceAddress),
						exprBuilder.Translate(inst.Size)
					},
					exprBuilder.compilation.FindType(KnownTypeCode.Void),
					inst
				)
			);
			stmt.InsertChildAfter(null, new Comment(" IL cpblk instruction"), Roles.Comment);
			return stmt.WithILInstruction(inst);
		}
	}
}
