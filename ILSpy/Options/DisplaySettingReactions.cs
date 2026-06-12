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

using System.Collections.Generic;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// How a live <see cref="DisplaySettings"/> change has to be surfaced. The Options page is
	/// non-modal and applies immediately, so -- unlike the WPF host, which ran a full assembly-list
	/// <c>Refresh()</c> when its modal Options dialog closed -- every setting has to drive its own
	/// reaction or it silently does nothing.
	/// </summary>
	public enum DisplaySettingReaction
	{
		/// <summary>The text view applies it to the editor itself (font, line numbers, word wrap,
		/// brace highlighting, ...); the assembly tree and the decompile output are unaffected.</summary>
		EditorLive,

		/// <summary>Re-pull of member-node text only (the metadata-token suffix).</summary>
		TreeText,

		/// <summary>In-place rebuild of a tree subtree whose shape the setting changes
		/// (nested namespaces, hidden empty metadata tables).</summary>
		TreeShape,

		/// <summary>Baked into the decompiler / disassembler output, so the active tab must be
		/// re-decompiled for the change to show.</summary>
		Redecompile,

		/// <summary>Deliberately no model-side reaction (window chrome, search ordering).</summary>
		None,
	}

	/// <summary>
	/// Single source of truth classifying every <see cref="DisplaySettings"/> property by its live
	/// reaction. <c>AssemblyTreeModel</c> dispatches on it; <c>DisplaySettingsReactionTests</c>
	/// asserts the table covers every setting, so a newly-added one can't fall through unhandled.
	/// </summary>
	public static class DisplaySettingReactions
	{
		static readonly Dictionary<string, DisplaySettingReaction> reactions = new() {
			// Tree.
			[nameof(DisplaySettings.ShowMetadataTokens)] = DisplaySettingReaction.TreeText,
			[nameof(DisplaySettings.ShowMetadataTokensInBase10)] = DisplaySettingReaction.TreeText,
			[nameof(DisplaySettings.UseNestedNamespaceNodes)] = DisplaySettingReaction.TreeShape,
			[nameof(DisplaySettings.HideEmptyMetadataTables)] = DisplaySettingReaction.TreeShape,

			// Decompiler / disassembler output (see SettingsService.ApplyDisplaySettings +
			// GetIndentationString, and CSharpILMixedLanguage / ILLanguage for the IL-detail ones).
			[nameof(DisplaySettings.FoldBraces)] = DisplaySettingReaction.Redecompile,
			[nameof(DisplaySettings.ExpandMemberDefinitions)] = DisplaySettingReaction.Redecompile,
			[nameof(DisplaySettings.ExpandUsingDeclarations)] = DisplaySettingReaction.Redecompile,
			[nameof(DisplaySettings.ShowDebugInfo)] = DisplaySettingReaction.Redecompile,
			[nameof(DisplaySettings.ShowRawOffsetsAndBytesBeforeInstruction)] = DisplaySettingReaction.Redecompile,
			[nameof(DisplaySettings.DecodeCustomAttributeBlobs)] = DisplaySettingReaction.Redecompile,
			[nameof(DisplaySettings.IndentationUseTabs)] = DisplaySettingReaction.Redecompile,
			[nameof(DisplaySettings.IndentationSize)] = DisplaySettingReaction.Redecompile,
			[nameof(DisplaySettings.IndentationTabSize)] = DisplaySettingReaction.Redecompile,

			// Editor-only (DecompilerTextView applies these directly to the AvaloniaEdit control).
			[nameof(DisplaySettings.SelectedFont)] = DisplaySettingReaction.EditorLive,
			[nameof(DisplaySettings.SelectedFontSize)] = DisplaySettingReaction.EditorLive,
			[nameof(DisplaySettings.ShowLineNumbers)] = DisplaySettingReaction.EditorLive,
			[nameof(DisplaySettings.EnableWordWrap)] = DisplaySettingReaction.EditorLive,
			[nameof(DisplaySettings.HighlightCurrentLine)] = DisplaySettingReaction.EditorLive,
			[nameof(DisplaySettings.HighlightMatchingBraces)] = DisplaySettingReaction.EditorLive,

			// No model-side reaction.
			[nameof(DisplaySettings.StyleWindowTitleBar)] = DisplaySettingReaction.None,
			[nameof(DisplaySettings.SortResults)] = DisplaySettingReaction.None,
		};

		/// <summary>Every classified property name. The coverage test asserts this equals the set of
		/// settable <see cref="DisplaySettings"/> properties.</summary>
		public static IReadOnlyCollection<string> ClassifiedProperties => reactions.Keys;

		/// <summary>
		/// The reaction for <paramref name="propertyName"/>, or <see cref="DisplaySettingReaction.None"/>
		/// for an unclassified one (the coverage test prevents that case from reaching production).
		/// </summary>
		public static DisplaySettingReaction For(string? propertyName)
			=> propertyName != null && reactions.TryGetValue(propertyName, out var reaction)
				? reaction
				: DisplaySettingReaction.None;
	}
}
