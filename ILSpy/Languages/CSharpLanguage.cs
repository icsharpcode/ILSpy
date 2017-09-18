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

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Options;
using Mono.Cecil;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.TypeSystem;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows.Media;

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
			var decompiler = new CSharpDecompiler(ModuleDefinition.CreateModule("Dummy", ModuleKind.Dll), new DecompilerSettings());
			string lastTransformName = "no transforms";
			int transformCount = 0;
			foreach (var transform in decompiler.AstTransforms) {
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

		CSharpDecompiler CreateDecompiler(ModuleDefinition module, DecompilationOptions options)
		{
			CSharpDecompiler decompiler = new CSharpDecompiler(module, options.DecompilerSettings);
			decompiler.CancellationToken = options.CancellationToken;
			while (decompiler.AstTransforms.Count > transformCount)
				decompiler.AstTransforms.RemoveAt(decompiler.AstTransforms.Count - 1);
			return decompiler;
		}

		void WriteCode(ITextOutput output, DecompilerSettings settings, SyntaxTree syntaxTree, IDecompilerTypeSystem typeSystem)
		{
			syntaxTree.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
			TokenWriter tokenWriter = new TextTokenWriter(output, settings, typeSystem) { FoldBraces = settings.FoldBraces };
			if (output is ISmartTextOutput highlightingOutput) {
				tokenWriter = new HighlightingTokenWriter(tokenWriter, highlightingOutput);
			}
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
		}

		public override void DecompileMethod(MethodDefinition method, ITextOutput output, DecompilationOptions options)
		{
			AddReferenceWarningMessage(method.Module.Assembly, output);
			WriteCommentLine(output, TypeToString(method.DeclaringType, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(method.Module, options);
			if (method.IsConstructor && !method.DeclaringType.IsValueType) {
				List<IMemberDefinition> members = CollectFieldsAndCtors(method.DeclaringType, method.IsStatic);
				decompiler.AstTransforms.Add(new SelectCtorTransform(decompiler.TypeSystem.Resolve(method)));
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(members), decompiler.TypeSystem);
			} else {
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(method), decompiler.TypeSystem);
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

		public override void DecompileProperty(PropertyDefinition property, ITextOutput output, DecompilationOptions options)
		{
			AddReferenceWarningMessage(property.Module.Assembly, output);
			WriteCommentLine(output, TypeToString(property.DeclaringType, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(property.Module, options);
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(property), decompiler.TypeSystem);
		}

		public override void DecompileField(FieldDefinition field, ITextOutput output, DecompilationOptions options)
		{
			AddReferenceWarningMessage(field.Module.Assembly, output);
			WriteCommentLine(output, TypeToString(field.DeclaringType, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(field.Module, options);
			if (field.IsLiteral) {
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(field), decompiler.TypeSystem);
			} else {
				List<IMemberDefinition> members = CollectFieldsAndCtors(field.DeclaringType, field.IsStatic);
				decompiler.AstTransforms.Add(new SelectFieldTransform(decompiler.TypeSystem.Resolve(field)));
				WriteCode(output, options.DecompilerSettings, decompiler.Decompile(members), decompiler.TypeSystem);
			}
		}

		private static List<IMemberDefinition> CollectFieldsAndCtors(TypeDefinition type, bool isStatic)
		{
			var members = new List<IMemberDefinition>();
			foreach (var field in type.Fields) {
				if (field.IsStatic == isStatic)
					members.Add(field);
			}
			foreach (var ctor in type.Methods) {
				if (ctor.IsConstructor && ctor.IsStatic == isStatic)
					members.Add(ctor);
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

		public override void DecompileEvent(EventDefinition ev, ITextOutput output, DecompilationOptions options)
		{
			AddReferenceWarningMessage(ev.Module.Assembly, output);
			WriteCommentLine(output, TypeToString(ev.DeclaringType, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(ev.Module, options);
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(ev), decompiler.TypeSystem);
		}

		public override void DecompileType(TypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			AddReferenceWarningMessage(type.Module.Assembly, output);
			WriteCommentLine(output, TypeToString(type, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(type.Module, options);
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(type), decompiler.TypeSystem);
		}

		public static string GetPlatformDisplayName(ModuleDefinition module)
		{
			switch (module.Architecture) {
				case TargetArchitecture.I386:
					if ((module.Attributes & ModuleAttributes.Preferred32Bit) == ModuleAttributes.Preferred32Bit)
						return "AnyCPU (32-bit preferred)";
					else if ((module.Attributes & ModuleAttributes.Required32Bit) == ModuleAttributes.Required32Bit)
						return "x86";
					else
						return "AnyCPU (64-bit preferred)";
				case TargetArchitecture.AMD64:
					return "x64";
				case TargetArchitecture.IA64:
					return "Itanium";
				default:
					return module.Architecture.ToString();
			}
		}

		public static string GetRuntimeDisplayName(ModuleDefinition module)
		{
			switch (module.Runtime) {
				case TargetRuntime.Net_1_0:
					return ".NET 1.0";
				case TargetRuntime.Net_1_1:
					return ".NET 1.1";
				case TargetRuntime.Net_2_0:
					return ".NET 2.0";
				case TargetRuntime.Net_4_0:
					return ".NET 4.0";
			}
			return null;
		}

		void AddReferenceWarningMessage(AssemblyDefinition assembly, ITextOutput output)
		{
			var loadedAssembly = MainWindow.Instance.CurrentAssemblyList.GetAssemblies().FirstOrDefault(la => la.AssemblyDefinition == assembly);
			if (loadedAssembly == null || !loadedAssembly.LoadedAssemblyReferencesInfo.Any(i => i.Value.HasErrors))
				return;
			const string line1 = "Warning: Some assembly references could not be loaded. This might lead to incorrect decompilation of some parts,";
			const string line2 = "for ex. property getter/setter access. To get optimal decompilation results, please manually add the references to the list of loaded assemblies.";
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
			if (options.FullDecompilation && options.SaveAsProjectDirectory != null) {
				var decompiler = new ILSpyWholeProjectDecompiler(assembly, options);
				decompiler.ProjectGuid = App.CommandLineArguments.FixedGuid;
				decompiler.DecompileProject(assembly.ModuleDefinition, options.SaveAsProjectDirectory, new TextOutputWriter(output), options.CancellationToken);
			} else {
				base.DecompileAssembly(assembly, output, options);
				AddReferenceWarningMessage(assembly.AssemblyDefinition, output);
				output.WriteLine();
				ModuleDefinition mainModule = assembly.ModuleDefinition;
				if (mainModule.Types.Count > 0) {
					output.Write("// Global type: ");
					output.WriteReference(mainModule.Types[0].FullName, mainModule.Types[0]);
					output.WriteLine();
				}
				if (mainModule.EntryPoint != null) {
					output.Write("// Entry point: ");
					output.WriteReference(mainModule.EntryPoint.DeclaringType.FullName + "." + mainModule.EntryPoint.Name, mainModule.EntryPoint);
					output.WriteLine();
				}
				output.WriteLine("// Architecture: " + GetPlatformDisplayName(mainModule));
				if ((mainModule.Attributes & ModuleAttributes.ILOnly) == 0) {
					output.WriteLine("// This assembly contains unmanaged code.");
				}
				string runtimeName = GetRuntimeDisplayName(mainModule);
				if (runtimeName != null) {
					output.WriteLine("// Runtime: " + runtimeName);
				}
				output.WriteLine();

				// don't automatically load additional assemblies when an assembly node is selected in the tree view
				using (options.FullDecompilation ? null : LoadedAssembly.DisableAssemblyLoad()) {
					CSharpDecompiler decompiler = new CSharpDecompiler(assembly.ModuleDefinition, options.DecompilerSettings);
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
				foreach (var handler in App.CompositionContainer.GetExportedValues<IResourceFileHandler>()) {
					if (handler.CanHandle(fileName, options)) {
						entryStream.Position = 0;
						return new[] { Tuple.Create(handler.EntryType, handler.WriteResourceToFile(assembly, fileName, entryStream, options)) };
					}
				}
				return base.WriteResourceToFile(fileName, resourceName, entryStream);
			}
		}

		public override string TypeToString(TypeReference type, bool includeNamespace, ICustomAttributeProvider typeAttributes = null)
		{
			ConvertTypeOptions options = ConvertTypeOptions.IncludeTypeParameterDefinitions;
			if (includeNamespace)
				options |= ConvertTypeOptions.IncludeNamespace;

			return TypeToString(options, type, typeAttributes);
		}

		string TypeToString(ConvertTypeOptions options, TypeReference type, ICustomAttributeProvider typeAttributes = null)
		{
			AstType astType = CSharpDecompiler.ConvertType(type, typeAttributes, options);

			StringWriter w = new StringWriter();
			if (type.IsByReference) {
				ParameterDefinition pd = typeAttributes as ParameterDefinition;
				if (pd != null && (!pd.IsIn && pd.IsOut))
					w.Write("out ");
				else
					w.Write("ref ");

				if (astType is ComposedType && ((ComposedType)astType).PointerRank > 0)
					((ComposedType)astType).PointerRank--;
			}

			astType.AcceptVisitor(new CSharpOutputVisitor(w, TypeToStringFormattingOptions));
			return w.ToString();
		}

		static readonly CSharpFormattingOptions TypeToStringFormattingOptions = FormattingOptionsFactory.CreateEmpty();

		public override string FormatPropertyName(PropertyDefinition property, bool? isIndexer)
		{
			if (property == null)
				throw new ArgumentNullException(nameof(property));

			if (!isIndexer.HasValue) {
				isIndexer = property.IsIndexer();
			}
			if (isIndexer.Value) {
				var buffer = new System.Text.StringBuilder();
				var accessor = property.GetMethod ?? property.SetMethod;
				if (accessor.HasOverrides) {
					var declaringType = accessor.Overrides[0].DeclaringType;
					buffer.Append(TypeToString(declaringType, includeNamespace: true));
					buffer.Append(@".");
				}
				buffer.Append(@"this[");
				bool addSeparator = false;
				foreach (var p in property.Parameters) {
					if (addSeparator)
						buffer.Append(@", ");
					else
						addSeparator = true;
					buffer.Append(TypeToString(p.ParameterType, includeNamespace: true));
				}
				buffer.Append(@"]");
				return buffer.ToString();
			} else
				return property.Name;
		}

		public override string FormatMethodName(MethodDefinition method)
		{
			if (method == null)
				throw new ArgumentNullException("method");

			return (method.IsConstructor) ? FormatTypeName(method.DeclaringType) : method.Name;
		}

		public override string FormatTypeName(TypeDefinition type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			return TypeToString(ConvertTypeOptions.DoNotUsePrimitiveTypeNames | ConvertTypeOptions.IncludeTypeParameterDefinitions, type);
		}

		public override bool ShowMember(MemberReference member)
		{
			return showAllMembers || !CSharpDecompiler.MemberIsHidden(member, new DecompilationOptions().DecompilerSettings);
		}

		public override MemberReference GetOriginalCodeLocation(MemberReference member)
		{
			if (showAllMembers || !DecompilerSettingsPanel.CurrentDecompilerSettings.AnonymousMethods)
				return member;
			else
				return TreeNodes.Analyzer.Helpers.GetOriginalCodeLocation(member);
		}

		public override string GetTooltip(MemberReference member)
		{
			var decompilerTypeSystem = new DecompilerTypeSystem(member.Module);
			ISymbol symbol;
			switch (member) {
				case MethodReference mr:
					symbol = decompilerTypeSystem.Resolve(mr);
					if (symbol == null) return base.GetTooltip(member);
					break;
				case PropertyReference pr:
					symbol = decompilerTypeSystem.Resolve(pr);
					if (symbol == null) return base.GetTooltip(member);
					break;
				case EventReference er:
					symbol = decompilerTypeSystem.Resolve(er);
					if (symbol == null) return base.GetTooltip(member);
					break;
				case FieldReference fr:
					symbol = decompilerTypeSystem.Resolve(fr);
					if (symbol == null) return base.GetTooltip(member);
					break;
				default:
					return base.GetTooltip(member);
			}
			var flags = ConversionFlags.All & ~ConversionFlags.ShowBody;
			return new CSharpAmbience() { ConversionFlags = flags }.ConvertSymbol(symbol);
		}

		class HighlightingTokenWriter : DecoratingTokenWriter
		{
			ISmartTextOutput textOutput;

			HighlightingColor defaultTextColor;

			HighlightingColor visibilityKeywordsColor;
			HighlightingColor namespaceKeywordsColor;
			HighlightingColor structureKeywordsColor;
			HighlightingColor gotoKeywordsColor;
			HighlightingColor queryKeywordsColor;
			HighlightingColor exceptionKeywordsColor;
			HighlightingColor checkedKeywordColor;
			HighlightingColor unsafeKeywordsColor;
			HighlightingColor valueTypeKeywordsColor;
			HighlightingColor referenceTypeKeywordsColor;
			HighlightingColor operatorKeywordsColor;
			HighlightingColor parameterModifierColor;
			HighlightingColor modifiersColor;
			HighlightingColor accessorKeywordsColor;
			HighlightingColor attributeKeywordsColor;

			HighlightingColor referenceTypeColor;
			HighlightingColor valueTypeColor;
			HighlightingColor interfaceTypeColor;
			HighlightingColor enumerationTypeColor;
			HighlightingColor typeParameterTypeColor;
			HighlightingColor delegateTypeColor;

			HighlightingColor methodCallColor;
			HighlightingColor methodDeclarationColor;

			HighlightingColor eventDeclarationColor;
			HighlightingColor eventAccessColor;

			HighlightingColor propertyDeclarationColor;
			HighlightingColor propertyAccessColor;

			HighlightingColor fieldDeclarationColor;
			HighlightingColor fieldAccessColor;

			HighlightingColor variableDeclarationColor;
			HighlightingColor variableAccessColor;

			HighlightingColor parameterDeclarationColor;
			HighlightingColor parameterAccessColor;

			HighlightingColor valueKeywordColor;
			HighlightingColor thisKeywordColor;
			HighlightingColor trueKeywordColor;
			HighlightingColor typeKeywordsColor;
			HighlightingColor externAliasKeywordColor;
			HighlightingColor varKeywordTypeColor;

			HighlightingColor stringFormatItemColor;

			public HighlightingTokenWriter(TokenWriter decoratedWriter, ISmartTextOutput textOutput) : base(decoratedWriter)
			{
				this.textOutput = textOutput;
				var highlighting = HighlightingManager.Instance.GetDefinition("C#");

				//this.defaultTextColor = ???;

				this.visibilityKeywordsColor = highlighting.GetNamedColor("Visibility");
				this.namespaceKeywordsColor = highlighting.GetNamedColor("NamespaceKeywords");
				this.structureKeywordsColor = highlighting.GetNamedColor("Keywords");
				this.gotoKeywordsColor = highlighting.GetNamedColor("GotoKeywords");
				this.queryKeywordsColor = highlighting.GetNamedColor("QueryKeywords");
				this.exceptionKeywordsColor = highlighting.GetNamedColor("ExceptionKeywords");
				this.checkedKeywordColor = highlighting.GetNamedColor("CheckedKeyword");
				this.unsafeKeywordsColor = highlighting.GetNamedColor("UnsafeKeywords");
				this.valueTypeKeywordsColor = highlighting.GetNamedColor("ValueTypeKeywords");
				this.referenceTypeKeywordsColor = highlighting.GetNamedColor("ReferenceTypeKeywords");
				this.operatorKeywordsColor = highlighting.GetNamedColor("OperatorKeywords");
				this.parameterModifierColor = highlighting.GetNamedColor("ParameterModifiers");
				this.modifiersColor = highlighting.GetNamedColor("Modifiers");
				this.accessorKeywordsColor = highlighting.GetNamedColor("GetSetAddRemove");

				this.referenceTypeColor = highlighting.GetNamedColor("ReferenceTypes");
				this.valueTypeColor = highlighting.GetNamedColor("ValueTypes");
				this.interfaceTypeColor = highlighting.GetNamedColor("InterfaceTypes");
				this.enumerationTypeColor = highlighting.GetNamedColor("EnumTypes");
				this.typeParameterTypeColor = highlighting.GetNamedColor("TypeParameters");
				this.delegateTypeColor = highlighting.GetNamedColor("DelegateTypes");
				this.methodDeclarationColor = this.methodCallColor = highlighting.GetNamedColor("MethodCall");
				//this.eventDeclarationColor = this.eventAccessColor = defaultTextColor;
				//this.propertyDeclarationColor = this.propertyAccessColor = defaultTextColor;
				this.fieldDeclarationColor = this.fieldAccessColor = highlighting.GetNamedColor("FieldAccess");
				//this.variableDeclarationColor = this.variableAccessColor = defaultTextColor;
				//this.parameterDeclarationColor = this.parameterAccessColor = defaultTextColor;
				this.valueKeywordColor = highlighting.GetNamedColor("NullOrValueKeywords");
				this.thisKeywordColor = highlighting.GetNamedColor("ThisOrBaseReference");
				this.trueKeywordColor = highlighting.GetNamedColor("TrueFalse");
				this.typeKeywordsColor = highlighting.GetNamedColor("TypeKeywords");
				this.attributeKeywordsColor = highlighting.GetNamedColor("AttributeKeywords");
				//this.externAliasKeywordColor = ...;
			}

			public override void WriteKeyword(Role role, string keyword)
			{
				HighlightingColor color = null;
				switch (keyword) {
					case "namespace":
					case "using":
						if (role == UsingStatement.UsingKeywordRole)
							color = structureKeywordsColor;
						else
							color = namespaceKeywordsColor;
						break;
					case "this":
					case "base":
						color = thisKeywordColor;
						break;
					case "true":
					case "false":
						color = trueKeywordColor;
						break;
					case "public":
					case "internal":
					case "protected":
					case "private":
						color = visibilityKeywordsColor;
						break;
					case "if":
					case "else":
					case "switch":
					case "case":
					case "default":
					case "while":
					case "do":
					case "for":
					case "foreach":
					case "lock":
					case "global":
					case "dynamic":
					case "await":
					case "where":
						color = structureKeywordsColor;
						break;
					case "in":
						if (nodeStack.PeekOrDefault() is ForeachStatement)
							color = structureKeywordsColor;
						else if (nodeStack.PeekOrDefault() is QueryExpression)
							color = queryKeywordsColor;
						else
							color = parameterModifierColor;
						break;
					case "as":
					case "is":
					case "new":
					case "sizeof":
					case "typeof":
					case "nameof":
					case "stackalloc":
						color = typeKeywordsColor;
						break;
					case "try":
					case "throw":
					case "catch":
					case "finally":
						color = exceptionKeywordsColor;
						break;
					case "when":
						if (role == CatchClause.WhenKeywordRole)
							color = exceptionKeywordsColor;
						break;
					case "get":
					case "set":
					case "add":
					case "remove":
						if (role == PropertyDeclaration.GetKeywordRole ||
							role == PropertyDeclaration.SetKeywordRole ||
							role == CustomEventDeclaration.AddKeywordRole ||
							role == CustomEventDeclaration.RemoveKeywordRole)
							color = accessorKeywordsColor;
						break;
					case "abstract":
					case "const":
					case "event":
					case "extern":
					case "override":
					case "readonly":
					case "sealed":
					case "static":
					case "virtual":
					case "volatile":
					case "async":
					case "partial":
						color = modifiersColor;
						break;
					case "checked":
					case "unchecked":
						color = checkedKeywordColor;
						break;
					case "fixed":
					case "unsafe":
						color = unsafeKeywordsColor;
						break;
					case "enum":
					case "struct":
						color = valueTypeKeywordsColor;
						break;
					case "class":
					case "interface":
					case "delegate":
						color = referenceTypeKeywordsColor;
						break;
					case "select":
					case "group":
					case "by":
					case "into":
					case "from":
					case "ascending":
					case "descending":
					case "orderby":
					case "let":
					case "join":
					case "on":
					case "equals":
						if (nodeStack.PeekOrDefault() is QueryExpression)
							color = queryKeywordsColor;
						break;
					case "explicit":
					case "implicit":
					case "operator":
						color = operatorKeywordsColor;
						break;
					case "params":
					case "ref":
					case "out":
						color = parameterModifierColor;
						break;
					case "break":
					case "continue":
					case "goto":
					case "yield":
					case "return":
						color = gotoKeywordsColor;
						break;
				}
				if (nodeStack.PeekOrDefault() is AttributeSection)
					color = attributeKeywordsColor;
				if (color != null) {
					textOutput.BeginSpan(color);
				}
				base.WriteKeyword(role, keyword);
				if (color != null) {
					textOutput.EndSpan();
				}
			}

			public override void WritePrimitiveType(string type)
			{
				HighlightingColor color = null;
				switch (type) {
					case "new":
						color = typeKeywordsColor;
						break;
					case "bool":
					case "byte":
					case "char":
					case "decimal":
					case "double":
					case "enum":
					case "float":
					case "int":
					case "long":
					case "sbyte":
					case "short":
					case "struct":
					case "uint":
					case "ushort":
					case "ulong":
						color = valueTypeKeywordsColor;
						break;
					case "object":
					case "string":
					case "void":
						color = referenceTypeKeywordsColor;
						break;
				}
				if (color != null) {
					textOutput.BeginSpan(color);
				}
				base.WritePrimitiveType(type);
				if (color != null) {
					textOutput.EndSpan();
				}
			}

			public override void WriteIdentifier(Identifier identifier)
			{
				HighlightingColor color = null;
				if (identifier.Name == "value" && nodeStack.PeekOrDefault() is Accessor accessor && accessor.Role != PropertyDeclaration.GetterRole)
					color = valueKeywordColor;
				switch (GetCurrentDefinition()) {
					case ITypeDefinition t:
						switch (t.Kind) {
							case TypeKind.Delegate:
								color = delegateTypeColor;
								break;
							case TypeKind.Class:
								color = referenceTypeColor;
								break;
							case TypeKind.Interface:
								color = interfaceTypeColor;
								break;
							case TypeKind.Enum:
								color = enumerationTypeColor;
								break;
							case TypeKind.Struct:
								color = valueTypeColor;
								break;
						}
						break;
					case IMethod m:
						color = methodDeclarationColor;
						break;
					case IField f:
						color = fieldDeclarationColor;
						break;
				}
				switch (GetCurrentMemberReference()) {
					case IType t:
						switch (t.Kind) {
							case TypeKind.Delegate:
								color = delegateTypeColor;
								break;
							case TypeKind.Class:
								color = referenceTypeColor;
								break;
							case TypeKind.Interface:
								color = interfaceTypeColor;
								break;
							case TypeKind.Enum:
								color = enumerationTypeColor;
								break;
							case TypeKind.Struct:
								color = valueTypeColor;
								break;
						}
						break;
					case IMethod m:
						color = methodCallColor;
						break;
					case IField f:
						color = fieldAccessColor;
						break;
				}
				if (color != null) {
					textOutput.BeginSpan(color);
				}
				base.WriteIdentifier(identifier);
				if (color != null) {
					textOutput.EndSpan();
				}
			}

			public override void WritePrimitiveValue(object value, string literalValue = null)
			{
				HighlightingColor color = null;
				if (value is null) {
					color = valueKeywordColor;
				}
				if (value is true || value is false) {
					color = trueKeywordColor;
				}
				if (color != null) {
					textOutput.BeginSpan(color);
				}
				base.WritePrimitiveValue(value, literalValue);
				if (color != null) {
					textOutput.EndSpan();
				}
			}

			ISymbol GetCurrentDefinition()
			{
				if (nodeStack == null || nodeStack.Count == 0)
					return null;

				var node = nodeStack.Peek();
				if (node is Identifier)
					node = node.Parent;
				if (IsDefinition(ref node))
					return node.GetSymbol();

				return null;
			}

			static bool IsDefinition(ref AstNode node)
			{
				if (node is EntityDeclaration)
					return true;
				if (node is VariableInitializer && node.Parent is FieldDeclaration) {
					node = node.Parent;
					return true;
				}
				if (node is FixedVariableInitializer)
					return true;
				return false;
			}

			ISymbol GetCurrentMemberReference()
			{
				AstNode node = nodeStack.Peek();
				var symbol = node.GetSymbol();
				if (symbol == null && node.Role == Roles.TargetExpression && node.Parent is InvocationExpression) {
					symbol = node.Parent.GetSymbol();
				}
				if (symbol != null && node.Parent is ObjectCreateExpression) {
					symbol = node.Parent.GetSymbol();
				}
				if (node is IdentifierExpression && node.Role == Roles.TargetExpression && node.Parent is InvocationExpression && symbol is IMember member) {
					var declaringType = member.DeclaringType;
					if (declaringType != null && declaringType.Kind == TypeKind.Delegate)
						return null;
				}
				return symbol;
			}

			Stack<AstNode> nodeStack = new Stack<AstNode>();

			public override void StartNode(AstNode node)
			{
				nodeStack.Push(node);
				base.StartNode(node);
			}

			public override void EndNode(AstNode node)
			{
				base.EndNode(node);
				nodeStack.Pop();
			}
		}
	}
}
