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
using System.Windows.Input;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Concrete metadata view for main-menu commands. System.Composition (lightweight MEF)
	/// requires metadata views to be a concrete class with a parameterless or
	/// IDictionary-typed constructor — interfaces aren't supported as views like they are in
	/// the full <c>System.ComponentModel.Composition</c>. The runtime auto-populates these
	/// properties from the matching properties on the export's <see cref="MetadataAttribute"/>.
	/// </summary>
	public class MainMenuCommandMetadata
	{
		public string? MenuID { get; set; }
		public string? MenuIcon { get; set; }
		public string? Header { get; set; }
		public string? ParentMenuID { get; set; }
		public string? MenuCategory { get; set; }
		public string? InputGestureText { get; set; }
		public bool IsEnabled { get; set; } = true;
		public double MenuOrder { get; set; }
	}

	/// <summary>
	/// Marks a command for inclusion in the main menu. The metadata drives where in the
	/// menu hierarchy the entry appears, what icon / accelerator it carries, and which
	/// command instance is invoked.
	/// </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExportMainMenuCommandAttribute : ExportAttribute
	{
		public ExportMainMenuCommandAttribute()
			: base("MainMenuCommand", typeof(ICommand))
		{
		}

		public string? MenuID { get; set; }
		public string? MenuIcon { get; set; }
		public string? Header { get; set; }
		public string? ParentMenuID { get; set; }
		public string? MenuCategory { get; set; }
		public string? InputGestureText { get; set; }
		public bool IsEnabled { get; set; } = true;
		public double MenuOrder { get; set; }
	}

	/// <summary>
	/// Metadata view for toolbar commands.
	/// </summary>
	public class ToolbarCommandMetadata
	{
		public string? ToolbarIcon { get; set; }
		public string? ToolTip { get; set; }
		public string? ToolbarCategory { get; set; }
		public double ToolbarOrder { get; set; }
	}

	/// <summary>
	/// Marks a command for inclusion in the main toolbar. A single command class can carry
	/// both this attribute and <see cref="ExportMainMenuCommandAttribute"/>, surfacing the
	/// same handler in the menu and the toolbar.
	/// </summary>
	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExportToolbarCommandAttribute : ExportAttribute
	{
		public ExportToolbarCommandAttribute()
			: base("ToolbarCommand", typeof(ICommand))
		{
		}

		public string? ToolbarIcon { get; set; }
		public string? ToolTip { get; set; }
		public string? ToolbarCategory { get; set; }
		public double ToolbarOrder { get; set; }
	}
}
