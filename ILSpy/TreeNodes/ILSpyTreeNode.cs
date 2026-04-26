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

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpyX.TreeView;

using ILSpy.AppEnv;
using ILSpy.Languages;

namespace ILSpy.TreeNodes
{
	public abstract class ILSpyTreeNode : SharpTreeNode
	{
		static LanguageService? cachedLanguageService;

		/// <summary>
		/// Resolves the LanguageService from the MEF composition. Tree nodes use this to access
		/// the active <see cref="Language"/> for formatting their <see cref="SharpTreeNode.Text"/>.
		/// </summary>
		protected static LanguageService LanguageService
			=> cachedLanguageService ??= AppComposition.Current.GetExport<LanguageService>();

		public Language Language => LanguageService.CurrentLanguage;

		/// <summary>
		/// Renders this node's decompiled representation to <paramref name="output"/> using
		/// <paramref name="language"/>. Default writes a stub comment so any node we forgot to
		/// override still produces *something*.
		/// </summary>
		public virtual void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, Text?.ToString() ?? GetType().Name);
		}
	}
}
