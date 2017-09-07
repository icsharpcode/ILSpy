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

		public CSharpLanguage()
		{
		}

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

		public override string Name
		{
			get { return name; }
		}

		public override string FileExtension
		{
			get { return ".cs"; }
		}

		public override string ProjectFileExtension
		{
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
			var outputFormatter = new TextTokenWriter(output, settings, typeSystem) { FoldBraces = settings.FoldBraces };
			var formattingPolicy = settings.CSharpFormattingOptions;
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(outputFormatter, formattingPolicy));
		}
		
		public override void DecompileMethod(MethodDefinition method, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(method.DeclaringType, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(method.Module, options);
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(method), decompiler.TypeSystem);
			/*
			AstBuilder codeDomBuilder = CreateAstBuilder(options, currentType: method.DeclaringType, isSingleMember: true);
			if (method.IsConstructor && !method.IsStatic && !method.DeclaringType.IsValueType) {
				// also fields and other ctors so that the field initializers can be shown as such
				AddFieldsAndCtors(codeDomBuilder, method.DeclaringType, method.IsStatic);
				RunTransformsAndGenerateCode(codeDomBuilder, output, options, new SelectCtorTransform(method));
			} else {
				codeDomBuilder.AddMethod(method);
				RunTransformsAndGenerateCode(codeDomBuilder, output, options);
			}*/
		}
		
		/*
		class SelectCtorTransform : IAstTransform
		{
			readonly MethodDefinition ctorDef;
			
			public SelectCtorTransform(MethodDefinition ctorDef)
			{
				this.ctorDef = ctorDef;
			}
			
			public void Run(AstNode compilationUnit)
			{
				ConstructorDeclaration ctorDecl = null;
				foreach (var node in compilationUnit.Children) {
					ConstructorDeclaration ctor = node as ConstructorDeclaration;
					if (ctor != null) {
						if (ctor.Annotation<MethodDefinition>() == ctorDef) {
							ctorDecl = ctor;
						} else {
							// remove other ctors
							ctor.Remove();
						}
					}
					// Remove any fields without initializers
					FieldDeclaration fd = node as FieldDeclaration;
					if (fd != null && fd.Variables.All(v => v.Initializer.IsNull))
						fd.Remove();
				}
				if (ctorDecl.Initializer.ConstructorInitializerType == ConstructorInitializerType.This) {
					// remove all fields
					foreach (var node in compilationUnit.Children)
						if (node is FieldDeclaration)
							node.Remove();
				}
			}
		}
		 */

		public override void DecompileProperty(PropertyDefinition property, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(property.DeclaringType, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(property.Module, options);
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(property), decompiler.TypeSystem);
		}
		/*
		public override void DecompileField(FieldDefinition field, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(field.DeclaringType, includeNamespace: true));
			AstBuilder codeDomBuilder = CreateAstBuilder(options, currentType: field.DeclaringType, isSingleMember: true);
			if (field.IsLiteral) {
				codeDomBuilder.AddField(field);
			} else {
				// also decompile ctors so that the field initializer can be shown
				AddFieldsAndCtors(codeDomBuilder, field.DeclaringType, field.IsStatic);
			}
			RunTransformsAndGenerateCode(codeDomBuilder, output, options, new SelectFieldTransform(field));
		}
		
		/// <summary>
		/// Removes all top-level members except for the specified fields.
		/// </summary>
		sealed class SelectFieldTransform : IAstTransform
		{
			readonly FieldDefinition field;
			
			public SelectFieldTransform(FieldDefinition field)
			{
				this.field = field;
			}
			
			public void Run(AstNode compilationUnit)
			{
				foreach (var child in compilationUnit.Children) {
					if (child is EntityDeclaration) {
						if (child.Annotation<FieldDefinition>() != field)
							child.Remove();
					}
				}
			}
		}
		
		void AddFieldsAndCtors(AstBuilder codeDomBuilder, TypeDefinition declaringType, bool isStatic)
		{
			foreach (var field in declaringType.Fields) {
				if (field.IsStatic == isStatic)
					codeDomBuilder.AddField(field);
			}
			foreach (var ctor in declaringType.Methods) {
				if (ctor.IsConstructor && ctor.IsStatic == isStatic)
					codeDomBuilder.AddMethod(ctor);
			}
		}
		 */
		public override void DecompileEvent(EventDefinition ev, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(ev.DeclaringType, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(ev.Module, options);
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(ev), decompiler.TypeSystem);
		}

		public override void DecompileType(TypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			WriteCommentLine(output, TypeToString(type, includeNamespace: true));
			CSharpDecompiler decompiler = CreateDecompiler(type.Module, options);
			WriteCode(output, options.DecompilerSettings, decompiler.Decompile(type), decompiler.TypeSystem);
		}

		/*
		void RunTransformsAndGenerateCode(AstBuilder astBuilder, ITextOutput output, DecompilationOptions options, IAstTransform additionalTransform = null)
		{
			astBuilder.RunTransformations(transformAbortCondition);
			if (additionalTransform != null) {
				additionalTransform.Run(astBuilder.SyntaxTree);
			}
			if (options.DecompilerSettings.ShowXmlDocumentation) {
				try {
					AddXmlDocTransform.Run(astBuilder.SyntaxTree);
				} catch (XmlException ex) {
					string[] msg = (" Exception while reading XmlDoc: " + ex.ToString()).Split(new[]{'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
					var insertionPoint = astBuilder.SyntaxTree.FirstChild;
					for (int i = 0; i < msg.Length; i++)
						astBuilder.SyntaxTree.InsertChildBefore(insertionPoint, new Comment(msg[i], CommentType.Documentation), Roles.Comment);
				}
			}
			astBuilder.GenerateCode(output);
		}*/

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

		public override void DecompileAssembly(LoadedAssembly assembly, ITextOutput output, DecompilationOptions options)
		{
			if (options.FullDecompilation && options.SaveAsProjectDirectory != null) {
				var decompiler = new ILSpyWholeProjectDecompiler(assembly, options);
				decompiler.ProjectGuid = App.CommandLineArguments.FixedGuid;
				decompiler.DecompileProject(assembly.ModuleDefinition, options.SaveAsProjectDirectory, new TextOutputWriter(output), options.CancellationToken);
			} else {
				base.DecompileAssembly(assembly, output, options);
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
		
		/*
		AstBuilder CreateAstBuilder(DecompilationOptions options, ModuleDefinition currentModule = null, TypeDefinition currentType = null, bool isSingleMember = false)
		{
			if (currentModule == null)
				currentModule = currentType.Module;
			DecompilerSettings settings = options.DecompilerSettings;
			if (isSingleMember) {
				settings = settings.Clone();
				settings.UsingDeclarations = false;
			}
			return new AstBuilder(
				new DecompilerContext(currentModule) {
					CancellationToken = options.CancellationToken,
					CurrentType = currentType,
					Settings = settings
				});
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
			AstType astType = AstBuilder.ConvertType(type, typeAttributes, options);

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

			astType.AcceptVisitor(new CSharpOutputVisitor(w, FormattingOptionsFactory.CreateAllman()));
			return w.ToString();
		}
		 */
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
					var declaringType = accessor.Overrides.First().DeclaringType;
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
		/*
		public override string FormatTypeName(TypeDefinition type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			
			return TypeToString(ConvertTypeOptions.DoNotUsePrimitiveTypeNames | ConvertTypeOptions.IncludeTypeParameterDefinitions, type);
		}
		*/
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
			MethodDefinition md = member as MethodDefinition;
			PropertyDefinition pd = member as PropertyDefinition;
			EventDefinition ed = member as EventDefinition;
			FieldDefinition fd = member as FieldDefinition;
			if (md != null || pd != null || ed != null || fd != null) {
				/*AstBuilder b = new AstBuilder(new DecompilerContext(member.Module) { Settings = new DecompilerSettings { UsingDeclarations = false } });
				b.DecompileMethodBodies = false;
				if (md != null)
					b.AddMethod(md);
				else if (pd != null)
					b.AddProperty(pd);
				else if (ed != null)
					b.AddEvent(ed);
				else
					b.AddField(fd);
				b.RunTransformations();
				foreach (var attribute in b.SyntaxTree.Descendants.OfType<AttributeSection>())
					attribute.Remove();

				StringWriter w = new StringWriter();
				b.GenerateCode(new PlainTextOutput(w));
				return Regex.Replace(w.ToString(), @"\s+", " ").TrimEnd();*/
			}

			return base.GetTooltip(member);
		}
	}
}
