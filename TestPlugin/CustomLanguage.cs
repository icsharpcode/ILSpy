﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System.ComponentModel.Composition;
using System.Reflection.Metadata;
using System.Windows.Controls;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy;

namespace TestPlugin
{
	/// <summary>
	/// Adds a new language to the decompiler.
	/// </summary>
	[Export(typeof(Language))]
	[PartCreationPolicy(CreationPolicy.Shared)]
	public class CustomLanguage : Language
	{
		public override string Name {
			get {
				return "Custom";
			}
		}

		public override string FileExtension {
			get {
				// used in 'Save As' dialog
				return ".txt";
			}
		}

		// There are several methods available to override; in this sample, we deal with methods only
		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			var module = ((MetadataModule)method.ParentModule).MetadataFile;
			var methodDef = module.Metadata.GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
			if (methodDef.HasBody())
			{
				var methodBody = module.GetMethodBody(methodDef.RelativeVirtualAddress);
				output.WriteLine("Size of method: {0} bytes", methodBody.GetCodeSize());

				ISmartTextOutput smartOutput = output as ISmartTextOutput;
				if (smartOutput != null)
				{
					// when writing to the text view (but not when writing to a file), we can even add UI elements such as buttons:
					smartOutput.AddButton(null, "Click me!", (sender, e) => (sender as Button).Content = "I was clicked!");
					smartOutput.WriteLine();
				}

				// ICSharpCode.Decompiler.CSharp.CSharpDecompiler can be used to decompile to C#.
				/*
					ModuleDefinition module = LoadModule(assemblyFileName);
					var typeSystem = new DecompilerTypeSystem(module);
					CSharpDecompiler decompiler = new CSharpDecompiler(typeSystem, new DecompilerSettings());

					decompiler.AstTransforms.Add(new EscapeInvalidIdentifiers());
					SyntaxTree syntaxTree = decompiler.DecompileWholeModuleAsSingleFile();
					var visitor = new CSharpOutputVisitor(output, FormattingOptionsFactory.CreateSharpDevelop());
					syntaxTree.AcceptVisitor(visitor);
				*/
			}
		}
	}
}
