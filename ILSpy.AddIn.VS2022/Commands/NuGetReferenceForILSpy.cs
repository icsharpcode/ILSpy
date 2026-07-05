// Copyright (c) 2018 Andreas Weizel
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EnvDTE;

using Microsoft.VisualStudio.Shell;

using VSLangProj;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	/// <summary>
	/// Represents a NuGet package item in Solution Explorer, which can be opened in ILSpy.
	/// </summary>
	class NuGetReferenceForILSpy
	{
		ProjectItem projectItem;

		NuGetReferenceForILSpy(ProjectItem projectItem)
		{
			this.projectItem = projectItem;
		}

		/// <summary>
		/// Detects whether the given selected item represents a supported project.
		/// </summary>
		/// <param name="itemData">Data object of selected item to check.</param>
		/// <returns><see cref="NuGetReferenceForILSpy"/> instance or <c>null</c>, if item is not a supported project.</returns>
		public static NuGetReferenceForILSpy Detect(object itemData)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (itemData is ProjectItem projectItem)
			{
				var properties = Utils.GetProperties(projectItem.Properties, "Type", "ExtenderCATID");
				if (((properties[0] as string) == "Package") || ((properties[1] as string) == PrjBrowseObjectCATID.prjCATIDCSharpReferenceBrowseObject))
				{
					return new NuGetReferenceForILSpy(projectItem);
				}
			}

			return null;
		}

		/// <summary>
		/// If possible retrieves parameters to use for launching ILSpy instance.
		/// </summary>
		/// <returns>Parameters object or <c>null, if not applicable.</c></returns>
		public ILSpyParameters GetILSpyParameters()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			var properties = Utils.GetProperties(projectItem.Properties, "Name", "Version", "Path");
			if (properties[0] != null && properties[1] != null && properties[2] != null)
			{
				return new ILSpyParameters(new[] { $"{properties[2]}\\{properties[0]}.{properties[1]}.nupkg" });
			}

			return null;
		}
	}
}
