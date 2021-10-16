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
