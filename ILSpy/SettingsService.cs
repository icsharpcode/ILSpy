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
