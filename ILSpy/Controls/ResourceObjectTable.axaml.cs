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

using global::Avalonia.Controls;

namespace ICSharpCode.ILSpy.Controls
{
	/// <summary>
	/// Tabular view of non-string entries inside a <c>.resources</c> file (ints, doubles,
	/// serialized objects, …). Used as an inline UI element from
	/// <see cref="TreeNodes.ResourcesFileTreeNode"/>'s decompilation output.
	/// </summary>
	public partial class ResourceObjectTable : UserControl
	{
		public ResourceObjectTable()
		{
			InitializeComponent();
		}

		public ResourceObjectTable(IEnumerable<SerializedObjectRepresentation> entries) : this()
		{
			EntriesGrid.ItemsSource = entries;
		}
	}

	/// <summary>Row model for <see cref="ResourceObjectTable"/>.</summary>
	public sealed class SerializedObjectRepresentation
	{
		public SerializedObjectRepresentation(string key, string type, string value)
		{
			Key = key;
			Type = type;
			Value = value;
		}

		public string Key { get; }
		public string Type { get; }
		public string Value { get; }
	}
}
