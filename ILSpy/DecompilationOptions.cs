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
using System.Threading;

using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Options passed to the decompiler.
	/// </summary>
	public class DecompilationOptions
	{
		/// <summary>
		/// Gets whether a full decompilation (all members recursively) is desired.
		/// If this option is false, language bindings are allowed to show the only headers of the decompiled element's children.
		/// </summary>
		public bool FullDecompilation { get; set; }

		/// <summary>
		/// Gets/Sets the directory into which the project is saved.
		/// </summary>
		public string SaveAsProjectDirectory { get; set; }

		/// <summary>
		/// Gets/sets whether invalid identifiers should be escaped (and therefore the code be made compilable).
		/// This setting is ignored in case <see cref="SaveAsProjectDirectory"/> is set.
		/// </summary>
		public bool EscapeInvalidIdentifiers { get; set; }

		/// <summary>
		/// Gets the cancellation token that is used to abort the decompiler.
		/// </summary>
		/// <remarks>
		/// Decompilers should regularly call <c>options.CancellationToken.ThrowIfCancellationRequested();</c>
		/// to allow for cooperative cancellation of the decompilation task.
		/// </remarks>
		public CancellationToken CancellationToken { get; set; }

		/// <summary>
		/// Gets the settings for the decompiler.
		/// </summary>
		public Decompiler.DecompilerSettings DecompilerSettings { get; private set; }

		/// <summary>
		/// Gets/sets an optional state of a decompiler text view.
		/// </summary>
		/// <remarks>
		/// This state is used to restore test view's state when decompilation is started by Go Back/Forward action.
		/// </remarks>
		public TextView.DecompilerTextViewState TextViewState { get; set; }

		/// <summary>
		/// Used internally for debugging.
		/// </summary>
		internal int StepLimit = int.MaxValue;
		internal bool IsDebug = false;

		public DecompilationOptions(LanguageVersion version, Decompiler.DecompilerSettings settings, DisplaySettings displaySettings)
		{
			if (!Enum.TryParse(version?.Version, out Decompiler.CSharp.LanguageVersion languageVersion))
				languageVersion = Decompiler.CSharp.LanguageVersion.Latest;
			var newSettings = this.DecompilerSettings = settings.Clone();
			newSettings.SetLanguageVersion(languageVersion);
			newSettings.ExpandMemberDefinitions = displaySettings.ExpandMemberDefinitions;
			newSettings.ExpandUsingDeclarations = displaySettings.ExpandUsingDeclarations;
			newSettings.FoldBraces = displaySettings.FoldBraces;
			newSettings.ShowDebugInfo = displaySettings.ShowDebugInfo;
			newSettings.CSharpFormattingOptions.IndentationString = GetIndentationString(displaySettings);
		}

		private string GetIndentationString(DisplaySettings displaySettings)
		{
			if (displaySettings.IndentationUseTabs)
			{
				int numberOfTabs = displaySettings.IndentationSize / displaySettings.IndentationTabSize;
				int numberOfSpaces = displaySettings.IndentationSize % displaySettings.IndentationTabSize;
				return new string('\t', numberOfTabs) + new string(' ', numberOfSpaces);
			}
			return new string(' ', displaySettings.IndentationSize);
		}
	}
}
