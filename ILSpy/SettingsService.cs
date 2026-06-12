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
using System.ComponentModel;
using System.Composition;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;

using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.Util;

namespace ICSharpCode.ILSpy
{
	[Export]
	[Shared]
	public sealed class SettingsService : SettingsServiceBase
	{
		public SettingsService() : base(ILSpySettings.Load())
		{
		}

		/// <summary>
		/// Override the base's per-section PropertyChanged forwarder to also fan out via
		/// <see cref="MessageBus{T}"/>. Panes (Debug Steps, future updater banner, …) that
		/// react to settings changes without holding a reference to the SettingsService
		/// subscribe on <see cref="SettingsChangedEventArgs"/>. The sender is the section
		/// instance (DecompilerSettings / DisplaySettings / LanguageSettings / …) so
		/// subscribers can dispatch on which section changed.
		/// </summary>
		protected override void Section_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			base.Section_PropertyChanged(sender, e);
			MessageBus.Send(sender, new SettingsChangedEventArgs(e));
		}

		public SessionSettings SessionSettings => GetSettings<SessionSettings>();

		public DecompilerSettings DecompilerSettings => GetSettings<DecompilerSettings>();

		public DisplaySettings DisplaySettings => GetSettings<DisplaySettings>();

		public MiscSettings MiscSettings => GetSettings<MiscSettings>();

		/// <summary>
		/// Returns the effective decompiler settings a decompilation started right now would
		/// use: a clone of <see cref="DecompilerSettings"/> with the Display options bridged in
		/// and the language version selected in the toolbar applied. Mutating the returned
		/// instance does not affect the persisted settings.
		/// </summary>
		public Decompiler.DecompilerSettings CreateEffectiveDecompilerSettings()
		{
			var settings = DecompilerSettings.Clone();
			ApplyDisplaySettings(settings, DisplaySettings);
			// No LanguageService (non-C# language or minimal test host) leaves the version
			// null, so the language version falls back to Latest below.
			var version = AppEnv.AppComposition.TryGetExport<Languages.LanguageService>()?.CurrentVersion;
			if (Enum.TryParse<Decompiler.CSharp.LanguageVersion>(version?.Version, out var languageVersion))
				settings.SetLanguageVersion(languageVersion);
			else
				settings.SetLanguageVersion(Decompiler.CSharp.LanguageVersion.Latest);
			return settings;
		}

		/// <summary>
		/// Bridges the Display options that affect decompiler output into <paramref name="settings"/>:
		/// the fold-expansion flags (TextTokenWriter reads them to set each fold's DefaultClosed),
		/// brace folding, debug-symbol info, and the indentation string. Without this these Display
		/// options would have no effect on the produced source.
		/// </summary>
		internal static void ApplyDisplaySettings(Decompiler.DecompilerSettings settings, DisplaySettings display)
		{
			settings.ExpandUsingDeclarations = display.ExpandUsingDeclarations;
			settings.ExpandMemberDefinitions = display.ExpandMemberDefinitions;
			settings.FoldBraces = display.FoldBraces;
			settings.ShowDebugInfo = display.ShowDebugInfo;
			settings.CSharpFormattingOptions.IndentationString = GetIndentationString(display);
		}

		static string GetIndentationString(DisplaySettings display)
		{
			if (display.IndentationUseTabs)
			{
				int tabs = display.IndentationSize / display.IndentationTabSize;
				int spaces = display.IndentationSize % display.IndentationTabSize;
				return new string('\t', tabs) + new string(' ', spaces);
			}
			return new string(' ', display.IndentationSize);
		}

		public Updates.UpdateSettings UpdateSettings => GetSettings<Updates.UpdateSettings>();

		AssemblyListManager? assemblyListManager;
		public AssemblyListManager AssemblyListManager => assemblyListManager ??= new(SpySettings) {
			ApplyWinRTProjections = DecompilerSettings.ApplyWindowsRuntimeProjections,
			UseDebugSymbols = DecompilerSettings.UseDebugSymbols,
		};

		/// <summary>
		/// Persists every materialised section to disk. Options panels bind two-way to the
		/// live instances so no in-memory commit is needed — <see cref="Save"/> just flushes
		/// current state to XML. Called at app-exit from <c>App.axaml.cs</c>.
		/// </summary>
		public void Save()
		{
			SpySettings.Update(root => {
				foreach (var section in sections.Values)
					SaveSection(section, root);
			});
		}
	}
}
