// Copyright (c) 2026 Selim Selimi
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

#nullable enable

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.CSharp.ProjectDecompiler
{
	/// <summary>
	/// The assembly references of a module together with the references of the application
	/// assemblies they resolve to, transitively. The C# compiler needs only the assemblies the IL
	/// names, but the XAML markup compiler resolves the type names in a page through the whole
	/// reference graph: a WPF project that lists only the direct references does not rebuild
	/// (MC1000, MC3050).
	/// Only assemblies shipped next to the module are walked. Framework assemblies are not: what
	/// they reference the SDK supplies, and which of them resolve, and to what, depends on the
	/// machine running the export, while the exported project has to come out the same everywhere.
	/// </summary>
	static class AssemblyReferenceClosure
	{
		/// <summary>
		/// Direct references first, in metadata order, then what they pull in, breadth-first.
		/// Each assembly name appears once. A reference that cannot be resolved is still listed,
		/// it just contributes nothing further; the project then names an assembly the build
		/// reports as missing, which is the state of affairs.
		/// </summary>
		public static IReadOnlyList<AssemblyReference> Of(MetadataFile module, IAssemblyResolver resolver)
		{
			string? applicationDirectory = DirectoryOf(module);
			var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { module.Name };
			var result = new List<AssemblyReference>();
			var pending = new Queue<AssemblyReference>();

			foreach (var reference in module.AssemblyReferences)
			{
				Add(reference);
			}
			while (pending.Count > 0)
			{
				MetadataFile? resolved;
				try
				{
					resolved = resolver.Resolve(pending.Dequeue());
				}
				catch (ResolutionException)
				{
					resolved = null;
				}
				if (resolved == null || !IsUnder(applicationDirectory, resolved))
				{
					continue;
				}
				foreach (var reference in resolved.AssemblyReferences)
				{
					Add(reference);
				}
			}
			return result;

			void Add(AssemblyReference reference)
			{
				if (seen.Add(reference.Name))
				{
					result.Add(reference);
					pending.Enqueue(reference);
				}
			}
		}

		static string? DirectoryOf(MetadataFile module)
		{
			return string.IsNullOrEmpty(module.FileName) ? null : Path.GetDirectoryName(Path.GetFullPath(module.FileName));
		}

		/// <summary>
		/// Whether the resolved assembly is shipped with the application: in the module's directory
		/// or below it. A module without a file has no application directory and nothing is walked.
		/// </summary>
		static bool IsUnder(string? applicationDirectory, MetadataFile resolved)
		{
			if (applicationDirectory == null || string.IsNullOrEmpty(resolved.FileName))
			{
				return false;
			}
			string directory = Path.GetDirectoryName(Path.GetFullPath(resolved.FileName)) ?? "";
			return directory.Equals(applicationDirectory, StringComparison.OrdinalIgnoreCase)
				|| directory.StartsWith(applicationDirectory + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
		}
	}
}
