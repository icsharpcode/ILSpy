// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Insert variable declarations.
	/// </summary>
	public class DeclareVariables : IAstTransform
	{
		/// <summary>
		/// Represents a position immediately before nextNode.
		/// nextNode is either an ExpressionStatement in a BlockStatement, or an initializer in a for-loop.
		/// </summary>
		[DebuggerDisplay("level = {level}, nextNode = {nextNode}")]
		struct InsertionPoint
		{
			/// <summary>
			/// The nesting level of `nextNode` within the AST.
			/// Used to speed up FindCommonParent().
			/// </summary>
			internal int level;
			internal AstNode nextNode;
			
			/// <summary>Go up one level</summary>
			internal InsertionPoint Up()
			{
				return new InsertionPoint {
					level = level - 1,
					nextNode = nextNode.Parent
				};
			}
			
			internal InsertionPoint UpTo(int targetLevel)
			{
				InsertionPoint result = this;
				while (result.level > targetLevel) {
					result.nextNode = result.nextNode.Parent;
					result.level -= 1;
				}
				return result;
			}
		}
		
		[DebuggerDisplay("VariableToDeclare(Name={Name})")]
		class VariableToDeclare
		{
			public readonly ILVariable ILVariable;
			public IType Type => ILVariable.Type;
			public string Name => ILVariable.Name;

			/// <summary>
			/// Whether the variable needs to be default-initialized.
			/// </summary>
			public bool DefaultInitialization;
			
			/// <summary>
			/// Integer value that can be used to compare to VariableToDeclare instances
			/// to determine which variable was used first in the source code.
			/// 
			/// The variable with the lower SourceOrder value has the insertion point
			/// that comes first in the source code.
			/// </summary>
			public int SourceOrder;
			
			/// <summary>
			/// The insertion point, i.e. the node before which the variable declaration should be inserted.
			/// </summary>
			public InsertionPoint InsertionPoint;

			/// <summary>
			/// The first use of the variable.
			/// </summary>
			public IdentifierExpression FirstUse;

			public VariableToDeclare ReplacementDueToCollision;
			public bool InvolvedInCollision;
			public bool RemovedDueToCollision => ReplacementDueToCollision != null;
			public bool DeclaredInDeconstruction;

			public VariableToDeclare(ILVariable variable, bool defaultInitialization, InsertionPoint insertionPoint, IdentifierExpression firstUse, int sourceOrder)
			{
				this.ILVariable = variable;
				this.DefaultInitialization = defaultInitialization;
				this.InsertionPoint = insertionPoint;
				this.FirstUse = firstUse;
				this.SourceOrder = sourceOrder;
			}
		}
		
		readonly Dictionary<ILVariable, VariableToDeclare> variableDict = new Dictionary<ILVariable, VariableToDeclare>();
		TransformContext context;

		public void Run(AstNode rootNode, TransformContext context)
		{
			try {
				if (this.context != null)
					throw new InvalidOperationException("Reentrancy in DeclareVariables?");
				this.context = context;
				variableDict.Clear();
				EnsureExpressionStatementsAreValid(rootNode);
				FindInsertionPoints(rootNode, 0);
				ResolveCollisions();
				InsertDeconstructionVariableDeclarations();
				InsertVariableDeclarations(context);
				UpdateAnnotations(rootNode);
			} finally {
				this.context = null;
				variableDict.Clear();
			}
		}
		/// <summary>
		/// Analyze the input AST (containing undeclared variables)
		/// for where those variables would be declared by this transform.
		/// Analysis does not modify the AST.
		/// </summary>
		public void Analyze(AstNode rootNode)
		{
			variableDict.Clear();
			FindInsertionPoints(rootNode, 0);
			ResolveCollisions();
		}

		/// <summary>
		/// Get the position where the declaration for the variable will be inserted.
		/// </summary>
		public AstNode GetDeclarationPoint(ILVariable variable)
		{
			VariableToDeclare v = variableDict[variable];
			while (v.ReplacementDueToCollision != null) {
				v = v.ReplacementDueToCollision;
			}
			return v.InsertionPoint.nextNode;
		}

		/// <summary>
		/// Determines whether a variable was merged with other variables.
		/// </summary>
		public bool WasMerged(ILVariable variable)
		{
			VariableToDeclare v = variableDict[variable];
			return v.InvolvedInCollision || v.RemovedDueToCollision;
		}

		public void ClearAnalysisResults()
		{
			variableDict.Clear();
		}

		#region EnsureExpressionStatementsAreValid
		void EnsureExpressionStatementsAreValid(AstNode rootNode)
		{
			foreach (var stmt in rootNode.DescendantsAndSelf.OfType<ExpressionStatement>()) {
				if (!IsValidInStatementExpression(stmt.Expression)) {
					// fetch ILFunction
					var function = stmt.Ancestors.SelectMany(a => a.Annotations.OfType<ILFunction>()).First(f => f.Parent == null);
					// if possible use C# 7.0 discard-assignment
					if (context.Settings.Discards && !ExpressionBuilder.HidesVariableWithName(function, "_")) {
						stmt.Expression = new AssignmentExpression(
							new IdentifierExpression("_"), // no ResolveResult
							stmt.Expression.Detach());
					} else {
						// assign result to dummy variable
						var type = stmt.Expression.GetResolveResult().Type;
						var v = function.RegisterVariable(
							VariableKind.StackSlot,
							type,
							AssignVariableNames.GenerateVariableName(function, type, stmt.Expression.Annotations.OfType<ILInstruction>().Where(AssignVariableNames.IsSupportedInstruction).FirstOrDefault())
						);
						stmt.Expression = new AssignmentExpression(
							new IdentifierExpression(v.Name).WithRR(new ILVariableResolveResult(v, v.Type)),
							stmt.Expression.Detach());
					}
				}
			}
		}

		private static bool IsValidInStatementExpression(Expression expr)
		{
			switch (expr) {
				case InvocationExpression _:
				case ObjectCreateExpression _:
				case AssignmentExpression _:
				case ErrorExpression _:
					return true;
				case UnaryOperatorExpression uoe:
					switch (uoe.Operator) {
						case UnaryOperatorType.PostIncrement:
						case UnaryOperatorType.PostDecrement:
						case UnaryOperatorType.Increment:
						case UnaryOperatorType.Decrement:
						case UnaryOperatorType.Await:
							return true;
						case UnaryOperatorType.NullConditionalRewrap:
							return IsValidInStatementExpression(uoe.Expression);
						default:
							return false;
					}
				default:
					return false;
			}
		}
		#endregion

		#region FindInsertionPoints
		List<(InsertionPoint InsertionPoint, BlockContainer Scope)> scopeTracking = new List<(InsertionPoint, BlockContainer)>();

		/// <summary>
		/// Finds insertion points for all variables used within `node`
		/// and adds them to the variableDict.
		/// 
		/// `level` == nesting depth of `node` within root node.
		/// </summary>
		/// <remarks>
		/// Insertion point for a variable = common parent of all uses of that variable
		/// = smallest possible scope that contains all the uses of the variable
		/// </remarks>
		void FindInsertionPoints(AstNode node, int nodeLevel)
		{
			BlockContainer scope = node.Annotation<BlockContainer>();
			if (scope != null && (scope.EntryPoint.IncomingEdgeCount > 1 || scope.Parent is ILFunction)) {
				// track loops and function bodies as scopes, for comparison with CaptureScope.
				scopeTracking.Add((new InsertionPoint { level = nodeLevel, nextNode = node }, scope));
			} else {
				scope = null; // don't remove a scope if we didn't add one
			}
			try {
				for (AstNode child = node.FirstChild; child != null; child = child.NextSibling) {
					FindInsertionPoints(child, nodeLevel + 1);
				}
				if (node is IdentifierExpression identExpr) {
					var rr = identExpr.GetResolveResult() as ILVariableResolveResult;
					if (rr != null && VariableNeedsDeclaration(rr.Variable.Kind)) {
						FindInsertionPointForVariable(rr.Variable);
					} else if (identExpr.Annotation<ILFunction>() is ILFunction localFunction && localFunction.Kind == ILFunctionKind.LocalFunction) {
						foreach (var v in localFunction.CapturedVariables) {
							if (VariableNeedsDeclaration(v.Kind))
								FindInsertionPointForVariable(v);
						}
					}

					void FindInsertionPointForVariable(ILVariable variable)
					{
						InsertionPoint newPoint;
						int startIndex = scopeTracking.Count - 1;
						if (variable.CaptureScope != null && startIndex > 0 && variable.CaptureScope != scopeTracking[startIndex].Scope) {
							while (startIndex > 0 && scopeTracking[startIndex].Scope != variable.CaptureScope)
								startIndex--;
							newPoint = scopeTracking[startIndex + 1].InsertionPoint;
						} else {
							newPoint = new InsertionPoint { level = nodeLevel, nextNode = identExpr };
							if (variable.HasInitialValue) {
								// Uninitialized variables are logically initialized at the beginning of the function
								// Because it's possible that the variable has a loop-carried dependency,
								// declare it outside of any loops.
								while (startIndex >= 0) {
									if (scopeTracking[startIndex].Scope.EntryPoint.IncomingEdgeCount > 1) {
										// declare variable outside of loop
										newPoint = scopeTracking[startIndex].InsertionPoint;
									} else if (scopeTracking[startIndex].Scope.Parent is ILFunction) {
										// stop at beginning of function
										break;
									}
									startIndex--;
								}
							}
						}
						if (variableDict.TryGetValue(variable, out VariableToDeclare v)) {
							v.InsertionPoint = FindCommonParent(v.InsertionPoint, newPoint);
						} else {
							v = new VariableToDeclare(variable, variable.HasInitialValue,
								newPoint, identExpr, sourceOrder: variableDict.Count);
							variableDict.Add(variable, v);
						}
					}
				}
			} finally {
				if (scope != null)
					scopeTracking.RemoveAt(scopeTracking.Count - 1);
			}
		}

		internal static bool VariableNeedsDeclaration(VariableKind kind)
		{
			switch (kind) {
				case VariableKind.PinnedRegionLocal:
				case VariableKind.Parameter:
				case VariableKind.ExceptionLocal:
				case VariableKind.ExceptionStackSlot:
				case VariableKind.UsingLocal:
				case VariableKind.ForeachLocal:
					return false;
				default:
					return true;
			}
		}
		
		/// <summary>
		/// Finds an insertion point in a common parent instruction.
		/// </summary>
		InsertionPoint FindCommonParent(InsertionPoint oldPoint, InsertionPoint newPoint)
		{
			// First ensure we're looking at nodes on the same level:
			oldPoint = oldPoint.UpTo(newPoint.level);
			newPoint = newPoint.UpTo(oldPoint.level);
			Debug.Assert(newPoint.level == oldPoint.level);
			// Then go up the tree until both points share the same parent:
			while (oldPoint.nextNode.Parent != newPoint.nextNode.Parent) {
				oldPoint = oldPoint.Up();
				newPoint = newPoint.Up();
			}
			// return oldPoint as that one comes first in the source code
			return oldPoint;
		}
		#endregion
		
		/// <summary>
		/// Some variable declarations in C# are illegal (colliding),
		/// even though the variable live ranges are not overlapping.
		/// 
		/// Multiple declarations in same block:
		/// <code>
		/// int i = 1; use(1);
		/// int i = 2; use(2);
		/// </code>
		/// 
		/// "Hiding" declaration in nested block:
		/// <code>
		/// int i = 1; use(1);
		/// if (...) {
		///   int i = 2; use(2);
		/// }
		/// </code>
		/// 
		/// Nested blocks are illegal even if the parent block
		/// declares the variable later:
		/// <code>
		/// if (...) {
		///   int i = 1; use(i);
		/// }
		/// int i = 2; use(i);
		/// </code>
		/// 
		/// ResolveCollisions() detects all these cases, and combines the variable declarations
		/// to a single declaration that is usable for the combined scopes.
		/// </summary>
		void ResolveCollisions()
		{
			var multiDict = new MultiDictionary<string, VariableToDeclare>();
			foreach (var v in variableDict.Values) {
				// We can only insert variable declarations in blocks, but FindInsertionPoints() didn't
				// guarantee that it finds only blocks.
				// Fix that up now.
				while (!(v.InsertionPoint.nextNode.Parent is BlockStatement)) {
					if (v.InsertionPoint.nextNode.Parent is ForStatement f && v.InsertionPoint.nextNode == f.Initializers.FirstOrDefault() && IsMatchingAssignment(v, out _)) {
						// Special case: the initializer of a ForStatement can also declare a variable (with scope local to the for loop).
						break;
					}
					v.InsertionPoint = v.InsertionPoint.Up();
				}
				// Note: 'out var', pattern matching etc. is not considered a valid insertion point here, because the scope of the
				// resulting variable is not restricted to the parent node of the insertion point, but extends to the whole BlockStatement.
				// We moved up the insertion point to the whole BlockStatement so that we can resolve collisions,
				// later we might decide to declare the variable more locally (as 'out var') instead if still possible.

				// Go through all potentially colliding variables:
				foreach (var prev in multiDict[v.Name]) {
					if (prev.RemovedDueToCollision)
						continue;
					// Go up until both nodes are on the same level:
					InsertionPoint point1 = prev.InsertionPoint.UpTo(v.InsertionPoint.level);
					InsertionPoint point2 = v.InsertionPoint.UpTo(prev.InsertionPoint.level);
					Debug.Assert(point1.level == point2.level);
					if (point1.nextNode.Parent == point2.nextNode.Parent) {
						// We found a collision!
						v.InvolvedInCollision = true;
						prev.ReplacementDueToCollision = v;
						// Continue checking other entries in multiDict against the new position of `v`.
						if (prev.SourceOrder < v.SourceOrder) {
							// Switch v's insertion point to prev's insertion point:
							v.InsertionPoint = point1;
							// Since prev was first, it has the correct SourceOrder/FirstUse values
							// for the new combined variable:
							v.SourceOrder = prev.SourceOrder;
							v.FirstUse = prev.FirstUse;
						} else {
							// v is first in source order, so it keeps its old insertion point
							// (and other properties), except that the insertion point is
							// moved up to prev's level.
							v.InsertionPoint = point2;
						}
						v.DefaultInitialization |= prev.DefaultInitialization;
						// I think we don't need to re-check the dict entries that we already checked earlier,
						// because the new v.InsertionPoint only collides with another point x if either
						// the old v.InsertionPoint or the old prev.InsertionPoint already collided with x.
					}
				}
				
				multiDict.Add(v.Name, v);
			}
		}

		private void InsertDeconstructionVariableDeclarations()
		{
			var usedVariables = new HashSet<ILVariable>();
			foreach (var g in variableDict.Values.GroupBy(v => v.InsertionPoint.nextNode)) {
				if (!(g.Key is ExpressionStatement { Expression: AssignmentExpression { Left: TupleExpression left, Operator: AssignmentOperatorType.Assign } assignment }))
					continue;
				usedVariables.Clear();
				var deconstruct = assignment.Annotation<DeconstructInstruction>();
				if (deconstruct == null || deconstruct.Init.Count > 0 || deconstruct.Conversions.Instructions.Count > 0)
					continue;
				if (!deconstruct.Assignments.Instructions.All(IsDeclarableVariable))
					continue;

				var designation = StatementBuilder.TranslateDeconstructionDesignation(deconstruct, isForeach: false);
				left.ReplaceWith(new DeclarationExpression { Type = new SimpleType("var"), Designation = designation });

				foreach (var v in usedVariables) {
					variableDict[v].DeclaredInDeconstruction = true;
				}

				bool IsDeclarableVariable(ILInstruction inst)
				{
					if (!inst.MatchStLoc(out var v, out var value))
						return false;
					if (!g.Any(vd => vd.ILVariable == v))
						return false;
					if (!usedVariables.Add(v))
						return false;
					var expectedType = ((LdLoc)value).Variable.Type;
					if (!v.Type.Equals(expectedType))
						return false;
					if (!(v.Kind == VariableKind.StackSlot || v.Kind == VariableKind.Local))
						return false;
					return true;
				}
			}
		}

		bool IsMatchingAssignment(VariableToDeclare v, out AssignmentExpression assignment)
		{
			assignment = v.InsertionPoint.nextNode as AssignmentExpression;
			if (assignment == null) {
				assignment = (v.InsertionPoint.nextNode as ExpressionStatement)?.Expression as AssignmentExpression;
				if (assignment == null)
					return false;
			}
			return assignment.Operator == AssignmentOperatorType.Assign
				&& assignment.Left is IdentifierExpression identExpr
				&& identExpr.Identifier == v.Name
				&& identExpr.TypeArguments.Count == 0;
		}

		bool CombineDeclarationAndInitializer(VariableToDeclare v, TransformContext context)
		{
			if (v.Type.IsByRefLike)
				return true; // by-ref-like variables always must be initialized at their declaration.

			if (v.InsertionPoint.nextNode.Role == ForStatement.InitializerRole)
				return true; // for-statement initializers always should combine declaration and initialization.

			return !context.Settings.SeparateLocalVariableDeclarations;
		}

		void InsertVariableDeclarations(TransformContext context)
		{
			var replacements = new List<(AstNode, AstNode)>();
			foreach (var (ilVariable, v) in variableDict) {
				if (v.RemovedDueToCollision || v.DeclaredInDeconstruction)
					continue;

				if (CombineDeclarationAndInitializer(v, context) && IsMatchingAssignment(v, out AssignmentExpression assignment)) {
					// 'int v; v = expr;' can be combined to 'int v = expr;'
					AstType type;
					if (context.Settings.AnonymousTypes && v.Type.ContainsAnonymousType()) {
						type = new SimpleType("var");
					} else {
						type = context.TypeSystemAstBuilder.ConvertType(v.Type);
					}
					if (v.ILVariable.IsRefReadOnly && type is ComposedType composedType && composedType.HasRefSpecifier) {
						composedType.HasReadOnlySpecifier = true;
					}
					if (v.ILVariable.Kind == VariableKind.PinnedLocal) {
						type.InsertChildAfter(null, new Comment("pinned", CommentType.MultiLine), Roles.Comment);
					}
					var vds = new VariableDeclarationStatement(type, v.Name, assignment.Right.Detach());
					var init = vds.Variables.Single();
					init.AddAnnotation(assignment.Left.GetResolveResult());
					foreach (object annotation in assignment.Left.Annotations.Concat(assignment.Annotations)) {
						if (!(annotation is ResolveResult)) {
							init.AddAnnotation(annotation);
						}
					}
					replacements.Add((v.InsertionPoint.nextNode, vds));
				} else if (CanBeDeclaredAsOutVariable(v, out var dirExpr)) {
					// 'T v; SomeCall(out v);' can be combined to 'SomeCall(out var v);'
					AstType type;
					if (context.Settings.AnonymousTypes && v.Type.ContainsAnonymousType()) {
						type = new SimpleType("var");
					} else {
						type = context.TypeSystemAstBuilder.ConvertType(v.Type);
					}
					string name;
					// Variable is not used and discards are allowed, we can simplify this to 'out var _'.
					// TODO : if there are no other (non-out) variables named '_' and type is 'var', we can simplify this to 'out _'.
					// However, this needs overload resolution, so we currently cannot do that, as OR does not yet understand out var.
					// (for the specific rules see https://github.com/dotnet/roslyn/blob/master/docs/features/discards.md)
					if (context.Settings.Discards && v.ILVariable.LoadCount == 0 && v.ILVariable.StoreCount == 0 && v.ILVariable.AddressCount == 1) {
						name = "_";
					} else {
						name = v.Name;
					}
					var ovd = new OutVarDeclarationExpression(type, name);
					ovd.Variable.AddAnnotation(new ILVariableResolveResult(ilVariable));
					ovd.CopyAnnotationsFrom(dirExpr);
					replacements.Add((dirExpr, ovd));
				} else {
					// Insert a separate declaration statement.
					Expression initializer = null;
					AstType type = context.TypeSystemAstBuilder.ConvertType(v.Type);
					if (v.DefaultInitialization) {
						initializer = new DefaultValueExpression(type.Clone());
					}
					var vds = new VariableDeclarationStatement(type, v.Name, initializer);
					vds.Variables.Single().AddAnnotation(new ILVariableResolveResult(ilVariable));
					Debug.Assert(v.InsertionPoint.nextNode.Role == BlockStatement.StatementRole);
					v.InsertionPoint.nextNode.Parent.InsertChildBefore(
						v.InsertionPoint.nextNode,
						vds,
						BlockStatement.StatementRole);
				}
			}
			// perform replacements at end, so that we don't replace a node while it is still referenced by a VariableToDeclare
			foreach (var (oldNode, newNode) in replacements) {
				oldNode.ReplaceWith(newNode);
			}
		}

		private bool CanBeDeclaredAsOutVariable(VariableToDeclare v, out DirectionExpression dirExpr)
		{
			dirExpr = v.FirstUse.Parent as DirectionExpression;
			if (dirExpr == null || dirExpr.FieldDirection != FieldDirection.Out)
				return false;
			if (!context.Settings.OutVariables)
				return false;
			if (v.DefaultInitialization)
				return false;
			for (AstNode node = v.FirstUse; node != null; node = node.Parent) {
				if (node.Role == Roles.EmbeddedStatement) {
					return false;
				}
				switch (node) {
					case IfElseStatement _:  // variable declared in if condition appears in parent scope
					case ExpressionStatement _:
						return node == v.InsertionPoint.nextNode;
					case Statement _:
						return false; // other statements (e.g. while) don't allow variables to be promoted to parent scope
				}
			}
			return false;
		}

		/// <summary>
		/// Update ILVariableResolveResult annotations of all ILVariables that have been replaced by ResolveCollisions.
		/// </summary>
		void UpdateAnnotations(AstNode rootNode)
		{
			foreach (var node in rootNode.Descendants) {
				ILVariable ilVar;
				switch (node) {
					case IdentifierExpression id:
						ilVar = id.GetILVariable();
						break;
					case VariableInitializer vi:
						ilVar = vi.GetILVariable();
						break;
					default:
						continue;
				}
				if (ilVar == null || !VariableNeedsDeclaration(ilVar.Kind)) continue;
				var v = variableDict[ilVar];
				if (!v.RemovedDueToCollision) continue;
				while (v.RemovedDueToCollision) {
					v = v.ReplacementDueToCollision;
				}
				node.RemoveAnnotations<ILVariableResolveResult>();
				node.AddAnnotation(new ILVariableResolveResult(v.ILVariable, v.Type));
			}
		}
	}
}
