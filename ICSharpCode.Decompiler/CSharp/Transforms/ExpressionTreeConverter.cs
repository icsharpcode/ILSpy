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
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.CSharp.TypeSystem;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.TypeSystem;
using Mono.Cecil;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	public class ExpressionTreeConverter : DepthFirstAstVisitor, IAstTransform
	{
		TransformContext context;
		TypeSystemAstBuilder astBuilder;

		public ExpressionTreeConverter()
		{
		}

		public void Run(AstNode rootNode, TransformContext context)
		{
			this.context = context;
			InitializeAstBuilderAndContext(rootNode.Annotation<UsingScope>());
			rootNode.AcceptVisitor(this);
		}

		#region TypeSystemAstBuilder context handling
		Stack<CSharpTypeResolveContext> resolveContextStack = new Stack<CSharpTypeResolveContext>();

		static TypeSystemAstBuilder CreateAstBuilder(CSharpTypeResolveContext context)
		{
			return new TypeSystemAstBuilder(new CSharpResolver(context)) {
				AddResolveResultAnnotations = true,
				UseAliases = true
			};
		}

		void InitializeAstBuilderAndContext(UsingScope usingScope)
		{
			this.resolveContextStack = new Stack<CSharpTypeResolveContext>();
			if (!string.IsNullOrEmpty(context.DecompiledTypeDefinition?.Namespace)) {
				foreach (string ns in context.DecompiledTypeDefinition.Namespace.Split('.')) {
					usingScope = new UsingScope(usingScope, ns);
				}
			}
			var currentContext = new CSharpTypeResolveContext(context.TypeSystem.MainAssembly, usingScope.Resolve(context.TypeSystem.Compilation), context.DecompiledTypeDefinition);
			this.resolveContextStack.Push(currentContext);
			this.astBuilder = CreateAstBuilder(currentContext);
		}

		public override void VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration)
		{
			var previousContext = resolveContextStack.Peek();
			var usingScope = previousContext.CurrentUsingScope.UnresolvedUsingScope;
			foreach (string ident in namespaceDeclaration.Identifiers) {
				usingScope = new UsingScope(usingScope, ident);
			}
			var currentContext = new CSharpTypeResolveContext(previousContext.CurrentAssembly, usingScope.Resolve(previousContext.Compilation));
			resolveContextStack.Push(currentContext);
			try {
				astBuilder = CreateAstBuilder(currentContext);
				base.VisitNamespaceDeclaration(namespaceDeclaration);
			} finally {
				astBuilder = CreateAstBuilder(previousContext);
				resolveContextStack.Pop();
			}
		}

		public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
		{
			var previousContext = resolveContextStack.Peek();
			var currentContext = previousContext.WithCurrentTypeDefinition(typeDeclaration.GetSymbol() as ITypeDefinition);
			resolveContextStack.Push(currentContext);
			try {
				astBuilder = CreateAstBuilder(currentContext);
				base.VisitTypeDeclaration(typeDeclaration);
			} finally {
				astBuilder = CreateAstBuilder(previousContext);
				resolveContextStack.Pop();
			}
		}
		#endregion

		public override void VisitInvocationExpression(InvocationExpression invocationExpression)
		{
			if (CouldBeExpressionTree(invocationExpression)) {
				Expression newExpression = Convert(invocationExpression);
				if (newExpression != null) {
					invocationExpression.ReplaceWith(newExpression);
				}
			}
			VisitChildren(invocationExpression);
		}

		#region static TryConvert method
		public static bool CouldBeExpressionTree(InvocationExpression expr)
		{
			if (expr != null && expr.Arguments.Count == 2) {
				IMethod mr = expr.GetSymbol() as IMethod;
				return mr != null && mr.Name == "Lambda" && mr.DeclaringType.FullName == "System.Linq.Expressions.Expression";
			}
			return false;
		}

		#endregion

		Stack<LambdaExpression> activeLambdas = new Stack<LambdaExpression>();

		#region Main Convert method
		Expression Convert(Expression expr)
		{
			if (expr is InvocationExpression invocation) {
				if (invocation.GetSymbol() is IMethod mr && mr.DeclaringType.FullName == "System.Linq.Expressions.Expression") {
					switch (mr.Name) {
						case "Add":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.Add, false);
						case "AddChecked":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.Add, true);
						case "AddAssign":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.Add, false);
						case "AddAssignChecked":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.Add, true);
						case "And":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.BitwiseAnd);
						case "AndAlso":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.ConditionalAnd);
						case "AndAssign":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.BitwiseAnd);
						case "ArrayAccess":
						case "ArrayIndex":
							return ConvertArrayIndex(invocation);
						case "ArrayLength":
							return ConvertArrayLength(invocation);
						case "Assign":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.Assign);
						case "Call":
							return ConvertCall(invocation);
						case "Coalesce":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.NullCoalescing);
						case "Condition":
							return ConvertCondition(invocation);
						case "Constant":
							if (invocation.Arguments.Count >= 1)
								return invocation.Arguments.First().Clone();
							else
								return NotSupported(expr);
						case "Convert":
							return ConvertCast(invocation, false);
						case "ConvertChecked":
							return ConvertCast(invocation, true);
						case "Divide":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.Divide);
						case "DivideAssign":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.Divide);
						case "Equal":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.Equality);
						case "ExclusiveOr":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.ExclusiveOr);
						case "ExclusiveOrAssign":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.ExclusiveOr);
						case "Field":
							return ConvertField(invocation);
						case "GreaterThan":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.GreaterThan);
						case "GreaterThanOrEqual":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.GreaterThanOrEqual);
						case "Invoke":
							return ConvertInvoke(invocation);
						case "Lambda":
							return ConvertLambda(invocation);
						case "LeftShift":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.ShiftLeft);
						case "LeftShiftAssign":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.ShiftLeft);
						case "LessThan":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.LessThan);
						case "LessThanOrEqual":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.LessThanOrEqual);
						case "ListInit":
							return ConvertListInit(invocation);
						case "MemberInit":
							return ConvertMemberInit(invocation);
						case "Modulo":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.Modulus);
						case "ModuloAssign":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.Modulus);
						case "Multiply":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.Multiply, false);
						case "MultiplyChecked":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.Multiply, true);
						case "MultiplyAssign":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.Multiply, false);
						case "MultiplyAssignChecked":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.Multiply, true);
						case "Negate":
							return ConvertUnaryOperator(invocation, UnaryOperatorType.Minus, false);
						case "NegateChecked":
							return ConvertUnaryOperator(invocation, UnaryOperatorType.Minus, true);
						case "New":
							return ConvertNewObject(invocation);
						case "NewArrayBounds":
							return ConvertNewArrayBounds(invocation);
						case "NewArrayInit":
							return ConvertNewArrayInit(invocation);
						case "Not":
							return ConvertUnaryOperator(invocation, UnaryOperatorType.Not);
						case "NotEqual":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.InEquality);
						case "OnesComplement":
							return ConvertUnaryOperator(invocation, UnaryOperatorType.BitNot);
						case "Or":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.BitwiseOr);
						case "OrAssign":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.BitwiseOr);
						case "OrElse":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.ConditionalOr);
						case "Parameter":
							return new IdentifierExpression(ConvertParameter(invocation).Name);
						case "Property":
							return ConvertProperty(invocation);
						case "Quote":
							if (invocation.Arguments.Count == 1)
								return Convert(invocation.Arguments.Single());
							else
								return NotSupported(invocation);
						case "RightShift":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.ShiftRight);
						case "RightShiftAssign":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.ShiftRight);
						case "Subtract":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.Subtract, false);
						case "SubtractChecked":
							return ConvertBinaryOperator(invocation, BinaryOperatorType.Subtract, true);
						case "SubtractAssign":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.Subtract, false);
						case "SubtractAssignChecked":
							return ConvertAssignmentOperator(invocation, AssignmentOperatorType.Subtract, true);
						case "TypeAs":
							return ConvertTypeAs(invocation);
						case "TypeIs":
							return ConvertTypeIs(invocation);
					}
				}
			}
			if (expr is IdentifierExpression ident) {
				if (ident.GetSymbol() is ILVariable v) {
					foreach (LambdaExpression lambda in activeLambdas) {
						foreach (ParameterDeclaration p in lambda.Parameters) {
							if ((p.GetSymbol() as ILVariable) == v)
								return new IdentifierExpression(p.Name);
						}
					}
				}
			}
			if (expr is CastExpression cast) {
				return Convert(cast.Expression);
			}
			return NotSupported(expr);
		}

		Expression NotSupported(Expression expr)
		{
			Debug.WriteLine("Expression Tree Conversion Failed: '" + expr + "' is not supported");
			return null;
		}
		#endregion



		#region Convert Lambda
		// Array.Empty<ParameterExpression> ()
		static readonly Expression emptyArrayPattern = new InvocationExpression(
			new MemberReferenceExpression(
				new TypeReferenceExpression(new SimpleType("Array")),
				"Empty",
				new SimpleType("ParameterExpression")
			)
		);

		private ParameterDeclaration ConvertParameter(InvocationExpression invocation)
		{
			if (invocation == null)
				return null;
			if (invocation.Arguments.Count != 2)
				return null;
			if (!(invocation.Arguments.ElementAt(0) is TypeOfExpression typeOfExpression))
				return null;
			if (!(invocation.Arguments.ElementAt(1) is PrimitiveExpression primitiveExpression))
				return null;
			return new ParameterDeclaration(typeOfExpression.Type.Clone(),
						primitiveExpression.Value.ToString());
		}

		Expression ConvertLambda(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
				return NotSupported(invocation);
			LambdaExpression lambda = new LambdaExpression();
			Expression body = invocation.Arguments.First();
			Expression parameterArray = invocation.Arguments.Last();

			// add missing type arguments - kind of a hack as they are missing sometimes (CurriedLambda)
			// this should be fixed somewhere else!!!
			if (invocation.Parent.GetSymbol() is IMethod parentMethod) {
				if (parentMethod.TypeArguments.Count > 0) {
					bool isAnonymous = false;
					foreach (var typearg in parentMethod.TypeArguments) {
						if (typearg.IsAnonymousType()) { 
							isAnonymous = true;
							break;
						}
					}
					if (!isAnonymous && invocation.Parent is InvocationExpression invocationExpression &&
						invocationExpression.Target is MemberReferenceExpression memberReferenceExpression) {
						invocationExpression.Target = new MemberReferenceExpression(memberReferenceExpression.Target.Clone(), memberReferenceExpression.MemberName, parentMethod.TypeArguments.Select(astBuilder.ConvertType));
					}
				}
			}

			if (parameterArray is ArrayCreateExpression arrayCreate) {
				foreach (var arg in arrayCreate.Initializer.Elements) {
					ParameterDeclaration parameter = ConvertParameter(arg as InvocationExpression);
					if (parameter == null)
						return NotSupported(invocation);
					IType type = parameter.Type.ToTypeReference().Resolve(context.TypeSystem.Compilation);
					if (type.IsAnonymousType()) {
						parameter.Type = null;
					}
					lambda.Parameters.Add(parameter);
				}
			} else if (parameterArray is InvocationExpression) {
				if (!emptyArrayPattern.IsMatch(parameterArray))
					return null;
			} else {
				return NotSupported(invocation);
			}

			activeLambdas.Push(lambda);
			Expression convertedBody = Convert(body);
			activeLambdas.Pop();
			if (convertedBody == null)
				return null;
			lambda.Body = convertedBody;
			return lambda;
		}
		#endregion

		#region Convert Field
		static readonly Expression getFieldFromHandlePattern =
			new InvocationExpression(
				new MemberReferenceExpression(new TypeReferenceExpression(new SimpleType("FieldInfo")), "GetFieldFromHandle"), new CastExpression(new SimpleType("RuntimeFieldHandle"), new LdTokenPattern("field")),
				new OptionalNode(new MemberReferenceExpression(new TypeOfExpression(new AnyNode("declaringType")), "TypeHandle")));

		Expression ConvertField(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
				return NotSupported(invocation);

			Expression fieldInfoExpr = invocation.Arguments.ElementAt(1);
			Match m = getFieldFromHandlePattern.Match(fieldInfoExpr);
			if (!m.Success)
				return NotSupported(invocation);

			AstNode field = m.Get<AstNode>("field").Single();
			IField fr = field.Annotation<LdMemberToken>().Member as IField;
			if (fr == null)
				return null;

			Expression target = invocation.Arguments.ElementAt(0);
			Expression convertedTarget;
			if (target is NullReferenceExpression) {
				if (m.Has("declaringType"))
					convertedTarget = new TypeReferenceExpression(m.Get<AstType>("declaringType").Single().Clone());
				else
					convertedTarget = new TypeReferenceExpression(astBuilder.ConvertType(fr.DeclaringType));
			} else {
				convertedTarget = Convert(target);
				if (convertedTarget == null)
					return null;
			}

			return convertedTarget.Member(fr.Name);
		}
		#endregion

		#region Convert Property
		static readonly Expression getMethodFromHandlePattern = 
			new Choice() {
				new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new SimpleType("Expression")),"Constant"),new CastExpression(new SimpleType("MethodInfo"),new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new SimpleType("MethodBase")),"GetMethodFromHandle"),new CastExpression(new SimpleType("RuntimeMethodHandle"),new LdTokenPattern("method")))),new TypeOfExpression(new SimpleType("MethodInfo"))),
				new CastExpression(new SimpleType("MethodInfo"), new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new SimpleType("MethodBase")), "GetMethodFromHandle"), new CastExpression(new SimpleType("RuntimeMethodHandle"), new LdTokenPattern("method")), new OptionalNode(new MemberReferenceExpression(new TypeOfExpression(new AnyNode("declaringType")), "TypeHandle")))),
				new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new SimpleType("Expression")),"Constant"),new CastExpression(new SimpleType("MethodInfo"),new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new SimpleType("MethodBase")),"GetMethodFromHandle"),new CastExpression(new SimpleType("RuntimeMethodHandle"),new LdTokenPattern("method")),new MemberReferenceExpression(new TypeOfExpression(new AnyNode("declaringType")),"TypeHandle"))),new TypeOfExpression(new SimpleType("MethodInfo")))
	};

	Expression ConvertProperty(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
				return NotSupported(invocation);

			Match m = getMethodFromHandlePattern.Match(invocation.Arguments.ElementAt(1));
			if (!m.Success)
				return NotSupported(invocation);

			IMethod mr = m.Get<AstNode>("method").Single().Annotation<LdMemberToken>().Member as IMethod;
			if (mr == null)
				return null;

			Expression target = invocation.Arguments.ElementAt(0);
			Expression convertedTarget;
			if (target is NullReferenceExpression) {
				if (m.Has("declaringType"))
					convertedTarget = new TypeReferenceExpression(m.Get<AstType>("declaringType").Single().Clone());
				else
					convertedTarget = new TypeReferenceExpression(astBuilder.ConvertType(mr.DeclaringType));
			} else {
				convertedTarget = Convert(target);
				if (convertedTarget == null)
					return null;
			}

			return convertedTarget.Member(GetPropertyName(mr));
		}

		string GetPropertyName(IMethod accessor)
		{
			string name = accessor.Name;
			if (name.StartsWith("get_", StringComparison.Ordinal) || name.StartsWith("set_", StringComparison.Ordinal))
				name = name.Substring(4);
			return name;
		}
		#endregion

		#region Convert Call
		Expression ConvertCall(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count < 2)
				return NotSupported(invocation);

			// System.Reflection.MethodInfo.CreateDelegate
			Match m1 = getMethodFromHandlePattern.Match(invocation.Arguments.ElementAt(1));
			bool isCreateDelegateCall = 
				m1.Success &&
				m1.Get<AstNode>("method").Single().Annotation<LdMemberToken>().Member is IMethod mr1 &&
				mr1.FullName.Equals("System.Reflection.MethodInfo.CreateDelegate");

			Expression target;
			int firstArgumentPosition;

			Match m = getMethodFromHandlePattern.Match(invocation.Arguments.ElementAt(0));
			if (m.Success) {
				target = null;
				firstArgumentPosition = isCreateDelegateCall ? 2 : 1;
			} else {
				m = getMethodFromHandlePattern.Match(invocation.Arguments.ElementAt(1));
				if (!m.Success)
					return NotSupported(invocation);
				target = invocation.Arguments.ElementAt(0);
				firstArgumentPosition = 2;
			}

			IMethod mr = m.Get<AstNode>("method").Single().Annotation<LdMemberToken>().Member as IMethod;
			if (mr == null)
				return null;

			Expression convertedTarget;
			if (target == null || target is NullReferenceExpression) {
				// static method
				if (m.Has("declaringType"))
					convertedTarget = new TypeReferenceExpression(m.Get<AstType>("declaringType").Single().Clone());
				else
					convertedTarget = new TypeReferenceExpression(astBuilder.ConvertType(mr.DeclaringType));
			} else {
				convertedTarget = Convert(target);
				if (convertedTarget == null)
					return null;
			}

			MemberReferenceExpression mre = convertedTarget.Member(mr.Name);
			foreach (IType tr in mr.TypeArguments) {
				if (tr.IsAnonymousType()) {
					mre.TypeArguments.Clear();
					break;
				}
				mre.TypeArguments.Add(astBuilder.ConvertType(tr));
			}
			IList<Expression> arguments = null;
			if (invocation.Arguments.Count == firstArgumentPosition + 1) {
				Expression argumentArray = invocation.Arguments.ElementAt(firstArgumentPosition);
				arguments = ConvertExpressionsArray(argumentArray);
			}
			if (arguments == null) {
				arguments = new List<Expression>();
				foreach (Expression argument in invocation.Arguments.Skip(firstArgumentPosition)) {
					Expression convertedArgument = Convert(argument);
					if (convertedArgument == null)
						return null;
					arguments.Add(convertedArgument);
				}
			}
			if (context.TypeSystem.GetCecil(mr) is MethodDefinition methodDef && methodDef.IsGetter) {
				PropertyDefinition indexer = GetIndexer(methodDef);
				if (indexer != null)
					return new IndexerExpression(mre.Target.Detach(), arguments);
			}

			if (isCreateDelegateCall) {
				if (arguments.Last().GetType() == typeof(NullReferenceExpression)) {
					// arguments[0] = typeof
					// arguments[1] = object is null -> no object on which the method is called
					return mre;
				} else {
					// arguments[0] = typeof
					// arguments[1] = object on which the delegate call is executed on
					return new MemberReferenceExpression(arguments.Last(), mre.MemberName);
				}
			}

			return new InvocationExpression(mre, arguments);
		}

		Expression ConvertInvoke(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
				return NotSupported(invocation);

			Expression convertedTarget = Convert(invocation.Arguments.ElementAt(0));
			IList<Expression> convertedArguments;
			Expression convertedArgument = Convert(invocation.Arguments.ElementAt(1));
			if (convertedArgument == null) {
				convertedArguments = ConvertExpressionsArray(invocation.Arguments.ElementAt(1));
			} else {
				convertedArguments = new Expression[] { convertedArgument };
			}
			if (convertedTarget != null && convertedArguments != null)
				return new InvocationExpression(convertedTarget, convertedArguments);
			else
				return null;
		}
		#endregion

		#region Convert Binary Operator
		static readonly Pattern trueOrFalse = new Choice {
			new PrimitiveExpression(true),
			new PrimitiveExpression(false)
		};

		Expression ConvertBinaryOperator(InvocationExpression invocation, BinaryOperatorType op, bool? isChecked = null)
		{
			if (invocation.Arguments.Count < 2)
				return NotSupported(invocation);

			Expression left = Convert(invocation.Arguments.ElementAt(0));
			if (left == null)
				return null;
			Expression right = Convert(invocation.Arguments.ElementAt(1));
			if (right == null)
				return null;

			BinaryOperatorExpression boe = new BinaryOperatorExpression(left, op, right);
			if (isChecked != null)
				boe.AddAnnotation(isChecked.Value ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);

			switch (invocation.Arguments.Count) {
				case 2:
					return boe;
				case 3:
					Match m = getMethodFromHandlePattern.Match(invocation.Arguments.ElementAt(2));
					if (m.Success)
						return boe;
					else
						return null;
				case 4:
					if (!trueOrFalse.IsMatch(invocation.Arguments.ElementAt(2)))
						return null;
					m = getMethodFromHandlePattern.Match(invocation.Arguments.ElementAt(3));
					if (m.Success)
						return boe;
					else
						return null;
				default:
					return NotSupported(invocation);
			}
		}
		#endregion

		#region Convert Assignment Operator
		Expression ConvertAssignmentOperator(InvocationExpression invocation, AssignmentOperatorType op, bool? isChecked = null)
		{
			return NotSupported(invocation);
		}
		#endregion

		#region Convert Unary Operator
		Expression ConvertUnaryOperator(InvocationExpression invocation, UnaryOperatorType op, bool? isChecked = null)
		{
			if (invocation.Arguments.Count < 1)
				return NotSupported(invocation);

			Expression expr = Convert(invocation.Arguments.ElementAt(0));
			if (expr == null)
				return null;

			IType type = null;
			if (expr is CastExpression cast) {
				type = cast.Type.ToTypeReference().Resolve(context.TypeSystem.Compilation);
			}
			if (expr is IdentifierExpression ident) {
				type = expr.GetResolveResult().Type;
			}
			ITypeDefinition typeDef = type?.GetEnumUnderlyingType()?.GetDefinition();
			if (typeDef != null) {
				switch (typeDef.KnownTypeCode) {
					case KnownTypeCode.Char:
					case KnownTypeCode.SByte:
					case KnownTypeCode.Byte:
					case KnownTypeCode.Int16:
					case KnownTypeCode.UInt16:
					case KnownTypeCode.Int32:
					case KnownTypeCode.UInt32:
					case KnownTypeCode.Int64:
					case KnownTypeCode.UInt64:
					case KnownTypeCode.IntPtr:
					case KnownTypeCode.UIntPtr:
						if (op == UnaryOperatorType.Not)
							op = UnaryOperatorType.BitNot;
						break;
				}
			}
			
			UnaryOperatorExpression uoe = new UnaryOperatorExpression(op, expr);
			if (isChecked != null)
				uoe.AddAnnotation(isChecked.Value ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);

			switch (invocation.Arguments.Count) {
				case 1:
					return uoe;
				case 2:
					Match m = getMethodFromHandlePattern.Match(invocation.Arguments.ElementAt(1));
					if (m.Success)
						return uoe;
					else
						return null;
				default:
					return NotSupported(invocation);
			}
		}
		#endregion

		#region Convert Condition Operator
		Expression ConvertCondition(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 3)
				return NotSupported(invocation);

			Expression condition = Convert(invocation.Arguments.ElementAt(0));
			Expression trueExpr = Convert(invocation.Arguments.ElementAt(1));
			Expression falseExpr = Convert(invocation.Arguments.ElementAt(2));
			if (condition != null && trueExpr != null && falseExpr != null)
				return new ConditionalExpression(condition, trueExpr, falseExpr);
			else
				return null;
		}
		#endregion

		#region Convert New Object
		static readonly Expression newObjectCtorPattern = new Choice() {
			new CastExpression(
				new SimpleType("ConstructorInfo"),
				new InvocationExpression(
					new MemberReferenceExpression(new TypeReferenceExpression(new SimpleType("MethodBase")), "GetMethodFromHandle"),
					new CastExpression(
						new SimpleType("RuntimeMethodHandle"),
						new LdTokenPattern("ctor")),
					new OptionalNode(new MemberReferenceExpression(new TypeOfExpression(new AnyNode("declaringType")), "TypeHandle")))),
			new TypeOfExpression(new AnyNode("declaringType"))
	};

		Expression ConvertNewObject(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count < 1)
				return NotSupported(invocation);

			Match m = newObjectCtorPattern.Match(invocation.Arguments.First());
			if (!m.Success)
				return NotSupported(invocation);

			IMethod ctor = m.Get<AstNode>("ctor").SingleOrDefault()?.Annotation<LdMemberToken>()?.Member as IMethod;
			if (ctor == null) {
				if (m.Has("declaringType")) {
					return new ObjectCreateExpression(m.Get<AstType>("declaringType").Single().Clone());
				}
				return null;
			}

			AstType declaringTypeNode;
			IType declaringType;
			if (m.Has("declaringType")) {
				declaringTypeNode = m.Get<AstType>("declaringType").Single().Clone();
				declaringType = declaringTypeNode.GetSymbol() as IType;
			} else {
				declaringTypeNode = astBuilder.ConvertType(ctor.DeclaringType);
				declaringType = ctor.DeclaringType;
			}
			if (declaringTypeNode == null)
				return null;

			ObjectCreateExpression oce = new ObjectCreateExpression(declaringTypeNode);
			if (invocation.Arguments.Count >= 2) {
				if (invocation.Arguments.Count == 2) { 
					IList<Expression> arguments = ConvertExpressionsArray(invocation.Arguments.ElementAtOrDefault(1));
					if (arguments == null)
						return null;
					oce.Arguments.AddRange(arguments);
				} else {
					List<Expression> arguments = new List<Expression>();
					foreach (Expression argument in invocation.Arguments.Skip(1)) {
						arguments.Add(Convert(argument));
					}
					oce.Arguments.AddRange(arguments);
				}
			}
			if (invocation.Arguments.Count >= 3 && declaringType.IsAnonymousType()) {
				IMethod resolvedCtor = ctor;
				if (resolvedCtor == null || resolvedCtor.Parameters.Count != oce.Arguments.Count)
					return null;
				AnonymousTypeCreateExpression atce = new AnonymousTypeCreateExpression();
				var arguments = oce.Arguments.ToArray();
				if (CallBuilder.CanInferAnonymousTypePropertyNamesFromArguments(arguments, resolvedCtor.Parameters)) {
					oce.Arguments.MoveTo(atce.Initializers);
				} else {
					for (int i = 0; i < resolvedCtor.Parameters.Count; i++) {
						atce.Initializers.Add(
							new NamedExpression {
								Name = resolvedCtor.Parameters[i].Name,
								Expression = arguments[i].Detach()
							});
					}
				}
				return atce;
			}

			return oce;
		}
		#endregion

		#region Convert ListInit
		static readonly Pattern elementInitArrayPattern = ArrayInitializationPattern(new SimpleType("ElementInit"),
			new InvocationExpression(new MemberReferenceExpression(new TypeReferenceExpression(new TypePattern(typeof(System.Linq.Expressions.Expression))), "ElementInit"), new AnyNode("methodInfos"), new AnyNode("addArgumentsArrays")).Clone()
		);

		Expression ConvertListInit(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count < 1)
				return NotSupported(invocation);
			ObjectCreateExpression oce = Convert(invocation.Arguments.ElementAt(0)) as ObjectCreateExpression;
			if (oce == null)
				return null;
			ArrayInitializerExpression initializer;
			if (invocation.Arguments.Count == 2) {
				initializer = ConvertElementInit(invocation.Arguments.ElementAt(1) as InvocationExpression);
				if (initializer != null) {
					oce.Initializer = initializer;
					return oce;
				} else {
					return null;
				}
			} else {
				initializer = new ArrayInitializerExpression();
				for (int i = 1; i < invocation.Arguments.Count; i++) {
					initializer.Elements.Add(ConvertElementInit(invocation.Arguments.ElementAt(i) as InvocationExpression));
				}
				oce.Initializer = initializer;
				return oce;
			}
		}

		ArrayInitializerExpression ConvertElementInit(InvocationExpression elementsArray)
		{
			IList<Expression> elements;
			if (elementsArray.Arguments.Count > 1) {
				elements = new List<Expression>();
				foreach (var element in elementsArray.Arguments.Skip(1)) {
					elements.Add(Convert(element));
				}
			} else {
				elements = ConvertExpressionsArray(elementsArray);
			}
			if (elements != null) {
				return new ArrayInitializerExpression(elements);
			}
			Match m = elementInitArrayPattern.Match(elementsArray);
			if (!m.Success)
				return null;
			ArrayInitializerExpression result = new ArrayInitializerExpression();
			foreach (var elementInit in m.Get<Expression>("addArgumentsArrays")) {
				IList<Expression> arguments = ConvertExpressionsArray(elementInit);
				if (arguments == null)
					return null;
				result.Elements.Add(new ArrayInitializerExpression(arguments));
			}
			return result;
		}
		#endregion

		#region Convert MemberInit
		Expression ConvertMemberInit(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count < 1)
				return NotSupported(invocation);
			ObjectCreateExpression oce = Convert(invocation.Arguments.ElementAt(0)) as ObjectCreateExpression;
			if (oce == null)
				return null;
			List<Expression> memberBindings = new List<Expression>();
			foreach (var memberBinding in invocation.Arguments.Skip(1)) {
				Expression binding = ConvertMemberBinding(memberBinding as InvocationExpression);
				if (binding == null)
					return null;
				memberBindings.Add(binding);
			}
			oce.Initializer = new ArrayInitializerExpression(memberBindings);
			return oce;
		}

		static readonly Pattern memberBindingArrayPattern = ArrayInitializationPattern(new SimpleType("MemberBinding"), new AnyNode("binding"));
		static readonly INode expressionTypeReference = new TypeReferenceExpression(new TypePattern(typeof(System.Linq.Expressions.Expression)));

		List<Expression> ConvertMemberBindings(Expression elementsArray)
		{
			List<Expression> memberBindings = new List<Expression>();
			Match m = memberBindingArrayPattern.Match(elementsArray);
			if (!m.Success)
				return null;
			foreach (var binding in m.Get<Expression>("binding")) {
				memberBindings.Add(ConvertMemberBinding(binding as InvocationExpression));
			}
			return memberBindings;
		}
		#endregion

		Expression ConvertMemberBinding(InvocationExpression bindingInvocation)
		{
			if (bindingInvocation == null || bindingInvocation.Arguments.Count != 2)
				return null;
			MemberReferenceExpression bindingMRE = bindingInvocation.Target as MemberReferenceExpression;
			if (bindingMRE == null || !expressionTypeReference.IsMatch(bindingMRE.Target))
				return null;

			Expression bindingTarget = bindingInvocation.Arguments.ElementAt(0);
			Expression bindingValue = bindingInvocation.Arguments.ElementAt(1);

			string memberName;
			Match m2 = getFieldFromHandlePattern.Match(bindingTarget); // getFieldFromHandle
			if (m2.Success) {
				IField setter = m2.Get<AstNode>("field").Single().Annotation<LdMemberToken>().Member as IField;
				if (setter == null)
					return null;
				memberName = setter.Name;
			} else {
				return null;
			}

			Expression convertedValue;
			switch (bindingMRE.MemberName) {
				case "Bind":
					convertedValue = Convert(bindingValue);
					break;
				case "MemberBind":
					convertedValue = new ArrayInitializerExpression(ConvertMemberBindings(bindingValue));
					break;
				case "ListBind":
					convertedValue = ConvertElementInit(bindingValue as InvocationExpression);
					break;
				default:
					return null;
			}
			if (convertedValue == null)
				return null;
			return new NamedExpression(memberName, convertedValue);
		}

		#region Convert Cast
		Expression ConvertCast(InvocationExpression invocation, bool isChecked)
		{
			if (invocation.Arguments.Count < 2)
				return null;
			Expression converted = Convert(invocation.Arguments.ElementAt(0));
			AstType type = ConvertTypeReference(invocation.Arguments.ElementAt(1));
			if (converted != null && type != null) {
				CastExpression cast = converted.CastTo(type);
				cast.AddAnnotation(isChecked ? AddCheckedBlocks.CheckedAnnotation : AddCheckedBlocks.UncheckedAnnotation);
				switch (invocation.Arguments.Count) {
					case 2:
						return cast;
					case 3:
						Match m = getMethodFromHandlePattern.Match(invocation.Arguments.ElementAt(2));
						if (m.Success)
							return cast;
						else
							return null;
				}
			}
			return null;
		}
		#endregion
		
		static T Use<T>(T node)
		{
			if (node is ICloneable cloneable) {
				return (T)cloneable.Clone();
			}
			return node;
		}

		#region ConvertExpressionsArray
		static Pattern ArrayInitializationPattern(SimpleType arrayElementType, INode elementPattern)
		{
			return new Choice {
				new ArrayCreateExpression {
					Type = Use(arrayElementType),
					Initializer = new ArrayInitializerExpression {
						Elements = { new Repeat(Use(elementPattern)) }
					},
					Arguments = { new PrimitiveExpression(PrimitiveExpression.AnyValue) }
				},
				new CastExpression(new SimpleType("IEnumerable",new SimpleType("Expression")),
					new ArrayCreateExpression {
						Type = Use(arrayElementType),
						Initializer = new ArrayInitializerExpression {
							Elements = { new Repeat(Use(elementPattern)) }
						},
						Arguments = { new PrimitiveExpression(PrimitiveExpression.AnyValue) }
					}
				)
			};
		}

		static readonly Pattern expressionArrayPattern = ArrayInitializationPattern(new SimpleType("Expression"), new AnyNode("elements"));

		IList<Expression> ConvertExpressionsArray(Expression arrayExpression)
		{
			Match m = expressionArrayPattern.Match(arrayExpression);
			if (m.Success) {
				List<Expression> result = new List<Expression>();
				foreach (Expression expr in m.Get<Expression>("elements")) {
					Expression converted = Convert(expr);
					if (converted == null)
						return null;
					result.Add(converted);
				}
				return result;
			}
			return null;
		}
		#endregion

		#region Convert TypeAs/TypeIs
		static readonly TypeOfExpression typeOfPattern = new TypeOfExpression(new AnyNode("type"));

		AstType ConvertTypeReference(Expression typeOfExpression)
		{
			Match m = typeOfPattern.Match(typeOfExpression);
			if (m.Success)
				return m.Get<AstType>("type").Single().Clone();
			else
				return null;
		}

		Expression ConvertTypeAs(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			Expression converted = Convert(invocation.Arguments.ElementAt(0));
			AstType type = ConvertTypeReference(invocation.Arguments.ElementAt(1));
			if (converted != null && type != null)
				return new AsExpression(converted, type);
			return null;
		}

		Expression ConvertTypeIs(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
				return null;
			Expression converted = Convert(invocation.Arguments.ElementAt(0));
			AstType type = ConvertTypeReference(invocation.Arguments.ElementAt(1));
			if (converted != null && type != null)
				return new IsExpression { Expression = converted, Type = type };
			return null;
		}
		#endregion

		#region Convert Array
		Expression ConvertArrayIndex(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 2)
				return NotSupported(invocation);

			Expression targetConverted = Convert(invocation.Arguments.First());
			if (targetConverted == null)
				return null;

			Expression index = invocation.Arguments.ElementAt(1);
			Expression indexConverted = Convert(index);
			if (indexConverted != null) {
				return new IndexerExpression(targetConverted, indexConverted);
			}
			IList<Expression> indexesConverted = ConvertExpressionsArray(index);
			if (indexesConverted != null) {
				return new IndexerExpression(targetConverted, indexesConverted);
			}
			return null;
		}

		Expression ConvertArrayLength(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count != 1)
				return NotSupported(invocation);

			Expression targetConverted = Convert(invocation.Arguments.Single());
			if (targetConverted != null)
				return targetConverted.Member("Length");
			else
				return null;
		}

		Expression ConvertNewArrayInit(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count < 1)
				return NotSupported(invocation);

			AstType elementType = ConvertTypeReference(invocation.Arguments.ElementAt(0));
			IList<Expression> elements = new List<Expression>();
			foreach (var element in invocation.Arguments.Skip(1)) {
				var convertedElement = Convert(element);
				if (convertedElement == null) {
					elements = null;
					break;
				}
				elements.Add(convertedElement);
			}
			if (elements == null && invocation.Arguments.Count == 2) {
				elements = ConvertExpressionsArray(invocation.Arguments.ElementAt(1));
			}
			
			if (elementType != null && elements != null) {
				if (ContainsAnonymousType(elementType)) {
					elementType = null;
				}
				return new ArrayCreateExpression {
					Type = elementType,
					AdditionalArraySpecifiers = { new ArraySpecifier() },
					Initializer = new ArrayInitializerExpression(elements)
				};
			}
			return null;
		}

		Expression ConvertNewArrayBounds(InvocationExpression invocation)
		{
			if (invocation.Arguments.Count < 2)
				return NotSupported(invocation);

			AstType elementType = ConvertTypeReference(invocation.Arguments.ElementAt(0));
			List<Expression> arguments = new List<Expression>();
			for (int i = 1; i < invocation.Arguments.Count; i++) {
				Expression convertedExpression = Convert(invocation.Arguments.ElementAt(i));
				if (convertedExpression != null) {
					arguments.Add(convertedExpression);
				} else { 
					arguments.AddRange(ConvertExpressionsArray(invocation.Arguments.ElementAt(i)));
				}
			}
			if (elementType != null && arguments != null) {
				if (ContainsAnonymousType(elementType)) {
					elementType = null;
				}
				ArrayCreateExpression ace = new ArrayCreateExpression();
				ace.Type = elementType;
				ace.Arguments.AddRange(arguments);
				return ace;
			}
			return null;
		}

		bool ContainsAnonymousType(AstType type)
		{
			foreach (AstType t in type.DescendantsAndSelf.OfType<AstType>()) {
				if (t.GetSymbol() is IType tr && tr.IsAnonymousType())
					return true;
			}
			return false;
		}

		static PropertyDefinition GetIndexer(MethodDefinition cecilMethodDef)
		{
			TypeDefinition typeDef = cecilMethodDef.DeclaringType;
			string indexerName = null;

			foreach (CustomAttribute ca in typeDef.CustomAttributes) {
				if (ca.Constructor.FullName == "System.Void System.Reflection.DefaultMemberAttribute::.ctor(System.String)") {
					indexerName = ca.ConstructorArguments.Single().Value as string;
					break;
				}
			}

			if (indexerName == null)
				return null;

			foreach (PropertyDefinition prop in typeDef.Properties) {
				if (prop.Name == indexerName) {
					if (prop.GetMethod == cecilMethodDef || prop.SetMethod == cecilMethodDef)
						return prop;
				}
			}
			return null;
		}
		#endregion

		private void PrintMatchPattern(string prefix, AstNode astNode)
		{
			if (astNode.GetType().Name.Equals("Identifier") || astNode.GetType().Name.Equals("Comment")) {
				Debug.Write("\"" + astNode.ToString() + "\"");
				return;
			}
			Debug.Write("new " + (astNode.GetType().Name.Equals("PrimitiveType") ? "Syntax.PrimitiveType" : astNode.GetType().Name) + "(");
			foreach (var child in astNode.Children) {
				PrintMatchPattern(prefix + " ", child);
				if (astNode.Children.Last() != child)
					Debug.Write(",");
			}
			if (!astNode.HasChildren) {
				Debug.Write("\"" + astNode.ToString() + "\"");
			}
			Debug.Write(")");
			if (prefix.Length == 0)
				Debug.WriteLine(";");
		}
	}
}
