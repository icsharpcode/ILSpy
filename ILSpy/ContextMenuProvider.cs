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
using System.Composition;
using System.Linq;

using Avalonia.Controls;

using ICSharpCode.ILSpy.AppEnv;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// One discovered context-menu entry: the materialised <see cref="IContextMenuEntry"/>
	/// implementation paired with its metadata. Abstracts over <c>System.Composition</c>'s
	/// <see cref="ExportFactory{T, TMetadata}"/> so call sites and tests can both feed entries
	/// into <see cref="ContextMenuProvider.Build"/> without round-tripping through MEF.
	/// </summary>
	public interface IContextMenuEntryExport
	{
		IContextMenuEntry Value { get; }
		ContextMenuEntryMetadata Metadata { get; }
	}

	/// <summary>
	/// MEF aggregator for <see cref="ExportContextMenuEntryAttribute"/>-tagged entries.
	/// Mirrors the <c>MainMenuCommandRegistry</c> pattern — XAML-instantiated views can
	/// retrieve this via <c>AppComposition.Current.GetExport&lt;ContextMenuEntryRegistry&gt;()</c>.
	/// </summary>
	[Export]
	[Shared]
	public sealed class ContextMenuEntryRegistry
	{
		[ImportingConstructor]
		public ContextMenuEntryRegistry(
			[ImportMany("ContextMenuEntry")] IEnumerable<ExportFactory<IContextMenuEntry, ContextMenuEntryMetadata>> entries)
		{
			Entries = entries.Select(MefExport.From).ToArray();
		}

		public IReadOnlyList<IContextMenuEntryExport> Entries { get; }

		// Adapter that exposes a System.Composition ExportFactory as the simpler
		// IContextMenuEntryExport surface the provider consumes. CreateExport is invoked once
		// at registry-construction time — we want stable, shared instances since each entry
		// is exported as [Shared] anyway.
		sealed class MefExport(ExportFactory<IContextMenuEntry, ContextMenuEntryMetadata> factory) : IContextMenuEntryExport
		{
			readonly Lazy<IContextMenuEntry> value = new(() => factory.CreateExport().Value);

			public IContextMenuEntry Value => value.Value;
			public ContextMenuEntryMetadata Metadata { get; } = factory.Metadata;

			public static IContextMenuEntryExport From(ExportFactory<IContextMenuEntry, ContextMenuEntryMetadata> factory)
				=> new MefExport(factory);
		}
	}

	/// <summary>
	/// Pure-function builder: turns a sequence of context-menu entries into a populated
	/// <see cref="ContextMenu"/>, given a <see cref="TextViewContext"/>. Entries hidden by
	/// <see cref="IContextMenuEntry.IsVisible"/> are dropped; entries disabled by
	/// <see cref="IContextMenuEntry.IsEnabled"/> appear greyed-out. Categories are separated
	/// by <see cref="Separator"/>s; sub-menus are nested via the <c>MenuID</c>/<c>ParentMenuID</c>
	/// metadata pair. Returns null when no entry would be visible — the caller can then
	/// suppress the right-click affordance entirely.
	/// </summary>
	public static class ContextMenuProvider
	{
		public static ContextMenu? Build(IEnumerable<IContextMenuEntryExport> entries, TextViewContext context)
		{
			ArgumentNullException.ThrowIfNull(entries);
			ArgumentNullException.ThrowIfNull(context);

			// Group by ParentMenuID so sub-menu entries are easy to nest. Sort each group by
			// its declared Order; entries without an Order land at the end (double.MaxValue).
			var groups = entries
				.OrderBy(e => e.Metadata.Order)
				.GroupBy(e => e.Metadata.ParentMenuID)
				.ToDictionary(g => g.Key ?? string.Empty, g => g.ToArray());

			var menu = new ContextMenu();
			BuildLevel(groups.GetValueOrDefault(string.Empty, Array.Empty<IContextMenuEntryExport>()), menu.Items, groups, context);
			return menu.Items.Count > 0 ? menu : null;
		}

		static void BuildLevel(
			IContextMenuEntryExport[] entries,
			global::Avalonia.Controls.ItemCollection parent,
			Dictionary<string, IContextMenuEntryExport[]> groups,
			TextViewContext context)
		{
			foreach (var category in entries.GroupBy(e => e.Metadata.Category))
			{
				var needSeparator = parent.Count > 0;
				foreach (var export in category)
				{
					var entry = export.Value;
					if (!entry.IsVisible(context))
						continue;
					if (needSeparator)
					{
						parent.Add(new Separator());
						needSeparator = false;
					}
					var item = new MenuItem {
						Header = ResourceHelper.GetString(export.Metadata.Header) ?? export.Metadata.Header,
						InputGesture = TryParseGesture(export.Metadata.InputGestureText),
						HotKey = TryParseGesture(export.Metadata.InputGestureText),
					};
					// MenuItem.Icon takes any Control here (unlike NativeMenuItem.Icon which
					// requires Bitmap), so we pass the IImage directly via an Image control.
					if (Images.ResolveByPath(export.Metadata.Icon) is { } icon)
					{
						item.Icon = new Image {
							Width = 16,
							Height = 16,
							Source = icon,
						};
					}
					if (entry.IsEnabled(context))
					{
						// Capture entry + context for the click handler — entries are stateless
						// w.r.t. context, so the snapshot taken at Build-time is what runs.
						item.Click += (_, _) => entry.Execute(context);
					}
					else
					{
						item.IsEnabled = false;
					}
					parent.Add(item);

					// Nested sub-menu — look up by MenuID and recurse into the item's own Items.
					if (export.Metadata.MenuID is { Length: > 0 } menuId
						&& groups.TryGetValue(menuId, out var children))
					{
						BuildLevel(children, item.Items, groups, context);
					}
				}
			}
		}

		static global::Avalonia.Input.KeyGesture? TryParseGesture(string? text)
		{
			if (string.IsNullOrWhiteSpace(text))
				return null;
			try
			{
				return global::Avalonia.Input.KeyGesture.Parse(text);
			}
			catch
			{
				return null;
			}
		}
	}
}
