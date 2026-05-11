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

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Settings;

namespace ILSpy.Options
{
	/// <summary>
	/// In-memory copy of the persisted settings, used by the Options tab so panel edits
	/// don't reach the live <see cref="ILSpy.SettingsService"/> until the user
	/// clicks Apply. Inherits the standard <see cref="SettingsServiceBase.GetSettings{T}"/>
	/// load+subscribe machinery from the shared base — sections materialise on first
	/// access reading from the same XML root as the parent service, but are independent
	/// object graphs. <see cref="Save"/> writes those graphs back through the parent's
	/// <see cref="ISettingsProvider"/> and asks the parent to reload so subscribers see
	/// the new values.
	/// </summary>
	public sealed class SettingsSnapshot : SettingsServiceBase
	{
		readonly SettingsService parent;

		public SettingsSnapshot(SettingsService parent, ISettingsProvider spySettings) : base(spySettings)
		{
			this.parent = parent;
		}

		public void Save()
		{
			SpySettings.Update(root => {
				foreach (var section in sections.Values)
				{
					SaveSection(section, root);
				}
			});

			parent.Reload();
		}
	}
}
