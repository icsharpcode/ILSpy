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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.Decompiler.Metadata;
using System.Reflection.Metadata;
using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.Decompiler.Util;
using System.Reflection;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// C# decompiler integration into ILSpy.
	/// Note: if you're interested in using the decompiler without the ILSpy UI,
	/// please directly use the CSharpDecompiler class.
	/// </summary>
	[Export(typeof(Language))]
	public class CSharpLanguage : Language
	{
		string name = "C#";
		bool showAllMembers = false;
		int transformCount = int.MaxValue;

#if DEBUG
		internal static IEnumerable<CSharpLanguage> GetDebugLanguages()
		{
			string lastTransformName = "no transforms";
			int transformCount = 0;
			foreach (var transform in CSharpDecompiler.GetAstTransforms()) {
				yield return new CSharpLanguage {
					transformCount = transformCount,
					name = "C# - " + lastTransformName,
					showAllMembers = true
				};
				lastTransformName = "after " + transform.GetType().Name;
				transformCount++;
			}
			yield return new CSharpLanguage {
				name = "C# - " + lastTransformName,
				showAllMembers = true
			};
		}
#endif

		public override string Name {
			get { return name; }
		}

		public override string FileExtension {
			get { return ".cs"; }
		}

		public override string ProjectFileExtension {
			get { return ".csproj"; }
		}

		IReadOnlyList<LanguageVersion> versions;

		public override IReadOnlyList<LanguageVersion> LanguageVersions {
			get {
				if (versions == null) {
					versions = new List<LanguageVersion>() {
						new LanguageVersion(Decompiler.CSharp.LanguageVersion.CSharp1.ToString(), "C# 1.0 / VS .NET"),
						new LanguageVersion(Decompiler.CSharp.LanguageVersion.CSharp2.ToString(), "C# 2.0 / VS 2005"),
						new LanguageVersion(Decompiler.CSharp.LanguageVersion.CSharp3.ToString(), "C# 3.0 / VS 2008"),
						new LanguageVersion(Decompiler.CSharp.LanguageVersion.CSharp4.ToString(), "C# 4.0 / VS 2010"),
						new LanguageVersion(Decompiler.CSharp.LanguageVersion.CSharp5.ToString(), "C# 5.0 / VS 2012"),
						new LanguageVersion(Decompiler.CSharp.LanguageVersion.CSharp6.ToString(), "C# 6.0 / VS 2015"),
						new LanguageVersion(Decompiler.CSharp.LanguageVersion.CSharp7.ToString(), "C# 7.0 / VS 2017"),
						new LanguageVersion(Decompiler.CSharp.LanguageVersion.CSharp7_1.ToString(), "C# 7.1 / VS 2017.3"),
						new LanguageVersion(Decompiler.CSharp.LanguageVersion.CSharp7_2.ToString(), "C# 7.2 / VS 2017.4"),
					};
				}
				return versions;
			}
		}

		CSharpDecompiler CreateDecompiler(Decompiler.Metadata.PEFile module, DecompilationOptions options)
		{
			CSharpDecompiler decompiler = new CSharpDecompiler(module, options.DecompilerSettings);
			decompiler.CancellationToken = options.CancellationToken;
			while (decompiler.AstTransforms.Count > transformCount)
				decompiler.AstTransforms.RemoveAt(decompiler.AstTransforms.Count - 1);
			return decompiler;
		}

		void WriteCode(ITextOutput output, Decompiler.DecompilerSettings settings, SyntaxTree syntaxTree, IDecompilerTypeSystem typeSystem)
		{
			syntaxTree.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
			TokenWriter tokenWriter = new TextTokenWriter(output, settings, typeSystem) { FoldBraces = settings.FoldBraces, ExpandMemberDefinitions = settings.ExpandMemberDefinitions };
			if (output is ISmartTextOutput highlightingOutput) {
				tokenWriter = new CSharpHighlightingTokenWriter(tokenWriter, highlightingOutput);
			}
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
		}

		public override void DecompileMethod(Decompiler.Metadata.MethodDefinition method, ITextOutput output, DecompilationOptions options)
		{
			AddReferenceWarningMessage(method.Module, output);
			var md = method.This();
			WriteCommentLine(output, TypeDefinitionToString(new Entity(method.Module, md.GetDeclaringType()), includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(method.Module, options);
			var methodDefinition = decompiler.TypeSystem.ResolveAsMethod(method.Handle);
			if (methodDefinition.IsConstructor && methodDefinition.DeclaringType.IsReferenceType != false) {
				var members = CollectFieldsAndCtors(methodDefinition.DeclaringTypeDefinition, methodDefinition.IsStatic);
				decompiler.AstTransforms.Add(new SelectCtorTransform(methodDefinition));
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(members), decompiler.TypeSystem);
			} else {
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(method.Handle), decompiler.TypeSystem);
			}
		}

		class SelectCtorTransform : IAstTransform
		{
			readonly IMethod ctor;
			readonly HashSet<ISymbol> removedSymbols = new HashSet<ISymbol>();

			public SelectCtorTransform(IMethod ctor)
			{
				this.ctor = ctor;
			}

			public void Run(AstNode rootNode, TransformContext context)
			{
				ConstructorDeclaration ctorDecl = null;
				foreach (var node in rootNode.Children) {
					switch (node) {
						case ConstructorDeclaration ctor:
							if (ctor.GetSymbol() == this.ctor) {
								ctorDecl = ctor;
							} else {
								// remove other ctors
								ctor.Remove();
								removedSymbols.Add(ctor.GetSymbol());
							}
							break;
						case FieldDeclaration fd:
							// Remove any fields without initializers
							if (fd.Variables.All(v => v.Initializer.IsNull)) {
								fd.Remove();
								removedSymbols.Add(fd.GetSymbol());
							}
							break;
					}
				}
				if (ctorDecl?.Initializer.ConstructorInitializerType == ConstructorInitializerType.This) {
					// remove all fields
					foreach (var node in rootNode.Children) {
						switch (node) {
							case FieldDeclaration fd:
								fd.Remove();
								removedSymbols.Add(fd.GetSymbol());
								break;
						}
					}
				}
				foreach (var node in rootNode.Children) {
					if (node is Comment && removedSymbols.Contains(node.GetSymbol()))
						node.Remove();
				}
			}
		}

		public override void DecompileProperty(Decompiler.Metadata.PropertyDefinition property, ITextOutput output, DecompilationOptions options)
		{
			AddReferenceWarningMessage(property.Module, output);
			CSharpDecompiler decompiler = CreateDecompiler(property.Module, options);
			var metadata = property.Module.GetMetadataReader();
			var accessorHandle = metadata.GetPropertyDefinition(property.Handle).GetAccessors().GetAny();
			WriteCommentLine(output, TypeDefinitionToString(new Decompiler.Metadata.TypeDefinition(property.Module, metadata.GetMethodDefinition(accessorHandle).GetDeclaringType()), includeNamespace: true));
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(property.Handle), decompiler.TypeSystem);
		}

		public override void DecompileField(Decompiler.Metadata.FieldDefinition field, ITextOutput output, DecompilationOptions options)
		{
			AddReferenceWarningMessage(field.Module, output);
			var fd = field.This();
			WriteCommentLine(output, TypeDefinitionToString(new Decompiler.Metadata.TypeDefinition(field.Module, fd.GetDeclaringType()), includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(field.Module, options);
			var fieldDefinition = decompiler.TypeSystem.ResolveAsField(field.Handle);
			if (fd.HasFlag(FieldAttributes.Literal)) {
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(field.Handle), decompiler.TypeSystem);
			} else {
				var members = CollectFieldsAndCtors(fieldDefinition.DeclaringTypeDefinition, fieldDefinition.IsStatic);
				decompiler.AstTransforms.Add(new SelectFieldTransform(fieldDefinition));
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(members), decompiler.TypeSystem);
			}
		}

		static List<EntityHandle> CollectFieldsAndCtors(ITypeDefinition type, bool isStatic)
		{
			var members = new List<EntityHandle>();
			foreach (var field in type.Fields) {
				if (field.IsStatic == isStatic)
					members.Add(field.MetadataToken);
			}
			foreach (var ctor in type.Methods) {
				if (ctor.IsConstructor && ctor.IsStatic == isStatic)
					members.Add(ctor.MetadataToken);
			}

			return members;
		}

		/// <summary>
		/// Removes all top-level members except for the specified fields.
		/// </summary>
		sealed class SelectFieldTransform : IAstTransform
		{
			readonly IField field;

			public SelectFieldTransform(IField field)
			{
				this.field = field;
			}

			public void Run(AstNode rootNode, TransformContext context)
			{
				foreach (var node in rootNode.Children) {
					switch (node) {
						case EntityDeclaration ed:
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

		public override void DecompileEvent(Decompiler.Metadata.EventDefinition ev, ITextOutput output, DecompilationOptions options)
		{
			AddReferenceWarningMessage(ev.Module, output);
			var metadata = ev.Module.GetMetadataReader();
			var accessorHandle = metadata.GetEventDefinition(ev.Handle).GetAccessors().GetAny();
			base.WriteCommentLine(output, TypeDefinitionToString(new Decompiler.Metadata.TypeDefinition(ev.Module, metadata.GetMethodDefinition(accessorHandle).GetDeclaringType()), includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(ev.Module, options);
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(ev.Handle), decompiler.TypeSystem);
		}

		public override void DecompileType(Decompiler.Metadata.TypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			AddReferenceWarningMessage(type.Module, output);
			WriteCommentLine(output, TypeDefinitionToString(type, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(type.Module, options);
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(type.Handle), decompiler.TypeSystem);
		}

		void AddReferenceWarningMessage(Decompiler.Metadata.PEFile assembly, ITextOutput output)
		{
			var loadedAssembly = MainWindow.Instance.CurrentAssemblyList.GetAssemblies().FirstOrDefault(la => la.GetPEFileOrNull() == assembly);
			if (loadedAssembly == null || !loadedAssembly.LoadedAssemblyReferencesInfo.HasErrors)
				return;
			const string line1 = "Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,";
			const string line2 = "for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.";
			if (output is ISmartTextOutput fancyOutput) {
				fancyOutput.AddUIElement(() => new StackPanel {
					Margin = new Thickness(5),
					Orientation = Orientation.Horizontal,
					Children = {
						new Image {
							Width = 32,
							Height = 32,
							Source = Images.LoadImage(this, "Images/Warning.png")
						},
						new TextBlock {
							Margin = new Thickness(5, 0, 0, 0),
							Text = line1 + Environment.NewLine + line2
						}
					}
				});
				fancyOutput.WriteLine();
				fancyOutput.AddButton(Images.ViewCode, "Show assembly load log", delegate {
					MainWindow.Instance.SelectNode(MainWindow.Instance.FindTreeNode(assembly).Children.OfType<ReferenceFolderTreeNode>().First());
				});
				fancyOutput.WriteLine();
			} else {
				WriteCommentLine(output, line1);
				WriteCommentLine(output, line2);
			}
		}

		public override void DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			var module = assembly.GetPEFileAsync().Result;
			if (options.FullDecompilation && options.SaveAsProjectDirectory != null) {
				var decompiler = new ILSpyWholeProjectDecompiler(assembly, options);
				decompiler.DecompileProject(module, options.SaveAsProjectDirectory, new TextOutputWriter(output), options.CancellationToken);
			} else {
				AddReferenceWarningMessage(module, output);
				output.WriteLine();
				base.DecompileAssembly(assembly, output, options);
				var metadata = module.GetMetadataReader();
				
				if (metadata.TypeDefinitions.Count > 0) {
					output.Write("// Global type: ");
					var globalType = metadata.TypeDefinitions.First();
					output.WriteReference(globalType.GetFullTypeName(metadata).ToString(), new Decompiler.Metadata.TypeDefinition(module, globalType));
					output.WriteLine();
				}
				var corHeader = module.Reader.PEHeaders.CorHeader;
				var entrypointHandle = MetadataTokens.MethodDefinitionHandle(corHeader.EntryPointTokenOrRelativeVirtualAddress);
				if (!entrypointHandle.IsNil) {
					var entrypoint = metadata.GetMethodDefinition(entrypointHandle);
					output.Write("// Entry point: ");
					output.WriteReference(entrypoint.GetDeclaringType().GetFullTypeName(metadata) + "." + metadata.GetString(entrypoint.Name), new Decompiler.Metadata.MethodDefinition(module, entrypointHandle));
					output.WriteLine();
				}
				output.WriteLine("// Architecture: " + GetPlatformDisplayName(module));
				if ((corHeader.Flags & System.Reflection.PortableExecutable.CorFlags.ILOnly) == 0) {
					output.WriteLine("// This assembly contains unmanaged code.");
				}
				string runtimeName = GetRuntimeDisplayName(module);
				if (runtimeName != null) {
					output.WriteLine("// Runtime: " + runtimeName);
				}
				output.WriteLine();

				// don't automatically load additional assemblies when an assembly node is selected in the tree view
				using (options.FullDecompilation ? null : LoadedAssembly.DisableAssemblyLoad()) {
					CSharpDecompiler decompiler = new CSharpDecompiler(module, options.DecompilerSettings);
					decompiler.CancellationToken = options.CancellationToken;
					SyntaxTree st;
					if (options.FullDecompilation) {
						st = decompiler.DecompileWholeModuleAsSingleFile();
					} else {
						st = decompiler.DecompileModuleAndAssemblyAttributes();
					}
					WriteCode(output, options.DecompilerSettings, st, decompiler.TypeSystem);
				}
			}
		}

		class ILSpyWholeProjectDecompiler : WholeProjectDecompiler
		{
			readonly LoadedAssembly assembly;
			readonly DecompilationOptions options;

			public ILSpyWholeProjectDecompiler(LoadedAssembly assembly, DecompilationOptions options)
			{
				this.assembly = assembly;
				this.options = options;
				base.Settings = options.DecompilerSettings;
			}

			protected override IEnumerable<Tuple<string, string>> WriteResourceToFile(string fileName, string resourceName, Stream entryStream)
			{
				if (fileName.EndsWith(".resource", StringComparison.OrdinalIgnoreCase)) {
					using (ResourceReader reader = new ResourceReader(entryStream))
					using (FileStream fs = new FileStream(Path.Combine(targetDirectory, fileName), FileMode.Create, FileAccess.Write))
					using (ResXResourceWriter writer = new ResXResourceWriter(fs)) {
						foreach (DictionaryEntry entry in reader) {
							writer.AddResource((string)entry.Key, entry.Value);
						}
					}
					return new[] { Tuple.Create("EmbeddedResource", fileName) };
				}
				foreach (var handler in App.ExportProvider.GetExportedValues<IResourceFileHandler>()) {
					if (handler.CanHandle(fileName, options)) {
						entryStream.Position = 0;
						return new[] { Tuple.Create(handler.EntryType, handler.WriteResourceToFile(assembly, fileName, entryStream, options)) };
					}
				}
				return base.WriteResourceToFile(fileName, resourceName, entryStream);
			}
		}

		public override string TypeDefinitionToString(Decompiler.Metadata.TypeDefinition type, bool includeNamespace)
		{
			ConvertTypeOptions options = ConvertTypeOptions.IncludeTypeParameterDefinitions;
			if (includeNamespace)
				options |= ConvertTypeOptions.IncludeNamespace;

			var metadata = type.Module.GetMetadataReader();
			var provider = new AstTypeBuilder(options);

			AstType astType = provider.GetTypeFromDefinition(metadata, type.Handle, 0);
			return TypeToString(astType, metadata, null);
		}

		public override string FieldToString(Decompiler.Metadata.FieldDefinition field, bool includeTypeName, bool includeNamespace)
		{
			if (field.Handle.IsNil)
				throw new ArgumentNullException(nameof(field));
			var metadata = field.Module.GetMetadataReader();
			var fd = metadata.GetFieldDefinition(field.Handle);
			AstType fieldType = fd.DecodeSignature(new AstTypeBuilder(ConvertTypeOptions.IncludeTypeParameterDefinitions), new GenericContext(fd.GetDeclaringType(), field.Module));
			string simple = metadata.GetString(fd.Name) + " : " + TypeToString(fieldType, metadata, fd.GetCustomAttributes());
			if (!includeTypeName)
				return simple;
			var typeName = fd.GetDeclaringType().GetFullTypeName(metadata);
			if (!includeNamespace)
				return typeName.Name + "." + simple;
			return typeName + "." + simple;
		}

		public override string PropertyToString(Decompiler.Metadata.PropertyDefinition property, bool includeTypeName, bool includeNamespace, bool? isIndexer = null)
		{
			if (property.IsNil)
				throw new ArgumentNullException(nameof(property));
			var metadata = property.Module.GetMetadataReader();
			var pd = metadata.GetPropertyDefinition(property.Handle);
			var accessors = pd.GetAccessors();
			var accessorHandle = accessors.GetAny();
			var accessor = metadata.GetMethodDefinition(accessorHandle);
			var declaringType = metadata.GetTypeDefinition(accessor.GetDeclaringType());
			if (!isIndexer.HasValue) {
				isIndexer = accessor.GetDeclaringType().GetDefaultMemberName(metadata) != null;
			}
			var buffer = new System.Text.StringBuilder();
			if (isIndexer.Value) {
				var overrides = accessorHandle.GetMethodImplementations(metadata);
				if (overrides.Any()) {
					string name = metadata.GetString(pd.Name);
					int index = name.LastIndexOf('.');
					if (index > 0) {
						buffer.Append(name.Substring(0, index));
						buffer.Append(@".");
					}
				}
				buffer.Append(@"this[");
				var signature = pd.DecodeSignature(new AstTypeBuilder(ConvertTypeOptions.IncludeTypeParameterDefinitions), new GenericContext(accessorHandle, property.Module));

				var parameterHandles = accessor.GetParameters();

				int i = 0;
				CustomAttributeHandleCollection? returnTypeAttributes = null;
				if (signature.RequiredParameterCount > parameterHandles.Count) {
					foreach (var type in signature.ParameterTypes) {
						if (i > 0)
							buffer.Append(", ");
						buffer.Append(TypeToString(signature.ParameterTypes[i], metadata, null));
						i++;
					}
				} else {
					foreach (var h in parameterHandles) {
						var p = metadata.GetParameter(h);
						if (p.SequenceNumber > 0 && i < signature.ParameterTypes.Length) {
							if (i > 0)
								buffer.Append(", ");
							buffer.Append(TypeToString(signature.ParameterTypes[i], metadata, p.GetCustomAttributes(), h));
							i++;
						}
						if (p.SequenceNumber == 0) {
							returnTypeAttributes = p.GetCustomAttributes();
						}
					}
				}
				if (signature.Header.CallingConvention == SignatureCallingConvention.VarArgs) {
					if (signature.ParameterTypes.Length > 0)
						buffer.Append(", ");
					buffer.Append("...");
				}

				buffer.Append(@"]");
				buffer.Append(" : ");
				buffer.Append(TypeToString(signature.ReturnType, metadata, returnTypeAttributes));
			} else {
				var signature = pd.DecodeSignature(new AstTypeBuilder(ConvertTypeOptions.IncludeTypeParameterDefinitions), new GenericContext(accessorHandle, property.Module));

				var parameterHandles = accessor.GetParameters();

				CustomAttributeHandleCollection? returnTypeAttributes = null;
				if (parameterHandles.Count > 0) {
					var p = metadata.GetParameter(parameterHandles.First());
					if (p.SequenceNumber == 0) {
						returnTypeAttributes = p.GetCustomAttributes();
					}
				}
				buffer.Append(metadata.GetString(pd.Name));
				buffer.Append(" : ");
				buffer.Append(TypeToString(signature.ReturnType, metadata, returnTypeAttributes));
			}
			return buffer.ToString();
		}

		static readonly CSharpFormattingOptions TypeToStringFormattingOptions = FormattingOptionsFactory.CreateEmpty();

		string TypeToString(AstType astType, MetadataReader metadata, CustomAttributeHandleCollection? customAttributes, ParameterHandle paramHandle = default)
		{
			StringWriter w = new StringWriter();

			if (astType is ComposedType ct && ct.HasRefSpecifier) {
				if (!paramHandle.IsNil) {
					var p = metadata.GetParameter(paramHandle);
					if ((p.Attributes & ParameterAttributes.In) == 0 && (p.Attributes & ParameterAttributes.Out) != 0) {
						w.Write("out ");
					} else {
						w.Write("ref ");
					}
				} else {
					w.Write("ref ");
				}

				astType = ct.BaseType;
				astType.Remove();
			}

			var st = new SyntaxTree();
			st.AddChild(astType, Roles.Type);
			st.AcceptVisitor(new InsertDynamicTypeVisitor(metadata, customAttributes));
			st.FirstChild.AcceptVisitor(new CSharpOutputVisitor(w, TypeToStringFormattingOptions));
			return w.ToString();
		}

		public override string MethodToString(Decompiler.Metadata.MethodDefinition method, bool includeTypeName, bool includeNamespace)
		{
			if (method.IsNil)
				throw new ArgumentNullException("method");
			var metadata = method.Module.GetMetadataReader();
			var md = metadata.GetMethodDefinition(method.Handle);
			var name = (md.IsConstructor(metadata)) ? TypeDefinitionToString(new Decompiler.Metadata.TypeDefinition(method.Module, md.GetDeclaringType()), includeNamespace) : metadata.GetString(md.Name);
			var signature = md.DecodeSignature(new AstTypeBuilder(ConvertTypeOptions.IncludeTypeParameterDefinitions), new GenericContext(method));

			int i = 0;
			var buffer = new System.Text.StringBuilder(name);
			var genericParams = md.GetGenericParameters();
			if (genericParams.Count > 0) {
				buffer.Append('<');
				foreach (var h in genericParams) {
					var gp = metadata.GetGenericParameter(h);
					if (i > 0)
						buffer.Append(", ");
					buffer.Append(metadata.GetString(gp.Name));
					i++;
				}
				buffer.Append('>');
			}
			buffer.Append('(');

			i = 0;
			var parameterHandles = md.GetParameters();
			CustomAttributeHandleCollection? returnTypeAttributes = null;
			if (signature.RequiredParameterCount > parameterHandles.Count) {
				foreach (var type in signature.ParameterTypes) {
					if (i > 0)
						buffer.Append(", ");
					buffer.Append(TypeToString(signature.ParameterTypes[i], metadata, null));
					i++;
				}
			} else {
				foreach (var h in parameterHandles) {
					var p = metadata.GetParameter(h);
					if (p.SequenceNumber > 0 && i < signature.ParameterTypes.Length) {
						if (i > 0)
							buffer.Append(", ");
						buffer.Append(TypeToString(signature.ParameterTypes[i], metadata, p.GetCustomAttributes(), h));
						i++;
					}
					if (p.SequenceNumber == 0) {
						returnTypeAttributes = p.GetCustomAttributes();
					}
				}
			}
			if (signature.Header.CallingConvention == SignatureCallingConvention.VarArgs) {
				if (signature.ParameterTypes.Length > 0)
					buffer.Append(", ");
				buffer.Append("...");
			}
			buffer.Append(')');
			buffer.Append(" : ");
			buffer.Append(TypeToString(signature.ReturnType, metadata, returnTypeAttributes));
			return buffer.ToString();
		}

		public override string EventToString(Decompiler.Metadata.EventDefinition @event, bool includeTypeName, bool includeNamespace)
		{
			if (@event.IsNil)
				throw new ArgumentNullException(nameof(@event));
			var metadata = @event.Module.GetMetadataReader();
			var ed = metadata.GetEventDefinition(@event.Handle);
			var accessors = ed.GetAccessors();
			var accessorHandle = accessors.GetAny();
			var accessor = metadata.GetMethodDefinition(accessorHandle);
			var declaringType = metadata.GetTypeDefinition(accessor.GetDeclaringType());
			var signature = ed.DecodeSignature(metadata, new AstTypeBuilder(ConvertTypeOptions.IncludeTypeParameterDefinitions), new GenericContext(accessorHandle, @event.Module));

			var parameterHandles = accessor.GetParameters();
			CustomAttributeHandleCollection? returnTypeAttributes = null;
			if (parameterHandles.Count > 0) {
				var p = metadata.GetParameter(parameterHandles.First());
				if (p.SequenceNumber == 0) {
					returnTypeAttributes = p.GetCustomAttributes();
				}
			}
			var buffer = new System.Text.StringBuilder();
			buffer.Append(metadata.GetString(ed.Name));
			buffer.Append(" : ");
			buffer.Append(TypeToString(signature, metadata, returnTypeAttributes));
			return buffer.ToString();
		}

		public override bool ShowMember(IMetadataEntity member)
		{
			return showAllMembers || !CSharpDecompiler.MemberIsHidden(member.Module, member.Handle, new DecompilationOptions().DecompilerSettings);
		}

		/*
		public override IMetadataEntity GetOriginalCodeLocation(IMetadataEntity member)
		{
			if (showAllMembers || !new DecompilationOptions().DecompilerSettings.AnonymousMethods)
				return member;
			else
				return TreeNodes.Analyzer.Helpers.GetOriginalCodeLocation(member);
		}
		*/

		public override string GetTooltip(Entity entity)
		{
			var decompilerTypeSystem = new DecompilerTypeSystem(entity.Module);
			ISymbol symbol;
			switch (entity.Handle.Kind) {
				case HandleKind.MethodDefinition:
					symbol = decompilerTypeSystem.ResolveAsMethod(entity.Handle);
					if (symbol == null) return base.GetTooltip(entity);
					break;
				case HandleKind.PropertyDefinition:
					symbol = decompilerTypeSystem.ResolveAsProperty(entity.Handle);
					if (symbol == null) return base.GetTooltip(entity);
					break;
				case HandleKind.EventDefinition:
					symbol = decompilerTypeSystem.ResolveAsEvent(entity.Handle);
					if (symbol == null) return base.GetTooltip(entity);
					break;
				case HandleKind.FieldDefinition:
					symbol = decompilerTypeSystem.ResolveAsField(entity.Handle);
					if (symbol == null) return base.GetTooltip(entity);
					break;
				case HandleKind.TypeDefinition:
					symbol = decompilerTypeSystem.ResolveAsType(entity.Handle).GetDefinition();
					if (symbol == null) return base.GetTooltip(entity);
					break;
				default:
					return base.GetTooltip(entity);
			}
			var flags = ConversionFlags.All & ~ConversionFlags.ShowBody;
			return new CSharpAmbience() { ConversionFlags = flags }.ConvertSymbol(symbol);
		}
	}

	class InsertDynamicTypeVisitor : DepthFirstAstVisitor
	{
		bool isDynamic;
		bool[] mapping;
		int typeIndex;

		public InsertDynamicTypeVisitor(MetadataReader metadata, CustomAttributeHandleCollection? customAttributes)
		{
			isDynamic = DynamicAwareTypeReference.HasDynamicAttribute(customAttributes, metadata, out mapping);
		}

		public override void VisitComposedType(ComposedType composedType)
		{
			typeIndex++;
			base.VisitComposedType(composedType);
		}

		public override void VisitPrimitiveType(PrimitiveType primitiveType)
		{
			if (isDynamic && primitiveType.KnownTypeCode == KnownTypeCode.Object && (mapping == null || typeIndex >= mapping.Length || mapping[typeIndex])) {
				primitiveType.ReplaceWith(new SimpleType("dynamic"));
			} else {
				base.VisitPrimitiveType(primitiveType);
			}
		}

		public override void VisitMemberType(MemberType memberType)
		{
			base.VisitMemberType(memberType);
		}

		public override void VisitSimpleType(SimpleType simpleType)
		{
			base.VisitSimpleType(simpleType);
		}
	}
}
