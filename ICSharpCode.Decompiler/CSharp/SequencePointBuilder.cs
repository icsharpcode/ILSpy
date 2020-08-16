// Copyright (c) 2017 Daniel Grunwald
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
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// Given a SyntaxTree that was output from the decompiler, constructs the list of sequence points.
	/// </summary>
	// Each statement / expression AST node is annotated with the ILInstruction(s) it was constructed from.
	// Each ILInstruction has a list of IL offsets corresponding to the original IL range(s). Note that the ILAst
	// instructions form a tree.
	//
	// This visitor constructs a list of sequence points from the syntax tree by visiting each node,
	// calling
	// 1. StartSequencePoint(AstNode)
	// 2. AddToSequencePoint(AstNode) (possibly multiple times)
	// 3. EndSequencePoint(TextLocation, TextLocation)
	// on each node.
	//
	// The VisitAsSequencePoint(AstNode) method encapsulates the steps above.
	//
	// The state we record for each sequence point is decribed in StatePerSequencePoint:
	// 1. primary AST node
	// 2. IL range intervals
	// 3. parent ILFunction (either a method or lambda)
	//
	// For each statement (at least) one sequence point is created and all expressions and their IL ranges
	// are added to it. Currently the debugger seems not to support breakpoints at an expression level, so
	// we stop at the statement level and add all sub-expressions to the same sequence point.
	//
	// LambdaExpression is one exception: we create new sequence points for the expression/statements of the lambda,
	// note however, that these are added to a different ILFunction.
	//
	// AddToSequencePoint(AstNode) handles the list of ILInstructions and visits each ILInstruction and its descendants.
	// We do not descend into nested ILFunctions as these create their own list of sequence points.
	class SequencePointBuilder : DepthFirstAstVisitor
	{
		struct StatePerSequencePoint
		{
			/// <summary>
			/// Main AST node associated with this sequence point.
			/// </summary>
			internal readonly AstNode PrimaryNode;

			/// <summary>
			/// List of IL intervals that are associated with this sequence point.
			/// </summary>
			internal readonly List<Interval> Intervals;

			/// <summary>
			/// The function containing this sequence point.
			/// </summary>
			internal ILFunction Function;

			public StatePerSequencePoint(AstNode primaryNode)
			{
				this.PrimaryNode = primaryNode;
				this.Intervals = new List<Interval>();
				this.Function = null;
			}
		}

		readonly List<(ILFunction, DebugInfo.SequencePoint)> sequencePoints = new List<(ILFunction, DebugInfo.SequencePoint)>();
		readonly HashSet<ILInstruction> mappedInstructions = new HashSet<ILInstruction>();
		
		// Stack holding information for outer statements.
		readonly Stack<StatePerSequencePoint> outerStates = new Stack<StatePerSequencePoint>();

		// Collects information for the current sequence point.
		StatePerSequencePoint current;

		void VisitAsSequencePoint(AstNode node)
		{
			if (node.IsNull) return;
			StartSequencePoint(node);
			node.AcceptVisitor(this);
			EndSequencePoint(node.StartLocation, node.EndLocation);
		}

		protected override void VisitChildren(AstNode node)
		{
			base.VisitChildren(node);
			AddToSequencePoint(node);
		}

		public override void VisitBlockStatement(BlockStatement blockStatement)
		{
			ILInstruction blockContainer = blockStatement.Annotations.OfType<ILInstruction>().FirstOrDefault();
			if (blockContainer != null) {
				StartSequencePoint(blockStatement.LBraceToken);
				int intervalStart;
				if (blockContainer.Parent is TryCatchHandler handler && !handler.ExceptionSpecifierILRange.IsEmpty) {
					// if this block container is part of a TryCatchHandler, do not steal the exception-specifier IL range
					intervalStart = handler.ExceptionSpecifierILRange.End;
				} else {
					intervalStart = blockContainer.StartILOffset;
				}
				// The end will be set to the first sequence point candidate location before the first statement of the function when the seqeunce point is adjusted
				int intervalEnd = intervalStart + 1; 

				Interval interval = new Interval(intervalStart, intervalEnd);
				List<Interval> intervals = new List<Interval>();
				intervals.Add(interval);
				current.Intervals.AddRange(intervals);
				current.Function = blockContainer.Ancestors.OfType<ILFunction>().FirstOrDefault();
				EndSequencePoint(blockStatement.LBraceToken.StartLocation, blockStatement.LBraceToken.EndLocation);
			}
			else {
				// Ideally, we'd be able to address this case. Blocks that are not the top-level function block have no ILInstruction annotations. It isn't clear to me how to determine the il range.
				// For now, do not add the opening brace sequence in this case.
			}

			foreach (var stmt in blockStatement.Statements) {
				VisitAsSequencePoint(stmt);
			}
			var implicitReturn = blockStatement.Annotation<ImplicitReturnAnnotation>();
			if (implicitReturn != null) {
				StartSequencePoint(blockStatement.RBraceToken);
				AddToSequencePoint(implicitReturn.Leave);
				EndSequencePoint(blockStatement.RBraceToken.StartLocation, blockStatement.RBraceToken.EndLocation);
			}
		}

		public override void VisitForStatement(ForStatement forStatement)
		{
			// Every element of a for-statement is its own sequence point.
			foreach (var init in forStatement.Initializers) {
				VisitAsSequencePoint(init);
			}
			VisitAsSequencePoint(forStatement.Condition);
			foreach (var inc in forStatement.Iterators) {
				VisitAsSequencePoint(inc);
			}
			VisitAsSequencePoint(forStatement.EmbeddedStatement);
		}
		
		public override void VisitSwitchStatement(SwitchStatement switchStatement)
		{
			StartSequencePoint(switchStatement);
			switchStatement.Expression.AcceptVisitor(this);
			foreach (var section in switchStatement.SwitchSections) {
				// note: sections will not contribute to the current sequence point
				section.AcceptVisitor(this);
			}
			// add switch statement itself to sequence point
			// (call only after the sections are visited)
			AddToSequencePoint(switchStatement);
			EndSequencePoint(switchStatement.StartLocation, switchStatement.RParToken.EndLocation);
		}

		public override void VisitSwitchSection(Syntax.SwitchSection switchSection)
		{
			// every statement in the switch section is its own sequence point
			foreach (var stmt in switchSection.Statements) {
				VisitAsSequencePoint(stmt);
			}
		}

		public override void VisitLambdaExpression(LambdaExpression lambdaExpression)
		{
			AddToSequencePoint(lambdaExpression);
			VisitAsSequencePoint(lambdaExpression.Body);
		}

		public override void VisitQueryContinuationClause(QueryContinuationClause queryContinuationClause)
		{
			AddToSequencePoint(queryContinuationClause);
			VisitAsSequencePoint(queryContinuationClause.PrecedingQuery);
		}

		public override void VisitQueryFromClause(QueryFromClause queryFromClause)
		{
			if (queryFromClause.Parent.FirstChild != queryFromClause) {
				AddToSequencePoint(queryFromClause);
				VisitAsSequencePoint(queryFromClause.Expression);
			} else {
				base.VisitQueryFromClause(queryFromClause);
			}
		}

		public override void VisitQueryGroupClause(QueryGroupClause queryGroupClause)
		{
			AddToSequencePoint(queryGroupClause);
			VisitAsSequencePoint(queryGroupClause.Projection);
			VisitAsSequencePoint(queryGroupClause.Key);
		}

		public override void VisitQueryJoinClause(QueryJoinClause queryJoinClause)
		{
			AddToSequencePoint(queryJoinClause);
			VisitAsSequencePoint(queryJoinClause.OnExpression);
			VisitAsSequencePoint(queryJoinClause.EqualsExpression);
		}

		public override void VisitQueryLetClause(QueryLetClause queryLetClause)
		{
			AddToSequencePoint(queryLetClause);
			VisitAsSequencePoint(queryLetClause.Expression);
		}

		public override void VisitQueryOrdering(QueryOrdering queryOrdering)
		{
			AddToSequencePoint(queryOrdering);
			VisitAsSequencePoint(queryOrdering.Expression);
		}

		public override void VisitQuerySelectClause(QuerySelectClause querySelectClause)
		{
			AddToSequencePoint(querySelectClause);
			VisitAsSequencePoint(querySelectClause.Expression);
		}

		public override void VisitQueryWhereClause(QueryWhereClause queryWhereClause)
		{
			AddToSequencePoint(queryWhereClause);
			VisitAsSequencePoint(queryWhereClause.Condition);
		}

		public override void VisitUsingStatement(UsingStatement usingStatement)
		{
			StartSequencePoint(usingStatement);
			usingStatement.ResourceAcquisition.AcceptVisitor(this);
			VisitAsSequencePoint(usingStatement.EmbeddedStatement);
			AddToSequencePoint(usingStatement);
			EndSequencePoint(usingStatement.StartLocation, usingStatement.RParToken.EndLocation);
		}

		public override void VisitForeachStatement(ForeachStatement foreachStatement)
		{
			var foreachInfo = foreachStatement.Annotation<ForeachAnnotation>();
			if (foreachInfo == null) {
				base.VisitForeachStatement(foreachStatement);
				return;
			}
			// TODO : Add a sequence point on foreach token (mapped to nop before using instruction).
			StartSequencePoint(foreachStatement);
			foreachStatement.InExpression.AcceptVisitor(this);
			AddToSequencePoint(foreachInfo.GetEnumeratorCall);
			EndSequencePoint(foreachStatement.InExpression.StartLocation, foreachStatement.InExpression.EndLocation);

			StartSequencePoint(foreachStatement);
			AddToSequencePoint(foreachInfo.MoveNextCall);
			EndSequencePoint(foreachStatement.InToken.StartLocation, foreachStatement.InToken.EndLocation);
			
			StartSequencePoint(foreachStatement);
			AddToSequencePoint(foreachInfo.GetCurrentCall);
			EndSequencePoint(foreachStatement.VariableType.StartLocation, foreachStatement.VariableDesignation.EndLocation);
			
			VisitAsSequencePoint(foreachStatement.EmbeddedStatement);
		}

		public override void VisitLockStatement(LockStatement lockStatement)
		{
			StartSequencePoint(lockStatement);
			lockStatement.Expression.AcceptVisitor(this);
			VisitAsSequencePoint(lockStatement.EmbeddedStatement);
			AddToSequencePoint(lockStatement);
			EndSequencePoint(lockStatement.StartLocation, lockStatement.RParToken.EndLocation);
		}

		public override void VisitIfElseStatement(IfElseStatement ifElseStatement)
		{
			StartSequencePoint(ifElseStatement);
			ifElseStatement.Condition.AcceptVisitor(this);
			VisitAsSequencePoint(ifElseStatement.TrueStatement);
			VisitAsSequencePoint(ifElseStatement.FalseStatement);
			AddToSequencePoint(ifElseStatement);
			EndSequencePoint(ifElseStatement.StartLocation, ifElseStatement.RParToken.EndLocation);
		}

		public override void VisitWhileStatement(WhileStatement whileStatement)
		{
			StartSequencePoint(whileStatement);
			whileStatement.Condition.AcceptVisitor(this);
			VisitAsSequencePoint(whileStatement.EmbeddedStatement);
			AddToSequencePoint(whileStatement);
			EndSequencePoint(whileStatement.StartLocation, whileStatement.RParToken.EndLocation);
		}

		public override void VisitDoWhileStatement(DoWhileStatement doWhileStatement)
		{
			StartSequencePoint(doWhileStatement);
			VisitAsSequencePoint(doWhileStatement.EmbeddedStatement);
			doWhileStatement.Condition.AcceptVisitor(this);
			AddToSequencePoint(doWhileStatement);
			EndSequencePoint(doWhileStatement.WhileToken.StartLocation, doWhileStatement.RParToken.EndLocation);
		}

		public override void VisitFixedStatement(FixedStatement fixedStatement)
		{
			foreach (var v in fixedStatement.Variables) {
				VisitAsSequencePoint(v);
			}
			VisitAsSequencePoint(fixedStatement.EmbeddedStatement);
		}

		public override void VisitTryCatchStatement(TryCatchStatement tryCatchStatement)
		{
			VisitAsSequencePoint(tryCatchStatement.TryBlock);
			foreach (var c in tryCatchStatement.CatchClauses) {
				VisitAsSequencePoint(c);
			}
			VisitAsSequencePoint(tryCatchStatement.FinallyBlock);
		}

		public override void VisitCatchClause(CatchClause catchClause)
		{
			if (catchClause.Condition.IsNull) {
				var tryCatchHandler = catchClause.Annotation<TryCatchHandler>();
				if (tryCatchHandler != null && !tryCatchHandler.ExceptionSpecifierILRange.IsEmpty) {
					StartSequencePoint(catchClause.CatchToken);
					var function = tryCatchHandler.Ancestors.OfType<ILFunction>().FirstOrDefault();
					AddToSequencePointRaw(function, new[] { tryCatchHandler.ExceptionSpecifierILRange });
					EndSequencePoint(catchClause.CatchToken.StartLocation, catchClause.RParToken.IsNull ? catchClause.CatchToken.EndLocation : catchClause.RParToken.EndLocation);
				}
			} else {
				StartSequencePoint(catchClause.WhenToken);
				AddToSequencePoint(catchClause.Condition);
				EndSequencePoint(catchClause.WhenToken.StartLocation, catchClause.CondRParToken.EndLocation);
			}
			VisitAsSequencePoint(catchClause.Body);
		}

		/// <summary>
		/// Start a new C# statement = new sequence point.
		/// </summary>
		void StartSequencePoint(AstNode primaryNode)
		{
			outerStates.Push(current);
			current = new StatePerSequencePoint(primaryNode);
		}

		void EndSequencePoint(TextLocation startLocation, TextLocation endLocation)
		{
			Debug.Assert(!startLocation.IsEmpty, "missing startLocation");
			Debug.Assert(!endLocation.IsEmpty, "missing endLocation");
			if (current.Intervals.Count > 0 && current.Function != null) {
				// use LongSet to deduplicate and merge the intervals
				var longSet = new LongSet(current.Intervals.Select(i => new LongInterval(i.Start, i.End)));
				Debug.Assert(!longSet.IsEmpty);
				sequencePoints.Add((current.Function, new DebugInfo.SequencePoint {
					Offset = (int)longSet.Intervals[0].Start,
					EndOffset = (int)longSet.Intervals[0].End,
					StartLine = startLocation.Line,
					StartColumn = startLocation.Column,
					EndLine = endLocation.Line,
					EndColumn = endLocation.Column
				}));
			}
			current = outerStates.Pop();
		}

		void AddToSequencePointRaw(ILFunction function, IEnumerable<Interval> ranges)
		{
			current.Intervals.AddRange(ranges);
			Debug.Assert(current.Function == null || current.Function == function);
			current.Function = function;
		}

		/// <summary>
		/// Add the ILAst instruction associated with the AstNode to the sequence point.
		/// Also add all its ILAst sub-instructions (unless they were already added to another sequence point).
		/// </summary>
		void AddToSequencePoint(AstNode node)
		{
			foreach (var inst in node.Annotations.OfType<ILInstruction>()) {
				AddToSequencePoint(inst);
			}
		}

		void AddToSequencePoint(ILInstruction inst)
		{
			if (!mappedInstructions.Add(inst)) {
				// inst was already used by a nested sequence point within this sequence point
				return;
			}
			// Add the IL range associated with this instruction to the current sequence point.
			if (HasUsableILRange(inst) && current.Intervals != null) {
				current.Intervals.AddRange(inst.ILRanges);
				var function = inst.Parent.Ancestors.OfType<ILFunction>().FirstOrDefault();
				Debug.Assert(current.Function == null || current.Function == function);
				current.Function = function;
			}

			// Do not add instructions of lambdas/delegates.
			if (inst is ILFunction)
				return;

			// Also add the child IL instructions, unless they were already processed by
			// another C# expression.
			foreach (var child in inst.Children) {
				AddToSequencePoint(child);
			}
		}

		internal static bool HasUsableILRange(ILInstruction inst)
		{
			if (inst.ILRangeIsEmpty)
				return false;
			return !(inst is BlockContainer || inst is Block);
		}

		/// <summary>
		/// Called after the visitor is done to return the results.
		/// </summary>
		internal Dictionary<ILFunction, List<DebugInfo.SequencePoint>> GetSequencePoints()
		{
			var dict = new Dictionary<ILFunction, List<DebugInfo.SequencePoint>>();
			foreach (var (function, sequencePoint) in this.sequencePoints) {
				if (!dict.TryGetValue(function, out var list)) {
					dict.Add(function, list = new List<DebugInfo.SequencePoint>());
				}
				list.Add(sequencePoint);
			}

			foreach (var (function, list) in dict.ToList()) {
				// For each function, sort sequence points and fix overlaps
				var newList = new List<DebugInfo.SequencePoint>();
				int pos = 0;
				IOrderedEnumerable<DebugInfo.SequencePoint> currFunctionSequencePoints = list.OrderBy(sp => sp.Offset).ThenBy(sp => sp.EndOffset);
				foreach (DebugInfo.SequencePoint sequencePoint in currFunctionSequencePoints) {
					if (sequencePoint.Offset < pos) {
						// overlapping sequence point?
						// delete previous sequence points that are after sequencePoint.Offset
						while (newList.Count > 0 && newList.Last().EndOffset > sequencePoint.Offset) {
							var last = newList.Last();
							if (last.Offset >= sequencePoint.Offset) {
								newList.RemoveAt(newList.Count - 1);
							} else {
								last.EndOffset = sequencePoint.Offset;
								newList[newList.Count - 1] = last;
							}
						}
					}

					newList.Add(sequencePoint);
					pos = sequencePoint.EndOffset;
				}
				// Add a hidden sequence point to account for the epilog of the function
				if (pos < function.CodeSize) {
					var hidden = new DebugInfo.SequencePoint();
					hidden.Offset = pos;
					hidden.EndOffset = function.CodeSize;
					hidden.SetHidden();
					newList.Add(hidden);
				}


				List<int> sequencePointCandidates = function.SequencePointCandidates;
				int currSPCandidateIndex = 0;

				for (int i = 0; i < newList.Count - 1; i++) {
					DebugInfo.SequencePoint currSequencePoint = newList[i];
					DebugInfo.SequencePoint nextSequencePoint = newList[i + 1];

					// Adjust the end offset of the current sequence point to the closest sequence point candidate
					// but do not create an overlapping sequence point. Moving the start of the current sequence
					// point is not required as it is 0 for the first sequence point and is moved during the last 
					// iteration for all others.
					while (currSPCandidateIndex < sequencePointCandidates.Count &&
						sequencePointCandidates[currSPCandidateIndex] < currSequencePoint.EndOffset) {
						currSPCandidateIndex++;
					}
					if (currSPCandidateIndex < sequencePointCandidates.Count && sequencePointCandidates[currSPCandidateIndex] <= nextSequencePoint.Offset) {
						currSequencePoint.EndOffset = sequencePointCandidates[currSPCandidateIndex];
					}

					// Adjust the start offset of the next sequence point to the closest previous sequence point candidate
					// but do not create an overlapping sequence point. 
					while (currSPCandidateIndex < sequencePointCandidates.Count &&
						sequencePointCandidates[currSPCandidateIndex] < nextSequencePoint.Offset) {
						currSPCandidateIndex++;
					}
					if (currSPCandidateIndex < sequencePointCandidates.Count && sequencePointCandidates[currSPCandidateIndex - 1] >= currSequencePoint.EndOffset) {
						nextSequencePoint.Offset = sequencePointCandidates[currSPCandidateIndex - 1];
						currSPCandidateIndex--;
					}

					// Fill in any gaps with a hidden sequence point
					if (currSequencePoint.EndOffset != nextSequencePoint.Offset) {
						SequencePoint newSP = new SequencePoint() { Offset = currSequencePoint.EndOffset, EndOffset = nextSequencePoint.Offset };
						newSP.SetHidden();
						newList.Insert(++i, newSP);
					}
				}
				dict[function] = newList;
			}			

			return dict;
		}
	}
}
