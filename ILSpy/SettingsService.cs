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
using System.Composition;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;

using ILSpy.Options;

namespace ILSpy
{
	[Export]
	[Shared]
	public sealed class SettingsService : SettingsServiceBase
	{
		public SettingsService() : base(ILSpySettings.Load())
		{
		}

		public SessionSettings SessionSettings => GetSettings<SessionSettings>();

		public DecompilerSettings DecompilerSettings => GetSettings<DecompilerSettings>();

		public DisplaySettings DisplaySettings => GetSettings<DisplaySettings>();

		public MiscSettings MiscSettings => GetSettings<MiscSettings>();

		AssemblyListManager? assemblyListManager;
		public AssemblyListManager AssemblyListManager => assemblyListManager ??= new(SpySettings);

		/// <summary>Fired after <see cref="Reload"/> propagates fresh values from disk into the
		/// live section instances. Subscribers (e.g. the assembly tree, the decompiler view)
		/// observe this to refresh derived UI state.</summary>
		public event EventHandler? SettingsChanged;

		/// <summary>
		/// Creates a parallel <see cref="SettingsSnapshot"/> bound to the same XML root. The
		/// snapshot returns fresh, independent section instances on first access — Options
		/// panels mutate those copies so the live service stays untouched until
		/// <see cref="SettingsSnapshot.Save"/>.
		/// </summary>
		public SettingsSnapshot CreateSnapshot() => new(this, SpySettings);

		/// <summary>
		/// Reloads every materialised section from <see cref="SpySettings"/>. Called after a
		/// snapshot commits so the live instances pick up the just-written values without
		/// rebuilding the dictionary (subscribers see PropertyChanged for every changed field).
		/// </summary>
		public void Reload()
		{
			foreach (var section in sections.Values)
			{
				var element = SpySettings[section.SectionName];
				section.LoadFromXml(element);
			}
			SettingsChanged?.Invoke(this, EventArgs.Empty);
		}

		public void Save()
		{
			SpySettings.Update(root => {
				foreach (var section in sections.Values)
					SaveSection(section, root);
			});
		}
	}
}
