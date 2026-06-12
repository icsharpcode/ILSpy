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
using System.Collections.Generic;

using ICSharpCode.ILSpyX;

namespace ICSharpCode.ILSpy.Analyzers
{
	/// <summary>
	/// Sentinel row shown under an <see cref="AnalyzerSearchTreeNode"/> when the analyser
	/// threw. The user-facing <see cref="SharpTreeNode.Text"/> is a one-line summary, the
	/// <see cref="SharpTreeNode.ToolTip"/> shows the full exception (stack trace included),
	/// and <see cref="Details"/> is the same payload that
	/// <see cref="CopyAnalyzerErrorContextMenuEntry"/> writes to the clipboard.
	/// </summary>
	internal sealed class AnalyzerErrorNode : AnalyzerTreeNode
	{
		readonly string summary;

		public AnalyzerErrorNode(string summary, string details)
		{
			this.summary = summary;
			Details = details;
		}

		public AnalyzerErrorNode(Exception exception)
		{
			ArgumentNullException.ThrowIfNull(exception);
			this.summary = "Error: " + exception.Message;
			Details = exception.ToString();
		}

		public override object Text => summary;

		public override object Icon => Images.AssemblyWarning;

		public override object? ToolTip => Details;

		/// <summary>
		/// Full exception text — typically <see cref="Exception.ToString"/> — for the copy
		/// entry to put on the clipboard. Plain <see cref="SharpTreeNode.Text"/> is kept
		/// short so the row stays compact in the tree.
		/// </summary>
		public string Details { get; }

		public override bool HandleAssemblyListChanged(
			ICollection<LoadedAssembly> removedAssemblies,
			ICollection<LoadedAssembly> addedAssemblies) => true;
	}
}
