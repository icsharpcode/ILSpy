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

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Plugin contract for individual panels in the Options tab. Each implementation is
	/// MEF-discovered via <see cref="ExportOptionPageAttribute"/>; the host calls
	/// <see cref="Load"/> with the live settings service when the tab opens, and
	/// <see cref="LoadDefaults"/> when the user clicks the per-panel reset button. Panels
	/// bind directly to the live section instances — every toggle is immediately visible
	/// to other consumers (the decompiler view, the tree, etc.). XML persistence rides on
	/// <see cref="ICSharpCode.ILSpy.SettingsService.Save"/> at app exit.
	/// </summary>
	public interface IOptionPage
	{
		string Title { get; }

		void Load(SettingsService settings);

		void LoadDefaults();
	}
}
