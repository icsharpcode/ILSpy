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
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using Mono.Cecil;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.ControlFlow;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Util;
using System.IO;

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
		private SyntaxTree syntaxTree;

		List<IILTransform> ilTransforms = GetILTransforms();

		/// <summary>
		/// Pre-yield/await transforms.
		/// </summary>
		internal static List<IILTransform> EarlyILTransforms(bool aggressivelyDuplicateReturnBlocks = false)
		{
			return new List<IILTransform> {
				new ControlFlowSimplification {
					aggressivelyDuplicateReturnBlocks = aggressivelyDuplicateReturnBlocks
				},
				new SplitVariables(),
				new ILInlining(),
			};
		}

		public static List<IILTransform> GetILTransforms()
		{
			return new List<IILTransform> {
				new ControlFlowSimplification(),
				// Run SplitVariables only after ControlFlowSimplification duplicates return blocks,
				// so that the return variable is split and can be inlined.
				new SplitVariables(),
				new ILInlining(),
				new DetectPinnedRegions(), // must run after inlining but before non-critical control flow transforms
				new InlineReturnTransform(),
				new YieldReturnDecompiler(), // must run after inlining but before loop detection
				new AsyncAwaitDecompiler(),  // must run after inlining but before loop detection
				new DetectCatchWhenConditionBlocks(), // must run after inlining but before loop detection
				new DetectExitPoints(canIntroduceExitForReturn: false),
				new EarlyExpressionTransforms(),
				// RemoveDeadVariableInit must run after EarlyExpressionTransforms so that stobj(ldloca V, ...)
				// is already collapsed into stloc(V, ...).
				new RemoveDeadVariableInit(),
				new SplitVariables(), // split variables once again, because the stobj(ldloca V, ...) may open up new replacements
				new SwitchDetection(),
				new SwitchOnStringTransform(),
				new SwitchOnNullableTransform(),
				new SplitVariables(), // split variables once again, because SwitchOnNullableTransform eliminates ldloca 
				new BlockILTransform { // per-block transforms
					PostOrderTransforms = {
						// Even though it's a post-order block-transform as most other transforms,
						// let's keep LoopDetection separate for now until there's a compelling
						// reason to combine it with the other block transforms.
						// If we ran loop detection after some if structures are already detected,
						// we might make our life introducing good exit points more difficult.
						new LoopDetection()
					}
				},
				// re-run DetectExitPoints after loop detection
				new DetectExitPoints(canIntroduceExitForReturn: true),
				new BlockILTransform { // per-block transforms
					PostOrderTransforms = {
						//new UseExitPoints(),
						new ConditionDetection(),
						new LockTransform(),
						new UsingTransform(),
						// CachedDelegateInitialization must run after ConditionDetection and before/in LoopingBlockTransform
						// and must run before NullCoalescingTransform
						new CachedDelegateInitialization(),
						// Run the assignment transform both before and after copy propagation.
						// Before is necessary because inline assignments of constants are otherwise
						// copy-propated (turned into two separate assignments of the constant).
						// After is necessary because the assigned value might involve null coalescing/etc.
						new StatementTransform(new ILInlining(), new TransformAssignment()),
						new CopyPropagation(),
						new StatementTransform(
							// per-block transforms that depend on each other, and thus need to
							// run interleaved (statement by statement).
							// Pretty much all transforms that open up new expression inlining
							// opportunities belong in this category.
							new ILInlining(),
							// Inlining must be first, because it doesn't trigger re-runs.
							// Any other transform that opens up new inlining opportunities should call RequestRerun().
							new ExpressionTransforms(),
							new TransformAssignment(), // inline and compound assignments
							new NullCoalescingTransform(),
							new NullableLiftingStatementTransform(),
							new TransformArrayInitializers(),
							new TransformCollectionAndObjectInitializers(),
							new TransformExpressionTrees()
						),
					}
				},
				new ProxyCallReplacer(),
				new DelegateConstruction(),
				new HighLevelLoopTransform(),
				new AssignVariableNames(),
			};
		}

		List<IAstTransform> astTransforms = new List<IAstTransform> {
			new PatternStatementTransform(),
			new ReplaceMethodCallsWithOperators(), // must run before DeclareVariables.EnsureExpressionStatementsAreValid
			new IntroduceUnsafeModifier(),
			new AddCheckedBlocks(),
			new DeclareVariables(), // should run after most transforms that modify statements
			new ConvertConstructorCallIntoInitializer(), // must run after DeclareVariables
			new DecimalConstantTransform(),
			new PrettifyAssignments(), // must run after DeclareVariables
			new IntroduceUsingDeclarations(),
			new IntroduceExtensionMethods(), // must run after IntroduceUsingDeclarations
			new IntroduceQueryExpressions(), // must run after IntroduceExtensionMethods
			new CombineQueryExpressions(),
			new NormalizeBlockStatements(),
			new FlattenSwitchBlocks(),
			new FixNameCollisions(),
			new AddXmlDocumentationTransform(),
		};

		public CancellationToken CancellationToken { get; set; }

		public IDecompilerTypeSystem TypeSystem => typeSystem;

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

		public CSharpDecompiler(string fileName, DecompilerSettings settings)
			: this(UniversalAssemblyResolver.LoadMainModule(fileName, settings.ThrowOnAssemblyResolveErrors, settings.LoadInMemory), settings)
		{
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
					if (settings.YieldReturn && YieldReturnDecompiler.IsCompilerGeneratorEnumerator(type))
						return true;
					if (settings.AsyncAwait && AsyncAwaitDecompiler.IsCompilerGeneratedStateMachine(type))
						return true;
					if (settings.FixedBuffers && type.Name.StartsWith("<", StringComparison.Ordinal) && type.Name.Contains("__FixedBuffer"))
						return true;
				} else if (type.IsCompilerGenerated()) {
					if (type.Name.StartsWith("<PrivateImplementationDetails>", StringComparison.Ordinal))
						return true;
					if (settings.AnonymousTypes && type.IsAnonymousType())
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
					if (settings.SwitchStatementOnString && IsSwitchOnStringCache(field))
						return true;
				}
				// event-fields are not [CompilerGenerated]
				if (settings.AutomaticEvents && field.DeclaringType.Events.Any(ev => ev.Name == field.Name))
					return true;
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

		string SyntaxTreeToString(SyntaxTree syntaxTree)
		{
			StringWriter w = new StringWriter();
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(w, settings.CSharpFormattingOptions));
			return w.ToString();
		}

		/// <summary>
		/// Decompile assembly and module attributes.
		/// </summary>
		public SyntaxTree DecompileModuleAndAssemblyAttributes()
		{
			var decompilationContext = new SimpleTypeResolveContext(typeSystem.MainAssembly);
			syntaxTree = new SyntaxTree();
			definedSymbols = new HashSet<string>();
			DoDecompileModuleAndAssemblyAttributes(decompilationContext, syntaxTree);
			RunTransforms(syntaxTree, decompilationContext);
			return syntaxTree;
		}

		/// <summary>
		/// Decompile assembly and module attributes.
		/// </summary>
		public string DecompileModuleAndAssemblyAttributesToString()
		{
			return SyntaxTreeToString(DecompileModuleAndAssemblyAttributes());
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
			syntaxTree = new SyntaxTree();
			definedSymbols = new HashSet<string>();
			DoDecompileModuleAndAssemblyAttributes(decompilationContext, syntaxTree);
			DoDecompileTypes(typeSystem.ModuleDefinition.Types, decompilationContext, syntaxTree);
			RunTransforms(syntaxTree, decompilationContext);
			return syntaxTree;
		}

		/// <summary>
		/// Decompiles the whole module into a single string.
		/// </summary>
		public string DecompileWholeModuleAsString()
		{
			return SyntaxTreeToString(DecompileWholeModuleAsSingleFile());
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
			syntaxTree = new SyntaxTree();
			definedSymbols = new HashSet<string>();
			DoDecompileTypes(types, decompilationContext, syntaxTree);
			RunTransforms(syntaxTree, decompilationContext);
			return syntaxTree;
		}

		/// <summary>
		/// Decompile the given types.
		/// </summary>
		/// <remarks>
		/// Unlike Decompile(IMemberDefinition[]), this method will add namespace declarations around the type definitions.
		/// </remarks>
		public string DecompileTypesAsString(IEnumerable<TypeDefinition> types)
		{
			return SyntaxTreeToString(DecompileTypes(types));
		}

		/// <summary>
		/// Decompile the given type.
		/// </summary>
		/// <remarks>
		/// Unlike Decompile(IMemberDefinition[]), this method will add namespace declarations around the type definition.
		/// </remarks>
		public SyntaxTree DecompileType(FullTypeName fullTypeName)
		{
			var type = typeSystem.Compilation.FindType(fullTypeName.TopLevelTypeName).GetDefinition();
			if (type == null)
				throw new InvalidOperationException($"Could not find type definition {fullTypeName} in type system.");
			var decompilationContext = new SimpleTypeResolveContext(typeSystem.MainAssembly);
			syntaxTree = new SyntaxTree();
			definedSymbols = new HashSet<string>();
			DoDecompileTypes(new[] { typeSystem.GetCecil(type) }, decompilationContext, syntaxTree);
			RunTransforms(syntaxTree, decompilationContext);
			return syntaxTree;
		}

		/// <summary>
		/// Decompile the given type.
		/// </summary>
		/// <remarks>
		/// Unlike Decompile(IMemberDefinition[]), this method will add namespace declarations around the type definition.
		/// </remarks>
		public string DecompileTypeAsString(FullTypeName fullTypeName)
		{
			return SyntaxTreeToString(DecompileType(fullTypeName));
		}

		/// <summary>
		/// Decompile the specified types and/or members.
		/// </summary>
		public SyntaxTree Decompile(params IMemberDefinition[] definitions)
		{
			return Decompile((IList<IMemberDefinition>)definitions);
		}

		/// <summary>
		/// Decompile the specified types and/or members.
		/// </summary>
		public SyntaxTree Decompile(IList<IMemberDefinition> definitions)
		{
			if (definitions == null)
				throw new ArgumentNullException(nameof(definitions));
			ITypeDefinition parentTypeDef = null;
			syntaxTree = new SyntaxTree();
			definedSymbols = new HashSet<string>();
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

		/// <summary>
		/// Decompile the specified types and/or members.
		/// </summary>
		public string DecompileAsString(params IMemberDefinition[] definitions)
		{
			return SyntaxTreeToString(Decompile(definitions));
		}

		/// <summary>
		/// Decompile the specified types and/or members.
		/// </summary>
		public string DecompileAsString(IList<IMemberDefinition> definitions)
		{
			return SyntaxTreeToString(Decompile(definitions));
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
				var forwardingCall = new InvocationExpression(new MemberReferenceExpression(new ThisReferenceExpression(), memberDecl.Name,
					methodDecl.TypeParameters.Select(tp => new SimpleType(tp.Name))),
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
						Left = new MemberReferenceExpression(new TypeReferenceExpression(typeSystemAstBuilder.ConvertType(typeSystem.Compilation.FindType(KnownTypeCode.IntPtr))), "Size"),
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
			try {
				var specializingTypeSystem = typeSystem.GetSpecializingTypeSystem(decompilationContext);
				var ilReader = new ILReader(specializingTypeSystem);
				ilReader.UseDebugSymbols = settings.UseDebugSymbols;
				var function = ilReader.ReadIL(methodDefinition.Body, CancellationToken);
				function.CheckInvariant(ILPhase.Normal);

				if (entityDecl != null) {
					int i = 0;
					var parameters = function.Variables.Where(v => v.Kind == VariableKind.Parameter).ToDictionary(v => v.Index);
					foreach (var parameter in entityDecl.GetChildrenByRole(Roles.Parameter)) {
						if (parameters.TryGetValue(i, out var v))
							parameter.AddAnnotation(new ILVariableResolveResult(v, method.Parameters[i].Type));
						i++;
					}
				}

				var context = new ILTransformContext(function, specializingTypeSystem, settings) {
					CancellationToken = CancellationToken
				};
				foreach (var transform in ilTransforms) {
					CancellationToken.ThrowIfCancellationRequested();
					transform.Run(function, context);
					function.CheckInvariant(ILPhase.Normal);
				}

				AddDefinesForConditionalAttributes(function);
				var statementBuilder = new StatementBuilder(specializingTypeSystem, decompilationContext, function, settings, CancellationToken);
				var body = statementBuilder.ConvertAsBlock(function.Body);

				Comment prev = null;
				foreach (string warning in function.Warnings) {
					body.InsertChildAfter(prev, prev = new Comment(warning), Roles.Comment);
				}

				entityDecl.AddChild(body, Roles.Body);
				entityDecl.AddAnnotation(function);

				if (function.IsIterator) {
					if (!body.Descendants.Any(d => d is YieldReturnStatement || d is YieldBreakStatement)) {
						body.Add(new YieldBreakStatement());
					}
					RemoveAttribute(entityDecl, new TopLevelTypeName("System.Runtime.CompilerServices", "IteratorStateMachineAttribute"));
					if (function.StateMachineCompiledWithMono) {
						RemoveAttribute(entityDecl, new TopLevelTypeName("System.Diagnostics", "DebuggerHiddenAttribute"));
					}
				}
				if (function.IsAsync) {
					entityDecl.Modifiers |= Modifiers.Async;
					RemoveAttribute(entityDecl, new TopLevelTypeName("System.Runtime.CompilerServices", "AsyncStateMachineAttribute"));
					RemoveAttribute(entityDecl, new TopLevelTypeName("System.Diagnostics", "DebuggerStepThroughAttribute"));
				}
			} catch (Exception innerException) when (!(innerException is OperationCanceledException)) {
				throw new DecompilerException(methodDefinition, innerException);
			}
		}

		void RemoveAttribute(EntityDeclaration entityDecl, FullTypeName attrName)
		{
			foreach (var section in entityDecl.Attributes) {
				foreach (var attr in section.Attributes) {
					var symbol = attr.Type.GetSymbol();
					if (symbol is ITypeDefinition td && td.FullTypeName == attrName) {
						attr.Remove();
					}
				}
				if (section.Attributes.Count == 0) {
					section.Remove();
				}
			}
		}

		HashSet<string> definedSymbols;

		void AddDefinesForConditionalAttributes(ILFunction function)
		{
			foreach (var call in function.Descendants.OfType<CallInstruction>()) {
				var attr = call.Method.GetAttribute(new TopLevelTypeName("System.Diagnostics", nameof(ConditionalAttribute)));
				var symbolName = attr?.PositionalArguments.FirstOrDefault()?.ConstantValue as string;
				if (symbolName == null || !definedSymbols.Add(symbolName))
					continue;
				syntaxTree.InsertChildAfter(null, new PreProcessorDirective(PreProcessorDirectiveType.Define, symbolName), Roles.PreProcessorDirective);
			}
		}

		EntityDeclaration DoDecompile(FieldDefinition fieldDefinition, IField field, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == field);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			if (decompilationContext.CurrentTypeDefinition.Kind == TypeKind.Enum && field.ConstantValue != null) {
				var index = decompilationContext.CurrentTypeDefinition.Members.IndexOf(field);
				long previousValue = -1;
				if (index > 0) {
					var previousMember = (IField)decompilationContext.CurrentTypeDefinition.Members[index - 1];
					previousValue = (long)CSharpPrimitiveCast.Cast(TypeCode.Int64, previousMember.ConstantValue, false);
				}
				var enumDec = new EnumMemberDeclaration { Name = field.Name };
				long initValue = (long)CSharpPrimitiveCast.Cast(TypeCode.Int64, field.ConstantValue, false);
				if (decompilationContext.CurrentTypeDefinition.Attributes.Any(a => a.AttributeType.FullName == "System.FlagsAttribute")) {
					enumDec.Initializer = typeSystemAstBuilder.ConvertConstantValue(decompilationContext.CurrentTypeDefinition.EnumUnderlyingType, field.ConstantValue);
					if (enumDec.Initializer is PrimitiveExpression primitive && initValue > 9)
						primitive.SetValue(initValue, $"0x{initValue:X}");
				} else if (previousValue + 1 != initValue) {
					enumDec.Initializer = typeSystemAstBuilder.ConvertConstantValue(decompilationContext.CurrentTypeDefinition.EnumUnderlyingType, field.ConstantValue);
					if (enumDec.Initializer is PrimitiveExpression primitive && initValue > 9 && ((initValue & (initValue - 1)) == 0 || (initValue & (initValue + 1)) == 0)) {
						primitive.SetValue(initValue, $"0x{initValue:X}");
					}
				}
				enumDec.Attributes.AddRange(field.Attributes.Select(a => new AttributeSection(typeSystemAstBuilder.ConvertAttribute(a))));
				enumDec.AddAnnotation(new Semantics.MemberResolveResult(null, field));
				return enumDec;
			}
			var fieldDecl = typeSystemAstBuilder.ConvertEntity(field);
			SetNewModifier(fieldDecl);
			if (settings.FixedBuffers && IsFixedField(field, out var elementType, out var elementCount)) {
				var fixedFieldDecl = new FixedFieldDeclaration();
				fieldDecl.Attributes.MoveTo(fixedFieldDecl.Attributes);
				fixedFieldDecl.Modifiers = fieldDecl.Modifiers;
				fixedFieldDecl.ReturnType = typeSystemAstBuilder.ConvertType(elementType);
				fixedFieldDecl.Variables.Add(new FixedVariableInitializer(field.Name, new PrimitiveExpression(elementCount)));
				fixedFieldDecl.Variables.Single().CopyAnnotationsFrom(((FieldDeclaration)fieldDecl).Variables.Single());
				fixedFieldDecl.CopyAnnotationsFrom(fieldDecl);
				RemoveAttribute(fixedFieldDecl, fixedBufferAttributeTypeName);
				return fixedFieldDecl;
			}
			return fieldDecl;
		}

		static readonly FullTypeName fixedBufferAttributeTypeName = new TopLevelTypeName("System.Runtime.CompilerServices", "FixedBufferAttribute");

		internal static bool IsFixedField(IField field, out IType type, out int elementCount)
		{
			type = null;
			elementCount = 0;
			IAttribute attr = field.GetAttribute(fixedBufferAttributeTypeName, inherit: false);
			if (attr != null && attr.PositionalArguments.Count == 2) {
				if (attr.PositionalArguments[0] is TypeOfResolveResult trr && attr.PositionalArguments[1].ConstantValue is int length) {
					type = trr.ReferencedType;
					elementCount = length;
					return true;
				}
			}
			return false;
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

		#region Convert Type Reference
		/// <summary>
		/// Converts a type reference.
		/// </summary>
		/// <param name="type">The Cecil type reference that should be converted into
		/// a type system type reference.</param>
		/// <param name="typeAttributes">Attributes associated with the Cecil type reference.
		/// This is used to support the 'dynamic' type.</param>
		public static AstType ConvertType(TypeReference type, ICustomAttributeProvider typeAttributes = null, ConvertTypeOptions options = ConvertTypeOptions.None)
		{
			int typeIndex = 0;
			return ConvertType(type, typeAttributes, ref typeIndex, options);
		}

		static AstType ConvertType(TypeReference type, ICustomAttributeProvider typeAttributes, ref int typeIndex, ConvertTypeOptions options)
		{
			while (type is OptionalModifierType || type is RequiredModifierType) {
				type = ((TypeSpecification)type).ElementType;
			}
			if (type == null) {
				return AstType.Null;
			}

			if (type is Mono.Cecil.ByReferenceType) {
				typeIndex++;
				// by reference type cannot be represented in C#; so we'll represent it as a pointer instead
				return ConvertType((type as Mono.Cecil.ByReferenceType).ElementType, typeAttributes, ref typeIndex, options)
					.MakePointerType();
			} else if (type is Mono.Cecil.PointerType) {
				typeIndex++;
				return ConvertType((type as Mono.Cecil.PointerType).ElementType, typeAttributes, ref typeIndex, options)
					.MakePointerType();
			} else if (type is Mono.Cecil.ArrayType) {
				typeIndex++;
				return ConvertType((type as Mono.Cecil.ArrayType).ElementType, typeAttributes, ref typeIndex, options)
					.MakeArrayType((type as Mono.Cecil.ArrayType).Rank);
			} else if (type is GenericInstanceType) {
				GenericInstanceType gType = (GenericInstanceType)type;
				if (gType.ElementType.Namespace == "System" && gType.ElementType.Name == "Nullable`1" && gType.GenericArguments.Count == 1) {
					typeIndex++;
					return new ComposedType {
						BaseType = ConvertType(gType.GenericArguments[0], typeAttributes, ref typeIndex, options),
						HasNullableSpecifier = true
					};
				}
				AstType baseType = ConvertType(gType.ElementType, typeAttributes, ref typeIndex, options & ~ConvertTypeOptions.IncludeTypeParameterDefinitions);
				List<AstType> typeArguments = new List<AstType>();
				foreach (var typeArgument in gType.GenericArguments) {
					typeIndex++;
					typeArguments.Add(ConvertType(typeArgument, typeAttributes, ref typeIndex, options));
				}
				ApplyTypeArgumentsTo(baseType, typeArguments);
				return baseType;
			} else if (type is GenericParameter) {
				return new SimpleType(type.Name);
			} else if (type.IsNested) {
				string namepart = ReflectionHelper.SplitTypeParameterCountFromReflectionName(type.Name);
				AstType memberType;
				if ((options & (ConvertTypeOptions.IncludeOuterTypeName | ConvertTypeOptions.IncludeNamespace)) != 0) {
				AstType typeRef = ConvertType(type.DeclaringType, typeAttributes, ref typeIndex, options & ~ConvertTypeOptions.IncludeTypeParameterDefinitions);
					memberType = new MemberType { Target = typeRef, MemberName = namepart };
				if ((options & ConvertTypeOptions.IncludeTypeParameterDefinitions) == ConvertTypeOptions.IncludeTypeParameterDefinitions) {
					AddTypeParameterDefininitionsTo(type, memberType);
				}
				} else {
					memberType = new SimpleType(namepart);
					if ((options & ConvertTypeOptions.IncludeTypeParameterDefinitions) == ConvertTypeOptions.IncludeTypeParameterDefinitions) {
						if (type.HasGenericParameters) {
							List<AstType> typeArguments = new List<AstType>();
							foreach (GenericParameter gp in type.GenericParameters) {
								typeArguments.Add(new SimpleType(gp.Name));
							}
							ReflectionHelper.SplitTypeParameterCountFromReflectionName(type.Name, out int typeParameterCount);
							if (typeParameterCount > typeArguments.Count)
								typeParameterCount = typeArguments.Count;
							((SimpleType)memberType).TypeArguments.AddRange(typeArguments.GetRange(typeArguments.Count - typeParameterCount, typeParameterCount));
							typeArguments.RemoveRange(typeArguments.Count - typeParameterCount, typeParameterCount);
						}
					}
				}
				memberType.AddAnnotation(type);
				return memberType;
			} else {
				string ns = type.Namespace ?? string.Empty;
				string name = type.Name;
				if (name == null)
					throw new InvalidOperationException("type.Name returned null. Type: " + type.ToString());

				if (name == "Object" && ns == "System" && HasDynamicAttribute(typeAttributes, typeIndex)) {
					return new Syntax.PrimitiveType("dynamic");
				} else {
					if (ns == "System") {
						if ((options & ConvertTypeOptions.DoNotUsePrimitiveTypeNames)
							!= ConvertTypeOptions.DoNotUsePrimitiveTypeNames) {
							switch (name) {
								case "SByte":
									return new Syntax.PrimitiveType("sbyte");
								case "Int16":
									return new Syntax.PrimitiveType("short");
								case "Int32":
									return new Syntax.PrimitiveType("int");
								case "Int64":
									return new Syntax.PrimitiveType("long");
								case "Byte":
									return new Syntax.PrimitiveType("byte");
								case "UInt16":
									return new Syntax.PrimitiveType("ushort");
								case "UInt32":
									return new Syntax.PrimitiveType("uint");
								case "UInt64":
									return new Syntax.PrimitiveType("ulong");
								case "String":
									return new Syntax.PrimitiveType("string");
								case "Single":
									return new Syntax.PrimitiveType("float");
								case "Double":
									return new Syntax.PrimitiveType("double");
								case "Decimal":
									return new Syntax.PrimitiveType("decimal");
								case "Char":
									return new Syntax.PrimitiveType("char");
								case "Boolean":
									return new Syntax.PrimitiveType("bool");
								case "Void":
									return new Syntax.PrimitiveType("void");
								case "Object":
									return new Syntax.PrimitiveType("object");
							}
						}
					}

					name = ReflectionHelper.SplitTypeParameterCountFromReflectionName(name);

					AstType astType;
					if ((options & ConvertTypeOptions.IncludeNamespace) == ConvertTypeOptions.IncludeNamespace && ns.Length > 0) {
						string[] parts = ns.Split('.');
						AstType nsType = new SimpleType(parts[0]);
						for (int i = 1; i < parts.Length; i++) {
							nsType = new MemberType { Target = nsType, MemberName = parts[i] };
						}
						astType = new MemberType { Target = nsType, MemberName = name };
					} else {
						astType = new SimpleType(name);
					}
					astType.AddAnnotation(type);

					if ((options & ConvertTypeOptions.IncludeTypeParameterDefinitions) == ConvertTypeOptions.IncludeTypeParameterDefinitions) {
						AddTypeParameterDefininitionsTo(type, astType);
					}
					return astType;
				}
			}
		}

		static void AddTypeParameterDefininitionsTo(TypeReference type, AstType astType)
		{
			if (type.HasGenericParameters) {
				List<AstType> typeArguments = new List<AstType>();
				foreach (GenericParameter gp in type.GenericParameters) {
					typeArguments.Add(new SimpleType(gp.Name));
				}
				ApplyTypeArgumentsTo(astType, typeArguments);
			}
		}

		static void ApplyTypeArgumentsTo(AstType baseType, List<AstType> typeArguments)
		{
			SimpleType st = baseType as SimpleType;
			if (st != null) {
				TypeReference type = st.Annotation<TypeReference>();
				if (type != null) {
					ReflectionHelper.SplitTypeParameterCountFromReflectionName(type.Name, out int typeParameterCount);
					if (typeParameterCount > typeArguments.Count)
						typeParameterCount = typeArguments.Count;
					st.TypeArguments.AddRange(typeArguments.GetRange(typeArguments.Count - typeParameterCount, typeParameterCount));
				} else {
				st.TypeArguments.AddRange(typeArguments);

			}
			}
			MemberType mt = baseType as MemberType;
			if (mt != null) {
				TypeReference type = mt.Annotation<TypeReference>();
				if (type != null) {
					ReflectionHelper.SplitTypeParameterCountFromReflectionName(type.Name, out int typeParameterCount);
					if (typeParameterCount > typeArguments.Count)
						typeParameterCount = typeArguments.Count;
					mt.TypeArguments.AddRange(typeArguments.GetRange(typeArguments.Count - typeParameterCount, typeParameterCount));
					typeArguments.RemoveRange(typeArguments.Count - typeParameterCount, typeParameterCount);
					if (typeArguments.Count > 0)
						ApplyTypeArgumentsTo(mt.Target, typeArguments);
				} else {
					mt.TypeArguments.AddRange(typeArguments);
				}
			}
		}

		const string DynamicAttributeFullName = "System.Runtime.CompilerServices.DynamicAttribute";

		static bool HasDynamicAttribute(ICustomAttributeProvider attributeProvider, int typeIndex)
		{
			if (attributeProvider == null || !attributeProvider.HasCustomAttributes)
				return false;
			foreach (CustomAttribute a in attributeProvider.CustomAttributes) {
				if (a.Constructor.DeclaringType.FullName == DynamicAttributeFullName) {
					if (a.ConstructorArguments.Count == 1) {
						CustomAttributeArgument[] values = a.ConstructorArguments[0].Value as CustomAttributeArgument[];
						if (values != null && typeIndex < values.Length && values[typeIndex].Value is bool)
							return (bool)values[typeIndex].Value;
					}
					return true;
				}
			}
			return false;
		}
		#endregion

		#region Sequence Points
		/// <summary>
		/// Creates sequence points for the given syntax tree.
		/// 
		/// This only works correctly when the nodes in the syntax tree have line/column information.
		/// </summary>
		public Dictionary<ILFunction, List<SequencePoint>> CreateSequencePoints(SyntaxTree syntaxTree)
		{
			SequencePointBuilder spb = new SequencePointBuilder();
			syntaxTree.AcceptVisitor(spb);
			return spb.GetSequencePoints();
	}
		#endregion
	}

	[Flags]
	public enum ConvertTypeOptions
	{
		None = 0,
		IncludeNamespace = 1,
		IncludeTypeParameterDefinitions = 2,
		DoNotUsePrimitiveTypeNames = 4,
		IncludeOuterTypeName = 8,
	}
}
