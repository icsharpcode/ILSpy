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
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ConversionFlags = ICSharpCode.Decompiler.Output.ConversionFlags;

namespace ILSpy.Languages
{
	[Export(typeof(Language))]
	[Shared]
	public sealed class CSharpLanguage : Language
	{
		public override string Name => "C#";

		public override string FileExtension => ".cs";

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

		public override void DecompileType(ITypeDefinition type, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(type.ParentModule?.MetadataFile != null);
			DecompileEntities(type.ParentModule.MetadataFile, new[] { type.MetadataToken }, output, options);
		}

		public override void DecompileMethod(IMethod method, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(method.ParentModule?.MetadataFile != null);
			DecompileEntities(method.ParentModule.MetadataFile, new[] { method.MetadataToken }, output, options);
		}

		public override void DecompileField(IField field, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(field.ParentModule?.MetadataFile != null);
			DecompileEntities(field.ParentModule.MetadataFile, new[] { field.MetadataToken }, output, options);
		}

		public override void DecompileProperty(IProperty property, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(property.ParentModule?.MetadataFile != null);
			DecompileEntities(property.ParentModule.MetadataFile, new[] { property.MetadataToken }, output, options);
		}

		public override void DecompileEvent(IEvent ev, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(ev.ParentModule?.MetadataFile != null);
			DecompileEntities(ev.ParentModule.MetadataFile, new[] { ev.MetadataToken }, output, options);
		}

		public override void DecompileNamespace(string nameSpace, IEnumerable<ITypeDefinition> types, ITextOutput output, DecompilationOptions options)
		{
			var typesByModule = types.GroupBy(t => {
				Debug.Assert(t.ParentModule?.MetadataFile != null);
				return t.ParentModule.MetadataFile;
			});
			bool first = true;
			foreach (var group in typesByModule)
			{
				if (!first)
					output.WriteLine();
				first = false;
				DecompileEntities(group.Key, group.Select(t => t.MetadataToken), output, options);
			}
		}

		void DecompileEntities(MetadataFile module, IEnumerable<EntityHandle> handles, ITextOutput output, DecompilationOptions options)
		{
			if (module == null)
			{
				WriteCommentLine(output, "(metadata file unavailable)");
				return;
			}
			var resolver = module.GetAssemblyResolver(options.DecompilerSettings.AutoLoadAssemblyReferences);
			var decompiler = new CSharpDecompiler(module, resolver, options.DecompilerSettings) {
				CancellationToken = options.CancellationToken,
				DebugInfoProvider = module.GetDebugInfoOrNull(),
			};
			SyntaxTree syntaxTree = decompiler.Decompile(handles);
			WriteCode(output, options.DecompilerSettings, syntaxTree, decompiler.TypeSystem);
		}

		static void WriteCode(ITextOutput output, DecompilerSettings settings, SyntaxTree syntaxTree, IDecompilerTypeSystem typeSystem)
		{
			syntaxTree.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
			output.IndentationString = settings.CSharpFormattingOptions.IndentationString;
			TokenWriter tokenWriter = new TextTokenWriter(output, settings, typeSystem);
			syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
		}
	}
}
