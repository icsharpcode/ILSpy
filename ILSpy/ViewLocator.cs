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
using System.Diagnostics.CodeAnalysis;

using Avalonia.Controls;
using Avalonia.Controls.Templates;

using Dock.Model.Core;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Compare;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.Search;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Single application-wide IDataTemplate that resolves a view-model to its view. Two
	/// resolution paths:
	///   1. Explicit <see cref="s_views"/> map (Dock-hosted view-models). Dispatched via
	///      a parameter-less ctor delegate — equivalent to MvvmSample's compile-time
	///      <c>[StaticViewLocator]</c>-generated dictionary.
	///   2. Convention fallback for any <see cref="ViewModelBase"/>-derived viewmodel:
	///      <c>SomeViewModel</c> → <c>SomeView</c> resolved via <see cref="Type.GetType(string)"/>.
	///
	/// <para>
	/// One locator instance handles every <see cref="IDockable"/> (and any
	/// <see cref="ViewModelBase"/>) rather than N typed
	/// <c>&lt;DataTemplate DataType="..."/&gt;</c> blocks in App.axaml: the owned-view
	/// resolver (<c>DockableViewRecycling</c>) builds each dockable's view through this map
	/// once, and a single shared template keeps view resolution uniform across the dock
	/// chrome. Mirrors <c>samples/DockMvvmSample</c>'s StaticViewLocator shape.
	/// </para>
	/// </summary>
	[RequiresUnreferencedCode(
		"Convention fallback uses reflection (Type.GetType) which may be trimmed.",
		Url = "https://docs.avaloniaui.net/docs/concepts/view-locator")]
	public class ViewLocator : IDataTemplate
	{
		static readonly Dictionary<Type, Func<Control>> s_views = new() {
			{ typeof(AssemblyTreeModel),       () => new AssemblyListPane() },
			{ typeof(SearchPaneModel),         () => new SearchPane() },
			{ typeof(AnalyzerTreeViewModel),   () => new AnalyzerTreeView() },
			{ typeof(ContentTabPage),          () => new ContentTabPageView() },
			{ typeof(DecompilerTabPageModel),  () => new DecompilerTextView() },
			{ typeof(MetadataTablePageModel),  () => new MetadataTablePage() },
			{ typeof(CompareTabPageModel),     () => new CompareView() },
			// OptionsPageModel is an ObservableObject, not a TabPageModel/ViewModelBase, and its
			// name doesn't fit the *ViewModel->*View convention, so it needs an explicit entry to
			// be resolvable as ContentTabPage.Content inside ContentTabPageView's ContentControl.
			{ typeof(OptionsPageModel),        () => new OptionsPageView() },
#if DEBUG
			{ typeof(DebugStepsPaneModel),     () => new DebugSteps() },
#endif
		};

		public Control? Build(object? param)
		{
			if (param is null)
				return null;
			if (s_views.TryGetValue(param.GetType(), out var ctor))
				return ctor();
			// Convention fallback: SomeViewModel -> SomeView. Type.GetType only searches the
			// calling assembly + corelib, so a plugin viewmodel (loaded into the AppDomain
			// at runtime by AppComposition.LoadPlugins) wouldn't be found via that path
			// alone. Walk every loaded assembly to find the matching View type.
			var name = param.GetType().FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
			var type = Type.GetType(name);
			if (type == null)
			{
				foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
				{
					type = asm.GetType(name);
					if (type != null)
						break;
				}
			}
			if (type != null)
				return (Control)Activator.CreateInstance(type)!;
			return new TextBlock { Text = "Not Found: " + name };
		}

		public bool Match(object? data)
		{
			if (data is null)
				return false;
			return data is IDockable
				|| data is ViewModelBase
				|| data is IOptionPage
				|| s_views.ContainsKey(data.GetType());
		}
	}
}
