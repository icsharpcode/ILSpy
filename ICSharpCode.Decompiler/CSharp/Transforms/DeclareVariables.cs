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
using System.Threading;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Analysis;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.Utils;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Insert variable declarations.
	/// </summary>
	public class DeclareVariables : IAstTransform
	{
		struct InsertionPoint
		{
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
		
		class VariableToDeclare
		{
			public readonly IType Type;
			public readonly string Name;
			/// <summary>
			/// Whether the variable needs to be default-initialized.
			/// </summary>
			public readonly bool DefaultInitialization;
			
			/// <summary>
			/// Integer value that can be used to compare to VariableToDeclare instances
			/// to determine which variable was used first in the source code.
			/// 
			/// Assuming both insertion points are on the same level, the variable
			/// with the lower SourceOrder value has the insertion point that comes
			/// first in the source code.
			/// </summary>
			public int SourceOrder;
			
			public InsertionPoint InsertionPoint;
			
			public bool RemovedDueToCollision;
			
			public VariableToDeclare(IType type, string name, bool defaultInitialization, InsertionPoint insertionPoint, int sourceOrder)
			{
				this.Type = type;
				this.Name = name;
				this.DefaultInitialization = defaultInitialization;
				this.InsertionPoint = insertionPoint;
				this.SourceOrder = sourceOrder;
			}
		}
		
		Dictionary<ILVariable, VariableToDeclare> variableDict = new Dictionary<ILVariable, VariableToDeclare>();
		
		TransformContext context;
		
		public void Run(AstNode rootNode, TransformContext context)
		{
			try {
				this.context = context;
				FindInsertionPoints(rootNode, 0);
				ResolveOverlap();
				InsertVariableDeclarations();
			} finally {
				variableDict.Clear();
				this.context = null;
			}
		}
		
		#region FindInsertionPoints
		/// <summary>
		/// Finds insertion points for all variables used within `node`
		/// and adds them to the variableDict.
		/// 
		/// `level` == nesting depth of `node` within root node.
		/// </summary>
		void FindInsertionPoints(AstNode node, int nodeLevel)
		{
			for (AstNode child = node.FirstChild; child != null; child = child.NextSibling) {
				FindInsertionPoints(child, nodeLevel + 1);
			}
			var identExpr = node as IdentifierExpression;
			if (identExpr != null) {
				var rr = identExpr.GetResolveResult() as ILVariableResolveResult;
				if (rr != null && rr.Variable.Kind != VariableKind.Parameter && rr.Variable.Kind != VariableKind.Exception) {
					var newPoint = new InsertionPoint { level = nodeLevel, nextNode = identExpr };
					VariableToDeclare v;
					if (variableDict.TryGetValue(rr.Variable, out v)) {
						v.InsertionPoint = FindCommonParent(v.InsertionPoint, newPoint);
					} else {
						v = new VariableToDeclare(
							rr.Variable.Type, rr.Variable.Name, rr.Variable.HasInitialValue,
							newPoint, sourceOrder: variableDict.Count);
						variableDict.Add(rr.Variable, v);
					}
				}
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
		
		void ResolveOverlap()
		{
			var multiDict = new MultiDictionary<string, VariableToDeclare>();
			foreach (var v in variableDict.Values) {
				// Go up to the next BlockStatement (we can't add variable declarations anywhere else in the AST)
				while (!(v.InsertionPoint.nextNode.Parent is BlockStatement)) {
					v.InsertionPoint = v.InsertionPoint.Up();
				}
				
				// Go through all potentially colliding variables:
				foreach (var prev in multiDict[v.Name]) {
					if (prev.RemovedDueToCollision)
						continue;
					// Go up until both nodes are on the same level:
					InsertionPoint point1 = prev.InsertionPoint.UpTo(v.InsertionPoint.level);
					InsertionPoint point2 = v.InsertionPoint.UpTo(prev.InsertionPoint.level);
					if (point1.nextNode.Parent == point2.nextNode.Parent) {
						// We found a collision!
						prev.RemovedDueToCollision = true;
						// Continue checking other entries in multiDict against the new position of `v`.
						if (prev.SourceOrder < v.SourceOrder) {
							// If we switch v's insertion point to prev's insertion point,
							// we also need to copy prev's SourceOrder value.
							v.InsertionPoint = point1;
							v.SourceOrder = prev.SourceOrder;
						} else {
							v.InsertionPoint = point2;
						}
						// I think we don't need to re-check the dict entries that we already checked earlier,
						// because the new v.InsertionPoint only collides with another point x if either
						// the old v.InsertionPoint or the old prev.InsertionPoint already collided with x.
					}
				}
				
				multiDict.Add(v.Name, v);
			}
		}

		void InsertVariableDeclarations()
		{
			var replacements = new List<KeyValuePair<AstNode, AstNode>>();
			foreach (var v in variableDict.Values) {
				if (v.RemovedDueToCollision)
					continue;
				
				AstType type = context.TypeSystemAstBuilder.ConvertType(v.Type);
				
				var boe = v.InsertionPoint.nextNode as BinaryOperatorExpression;
				if (boe != null && boe.Left.IsMatch(new IdentifierExpression(v.Name))) {
					
					var vds = new VariableDeclarationStatement(type, v.Name, boe.Right.Detach());
					var init = vds.Variables.Single();
					init.AddAnnotation(boe.Left.GetResolveResult());
					foreach (object annotation in boe.Left.Annotations.Concat(boe.Annotations)) {
						if (!(annotation is ResolveResult)) {
							init.AddAnnotation(annotation);
						}
					}
					replacements.Add(new KeyValuePair<AstNode, AstNode>(v.InsertionPoint.nextNode, vds));
				} else {
					Expression initializer = null;
					if (v.DefaultInitialization) {
						initializer = new DefaultValueExpression(type.Clone());
					}
					v.InsertionPoint.nextNode.Parent.InsertChildBefore(
						v.InsertionPoint.nextNode,
						new VariableDeclarationStatement(type, v.Name, initializer),
						BlockStatement.StatementRole);
				}
			}
			// perform replacements at end, so that we don't replace a node while it is still referenced by a VariableToDeclare
			foreach (var pair in replacements) {
				pair.Key.ReplaceWith(pair.Value);
			}
		}
	}
}
