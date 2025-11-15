// Copyright (c) 2025 Siegfried Pammer
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.CSharp.Transforms
{
	/// <summary>
	/// This transform moves field initializers at the start of constructors to their respective field declarations
	/// and transforms this-/base-ctor calls in constructors to constructor initializers.
	/// </summary>
	public class TransformFieldAndConstructorInitializers : IAstTransform
	{
		/// <summary>
		/// Pattern for reference types:
		/// this..ctor(...);
		/// </summary>
		internal static readonly AstNode ThisCallClassPattern = new ExpressionStatement(
			new NamedNode("invocation", new InvocationExpression(
				new MemberReferenceExpression(
					new Choice {
						new NamedNode("target", new ThisReferenceExpression()),
						new NamedNode("target", new BaseReferenceExpression()),
						new CastExpression {
							Type = new AnyNode(),
							Expression = new Choice {
							new NamedNode("target", new ThisReferenceExpression()),
								new NamedNode("target", new BaseReferenceExpression()),
							}
						}
					}, ".ctor"),
				new Repeat(new AnyNode()))
			)
		);

		/// <summary>
		/// Pattern for value types:
		/// this = new TSelf(...);
		/// </summary>
		internal static readonly AstNode ThisCallStructPattern = new ExpressionStatement(
				new AssignmentExpression(
					new NamedNode("target", new ThisReferenceExpression()),
					new NamedNode("invocation", new ObjectCreateExpression(new AnyNode(), new Repeat(new AnyNode())))
				)
		);

		internal static bool IsGeneratedPrimaryConstructorBackingField(IField field)
		{
			var name = field.Name;
			return name.StartsWith("<", StringComparison.Ordinal)
				&& name.EndsWith(">P", StringComparison.Ordinal)
				&& field.IsCompilerGenerated();
		}

		enum InitializerKind
		{
			Static,
			Instance,
			Primary
		}

		class InitializerSequence
		{
			static readonly ExpressionStatement memberInitializerPattern = new() {
				Expression = new AssignmentExpression(
					new Choice {
							new NamedNode("fieldAccess", new MemberReferenceExpression {
								Target = new ThisReferenceExpression(),
								MemberName = Pattern.AnyString
							}),
							new NamedNode("fieldAccess", new IdentifierExpression(Pattern.AnyString))
					},
					new AnyNode("initializer")
				)
			};

			[AllowNull]
			public List<(Statement Statement, IMember Member, Expression Initializer, bool DependsOnConstructorBody)> Statements;

			public Dictionary<Statement, List<Statement>>? StatementToOtherCtorsMap;

			public bool HasDuplicateAssignments { get; private set; }
			public bool IsUnsafe { get; private set; }
			public bool CoversFullBody { get; private set; }

			public static InitializerSequence? Analyze(ConstructorInitializerAnalyzer context, ConstructorDeclaration ctor, IMethod ctorMethod)
			{
				var sequence = new InitializerSequence() {
					Statements = [],
					IsUnsafe = ctor.HasModifier(Modifiers.Unsafe)
				};
				var initializedMembers = new HashSet<IMember>();
				var function = ctor.Annotation<ILFunction>();
				var isStruct = ctorMethod.DeclaringType.Kind == TypeKind.Struct;

				bool onlyMoveConstants = !context.context.Settings.AlwaysMoveInitializer
					&& !context.IsBeforeFieldInit && ctorMethod.IsStatic;
				bool skippedStmts = false;

				Statement? stmt;
				for (stmt = ctor.Body.Statements.FirstOrDefault(); stmt != null; stmt = stmt.GetNextStatement())
				{
					var m = memberInitializerPattern.Match(stmt);
					if (!m.Success)
						break;
					var fieldAccess = m.Get<AstNode>("fieldAccess").Single();
					var initializer = m.Get<Expression>("initializer").Single();
					var member = (fieldAccess.GetSymbol() as IMember)?.MemberDefinition;
					if (member == null || !CanHaveInitializer(member, context))
						break;
					if (onlyMoveConstants && member is not IField { IsConst: true })
					{
						skippedStmts = true;
						continue;
					}
					if (!initializedMembers.Add(member))
						sequence.HasDuplicateAssignments = true;

					bool dependsOnBody = false;

					foreach (var instruction in initializer.Annotations.OfType<ILInstruction>())
					{
						foreach (var inst in instruction.Descendants)
						{
							if (inst is IInstructionWithVariableOperand { Variable: var v }
								&& v.Function == function && v.Kind == VariableKind.Parameter)
							{
								dependsOnBody = true;
							}
						}
					}

					sequence.Statements.Add((stmt, member, initializer, dependsOnBody));
				}

				if (!skippedStmts)
				{
					if (stmt == null)
					{
						sequence.CoversFullBody = true;
					}
					else
					{
						var m = isStruct
							? ThisCallStructPattern.Match(stmt)
							: ThisCallClassPattern.Match(stmt);
						if (m.Success)
						{
							sequence.CoversFullBody = stmt.GetNextStatement() == null;
						}
					}
				}

				return sequence;
			}

			private static bool CanHaveInitializer(IMember member, ConstructorInitializerAnalyzer context)
			{
				if (context.MemberToDeclaringSyntaxNodeMap == null)
					return false;
				if (!context.MemberToDeclaringSyntaxNodeMap.TryGetValue(member, out var declaringSyntaxNode))
					return true;
				return declaringSyntaxNode is FieldDeclaration
					or PropertyDeclaration { IsAutomaticProperty: true }
					or EventDeclaration;
			}

			public bool IsMatch(ConstructorDeclaration ctor)
			{
				var stmts = ctor.Body.Statements;
				var otherStmt = stmts.FirstOrDefault();
				foreach (var (stmt, member, initializer, _) in Statements)
				{
					var m = memberInitializerPattern.Match(otherStmt);
					if (!m.Success)
						return false;
					Debug.Assert(otherStmt != null); // because m.Success
					var fieldAccess = m.Get<AstNode>("fieldAccess").Single();
					var otherMember = (fieldAccess.GetSymbol() as IMember)?.MemberDefinition;
					if (!member.Equals(otherMember))
						return false;
					var otherInitializer = m.Get<Expression>("initializer").Single();
					if (!initializer.IsMatch(otherInitializer))
						return false;
					StatementToOtherCtorsMap ??= [];
					if (!StatementToOtherCtorsMap.TryGetValue(stmt, out var list))
					{
						list = [];
						StatementToOtherCtorsMap[stmt] = list;
					}
					list.Add(otherStmt);
					otherStmt = otherStmt.GetNextStatement();
				}
				return true;
			}
		}

		class ConstructorInitializerAnalyzer
		{
			internal readonly TransformContext context;
			public readonly ITypeDefinition TypeDefinition;
			public readonly TypeDeclaration? TypeDeclaration;
			public readonly RecordDecompiler? RecordDecompiler;

			[AllowNull]
			public Dictionary<IMember, EntityDeclaration> MemberToDeclaringSyntaxNodeMap;

			public Dictionary<IParameter, (IProperty?, IField)>? PrimaryConstructorParameterToBackingStoreMap;
			public Dictionary<IField, ILVariable>? BackingFieldToPrimaryConstructorParameterVariableMap;

			public InitializerSequence? InstanceInitializers;
			public InitializerSequence? StaticInitializers;
			public InitializerSequence? PrimaryConstructorInitializers;

			public IMethod? StaticConstructor;
			public ConstructorDeclaration? StaticConstructorDecl;

			public IMethod? RecordCopyConstructor;
			public ConstructorDeclaration? RecordCopyConstructorDecl;

			public IMethod? PrimaryConstructor;
			public ConstructorDeclaration? PrimaryConstructorDecl;

			[AllowNull]
			public ConstructorDeclaration[] InstanceConstructors;

			public bool IsBeforeFieldInit {
				get {
					if (TypeDefinition?.MetadataToken.IsNil != false)
						return false;

					var metadata = context.TypeSystem.MainModule.MetadataFile.Metadata;
					var td = metadata.GetTypeDefinition((SRM.TypeDefinitionHandle)TypeDefinition.MetadataToken);

					return td.HasFlag(TypeAttributes.BeforeFieldInit);
				}
			}

			public ConstructorInitializerAnalyzer(TransformContext context, ITypeDefinition typeDefinition, TypeDeclaration? typeDeclaration)
			{
				this.context = context;
				TypeDefinition = typeDefinition;
				TypeDeclaration = typeDeclaration;
				RecordDecompiler = context.DecompileRun.RecordDecompilers.TryGetValue(typeDefinition, out var record) ? record : null;
			}

			public bool Analyze(IEnumerable<AstNode> members)
			{
				MemberToDeclaringSyntaxNodeMap = members
					.Select(m => (symbol: m.GetSymbol(), entity: (EntityDeclaration)m))
					.Where(_ => _.symbol is IMember)
					.ToDictionary(_ => (IMember)_.symbol, _ => _.entity);

				List<ConstructorDeclaration> constructorsNotChainedWithThis = [];
				List<ConstructorDeclaration> allCtors = [];

				// we use the row number of the first method in the metadata table as a heuristic
				// to determine the primary constructor in non-record structs.
				// This is not completely reliable, but works.
				var metadata = context.TypeSystem.MainModule.metadata;
				var typeDef = metadata.GetTypeDefinition((SRM.TypeDefinitionHandle)TypeDefinition.MetadataToken);
				var firstMethodRowNumber = MetadataTokens.GetRowNumber(typeDef.GetMethods().FirstOrDefault());

				foreach (var ctor in members.OfType<ConstructorDeclaration>())
				{
					var ctorMethod = (IMethod)ctor.GetSymbol();
					Debug.Assert(ctorMethod.IsConstructor);
					Debug.Assert(ctorMethod.MetadataToken.IsNil == false);

					int rowNumber = MetadataTokens.GetRowNumber(ctorMethod.MetadataToken);
					Debug.Assert(rowNumber > 0);

					if (ctorMethod.Equals(RecordDecompiler?.PrimaryConstructor))
					{
						Debug.Assert(PrimaryConstructorDecl == null);
						PrimaryConstructor = ctorMethod;
						PrimaryConstructorDecl = ctor;
						PrimaryConstructorParameterToBackingStoreMap = RecordDecompiler.GetParameterToBackingStoreMap();
						constructorsNotChainedWithThis.Add(ctor);
					}
					else if (ctorMethod.IsStatic)
					{
						Debug.Assert(StaticConstructorDecl == null);
						StaticConstructor = ctorMethod;
						StaticConstructorDecl = ctor;
					}
					else if (RecordDecompiler != null && RecordDecompiler.IsCopyConstructor(ctorMethod))
					{
						Debug.Assert(RecordCopyConstructorDecl == null);
						RecordCopyConstructor = ctorMethod;
						RecordCopyConstructorDecl = ctor;
					}
					else
					{
						// find this-ctor call
						var stmt = ctor.Body.Statements.FirstOrDefault();
						var m = ctorMethod.DeclaringType.Kind == TypeKind.Struct
							? ThisCallStructPattern.Match(stmt)
							: ThisCallClassPattern.Match(stmt);

						allCtors.Add(ctor);

						if (m.Success && m.Get<Expression>("target").Single() is ThisReferenceExpression)
							continue;

						constructorsNotChainedWithThis.Add(ctor);
					}
				}

				if (context.Settings.UsePrimaryConstructorSyntaxForNonRecordTypes
					&& RecordDecompiler == null && constructorsNotChainedWithThis.Count == 1)
				{
					Debug.Assert(PrimaryConstructor == null);

					// this constructor could be converted to a primary constructor
					var ctor = constructorsNotChainedWithThis[0];
					var ctorMethod = (IMethod)constructorsNotChainedWithThis[0].GetSymbol();

					var initializer = InitializerSequence.Analyze(this, ctor, ctorMethod);

					if (initializer is { CoversFullBody: true, HasDuplicateAssignments: false, Statements.Count: > 0 })
					{
						bool transformToPrimaryConstructor = MetadataTokens.GetRowNumber(ctorMethod.MetadataToken) == firstMethodRowNumber;

						if (ctorMethod.Parameters.Count == 0)
						{
							transformToPrimaryConstructor = false;
						}

						foreach (var (stmt, member, expr, dependsOnBody) in initializer.Statements)
						{
							if (member is IField f && IsGeneratedPrimaryConstructorBackingField(f))
							{
								var variable = expr.Annotation<ILVariableResolveResult>()?.Variable;
								if (variable is { Kind: VariableKind.Parameter, Index: >= 0 and int index })
								{
									var p = ctorMethod.Parameters[index];
									PrimaryConstructorParameterToBackingStoreMap ??= [];
									BackingFieldToPrimaryConstructorParameterVariableMap ??= [];
									PrimaryConstructorParameterToBackingStoreMap[p] = (null, f);
									BackingFieldToPrimaryConstructorParameterVariableMap[f] = variable;
									transformToPrimaryConstructor = true;
								}
							}
						}

						if (context.Settings.ShowXmlDocumentation && context.DecompileRun.DocumentationProvider is { } provider)
						{
							var classDoc = provider.GetDocumentation(ctorMethod.DeclaringTypeDefinition);
							var ctorDoc = provider.GetDocumentation(ctorMethod);

							if (ctorDoc != null && ctorDoc != classDoc)
							{
								transformToPrimaryConstructor = false;
							}
						}

						if (transformToPrimaryConstructor)
						{
							PrimaryConstructorParameterToBackingStoreMap ??= [];
							PrimaryConstructorDecl = ctor;
							PrimaryConstructor = ctorMethod;
							PrimaryConstructorInitializers = initializer;
						}
					}
				}

				if (StaticConstructor != null)
				{
					StaticInitializers = InitializerSequence.Analyze(this, StaticConstructorDecl!, StaticConstructor);
				}

				if (PrimaryConstructor != null)
				{
					Debug.Assert(PrimaryConstructorDecl != null);

					// if there exists a primary constructor, all other constructors must call it
					Debug.Assert(constructorsNotChainedWithThis.Count == 1);

					PrimaryConstructorInitializers ??= InitializerSequence.Analyze(this, PrimaryConstructorDecl, PrimaryConstructor);
				}

				if (constructorsNotChainedWithThis.Count > 0)
				{
					if (TypeDefinition?.Kind != TypeKind.Struct || (context.Settings.StructDefaultConstructorsAndFieldInitializers && !TypeDefinition.IsRecord))
					{
						bool isPrimaryCtor = constructorsNotChainedWithThis[0] == PrimaryConstructorDecl;
						var sequence = isPrimaryCtor
							? PrimaryConstructorInitializers
							: InitializerSequence.Analyze(this, constructorsNotChainedWithThis[0], (IMethod)constructorsNotChainedWithThis[0].GetSymbol());

						if (sequence == null)
							return false;

						for (int i = 1; i < constructorsNotChainedWithThis.Count; i++)
						{
							if (!sequence.IsMatch(constructorsNotChainedWithThis[i]))
								return false;
						}

						if (!isPrimaryCtor)
						{
							if (!sequence.Statements.Any(s => s.DependsOnConstructorBody))
								InstanceInitializers = sequence;
						}
					}
				}

				InstanceConstructors = allCtors.ToArray();

				return true;
			}

			public bool MoveConstructorInitializer(ConstructorDeclaration constructorDeclaration, IMethod ctorMethod)
			{
				Statement stmt = constructorDeclaration.Body.Statements.FirstOrDefault()!;
				var isValueType = ctorMethod.DeclaringType.Kind == TypeKind.Struct;

				// value types may omit the constructor initializer completely
				if (stmt == null && isValueType)
				{
					return true;
				}

				var m = isValueType
					? ThisCallStructPattern.Match(stmt)
					: ThisCallClassPattern.Match(stmt);

				if (!m.Success)
					return isValueType;

				Debug.Assert(stmt != null); // because m.Success

				AstNode invocation = m.Get<AstNode>("invocation").Single();
				if (invocation.GetSymbol() is not IMethod { IsConstructor: true } ctor)
					return false;

				ConstructorInitializerType type = ctor.DeclaringTypeDefinition == ctorMethod.DeclaringTypeDefinition
					? ConstructorInitializerType.This
					: ConstructorInitializerType.Base;

				var ci = new ConstructorInitializer { ConstructorInitializerType = type };

				// Move arguments from invocation to initializer:
				invocation.GetChildrenByRole(Roles.Argument).MoveTo(ci.Arguments);
				// Add the initializer: (unless it is the default 'base()')
				if (!(ci.ConstructorInitializerType == ConstructorInitializerType.Base && ci.Arguments.Count == 0))
					constructorDeclaration.Initializer = ci.CopyAnnotationsFrom(invocation);

				// Remove the statement
				stmt.Remove();

				return true;
			}

			public bool MoveFieldInitializersToDeclarations(InitializerSequence sequence, InitializerKind kind)
			{
				foreach (var (stmt, member, initializer, dependsOnBody) in sequence.Statements)
				{
					Debug.Assert(!dependsOnBody || kind is InitializerKind.Primary);

					if (!MemberToDeclaringSyntaxNodeMap.TryGetValue(member, out var declaringSyntaxNode))
					{
						Debug.Assert(kind is InitializerKind.Primary);
						stmt.Remove();
						continue;
					}

					VariableInitializer v;
					switch (declaringSyntaxNode)
					{
						case FieldDeclaration fd:
							v = fd.Variables.Single();
							if (v.Initializer.IsNull)
							{
								v.Initializer = initializer.Detach();
							}
							else if (kind == InitializerKind.Static)
							{
								// decimal constants already have an initializer in the AST at this point,
								// because it was added in CSharpDecompiler.DoDecompile(IField, ...)
								var constant = v.Initializer.GetResolveResult();
								var expression = initializer.GetResolveResult();
								if (constant.IsCompileTimeConstant && TryEvaluateDecimalConstant(expression, out decimal value))
								{
									// decimal values do not match, skip transformation?
									if (!value.Equals(constant.ConstantValue))
										continue;
								}
								else
								{
									// already has an initializer - do not modify
									Debug.Fail("Field already has an initializer");
								}
							}
							else
							{
								// already has an initializer - do not modify
								Debug.Fail("Field already has an initializer");
							}
							break;
						case PropertyDeclaration pd:
							Debug.Assert(pd.IsAutomaticProperty);
							if (pd.Initializer.IsNull)
							{
								pd.Initializer = initializer.Detach();
							}
							else
							{
								// already has an initializer - do not modify
								Debug.Fail("Property already has an initializer");
							}
							break;
						case EventDeclaration ev:
							v = ev.Variables.Single();
							if (v.Initializer.IsNull)
							{
								v.Initializer = initializer.Detach();
							}
							else
							{
								// already has an initializer - do not modify
								Debug.Fail("Event already has an initializer");
							}
							break;
						default:
							// cannot move initializer
							continue;
					}
					// Remove the statement from all constructors
					stmt.Remove();

					if (sequence.StatementToOtherCtorsMap != null &&
						sequence.StatementToOtherCtorsMap.TryGetValue(stmt, out var otherStmts))
					{
						foreach (var otherStmt in otherStmts)
						{
							otherStmt.Remove();
						}
					}

					if (sequence.IsUnsafe && IntroduceUnsafeModifier.IsUnsafe(initializer))
					{
						declaringSyntaxNode.Modifiers |= Modifiers.Unsafe;
					}
				}
				return true;
			}

			public void RemoveImplicitConstructor()
			{
				Debug.Assert(MemberToDeclaringSyntaxNodeMap != null);
				Debug.Assert(TypeDefinition != null);

				// We do not want to hide the constructor if the user explicitly selected it in the tree view.
				if (TypeDeclaration == null)
					return;

				// Remove primary constructor body
				if (PrimaryConstructor != null)
				{
					Debug.Assert(PrimaryConstructorDecl != null);

					this.TypeDeclaration.HasPrimaryConstructor = PrimaryConstructor.Parameters.Any()
						|| !PrimaryConstructorDecl.Initializer.IsNull
						|| TypeDefinition.Kind == TypeKind.Struct;

					PrimaryConstructorDecl.Parameters.MoveTo(this.TypeDeclaration.PrimaryConstructorParameters);

					Debug.Assert(PrimaryConstructorParameterToBackingStoreMap != null);

					foreach (var pd in this.TypeDeclaration.PrimaryConstructorParameters)
					{
						var v = pd.Annotation<ILVariableResolveResult>()?.Variable;
						Debug.Assert(v?.Index >= 0);
						var p = PrimaryConstructor.Parameters[v.Index.Value];

						if (!PrimaryConstructorParameterToBackingStoreMap.TryGetValue(p, out var backingStore))
						{
							// no backing store, constructor parameter was left unassigned or
							// assigned to a different member in a sub-expression:
							// private int someField = param + param2;
							continue;
						}

						var (prop, field) = backingStore;

						if (prop != null)
						{
							var attributes = prop?.GetAttributes().Select(attr => context.TypeSystemAstBuilder.ConvertAttribute(attr)).ToArray();
							if (attributes?.Length > 0)
							{
								var section = new AttributeSection {
									AttributeTarget = "property"
								};
								section.Attributes.AddRange(attributes);
								pd.Attributes.Add(section);
							}
						}
						if (field != null)
						{
							var attributes = field.GetAttributes()
								.Where(a => !PatternStatementTransform.attributeTypesToRemoveFromAutoProperties.Contains(a.AttributeType.FullName))
								.Select(attr => context.TypeSystemAstBuilder.ConvertAttribute(attr)).ToArray();
							if (attributes.Length > 0)
							{
								var section = new AttributeSection {
									AttributeTarget = "field"
								};
								section.Attributes.AddRange(attributes);
								pd.Attributes.Add(section);
							}
						}
					}

					if (PrimaryConstructorDecl.HasModifier(Modifiers.Unsafe))
					{
						this.TypeDeclaration.Modifiers |= Modifiers.Unsafe;
					}

					if (!PrimaryConstructorDecl.Initializer.IsNull && TypeDeclaration is { BaseTypes.Count: > 0 })
					{
						Debug.Assert(PrimaryConstructorDecl.Initializer is { ConstructorInitializerType: ConstructorInitializerType.Base });

						var baseType = TypeDeclaration.BaseTypes.First();
						var newBaseType = new InvocationAstType();
						baseType.ReplaceWith(newBaseType);
						newBaseType.BaseType = baseType;
						PrimaryConstructorDecl.Initializer.GetChildrenByRole(Roles.Argument).MoveTo(newBaseType.Arguments);
					}

					PrimaryConstructorDecl.Remove();
				}

				// Remove static constructor
				if (StaticConstructor != null)
				{
					Debug.Assert(StaticConstructorDecl != null);

					if (IsBeforeFieldInit && StaticConstructorDecl.Body.Statements.Count == 0)
					{
						StaticConstructorDecl.Remove();
					}
				}

				// More than one constructor - do not remove anything
				if (InstanceConstructors.Length != 1)
					return;

				var ctor = InstanceConstructors[0];
				var ctorMethod = (IMethod)ctor.GetSymbol();

				if (TypeDefinition.Kind == TypeKind.Struct && ctorMethod.Parameters.Count == 0 && InstanceInitializers != null)
				{
					// struct constructor with initializers is not optional
					return;
				}

				// dynamically create a pattern of an empty ctor
				ConstructorDeclaration emptyCtorPattern = new ConstructorDeclaration();
				emptyCtorPattern.Modifiers = TypeDefinition.IsAbstract ? Modifiers.Protected : Modifiers.Public;
				if (ctor.HasModifier(Modifiers.Unsafe))
					emptyCtorPattern.Modifiers |= Modifiers.Unsafe;
				emptyCtorPattern.Body = new BlockStatement();

				if (emptyCtorPattern.IsMatch(ctor))
				{
					bool retainBecauseOfDocumentation = context.Settings.ShowXmlDocumentation
						&& context.DecompileRun.DocumentationProvider?.GetDocumentation(ctorMethod) != null;
					if (!retainBecauseOfDocumentation)
						ctor.Remove();
				}
			}

			/// <summary>
			/// Evaluates a call to the decimal-ctor.
			/// </summary>
			private static bool TryEvaluateDecimalConstant(Semantics.ResolveResult expression, out decimal value)
			{
				value = 0;
				if (!expression.Type.IsKnownType(KnownTypeCode.Decimal))
				{
					return false;
				}
				switch (expression)
				{
					case CSharpInvocationResolveResult rr:
						if (!(rr.GetSymbol() is IMethod { SymbolKind: SymbolKind.Constructor } ctor))
							return false;
						var args = rr.GetArgumentsForCall();
						if (args.Count == 1)
						{
							switch (args[0].ConstantValue)
							{
								case double d:
									value = new decimal(d);
									return true;
								case float f:
									value = new decimal(f);
									return true;
								case long l:
									value = new decimal(l);
									return true;
								case int i:
									value = new decimal(i);
									return true;
								case ulong ul:
									value = new decimal(ul);
									return true;
								case uint ui:
									value = new decimal(ui);
									return true;
								case int[] bits when bits.Length == 4 && (bits[3] & 0x7F00FFFF) == 0 && (bits[3] & 0xFF000000) <= 0x1C000000:
									value = new decimal(bits);
									return true;
								default:
									return false;
							}
						}
						else if (args.Count == 5 &&
							args[0].ConstantValue is int lo &&
							args[1].ConstantValue is int mid &&
							args[2].ConstantValue is int hi &&
							args[3].ConstantValue is bool isNegative &&
							args[4].ConstantValue is byte scale)
						{
							value = new decimal(lo, mid, hi, isNegative, scale);
							return true;
						}
						return false;
					default:
						if (expression.ConstantValue is decimal v)
						{
							value = v;
							return true;
						}
						return false;
				}
			}
		}

		[AllowNull]
		TransformContext context;

		public void Run(AstNode node, TransformContext context)
		{
			this.context = context;

			if (context.CurrentTypeDefinition != null)
			{
				TransformDeclaration(context.CurrentTypeDefinition, node, node.Children.OfType<EntityDeclaration>());
			}

			foreach (var typeDeclaration in node.Descendants.OfType<TypeDeclaration>())
			{
				var currentTypeDefinition = (ITypeDefinition)typeDeclaration.GetSymbol();
				TransformDeclaration(currentTypeDefinition, typeDeclaration, typeDeclaration.Members);
			}
		}

		private bool TransformDeclaration(ITypeDefinition currentTypeDefinition, AstNode node, IEnumerable<EntityDeclaration> members)
		{
			var analyzer = new ConstructorInitializerAnalyzer(context, currentTypeDefinition, node as TypeDeclaration);

			if (!analyzer.Analyze(members))
				return false;

			if (analyzer.PrimaryConstructorInitializers is { HasDuplicateAssignments: false })
			{
				analyzer.MoveFieldInitializersToDeclarations(analyzer.PrimaryConstructorInitializers, InitializerKind.Primary);
			}
			else if (analyzer.InstanceInitializers is { HasDuplicateAssignments: false })
			{
				analyzer.MoveFieldInitializersToDeclarations(analyzer.InstanceInitializers, InitializerKind.Instance);
			}

			if (analyzer.StaticInitializers is { HasDuplicateAssignments: false })
			{
				analyzer.MoveFieldInitializersToDeclarations(analyzer.StaticInitializers, InitializerKind.Static);
			}

			foreach (var constructorDeclaration in members.OfType<ConstructorDeclaration>())
			{
				analyzer.MoveConstructorInitializer(constructorDeclaration, (IMethod)constructorDeclaration.GetSymbol());
			}

			analyzer.RemoveImplicitConstructor();

			if (analyzer.BackingFieldToPrimaryConstructorParameterVariableMap == null)
			{
				return false;
			}

			foreach (Identifier identifier in node.Descendants.OfType<Identifier>())
			{
				if (identifier.Parent?.GetSymbol() is not IField field)
				{
					continue;
				}
				if (!analyzer.BackingFieldToPrimaryConstructorParameterVariableMap.TryGetValue((IField)field.MemberDefinition, out var v))
				{
					continue;
				}
				identifier.Parent.RemoveAnnotations<MemberResolveResult>();
				identifier.Parent.AddAnnotation(new ILVariableResolveResult(v));
				identifier.ReplaceWith(Identifier.Create(v.Name));
			}

			return true;
		}
	}
}
