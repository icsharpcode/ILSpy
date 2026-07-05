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

using VSLangProj;

namespace ICSharpCode.ILSpy.AddIn.Commands
{
	/// <summary>
	/// Represents an assembly reference item in Solution Explorer, which can be opened in ILSpy.
	/// </summary>
	class AssemblyReferenceForILSpy
	{
		Reference reference;

		AssemblyReferenceForILSpy(Reference reference)
		{
			this.reference = reference;
		}

		/// <summary>
		/// Detects whether the given selected item represents a supported project.
		/// </summary>
		/// <param name="itemData">Data object of selected item to check.</param>
		/// <returns><see cref="AssemblyReferenceForILSpy"/> instance or <c>null</c>, if item is not a supported project.</returns>
		public static AssemblyReferenceForILSpy Detect(object itemData)
		{
			return (itemData is Reference reference) ? new AssemblyReferenceForILSpy(reference) : null;
		}

		/// <summary>
		/// If possible retrieves parameters to use for launching ILSpy instance.
		/// </summary>
		/// <param name="projectReferences">List of current project's references.</param>
		/// <returns>Parameters object or <c>null, if not applicable.</c></returns>
		public ILSpyParameters GetILSpyParameters(Dictionary<string, DetectedReference> projectReferences)
		{
			if (projectReferences.TryGetValue(reference.Name, out var refentry))
				return new ILSpyParameters(new[] { refentry.AssemblyFile });

			return null;
		}
	}
}
