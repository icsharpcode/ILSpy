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

using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Plug-in contract for resolving non-default reference protocols (e.g.
	/// <c>metadata://</c>) to tree nodes. Implementations are MEF-discovered via
	/// <c>[Export(typeof(IProtocolHandler))]</c> and consulted by
	/// <see cref="Docking.DockWorkspace"/>'s navigation router; the first handler to
	/// return a non-<c>null</c> node wins.
	/// </summary>
	public interface IProtocolHandler
	{
		/// <summary>
		/// Resolves a reference to a tree node, or returns <c>null</c> to signal "not my
		/// protocol". <paramref name="newTabPage"/> is set to <c>true</c> when the handler
		/// wants the host to carve out a new tab instead of reusing the active one.
		/// </summary>
		ILSpyTreeNode? Resolve(string protocol, MetadataFile module, Handle handle, out bool newTabPage);
	}
}
