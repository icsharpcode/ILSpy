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
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// Converts "new Action(obj, ldftn(func))" into "new Action(obj.func)".
	/// For anonymous methods, creates an AnonymousMethodExpression.
	/// Also gets rid of any "Display Classes" left over after inlining an anonymous method.
	/// </summary>
	public class DelegateConstruction : ContextTrackingVisitor<object>, IAstTransform
	{
		internal sealed class Annotation
		{
			/// <summary>
			/// ldftn or ldvirtftn?
			/// </summary>
			public readonly bool IsVirtual;
			
			public readonly Expression InvocationTarget;
			public readonly string MethodName;
			
			public Annotation(bool isVirtual, Expression invocationTarget, string methodName)
			{
				this.IsVirtual = isVirtual;
				this.InvocationTarget = invocationTarget;
				this.MethodName = methodName;
			}
		}
		
		TransformContext context;
		List<string> currentlyUsedVariableNames = new List<string>();
		int nextLocalVariableIndex;
		
		public void Run(AstNode rootNode, TransformContext context)
		{
			this.context = context;
			base.Initialize(context);
			rootNode.AcceptVisitor(this);
		}
		
		public override object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression)
		{
			Annotation annotation = objectCreateExpression.Annotation<Annotation>();
			IMethod method = objectCreateExpression.GetSymbol() as IMethod;
			if (annotation != null && method != null) {
				if (HandleAnonymousMethod(objectCreateExpression, annotation.InvocationTarget, method))
					return null;
			}
			return base.VisitObjectCreateExpression(objectCreateExpression);
		}

		internal static bool IsAnonymousMethod(ITypeDefinition decompiledTypeDefinition, IMethod method)
		{
			if (method == null || !(method.HasGeneratedName() || method.Name.Contains("$")))
				return false;
			if (!(method.IsCompilerGenerated() || IsPotentialClosure(decompiledTypeDefinition, method.DeclaringTypeDefinition)))
				return false;
			return true;
		}
		
		bool HandleAnonymousMethod(ObjectCreateExpression objectCreateExpression, Expression target, IMethod method)
		{
			if (!context.Settings.AnonymousMethods)
				return false; // anonymous method decompilation is disabled
			if (target != null && !(target is TypeReferenceExpression || target is IdentifierExpression || target is ThisReferenceExpression || target is NullReferenceExpression))
				return false; // don't copy arbitrary expressions, deal with identifiers only
			if (!IsAnonymousMethod(context.DecompiledTypeDefinition, method))
				return false;

			// Create AnonymousMethodExpression and prepare parameters
			AnonymousMethodExpression ame = new AnonymousMethodExpression();
			ame.CopyAnnotationsFrom(objectCreateExpression); // copy ILRanges etc.
			ame.RemoveAnnotations<MethodReference>(); // remove reference to delegate ctor
			ame.AddAnnotation(method); // add reference to anonymous method
			ame.Parameters.AddRange(MakeParameters(method));
			ame.HasParameterList = true;
			
			// rename variables so that they don't conflict with the parameters:
			foreach (ParameterDeclaration pd in ame.Parameters) {
				EnsureVariableNameIsAvailable(objectCreateExpression, pd.Name);
			}
			
			// Decompile the anonymous method:
			var body = DecompileBody(method);

			bool isLambda = false;
			if (ame.Parameters.All(p => p.ParameterModifier == ParameterModifier.None)) {
				isLambda = (body.Statements.Count == 1 && body.Statements.Single() is ReturnStatement);
			}
			// Remove the parameter list from an AnonymousMethodExpression if the original method had no names,
			// and the parameters are not used in the method body
			if (!isLambda && method.Parameters.All(p => string.IsNullOrEmpty(p.Name))) {
				var parameterReferencingIdentifiers =
					from ident in body.Descendants.OfType<IdentifierExpression>()
					let v = ident.Annotation<ILVariable>()
					where v != null && v.Kind == VariableKind.Parameter
					select ident;
				if (!parameterReferencingIdentifiers.Any()) {
					ame.Parameters.Clear();
					ame.HasParameterList = false;
				}
			}
			
			// Replace all occurrences of 'this' in the method body with the delegate's target:
			foreach (AstNode node in body.Descendants) {
				if (node is ThisReferenceExpression)
					node.ReplaceWith(target.Clone());
			}
			Expression replacement;
			if (isLambda) {
				LambdaExpression lambda = new LambdaExpression();
				lambda.CopyAnnotationsFrom(ame);
				ame.Parameters.MoveTo(lambda.Parameters);
				Expression returnExpr = ((ReturnStatement)body.Statements.Single()).Expression;
				returnExpr.Remove();
				lambda.Body = returnExpr;
				replacement = lambda;
			} else {
				ame.Body = body;
				replacement = ame;
			}
			var expectedType = objectCreateExpression.GetResolveResult()?.Type.GetDefinition();
			if (expectedType != null && expectedType.Kind != TypeKind.Delegate) {
				var simplifiedDelegateCreation = (ObjectCreateExpression)objectCreateExpression.Clone();
				simplifiedDelegateCreation.Arguments.Clear();
				simplifiedDelegateCreation.Arguments.Add(replacement);
				replacement = simplifiedDelegateCreation;
			}
			objectCreateExpression.ReplaceWith(replacement);
			return true;
		}

		IEnumerable<ParameterDeclaration> MakeParameters(IMethod method)
		{
			foreach (var parameter in method.Parameters) {
				var pd = context.TypeSystemAstBuilder.ConvertParameter(parameter);
				if (parameter.Type.ContainsAnonymousType())
					pd.Type = null;
				yield return pd;
			}
		}
		
		BlockStatement DecompileBody(IMethod method)
		{
//			subContext.ReservedVariableNames.AddRange(currentlyUsedVariableNames);
			return new CSharpDecompiler(context.TypeSystem, context.Settings).DecompileLambdaBody(method);
		}
		
		internal static bool IsPotentialClosure(ITypeDefinition decompiledTypeDefinition, ITypeDefinition potentialDisplayClass)
		{
			if (potentialDisplayClass == null || !potentialDisplayClass.IsCompilerGeneratedOrIsInCompilerGeneratedClass())
				return false;
			// check that methodContainingType is within containingType
			while (potentialDisplayClass != decompiledTypeDefinition) {
				potentialDisplayClass = potentialDisplayClass.DeclaringTypeDefinition;
				if (potentialDisplayClass == null)
					return false;
			}
			return true;
		}
		/*
		public override object VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			if (context.Settings.ExpressionTrees && ExpressionTreeConverter.CouldBeExpressionTree(invocationExpression)) {
				Expression converted = ExpressionTreeConverter.TryConvert(context, invocationExpression);
				if (converted != null) {
					invocationExpression.ReplaceWith(converted);
					return converted.AcceptVisitor(this);
				}
			}
			return base.VisitInvocationExpression(invocationExpression);
		}
		 */
		#region Track current variables
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration)
		{
			Debug.Assert(currentlyUsedVariableNames.Count == 0);
			try {
				nextLocalVariableIndex = methodDeclaration.Body.Annotation<ICollection<ILVariable>>()?.Where(v => v.Kind == VariableKind.Local).MaxOrDefault(v => v.Index) + 1 ?? 0;
				currentlyUsedVariableNames.AddRange(methodDeclaration.Parameters.Select(p => p.Name));
				return base.VisitMethodDeclaration(methodDeclaration);
			} finally {
				currentlyUsedVariableNames.Clear();
			}
		}
		
		public override object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration)
		{
			Debug.Assert(currentlyUsedVariableNames.Count == 0);
			try {
				nextLocalVariableIndex = operatorDeclaration.Body.Annotation<ICollection<ILVariable>>()?.Where(v => v.Kind == VariableKind.Local).MaxOrDefault(v => v.Index) + 1 ?? 0;
				currentlyUsedVariableNames.AddRange(operatorDeclaration.Parameters.Select(p => p.Name));
				return base.VisitOperatorDeclaration(operatorDeclaration);
			} finally {
				currentlyUsedVariableNames.Clear();
			}
		}
		
		public override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
		{
			Debug.Assert(currentlyUsedVariableNames.Count == 0);
			try {
				nextLocalVariableIndex = constructorDeclaration.Body.Annotation<ICollection<ILVariable>>()?.Where(v => v.Kind == VariableKind.Local).MaxOrDefault(v => v.Index) + 1 ?? 0;
				currentlyUsedVariableNames.AddRange(constructorDeclaration.Parameters.Select(p => p.Name));
				return base.VisitConstructorDeclaration(constructorDeclaration);
			} finally {
				currentlyUsedVariableNames.Clear();
			}
		}
		
		public override object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration)
		{
			Debug.Assert(currentlyUsedVariableNames.Count == 0);
			try {
				currentlyUsedVariableNames.AddRange(indexerDeclaration.Parameters.Select(p => p.Name));
				return base.VisitIndexerDeclaration(indexerDeclaration);
			} finally {
				currentlyUsedVariableNames.Clear();
			}
		}
		
		public override object VisitAccessor(Accessor accessor)
		{
			try {
				nextLocalVariableIndex = accessor.Body.Annotation<ICollection<ILVariable>>()?.Where(v => v.Kind == VariableKind.Local).MaxOrDefault(v => v.Index) + 1 ?? 0;
				currentlyUsedVariableNames.Add("value");
				return base.VisitAccessor(accessor);
			} finally {
				currentlyUsedVariableNames.RemoveAt(currentlyUsedVariableNames.Count - 1);
			}
		}
		
		public override object VisitVariableDeclarationStatement(VariableDeclarationStatement variableDeclarationStatement)
		{
			foreach (VariableInitializer v in variableDeclarationStatement.Variables)
				currentlyUsedVariableNames.Add(v.Name);
			return base.VisitVariableDeclarationStatement(variableDeclarationStatement);
		}
		
		public override object VisitFixedStatement(FixedStatement fixedStatement)
		{
			foreach (VariableInitializer v in fixedStatement.Variables)
				currentlyUsedVariableNames.Add(v.Name);
			return base.VisitFixedStatement(fixedStatement);
		}
		#endregion
		
		static readonly ExpressionStatement displayClassAssignmentPattern =
			new ExpressionStatement(new AssignmentExpression(
				new NamedNode("variable", new IdentifierExpression(Pattern.AnyString)),
				new ObjectCreateExpression { Type = new AnyNode("type") }
			));

		public override object VisitBlockStatement(BlockStatement blockStatement)
		{
			int numberOfVariablesOutsideBlock = currentlyUsedVariableNames.Count;
			base.VisitBlockStatement(blockStatement);
			foreach (ExpressionStatement stmt in blockStatement.Statements.OfType<ExpressionStatement>().ToArray()) {
				Match displayClassAssignmentMatch = displayClassAssignmentPattern.Match(stmt);
				if (!displayClassAssignmentMatch.Success)
					continue;
				
				ILVariable variable = displayClassAssignmentMatch.Get<AstNode>("variable").Single().Annotation<ILVariableResolveResult>()?.Variable;
				if (variable == null)
					continue;
				var type = variable.Type.GetDefinition();
				if (!IsPotentialClosure(context.DecompiledTypeDefinition, type))
					continue;
				if (!(displayClassAssignmentMatch.Get<AstType>("type").Single().GetSymbol() as IType).GetDefinition().Equals(type))
					continue;
				
				// Looks like we found a display class creation. Now let's verify that the variable is used only for field accesses:
				bool ok = true;
				foreach (var identExpr in blockStatement.Descendants.OfType<IdentifierExpression>()) {
					if (identExpr.Identifier == variable.Name && identExpr != displayClassAssignmentMatch.Get("variable").Single()) {
						if (!(identExpr.Parent is MemberReferenceExpression && identExpr.Parent.GetSymbol() is IField))
							ok = false;
					}
				}
				if (!ok)
					continue;
				Dictionary<IField, AstNode> dict = new Dictionary<IField, AstNode>();
				
				// Delete the variable declaration statement:
				VariableDeclarationStatement displayClassVarDecl = PatternStatementTransform.FindVariableDeclaration(stmt, variable.Name);
				if (displayClassVarDecl != null)
					displayClassVarDecl.Remove();
				
				// Delete the assignment statement:
				AstNode cur = stmt.NextSibling;
				stmt.Remove();
				
				// Delete any following statements as long as they assign parameters to the display class
				BlockStatement rootBlock = blockStatement.Ancestors.OfType<BlockStatement>().LastOrDefault() ?? blockStatement;
				List<ILVariable> parameterOccurrances = rootBlock.Descendants.OfType<IdentifierExpression>()
					.Select(n => n.Annotation<ILVariable>()).Where(p => p != null && p.Kind == VariableKind.Parameter).ToList();
				AstNode next;
				for (; cur != null; cur = next) {
					next = cur.NextSibling;
					
					// Test for the pattern:
					// "variableName.MemberName = right;"
					ExpressionStatement closureFieldAssignmentPattern = new ExpressionStatement(
						new AssignmentExpression(
							new NamedNode("left", new MemberReferenceExpression {
							              	Target = new IdentifierExpression(variable.Name),
							              	MemberName = Pattern.AnyString
							              }),
							new AnyNode("right")
						)
					);
					Match m = closureFieldAssignmentPattern.Match(cur);
					if (m.Success) {
						AstNode right = m.Get<AstNode>("right").Single();
						bool isParameter = false;
						bool isDisplayClassParentPointerAssignment = false;
						if (right is ThisReferenceExpression) {
							isParameter = true;
						} else if (right is IdentifierExpression) {
							// handle parameters only if the whole method contains no other occurrence except for 'right'
							ILVariable v = right.Annotation<ILVariable>();
							isParameter = v.Kind == VariableKind.Parameter && parameterOccurrances.Count(c => c == v) == 1;
							if (!isParameter && IsPotentialClosure(context.DecompiledTypeDefinition, v.Type.GetDefinition())) {
								// parent display class within the same method
								// (closure2.localsX = closure1;)
								isDisplayClassParentPointerAssignment = true;
							}
						} else if (right is MemberReferenceExpression) {
							// copy of parent display class reference from an outer lambda
							// closure2.localsX = this.localsY
							MemberReferenceExpression mre = m.Get<MemberReferenceExpression>("right").Single();
							do {
								// descend into the targets of the mre as long as the field types are closures
								var fieldDef2 = mre.GetSymbol() as IField;
								if (fieldDef2 == null || !IsPotentialClosure(context.DecompiledTypeDefinition, fieldDef2.Type.GetDefinition())) {
									break;
								}
								// if we finally get to a this reference, it's copying a display class parent pointer
								if (mre.Target is ThisReferenceExpression) {
									isDisplayClassParentPointerAssignment = true;
								}
								mre = mre.Target as MemberReferenceExpression;
							} while (mre != null);
						}
						var field = m.Get<MemberReferenceExpression>("left").Single().GetSymbol() as IField;
						if (field != null && (isParameter || isDisplayClassParentPointerAssignment)) {
							dict[field] = right;
							cur.Remove();
						} else {
							break;
						}
					} else {
						break;
					}
				}
				
				// Now create variables for all fields of the display class (except for those that we already handled as parameters)
				List<Tuple<AstType, ILVariable>> variablesToDeclare = new List<Tuple<AstType, ILVariable>>();
				foreach (var field in type.Fields) {
					if (field.IsStatic)
						continue; // skip static fields
					if (dict.ContainsKey(field)) // skip field if it already was handled as parameter
						continue;
					string capturedVariableName = field.Name;
					if (capturedVariableName.StartsWith("$VB$Local_", StringComparison.Ordinal) && capturedVariableName.Length > 10)
						capturedVariableName = capturedVariableName.Substring(10);
					EnsureVariableNameIsAvailable(blockStatement, capturedVariableName);
					currentlyUsedVariableNames.Add(capturedVariableName);
					ILVariable ilVar = new ILVariable(VariableKind.Local, field.Type, nextLocalVariableIndex++)
					{
						Name = capturedVariableName
					};
					variablesToDeclare.Add(Tuple.Create(context.TypeSystemAstBuilder.ConvertType(field.Type), ilVar));
					dict[field] = new IdentifierExpression(capturedVariableName).WithAnnotation(ilVar);
				}
				
				// Now figure out where the closure was accessed and use the simpler replacement expression there:
				foreach (var identExpr in blockStatement.Descendants.OfType<IdentifierExpression>()) {
					if (identExpr.Identifier == variable.Name) {
						MemberReferenceExpression mre = (MemberReferenceExpression)identExpr.Parent;
						AstNode replacement;
						if (dict.TryGetValue((IField)mre.GetSymbol(), out replacement)) {
							mre.ReplaceWith(replacement.Clone());
						}
					}
				}
				// Now insert the variable declarations (we can do this after the replacements only so that the scope detection works):
				Statement insertionPoint = blockStatement.Statements.FirstOrDefault();
				foreach (var tuple in variablesToDeclare) {
					var newVarDecl = new VariableDeclarationStatement(tuple.Item1, tuple.Item2.Name);
					newVarDecl.Variables.Single().AddAnnotation(new CapturedVariableAnnotation());
					newVarDecl.Variables.Single().AddAnnotation(tuple.Item2);
					blockStatement.Statements.InsertBefore(insertionPoint, newVarDecl);
				}
			}
			currentlyUsedVariableNames.RemoveRange(numberOfVariablesOutsideBlock, currentlyUsedVariableNames.Count - numberOfVariablesOutsideBlock);
			return null;
		}

		void EnsureVariableNameIsAvailable(AstNode currentNode, string name)
		{
			int pos = currentlyUsedVariableNames.IndexOf(name);
			if (pos < 0) {
				// name is still available
				return;
			}
			throw new NotImplementedException("naming conflict: " + name);
		}/*
			// Naming conflict. Let's rename the existing variable so that the field keeps the name from metadata.
			NameVariables nv = new NameVariables();
			// Add currently used variable and parameter names
			foreach (string nameInUse in currentlyUsedVariableNames)
				nv.AddExistingName(nameInUse);
			// variables declared in child nodes of this block
			foreach (VariableInitializer vi in currentNode.Descendants.OfType<VariableInitializer>())
				nv.AddExistingName(vi.Name);
			// parameters in child lambdas
			foreach (ParameterDeclaration pd in currentNode.Descendants.OfType<ParameterDeclaration>())
				nv.AddExistingName(pd.Name);
			
			string newName = nv.GetAlternativeName(name);
			currentlyUsedVariableNames[pos] = newName;
			
			// find top-most block
			AstNode topMostBlock = currentNode.Ancestors.OfType<BlockStatement>().LastOrDefault() ?? currentNode;
			
			// rename identifiers
			foreach (IdentifierExpression ident in topMostBlock.Descendants.OfType<IdentifierExpression>()) {
				if (ident.Identifier == name) {
					ident.Identifier = newName;
					ILVariable v = ident.Annotation<ILVariable>();
					if (v != null)
						v.Name = newName;
				}
			}
			// rename variable declarations
			foreach (VariableInitializer vi in topMostBlock.Descendants.OfType<VariableInitializer>()) {
				if (vi.Name == name) {
					vi.Name = newName;
					ILVariable v = vi.Annotation<ILVariable>();
					if (v != null)
						v.Name = newName;
				}
			}
		}*/
	}
}
