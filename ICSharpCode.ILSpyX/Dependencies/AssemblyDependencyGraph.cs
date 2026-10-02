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
	/// An assembly in an <see cref="AssemblyDependencyGraph"/>: either a resolved assembly (it has
	/// a <see cref="MetadataFile"/>) or a reference that could not be resolved.
	/// </summary>
	public sealed class AssemblyDependencyNode
	{
		public AssemblyDependencyNode(int index, string name, string fullName, Version? version,
			MetadataFile? metadataFile, bool isRoot)
		{
			Index = index;
			Name = name ?? throw new ArgumentNullException(nameof(name));
			FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
			Version = version;
			MetadataFile = metadataFile;
			IsRoot = isRoot;
		}

		/// <summary>Position of this node in <see cref="AssemblyDependencyGraph.Nodes"/>.</summary>
		public int Index { get; }

		/// <summary>Simple assembly name.</summary>
		public string Name { get; }

		/// <summary>Full assembly name (the definition's for a resolved node, the reference's otherwise).</summary>
		public string FullName { get; }

		public Version? Version { get; }

		/// <summary>The resolved metadata, or <see langword="null"/> for an unresolved reference.</summary>
		public MetadataFile? MetadataFile { get; }

		/// <summary>The file the assembly was loaded from, or null when unresolved.</summary>
		public string? FileName => MetadataFile?.FileName;

		public bool IsResolved => MetadataFile != null;

		/// <summary>True for the assemblies the graph was built from.</summary>
		public bool IsRoot { get; }

		public override string ToString() => FullName;
	}

	/// <summary>
	/// A "references" edge between two nodes of an <see cref="AssemblyDependencyGraph"/>.
	/// </summary>
	public sealed class AssemblyDependencyEdge
	{
		public AssemblyDependencyEdge(int from, int to, string referenceFullName, Version? referencedVersion, bool isVersionMismatch)
		{
			From = from;
			To = to;
			ReferenceFullName = referenceFullName ?? throw new ArgumentNullException(nameof(referenceFullName));
			ReferencedVersion = referencedVersion;
			IsVersionMismatch = isVersionMismatch;
		}

		/// <summary>Index of the referencing node.</summary>
		public int From { get; }

		/// <summary>Index of the referenced node.</summary>
		public int To { get; }

		/// <summary>The full name the reference row asks for.</summary>
		public string ReferenceFullName { get; }

		public Version? ReferencedVersion { get; }

		/// <summary>True when the reference was satisfied by a different version.</summary>
		public bool IsVersionMismatch { get; }
	}

	/// <summary>
	/// The assembly-reference graph reachable from a set of root assemblies: one node per distinct
	/// assembly (resolved assemblies keyed by file, unresolved references keyed by simple name) and
	/// one edge per referencing/referenced pair. Node order is breadth-first from the roots, with
	/// each assembly's references visited by name, so the same input always yields the same graph.
	/// </summary>
	public sealed class AssemblyDependencyGraph
	{
		internal AssemblyDependencyGraph(IReadOnlyList<AssemblyDependencyNode> nodes, IReadOnlyList<AssemblyDependencyEdge> edges)
		{
			Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
			Edges = edges ?? throw new ArgumentNullException(nameof(edges));
		}

		public IReadOnlyList<AssemblyDependencyNode> Nodes { get; }

		public IReadOnlyList<AssemblyDependencyEdge> Edges { get; }

		/// <summary>Edges in (From, To) form, as consumed by <see cref="LayeredLayout"/>.</summary>
		public IReadOnlyList<(int From, int To)> EdgePairs => Edges.Select(e => (e.From, e.To)).ToList();

		/// <summary>
		/// Builds the graph for <paramref name="roots"/>. Each reference is resolved with the
		/// resolver of the loaded assembly that owns the referencing file (or, for a file that is not
		/// in the list, with the resolver of the root it was reached from), exactly as the
		/// References folder of the assembly tree resolves it.
		/// </summary>
		/// <param name="transitive">When false only the roots' direct references are added.</param>
		public static AssemblyDependencyGraph Build(IEnumerable<LoadedAssembly> roots, bool transitive = true,
			CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(roots);
			var rootList = roots.ToList();
			var owners = new Dictionary<MetadataFile, LoadedAssembly>();
			var rootFiles = new List<MetadataFile>();
			foreach (var root in rootList)
			{
				var file = root.GetMetadataFileOrNull();
				if (file == null)
					continue;
				owners[file] = root;
				rootFiles.Add(file);
			}
			var resolvers = new Dictionary<LoadedAssembly, IAssemblyResolver>();

			return Build(rootFiles, Resolve, transitive, cancellationToken);

			MetadataFile? Resolve(MetadataFile referencing, AssemblyReference reference, MetadataFile reachedFrom)
			{
				if (!owners.TryGetValue(referencing, out var owner))
				{
					var root = owners.GetValueOrDefault(reachedFrom) ?? rootList[0];
					owner = root.AssemblyList.FindAssembly(referencing.FileName) ?? root;
					owners[referencing] = owner;
				}
				if (!resolvers.TryGetValue(owner, out var resolver))
				{
					resolver = owner.GetAssemblyResolver();
					resolvers[owner] = resolver;
				}
				try
				{
					return resolver.Resolve(reference);
				}
				catch (Exception ex) when (ex is not OperationCanceledException)
				{
					// A reference whose file exists but cannot be read is shown as unresolved.
					return null;
				}
			}
		}

		/// <summary>
		/// Core graph construction over raw metadata. <paramref name="resolve"/> receives the
		/// referencing file, the reference and the root the referencing file was reached from.
		/// </summary>
		internal static AssemblyDependencyGraph Build(IReadOnlyList<MetadataFile> roots,
			Func<MetadataFile, AssemblyReference, MetadataFile, MetadataFile?> resolve, bool transitive,
			CancellationToken cancellationToken)
		{
			var nodes = new List<AssemblyDependencyNode>();
			var edges = new List<AssemblyDependencyEdge>();
			var edgeKeys = new HashSet<(int, int)>();
			var byKey = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
			var queue = new Queue<(MetadataFile File, int Index, MetadataFile Root)>();

			foreach (var root in roots)
			{
				if (byKey.ContainsKey(FileKey(root)))
					continue;
				int index = AddResolved(root, isRoot: true);
				queue.Enqueue((root, index, root));
			}

			while (queue.Count > 0)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var (file, index, root) = queue.Dequeue();
				foreach (var reference in file.AssemblyReferences.OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase))
				{
					cancellationToken.ThrowIfCancellationRequested();
					var resolved = resolve(file, reference, root);
					int target;
					bool mismatch = false;
					if (resolved != null)
					{
						if (!byKey.TryGetValue(FileKey(resolved), out target))
						{
							target = AddResolved(resolved, isRoot: false);
							if (transitive)
								queue.Enqueue((resolved, target, root));
						}
						mismatch = AssemblyReferenceMatcher.IsVersionMismatch(reference, resolved);
					}
					else
					{
						string key = UnresolvedKey(reference.Name);
						if (!byKey.TryGetValue(key, out target))
						{
							target = nodes.Count;
							nodes.Add(new AssemblyDependencyNode(target, reference.Name, reference.FullName,
								reference.Version, metadataFile: null, isRoot: false));
							byKey.Add(key, target);
						}
					}
					if (target != index && edgeKeys.Add((index, target)))
						edges.Add(new AssemblyDependencyEdge(index, target, reference.FullName, reference.Version, mismatch));
				}
			}

			return new AssemblyDependencyGraph(nodes, edges);

			int AddResolved(MetadataFile file, bool isRoot)
			{
				int index = nodes.Count;
				var metadata = file.Metadata;
				string name = AssemblyReferenceMatcher.GetAssemblyName(metadata) ?? file.Name;
				string fullName = metadata.IsAssembly ? file.FullName : file.Name;
				nodes.Add(new AssemblyDependencyNode(index, name, fullName,
					AssemblyReferenceMatcher.GetAssemblyVersion(metadata), file, isRoot));
				byKey.Add(FileKey(file), index);
				return index;
			}
		}

		static string FileKey(MetadataFile file) => "file:" + file.FileName;

		static string UnresolvedKey(string name) => "unresolved:" + name;
	}
}
