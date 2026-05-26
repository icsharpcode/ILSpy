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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Util;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.TreeView;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

#nullable enable

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Base class for entity nodes.
	/// </summary>
	public abstract class AnalyzerEntityTreeNode : AnalyzerTreeNode, IMemberTreeNode
	{
		static readonly string[] TypeHighlightingColorNames = {
			"ReferenceTypes",
			"ValueTypes",
			"InterfaceTypes",
			"EnumTypes",
			"DelegateTypes",
		};

		public abstract IEntity? Member { get; }

		public IEntity? SourceMember { get; protected set; }

		public override void ActivateItem(IPlatformRoutedEventArgs e)
		{
			e.Handled = true;
			if (this.Member == null || this.Member.MetadataToken.IsNil)
			{
				MessageBox.Show(Properties.Resources.CannotAnalyzeMissingRef, "ILSpy");
				return;
			}

			var module = this.Member.ParentModule?.MetadataFile;

			Debug.Assert(module != null);

			MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(module, this.Member.MetadataToken), this.SourceMember));
		}

		public override object? ToolTip => Member?.ParentModule?.MetadataFile?.FileName;

		/// <summary>
		/// Renders a member signature with semantic C# highlighting and bold type names for the Analyze tree view.
		/// </summary>
		protected object CreateHighlightedMemberText(string prefix, ConversionFlags conversionFlags)
		{
			var member = Member;
			if (member == null)
				return prefix;

			var fallbackText = prefix + Language.EntityToString(member, conversionFlags);
			IHighlightingDefinition? highlighting = Language.SyntaxHighlighting;
			if (highlighting == null)
				return fallbackText;

			var richText = TryCreateSemanticRichText(member, conversionFlags);
			if (richText == null)
				return CreateLexicalHighlightedText(fallbackText);

			var signature = richText.Text;
			var document = new TextDocument(signature);
			var textBlock = new TextBlock();
			if (prefix.Length > 0)
				textBlock.Inlines.Add(new Run(prefix));
			textBlock.Inlines.AddRange(richText.CreateRuns(document));
			ApplyBoldToTypeNames(textBlock, richText, document, highlighting, prefix.Length);
			return textBlock;
		}

		static RichText? TryCreateSemanticRichText(IEntity member, ConversionFlags conversionFlags)
		{
			if (Language is not CSharpLanguage)
				return null;

			var output = new StringWriter();
			var decoratedWriter = new TextWriterTokenWriter(output);
			var writer = new CSharpHighlightingTokenWriter(TokenWriter.InsertRequiredSpaces(decoratedWriter), locatable: decoratedWriter);
			var settings = SettingsService.DecompilerSettings.Clone();
			if (!Enum.TryParse(AssemblyTreeModel.CurrentLanguageVersion?.Version, out Decompiler.CSharp.LanguageVersion languageVersion))
				languageVersion = Decompiler.CSharp.LanguageVersion.Latest;
			settings.SetLanguageVersion(languageVersion);
			if (member is IMethod { IsLocalFunction: true })
			{
				writer.WriteIdentifier(Identifier.Create("(local)"));
			}
			new CSharpAmbience() {
				ConversionFlags = conversionFlags,
			}.ConvertSymbol(member, writer, settings.CSharpFormattingOptions);
			return new RichText(output.ToString(), writer.HighlightingModel);
		}

		static object CreateLexicalHighlightedText(string signature)
		{
			IHighlightingDefinition? highlighting = Language.SyntaxHighlighting;
			if (highlighting == null)
				return signature;

			var document = new TextDocument(signature);
			var richText = DocumentPrinter.ConvertTextDocumentToRichText(document, new DocumentHighlighter(document, highlighting)).ToRichTextModel();
			var textBlock = new TextBlock();
			textBlock.Inlines.AddRange(richText.CreateRuns(document));
			return textBlock;
		}

		static void ApplyBoldToTypeNames(TextBlock textBlock, RichText richText, TextDocument document, IHighlightingDefinition highlighting, int textOffset)
		{
			var typeColors = GetTypeHighlightingColors(highlighting);
			var model = richText.ToRichTextModel();
			if (model == null || typeColors.Count == 0)
				return;

			var boldRanges = model.GetHighlightedSections(0, document.TextLength)
				.Where(section => section.Color != null && typeColors.Contains(section.Color))
				.Select(section => (Start: textOffset + section.Offset, End: textOffset + section.Offset + section.Length))
				.ToList();
			if (boldRanges.Count == 0)
				return;

			int charOffset = 0;
			foreach (var run in textBlock.Inlines.OfType<Run>())
			{
				int runLength = run.Text?.Length ?? 0;
				int runStart = charOffset;
				int runEnd = charOffset + runLength;
				if (boldRanges.Any(range => runEnd > range.Start && runStart < range.End))
					run.FontWeight = FontWeights.Bold;
				charOffset += runLength;
			}
		}

		static HashSet<HighlightingColor> GetTypeHighlightingColors(IHighlightingDefinition highlighting)
		{
			var typeColors = new HashSet<HighlightingColor>();
			foreach (string colorName in TypeHighlightingColorNames)
			{
				var color = highlighting.GetNamedColor(colorName);
				if (color != null)
					typeColors.Add(color);
			}
			return typeColors;
		}

		public override bool HandleAssemblyListChanged(ICollection<LoadedAssembly> removedAssemblies, ICollection<LoadedAssembly> addedAssemblies)
		{
			if (Member == null)
			{
				return true;
			}
			foreach (LoadedAssembly asm in removedAssemblies)
			{
				if (this.Member.ParentModule!.MetadataFile == asm.GetMetadataFileOrNull())
					return false; // remove this node
			}
			this.Children.RemoveAll(
				delegate (SharpTreeNode n) {
					return n is not AnalyzerTreeNode an || !an.HandleAssemblyListChanged(removedAssemblies, addedAssemblies);
				});
			return true;
		}
	}
}
