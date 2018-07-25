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
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
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
using ICSharpCode.Decompiler.Disassembler;
using GenericContext = ICSharpCode.Decompiler.Metadata.GenericContext;
using System.Text;

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
						new LanguageVersion(Decompiler.CSharp.LanguageVersion.CSharp7_3.ToString(), "C# 7.3 / VS 2017.7"),
					};
				}
				return versions;
			}
		}

		CSharpDecompiler CreateDecompiler(PEFile module, DecompilationOptions options)
		{
			CSharpDecompiler decompiler = new CSharpDecompiler(module, module.GetAssemblyResolver(), options.DecompilerSettings);
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

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			PEFile assembly = method.ParentModule.PEFile;
			AddReferenceWarningMessage(assembly, output);
			WriteCommentLine(output, TypeToString(method.DeclaringType, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			var methodDefinition = decompiler.TypeSystem.MainModule.ResolveEntity(method.MetadataToken) as IMethod;
			if (methodDefinition.IsConstructor && methodDefinition.DeclaringType.IsReferenceType != false) {
				var members = CollectFieldsAndCtors(methodDefinition.DeclaringTypeDefinition, methodDefinition.IsStatic);
				decompiler.AstTransforms.Add(new SelectCtorTransform(methodDefinition));
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(members), decompiler.TypeSystem);
			} else {
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(method.MetadataToken), decompiler.TypeSystem);
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

		public override void DecompileProperty(IProperty property, ITextOutput output, DecompilationOptions options)
		{
			PEFile assembly = property.ParentModule.PEFile;
			AddReferenceWarningMessage(assembly, output);
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			WriteCommentLine(output, TypeToString(property.DeclaringType, includeNamespace: true));
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(property.MetadataToken), decompiler.TypeSystem);
		}

		public override void DecompileField(IField field, ITextOutput output, DecompilationOptions options)
		{
			PEFile assembly = field.ParentModule.PEFile;
			AddReferenceWarningMessage(assembly, output);
			WriteCommentLine(output, TypeToString(field.DeclaringType, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			if (field.IsConst) {
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(field.MetadataToken), decompiler.TypeSystem);
			} else {
				var members = CollectFieldsAndCtors(field.DeclaringTypeDefinition, field.IsStatic);
				var resolvedField = decompiler.TypeSystem.MainModule.GetDefinition((FieldDefinitionHandle)field.MetadataToken);
				decompiler.AstTransforms.Add(new SelectFieldTransform(resolvedField));
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(members), decompiler.TypeSystem);
			}
		}

		static List<EntityHandle> CollectFieldsAndCtors(ITypeDefinition type, bool isStatic)
		{
			var members = new List<EntityHandle>();
			foreach (var field in type.Fields) {
				if (!field.MetadataToken.IsNil && field.IsStatic == isStatic)
					members.Add(field.MetadataToken);
			}
			foreach (var ctor in type.Methods) {
				if (!ctor.MetadataToken.IsNil && ctor.IsConstructor && ctor.IsStatic == isStatic)
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

		public override void DecompileEvent(IEvent @event, ITextOutput output, DecompilationOptions options)
		{
			PEFile assembly = @event.ParentModule.PEFile;
			AddReferenceWarningMessage(assembly, output);
			base.WriteCommentLine(output, TypeToString(@event.DeclaringType, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(@event.MetadataToken), decompiler.TypeSystem);
		}

		public override void DecompileType(ITypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			PEFile assembly = type.ParentModule.PEFile;
			AddReferenceWarningMessage(assembly, output);
			WriteCommentLine(output, TypeToString(type, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(assembly, options);
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(type.MetadataToken), decompiler.TypeSystem);
		}

		void AddReferenceWarningMessage(PEFile assembly, ITextOutput output)
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
			var module = assembly.GetPEFileOrNull();
			if (options.FullDecompilation && options.SaveAsProjectDirectory != null) {
				var decompiler = new ILSpyWholeProjectDecompiler(assembly, options);
				decompiler.DecompileProject(module, options.SaveAsProjectDirectory, new TextOutputWriter(output), options.CancellationToken);
			} else {
				AddReferenceWarningMessage(module, output);
				output.WriteLine();
				base.DecompileAssembly(assembly, output, options);

				// don't automatically load additional assemblies when an assembly node is selected in the tree view
				using (options.FullDecompilation ? null : LoadedAssembly.DisableAssemblyLoad()) {
					IAssemblyResolver assemblyResolver = assembly.GetAssemblyResolver();
					var typeSystem = new DecompilerTypeSystem(module, assemblyResolver, options.DecompilerSettings);
					var globalType = typeSystem.MainModule.TypeDefinitions.FirstOrDefault();
					if (globalType != null) {
						output.Write("// Global type: ");
						output.WriteReference(globalType, globalType.FullName);
						output.WriteLine();
					}
					var metadata = module.Metadata;
					var corHeader = module.Reader.PEHeaders.CorHeader;
					var entrypointHandle = MetadataTokenHelpers.EntityHandleOrNil(corHeader.EntryPointTokenOrRelativeVirtualAddress);
					if (!entrypointHandle.IsNil && entrypointHandle.Kind == HandleKind.MethodDefinition) {
						var entrypoint = typeSystem.MainModule.ResolveMethod(entrypointHandle, new Decompiler.TypeSystem.GenericContext());
						if (entrypoint != null) {
							output.Write("// Entry point: ");
							output.WriteReference(entrypoint, entrypoint.DeclaringType.FullName + "." + entrypoint.Name);
							output.WriteLine();
						}
					}
					output.WriteLine("// Architecture: " + GetPlatformDisplayName(module));
					if ((corHeader.Flags & System.Reflection.PortableExecutable.CorFlags.ILOnly) == 0) {
						output.WriteLine("// This assembly contains unmanaged code.");
					}
					string runtimeName = GetRuntimeDisplayName(module);
					if (runtimeName != null) {
						output.WriteLine("// Runtime: " + runtimeName);
					}
					var debugInfo = assembly.GetDebugInfoOrNull();
					if (debugInfo != null) {
						output.WriteLine("// Debug info: " + debugInfo.Description);
					}
					output.WriteLine();

					CSharpDecompiler decompiler = new CSharpDecompiler(typeSystem, options.DecompilerSettings);
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
				base.AssemblyResolver = assembly.GetAssemblyResolver();
			}

			protected override IEnumerable<Tuple<string, string>> WriteResourceToFile(string fileName, string resourceName, Stream entryStream)
			{
				foreach (var handler in App.ExportProvider.GetExportedValues<IResourceFileHandler>()) {
					if (handler.CanHandle(fileName, options)) {
						entryStream.Position = 0;
						fileName = Path.Combine(targetDirectory, fileName);
						fileName = handler.WriteResourceToFile(assembly, fileName, entryStream, options);
						return new[] { Tuple.Create(handler.EntryType, fileName) };
					}
				}
				return base.WriteResourceToFile(fileName, resourceName, entryStream);
			}
		}

		static readonly CSharpFormattingOptions TypeToStringFormattingOptions = FormattingOptionsFactory.CreateEmpty();

		public override string TypeToString(IType type, bool includeNamespace)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (type is ITypeDefinition definition && definition.TypeParameterCount > 0) {
				return TypeToStringInternal(new ParameterizedType(definition, definition.TypeParameters), includeNamespace, false);
			}
			return TypeToStringInternal(type, includeNamespace, false);
		}

		string TypeToStringInternal(IType t, bool includeNamespace, bool useBuiltinTypeNames = true, bool? isOutParameter = null)
		{
			TypeSystemAstBuilder builder = new TypeSystemAstBuilder();
			builder.AlwaysUseShortTypeNames = !includeNamespace;
			builder.AlwaysUseBuiltinTypeNames = useBuiltinTypeNames;

			AstType astType = builder.ConvertType(t);
			if (isOutParameter != null && astType is ComposedType ct && ct.HasRefSpecifier) {
				ct.HasRefSpecifier = false;
			} else {
				isOutParameter = null;
			}

			StringWriter w = new StringWriter();

			astType.AcceptVisitor(new CSharpOutputVisitor(w, TypeToStringFormattingOptions));
			string output = w.ToString();

			if (isOutParameter == true)
				output = "out " + output;
			else if (isOutParameter == false)
				output = "ref " + output;

			return output;
		}

		public override string FieldToString(IField field, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			if (field == null)
				throw new ArgumentNullException(nameof(field));

			string simple = field.Name + " : " + TypeToString(field.Type, includeNamespace);
			if (!includeDeclaringTypeName)
				return simple;
			return TypeToStringInternal(field.DeclaringType, includeNamespaceOfDeclaringTypeName) + "." + simple;
		}

		public override string PropertyToString(IProperty property, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			if (property == null)
				throw new ArgumentNullException(nameof(property));
			var buffer = new System.Text.StringBuilder();
			if (includeDeclaringTypeName) {
				buffer.Append(TypeToString(property.DeclaringType, includeNamespaceOfDeclaringTypeName));
				buffer.Append('.');
			}
			if (property.IsIndexer) {
				if (property.IsExplicitInterfaceImplementation) {
					string name = property.Name;
					int index = name.LastIndexOf('.');
					if (index > 0) {
						buffer.Append(name.Substring(0, index));
						buffer.Append('.');
					}
				}
				buffer.Append(@"this[");

				int i = 0;
				var parameters = property.Parameters;
				foreach (var param in parameters) {
					if (i > 0)
						buffer.Append(", ");
					buffer.Append(TypeToStringInternal(param.Type, includeNamespace, isOutParameter: param.IsOut));
					i++;
				}

				buffer.Append(@"]");
			} else {
				buffer.Append(property.Name);
			}
			buffer.Append(" : ");
			buffer.Append(TypeToStringInternal(property.ReturnType, includeNamespace));
			return buffer.ToString();
		}

		public override string MethodToString(IMethod method, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			if (method == null)
				throw new ArgumentNullException(nameof(method));
			string name;
			if (includeDeclaringTypeName) {
				name = TypeToString(method.DeclaringType, includeNamespace: includeNamespaceOfDeclaringTypeName) + ".";
			} else {
				name = "";
			}
			if (method.IsConstructor) {
				name += TypeToString(method.DeclaringType, false);
			} else {
				name += method.Name;
			}
			int i = 0;
			var buffer = new StringBuilder(name);

			if (method.TypeParameters.Count > 0) {
				buffer.Append('<');
				foreach (var tp in method.TypeParameters) {
					if (i > 0)
						buffer.Append(", ");
					buffer.Append(tp.Name);
					i++;
				}
				buffer.Append('>');
			}
			buffer.Append('(');

			i = 0;
			var parameters = method.Parameters;
			foreach (var param in parameters) {
				if (i > 0)
					buffer.Append(", ");
				buffer.Append(TypeToStringInternal(param.Type, includeNamespace, isOutParameter: param.IsOut));
				i++;
			}

			buffer.Append(')');
			if (!method.IsConstructor) {
				buffer.Append(" : ");
				buffer.Append(TypeToStringInternal(method.ReturnType, includeNamespace));
			}
			return buffer.ToString();
		}

		public override string EventToString(IEvent @event, bool includeDeclaringTypeName, bool includeNamespace, bool includeNamespaceOfDeclaringTypeName)
		{
			if (@event == null)
				throw new ArgumentNullException(nameof(@event));
			var buffer = new System.Text.StringBuilder();
			if (includeDeclaringTypeName) {
				buffer.Append(TypeToString(@event.DeclaringType, includeNamespaceOfDeclaringTypeName) + ".");
			}
			buffer.Append(@event.Name);
			buffer.Append(" : ");
			buffer.Append(TypeToStringInternal(@event.ReturnType, includeNamespace));
			return buffer.ToString();
		}

		string ToCSharpString(MetadataReader metadata, TypeDefinitionHandle handle, bool fullName)
		{
			StringBuilder builder = new StringBuilder();
			var currentTypeDefHandle = handle;
			var typeDef = metadata.GetTypeDefinition(currentTypeDefHandle);

			while (!currentTypeDefHandle.IsNil) {
				if (builder.Length > 0)
					builder.Insert(0, '.');
				typeDef = metadata.GetTypeDefinition(currentTypeDefHandle);
				var part = ReflectionHelper.SplitTypeParameterCountFromReflectionName(metadata.GetString(typeDef.Name), out int typeParamCount);
				var genericParams = typeDef.GetGenericParameters();
				if (genericParams.Count > 0) {
					builder.Insert(0, '>');
					int firstIndex = genericParams.Count - typeParamCount;
					for (int i = genericParams.Count - 1; i >= genericParams.Count - typeParamCount; i--) {
						builder.Insert(0, metadata.GetString(metadata.GetGenericParameter(genericParams[i]).Name));
						builder.Insert(0, i == firstIndex ? '<' : ',');
					}
				}
				builder.Insert(0, part);
				currentTypeDefHandle = typeDef.GetDeclaringType();
				if (!fullName) break;
			}

			if (fullName && !typeDef.Namespace.IsNil) {
				builder.Insert(0, '.');
				builder.Insert(0, metadata.GetString(typeDef.Namespace));
			}

			return builder.ToString();
		}

		public override string GetEntityName(PEFile module, EntityHandle handle, bool fullName)
		{
			MetadataReader metadata = module.Metadata;
			switch (handle.Kind) {
				case HandleKind.TypeDefinition:
					return ToCSharpString(metadata, (TypeDefinitionHandle)handle, fullName);
				case HandleKind.FieldDefinition:
					var fd = metadata.GetFieldDefinition((FieldDefinitionHandle)handle);
					var declaringType = fd.GetDeclaringType();
					if (fullName)
						return ToCSharpString(metadata, declaringType, fullName) + "." + metadata.GetString(fd.Name);
					return metadata.GetString(fd.Name);
				case HandleKind.MethodDefinition:
					var md = metadata.GetMethodDefinition((MethodDefinitionHandle)handle);
					declaringType = md.GetDeclaringType();
					string methodName = metadata.GetString(md.Name);
					if (methodName == ".ctor" || methodName == ".cctor") {
						var td = metadata.GetTypeDefinition(declaringType);
						methodName = ReflectionHelper.SplitTypeParameterCountFromReflectionName(metadata.GetString(td.Name));
					} else {
						var genericParams = md.GetGenericParameters();
						if (genericParams.Count > 0) {
							methodName += "<";
							int i = 0;
							foreach (var h in genericParams) {
								if (i > 0)
									methodName += ",";
								var gp = metadata.GetGenericParameter(h);
								methodName += metadata.GetString(gp.Name);
							}
							methodName += ">";
						}
					}
					if (fullName)
						return ToCSharpString(metadata, declaringType, fullName) + "." + methodName;
					return methodName;
				case HandleKind.EventDefinition:
					var ed = metadata.GetEventDefinition((EventDefinitionHandle)handle);
					declaringType = metadata.GetMethodDefinition(ed.GetAccessors().GetAny()).GetDeclaringType();
					if (fullName)
						return ToCSharpString(metadata, declaringType, fullName) + "." + metadata.GetString(ed.Name);
					return metadata.GetString(ed.Name);
				case HandleKind.PropertyDefinition:
					var pd = metadata.GetPropertyDefinition((PropertyDefinitionHandle)handle);
					declaringType = metadata.GetMethodDefinition(pd.GetAccessors().GetAny()).GetDeclaringType();
					if (fullName)
						return ToCSharpString(metadata, declaringType, fullName) + "." + metadata.GetString(pd.Name);
					return metadata.GetString(pd.Name);
				default:
					return null;
			}
		}

		public override bool ShowMember(IEntity member)
		{
			PEFile assembly = member.ParentModule.PEFile;
			return showAllMembers || !CSharpDecompiler.MemberIsHidden(assembly, member.MetadataToken, new DecompilationOptions().DecompilerSettings);
		}

		public override string GetTooltip(IEntity entity)
		{
			var flags = ConversionFlags.All & ~ConversionFlags.ShowBody;
			return new CSharpAmbience() { ConversionFlags = flags }.ConvertSymbol(entity);
		}

		public override CodeMappingInfo GetCodeMappingInfo(PEFile module, EntityHandle member)
		{
			return CSharpDecompiler.GetCodeMappingInfo(module, member);
		}
	}
}
