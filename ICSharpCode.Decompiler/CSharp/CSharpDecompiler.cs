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
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Resolver;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Syntax.PatternMatching;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.ControlFlow;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;

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
		readonly IDecompilerTypeSystem typeSystem;
		readonly MetadataModule module;
		readonly MetadataReader metadata;
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

		/// <summary>
		/// Returns all built-in transforms of the ILAst pipeline.
		/// </summary>
		public static List<IILTransform> GetILTransforms()
		{
			return new List<IILTransform> {
				new ControlFlowSimplification(),
				// Run SplitVariables only after ControlFlowSimplification duplicates return blocks,
				// so that the return variable is split and can be inlined.
				new SplitVariables(),
				new ILInlining(),
				new InlineReturnTransform(), // must run before DetectPinnedRegions
				new DetectPinnedRegions(), // must run after inlining but before non-critical control flow transforms
				new YieldReturnDecompiler(), // must run after inlining but before loop detection
				new AsyncAwaitDecompiler(),  // must run after inlining but before loop detection
				new DetectCatchWhenConditionBlocks(), // must run after inlining but before loop detection
				new DetectExitPoints(canIntroduceExitForReturn: false),
				new EarlyExpressionTransforms(),
				// RemoveDeadVariableInit must run after EarlyExpressionTransforms so that stobj(ldloca V, ...)
				// is already collapsed into stloc(V, ...).
				new RemoveDeadVariableInit(),
				new SplitVariables(), // split variables once again, because the stobj(ldloca V, ...) may open up new replacements
				new ControlFlowSimplification(), //split variables may enable new branch to leave inlining
				new DynamicCallSiteTransform(),
				new SwitchDetection(),
				new SwitchOnStringTransform(),
				new SwitchOnNullableTransform(),
				new SplitVariables(), // split variables once again, because SwitchOnNullableTransform eliminates ldloca 
				new IntroduceRefReadOnlyModifierOnLocals(),
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
						new ConditionDetection(),
						new LockTransform(),
						new UsingTransform(),
						// CachedDelegateInitialization must run after ConditionDetection and before/in LoopingBlockTransform
						// and must run before NullCoalescingTransform
						new CachedDelegateInitialization(),
						new StatementTransform(
							// per-block transforms that depend on each other, and thus need to
							// run interleaved (statement by statement).
							// Pretty much all transforms that open up new expression inlining
							// opportunities belong in this category.
							new ILInlining(),
							// Inlining must be first, because it doesn't trigger re-runs.
							// Any other transform that opens up new inlining opportunities should call RequestRerun().
							new ExpressionTransforms(),
							new DynamicIsEventAssignmentTransform(),
							new TransformAssignment(), // inline and compound assignments
							new NullCoalescingTransform(),
							new NullableLiftingStatementTransform(),
							new NullPropagationStatementTransform(),
							new TransformArrayInitializers(),
							new TransformCollectionAndObjectInitializers(),
							new TransformExpressionTrees(),
							new IndexRangeTransform(),
							new DeconstructionTransform(),
							new NamedArgumentTransform(),
							new UserDefinedLogicTransform()
						),
					}
				},
				new ProxyCallReplacer(),
				new FixRemainingIncrements(),
				new FixLoneIsInst(),
				new CopyPropagation(),
				new DelegateConstruction(),
				new LocalFunctionDecompiler(),
				new TransformDisplayClassUsage(),
				new HighLevelLoopTransform(),
				new ReduceNestingTransform(),
				new IntroduceDynamicTypeOnLocals(),
				new IntroduceNativeIntTypeOnLocals(),
				new AssignVariableNames(),
			};
		}

		List<IAstTransform> astTransforms = GetAstTransforms();

		/// <summary>
		/// Returns all built-in transforms of the C# AST pipeline.
		/// </summary>
		public static List<IAstTransform> GetAstTransforms()
		{
			return new List<IAstTransform> {
				new PatternStatementTransform(),
				new ReplaceMethodCallsWithOperators(), // must run before DeclareVariables.EnsureExpressionStatementsAreValid
				new IntroduceUnsafeModifier(),
				new AddCheckedBlocks(),
				new DeclareVariables(), // should run after most transforms that modify statements
				new TransformFieldAndConstructorInitializers(), // must run after DeclareVariables
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

		/// <summary>
		/// Token to check for requested cancellation of the decompilation.
		/// </summary>
		public CancellationToken CancellationToken { get; set; }

		/// <summary>
		/// The type system created from the main module and referenced modules.
		/// </summary>
		public IDecompilerTypeSystem TypeSystem => typeSystem;

		/// <summary>
		/// Gets or sets the optional provider for debug info.
		/// </summary>
		public IDebugInfoProvider DebugInfoProvider { get; set; }

		/// <summary>
		/// Gets or sets the optional provider for XML documentation strings.
		/// </summary>
		public IDocumentationProvider DocumentationProvider { get; set; }

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

		/// <summary>
		/// Creates a new <see cref="CSharpDecompiler"/> instance from the given <paramref name="fileName"/> using the given <paramref name="settings"/>.
		/// </summary>
		public CSharpDecompiler(string fileName, DecompilerSettings settings)
			: this(CreateTypeSystemFromFile(fileName, settings), settings)
		{
		}

		/// <summary>
		/// Creates a new <see cref="CSharpDecompiler"/> instance from the given <paramref name="fileName"/> using the given <paramref name="assemblyResolver"/> and <paramref name="settings"/>.
		/// </summary>
		public CSharpDecompiler(string fileName, IAssemblyResolver assemblyResolver, DecompilerSettings settings)
			: this(LoadPEFile(fileName, settings), assemblyResolver, settings)
		{
		}

		/// <summary>
		/// Creates a new <see cref="CSharpDecompiler"/> instance from the given <paramref name="module"/> using the given <paramref name="assemblyResolver"/> and <paramref name="settings"/>.
		/// </summary>
		public CSharpDecompiler(PEFile module, IAssemblyResolver assemblyResolver, DecompilerSettings settings)
			: this(new DecompilerTypeSystem(module, assemblyResolver, settings), settings)
		{
		}

		/// <summary>
		/// Creates a new <see cref="CSharpDecompiler"/> instance from the given <paramref name="typeSystem"/> and the given <paramref name="settings"/>.
		/// </summary>
		public CSharpDecompiler(DecompilerTypeSystem typeSystem, DecompilerSettings settings)
		{
			this.typeSystem = typeSystem ?? throw new ArgumentNullException(nameof(typeSystem));
			this.settings = settings;
			this.module = typeSystem.MainModule;
			this.metadata = module.PEFile.Metadata;
			if (module.TypeSystemOptions.HasFlag(TypeSystemOptions.Uncached))
				throw new ArgumentException("Cannot use an uncached type system in the decompiler.");
		}

		#region MemberIsHidden
		/// <summary>
		/// Determines whether a <paramref name="member"/> should be hidden from the decompiled code. This is used to exclude compiler-generated code that is handled by transforms from the output.
		/// </summary>
		/// <param name="module">The module containing the member.</param>
		/// <param name="member">The metadata token/handle of the member. Can be a TypeDef, MethodDef or FieldDef.</param>
		/// <param name="settings">THe settings used to determine whether code should be hidden. E.g. if async methods are not transformed, async state machines are included in the decompiled code.</param>
		public static bool MemberIsHidden(Metadata.PEFile module, EntityHandle member, DecompilerSettings settings)
		{
			if (module == null || member.IsNil)
				return false;
			var metadata = module.Metadata;
			string name;
			switch (member.Kind)
			{
				case HandleKind.MethodDefinition:
					var methodHandle = (MethodDefinitionHandle)member;
					var method = metadata.GetMethodDefinition(methodHandle);
					var methodSemantics = module.MethodSemanticsLookup.GetSemantics(methodHandle).Item2;
					if (methodSemantics != 0 && methodSemantics != System.Reflection.MethodSemanticsAttributes.Other)
						return true;
					if (settings.LocalFunctions && LocalFunctionDecompiler.IsLocalFunctionMethod(module, methodHandle))
						return true;
					if (settings.AnonymousMethods && methodHandle.HasGeneratedName(metadata) && methodHandle.IsCompilerGenerated(metadata))
						return true;
					if (settings.AsyncAwait && AsyncAwaitDecompiler.IsCompilerGeneratedMainMethod(module, methodHandle))
						return true;
					return false;
				case HandleKind.TypeDefinition:
					var typeHandle = (TypeDefinitionHandle)member;
					var type = metadata.GetTypeDefinition(typeHandle);
					name = metadata.GetString(type.Name);
					if (!type.GetDeclaringType().IsNil)
					{
						if (settings.LocalFunctions && LocalFunctionDecompiler.IsLocalFunctionDisplayClass(module, typeHandle))
							return true;
						if (settings.AnonymousMethods && IsClosureType(type, metadata))
							return true;
						if (settings.YieldReturn && YieldReturnDecompiler.IsCompilerGeneratorEnumerator(typeHandle, metadata))
							return true;
						if (settings.AsyncAwait && AsyncAwaitDecompiler.IsCompilerGeneratedStateMachine(typeHandle, metadata))
							return true;
						if (settings.AsyncEnumerator && AsyncAwaitDecompiler.IsCompilerGeneratorAsyncEnumerator(typeHandle, metadata))
							return true;
						if (settings.FixedBuffers && name.StartsWith("<", StringComparison.Ordinal) && name.Contains("__FixedBuffer"))
							return true;
					}
					else if (type.IsCompilerGenerated(metadata))
					{
						if (settings.ArrayInitializers && name.StartsWith("<PrivateImplementationDetails>", StringComparison.Ordinal))
							return true;
						if (settings.AnonymousTypes && type.IsAnonymousType(metadata))
							return true;
						if (settings.Dynamic && type.IsDelegate(metadata) && (name.StartsWith("<>A", StringComparison.Ordinal) || name.StartsWith("<>F", StringComparison.Ordinal)))
							return true;
					}
					if (settings.ArrayInitializers && settings.SwitchStatementOnString && name.StartsWith("<PrivateImplementationDetails>", StringComparison.Ordinal))
						return true;
					return false;
				case HandleKind.FieldDefinition:
					var fieldHandle = (FieldDefinitionHandle)member;
					var field = metadata.GetFieldDefinition(fieldHandle);
					name = metadata.GetString(field.Name);
					if (field.IsCompilerGenerated(metadata))
					{
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
					if (settings.ArrayInitializers && metadata.GetString(metadata.GetTypeDefinition(field.GetDeclaringType()).Name).StartsWith("<PrivateImplementationDetails>", StringComparison.Ordinal))
					{
						// only hide fields starting with '__StaticArrayInit'
						if (name.StartsWith("__StaticArrayInit", StringComparison.Ordinal))
							return true;
						// hide fields starting with '$$method'
						if (name.StartsWith("$$method", StringComparison.Ordinal))
							return true;
						if (field.DecodeSignature(new Metadata.FullTypeNameSignatureDecoder(metadata), default).ToString().StartsWith("__StaticArrayInit", StringComparison.Ordinal))
							return true;
					}
					return false;
			}

			return false;
		}

		static bool IsSwitchOnStringCache(SRM.FieldDefinition field, MetadataReader metadata)
		{
			return metadata.GetString(field.Name).StartsWith("<>f__switch", StringComparison.Ordinal);
		}

		static bool IsAutomaticPropertyBackingField(SRM.FieldDefinition field, MetadataReader metadata)
		{
			var name = metadata.GetString(field.Name);
			return name.StartsWith("<", StringComparison.Ordinal) && name.EndsWith("BackingField", StringComparison.Ordinal);
		}

		static bool IsAnonymousMethodCacheField(SRM.FieldDefinition field, MetadataReader metadata)
		{
			var name = metadata.GetString(field.Name);
			return name.StartsWith("CS$<>", StringComparison.Ordinal) || name.StartsWith("<>f__am", StringComparison.Ordinal) || name.StartsWith("<>f__mg", StringComparison.Ordinal);
		}

		static bool IsClosureType(SRM.TypeDefinition type, MetadataReader metadata)
		{
			var name = metadata.GetString(type.Name);
			if (!type.Name.IsGeneratedName(metadata) || !type.IsCompilerGenerated(metadata))
				return false;
			if (name.Contains("DisplayClass") || name.Contains("AnonStorey") || name.Contains("Closure$"))
				return true;
			return type.BaseType.IsKnownType(metadata, KnownTypeCode.Object) && !type.GetInterfaceImplementations().Any();
		}
		#endregion

		static PEFile LoadPEFile(string fileName, DecompilerSettings settings)
		{
			settings.LoadInMemory = true;
			return new PEFile(
				fileName,
				new FileStream(fileName, FileMode.Open, FileAccess.Read),
				streamOptions: settings.LoadInMemory ? PEStreamOptions.PrefetchEntireImage : PEStreamOptions.Default,
				metadataOptions: settings.ApplyWindowsRuntimeProjections ? MetadataReaderOptions.ApplyWindowsRuntimeProjections : MetadataReaderOptions.None
			);
		}

		static DecompilerTypeSystem CreateTypeSystemFromFile(string fileName, DecompilerSettings settings)
		{
			settings.LoadInMemory = true;
			var file = LoadPEFile(fileName, settings);
			var resolver = new UniversalAssemblyResolver(fileName, settings.ThrowOnAssemblyResolveErrors,
				file.DetectTargetFrameworkId(),
				settings.LoadInMemory ? PEStreamOptions.PrefetchMetadata : PEStreamOptions.Default,
				settings.ApplyWindowsRuntimeProjections ? MetadataReaderOptions.ApplyWindowsRuntimeProjections : MetadataReaderOptions.None);
			return new DecompilerTypeSystem(file, resolver);
		}

		static TypeSystemAstBuilder CreateAstBuilder(DecompilerSettings settings)
		{
			var typeSystemAstBuilder = new TypeSystemAstBuilder();
			typeSystemAstBuilder.ShowAttributes = true;
			typeSystemAstBuilder.AlwaysUseShortTypeNames = true;
			typeSystemAstBuilder.AddResolveResultAnnotations = true;
			typeSystemAstBuilder.UseNullableSpecifierForValueTypes = settings.LiftNullables;
			typeSystemAstBuilder.SupportInitAccessors = settings.InitAccessors;
			typeSystemAstBuilder.SupportRecordClasses = settings.RecordClasses;
			return typeSystemAstBuilder;
		}

		IDocumentationProvider CreateDefaultDocumentationProvider()
		{
			try
			{
				return XmlDocLoader.LoadDocumentation(module.PEFile);
			}
			catch (System.Xml.XmlException)
			{
				return null;
			}
		}

		void RunTransforms(AstNode rootNode, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
		{
			var typeSystemAstBuilder = CreateAstBuilder(decompileRun.Settings);
			var context = new TransformContext(typeSystem, decompileRun, decompilationContext, typeSystemAstBuilder);
			foreach (var transform in astTransforms)
			{
				CancellationToken.ThrowIfCancellationRequested();
				transform.Run(rootNode, context);
			}
			CancellationToken.ThrowIfCancellationRequested();
			rootNode.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
			CancellationToken.ThrowIfCancellationRequested();
			GenericGrammarAmbiguityVisitor.ResolveAmbiguities(rootNode);
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
			var decompilationContext = new SimpleTypeResolveContext(typeSystem.MainModule);
			var decompileRun = new DecompileRun(settings) {
				DocumentationProvider = DocumentationProvider ?? CreateDefaultDocumentationProvider(),
				CancellationToken = CancellationToken
			};
			syntaxTree = new SyntaxTree();
			RequiredNamespaceCollector.CollectAttributeNamespaces(module, decompileRun.Namespaces);
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
			try
			{
				foreach (var a in typeSystem.MainModule.GetAssemblyAttributes())
				{
					var astBuilder = CreateAstBuilder(decompileRun.Settings);
					var attrSection = new AttributeSection(astBuilder.ConvertAttribute(a));
					attrSection.AttributeTarget = "assembly";
					syntaxTree.AddChild(attrSection, SyntaxTree.MemberRole);
				}
				foreach (var a in typeSystem.MainModule.GetModuleAttributes())
				{
					var astBuilder = CreateAstBuilder(decompileRun.Settings);
					var attrSection = new AttributeSection(astBuilder.ConvertAttribute(a));
					attrSection.AttributeTarget = "module";
					syntaxTree.AddChild(attrSection, SyntaxTree.MemberRole);
				}
			}
			catch (Exception innerException) when (!(innerException is OperationCanceledException || innerException is DecompilerException))
			{
				throw new DecompilerException(module, null, innerException, "Error decompiling module and assembly attributes of " + module.AssemblyName);
			}
		}

		void DoDecompileTypes(IEnumerable<TypeDefinitionHandle> types, DecompileRun decompileRun, ITypeResolveContext decompilationContext, SyntaxTree syntaxTree)
		{
			string currentNamespace = null;
			AstNode groupNode = null;
			foreach (var typeDefHandle in types)
			{
				var typeDef = module.GetDefinition(typeDefHandle);
				if (typeDef.Name == "<Module>" && typeDef.Members.Count == 0)
					continue;
				if (MemberIsHidden(module.PEFile, typeDefHandle, settings))
					continue;
				if (string.IsNullOrEmpty(typeDef.Namespace))
				{
					groupNode = syntaxTree;
				}
				else
				{
					if (currentNamespace != typeDef.Namespace)
					{
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
			return DecompileWholeModuleAsSingleFile(false);
		}

		/// <summary>
		/// Decompiles the whole module into a single syntax tree.
		/// </summary>
		/// <param name="sortTypes">If true, top-level-types are emitted sorted by namespace/name.
		/// If false, types are emitted in metadata order.</param>
		public SyntaxTree DecompileWholeModuleAsSingleFile(bool sortTypes)
		{
			var decompilationContext = new SimpleTypeResolveContext(typeSystem.MainModule);
			var decompileRun = new DecompileRun(settings) {
				DocumentationProvider = DocumentationProvider ?? CreateDefaultDocumentationProvider(),
				CancellationToken = CancellationToken
			};
			syntaxTree = new SyntaxTree();
			RequiredNamespaceCollector.CollectNamespaces(module, decompileRun.Namespaces);
			DoDecompileModuleAndAssemblyAttributes(decompileRun, decompilationContext, syntaxTree);
			var typeDefs = metadata.GetTopLevelTypeDefinitions();
			if (sortTypes)
			{
				typeDefs = typeDefs.OrderBy(td => {
					var typeDef = module.metadata.GetTypeDefinition(td);
					return (module.metadata.GetString(typeDef.Namespace), module.metadata.GetString(typeDef.Name));
				});
			}
			DoDecompileTypes(typeDefs, decompileRun, decompilationContext, syntaxTree);
			RunTransforms(syntaxTree, decompileRun, decompilationContext);
			return syntaxTree;
		}

		/// <summary>
		/// Creates an <see cref="ILTransformContext"/> for the given <paramref name="function"/>.
		/// </summary>
		public ILTransformContext CreateILTransformContext(ILFunction function)
		{
			var decompileRun = new DecompileRun(settings) {
				DocumentationProvider = DocumentationProvider ?? CreateDefaultDocumentationProvider(),
				CancellationToken = CancellationToken
			};
			RequiredNamespaceCollector.CollectNamespaces(function.Method, module, decompileRun.Namespaces);
			return new ILTransformContext(function, typeSystem, DebugInfoProvider, settings) {
				CancellationToken = CancellationToken,
				DecompileRun = decompileRun
			};
		}

		/// <summary>
		/// Determines the "code-mappings" for a given TypeDef or MethodDef. See <see cref="CodeMappingInfo"/> for more information.
		/// </summary>
		public static CodeMappingInfo GetCodeMappingInfo(PEFile module, EntityHandle member)
		{
			var declaringType = member.GetDeclaringType(module.Metadata);

			if (declaringType.IsNil && member.Kind == HandleKind.TypeDefinition)
			{
				declaringType = (TypeDefinitionHandle)member;
			}

			var info = new CodeMappingInfo(module, declaringType);

			var td = module.Metadata.GetTypeDefinition(declaringType);

			foreach (var method in td.GetMethods())
			{
				var parent = method;
				var part = method;

				var connectedMethods = new Queue<MethodDefinitionHandle>();
				var processedMethods = new HashSet<MethodDefinitionHandle>();
				var processedNestedTypes = new HashSet<TypeDefinitionHandle>();
				connectedMethods.Enqueue(part);

				while (connectedMethods.Count > 0)
				{
					part = connectedMethods.Dequeue();
					if (!processedMethods.Add(part))
						continue;
					try
					{
						ReadCodeMappingInfo(module, info, parent, part, connectedMethods, processedNestedTypes);
					}
					catch (BadImageFormatException)
					{
						// ignore invalid IL
					}
				}
			}

			return info;
		}

		private static void ReadCodeMappingInfo(PEFile module, CodeMappingInfo info, MethodDefinitionHandle parent, MethodDefinitionHandle part, Queue<MethodDefinitionHandle> connectedMethods, HashSet<TypeDefinitionHandle> processedNestedTypes)
		{
			var md = module.Metadata.GetMethodDefinition(part);

			if (!md.HasBody())
			{
				info.AddMapping(parent, part);
				return;
			}

			var declaringType = md.GetDeclaringType();

			var blob = module.Reader.GetMethodBody(md.RelativeVirtualAddress).GetILReader();
			while (blob.RemainingBytes > 0)
			{
				var code = blob.DecodeOpCode();
				switch (code)
				{
					case ILOpCode.Newobj:
					case ILOpCode.Stfld:
						// async and yield fsms:
						var token = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
						if (token.IsNil)
							continue;
						TypeDefinitionHandle fsmTypeDef;
						switch (token.Kind)
						{
							case HandleKind.MethodDefinition:
								var fsmMethod = module.Metadata.GetMethodDefinition((MethodDefinitionHandle)token);
								fsmTypeDef = fsmMethod.GetDeclaringType();
								break;
							case HandleKind.FieldDefinition:
								var fsmField = module.Metadata.GetFieldDefinition((FieldDefinitionHandle)token);
								fsmTypeDef = fsmField.GetDeclaringType();
								break;
							case HandleKind.MemberReference:
								var memberRef = module.Metadata.GetMemberReference((MemberReferenceHandle)token);
								fsmTypeDef = ExtractDeclaringType(memberRef);
								break;
							default:
								continue;
						}
						if (!fsmTypeDef.IsNil)
						{
							var fsmType = module.Metadata.GetTypeDefinition(fsmTypeDef);
							// Must be a nested type of the containing type.
							if (fsmType.GetDeclaringType() != declaringType)
								break;
							if (YieldReturnDecompiler.IsCompilerGeneratorEnumerator(fsmTypeDef, module.Metadata)
								|| AsyncAwaitDecompiler.IsCompilerGeneratedStateMachine(fsmTypeDef, module.Metadata))
							{
								if (!processedNestedTypes.Add(fsmTypeDef))
									break;
								foreach (var h in fsmType.GetMethods())
								{
									if (module.MethodSemanticsLookup.GetSemantics(h).Item2 != 0)
										continue;
									var otherMethod = module.Metadata.GetMethodDefinition(h);
									if (!otherMethod.GetCustomAttributes().HasKnownAttribute(module.Metadata, KnownAttribute.DebuggerHidden))
									{
										connectedMethods.Enqueue(h);
									}
								}
							}
						}
						break;
					case ILOpCode.Ldftn:
						// deal with ldftn instructions, i.e., lambdas
						token = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
						if (token.IsNil)
							continue;
						TypeDefinitionHandle closureTypeHandle;
						switch (token.Kind)
						{
							case HandleKind.MethodDefinition:
								if (((MethodDefinitionHandle)token).IsCompilerGeneratedOrIsInCompilerGeneratedClass(module.Metadata))
								{
									connectedMethods.Enqueue((MethodDefinitionHandle)token);
								}
								continue;
							case HandleKind.MemberReference:
								var memberRef = module.Metadata.GetMemberReference((MemberReferenceHandle)token);
								if (memberRef.GetKind() != MemberReferenceKind.Method)
									continue;
								closureTypeHandle = ExtractDeclaringType(memberRef);
								if (!closureTypeHandle.IsNil)
								{
									var closureType = module.Metadata.GetTypeDefinition(closureTypeHandle);
									if (closureTypeHandle != declaringType)
									{
										// Must be a nested type of the containing type.
										if (closureType.GetDeclaringType() != declaringType)
											break;
										if (!processedNestedTypes.Add(closureTypeHandle))
											break;
										foreach (var m in closureType.GetMethods())
										{
											connectedMethods.Enqueue(m);
										}
									}
									else
									{
										// Delegate body is declared in the same type
										foreach (var m in closureType.GetMethods())
										{
											var methodDef = module.Metadata.GetMethodDefinition(m);
											if (methodDef.Name == memberRef.Name)
												connectedMethods.Enqueue(m);
										}
									}
									break;
								}
								break;
							default:
								continue;
						}
						break;
					case ILOpCode.Call:
					case ILOpCode.Callvirt:
						// deal with call/callvirt instructions, i.e., local function invocations
						token = MetadataTokenHelpers.EntityHandleOrNil(blob.ReadInt32());
						if (token.IsNil)
							continue;
						switch (token.Kind)
						{
							case HandleKind.MethodDefinition:
								break;
							case HandleKind.MethodSpecification:
								var methodSpec = module.Metadata.GetMethodSpecification((MethodSpecificationHandle)token);
								if (methodSpec.Method.IsNil || methodSpec.Method.Kind != HandleKind.MethodDefinition)
									continue;
								token = methodSpec.Method;
								break;
							default:
								continue;
						}
						if (LocalFunctionDecompiler.IsLocalFunctionMethod(module, (MethodDefinitionHandle)token))
						{
							connectedMethods.Enqueue((MethodDefinitionHandle)token);
						}
						break;
					default:
						blob.SkipOperand(code);
						break;
				}
			}

			info.AddMapping(parent, part);

			TypeDefinitionHandle ExtractDeclaringType(MemberReference memberRef)
			{
				switch (memberRef.Parent.Kind)
				{
					case HandleKind.TypeReference:
						// This should never happen in normal code, because we are looking at nested types
						// If it's not a nested type, it can't be a reference to the state machine or lambda anyway, and
						// those should be either TypeDef or TypeSpec.
						return default;
					case HandleKind.TypeDefinition:
						return (TypeDefinitionHandle)memberRef.Parent;
					case HandleKind.TypeSpecification:
						var ts = module.Metadata.GetTypeSpecification((TypeSpecificationHandle)memberRef.Parent);
						if (ts.Signature.IsNil)
							return default;
						// Do a quick scan using BlobReader
						var signature = module.Metadata.GetBlobReader(ts.Signature);
						// When dealing with FSM implementations, we can safely assume that if it's a type spec,
						// it must be a generic type instance.
						if (signature.ReadByte() != (byte)SignatureTypeCode.GenericTypeInstance)
							return default;
						// Skip over the rawTypeKind: value type or class
						var rawTypeKind = signature.ReadCompressedInteger();
						if (rawTypeKind < 17 || rawTypeKind > 18)
							return default;
						// Only read the generic type, ignore the type arguments
						var genericType = signature.ReadTypeHandle();
						// Again, we assume this is a type def, because we are only looking at nested types
						if (genericType.Kind != HandleKind.TypeDefinition)
							return default;
						return (TypeDefinitionHandle)genericType;
				}
				return default;
			}
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
			var decompilationContext = new SimpleTypeResolveContext(typeSystem.MainModule);
			var decompileRun = new DecompileRun(settings) {
				DocumentationProvider = DocumentationProvider ?? CreateDefaultDocumentationProvider(),
				CancellationToken = CancellationToken
			};
			syntaxTree = new SyntaxTree();

			foreach (var type in types)
			{
				CancellationToken.ThrowIfCancellationRequested();
				if (type.IsNil)
					throw new ArgumentException("types contains null element");
				RequiredNamespaceCollector.CollectNamespaces(type, module, decompileRun.Namespaces);
			}

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
		/// Note that decompiling types from modules other than the main module is not supported.
		/// </remarks>
		public SyntaxTree DecompileType(FullTypeName fullTypeName)
		{
			var type = typeSystem.FindType(fullTypeName.TopLevelTypeName).GetDefinition();
			if (type == null)
				throw new InvalidOperationException($"Could not find type definition {fullTypeName} in type system.");
			if (type.ParentModule != typeSystem.MainModule)
				throw new NotSupportedException("Decompiling types that are not part of the main module is not supported.");
			var decompilationContext = new SimpleTypeResolveContext(typeSystem.MainModule);
			var decompileRun = new DecompileRun(settings) {
				DocumentationProvider = DocumentationProvider ?? CreateDefaultDocumentationProvider(),
				CancellationToken = CancellationToken
			};
			syntaxTree = new SyntaxTree();
			RequiredNamespaceCollector.CollectNamespaces(type.MetadataToken, module, decompileRun.Namespaces);
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
			return Decompile((IEnumerable<EntityHandle>)definitions);
		}

		/// <summary>
		/// Decompile the specified types and/or members.
		/// </summary>
		public SyntaxTree Decompile(IEnumerable<EntityHandle> definitions)
		{
			if (definitions == null)
				throw new ArgumentNullException(nameof(definitions));
			syntaxTree = new SyntaxTree();
			var decompileRun = new DecompileRun(settings) {
				DocumentationProvider = DocumentationProvider ?? CreateDefaultDocumentationProvider(),
				CancellationToken = CancellationToken
			};
			foreach (var entity in definitions)
			{
				if (entity.IsNil)
					throw new ArgumentException("definitions contains null element");
				RequiredNamespaceCollector.CollectNamespaces(entity, module, decompileRun.Namespaces);
			}

			bool first = true;
			ITypeDefinition parentTypeDef = null;

			foreach (var entity in definitions)
			{
				switch (entity.Kind)
				{
					case HandleKind.TypeDefinition:
						ITypeDefinition typeDef = module.GetDefinition((TypeDefinitionHandle)entity);
						syntaxTree.Members.Add(DoDecompile(typeDef, decompileRun, new SimpleTypeResolveContext(typeDef)));
						if (first)
						{
							parentTypeDef = typeDef.DeclaringTypeDefinition;
						}
						else if (parentTypeDef != null)
						{
							parentTypeDef = FindCommonDeclaringTypeDefinition(parentTypeDef, typeDef.DeclaringTypeDefinition);
						}
						break;
					case HandleKind.MethodDefinition:
						IMethod method = module.GetDefinition((MethodDefinitionHandle)entity);
						syntaxTree.Members.Add(DoDecompile(method, decompileRun, new SimpleTypeResolveContext(method)));
						if (first)
						{
							parentTypeDef = method.DeclaringTypeDefinition;
						}
						else if (parentTypeDef != null)
						{
							parentTypeDef = FindCommonDeclaringTypeDefinition(parentTypeDef, method.DeclaringTypeDefinition);
						}
						break;
					case HandleKind.FieldDefinition:
						IField field = module.GetDefinition((FieldDefinitionHandle)entity);
						syntaxTree.Members.Add(DoDecompile(field, decompileRun, new SimpleTypeResolveContext(field)));
						parentTypeDef = field.DeclaringTypeDefinition;
						break;
					case HandleKind.PropertyDefinition:
						IProperty property = module.GetDefinition((PropertyDefinitionHandle)entity);
						syntaxTree.Members.Add(DoDecompile(property, decompileRun, new SimpleTypeResolveContext(property)));
						if (first)
						{
							parentTypeDef = property.DeclaringTypeDefinition;
						}
						else if (parentTypeDef != null)
						{
							parentTypeDef = FindCommonDeclaringTypeDefinition(parentTypeDef, property.DeclaringTypeDefinition);
						}
						break;
					case HandleKind.EventDefinition:
						IEvent ev = module.GetDefinition((EventDefinitionHandle)entity);
						syntaxTree.Members.Add(DoDecompile(ev, decompileRun, new SimpleTypeResolveContext(ev)));
						if (first)
						{
							parentTypeDef = ev.DeclaringTypeDefinition;
						}
						else if (parentTypeDef != null)
						{
							parentTypeDef = FindCommonDeclaringTypeDefinition(parentTypeDef, ev.DeclaringTypeDefinition);
						}
						break;
					default:
						throw new NotSupportedException(entity.Kind.ToString());
				}
				first = false;
			}
			RunTransforms(syntaxTree, decompileRun, parentTypeDef != null ? new SimpleTypeResolveContext(parentTypeDef) : new SimpleTypeResolveContext(typeSystem.MainModule));
			return syntaxTree;
		}

		ITypeDefinition FindCommonDeclaringTypeDefinition(ITypeDefinition a, ITypeDefinition b)
		{
			if (a == null || b == null)
				return null;
			var declaringTypes = a.GetDeclaringTypeDefinitions();
			var set = new HashSet<ITypeDefinition>(b.GetDeclaringTypeDefinitions());
			return declaringTypes.FirstOrDefault(set.Contains);
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
		public string DecompileAsString(IEnumerable<EntityHandle> definitions)
		{
			return SyntaxTreeToString(Decompile(definitions));
		}

		IEnumerable<EntityDeclaration> AddInterfaceImplHelpers(
			EntityDeclaration memberDecl, IMethod method,
			TypeSystemAstBuilder astBuilder)
		{
			if (!memberDecl.GetChildByRole(EntityDeclaration.PrivateImplementationTypeRole).IsNull)
			{
				yield break; // cannot create forwarder for existing explicit interface impl
			}
			var genericContext = new Decompiler.TypeSystem.GenericContext(method);
			var methodHandle = (MethodDefinitionHandle)method.MetadataToken;
			foreach (var h in methodHandle.GetMethodImplementations(metadata))
			{
				var mi = metadata.GetMethodImplementation(h);
				IMethod m = module.ResolveMethod(mi.MethodDeclaration, genericContext);
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
				if (m.ReturnType.IsKnownType(KnownTypeCode.Void))
				{
					methodDecl.Body.Add(new ExpressionStatement(forwardingCall));
				}
				else
				{
					methodDecl.Body.Add(new ReturnStatement(forwardingCall));
				}
				yield return methodDecl;
			}
		}

		Expression ForwardParameter(ParameterDeclaration p)
		{
			switch (p.ParameterModifier)
			{
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
			var entity = (IEntity)member.GetSymbol();
			var lookup = new MemberLookup(entity.DeclaringTypeDefinition, entity.ParentModule);

			var baseTypes = entity.DeclaringType.GetNonInterfaceBaseTypes().Where(t => entity.DeclaringType != t).ToList();

			// A constant, field, property, event, or type introduced in a class or struct hides all base class members with the same name.
			bool hideBasedOnSignature = !(entity is ITypeDefinition
				|| entity.SymbolKind == SymbolKind.Field
				|| entity.SymbolKind == SymbolKind.Property
				|| entity.SymbolKind == SymbolKind.Event);

			const GetMemberOptions options = GetMemberOptions.IgnoreInheritedMembers | GetMemberOptions.ReturnMemberDefinitions;

			if (HidesMemberOrTypeOfBaseType())
				member.Modifiers |= Modifiers.New;

			bool HidesMemberOrTypeOfBaseType()
			{
				var parameterListComparer = ParameterListComparer.WithOptions(includeModifiers: true);

				foreach (IType baseType in baseTypes)
				{
					if (!hideBasedOnSignature)
					{
						if (baseType.GetNestedTypes(t => t.Name == entity.Name && lookup.IsAccessible(t, true), options).Any())
							return true;
						if (baseType.GetMembers(m => m.Name == entity.Name && m.SymbolKind != SymbolKind.Indexer && lookup.IsAccessible(m, true), options).Any())
							return true;
					}
					else
					{
						if (entity.SymbolKind == SymbolKind.Indexer)
						{
							// An indexer introduced in a class or struct hides all base class indexers with the same signature (parameter count and types).
							if (baseType.GetProperties(p => p.SymbolKind == SymbolKind.Indexer && lookup.IsAccessible(p, true))
									.Any(p => parameterListComparer.Equals(((IProperty)entity).Parameters, p.Parameters)))
							{
								return true;
							}
						}
						else if (entity.SymbolKind == SymbolKind.Method)
						{
							// A method introduced in a class or struct hides all non-method base class members with the same name, and all
							// base class methods with the same signature (method name and parameter count, modifiers, and types).
							if (baseType.GetMembers(m => m.SymbolKind != SymbolKind.Indexer && m.Name == entity.Name && lookup.IsAccessible(m, true))
									.Any(m => m.SymbolKind != SymbolKind.Method || (((IMethod)entity).TypeParameters.Count == ((IMethod)m).TypeParameters.Count
																					&& parameterListComparer.Equals(((IMethod)entity).Parameters, ((IMethod)m).Parameters))))
							{
								return true;
							}
						}
					}
				}

				return false;
			}
		}

		void FixParameterNames(EntityDeclaration entity)
		{
			int i = 0;
			foreach (var parameter in entity.GetChildrenByRole(Roles.Parameter))
			{
				if (string.IsNullOrEmpty(parameter.Name) && !parameter.Type.IsArgList())
				{
					// needs to be consistent with logic in ILReader.CreateILVarable(ParameterDefinition)
					parameter.Name = "P_" + i;
				}
				i++;
			}
		}

		EntityDeclaration DoDecompile(ITypeDefinition typeDef, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentTypeDefinition == typeDef);
			try
			{
				var typeSystemAstBuilder = CreateAstBuilder(decompileRun.Settings);
				var entityDecl = typeSystemAstBuilder.ConvertEntity(typeDef);
				var typeDecl = entityDecl as TypeDeclaration;
				if (typeDecl == null)
				{
					// e.g. DelegateDeclaration
					return entityDecl;
				}
				bool isRecord = settings.RecordClasses && typeDef.IsRecord;
				foreach (var type in typeDef.NestedTypes)
				{
					if (!type.MetadataToken.IsNil && !MemberIsHidden(module.PEFile, type.MetadataToken, settings))
					{
						var nestedType = DoDecompile(type, decompileRun, decompilationContext.WithCurrentTypeDefinition(type));
						SetNewModifier(nestedType);
						typeDecl.Members.Add(nestedType);
					}
				}
				foreach (var field in typeDef.Fields)
				{
					if (!field.MetadataToken.IsNil && !MemberIsHidden(module.PEFile, field.MetadataToken, settings))
					{
						if (typeDef.Kind == TypeKind.Enum && !field.IsConst)
							continue;
						var memberDecl = DoDecompile(field, decompileRun, decompilationContext.WithCurrentMember(field));
						typeDecl.Members.Add(memberDecl);
					}
				}
				foreach (var property in typeDef.Properties)
				{
					if (!property.MetadataToken.IsNil && !MemberIsHidden(module.PEFile, property.MetadataToken, settings))
					{
						var propDecl = DoDecompile(property, decompileRun, decompilationContext.WithCurrentMember(property));
						typeDecl.Members.Add(propDecl);
					}
				}
				foreach (var @event in typeDef.Events)
				{
					if (!@event.MetadataToken.IsNil && !MemberIsHidden(module.PEFile, @event.MetadataToken, settings))
					{
						var eventDecl = DoDecompile(@event, decompileRun, decompilationContext.WithCurrentMember(@event));
						typeDecl.Members.Add(eventDecl);
					}
				}
				foreach (var method in typeDef.Methods)
				{
					if (isRecord)
					{
						// Some members in records are always compiler-generated and lead to a
						// "duplicate definition" error if we emit the generated code.
						if ((method.Name == "op_Equality" || method.Name == "op_Inequality")
							&& method.Parameters.Count == 2
							&& method.Parameters.All(p => p.Type.GetDefinition() == typeDef))
						{
							// Don't emit comparison operators into C# record definition
							// Note: user can declare additional operator== as long as they have
							// different parameter types.
							continue;
						}
						if (method.Name == "Equals" && method.Parameters.Count == 1 && method.Parameters[0].Type.IsKnownType(KnownTypeCode.Object))
						{
							// override bool Equals(object? obj)
							// Note: the "virtual bool Equals(R? other)" overload can be customized
							continue;
						}
						if (method.Name == "<Clone>$" && method.Parameters.Count == 0)
						{
							// Method name cannot be expressed in C#
							continue;
						}
					}
					if (!method.MetadataToken.IsNil && !MemberIsHidden(module.PEFile, method.MetadataToken, settings))
					{
						var memberDecl = DoDecompile(method, decompileRun, decompilationContext.WithCurrentMember(method));
						typeDecl.Members.Add(memberDecl);
						typeDecl.Members.AddRange(AddInterfaceImplHelpers(memberDecl, method, typeSystemAstBuilder));
					}
				}
				if (typeDecl.Members.OfType<IndexerDeclaration>().Any(idx => idx.PrivateImplementationType.IsNull))
				{
					// Remove the [DefaultMember] attribute if the class contains indexers
					RemoveAttribute(typeDecl, KnownAttribute.DefaultMember);
				}
				if (settings.IntroduceRefModifiersOnStructs)
				{
					if (FindAttribute(typeDecl, KnownAttribute.Obsolete, out var attr))
					{
						if (obsoleteAttributePattern.IsMatch(attr))
						{
							if (attr.Parent is AttributeSection section && section.Attributes.Count == 1)
								section.Remove();
							else
								attr.Remove();
						}
					}
				}
				if (typeDecl.ClassType == ClassType.Enum)
				{
					switch (DetectBestEnumValueDisplayMode(typeDef, module.PEFile))
					{
						case EnumValueDisplayMode.FirstOnly:
							foreach (var enumMember in typeDecl.Members.OfType<EnumMemberDeclaration>().Skip(1))
							{
								enumMember.Initializer = null;
							}
							break;
						case EnumValueDisplayMode.None:
							foreach (var enumMember in typeDecl.Members.OfType<EnumMemberDeclaration>())
							{
								enumMember.Initializer = null;
								if (enumMember.GetSymbol() is IField f && f.GetConstantValue() == null)
								{
									typeDecl.InsertChildBefore(enumMember, new Comment(" error: enumerator has no value"), Roles.Comment);
								}
							}
							break;
						case EnumValueDisplayMode.All:
							// nothing needs to be changed.
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}
				return typeDecl;
			}
			catch (Exception innerException) when (!(innerException is OperationCanceledException || innerException is DecompilerException))
			{
				throw new DecompilerException(module, typeDef, innerException);
			}
		}

		enum EnumValueDisplayMode
		{
			None,
			All,
			FirstOnly
		}

		EnumValueDisplayMode DetectBestEnumValueDisplayMode(ITypeDefinition typeDef, PEFile module)
		{
			if (settings.AlwaysShowEnumMemberValues)
				return EnumValueDisplayMode.All;
			if (typeDef.HasAttribute(KnownAttribute.Flags, inherit: false))
				return EnumValueDisplayMode.All;
			bool first = true;
			long firstValue = 0, previousValue = 0;
			foreach (var field in typeDef.Fields)
			{
				if (MemberIsHidden(module, field.MetadataToken, settings))
					continue;
				object constantValue = field.GetConstantValue();
				if (constantValue == null)
					continue;
				long currentValue = (long)CSharpPrimitiveCast.Cast(TypeCode.Int64, constantValue, false);
				if (first)
				{
					firstValue = currentValue;
					first = false;
				}
				else if (previousValue + 1 != currentValue)
				{
					return EnumValueDisplayMode.All;
				}
				previousValue = currentValue;
			}
			return firstValue == 0 ? EnumValueDisplayMode.None : EnumValueDisplayMode.FirstOnly;
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
			method.Parameters.Add(new ParameterDeclaration(typeSystemAstBuilder.ConvertType(typeSystem.FindType(source)), "input"));
			method.ReturnType = typeSystemAstBuilder.ConvertType(typeSystem.FindType(target));
			method.Body = new BlockStatement {
				new IfElseStatement {
					Condition = new BinaryOperatorExpression {
						Left = new MemberReferenceExpression(new TypeReferenceExpression(typeSystemAstBuilder.ConvertType(typeSystem.FindType(KnownTypeCode.IntPtr))), "Size"),
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
			var typeSystemAstBuilder = CreateAstBuilder(decompileRun.Settings);
			var methodDecl = typeSystemAstBuilder.ConvertEntity(method);
			int lastDot = method.Name.LastIndexOf('.');
			if (method.IsExplicitInterfaceImplementation && lastDot >= 0)
			{
				methodDecl.Name = method.Name.Substring(lastDot + 1);
			}
			FixParameterNames(methodDecl);
			var methodDefinition = metadata.GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
			if (!settings.LocalFunctions && LocalFunctionDecompiler.LocalFunctionNeedsAccessibilityChange(method.ParentModule.PEFile, (MethodDefinitionHandle)method.MetadataToken))
			{
				// if local functions are not active and we're dealing with a local function,
				// reduce the visibility of the method to private,
				// otherwise this leads to compile errors because the display classes have lesser accessibility.
				// Note: removing and then adding the static modifier again is necessary to set the private modifier before all other modifiers.
				methodDecl.Modifiers &= ~(Modifiers.Internal | Modifiers.Static);
				methodDecl.Modifiers |= Modifiers.Private | (method.IsStatic ? Modifiers.Static : 0);
			}
			if (methodDefinition.HasBody())
			{
				DecompileBody(method, methodDecl, decompileRun, decompilationContext);
			}
			else if (!method.IsAbstract && method.DeclaringType.Kind != TypeKind.Interface)
			{
				methodDecl.Modifiers |= Modifiers.Extern;
			}
			if (method.SymbolKind == SymbolKind.Method && !method.IsExplicitInterfaceImplementation && methodDefinition.HasFlag(System.Reflection.MethodAttributes.Virtual) == methodDefinition.HasFlag(System.Reflection.MethodAttributes.NewSlot))
			{
				SetNewModifier(methodDecl);
			}
			return methodDecl;
		}

		internal static bool IsWindowsFormsInitializeComponentMethod(IMethod method)
		{
			return method.ReturnType.Kind == TypeKind.Void && method.Name == "InitializeComponent" && method.DeclaringTypeDefinition.GetNonInterfaceBaseTypes().Any(t => t.FullName == "System.Windows.Forms.Control");
		}

		void DecompileBody(IMethod method, EntityDeclaration entityDecl, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
		{
			try
			{
				var ilReader = new ILReader(typeSystem.MainModule) {
					UseDebugSymbols = settings.UseDebugSymbols,
					DebugInfo = DebugInfoProvider
				};
				var methodDef = metadata.GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
				var body = BlockStatement.Null;
				MethodBodyBlock methodBody;
				try
				{
					methodBody = module.PEFile.Reader.GetMethodBody(methodDef.RelativeVirtualAddress);
				}
				catch (BadImageFormatException ex)
				{
					body = new BlockStatement();
					body.AddChild(new Comment("Invalid MethodBodyBlock: " + ex.Message), Roles.Comment);
					entityDecl.AddChild(body, Roles.Body);
					return;
				}
				var function = ilReader.ReadIL((MethodDefinitionHandle)method.MetadataToken, methodBody, cancellationToken: CancellationToken);
				function.CheckInvariant(ILPhase.Normal);

				if (entityDecl != null)
				{
					int i = 0;
					var parameters = function.Variables.Where(v => v.Kind == VariableKind.Parameter).ToDictionary(v => v.Index);
					foreach (var parameter in entityDecl.GetChildrenByRole(Roles.Parameter))
					{
						if (parameters.TryGetValue(i, out var v))
							parameter.AddAnnotation(new ILVariableResolveResult(v, method.Parameters[i].Type));
						i++;
					}
					entityDecl.AddAnnotation(function);
				}

				var localSettings = settings.Clone();
				if (IsWindowsFormsInitializeComponentMethod(method))
				{
					localSettings.UseImplicitMethodGroupConversion = false;
					localSettings.UsingDeclarations = false;
					localSettings.AlwaysCastTargetsOfExplicitInterfaceImplementationCalls = true;
					localSettings.NamedArguments = false;
				}

				var context = new ILTransformContext(function, typeSystem, DebugInfoProvider, localSettings) {
					CancellationToken = CancellationToken,
					DecompileRun = decompileRun
				};
				foreach (var transform in ilTransforms)
				{
					CancellationToken.ThrowIfCancellationRequested();
					transform.Run(function, context);
					function.CheckInvariant(ILPhase.Normal);
					// When decompiling definitions only, we can cancel decompilation of all steps
					// after yield and async detection, because only those are needed to properly set
					// IsAsync/IsIterator flags on ILFunction.
					if (!localSettings.DecompileMemberBodies && transform is AsyncAwaitDecompiler)
						break;
				}

				// Generate C# AST only if bodies should be displayed.
				if (localSettings.DecompileMemberBodies)
				{
					AddDefinesForConditionalAttributes(function, decompileRun);
					var statementBuilder = new StatementBuilder(
						typeSystem,
						decompilationContext,
						function,
						localSettings,
						decompileRun,
						CancellationToken
					);
					body = statementBuilder.ConvertAsBlock(function.Body);

					Comment prev = null;
					foreach (string warning in function.Warnings)
					{
						body.InsertChildAfter(prev, prev = new Comment(warning), Roles.Comment);
					}

					entityDecl.AddChild(body, Roles.Body);
				}
				entityDecl.AddAnnotation(function);

				CleanUpMethodDeclaration(entityDecl, body, function, localSettings.DecompileMemberBodies);
			}
			catch (Exception innerException) when (!(innerException is OperationCanceledException || innerException is DecompilerException))
			{
				throw new DecompilerException(module, method, innerException);
			}
		}

		internal static void CleanUpMethodDeclaration(EntityDeclaration entityDecl, BlockStatement body, ILFunction function, bool decompileBody = true)
		{
			if (function.IsIterator)
			{
				if (decompileBody && !body.Descendants.Any(d => d is YieldReturnStatement || d is YieldBreakStatement))
				{
					body.Add(new YieldBreakStatement());
				}
				if (function.IsAsync)
				{
					RemoveAttribute(entityDecl, KnownAttribute.AsyncIteratorStateMachine);
				}
				else
				{
					RemoveAttribute(entityDecl, KnownAttribute.IteratorStateMachine);
				}
				if (function.StateMachineCompiledWithMono)
				{
					RemoveAttribute(entityDecl, KnownAttribute.DebuggerHidden);
				}
			}
			if (function.IsAsync)
			{
				entityDecl.Modifiers |= Modifiers.Async;
				RemoveAttribute(entityDecl, KnownAttribute.AsyncStateMachine);
				RemoveAttribute(entityDecl, KnownAttribute.DebuggerStepThrough);
			}
		}

		internal static bool RemoveAttribute(EntityDeclaration entityDecl, KnownAttribute attributeType)
		{
			bool found = false;
			foreach (var section in entityDecl.Attributes)
			{
				foreach (var attr in section.Attributes)
				{
					var symbol = attr.Type.GetSymbol();
					if (symbol is ITypeDefinition td && td.FullTypeName == attributeType.GetTypeName())
					{
						attr.Remove();
						found = true;
					}
				}
				if (section.Attributes.Count == 0)
				{
					section.Remove();
				}
			}
			return found;
		}

		bool FindAttribute(EntityDeclaration entityDecl, KnownAttribute attributeType, out Syntax.Attribute attribute)
		{
			attribute = null;
			foreach (var section in entityDecl.Attributes)
			{
				foreach (var attr in section.Attributes)
				{
					var symbol = attr.Type.GetSymbol();
					if (symbol is ITypeDefinition td && td.FullTypeName == attributeType.GetTypeName())
					{
						attribute = attr;
						return true;
					}
				}
			}
			return false;
		}

		void AddDefinesForConditionalAttributes(ILFunction function, DecompileRun decompileRun)
		{
			foreach (var call in function.Descendants.OfType<CallInstruction>())
			{
				var attr = call.Method.GetAttribute(KnownAttribute.Conditional, inherit: true);
				var symbolName = attr?.FixedArguments.FirstOrDefault().Value as string;
				if (symbolName == null || !decompileRun.DefinedSymbols.Add(symbolName))
					continue;
				syntaxTree.InsertChildAfter(null, new PreProcessorDirective(PreProcessorDirectiveType.Define, symbolName), Roles.PreProcessorDirective);
			}
		}

		EntityDeclaration DoDecompile(IField field, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == field);
			try
			{
				var typeSystemAstBuilder = CreateAstBuilder(decompileRun.Settings);
				if (decompilationContext.CurrentTypeDefinition.Kind == TypeKind.Enum && field.IsConst)
				{
					var enumDec = new EnumMemberDeclaration { Name = field.Name };
					object constantValue = field.GetConstantValue();
					if (constantValue != null)
					{
						long initValue = (long)CSharpPrimitiveCast.Cast(TypeCode.Int64, constantValue, false);
						enumDec.Initializer = typeSystemAstBuilder.ConvertConstantValue(decompilationContext.CurrentTypeDefinition.EnumUnderlyingType, constantValue);
						if (enumDec.Initializer is PrimitiveExpression primitive
							&& initValue >= 0 && (decompilationContext.CurrentTypeDefinition.HasAttribute(KnownAttribute.Flags)
								|| (initValue > 9 && (unchecked(initValue & (initValue - 1)) == 0 || unchecked(initValue & (initValue + 1)) == 0))))
						{
							primitive.Format = LiteralFormat.HexadecimalNumber;
						}
					}
					enumDec.Attributes.AddRange(field.GetAttributes().Select(a => new AttributeSection(typeSystemAstBuilder.ConvertAttribute(a))));
					enumDec.AddAnnotation(new MemberResolveResult(null, field));
					return enumDec;
				}
				bool isMathPIOrE = ((field.Name == "PI" || field.Name == "E") && (field.DeclaringType.FullName == "System.Math" || field.DeclaringType.FullName == "System.MathF"));
				typeSystemAstBuilder.UseSpecialConstants = !(field.DeclaringType.Equals(field.ReturnType) || isMathPIOrE);
				var fieldDecl = typeSystemAstBuilder.ConvertEntity(field);
				SetNewModifier(fieldDecl);
				if (settings.FixedBuffers && IsFixedField(field, out var elementType, out var elementCount))
				{
					var fixedFieldDecl = new FixedFieldDeclaration();
					fieldDecl.Attributes.MoveTo(fixedFieldDecl.Attributes);
					fixedFieldDecl.Modifiers = fieldDecl.Modifiers;
					fixedFieldDecl.ReturnType = typeSystemAstBuilder.ConvertType(elementType);
					fixedFieldDecl.Variables.Add(new FixedVariableInitializer(field.Name, new PrimitiveExpression(elementCount)));
					fixedFieldDecl.Variables.Single().CopyAnnotationsFrom(((FieldDeclaration)fieldDecl).Variables.Single());
					fixedFieldDecl.CopyAnnotationsFrom(fieldDecl);
					RemoveAttribute(fixedFieldDecl, KnownAttribute.FixedBuffer);
					return fixedFieldDecl;
				}
				var fieldDefinition = metadata.GetFieldDefinition((FieldDefinitionHandle)field.MetadataToken);
				if (fieldDefinition.HasFlag(System.Reflection.FieldAttributes.HasFieldRVA))
				{
					// Field data as specified in II.16.3.1 of ECMA-335 6th edition:
					// .data I_X = int32(123)
					// .field public static int32 _x at I_X
					string message;
					try
					{
						var initVal = fieldDefinition.GetInitialValue(module.PEFile.Reader, TypeSystem);
						message = string.Format(" Not supported: data({0}) ", BitConverter.ToString(initVal.ReadBytes(initVal.RemainingBytes)).Replace('-', ' '));
					}
					catch (BadImageFormatException ex)
					{
						message = ex.Message;
					}
					((FieldDeclaration)fieldDecl).Variables.Single().AddChild(new Comment(message, CommentType.MultiLine), Roles.Comment);
				}
				return fieldDecl;
			}
			catch (Exception innerException) when (!(innerException is OperationCanceledException || innerException is DecompilerException))
			{
				throw new DecompilerException(module, field, innerException);
			}
		}

		internal static bool IsFixedField(IField field, out IType type, out int elementCount)
		{
			type = null;
			elementCount = 0;
			IAttribute attr = field.GetAttribute(KnownAttribute.FixedBuffer, inherit: false);
			if (attr != null && attr.FixedArguments.Length == 2)
			{
				if (attr.FixedArguments[0].Value is IType trr && attr.FixedArguments[1].Value is int length)
				{
					type = trr;
					elementCount = length;
					return true;
				}
			}
			return false;
		}

		EntityDeclaration DoDecompile(IProperty property, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == property);
			try
			{
				var typeSystemAstBuilder = CreateAstBuilder(decompileRun.Settings);
				EntityDeclaration propertyDecl = typeSystemAstBuilder.ConvertEntity(property);
				if (property.IsExplicitInterfaceImplementation && !property.IsIndexer)
				{
					int lastDot = property.Name.LastIndexOf('.');
					propertyDecl.Name = property.Name.Substring(lastDot + 1);
				}
				FixParameterNames(propertyDecl);
				Accessor getter, setter;
				if (propertyDecl is PropertyDeclaration)
				{
					getter = ((PropertyDeclaration)propertyDecl).Getter;
					setter = ((PropertyDeclaration)propertyDecl).Setter;
				}
				else
				{
					getter = ((IndexerDeclaration)propertyDecl).Getter;
					setter = ((IndexerDeclaration)propertyDecl).Setter;
				}
				if (property.CanGet && property.Getter.HasBody)
				{
					DecompileBody(property.Getter, getter, decompileRun, decompilationContext);
				}
				if (property.CanSet && property.Setter.HasBody)
				{
					DecompileBody(property.Setter, setter, decompileRun, decompilationContext);
				}
				var accessorHandle = (MethodDefinitionHandle)(property.Getter ?? property.Setter).MetadataToken;
				var accessor = metadata.GetMethodDefinition(accessorHandle);
				if (!accessorHandle.GetMethodImplementations(metadata).Any() && accessor.HasFlag(System.Reflection.MethodAttributes.Virtual) == accessor.HasFlag(System.Reflection.MethodAttributes.NewSlot))
					SetNewModifier(propertyDecl);
				return propertyDecl;
			}
			catch (Exception innerException) when (!(innerException is OperationCanceledException || innerException is DecompilerException))
			{
				throw new DecompilerException(module, property, innerException);
			}
		}

		EntityDeclaration DoDecompile(IEvent ev, DecompileRun decompileRun, ITypeResolveContext decompilationContext)
		{
			Debug.Assert(decompilationContext.CurrentMember == ev);
			try
			{
				var typeSystemAstBuilder = CreateAstBuilder(decompileRun.Settings);
				typeSystemAstBuilder.UseCustomEvents = ev.DeclaringTypeDefinition.Kind != TypeKind.Interface;
				var eventDecl = typeSystemAstBuilder.ConvertEntity(ev);
				int lastDot = ev.Name.LastIndexOf('.');
				if (ev.IsExplicitInterfaceImplementation)
				{
					eventDecl.Name = ev.Name.Substring(lastDot + 1);
				}
				if (ev.CanAdd && ev.AddAccessor.HasBody)
				{
					DecompileBody(ev.AddAccessor, ((CustomEventDeclaration)eventDecl).AddAccessor, decompileRun, decompilationContext);
				}
				if (ev.CanRemove && ev.RemoveAccessor.HasBody)
				{
					DecompileBody(ev.RemoveAccessor, ((CustomEventDeclaration)eventDecl).RemoveAccessor, decompileRun, decompilationContext);
				}
				var accessor = metadata.GetMethodDefinition((MethodDefinitionHandle)(ev.AddAccessor ?? ev.RemoveAccessor).MetadataToken);
				if (accessor.HasFlag(System.Reflection.MethodAttributes.Virtual) == accessor.HasFlag(System.Reflection.MethodAttributes.NewSlot))
				{
					SetNewModifier(eventDecl);
				}
				return eventDecl;
			}
			catch (Exception innerException) when (!(innerException is OperationCanceledException || innerException is DecompilerException))
			{
				throw new DecompilerException(module, ev, innerException);
			}
		}

		#region Sequence Points
		/// <summary>
		/// Creates sequence points for the given syntax tree.
		/// 
		/// This only works correctly when the nodes in the syntax tree have line/column information.
		/// </summary>
		public Dictionary<ILFunction, List<DebugInfo.SequencePoint>> CreateSequencePoints(SyntaxTree syntaxTree)
		{
			SequencePointBuilder spb = new SequencePointBuilder();
			syntaxTree.AcceptVisitor(spb);
			return spb.GetSequencePoints();
		}
		#endregion
	}
}
