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
using ICSharpCode.Decompiler.Ast;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using Mono.Cecil;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.ControlFlow;
using ICSharpCode.Decompiler.IL.Transforms;

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// Main class of the C# decompiler engine.
	/// </summary>
	/// <remarks>
	/// Instances of this class are not thread-safe. Use separate instances to decompile multiple members in parallel.
	/// (in particular, the transform instances are not thread-safe)
	/// </remarks>
	public class CSharpDecompiler
	{
		readonly DecompilerTypeSystem typeSystem;
		readonly DecompilerSettings settings;

		List<IILTransform> ilTransforms = GetILTransforms();

		public static List<IILTransform> GetILTransforms()
		{
			return new List<IILTransform> {
				new ControlFlowSimplification(),
				// Run SplitVariables only after ControlFlowSimplification duplicates return blocks,
				// so that the return variable is split and can be inlined.
				new SplitVariables(),
				new ILInlining(),
				new DetectPinnedRegions(), // must run after inlining but before non-critical control flow transforms
				new SwitchDetection(),
				new LoopDetection(),
				new IntroduceExitPoints(),
				new BlockILTransform( // per-block transforms
					new ConditionDetection(),
					new LoopingBlockTransform( // per-block transforms that depend on each other, and thus need to loop
					)
				),
				new ILInlining(),
				new TransformAssignment(),
				new CopyPropagation(),
				new InlineCompilerGeneratedVariables(),
				new ExpressionTransforms(), // must run once before "the loop" to allow RemoveDeadVariablesInit
				new RemoveDeadVariableInit(), // must run after ExpressionTransforms because it does not handle stobj(ldloca V, ...)
				new DelegateConstruction(),
				new LoopingTransform( // the loop: transforms that cyclicly depend on each other
					new ExpressionTransforms(),
					new TransformArrayInitializers(),
					new ILInlining()
				)
			};
		}

		List<IAstTransform> astTransforms = new List<IAstTransform> {
			new PatternStatementTransform(),
			new ReplaceMethodCallsWithOperators(),
			new IntroduceUnsafeModifier(),
			new AddCheckedBlocks(),
			new DeclareVariables(), // should run after most transforms that modify statements
			new ConvertConstructorCallIntoInitializer(), // must run after DeclareVariables
			new DecimalConstantTransform(),
			new IntroduceUsingDeclarations(),
			new FixNameCollisions(),
			//new IntroduceExtensionMethods(context), // must run after IntroduceUsingDeclarations
			//new IntroduceQueryExpressions(context), // must run after IntroduceExtensionMethods
			//new CombineQueryExpressions(context),
			//new FlattenSwitchBlocks(),
		};

		public CancellationToken CancellationToken { get; set; }

		/// <summary>
		/// IL transforms.
		/// </summary>
		public IList<IILTransform> ILTransforms {
			get { return ilTransforms; }
		}

		/// <summary>
		/// C# AST transforms.
		/// </summary>
		public IList<IAstTransform> AstTransforms {
			get { return astTransforms; }
		}

		public CSharpDecompiler(ModuleDefinition module, DecompilerSettings settings)
			: this(new DecompilerTypeSystem(module), settings)
		{
		}

		public CSharpDecompiler(DecompilerTypeSystem typeSystem, DecompilerSettings settings)
		{
			if (typeSystem == null)
				throw new ArgumentNullException(nameof(typeSystem));
			this.typeSystem = typeSystem;
			this.settings = settings;
		}
		
		#region MemberIsHidden
		public static bool MemberIsHidden(MemberReference member, DecompilerSettings settings)
		{
			MethodDefinition method = member as MethodDefinition;
			if (method != null) {
				if (method.IsGetter || method.IsSetter || method.IsAddOn || method.IsRemoveOn)
					return true;
				if (settings.AnonymousMethods && method.HasGeneratedName() && method.IsCompilerGenerated())
					return true;
			}

			TypeDefinition type = member as TypeDefinition;
			if (type != null) {
				if (type.DeclaringType != null) {
					if (settings.AnonymousMethods && IsClosureType(type))
						return true;
//					if (settings.YieldReturn && YieldReturnDecompiler.IsCompilerGeneratorEnumerator(type))
//						return true;
//					if (settings.AsyncAwait && AsyncDecompiler.IsCompilerGeneratedStateMachine(type))
//						return true;
				} else if (type.IsCompilerGenerated()) {
//					if (type.Name.StartsWith("<PrivateImplementationDetails>", StringComparison.Ordinal))
//						return true;
					if (type.IsAnonymousType())
						return true;
				}
			}
			
			FieldDefinition field = member as FieldDefinition;
			if (field != null) {
				if (field.IsCompilerGenerated()) {
					if (settings.AnonymousMethods && IsAnonymousMethodCacheField(field))
						return true;
					if (settings.AutomaticProperties && IsAutomaticPropertyBackingField(field))
						return true;
//					if (settings.SwitchStatementOnString && IsSwitchOnStringCache(field))
//						return true;
				}
				// event-fields are not [CompilerGenerated]
//				if (settings.AutomaticEvents && field.DeclaringType.Events.Any(ev => ev.Name == field.Name))
//					return true;
				// HACK : only hide fields starting with '__StaticArrayInit'
				if (field.DeclaringType.Name.StartsWith("<PrivateImplementationDetails>", StringComparison.Ordinal)) {
					if (field.Name.StartsWith("__StaticArrayInit", StringComparison.Ordinal))
						return true;
					if (field.FieldType.Name.StartsWith("__StaticArrayInit", StringComparison.Ordinal))
						return true;
				}
			}
			
			return false;
		}
		
		static bool IsSwitchOnStringCache(FieldDefinition field)
		{
			return field.Name.StartsWith("<>f__switch", StringComparison.Ordinal);
		}

		static bool IsAutomaticPropertyBackingField(FieldDefinition field)
		{
			return field.HasGeneratedName() && field.Name.EndsWith("BackingField", StringComparison.Ordinal);
		}

		static bool IsAnonymousMethodCacheField(FieldDefinition field)
		{
			return field.Name.StartsWith("CS$<>", StringComparison.Ordinal) || field.Name.StartsWith("<>f__am", StringComparison.Ordinal);
		}

		static bool IsClosureType(TypeDefinition type)
		{
			if (!type.HasGeneratedName() || !type.IsCompilerGenerated())
				return false;
			if (type.Name.Contains("DisplayClass") || type.Name.Contains("AnonStorey"))
				return true;
			return type.BaseType.FullName == "System.Object" && !type.HasInterfaces;
		}
		#endregion
		
		TypeSystemAstBuilder CreateAstBuilder(ITypeResolveContext decompilationContext)
		{
			var typeSystemAstBuilder = new TypeSystemAstBuilder();
			typeSystemAstBuilder.ShowAttributes = true;
			typeSystemAstBuilder.AlwaysUseShortTypeNames = true;
			typeSystemAstBuilder.AddResolveResultAnnotations = true;
			return typeSystemAstBuilder;
		}
		
		void RunTransforms(AstNode rootNode, ITypeResolveContext decompilationContext)
		{
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			var context = new TransformContext(typeSystem, decompilationContext, typeSystemAstBuilder, settings, CancellationToken);
			foreach (var transform in astTransforms) {
				CancellationToken.ThrowIfCancellationRequested();
				transform.Run(rootNode, context);
			}
			rootNode.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
		}
		
		/// <summary>
		/// Decompile assembly and module attributes.
		/// </summary>
		public SyntaxTree DecompileModuleAndAssemblyAttributes()
		{
			var decompilationContext = new SimpleTypeResolveContext(typeSystem.MainAssembly);
			SyntaxTree syntaxTree = new SyntaxTree();
			DoDecompileModuleAndAssemblyAttributes(decompilationContext, syntaxTree);
			RunTransforms(syntaxTree, decompilationContext);
			return syntaxTree;
		}

		void DoDecompileModuleAndAssemblyAttributes(ITypeResolveContext decompilationContext, SyntaxTree syntaxTree)
		{
			foreach (var a in typeSystem.Compilation.MainAssembly.AssemblyAttributes) {
				var astBuilder = CreateAstBuilder(decompilationContext);
				var attrSection = new AttributeSection(astBuilder.ConvertAttribute(a));
				attrSection.AttributeTarget = "assembly";
				syntaxTree.AddChild(attrSection, SyntaxTree.MemberRole);
			}
		}

		void DoDecompileTypes(IEnumerable<TypeDefinition> types, ITypeResolveContext decompilationContext, SyntaxTree syntaxTree)
		{
			string currentNamespace = null;
			AstNode groupNode = null;
			foreach (var cecilType in types) {
				var typeDef = typeSystem.Resolve(cecilType).GetDefinition();
				if (typeDef.Name == "<Module>" && typeDef.Members.Count == 0)
					continue;
				if (MemberIsHidden(cecilType, settings))
					continue;
				if(string.IsNullOrEmpty(cecilType.Namespace)) {
					groupNode = syntaxTree;
				} else {
					if (currentNamespace != cecilType.Namespace) {
						groupNode = new NamespaceDeclaration(cecilType.Namespace);
						syntaxTree.AddChild(groupNode, SyntaxTree.MemberRole);
					}
				}
				currentNamespace = cecilType.Namespace;
				var typeDecl = DoDecompile(typeDef, decompilationContext.WithCurrentTypeDefinition(typeDef));
				groupNode.AddChild(typeDecl, SyntaxTree.MemberRole);
			}
		}
		
		/// <summary>
		/// Decompiles the whole module into a single syntax tree.
		/// </summary>
		public SyntaxTree DecompileWholeModuleAsSingleFile()
		{
			var decompilationContext = new SimpleTypeResolveContext(typeSystem.MainAssembly);
			SyntaxTree syntaxTree = new SyntaxTree();
			DoDecompileModuleAndAssemblyAttributes(decompilationContext, syntaxTree);
			DoDecompileTypes(typeSystem.ModuleDefinition.Types, decompilationContext, syntaxTree);
			RunTransforms(syntaxTree, decompilationContext);
			return syntaxTree;
		}
		
		/// <summary>
		/// Decompile the given types.
		/// </summary>
		/// <remarks>
		/// Unlike Decompile(IMemberDefinition[]), this method will add namespace declarations around the type definitions.
		/// </remarks>
		public SyntaxTree DecompileTypes(IEnumerable<TypeDefinition> types)
		{
			if (types == null)
				throw new ArgumentNullException(nameof(types));
			var decompilationContext = new SimpleTypeResolveContext(typeSystem.MainAssembly);
			SyntaxTree syntaxTree = new SyntaxTree();
			DoDecompileTypes(types, decompilationContext, syntaxTree);
			RunTransforms(syntaxTree, decompilationContext);
			return syntaxTree;
		}
		
		/// <summary>
		/// Decompile the specified types and/or members.
		/// </summary>
		public SyntaxTree Decompile(params IMemberDefinition[] definitions)
		{
			if (definitions == null)
				throw new ArgumentNullException(nameof(definitions));
			ITypeDefinition parentTypeDef = null;
			var syntaxTree = new SyntaxTree();
			foreach (var def in definitions) {
				if (def == null)
					throw new ArgumentException("definitions contains null element");
				var typeDefinition = def as TypeDefinition;
				var methodDefinition = def as MethodDefinition;
				var fieldDefinition = def as FieldDefinition;
				var propertyDefinition = def as PropertyDefinition;
				var eventDefinition = def as EventDefinition;
				if (typeDefinition != null) {
					ITypeDefinition typeDef = typeSystem.Resolve(typeDefinition).GetDefinition();
					if (typeDef == null)
						throw new InvalidOperationException("Could not find type definition in NR type system");
					syntaxTree.Members.Add(DoDecompile(typeDef, new SimpleTypeResolveContext(typeDef)));
					parentTypeDef = typeDef.DeclaringTypeDefinition;
				} else if (methodDefinition != null) {
					IMethod method = typeSystem.Resolve(methodDefinition);
					if (method == null)
						throw new InvalidOperationException("Could not find method definition in NR type system");
					syntaxTree.Members.Add(DoDecompile(methodDefinition, method, new SimpleTypeResolveContext(method)));
					parentTypeDef = method.DeclaringTypeDefinition;
				} else if (fieldDefinition != null) {
					IField field = typeSystem.Resolve(fieldDefinition);
					if (field == null)
						throw new InvalidOperationException("Could not find field definition in NR type system");
					syntaxTree.Members.Add(DoDecompile(fieldDefinition, field, new SimpleTypeResolveContext(field)));
					parentTypeDef = field.DeclaringTypeDefinition;
				} else if (propertyDefinition != null) {
					IProperty property = typeSystem.Resolve(propertyDefinition);
					if (property == null)
						throw new InvalidOperationException("Could not find field definition in NR type system");
					syntaxTree.Members.Add(DoDecompile(propertyDefinition, property, new SimpleTypeResolveContext(property)));
					parentTypeDef = property.DeclaringTypeDefinition;
				} else if (eventDefinition != null) {
					IEvent ev = typeSystem.Resolve(eventDefinition);
					if (ev == null)
						throw new InvalidOperationException("Could not find field definition in NR type system");
					syntaxTree.Members.Add(DoDecompile(eventDefinition, ev, new SimpleTypeResolveContext(ev)));
					parentTypeDef = ev.DeclaringTypeDefinition;
				} else {
					throw new NotSupportedException(def.GetType().Name);
				}
			}
			RunTransforms(syntaxTree, parentTypeDef != null ? new SimpleTypeResolveContext(parentTypeDef) : new SimpleTypeResolveContext(typeSystem.MainAssembly));
			return syntaxTree;
		}

		IEnumerable<EntityDeclaration> AddInterfaceImplHelpers(EntityDeclaration memberDecl, MethodDefinition methodDef,
		                                                       TypeSystemAstBuilder astBuilder)
		{
			if (!memberDecl.GetChildByRole(EntityDeclaration.PrivateImplementationTypeRole).IsNull) {
				yield break; // cannot create forwarder for existing explicit interface impl
			}
			foreach (var mr in methodDef.Overrides) {
				IMethod m = typeSystem.Resolve(mr);
				if (m == null || m.DeclaringType.Kind != TypeKind.Interface)
					continue;
				var methodDecl = new MethodDeclaration();
				methodDecl.ReturnType = memberDecl.ReturnType.Clone();
				methodDecl.PrivateImplementationType = astBuilder.ConvertType(m.DeclaringType);
				methodDecl.Name = m.Name;
				methodDecl.TypeParameters.AddRange(memberDecl.GetChildrenByRole(Roles.TypeParameter)
				                                   .Select(n => (TypeParameterDeclaration)n.Clone()));
				methodDecl.Parameters.AddRange(memberDecl.GetChildrenByRole(Roles.Parameter).Select(n => n.Clone()));
				methodDecl.Constraints.AddRange(memberDecl.GetChildrenByRole(Roles.Constraint)
				                                .Select(n => (Constraint)n.Clone()));
				
				methodDecl.Body = new BlockStatement();
				methodDecl.Body.AddChild(new Comment(
					"ILSpy generated this explicit interface implementation from .override directive in " + memberDecl.Name),
				                         Roles.Comment);
				var forwardingCall = new ThisReferenceExpression().Invoke(
					memberDecl.Name,
					methodDecl.TypeParameters.Select(tp => new SimpleType(tp.Name)),
					methodDecl.Parameters.Select(p => ForwardParameter(p))
				);
				if (m.ReturnType.IsKnownType(KnownTypeCode.Void)) {
					methodDecl.Body.Add(new ExpressionStatement(forwardingCall));
				} else {
					methodDecl.Body.Add(new ReturnStatement(forwardingCall));
				}
				yield return methodDecl;
			}
		}
		
		Expression ForwardParameter(ParameterDeclaration p)
		{
			switch (p.ParameterModifier) {
				case ParameterModifier.Ref:
					return new DirectionExpression(FieldDirection.Ref, new IdentifierExpression(p.Name));
				case ParameterModifier.Out:
					return new DirectionExpression(FieldDirection.Out, new IdentifierExpression(p.Name));
				default:
					return new IdentifierExpression(p.Name);
			}
		}
		
		/// <summary>
		/// Sets new modifier if the member hides some other member from a base type.
		/// </summary>
		/// <param name="member">The node of the member which new modifier state should be determined.</param>
		void SetNewModifier(EntityDeclaration member)
		{
			bool addNewModifier = false;
			var entity = (IEntity)member.GetSymbol();
			var lookup = new MemberLookup(entity.DeclaringTypeDefinition, entity.ParentAssembly);
			
			var baseTypes = entity.DeclaringType.GetNonInterfaceBaseTypes().Where(t => entity.DeclaringType != t);
			if (entity is ITypeDefinition) {
				addNewModifier = baseTypes.SelectMany(b => b.GetNestedTypes(t => t.Name == entity.Name && lookup.IsAccessible(t, true))).Any();
			} else {
				var members = baseTypes.SelectMany(b => b.GetMembers(m => m.Name == entity.Name).Where(m => lookup.IsAccessible(m, true)));
				switch (entity.SymbolKind) {
					case SymbolKind.Field:
					case SymbolKind.Property:
					case SymbolKind.Event:
						addNewModifier = members.Any();
						break;
					case SymbolKind.Method:
					case SymbolKind.Constructor:
					case SymbolKind.Indexer:
					case SymbolKind.Operator:
						addNewModifier = members.Any(m => SignatureComparer.Ordinal.Equals(m, (IMember)entity));
						break;
					default:
						throw new NotSupportedException();
				}
			}

			if (addNewModifier)
				member.Modifiers |= Modifiers.New;
		}

		void FixParameterNames(EntityDeclaration entity)
		{
			int i = 0;
			foreach (var parameter in entity.GetChildrenByRole(Roles.Parameter)) {
				if (string.IsNullOrEmpty(parameter.Name) && !parameter.Type.IsArgList()) {
					// needs to be consistent with logic in ILReader.CreateILVarable(ParameterDefinition)
					parameter.Name = "P_" + i;
				}
				i++;
			}
		}
		
		EntityDeclaration DoDecompile(ITypeDefinition typeDef, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentTypeDefinition == typeDef);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			var entityDecl = typeSystemAstBuilder.ConvertEntity(typeDef);
			var typeDecl = entityDecl as TypeDeclaration;
			if (typeDecl == null) {
				// e.g. DelegateDeclaration
				return entityDecl;
			}
			foreach (var type in typeDef.NestedTypes) {
				var cecilType = typeSystem.GetCecil(type);
				if (cecilType != null && !MemberIsHidden(cecilType, settings)) {
					var nestedType = DoDecompile(type, decompilationContext.WithCurrentTypeDefinition(type));
					SetNewModifier(nestedType);
					typeDecl.Members.Add(nestedType);
				}
			}
			foreach (var field in typeDef.Fields) {
				var fieldDef = typeSystem.GetCecil(field) as FieldDefinition;
				if (fieldDef != null && !MemberIsHidden(fieldDef, settings)) {
					var memberDecl = DoDecompile(fieldDef, field, decompilationContext.WithCurrentMember(field));
					typeDecl.Members.Add(memberDecl);
				}
			}
			foreach (var property in typeDef.Properties) {
				var propDef = typeSystem.GetCecil(property) as PropertyDefinition;
				if (propDef != null && !MemberIsHidden(propDef, settings)) {
					var propDecl = DoDecompile(propDef, property, decompilationContext.WithCurrentMember(property));
					typeDecl.Members.Add(propDecl);
				}
			}
			foreach (var @event in typeDef.Events) {
				var eventDef = typeSystem.GetCecil(@event) as EventDefinition;
				if (eventDef != null && !MemberIsHidden(eventDef, settings)) {
					var eventDecl = DoDecompile(eventDef, @event, decompilationContext.WithCurrentMember(@event));
					typeDecl.Members.Add(eventDecl);
				}
			}
			foreach (var method in typeDef.Methods) {
				var methodDef = typeSystem.GetCecil(method) as MethodDefinition;
				if (methodDef != null && !MemberIsHidden(methodDef, settings)) {
					var memberDecl = DoDecompile(methodDef, method, decompilationContext.WithCurrentMember(method));
					typeDecl.Members.Add(memberDecl);
					typeDecl.Members.AddRange(AddInterfaceImplHelpers(memberDecl, methodDef, typeSystemAstBuilder));
				}
			}
			if (typeDecl.Members.OfType<IndexerDeclaration>().Any(idx => idx.PrivateImplementationType.IsNull)) {
				// Remove the [DefaultMember] attribute if the class contains indexers
				foreach (AttributeSection section in typeDecl.Attributes) {
					foreach (var attr in section.Attributes) {
						var tr = attr.Type.GetResolveResult().Type;
						if (tr.Name == "DefaultMemberAttribute" && tr.Namespace == "System.Reflection") {
							attr.Remove();
						}
					}
					if (section.Attributes.Count == 0)
						section.Remove();
				}
			}
			return typeDecl;
		}

		MethodDeclaration GenerateConvHelper(string name, KnownTypeCode source, KnownTypeCode target, TypeSystemAstBuilder typeSystemAstBuilder,
		                                     Expression intermediate32, Expression intermediate64)
		{
			MethodDeclaration method = new MethodDeclaration();
			method.Name = name;
			method.Modifiers = Modifiers.Private | Modifiers.Static;
			method.Parameters.Add(new ParameterDeclaration(typeSystemAstBuilder.ConvertType(typeSystem.Compilation.FindType(source)), "input"));
			method.ReturnType = typeSystemAstBuilder.ConvertType(typeSystem.Compilation.FindType(target));
			method.Body = new BlockStatement {
				new IfElseStatement {
					Condition = new BinaryOperatorExpression {
						Left = typeSystemAstBuilder.ConvertType(typeSystem.Compilation.FindType(KnownTypeCode.IntPtr)).Member("Size"),
						Operator = BinaryOperatorType.Equality,
						Right = new PrimitiveExpression(4)
					},
					TrueStatement = new BlockStatement { // 32-bit
						new ReturnStatement(
							new CastExpression(
								method.ReturnType.Clone(),
								intermediate32
							)
						)
					},
					FalseStatement = new BlockStatement { // 64-bit
						new ReturnStatement(
							new CastExpression(
								method.ReturnType.Clone(),
								intermediate64
							)
						)
					},
				}
			};
			return method;
		}
		
		EntityDeclaration DoDecompile(MethodDefinition methodDefinition, IMethod method, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == method);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			var methodDecl = typeSystemAstBuilder.ConvertEntity(method);
			int lastDot = method.Name.LastIndexOf('.');
			if (method.IsExplicitInterfaceImplementation && lastDot >= 0) {
				methodDecl.Name = method.Name.Substring(lastDot + 1);
			}
			FixParameterNames(methodDecl);
			if (methodDefinition.HasBody) {
				DecompileBody(methodDefinition, method, methodDecl, decompilationContext);
			} else if (!method.IsAbstract && method.DeclaringType.Kind != TypeKind.Interface) {
				methodDecl.Modifiers |= Modifiers.Extern;
			}
			if (method.SymbolKind == SymbolKind.Method && !method.IsExplicitInterfaceImplementation && methodDefinition.IsVirtual == methodDefinition.IsNewSlot) {
				SetNewModifier(methodDecl);
			}
			return methodDecl;
		}

		void DecompileBody(MethodDefinition methodDefinition, IMethod method, EntityDeclaration entityDecl, ITypeResolveContext decompilationContext)
		{
			var specializingTypeSystem = typeSystem.GetSpecializingTypeSystem(decompilationContext);
			ILFunction function = ILFunction.Read(specializingTypeSystem, methodDefinition, CancellationToken);
			
			if (entityDecl != null) {
				int i = 0;
				var parameters = function.Variables.Where(v => v.Kind == VariableKind.Parameter).ToDictionary(v => v.Index);
				foreach (var parameter in entityDecl.GetChildrenByRole(Roles.Parameter)) {
					ILVariable v;
					if (parameters.TryGetValue(i, out v))
						parameter.AddAnnotation(new ILVariableResolveResult(v, method.Parameters[i].Type));
					i++;
				}
			}
			
			var context = new ILTransformContext { Settings = settings, TypeSystem = specializingTypeSystem, CancellationToken = CancellationToken };
			foreach (var transform in ilTransforms) {
				CancellationToken.ThrowIfCancellationRequested();
				transform.Run(function, context);
				function.CheckInvariant(ILPhase.Normal);
			}
			
			var statementBuilder = new StatementBuilder(specializingTypeSystem, decompilationContext, method);
			entityDecl.AddChild(statementBuilder.ConvertAsBlock(function.Body), Roles.Body);
		}

		EntityDeclaration DoDecompile(FieldDefinition fieldDefinition, IField field, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == field);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			if (decompilationContext.CurrentTypeDefinition.Kind == TypeKind.Enum) {
				var enumDec = new EnumMemberDeclaration {
					Name = field.Name,
					Initializer = typeSystemAstBuilder.ConvertConstantValue(decompilationContext.CurrentTypeDefinition.EnumUnderlyingType, field.ConstantValue),
				};
				enumDec.Attributes.AddRange(field.Attributes.Select(a => new AttributeSection(typeSystemAstBuilder.ConvertAttribute(a))));
				return enumDec;
			}
			var fieldDecl = typeSystemAstBuilder.ConvertEntity(field);
			SetNewModifier(fieldDecl);
			return fieldDecl;
		}

		EntityDeclaration DoDecompile(PropertyDefinition propertyDefinition, IProperty property, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == property);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			EntityDeclaration propertyDecl = typeSystemAstBuilder.ConvertEntity(property);
			int lastDot = property.Name.LastIndexOf('.');
			if (property.IsExplicitInterfaceImplementation && !property.IsIndexer) {
				propertyDecl.Name = property.Name.Substring(lastDot + 1);
			}
			FixParameterNames(propertyDecl);
			Accessor getter, setter;
			if (propertyDecl is PropertyDeclaration) {
				getter = ((PropertyDeclaration)propertyDecl).Getter;
				setter = ((PropertyDeclaration)propertyDecl).Setter;
			} else {
				getter = ((IndexerDeclaration)propertyDecl).Getter;
				setter = ((IndexerDeclaration)propertyDecl).Setter;
			}
			if (property.CanGet && property.Getter.HasBody) {
				DecompileBody(propertyDefinition.GetMethod, property.Getter, getter, decompilationContext);
			}
			if (property.CanSet && property.Setter.HasBody) {
				DecompileBody(propertyDefinition.SetMethod, property.Setter, setter, decompilationContext);
			}
			var accessor = propertyDefinition.GetMethod ?? propertyDefinition.SetMethod;
			if (!accessor.HasOverrides && accessor.IsVirtual == accessor.IsNewSlot)
				SetNewModifier(propertyDecl);
			return propertyDecl;
		}
		
		EntityDeclaration DoDecompile(EventDefinition eventDefinition, IEvent ev, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == ev);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			typeSystemAstBuilder.UseCustomEvents = ev.DeclaringTypeDefinition.Kind != TypeKind.Interface;
			var eventDecl = typeSystemAstBuilder.ConvertEntity(ev);
			int lastDot = ev.Name.LastIndexOf('.');
			if (ev.IsExplicitInterfaceImplementation) {
				eventDecl.Name = ev.Name.Substring(lastDot + 1);
			}
			if (eventDefinition.AddMethod != null && eventDefinition.AddMethod.HasBody) {
				DecompileBody(eventDefinition.AddMethod, ev.AddAccessor, ((CustomEventDeclaration)eventDecl).AddAccessor, decompilationContext);
			}
			if (eventDefinition.RemoveMethod != null && eventDefinition.RemoveMethod.HasBody) {
				DecompileBody(eventDefinition.RemoveMethod, ev.RemoveAccessor, ((CustomEventDeclaration)eventDecl).RemoveAccessor, decompilationContext);
			}
			var accessor = eventDefinition.AddMethod ?? eventDefinition.RemoveMethod;
			if (accessor.IsVirtual == accessor.IsNewSlot) {
				SetNewModifier(eventDecl);
			}
			return eventDecl;
		}
	}
}
