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
	/// </remarks>
	public class CSharpDecompiler
	{
		readonly DecompilerTypeSystem typeSystem;
		readonly DecompilerSettings settings;

		List<IILTransform> ilTransforms = new List<IILTransform> {
			//new RemoveDeadVariableInit(),
			new SplitVariables(),
			new ControlFlowSimplification(),
			new ILInlining(), // temporary pass, just to make the ILAst easier to read while debugging loop detection
			new LoopDetection(),
			new IntroduceExitPoints(),
			new ConditionDetection(),
			new ILInlining(),
			new CopyPropagation(),
			new InlineCompilerGeneratedVariables(),
			new TransformingVisitor(),
			new LoopingTransform(
				new ExpressionTransforms(),
				new TransformArrayInitializers(),
				new ILInlining()
			)
		};

		List<IAstTransform> astTransforms = new List<IAstTransform> {
			//new PushNegation(),
			//new DelegateConstruction(context),
			//new PatternStatementTransform(context),
			new ReplaceMethodCallsWithOperators(),
			new IntroduceUnsafeModifier(),
			new AddCheckedBlocks(),
			//new DeclareVariables(), // should run after most transforms that modify statements
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
				throw new ArgumentNullException("typeSystem");
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
//				if (settings.AnonymousMethods && method.HasGeneratedName() && method.IsCompilerGenerated())
//					return true;
			}

			TypeDefinition type = member as TypeDefinition;
			if (type != null) {
				if (type.DeclaringType != null) {
//					if (settings.AnonymousMethods && IsClosureType(type))
//						return true;
//					if (settings.YieldReturn && YieldReturnDecompiler.IsCompilerGeneratorEnumerator(type))
//						return true;
//					if (settings.AsyncAwait && AsyncDecompiler.IsCompilerGeneratedStateMachine(type))
//						return true;
				} else if (type.IsCompilerGenerated()) {
//					if (type.Name.StartsWith("<PrivateImplementationDetails>", StringComparison.Ordinal))
//						return true;
//					if (type.IsAnonymousType())
//						return true;
				}
			}
			
			FieldDefinition field = member as FieldDefinition;
			if (field != null) {
				if (field.IsCompilerGenerated()) {
//					if (settings.AnonymousMethods && IsAnonymousMethodCacheField(field))
//						return true;
//					if (settings.AutomaticProperties && IsAutomaticPropertyBackingField(field))
//						return true;
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
			return type.HasGeneratedName() && type.IsCompilerGenerated() && (type.Name.Contains("DisplayClass") || type.Name.Contains("AnonStorey"));
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
			var context = new TransformContext(typeSystem, decompilationContext, typeSystemAstBuilder, CancellationToken);
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
				throw new ArgumentNullException("types");
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
				throw new ArgumentNullException("definitions");
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
				} else if (methodDefinition != null) {
					IMethod method = typeSystem.Resolve(methodDefinition);
					if (method == null)
						throw new InvalidOperationException("Could not find method definition in NR type system");
					syntaxTree.Members.Add(DoDecompile(methodDefinition, method, new SimpleTypeResolveContext(method)));
				} else if (fieldDefinition != null) {
					IField field = typeSystem.Resolve(fieldDefinition);
					if (field == null)
						throw new InvalidOperationException("Could not find field definition in NR type system");
					syntaxTree.Members.Add(DoDecompile(fieldDefinition, field, new SimpleTypeResolveContext(field)));
				} else if (propertyDefinition != null) {
					IProperty property = typeSystem.Resolve(propertyDefinition);
					if (property == null)
						throw new InvalidOperationException("Could not find field definition in NR type system");
					syntaxTree.Members.Add(DoDecompile(propertyDefinition, property, new SimpleTypeResolveContext(property)));
				} else if (eventDefinition != null) {
					IEvent ev = typeSystem.Resolve(eventDefinition);
					if (ev == null)
						throw new InvalidOperationException("Could not find field definition in NR type system");
					syntaxTree.Members.Add(DoDecompile(eventDefinition, ev, new SimpleTypeResolveContext(ev)));
				} else {
					throw new NotSupportedException(def.GetType().Name);
				}
			}
			RunTransforms(syntaxTree, new SimpleTypeResolveContext(typeSystem.MainAssembly));
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
		
		#region SetNewModifier
		/// <summary>
		/// Sets new modifier if the member hides some other member from a base type.
		/// </summary>
		/// <param name="member">The node of the member which new modifier state should be determined.</param>
		void SetNewModifier(EntityDeclaration member)
		{
			bool addNewModifier = false;
			if (member is IndexerDeclaration) {
				var propertyDef = GetDefinitionFromAnnotation(member) as PropertyDefinition;
				if (propertyDef != null)
					addNewModifier = TypesHierarchyHelpers.FindBaseProperties(propertyDef).Any();
			} else
				addNewModifier = HidesBaseMember(member) == true;

			if (addNewModifier)
				member.Modifiers |= Modifiers.New;
		}
		
		IMemberDefinition GetDefinitionFromAnnotation(EntityDeclaration member)
		{
			if (member is TypeDeclaration) {
				return (IMemberDefinition)typeSystem.GetCecil(member.Annotation<TypeResolveResult>()?.Type.GetDefinition());
			} else {
				return (IMemberDefinition)typeSystem.GetCecil(member.Annotation<MemberResolveResult>()?.Member.MemberDefinition);
			}
		}
		
		bool? HidesBaseMember(EntityDeclaration member)
		{
			var memberDefinition = GetDefinitionFromAnnotation(member);
			if (memberDefinition == null)
				return null;
			var methodDefinition = memberDefinition as MethodDefinition;
			if (methodDefinition != null) {
				bool? hidesByName = HidesByName(memberDefinition, includeBaseMethods: false);
				return hidesByName != true && TypesHierarchyHelpers.FindBaseMethods(methodDefinition).Any();
			} else
				return HidesByName(memberDefinition, includeBaseMethods: true);
		}
		
		/// <summary>
		/// Determines whether any base class member has the same name as the given member.
		/// </summary>
		/// <param name="member">The derived type's member.</param>
		/// <param name="includeBaseMethods">true if names of methods declared in base types should also be checked.</param>
		/// <returns>true if any base member has the same name as given member, otherwise false. Returns null on error.</returns>
		static bool? HidesByName(IMemberDefinition member, bool includeBaseMethods)
		{
			Debug.Assert(!(member is PropertyDefinition) || !((PropertyDefinition)member).IsIndexer());

			if (member.DeclaringType.BaseType != null) {
				var baseTypeRef = member.DeclaringType.BaseType;
				while (baseTypeRef != null) {
					var baseType = baseTypeRef.Resolve();
					if (baseType == null)
						return null;
					if (baseType.HasProperties && AnyIsHiddenBy(baseType.Properties, member, m => !m.IsIndexer()))
						return true;
					if (baseType.HasEvents && AnyIsHiddenBy(baseType.Events, member))
						return true;
					if (baseType.HasFields && AnyIsHiddenBy(baseType.Fields, member))
						return true;
					if (includeBaseMethods && baseType.HasMethods
					    && AnyIsHiddenBy(baseType.Methods, member, m => !m.IsSpecialName))
						return true;
					if (baseType.HasNestedTypes && AnyIsHiddenBy(baseType.NestedTypes, member))
						return true;
					baseTypeRef = baseType.BaseType;
				}
			}
			return false;
		}

		static bool AnyIsHiddenBy<T>(IEnumerable<T> members, IMemberDefinition derived, Predicate<T> condition = null)
			where T : IMemberDefinition
		{
			int numberOfGenericParameters = (derived as IGenericParameterProvider)?.GenericParameters.Count ?? 0;
			return members.Any(m => m.Name == derived.Name
			                   && ((m as IGenericParameterProvider)?.GenericParameters.Count ?? 0) == numberOfGenericParameters
			                   && (condition == null || condition(m))
			                   && TypesHierarchyHelpers.IsVisibleFromDerived(m, derived.DeclaringType));
		}
		#endregion

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
				var nestedType = DoDecompile(type, decompilationContext.WithCurrentTypeDefinition(type));
				SetNewModifier(nestedType);
				typeDecl.Members.Add(nestedType);
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
				DecompileBody(methodDefinition, method, methodDecl, decompilationContext, typeSystemAstBuilder);
			} else if (!method.IsAbstract && method.DeclaringType.Kind != TypeKind.Interface) {
				methodDecl.Modifiers |= Modifiers.Extern;
			}
			if (decompilationContext.CurrentTypeDefinition.Kind != TypeKind.Interface
			    && method.SymbolKind == SymbolKind.Method
			    && methodDefinition.IsVirtual == methodDefinition.IsNewSlot) {
				SetNewModifier(methodDecl);
			}
			return methodDecl;
		}

		IDecompilerTypeSystem GetSpecializingTypeSystem(ITypeResolveContext decompilationContext)
		{
			IList<IType> classTypeParameters = null;
			IList<IType> methodTypeParameters = null;
			
			if (decompilationContext.CurrentTypeDefinition != null)
				classTypeParameters = decompilationContext.CurrentTypeDefinition.TypeArguments;
			IMethod method = decompilationContext.CurrentMember as IMethod;
			if (method != null)
				methodTypeParameters = method.TypeArguments;
			
			if ((classTypeParameters != null && classTypeParameters.Count > 0) || (methodTypeParameters != null && methodTypeParameters.Count > 0))
				return new SpecializingDecompilerTypeSystem(typeSystem, new TypeParameterSubstitution(classTypeParameters, methodTypeParameters));
			else
				return typeSystem;
		}
		
		void DecompileBody(MethodDefinition methodDefinition, IMethod method, EntityDeclaration entityDecl, ITypeResolveContext decompilationContext, TypeSystemAstBuilder typeSystemAstBuilder)
		{
			var specializingTypeSystem = GetSpecializingTypeSystem(decompilationContext);
			var ilReader = new ILReader(specializingTypeSystem);
			var function = ilReader.ReadIL(methodDefinition.Body, CancellationToken);
			function.CheckInvariant(ILPhase.Normal);
			var context = new ILTransformContext { TypeSystem = specializingTypeSystem, CancellationToken = CancellationToken };
			foreach (var transform in ilTransforms) {
				CancellationToken.ThrowIfCancellationRequested();
				transform.Run(function, context);
				function.CheckInvariant(ILPhase.Normal);
			}
			var statementBuilder = new StatementBuilder(decompilationContext, method);
			var body = statementBuilder.ConvertAsBlock(function.Body);

			entityDecl.AddChild(body, Roles.Body);
		}

		EntityDeclaration DoDecompile(FieldDefinition fieldDefinition, IField field, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == field);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			if (decompilationContext.CurrentTypeDefinition.Kind == TypeKind.Enum) {
				var enumDec = new EnumMemberDeclaration() {
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
				DecompileBody(propertyDefinition.GetMethod, property.Getter, getter, decompilationContext, typeSystemAstBuilder);
			}
			if (property.CanSet && property.Setter.HasBody) {
				DecompileBody(propertyDefinition.SetMethod, property.Setter, setter, decompilationContext, typeSystemAstBuilder);
			}
			var accessor = propertyDefinition.GetMethod ?? propertyDefinition.SetMethod;
			if (!accessor.HasOverrides && !accessor.DeclaringType.IsInterface && accessor.IsVirtual == accessor.IsNewSlot)
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
				DecompileBody(eventDefinition.AddMethod, ev.AddAccessor, ((CustomEventDeclaration)eventDecl).AddAccessor, decompilationContext, typeSystemAstBuilder);
			}
			if (eventDefinition.RemoveMethod != null && eventDefinition.RemoveMethod.HasBody) {
				DecompileBody(eventDefinition.RemoveMethod, ev.RemoveAccessor, ((CustomEventDeclaration)eventDecl).RemoveAccessor, decompilationContext, typeSystemAstBuilder);
			}
			var accessor = eventDefinition.AddMethod ?? eventDefinition.RemoveMethod;
			if (accessor.IsVirtual == accessor.IsNewSlot) {
				SetNewModifier(eventDecl);
			}
			return eventDecl;
		}
	}
}
