// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Composition;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using AvaloniaEdit.Highlighting;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX;

using ConversionFlags = ICSharpCode.Decompiler.Output.ConversionFlags;
// Two unrelated `LanguageVersion` types are in scope: a DTO in ILSpyX (toolbar dropdown
// items) and an enum in Decompiler.CSharp (the version key the decompiler reads). Alias
// the enum to disambiguate; the DTO keeps its plain name to match WPF's call sites.
using CSharpLanguageVersion = ICSharpCode.Decompiler.CSharp.LanguageVersion;
using LanguageVersionDto = ICSharpCode.ILSpyX.LanguageVersion;

namespace ILSpy.Languages
{
	[Export(typeof(Language))]
	[Shared]
	public sealed class CSharpLanguage : Language
	{
		public override string Name => "C#";

		public override string FileExtension => ".cs";

		public override string ProjectFileExtension => ".csproj";

		public override ILSpy.TextView.IBracketSearcher BracketSearcher { get; } = new CSharpBracketSearcher();

		static IReadOnlyList<LanguageVersionDto>? cachedVersions;

		public override IReadOnlyList<LanguageVersionDto> LanguageVersions => cachedVersions ??= new List<LanguageVersionDto> {
			new(CSharpLanguageVersion.CSharp1.ToString(), "C# 1.0 / VS .NET"),
			new(CSharpLanguageVersion.CSharp2.ToString(), "C# 2.0 / VS 2005"),
			new(CSharpLanguageVersion.CSharp3.ToString(), "C# 3.0 / VS 2008"),
			new(CSharpLanguageVersion.CSharp4.ToString(), "C# 4.0 / VS 2010"),
			new(CSharpLanguageVersion.CSharp5.ToString(), "C# 5.0 / VS 2012"),
			new(CSharpLanguageVersion.CSharp6.ToString(), "C# 6.0 / VS 2015"),
			new(CSharpLanguageVersion.CSharp7.ToString(), "C# 7.0 / VS 2017"),
			new(CSharpLanguageVersion.CSharp7_1.ToString(), "C# 7.1 / VS 2017.3"),
			new(CSharpLanguageVersion.CSharp7_2.ToString(), "C# 7.2 / VS 2017.4"),
			new(CSharpLanguageVersion.CSharp7_3.ToString(), "C# 7.3 / VS 2017.7"),
			new(CSharpLanguageVersion.CSharp8_0.ToString(), "C# 8.0 / VS 2019"),
			new(CSharpLanguageVersion.CSharp9_0.ToString(), "C# 9.0 / VS 2019.8"),
			new(CSharpLanguageVersion.CSharp10_0.ToString(), "C# 10.0 / VS 2022"),
			new(CSharpLanguageVersion.CSharp11_0.ToString(), "C# 11.0 / VS 2022.4"),
			new(CSharpLanguageVersion.CSharp12_0.ToString(), "C# 12.0 / VS 2022.8"),
			new(CSharpLanguageVersion.CSharp13_0.ToString(), "C# 13.0 / VS 2022.12"),
			new(CSharpLanguageVersion.CSharp14_0.ToString(), "C# 14.0 / VS 2026"),
		};

		static CSharpAmbience CreateAmbience() => new() {
			ConversionFlags = ConversionFlags.ShowTypeParameterList | ConversionFlags.PlaceReturnTypeAfterParameterList,
		};

		public override string TypeToString(IType type, ConversionFlags conversionFlags = ConversionFlags.UseFullyQualifiedEntityNames | ConversionFlags.UseFullyQualifiedTypeNames)
		{
			ArgumentNullException.ThrowIfNull(type);
			var ambience = CreateAmbience();
			ambience.ConversionFlags |= conversionFlags;
			return type is ITypeDefinition def
				? ambience.ConvertSymbol(def)
				: ambience.ConvertType(type);
		}

		public override string EntityToString(IEntity entity, ConversionFlags conversionFlags)
		{
			ArgumentNullException.ThrowIfNull(entity);
			var ambience = CreateAmbience();
			ambience.ConversionFlags |= conversionFlags
				| ConversionFlags.ShowReturnType
				| ConversionFlags.ShowParameterList
				| ConversionFlags.ShowParameterModifiers;
			return ambience.ConvertSymbol(entity);
		}

		public override void WriteCommentLine(ITextOutput output, string comment) => output.WriteLine("// " + comment);

		public override bool ShowMember(IEntity member)
		{
			ArgumentNullException.ThrowIfNull(member);
			if (member.MetadataToken.IsNil)
				return true;
			var assembly = member.ParentModule?.MetadataFile;
			if (assembly == null)
				return true;
			return !CSharpDecompiler.MemberIsHidden(assembly, member.MetadataToken, new DecompilerSettings());
		}

		public override RichText GetRichTextTooltip(IEntity entity)
		{
			ArgumentNullException.ThrowIfNull(entity);
			var flags = ConversionFlags.All & ~(ConversionFlags.ShowBody | ConversionFlags.PlaceReturnTypeAfterParameterList);
			var output = new StringWriter();
			var decoratedWriter = new TextWriterTokenWriter(output);
			var writer = new CSharpHighlightingTokenWriter(TokenWriter.InsertRequiredSpaces(decoratedWriter), locatable: decoratedWriter);
			if (entity is IMethod m && m.IsLocalFunction)
				writer.WriteIdentifier(Identifier.Create("(local)"));
			new CSharpAmbience { ConversionFlags = flags }.ConvertSymbol(entity, writer, FormattingOptionsFactory.CreateAllman());
			return new RichText(output.ToString(), writer.HighlightingModel);
		}

		CSharpDecompiler CreateDecompiler(MetadataFile module, DecompilationOptions options)
		{
			var decompiler = new CSharpDecompiler(module, module.GetAssemblyResolver(options.DecompilerSettings.AutoLoadAssemblyReferences), options.DecompilerSettings) {
				CancellationToken = options.CancellationToken,
				DebugInfoProvider = module.GetDebugInfoOrNull(),
			};
			if (options.EscapeInvalidIdentifiers)
				decompiler.AstTransforms.Add(new EscapeInvalidIdentifiers());
			return decompiler;
		}

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile assembly = method.ParentModule!.MetadataFile!;
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			AddReferenceAssemblyWarningMessage(assembly, output);
			AddReferenceWarningMessage(assembly, output);
			WriteCommentLine(output, assembly.FullName);
			WriteCommentLine(output, TypeToString(method.DeclaringType));
			var methodDefinition = decompiler.TypeSystem.MainModule.ResolveEntity(method.MetadataToken) as IMethod;
			if (methodDefinition!.IsConstructor && methodDefinition.DeclaringType.IsReferenceType != false)
			{
				var members = CollectFieldsAndCtors(methodDefinition.DeclaringTypeDefinition!, methodDefinition.IsStatic);
				decompiler.AstTransforms.Add(new SelectCtorTransform(methodDefinition));
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(members), decompiler.TypeSystem);
			}
			else
			{
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(method.MetadataToken), decompiler.TypeSystem);
			}
		}

		public override void DecompileProperty(IProperty property, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile assembly = property.ParentModule!.MetadataFile!;
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			AddReferenceAssemblyWarningMessage(assembly, output);
			AddReferenceWarningMessage(assembly, output);
			WriteCommentLine(output, assembly.FullName);
			WriteCommentLine(output, TypeToString(property.DeclaringType));
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(property.MetadataToken), decompiler.TypeSystem);
		}

		public override void DecompileField(IField field, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile assembly = field.ParentModule!.MetadataFile!;
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			AddReferenceAssemblyWarningMessage(assembly, output);
			AddReferenceWarningMessage(assembly, output);
			WriteCommentLine(output, assembly.FullName);
			WriteCommentLine(output, TypeToString(field.DeclaringType));
			if (field.IsConst)
			{
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(field.MetadataToken), decompiler.TypeSystem);
			}
			else
			{
				var members = CollectFieldsAndCtors(field.DeclaringTypeDefinition!, field.IsStatic);
				var resolvedField = decompiler.TypeSystem.MainModule.GetDefinition((FieldDefinitionHandle)field.MetadataToken);
				decompiler.AstTransforms.Add(new SelectFieldTransform(resolvedField));
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(members), decompiler.TypeSystem);
			}
		}

		/// <summary>
		/// Decompiles a C# 14 explicit-extension declaration block back to source. Used by
		/// <see cref="TreeNodes.ExtensionTreeNode"/> when the user activates an extension
		/// container; the type-level overload is what the node actually calls. The method
		/// and property overloads exist so individual members can also be decompiled in
		/// isolation (e.g. when navigated to by analyzer results in a future commit).
		/// </summary>
		public void DecompileExtension(ITypeDefinition extension, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile assembly = extension.ParentModule!.MetadataFile!;
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			AddReferenceAssemblyWarningMessage(assembly, output);
			AddReferenceWarningMessage(assembly, output);
			WriteCommentLine(output, assembly.FullName);
			WriteCommentLine(output, TypeToString(extension,
				ConversionFlags.UseFullyQualifiedTypeNames | ConversionFlags.UseFullyQualifiedEntityNames | ConversionFlags.SupportExtensionDeclarations));
			WriteCode(output, options.DecompilerSettings, decompiler.DecompileExtension(extension.MetadataToken), decompiler.TypeSystem);
		}

		public void DecompileExtension(IMethod extension, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile assembly = extension.ParentModule!.MetadataFile!;
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			AddReferenceAssemblyWarningMessage(assembly, output);
			AddReferenceWarningMessage(assembly, output);
			WriteCommentLine(output, assembly.FullName);
			WriteCommentLine(output, TypeToString(extension.DeclaringType,
				ConversionFlags.UseFullyQualifiedTypeNames | ConversionFlags.UseFullyQualifiedEntityNames | ConversionFlags.SupportExtensionDeclarations));
			WriteCode(output, options.DecompilerSettings, decompiler.DecompileExtension(extension.MetadataToken), decompiler.TypeSystem);
		}

		public void DecompileExtension(IProperty extension, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile assembly = extension.ParentModule!.MetadataFile!;
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			AddReferenceAssemblyWarningMessage(assembly, output);
			AddReferenceWarningMessage(assembly, output);
			WriteCommentLine(output, assembly.FullName);
			WriteCommentLine(output, TypeToString(extension.DeclaringType,
				ConversionFlags.UseFullyQualifiedTypeNames | ConversionFlags.UseFullyQualifiedEntityNames | ConversionFlags.SupportExtensionDeclarations));
			WriteCode(output, options.DecompilerSettings, decompiler.DecompileExtension(extension.MetadataToken), decompiler.TypeSystem);
		}

		public override void DecompileEvent(IEvent ev, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile assembly = ev.ParentModule!.MetadataFile!;
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			AddReferenceAssemblyWarningMessage(assembly, output);
			AddReferenceWarningMessage(assembly, output);
			WriteCommentLine(output, assembly.FullName);
			WriteCommentLine(output, TypeToString(ev.DeclaringType));
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(ev.MetadataToken), decompiler.TypeSystem);
		}

		public override void DecompileType(ITypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			MetadataFile assembly = type.ParentModule!.MetadataFile!;
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			AddReferenceAssemblyWarningMessage(assembly, output);
			AddReferenceWarningMessage(assembly, output);
			WriteCommentLine(output, assembly.FullName);
			WriteCommentLine(output, TypeToString(type, ConversionFlags.UseFullyQualifiedTypeNames | ConversionFlags.UseFullyQualifiedEntityNames));
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(type.MetadataToken), decompiler.TypeSystem);
		}

		public override void DecompileNamespace(string nameSpace, IEnumerable<ITypeDefinition> types, ITextOutput output, DecompilationOptions options)
		{
			var typesByModule = types.GroupBy(t => t.ParentModule!.MetadataFile);
			bool first = true;
			foreach (var group in typesByModule)
			{
				if (!first)
					output.WriteLine();
				first = false;
				MetadataFile assembly = group.Key!;
				CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
				AddReferenceAssemblyWarningMessage(assembly, output);
				AddReferenceWarningMessage(assembly, output);
				WriteCommentLine(output, assembly.FullName);
				WriteCommentLine(output, nameSpace);
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(group.Select(t => t.MetadataToken)), decompiler.TypeSystem);
			}
		}

		public override ProjectId? DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			var module = assembly.GetMetadataFileOrNull();
			if (module == null)
				return null;
			if (options.FullDecompilation && options.SaveAsProjectDirectory != null)
				return DecompileAsProject(assembly, module, output, options);

			AddReferenceAssemblyWarningMessage(module, output);
			AddReferenceWarningMessage(module, output);
			output.WriteLine();
			base.DecompileAssembly(assembly, output, options);

			var assemblyResolver = assembly.GetAssemblyResolver(loadOnDemand: options.FullDecompilation && options.DecompilerSettings.AutoLoadAssemblyReferences);
			var typeSystem = new DecompilerTypeSystem(module, assemblyResolver, options.DecompilerSettings);
			var globalType = typeSystem.MainModule.TypeDefinitions.FirstOrDefault();
			if (globalType != null)
			{
				output.Write("// Global type: ");
				output.WriteReference(globalType, ILAmbience.EscapeName(globalType.FullName));
				output.WriteLine();
			}
			var metadata = module.Metadata;
			var corHeader = module.CorHeader;
			if (module is PEFile peFile && corHeader != null)
			{
				var entrypointHandle = MetadataTokenHelpers.EntityHandleOrNil(corHeader.EntryPointTokenOrRelativeVirtualAddress);
				if (!entrypointHandle.IsNil && entrypointHandle.Kind == HandleKind.MethodDefinition)
				{
					var entrypoint = typeSystem.MainModule.ResolveMethod(entrypointHandle, new GenericContext());
					if (entrypoint != null)
					{
						output.Write("// Entry point: ");
						output.WriteReference(entrypoint, ILAmbience.EscapeName(entrypoint.DeclaringType.FullName + "." + entrypoint.Name));
						output.WriteLine();
					}
				}
				output.WriteLine("// Architecture: " + GetPlatformDisplayName(peFile));
				if ((corHeader.Flags & CorFlags.ILOnly) == 0)
					output.WriteLine("// This assembly contains unmanaged code.");
				string runtimeName = GetRuntimeDisplayName(module);
				if (runtimeName != null)
					output.WriteLine("// Runtime: " + runtimeName);
				if ((corHeader.Flags & CorFlags.StrongNameSigned) != 0)
					output.WriteLine("// This assembly is signed with a strong name key.");
				if (peFile.Reader.ReadDebugDirectory().Any(d => d.Type == DebugDirectoryEntryType.Reproducible))
					output.WriteLine("// This assembly was compiled using the /deterministic option.");
				if (module.Metadata.MetadataKind != MetadataKind.Ecma335)
					output.WriteLine("// This assembly was loaded with Windows Runtime projections applied.");
			}
			else
			{
				string runtimeName = GetRuntimeDisplayName(module);
				if (runtimeName != null)
					output.WriteLine("// Runtime: " + runtimeName);
			}
			if (metadata.IsAssembly)
			{
				var asm = metadata.GetAssemblyDefinition();
				if (asm.HashAlgorithm != System.Reflection.AssemblyHashAlgorithm.None)
					output.WriteLine("// Hash algorithm: " + asm.HashAlgorithm.ToString().ToUpper());
				if (!asm.PublicKey.IsNil)
				{
					output.Write("// Public key: ");
					var reader = metadata.GetBlobReader(asm.PublicKey);
					while (reader.RemainingBytes > 0)
						output.Write(reader.ReadByte().ToString("x2"));
					output.WriteLine();
				}
			}
			var debugInfo = assembly.GetDebugInfoOrNull();
			if (debugInfo != null)
				output.WriteLine("// Debug info: " + debugInfo.Description);
			output.WriteLine();

			var decompiler = new CSharpDecompiler(typeSystem, options.DecompilerSettings) {
				CancellationToken = options.CancellationToken,
			};
			if (options.EscapeInvalidIdentifiers)
				decompiler.AstTransforms.Add(new EscapeInvalidIdentifiers());
			SyntaxTree st = options.FullDecompilation
				? decompiler.DecompileWholeModuleAsSingleFile()
				: decompiler.DecompileModuleAndAssemblyAttributes();
			WriteCode(output, options.DecompilerSettings, st, decompiler.TypeSystem);
			return null;
		}

		/// <summary>
		/// Runs <see cref="WholeProjectDecompiler"/> against the loaded assembly and emits a
		/// buildable .csproj + per-type .cs files under
		/// <see cref="DecompilationOptions.SaveAsProjectDirectory"/>. The <paramref name="output"/>
		/// ITextOutput is the buffer the caller will display when the export finishes — we
		/// write a short summary into it so the user gets feedback in the editor tab. The
		/// project file's name is derived from the assembly's <see cref="MetadataFile.Name"/>
		/// via <see cref="WholeProjectDecompiler.CleanUpFileName"/>.
		/// </summary>
		ProjectId? DecompileAsProject(LoadedAssembly assembly, MetadataFile module, ITextOutput output, DecompilationOptions options)
		{
			var targetDirectory = options.SaveAsProjectDirectory!;
			var resolver = assembly.GetAssemblyResolver(loadOnDemand: options.DecompilerSettings.AutoLoadAssemblyReferences);
			var debugInfo = assembly.GetDebugInfoOrNull();
			var decompiler = new ResourceHandlerProjectDecompiler(
				assembly,
				options,
				options.DecompilerSettings,
				resolver,
				projectWriter: null,
				assemblyReferenceClassifier: null,
				debugInfoProvider: debugInfo);
			var projectFileName = System.IO.Path.Combine(
				targetDirectory,
				WholeProjectDecompiler.CleanUpFileName(module.Name, ProjectFileExtension));
			ProjectId? id;
			using (var writer = new System.IO.StreamWriter(projectFileName))
				id = decompiler.DecompileProject(module, targetDirectory, writer, options.CancellationToken);
			output.WriteLine("// Project written to " + targetDirectory);
			return id;
		}

		/// <summary>
		/// <see cref="WholeProjectDecompiler"/> subclass that delegates resource entries to
		/// MEF-discovered <see cref="IResourceFileHandler"/> implementations. The first
		/// handler whose <c>CanHandle</c> returns true wins; its emitted file plus any
		/// partial-type info or extra MSBuild properties land in the produced .csproj.
		/// Falls through to <see cref="WholeProjectDecompiler.WriteResourceToFile"/>'s
		/// default "raw bytes as embedded resource" behaviour when no handler claims it.
		/// </summary>
		sealed class ResourceHandlerProjectDecompiler : WholeProjectDecompiler
		{
			readonly LoadedAssembly assembly;
			readonly DecompilationOptions options;
			static readonly IReadOnlyList<IResourceFileHandler> handlers = TryDiscoverHandlers();

			public ResourceHandlerProjectDecompiler(
				LoadedAssembly assembly,
				DecompilationOptions options,
				DecompilerSettings settings,
				IAssemblyResolver resolver,
				IProjectFileWriter? projectWriter,
				AssemblyReferenceClassifier? assemblyReferenceClassifier,
				ICSharpCode.Decompiler.DebugInfo.IDebugInfoProvider? debugInfoProvider)
				: base(settings, resolver, projectWriter!, assemblyReferenceClassifier!, debugInfoProvider!)
			{
				this.assembly = assembly;
				this.options = options;
			}

			protected override IEnumerable<ProjectItemInfo> WriteResourceToFile(string fileName, string resourceName, Stream entryStream)
			{
				var context = new ResourceFileHandlerContext(options);
				foreach (var handler in handlers)
				{
					if (!handler.CanHandle(fileName, context))
						continue;
					entryStream.Position = 0;
					fileName = handler.WriteResourceToFile(assembly, fileName, entryStream, context);
					var item = new ProjectItemInfo(handler.EntryType, fileName) { PartialTypes = context.PartialTypes };
					foreach (var (k, v) in context.AdditionalProperties)
						item.AdditionalProperties.Add(k, v);
					return new[] { item };
				}
				return base.WriteResourceToFile(fileName, resourceName, entryStream);
			}

			static IReadOnlyList<IResourceFileHandler> TryDiscoverHandlers()
			{
				try
				{
					return AppEnv.AppComposition.Current.GetExports<IResourceFileHandler>().ToArray();
				}
				catch
				{
					// Composition isn't available in tests that bypass the host (e.g. invoking
					// DecompileAsProject directly with a self-built LoadedAssembly). Fall back
					// to the raw-bytes behaviour from the base class.
					return System.Array.Empty<IResourceFileHandler>();
				}
			}
		}

		static List<EntityHandle> CollectFieldsAndCtors(ITypeDefinition type, bool isStatic)
		{
			var members = new List<EntityHandle>();
			foreach (var field in type.Fields)
				if (!field.MetadataToken.IsNil && field.IsStatic == isStatic)
					members.Add(field.MetadataToken);
			foreach (var e in type.Events)
				if (!e.MetadataToken.IsNil && e.IsStatic == isStatic)
					members.Add(e.MetadataToken);
			foreach (var p in type.Properties)
				if (!p.MetadataToken.IsNil && p.IsStatic == isStatic && !p.IsIndexer)
					members.Add(p.MetadataToken);
			foreach (var ctor in type.Methods)
				if (!ctor.MetadataToken.IsNil && ctor.IsConstructor && ctor.IsStatic == isStatic)
					members.Add(ctor.MetadataToken);
			return members;
		}

		sealed class SelectCtorTransform(IMethod ctor) : IAstTransform
		{
			readonly HashSet<ISymbol?> removedSymbols = new();

			public void Run(AstNode rootNode, TransformContext context)
			{
				ConstructorDeclaration? ctorDecl = null;
				foreach (var node in rootNode.Children)
				{
					switch (node)
					{
						case ConstructorDeclaration cd:
							if (cd.GetSymbol() == ctor)
								ctorDecl = cd;
							else
							{
								cd.Remove();
								removedSymbols.Add(cd.GetSymbol());
							}
							break;
						case FieldDeclaration fd:
							if (fd.Variables.All(v => v.Initializer.IsNull))
							{
								fd.Remove();
								removedSymbols.Add(fd.GetSymbol());
							}
							break;
						case EventDeclaration ed:
							if (ed.Variables.All(v => v.Initializer.IsNull))
							{
								ed.Remove();
								removedSymbols.Add(ed.GetSymbol());
							}
							break;
						case PropertyDeclaration pd:
							if (pd.Initializer.IsNull)
							{
								pd.Remove();
								removedSymbols.Add(pd.GetSymbol());
							}
							break;
						case CustomEventDeclaration:
						case IndexerDeclaration:
							node.Remove();
							removedSymbols.Add(node.GetSymbol());
							break;
					}
				}
				if (ctorDecl?.Initializer.ConstructorInitializerType == ConstructorInitializerType.This)
				{
					foreach (var node in rootNode.Children)
					{
						if (node is not ConstructorDeclaration)
						{
							node.Remove();
							removedSymbols.Add(node.GetSymbol());
						}
					}
				}
				foreach (var node in rootNode.Children)
				{
					if (node is Comment && removedSymbols.Contains(node.GetSymbol()))
						node.Remove();
				}
			}
		}

		sealed class SelectFieldTransform(IField field) : IAstTransform
		{
			public void Run(AstNode rootNode, TransformContext context)
			{
				foreach (var node in rootNode.Children)
				{
					switch (node)
					{
						case EntityDeclaration:
							if (node.GetSymbol() != field)
								node.Remove();
							break;
						case Comment c:
							if (c.GetSymbol() != field)
								node.Remove();
							break;
					}
				}
			}
		}

		static void WriteCode(ITextOutput output, DecompilerSettings settings, SyntaxTree syntaxTree, IDecompilerTypeSystem typeSystem)
		{
			syntaxTree.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
			output.IndentationString = settings.CSharpFormattingOptions.IndentationString;
			TokenWriter tokenWriter = new TextTokenWriter(output, settings, typeSystem);
			if (output is TextView.ISmartTextOutput smartOutput)
				tokenWriter = new CSharpHighlightingTokenWriter(tokenWriter, smartOutput);
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
		}

		void AddWarningMessage(MetadataFile module, ITextOutput output, string line1, string? line2 = null,
			string? buttonText = null, global::Avalonia.Media.IImage? buttonImage = null,
			System.EventHandler<global::Avalonia.Interactivity.RoutedEventArgs>? buttonClickHandler = null)
		{
			if (output is TextView.ISmartTextOutput fancyOutput)
			{
				string text = line1;
				if (!string.IsNullOrEmpty(line2))
					text += System.Environment.NewLine + line2;
				fancyOutput.AddUIElement(() => new global::Avalonia.Controls.StackPanel {
					Margin = new global::Avalonia.Thickness(5),
					Orientation = global::Avalonia.Layout.Orientation.Horizontal,
					Children = {
						new global::Avalonia.Controls.Image {
							Width = 32,
							Height = 32,
							Source = Images.Images.Warning,
						},
						new global::Avalonia.Controls.TextBlock {
							Margin = new global::Avalonia.Thickness(5, 0, 0, 0),
							Text = text,
						},
					},
				});
				fancyOutput.WriteLine();
				if (buttonText != null && buttonClickHandler != null)
				{
					fancyOutput.AddButton(buttonImage, buttonText, buttonClickHandler);
					fancyOutput.WriteLine();
				}
			}
			else
			{
				WriteCommentLine(output, line1);
				if (!string.IsNullOrEmpty(line2))
					WriteCommentLine(output, line2);
			}
		}

		void AddReferenceAssemblyWarningMessage(MetadataFile module, ITextOutput output)
		{
			var metadata = module.Metadata;
			if (!metadata.GetCustomAttributes(Handle.AssemblyDefinition).HasKnownAttribute(metadata, KnownAttribute.ReferenceAssembly))
				return;
			AddWarningMessage(module, output, Resources.WarningAsmMarkedRef);
		}

		void AddReferenceWarningMessage(MetadataFile module, ITextOutput output)
		{
			// Resolving AssemblyTreeModel via composition would create a circular registration
			// (LanguageService → Language → AssemblyTreeModel → LanguageService). Match the WPF
			// output by looking the assembly up directly off the module — same predicate (the
			// metadata file equality), no service dependency.
			if (!HasReferenceErrors(module))
				return;
			AddWarningMessage(module, output,
				Resources.WarningSomeAssemblyReference,
				Resources.PropertyManuallyMissingReferencesListLoadedAssemblies);
		}

		static bool HasReferenceErrors(MetadataFile module)
		{
			try
			{
				var atm = AppEnv.AppComposition.Current.GetExport<AssemblyTree.AssemblyTreeModel>();
				var loadedAssembly = atm.AssemblyList?.GetAssemblies()
					.FirstOrDefault(la => la.GetMetadataFileOrNull() == module);
				return loadedAssembly?.LoadedAssemblyReferencesInfo.HasErrors == true;
			}
			catch
			{
				return false;
			}
		}
	}
}
