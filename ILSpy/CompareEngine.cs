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

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.Compare
{
	/// <summary>
	/// Builds a merged <see cref="Entry"/> tree from two assemblies' type systems. The
	/// process is:
	/// <list type="number">
	/// <item>Walk each assembly's public surface (<see cref="CreateEntityTree"/>) into a
	///       per-side <see cref="Entry"/> tree keyed on C#-formatted signature strings.</item>
	/// <item>Recursively merge the two trees (<see cref="MergeTrees"/>) — paired entries
	///       get an <see cref="Entry.OtherEntity"/>; left-only entries are tagged
	///       <see cref="DiffKind.Remove"/>; right-only are tagged <see cref="DiffKind.Add"/>.</item>
	/// </list>
	/// Signatures are the keying mechanism because metadata tokens don't survive
	/// re-compilation; <see cref="EntryComparer"/> wraps the string-equality check.
	/// </summary>
	public static class CompareEngine
	{
		/// <summary>
		/// Walks <paramref name="typeSystem"/>'s public types and members and returns
		/// (flat-list-of-leaves, namespace-rooted-tree). The flat list is for the side
		/// that drives diff pairing in <see cref="MergeTrees"/>; the tree is the
		/// hierarchical view the UI consumes.
		/// </summary>
		public static (List<Entry> AllEntries, Entry Root) CreateEntityTree(ICompilation typeSystem)
		{
			var module = (MetadataModule)typeSystem.MainModule!;
			var metadata = module.MetadataFile!.Metadata;
			var ambience = new CSharpAmbience {
				ConversionFlags = ConversionFlags.All & ~ConversionFlags.ShowDeclaringType,
			};

			var results = new List<Entry>();
			var typeEntries = new Dictionary<TypeDefinitionHandle, Entry>();
			var namespaceEntries = new Dictionary<string, Entry>(System.StringComparer.Ordinal);
			var root = new Entry { Entity = module, Signature = module.FullAssemblyName };

			Entry? TryCreateEntry(IEntity entity)
			{
				if (entity.EffectiveAccessibility() != Accessibility.Public)
					return null;
				Entry? parent = null;
				// Nested entities are only included when their declaring type is itself a
				// recorded entry (i.e. public). Skipping otherwise drops the whole subtree —
				// matching the WPF behaviour that "compare" shows public-API only.
				if (entity.DeclaringTypeDefinition != null
					&& !typeEntries.TryGetValue((TypeDefinitionHandle)entity.DeclaringTypeDefinition.MetadataToken, out parent))
				{
					return null;
				}
				var entry = new Entry {
					Signature = ambience.ConvertSymbol(entity),
					Entity = entity,
					Parent = parent,
				};
				if (parent != null)
				{
					parent.Children ??= new List<Entry>();
					parent.Children.Add(entry);
				}
				return entry;
			}

			foreach (var typeDefHandle in metadata.TypeDefinitions)
			{
				var typeDef = module.GetDefinition(typeDefHandle);
				if (typeDef.EffectiveAccessibility() != Accessibility.Public)
					continue;
				var entry = typeEntries[typeDefHandle] = new Entry {
					Signature = ambience.ConvertSymbol(typeDef),
					Entity = typeDef,
				};
				if (typeDef.DeclaringType == null)
				{
					if (!namespaceEntries.TryGetValue(typeDef.Namespace, out var nsEntry))
					{
						namespaceEntries[typeDef.Namespace] = nsEntry = new Entry {
							Parent = root,
							Signature = typeDef.Namespace,
							Entity = ResolveNamespace(typeDef.Namespace, typeDef.ParentModule!)!,
						};
						root.Children ??= new List<Entry>();
						root.Children.Add(nsEntry);
					}
					entry.Parent = nsEntry;
					nsEntry.Children ??= new List<Entry>();
					nsEntry.Children.Add(entry);
				}
			}

			foreach (var fieldHandle in metadata.FieldDefinitions)
			{
				var entry = TryCreateEntry(module.GetDefinition(fieldHandle));
				if (entry != null)
					results.Add(entry);
			}
			foreach (var eventHandle in metadata.EventDefinitions)
			{
				var entry = TryCreateEntry(module.GetDefinition(eventHandle));
				if (entry != null)
					results.Add(entry);
			}
			foreach (var propertyHandle in metadata.PropertyDefinitions)
			{
				var entry = TryCreateEntry(module.GetDefinition(propertyHandle));
				if (entry != null)
					results.Add(entry);
			}
			foreach (var methodHandle in metadata.MethodDefinitions)
			{
				var methodDef = module.GetDefinition(methodHandle);
				// Accessor methods (get_X / set_X / add_X / remove_X) are folded into their
				// owning property / event row in the tree; comparing them separately would
				// double-count every property as four entries.
				if (methodDef.AccessorOwner != null)
					continue;
				var entry = TryCreateEntry(methodDef);
				if (entry != null)
					results.Add(entry);
			}

			return (results, root);
		}

		static INamespace? ResolveNamespace(string namespaceName, IModule module)
		{
			INamespace current = module.RootNamespace;
			var parts = namespaceName.Split('.');
			for (int i = 0; i < parts.Length; i++)
			{
				if (i == 0 && string.IsNullOrEmpty(parts[i]))
					continue;
				var next = current.GetChildNamespace(parts[i]);
				if (next != null)
					current = next;
				else
					return null;
			}
			return current;
		}

		/// <summary>
		/// Recursively merges two parallel entry trees into a single one. Each interior
		/// node's children are paired by <see cref="EntryComparer"/>; unmatched children
		/// keep their leaf <see cref="Entry.Kind"/> tag (Add / Remove) so the recursive
		/// summary on ancestors reflects the actual change shape.
		/// </summary>
		public static Entry MergeTrees(Entry a, Entry b)
		{
			var m = new Entry {
				Entity = a.Entity,
				OtherEntity = b.Entity,
				Signature = a.Signature,
			};
			if (a.Children?.Count > 0 && b.Children?.Count > 0)
			{
				var diff = CalculateDiff(a.Children, b.Children);
				m.Children ??= new List<Entry>();
				foreach (var (left, right) in diff)
				{
					if (left != null && right != null)
						m.Children.Add(MergeTrees(left, right));
					else if (left != null)
						m.Children.Add(left);
					else if (right != null)
						m.Children.Add(right);
					else
						Debug.Fail("CalculateDiff returned a (null, null) pair");
					m.Children[^1].Parent = m;
				}
			}
			else if (a.Children?.Count > 0)
			{
				m.Children ??= new List<Entry>();
				foreach (var child in a.Children)
				{
					child.Parent = m;
					m.Children.Add(child);
				}
			}
			else if (b.Children?.Count > 0)
			{
				m.Children ??= new List<Entry>();
				foreach (var child in b.Children)
				{
					child.Parent = m;
					m.Children.Add(child);
				}
			}
			return m;
		}

		/// <summary>
		/// Pairs <paramref name="left"/> and <paramref name="right"/> by signature.
		/// Returns one <c>(Left, Right)</c> tuple per slot: both non-null when matched;
		/// only Left → Remove (and tags the leaf <see cref="DiffKind.Remove"/>); only
		/// Right → Add (and tags the leaf <see cref="DiffKind.Add"/>).
		/// </summary>
		public static List<(Entry? Left, Entry? Right)> CalculateDiff(List<Entry> left, List<Entry> right)
		{
			var leftMap = new Dictionary<string, List<Entry>>();
			var rightMap = new Dictionary<string, List<Entry>>();
			foreach (var item in left)
			{
				if (leftMap.TryGetValue(item.Signature, out var bucket))
					bucket.Add(item);
				else
					leftMap[item.Signature] = new List<Entry> { item };
			}
			foreach (var item in right)
			{
				if (rightMap.TryGetValue(item.Signature, out var bucket))
					bucket.Add(item);
				else
					rightMap[item.Signature] = new List<Entry> { item };
			}

			var results = new List<(Entry? Left, Entry? Right)>();
			foreach (var (key, items) in leftMap)
			{
				if (rightMap.TryGetValue(key, out var rightEntries))
				{
					foreach (var item in items)
					{
						var other = rightEntries.Find(r => EntryComparer.Instance.Equals(r, item));
						results.Add((item, other));
						if (other == null)
							SetKind(item, DiffKind.Remove);
					}
				}
				else
				{
					foreach (var item in items)
					{
						SetKind(item, DiffKind.Remove);
						results.Add((item, null));
					}
				}
			}
			foreach (var (key, items) in rightMap)
			{
				if (leftMap.TryGetValue(key, out var leftEntries))
				{
					foreach (var item in items)
					{
						if (!leftEntries.Exists(l => EntryComparer.Instance.Equals(l, item)))
						{
							results.Add((null, item));
							SetKind(item, DiffKind.Add);
						}
					}
				}
				else
				{
					foreach (var item in items)
					{
						SetKind(item, DiffKind.Add);
						results.Add((null, item));
					}
				}
			}
			return results;
		}

		static void SetKind(Entry item, DiffKind kind)
		{
			if (item.Children?.Count > 0)
			{
				foreach (var child in item.Children)
					SetKind(child, kind);
			}
			else
			{
				item.Kind = kind;
			}
		}
	}
}
