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
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.ControlFlow;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Util;
using System.IO;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;
using SRM = System.Reflection.Metadata;

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
		SyntaxTree syntaxTree;

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
				//new YieldReturnDecompiler(), // must run after inlining but before loop detection
				//new AsyncAwaitDecompiler(),  // must run after inlining but before loop detection
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
							new NullPropagationStatementTransform(),
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

		List<IAstTransform> astTransforms = GetAstTransforms();

		public static List<IAstTransform> GetAstTransforms()
		{
			return new List<IAstTransform> {
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
		}

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
			: this(LoadPEFile(fileName, settings), settings)
		{
		}

		public CSharpDecompiler(Metadata.PEFile module, DecompilerSettings settings)
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
		public static bool MemberIsHidden(Metadata.PEFile module, EntityHandle member, DecompilerSettings settings)
		{
			if (module == null || member.IsNil)
				return false;
			var metadata = module.GetMetadataReader();
			string name;
			switch (member.Kind) {
				case HandleKind.MethodDefinition:
					var methodHandle = (MethodDefinitionHandle)member;
					var method = metadata.GetMethodDefinition(methodHandle);
					var methodSemantics = ((MethodDefinitionHandle)member).GetMethodSemanticsAttributes(metadata);
					if (methodSemantics != 0 && methodSemantics != System.Reflection.MethodSemanticsAttributes.Other)
						return true;
					if (settings.AnonymousMethods && methodHandle.HasGeneratedName(metadata) && methodHandle.IsCompilerGenerated(metadata))
						return true;
					/*if (settings.AsyncAwait && AsyncAwaitDecompiler.IsCompilerGeneratedMainMethod(module, (MethodDefinitionHandle)member))
						return true;*/
					return false;
				case HandleKind.TypeDefinition:
					var typeHandle = (TypeDefinitionHandle)member;
					var type = metadata.GetTypeDefinition(typeHandle);
					name = metadata.GetString(type.Name);
					if (!type.GetDeclaringType().IsNil) {
						if (settings.AnonymousMethods && IsClosureType(type, metadata))
							return true;
						/*if (settings.YieldReturn && YieldReturnDecompiler.IsCompilerGeneratorEnumerator(type))
							return true;
						if (settings.AsyncAwait && AsyncAwaitDecompiler.IsCompilerGeneratedStateMachine(type))
							return true;*/
						if (settings.FixedBuffers && name.StartsWith("<", StringComparison.Ordinal) && name.Contains("__FixedBuffer"))
							return true;
					} else if (type.IsCompilerGenerated(metadata)) {
						if (settings.ArrayInitializers && name.StartsWith("<PrivateImplementationDetails>", StringComparison.Ordinal))
							return true;
						if (settings.AnonymousTypes && type.IsAnonymousType(metadata))
							return true;
					}
					return false;
				case HandleKind.FieldDefinition:
					var fieldHandle = (FieldDefinitionHandle)member;
					var field = metadata.GetFieldDefinition(fieldHandle);
					name = metadata.GetString(field.Name);
					if (field.IsCompilerGenerated(metadata)) {
						if (settings.AnonymousMethods && IsAnonymousMethodCacheField(field, metadata))
							return true;
						if (settings.AutomaticProperties && IsAutomaticPropertyBackingField(field, metadata))
							return true;
						if (settings.SwitchStatementOnString && IsSwitchOnStringCache(field, metadata))
							return true;
					}
					// event-fields are not [CompilerGenerated]
					if (settings.AutomaticEvents && metadata.GetTypeDefinition(field.GetDeclaringType()).GetEvents().Any(ev => metadata.GetEventDefinition(ev).Name == field.Name))
						return true;
					// HACK : only hide fields starting with '__StaticArrayInit'
					if (settings.ArrayInitializers && metadata.GetString(metadata.GetTypeDefinition(field.GetDeclaringType()).Name).StartsWith("<PrivateImplementationDetails>", StringComparison.Ordinal)) {
						if (name.StartsWith("__StaticArrayInit", StringComparison.Ordinal))
							return true;
						if (field.DecodeSignature(new Metadata.FullTypeNameSignatureDecoder(metadata), default).ToString().StartsWith("__StaticArrayInit", StringComparison.Ordinal))
							return true;
					}
					return false;
			}

			return false;
		}

		static bool IsSwitchOnStringCache(FieldDefinition field, MetadataReader metadata)
		{
			return metadata.GetString(field.Name).StartsWith("<>f__switch", StringComparison.Ordinal);
		}

		static bool IsAutomaticPropertyBackingField(FieldDefinition field, MetadataReader metadata)
		{
			var name = metadata.GetString(field.Name);
			return name.StartsWith("<", StringComparison.Ordinal) && name.EndsWith("BackingField", StringComparison.Ordinal);
		}

		static bool IsAnonymousMethodCacheField(FieldDefinition field, MetadataReader metadata)
		{
			var name = metadata.GetString(field.Name);
			return name.StartsWith("CS$<>", StringComparison.Ordinal) || name.StartsWith("<>f__am", StringComparison.Ordinal);
		}

		static bool IsClosureType(TypeDefinition type, MetadataReader metadata)
		{
			var name = metadata.GetString(type.Name);
			if (!type.Name.IsGeneratedName(metadata) || !type.IsCompilerGenerated(metadata))
				return false;
			if (name.Contains("DisplayClass") || name.Contains("AnonStorey"))
				return true;
			return type.BaseType.GetFullTypeName(metadata).ToString() == "System.Object" && !type.GetInterfaceImplementations().Any();
		}
		#endregion

		static Metadata.PEFile LoadPEFile(string fileName, DecompilerSettings settings)
		{
			Stream stream;
			if (settings.LoadInMemory) {
				using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read)) {
					stream = new MemoryStream();
					fileStream.CopyTo(stream);
				}
			} else {
				stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			}
			return new Metadata.PEFile(fileName, stream, System.Reflection.PortableExecutable.PEStreamOptions.Default);
		}

		TypeSystemAstBuilder CreateAstBuilder(ITypeResolveContext decompilationContext)
		{
			var typeSystemAstBuilder = new TypeSystemAstBuilder();
			typeSystemAstBuilder.ShowAttributes = true;
			typeSystemAstBuilder.AlwaysUseShortTypeNames = true;
			typeSystemAstBuilder.AddResolveResultAnnotations = true;
			return typeSystemAstBuilder;
		}

		void RunTransforms(AstNode rootNode, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
		{
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			var context = new TransformContext(typeSystem, decompileRun, decompilationContext, typeSystemAstBuilder);
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
			var decompileRun = new DecompileRun(settings) {
				CancellationToken = CancellationToken
			};
			syntaxTree = new SyntaxTree();
			DoDecompileModuleAndAssemblyAttributes(decompileRun, decompilationContext, syntaxTree);
			RunTransforms(syntaxTree, decompileRun, decompilationContext);
			return syntaxTree;
		}

		/// <summary>
		/// Decompile assembly and module attributes.
		/// </summary>
		public string DecompileModuleAndAssemblyAttributesToString()
		{
			return SyntaxTreeToString(DecompileModuleAndAssemblyAttributes());
		}

		void DoDecompileModuleAndAssemblyAttributes(DecompileRun decompileRun, ITypeResolveContext decompilationContext, SyntaxTree syntaxTree)
		{
			foreach (var a in typeSystem.Compilation.MainAssembly.AssemblyAttributes) {
				decompileRun.Namespaces.Add(a.AttributeType.Namespace);
				if (a.AttributeType.FullName == typeof(System.Runtime.CompilerServices.TypeForwardedToAttribute).FullName) {
					decompileRun.Namespaces.Add(((TypeOfResolveResult)a.PositionalArguments[0]).ReferencedType.Namespace);
				} else {
					decompileRun.Namespaces.AddRange(a.PositionalArguments.Select(pa => pa.Type.Namespace));
					decompileRun.Namespaces.AddRange(a.NamedArguments.Select(na => na.Value.Type.Namespace));
				}
				var astBuilder = CreateAstBuilder(decompilationContext);
				var attrSection = new AttributeSection(astBuilder.ConvertAttribute(a));
				attrSection.AttributeTarget = "assembly";
				syntaxTree.AddChild(attrSection, SyntaxTree.MemberRole);
			}
		}

		void DoDecompileTypes(IEnumerable<TypeDefinitionHandle> types, DecompileRun decompileRun, ITypeResolveContext decompilationContext, SyntaxTree syntaxTree)
		{
			string currentNamespace = null;
			AstNode groupNode = null;
			foreach (var cecilType in types) {
				var typeDef = typeSystem.ResolveAsType(cecilType).GetDefinition();
				if (typeDef.Name == "<Module>" && typeDef.Members.Count == 0)
					continue;
				if (MemberIsHidden(typeSystem.ModuleDefinition, cecilType, settings))
					continue;
				if(string.IsNullOrEmpty(typeDef.Namespace)) {
					groupNode = syntaxTree;
				} else {
					if (currentNamespace != typeDef.Namespace) {
						groupNode = new NamespaceDeclaration(typeDef.Namespace);
						syntaxTree.AddChild(groupNode, SyntaxTree.MemberRole);
					}
				}
				currentNamespace = typeDef.Namespace;
				var typeDecl = DoDecompile(typeDef, decompileRun, decompilationContext.WithCurrentTypeDefinition(typeDef));
				groupNode.AddChild(typeDecl, SyntaxTree.MemberRole);
			}
		}

		/// <summary>
		/// Decompiles the whole module into a single syntax tree.
		/// </summary>
		public SyntaxTree DecompileWholeModuleAsSingleFile()
		{
			var decompilationContext = new SimpleTypeResolveContext(typeSystem.MainAssembly);
			var decompileRun = new DecompileRun(settings) {
				CancellationToken = CancellationToken
			};
			syntaxTree = new SyntaxTree();
			MetadataReader metadata = typeSystem.ModuleDefinition.GetMetadataReader();
			RequiredNamespaceCollector.CollectNamespaces(metadata, decompileRun.Namespaces);
			DoDecompileModuleAndAssemblyAttributes(decompileRun, decompilationContext, syntaxTree);
			DoDecompileTypes(metadata.TypeDefinitions, decompileRun, decompilationContext, syntaxTree);
			RunTransforms(syntaxTree, decompileRun, decompilationContext);
			return syntaxTree;
		}

		public ILTransformContext CreateILTransformContext(ILFunction function)
		{
			var decompileRun = new DecompileRun(settings) { CancellationToken = CancellationToken };
			RequiredNamespaceCollector.CollectNamespaces((MethodDefinitionHandle)function.Method.MetadataToken, typeSystem.ModuleDefinition.Reader, decompileRun.Namespaces);
			return new ILTransformContext(function, typeSystem, settings) {
				CancellationToken = CancellationToken,
				DecompileRun = decompileRun
			};
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
		public SyntaxTree DecompileTypes(IEnumerable<TypeDefinitionHandle> types)
		{
			if (types == null)
				throw new ArgumentNullException(nameof(types));
			var decompilationContext = new SimpleTypeResolveContext(typeSystem.MainAssembly);
			var decompileRun = new DecompileRun(settings) {
				CancellationToken = CancellationToken
			};
			syntaxTree = new SyntaxTree();

			foreach (var type in types)
				RequiredNamespaceCollector.CollectNamespaces(type, typeSystem.ModuleDefinition.Reader, decompileRun.Namespaces);
			DoDecompileTypes(types, decompileRun, decompilationContext, syntaxTree);
			RunTransforms(syntaxTree, decompileRun, decompilationContext);
			return syntaxTree;
		}

		/// <summary>
		/// Decompile the given types.
		/// </summary>
		/// <remarks>
		/// Unlike Decompile(IMemberDefinition[]), this method will add namespace declarations around the type definitions.
		/// </remarks>
		public string DecompileTypesAsString(IEnumerable<TypeDefinitionHandle> types)
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
			var decompileRun = new DecompileRun(settings) {
				CancellationToken = CancellationToken
			};
			syntaxTree = new SyntaxTree();
			RequiredNamespaceCollector.CollectNamespaces((TypeDefinitionHandle)type.MetadataToken, typeSystem.ModuleDefinition.Reader, decompileRun.Namespaces);
			DoDecompileTypes(new[] { (TypeDefinitionHandle)type.MetadataToken }, decompileRun, decompilationContext, syntaxTree);
			RunTransforms(syntaxTree, decompileRun, decompilationContext);
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
		public SyntaxTree Decompile(params EntityHandle[] definitions)
		{
			return Decompile((IList<EntityHandle>)definitions);
		}

		/// <summary>
		/// Decompile the specified types and/or members.
		/// </summary>
		public SyntaxTree Decompile(IList<EntityHandle> definitions)
		{
			if (definitions == null)
				throw new ArgumentNullException(nameof(definitions));
			ITypeDefinition parentTypeDef = null;
			syntaxTree = new SyntaxTree();
			var decompileRun = new DecompileRun(settings) { CancellationToken = CancellationToken };
			foreach (var entity in definitions)
				RequiredNamespaceCollector.CollectNamespaces(entity, typeSystem.ModuleDefinition.Reader, decompileRun.Namespaces);
			foreach (var entity in definitions) {
				if (entity.IsNil)
					throw new ArgumentException("definitions contains null element");
				switch (entity.Kind) {
					case HandleKind.TypeDefinition:
						ITypeDefinition typeDef = typeSystem.ResolveAsType(entity).GetDefinition();
						if (typeDef == null)
							throw new InvalidOperationException("Could not find type definition in NR type system");
						syntaxTree.Members.Add(DoDecompile(typeDef, decompileRun, new SimpleTypeResolveContext(typeDef)));
						parentTypeDef = typeDef.DeclaringTypeDefinition;
						break;
					case HandleKind.MethodDefinition:
						IMethod method = typeSystem.ResolveAsMethod(entity);
						if (method == null)
							throw new InvalidOperationException("Could not find method definition in NR type system");
						syntaxTree.Members.Add(DoDecompile(method, decompileRun, new SimpleTypeResolveContext(method)));
						parentTypeDef = method.DeclaringTypeDefinition;
						break;
					case HandleKind.FieldDefinition:
						IField field = typeSystem.ResolveAsField(entity);
						if (field == null)
							throw new InvalidOperationException("Could not find field definition in NR type system");
						syntaxTree.Members.Add(DoDecompile(field, decompileRun, new SimpleTypeResolveContext(field)));
						parentTypeDef = field.DeclaringTypeDefinition;
						break;
					case HandleKind.PropertyDefinition:
						IProperty property = typeSystem.ResolveAsProperty(entity);
						if (property == null)
							throw new InvalidOperationException("Could not find property definition in NR type system");
						syntaxTree.Members.Add(DoDecompile(property, decompileRun, new SimpleTypeResolveContext(property)));
						parentTypeDef = property.DeclaringTypeDefinition;
						break;
					case HandleKind.EventDefinition:
						IEvent ev = typeSystem.ResolveAsEvent(entity);
						if (ev == null)
							throw new InvalidOperationException("Could not find event definition in NR type system");
						syntaxTree.Members.Add(DoDecompile(ev, decompileRun, new SimpleTypeResolveContext(ev)));
						parentTypeDef = ev.DeclaringTypeDefinition;
						break;
					default:
						throw new NotSupportedException(entity.Kind.ToString());
				}
			}
			RunTransforms(syntaxTree, decompileRun, parentTypeDef != null ? new SimpleTypeResolveContext(parentTypeDef) : new SimpleTypeResolveContext(typeSystem.MainAssembly));
			return syntaxTree;
		}

		/// <summary>
		/// Decompile the specified types and/or members.
		/// </summary>
		public string DecompileAsString(params EntityHandle[] definitions)
		{
			return SyntaxTreeToString(Decompile(definitions));
		}

		/// <summary>
		/// Decompile the specified types and/or members.
		/// </summary>
		public string DecompileAsString(IList<EntityHandle> definitions)
		{
			return SyntaxTreeToString(Decompile(definitions));
		}

		IEnumerable<EntityDeclaration> AddInterfaceImplHelpers(EntityDeclaration memberDecl, MethodDefinitionHandle methodHandle,
		                                                       TypeSystemAstBuilder astBuilder)
		{
			if (!memberDecl.GetChildByRole(EntityDeclaration.PrivateImplementationTypeRole).IsNull) {
				yield break; // cannot create forwarder for existing explicit interface impl
			}
			MetadataReader metadata = typeSystem.ModuleDefinition.GetMetadataReader();
			foreach (var h in methodHandle.GetMethodImplementations(metadata)) {
				var mi = metadata.GetMethodImplementation(h);
				IMethod m = typeSystem.ResolveAsMethod(mi.MethodDeclaration);
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

		EntityDeclaration DoDecompile(ITypeDefinition typeDef, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
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
				if (!type.MetadataToken.IsNil && !MemberIsHidden(typeSystem.ModuleDefinition, type.MetadataToken, settings)) {
					var nestedType = DoDecompile(type, decompileRun, decompilationContext.WithCurrentTypeDefinition(type));
					SetNewModifier(nestedType);
					typeDecl.Members.Add(nestedType);
				}
			}
			foreach (var field in typeDef.Fields) {
				if (!field.MetadataToken.IsNil && !MemberIsHidden(typeSystem.ModuleDefinition, field.MetadataToken, settings)) {
					var memberDecl = DoDecompile(field, decompileRun, decompilationContext.WithCurrentMember(field));
					typeDecl.Members.Add(memberDecl);
				}
			}
			foreach (var property in typeDef.Properties) {
				if (!property.MetadataToken.IsNil && !MemberIsHidden(typeSystem.ModuleDefinition, property.MetadataToken, settings)) {
					var propDecl = DoDecompile(property, decompileRun, decompilationContext.WithCurrentMember(property));
					typeDecl.Members.Add(propDecl);
				}
			}
			foreach (var @event in typeDef.Events) {
				if (!@event.MetadataToken.IsNil && !MemberIsHidden(typeSystem.ModuleDefinition, @event.MetadataToken, settings)) {
					var eventDecl = DoDecompile(@event, decompileRun, decompilationContext.WithCurrentMember(@event));
					typeDecl.Members.Add(eventDecl);
				}
			}
			foreach (var method in typeDef.Methods) {
				if (!method.MetadataToken.IsNil && !MemberIsHidden(typeSystem.ModuleDefinition, method.MetadataToken, settings)) {
					var memberDecl = DoDecompile(method, decompileRun, decompilationContext.WithCurrentMember(method));
					typeDecl.Members.Add(memberDecl);
					typeDecl.Members.AddRange(AddInterfaceImplHelpers(memberDecl, (MethodDefinitionHandle)method.MetadataToken, typeSystemAstBuilder));
				}
			}
			if (typeDecl.Members.OfType<IndexerDeclaration>().Any(idx => idx.PrivateImplementationType.IsNull)) {
				// Remove the [DefaultMember] attribute if the class contains indexers
				RemoveAttribute(typeDecl, new TopLevelTypeName("System.Reflection", "DefaultMemberAttribute"));
			}
			if (settings.IntroduceRefAndReadonlyModifiersOnStructs && typeDecl.ClassType == ClassType.Struct) {
				if (RemoveAttribute(typeDecl, new TopLevelTypeName("System.Runtime.CompilerServices", "IsByRefLikeAttribute"))) {
					typeDecl.Modifiers |= Modifiers.Ref;
				}
				if (RemoveAttribute(typeDecl, new TopLevelTypeName("System.Runtime.CompilerServices", "IsReadOnlyAttribute"))) {
					typeDecl.Modifiers |= Modifiers.Readonly;
				}
				if (FindAttribute(typeDecl, new TopLevelTypeName("System", "ObsoleteAttribute"), out var attr)) {
					if (obsoleteAttributePattern.IsMatch(attr)) {
						if (attr.Parent is Syntax.AttributeSection section && section.Attributes.Count == 1)
							section.Remove();
						else
							attr.Remove();
					}
				}
			}
			return typeDecl;
		}

		static readonly Syntax.Attribute obsoleteAttributePattern = new Syntax.Attribute() {
			Type = new TypePattern(typeof(ObsoleteAttribute)),
			Arguments = {
				new PrimitiveExpression("Types with embedded references are not supported in this version of your compiler."),
				new Choice() { new PrimitiveExpression(true), new PrimitiveExpression(false) }
			}
		};

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

		EntityDeclaration DoDecompile(IMethod method, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == method);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			var methodDecl = typeSystemAstBuilder.ConvertEntity(method);
			int lastDot = method.Name.LastIndexOf('.');
			if (method.IsExplicitInterfaceImplementation && lastDot >= 0) {
				methodDecl.Name = method.Name.Substring(lastDot + 1);
			}
			FixParameterNames(methodDecl);
			var methodDefinition = typeSystem.GetMetadata().GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
			if (methodDefinition.HasBody()) {
				DecompileBody(method, methodDecl, decompileRun, decompilationContext);
			} else if (!method.IsAbstract && method.DeclaringType.Kind != TypeKind.Interface) {
				methodDecl.Modifiers |= Modifiers.Extern;
			}
			if (method.SymbolKind == SymbolKind.Method && !method.IsExplicitInterfaceImplementation && methodDefinition.HasFlag(System.Reflection.MethodAttributes.Virtual) == methodDefinition.HasFlag(System.Reflection.MethodAttributes.NewSlot)) {
				SetNewModifier(methodDecl);
			}
			return methodDecl;
		}

		void DecompileBody(IMethod method, EntityDeclaration entityDecl, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
		{
			try {
				var specializingTypeSystem = typeSystem.GetSpecializingTypeSystem(decompilationContext);
				var ilReader = new ILReader(specializingTypeSystem);
				ilReader.UseDebugSymbols = settings.UseDebugSymbols;
				var methodDef = typeSystem.ModuleDefinition.GetMetadataReader().GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
				var function = ilReader.ReadIL(typeSystem.ModuleDefinition, (MethodDefinitionHandle)method.MetadataToken, typeSystem.ModuleDefinition.Reader.GetMethodBody(methodDef.RelativeVirtualAddress), CancellationToken);
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
					CancellationToken = CancellationToken,
					DecompileRun = decompileRun
				};
				foreach (var transform in ilTransforms) {
					CancellationToken.ThrowIfCancellationRequested();
					transform.Run(function, context);
					function.CheckInvariant(ILPhase.Normal);
					// When decompiling definitions only, we can cancel decompilation of all steps
					// after yield and async detection, because only those are needed to properly set
					// IsAsync/IsIterator flags on ILFunction.
					//if (!settings.DecompileMemberBodies && transform is AsyncAwaitDecompiler)
					//	break;
				}

				var body = BlockStatement.Null;
				// Generate C# AST only if bodies should be displayed.
				if (settings.DecompileMemberBodies) {
					AddDefinesForConditionalAttributes(function, decompileRun, decompilationContext);
					var statementBuilder = new StatementBuilder(specializingTypeSystem, decompilationContext, function, settings, CancellationToken);
					body = statementBuilder.ConvertAsBlock(function.Body);

					Comment prev = null;
					foreach (string warning in function.Warnings) {
						body.InsertChildAfter(prev, prev = new Comment(warning), Roles.Comment);
					}

					entityDecl.AddChild(body, Roles.Body);
				}
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
				throw new DecompilerException(typeSystem.ModuleDefinition, (MethodDefinitionHandle)method.MetadataToken, innerException);
			}
		}

		bool RemoveAttribute(EntityDeclaration entityDecl, FullTypeName attrName)
		{
			bool found = false;
			foreach (var section in entityDecl.Attributes) {
				foreach (var attr in section.Attributes) {
					var symbol = attr.Type.GetSymbol();
					if (symbol is ITypeDefinition td && td.FullTypeName == attrName) {
						attr.Remove();
						found = true;
					}
				}
				if (section.Attributes.Count == 0) {
					section.Remove();
				}
			}
			return found;
		}

		bool FindAttribute(EntityDeclaration entityDecl, FullTypeName attrName, out Syntax.Attribute attribute)
		{
			attribute = null;
			foreach (var section in entityDecl.Attributes) {
				foreach (var attr in section.Attributes) {
					var symbol = attr.Type.GetSymbol();
					if (symbol is ITypeDefinition td && td.FullTypeName == attrName) {
						attribute = attr;
						return true;
					}
				}
			}
			return false;
		}

		void AddDefinesForConditionalAttributes(ILFunction function, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
		{
			foreach (var call in function.Descendants.OfType<CallInstruction>()) {
				var attr = call.Method.GetAttribute(new TopLevelTypeName("System.Diagnostics", nameof(ConditionalAttribute)));
				var symbolName = attr?.PositionalArguments.FirstOrDefault()?.ConstantValue as string;
				if (symbolName == null || !decompileRun.DefinedSymbols.Add(symbolName))
					continue;
				syntaxTree.InsertChildAfter(null, new PreProcessorDirective(PreProcessorDirectiveType.Define, symbolName), Roles.PreProcessorDirective);
			}
		}

		EntityDeclaration DoDecompile(IField field, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
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
					if (enumDec.Initializer is PrimitiveExpression primitive)
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
			typeSystemAstBuilder.UseSpecialConstants = !field.DeclaringType.Equals(field.ReturnType);
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
			var fieldDefinition = typeSystem.GetMetadata().GetFieldDefinition((FieldDefinitionHandle)field.MetadataToken);
			if (fieldDefinition.HasFlag(System.Reflection.FieldAttributes.HasFieldRVA)) {
				// Field data as specified in II.16.3.2 of ECMA-335 6th edition:
				// .data I_X = int32(123)
				// .field public static int32 _x at I_X
				var message = string.Format(" Not supported: data({0}) ", BitConverter.ToString(fieldDefinition.GetInitialValue(typeSystem.ModuleDefinition.Reader)).Replace('-', ' '));
				((FieldDeclaration)fieldDecl).Variables.Single().AddChild(new Comment(message, CommentType.MultiLine), Roles.Comment);
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

		EntityDeclaration DoDecompile(IProperty property, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
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
				DecompileBody(property.Getter, getter, decompileRun, decompilationContext);
			}
			if (property.CanSet && property.Setter.HasBody) {
				DecompileBody(property.Setter, setter, decompileRun, decompilationContext);
			}
			var metadata = typeSystem.GetMetadata();
			var accessorHandle = (MethodDefinitionHandle)(property.Getter ?? property.Setter).MetadataToken;
			var accessor = metadata.GetMethodDefinition(accessorHandle);
			if (!accessorHandle.GetMethodImplementations(metadata).Any() && accessor.HasFlag(System.Reflection.MethodAttributes.Virtual) == accessor.HasFlag(System.Reflection.MethodAttributes.NewSlot))
				SetNewModifier(propertyDecl);
			return propertyDecl;
		}

		EntityDeclaration DoDecompile(IEvent ev, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == ev);
			var typeSystemAstBuilder = CreateAstBuilder(decompilationContext);
			typeSystemAstBuilder.UseCustomEvents = ev.DeclaringTypeDefinition.Kind != TypeKind.Interface;
			var eventDecl = typeSystemAstBuilder.ConvertEntity(ev);
			int lastDot = ev.Name.LastIndexOf('.');
			if (ev.IsExplicitInterfaceImplementation) {
				eventDecl.Name = ev.Name.Substring(lastDot + 1);
			}
			var metadata = typeSystem.ModuleDefinition.GetMetadataReader();
			if (ev.CanAdd && ev.AddAccessor.HasBody) {
				DecompileBody(ev.AddAccessor, ((CustomEventDeclaration)eventDecl).AddAccessor, decompileRun, decompilationContext);
			}
			if (ev.CanRemove && ev.RemoveAccessor.HasBody) {
				DecompileBody(ev.RemoveAccessor, ((CustomEventDeclaration)eventDecl).RemoveAccessor, decompileRun, decompilationContext);
			}
			var accessor = metadata.GetMethodDefinition((MethodDefinitionHandle)(ev.AddAccessor ?? ev.RemoveAccessor).MetadataToken);
			if (accessor.HasFlag(System.Reflection.MethodAttributes.Virtual) == accessor.HasFlag(System.Reflection.MethodAttributes.NewSlot)) {
				SetNewModifier(eventDecl);
			}
			return eventDecl;
		}

		#region Sequence Points
		/// <summary>
		/// Creates sequence points for the given syntax tree.
		/// 
		/// This only works correctly when the nodes in the syntax tree have line/column information.
		/// </summary>
		public Dictionary<ILFunction, List<Metadata.SequencePoint>> CreateSequencePoints(SyntaxTree syntaxTree)
		{
			SequencePointBuilder spb = new SequencePointBuilder();
			syntaxTree.AcceptVisitor(spb);
			return spb.GetSequencePoints();
	}
		#endregion
	}
}
