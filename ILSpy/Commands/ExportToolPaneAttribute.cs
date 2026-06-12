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
using System.Composition;

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Identifies which dock zone a tool pane belongs to. The dock factory groups exported
	/// panes by alignment when building the default layout.
	/// </summary>
	public enum ToolPaneAlignment
	{
		Left,
		Top,
		Right,
		Bottom,
	}

	/// <summary>
	/// Metadata view consumed by <see cref="ToolPaneRegistry"/>. System.Composition needs a
	/// concrete metadata class with public settable properties matching the attribute's.
	/// </summary>
	public class ToolPaneMetadata
	{
		public string? ContentId { get; set; }
		public ToolPaneAlignment Alignment { get; set; } = ToolPaneAlignment.Left;
		public double Order { get; set; }
		public bool IsVisibleByDefault { get; set; } = true;
	}

	/// <summary>
	/// Marks a <see cref="ToolPaneModel"/>-derived class as a discoverable tool pane. The
	/// dock factory iterates all such exports to build the default layout, so plugins can
	/// drop additional panes in without modifying core wiring.
	/// </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExportToolPaneAttribute : ExportAttribute
	{
		public ExportToolPaneAttribute()
			: base("ToolPane", typeof(ToolPaneModel))
		{
		}

		public string? ContentId { get; set; }
		public ToolPaneAlignment Alignment { get; set; } = ToolPaneAlignment.Left;
		public double Order { get; set; }
		public bool IsVisibleByDefault { get; set; } = true;
	}
}
