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

using System;
using System.Linq;

using ICSharpCode.ILSpyX.TreeView;

using global::ILSpy.AssemblyTree;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Path-based tree navigation built on the production
/// <see cref="AssemblyTreeModel.FindNodeByPath(string[], bool)"/> walk. First segment is an
/// assembly short name (looked up in the assembly list, substituted with its file path);
/// the rest are the stable <c>ToString()</c> identities each tree node emits — namespace
/// name, type reflection name. Member-level identities go through ILAmbience and aren't
/// hand-authorable, so tests should drill to the type with <see cref="FindNode{T}"/> and
/// pick the member from <c>typeNode.Children</c> by <c>Name</c>.
/// </summary>
public static class TreeNavigation
{
	public static SharpTreeNode FindNode(this AssemblyTreeModel atm, params string[] segments)
	{
		ArgumentNullException.ThrowIfNull(atm);
		ArgumentNullException.ThrowIfNull(segments);
		if (segments.Length == 0)
			throw new ArgumentException("At least one segment (the assembly short name) is required.", nameof(segments));

		var path = ResolveAssembly(atm, segments);
		return atm.FindNodeByPath(path, returnBestMatch: false)
			?? Diagnose(atm, path, segments);
	}

	public static T FindNode<T>(this AssemblyTreeModel atm, params string[] segments)
		where T : SharpTreeNode
	{
		var node = FindNode(atm, segments);
		return node as T
			?? throw new InvalidOperationException(
				$"Path [{string.Join(" / ", segments)}] resolved to {node.GetType().Name}, expected {typeof(T).Name}.");
	}

	public static SharpTreeNode SelectNode(this AssemblyTreeModel atm, params string[] segments)
	{
		var node = FindNode(atm, segments);
		atm.SelectedItem = node;
		return node;
	}

	public static T SelectNode<T>(this AssemblyTreeModel atm, params string[] segments)
		where T : SharpTreeNode
	{
		var node = FindNode<T>(atm, segments);
		atm.SelectedItem = node;
		return node;
	}

	static string[] ResolveAssembly(AssemblyTreeModel atm, string[] segments)
	{
		var shortName = segments[0];
		var asm = atm.AssemblyList?.GetAssemblies()
			.FirstOrDefault(a => string.Equals(a.ShortName, shortName, StringComparison.Ordinal))
			?? throw new InvalidOperationException(
				$"Assembly with ShortName '{shortName}' is not in the active list. Loaded: " +
				$"[{string.Join(", ", atm.AssemblyList?.GetAssemblies().Select(a => a.ShortName) ?? Array.Empty<string>())}]");

		var translated = (string[])segments.Clone();
		translated[0] = asm.FileName;
		return translated;
	}

	static SharpTreeNode Diagnose(AssemblyTreeModel atm, string[] stablePath, string[] originalSegments)
	{
		var partial = atm.FindNodeByPath(stablePath, returnBestMatch: true);
		var children = partial == null
			? "(root null)"
			: string.Join(", ", partial.Children.Select(c => c.ToString()));
		throw new InvalidOperationException(
			$"FindNodeByPath could not resolve [{string.Join(" / ", originalSegments)}]. " +
			$"Reached '{partial?.ToString() ?? "(null)"}'; available children: [{children}].");
	}
}
