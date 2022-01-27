// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace ICSharpCode.ILSpy
{
	#region Toolbar
	public interface IToolbarCommandMetadata
	{
		string ToolbarIcon { get; }
		string ToolTip { get; }
		string ToolbarCategory { get; }
		object Tag { get; }
		double ToolbarOrder { get; }
	}

	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExportToolbarCommandAttribute : ExportAttribute, IToolbarCommandMetadata
	{
		public ExportToolbarCommandAttribute()
			: base("ToolbarCommand", typeof(ICommand))
		{
		}

		public string ToolTip { get; set; }
		public string ToolbarIcon { get; set; }
		public string ToolbarCategory { get; set; }
		public double ToolbarOrder { get; set; }
		public object Tag { get; set; }
	}
	#endregion

	#region Main Menu
	public interface IMainMenuCommandMetadata
	{
		string MenuID { get; }
		string MenuIcon { get; }
		string Header { get; }
		string ParentMenuID { get; }
		[Obsolete("Please use ParentMenuID instead. We decided to rename the property for clarity. It will be removed in ILSpy 8.0.")]
		string Menu { get; }
		string MenuCategory { get; }
		string InputGestureText { get; }
		bool IsEnabled { get; }
		double MenuOrder { get; }
	}

	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExportMainMenuCommandAttribute : ExportAttribute, IMainMenuCommandMetadata
	{
		public ExportMainMenuCommandAttribute()
			: base("MainMenuCommand", typeof(ICommand))
		{
		}
		/// <summary>
		/// Gets/Sets the ID of this menu item. Menu entries are not required to have an ID,
		/// however, setting it allows to declare nested menu structures.
		/// The built-in menus have the IDs "_File", "_View", "_Window" and "_Help".
		/// Plugin authors are advised to use GUIDs as identifiers to prevent conflicts.
		/// <para/>
		/// NOTE: Defining cycles (for example by accidentally setting <see cref="MenuID"/> equal to <see cref="ParentMenuID"/>)
		/// will lead to a stack-overflow and crash of ILSpy at startup.
		/// </summary>
		public string MenuID { get; set; }
		public string MenuIcon { get; set; }
		public string Header { get; set; }
		/// <summary>
		/// Gets/Sets the parent of this menu item. All menu items sharing the same parent will be displayed as sub-menu items.
		/// If this property is set to <see langword="null"/>, the menu item is displayed in the top-level menu.
		/// The built-in menus have the IDs "_File", "_View", "_Window" and "_Help".
		/// <para/>
		/// NOTE: Defining cycles (for example by accidentally setting <see cref="MenuID"/> equal to <see cref="ParentMenuID"/>)
		/// will lead to a stack-overflow and crash of ILSpy at startup.
		/// </summary>
		public string ParentMenuID { get; set; }
		[Obsolete("Please use ParentMenuID instead. We decided to rename the property for clarity. It will be removed in ILSpy 8.0.")]
		public string Menu { get => ParentMenuID; set => ParentMenuID = value; }
		public string MenuCategory { get; set; }
		public string InputGestureText { get; set; }
		public bool IsEnabled { get; set; } = true;
		public double MenuOrder { get; set; }
	}
	#endregion

	#region Tool Panes
	public interface IToolPaneMetadata
	{
		string ContentId { get; }
	}

	[MetadataAttribute]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ExportToolPaneAttribute : ExportAttribute, IToolPaneMetadata
	{
		public ExportToolPaneAttribute()
			: base("ToolPane", typeof(ViewModels.ToolPaneModel))
		{
		}

		public string ContentId { get; set; }
	}
	#endregion
}
