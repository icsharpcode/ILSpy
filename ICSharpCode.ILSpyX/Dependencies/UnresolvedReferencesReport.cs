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
	/// <summary>An assembly reference that could not be resolved, with its resolution log.</summary>
	public sealed class UnresolvedReference
	{
		public UnresolvedReference(AssemblyReference reference, IReadOnlyList<(MessageKind Kind, string Message)> messages)
		{
			Reference = reference ?? throw new ArgumentNullException(nameof(reference));
			Messages = messages ?? throw new ArgumentNullException(nameof(messages));
		}

		public AssemblyReference Reference { get; }

		/// <summary>The probing messages the assembly resolver logged for this reference.</summary>
		public IReadOnlyList<(MessageKind Kind, string Message)> Messages { get; }
	}

	/// <summary>The unresolved references of one referencing assembly.</summary>
	public sealed class UnresolvedReferenceGroup
	{
		public UnresolvedReferenceGroup(LoadedAssembly assembly, IReadOnlyList<UnresolvedReference> references)
		{
			Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
			References = references ?? throw new ArgumentNullException(nameof(references));
		}

		public LoadedAssembly Assembly { get; }

		public IReadOnlyList<UnresolvedReference> References { get; }
	}

	/// <summary>
	/// Collects every assembly reference that does not resolve, grouped by the referencing
	/// assembly. Resolution goes through each assembly's own resolver (the same one the References
	/// folder uses), so the probing log it records is what the report shows.
	/// </summary>
	public static class UnresolvedReferencesReport
	{
		/// <summary>
		/// Resolves every assembly reference of <paramref name="assemblies"/> and returns the groups
		/// that have at least one unresolved reference, in input order. Assemblies that failed to
		/// load are skipped. Must not run on the UI thread: resolving may probe the file system.
		/// </summary>
		public static IReadOnlyList<UnresolvedReferenceGroup> Collect(IEnumerable<LoadedAssembly> assemblies,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(assemblies);
			var groups = new List<UnresolvedReferenceGroup>();
			foreach (var assembly in assemblies.ToList())
			{
				cancellationToken.ThrowIfCancellationRequested();
				var module = assembly.GetMetadataFileOrNull();
				if (module == null)
					continue;
				var resolver = assembly.GetAssemblyResolver();
				var unresolved = new List<UnresolvedReference>();
				foreach (var reference in module.AssemblyReferences)
				{
					cancellationToken.ThrowIfCancellationRequested();
					MetadataFile? resolved;
					try
					{
						resolved = resolver.Resolve(reference);
					}
					catch (Exception ex) when (ex is not OperationCanceledException)
					{
						resolved = null;
					}
					if (resolved != null)
						continue;
					IReadOnlyList<(MessageKind, string)> messages =
						assembly.LoadedAssemblyReferencesInfo.TryGetInfo(reference.FullName, out var info)
							? info.Messages.ToList()
							: Array.Empty<(MessageKind, string)>();
					unresolved.Add(new UnresolvedReference(reference, messages));
				}
				if (unresolved.Count > 0)
					groups.Add(new UnresolvedReferenceGroup(assembly, unresolved));
			}
			return groups;
		}
	}
}
