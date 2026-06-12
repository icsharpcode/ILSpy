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

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy
{
	public static class ExtensionMethods
	{
		/// <summary>
		/// Returns the cached type system for <paramref name="file"/> derived from the user's
		/// current decompiler settings (see
		/// <see cref="SettingsService.CreateEffectiveDecompilerSettings"/>). Everything that
		/// resolves entities for display — tree nodes, search, metadata views — should go
		/// through this so it agrees with what decompilation produces and shares the
		/// per-module type-system cache instead of materialising another instance. Falls back
		/// to default settings when composition is unavailable (design-time, minimal test
		/// hosts).
		/// </summary>
		public static ICompilation? GetTypeSystemWithCurrentOptionsOrNull(this MetadataFile file)
		{
			var settings = AppEnv.AppComposition.TryGetExport<SettingsService>()?.CreateEffectiveDecompilerSettings()
				?? new Decompiler.DecompilerSettings();
			return file.GetTypeSystemWithDecompilerSettingsOrNull(settings);
		}
	}
}
