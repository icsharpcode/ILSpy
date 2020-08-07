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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Microsoft.VisualStudio.Composition;

namespace ICSharpCode.ILSpy
{
	public static class Languages
	{
		// Start with a dummy list with an IL entry so that crashes
		// in Initialize() (e.g. due to invalid plugins) don't lead to
		// confusing follow-up errors in GetLanguage().
		private static ReadOnlyCollection<Language> allLanguages = new ReadOnlyCollection<Language>(
			new Language[] { new ILLanguage() });

		/// <summary>
		/// A list of all languages.
		/// </summary>
		public static ReadOnlyCollection<Language> AllLanguages
		{
			get { return allLanguages; }
		}

		internal static void Initialize(ExportProvider ep)
		{
			List<Language> languages = new List<Language>();
			languages.AddRange(ep.GetExportedValues<Language>());
			languages.Sort((a, b) => a.Name.CompareTo(b.Name));
			#if DEBUG
			languages.AddRange(ILAstLanguage.GetDebugLanguages());
			languages.AddRange(CSharpLanguage.GetDebugLanguages());
			#endif
			allLanguages = languages.AsReadOnly();
		}

		/// <summary>
		/// Gets a language using its name.
		/// If the language is not found, C# is returned instead.
		/// </summary>
		public static Language GetLanguage(string name)
		{
			return AllLanguages.FirstOrDefault(l => l.Name == name) ?? AllLanguages.First();
		}

		static ILLanguage ilLanguage;
		public static ILLanguage ILLanguage {
			get {
				if (ilLanguage == null) {
					ilLanguage = (ILLanguage)GetLanguage("IL");
				}
				return ilLanguage;
			}
		}
	}
}
