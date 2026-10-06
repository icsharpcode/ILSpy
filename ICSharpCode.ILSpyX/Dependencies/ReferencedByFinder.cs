// Copyright (c) 2026 Piero Viano
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
using System.Threading;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpyX.Dependencies
{
	/// <summary>
	/// One loaded assembly that references a given target assembly, together with the reference
	/// row that names it.
	/// </summary>
	public sealed class ReferencingAssembly
	{
		public ReferencingAssembly(LoadedAssembly assembly, MetadataFile module, AssemblyReference reference, bool isVersionMismatch)
		{
			Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
			Module = module ?? throw new ArgumentNullException(nameof(module));
			Reference = reference ?? throw new ArgumentNullException(nameof(reference));
			IsVersionMismatch = isVersionMismatch;
		}

		/// <summary>The referencing assembly.</summary>
		public LoadedAssembly Assembly { get; }

		/// <summary>The metadata of <see cref="Assembly"/>.</summary>
		public MetadataFile Module { get; }

		/// <summary>The assembly reference in <see cref="Module"/> that names the target.</summary>
		public AssemblyReference Reference { get; }

		/// <summary>True when the reference asks for a different version than the target defines.</summary>
		public bool IsVersionMismatch { get; }
	}

	/// <summary>
	/// Finds the assemblies of a list that reference a given assembly (the reverse direction of the
	/// References folder). A reference matches by simple name and public key token; the version may
	/// differ and is reported through <see cref="ReferencingAssembly.IsVersionMismatch"/>.
	/// </summary>
	public static class ReferencedByFinder
	{
		/// <summary>
		/// Returns every assembly in <paramref name="candidates"/> other than <paramref name="target"/>
		/// itself that has an assembly reference matching <paramref name="target"/>, ordered by
		/// short name. Assemblies that failed to load are skipped.
		/// </summary>
		public static IReadOnlyList<ReferencingAssembly> FindReferencingAssemblies(
			IEnumerable<LoadedAssembly> candidates, MetadataFile target, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(candidates);
			ArgumentNullException.ThrowIfNull(target);
			var results = new List<ReferencingAssembly>();
			if (!target.IsAssembly)
				return results;
			foreach (var candidate in candidates)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var module = candidate.GetMetadataFileOrNull();
				if (module == null || ReferenceEquals(module, target) || !module.IsAssembly)
					continue;
				if (string.Equals(module.FileName, target.FileName, StringComparison.OrdinalIgnoreCase))
					continue;
				foreach (var reference in module.AssemblyReferences)
				{
					if (reference.IsReferenceTo(target.Metadata))
					{
						results.Add(new ReferencingAssembly(candidate, module, reference,
							reference.IsVersionMismatch(target.Metadata)));
						break;
					}
				}
			}
			return results
				.OrderBy(r => r.Assembly.ShortName, StringComparer.OrdinalIgnoreCase)
				.ThenBy(r => r.Assembly.FileName, StringComparer.OrdinalIgnoreCase)
				.ToList();
		}
	}
}
