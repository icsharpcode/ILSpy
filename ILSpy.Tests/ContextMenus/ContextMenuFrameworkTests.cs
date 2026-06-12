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
using System.Composition;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class ContextMenuFrameworkTests
{
	[AvaloniaTest]
	public void Build_Includes_Visible_Entries_With_Header_And_Skips_Hidden_Ones()
	{
		// ContextMenuProvider.Build is the pure-function core: it takes (entries, context) and
		// returns a ContextMenu populated with one MenuItem per entry whose IsVisible(context)
		// returns true. Entries whose IsVisible returns false are skipped entirely.

		// Arrange — three entries, one hidden.
		var entries = new IContextMenuEntryExport[] {
			Stub("Visible one", category: null, order: 0, visible: true, enabled: true),
			Stub("Hidden", category: null, order: 1, visible: false, enabled: true),
			Stub("Visible two", category: null, order: 2, visible: true, enabled: true),
		};
		var context = new TextViewContext();

		// Act — build the menu.
		var menu = ContextMenuProvider.Build(entries, context);

		// Assert — only the two visible entries land, in declared order.
		menu.Should().NotBeNull();
		var items = menu!.Items.OfType<MenuItem>().ToList();
		items.Should().HaveCount(2);
		items[0].Header.Should().Be("Visible one");
		items[1].Header.Should().Be("Visible two");
	}

	[AvaloniaTest]
	public void Build_Marks_Entry_Disabled_When_Is_Enabled_Returns_False()
	{
		// Arrange — entry that's visible but disabled.
		var entries = new IContextMenuEntryExport[] {
			Stub("Disabled entry", category: null, order: 0, visible: true, enabled: false),
		};
		var context = new TextViewContext();

		// Act
		var menu = ContextMenuProvider.Build(entries, context)!;

		// Assert — item is present but its IsEnabled flag is false; clicking it does nothing.
		var item = menu.Items.OfType<MenuItem>().Single();
		item.IsEnabled.Should().BeFalse();
	}

	[AvaloniaTest]
	public void Build_Returns_Null_When_No_Visible_Entries_So_The_Menu_Is_Suppressed()
	{
		// An empty menu shouldn't be shown at all — the call site decides whether to suppress
		// the right-click affordance based on a null return.

		// Arrange — single entry, hidden.
		var entries = new IContextMenuEntryExport[] {
			Stub("Hidden", category: null, order: 0, visible: false, enabled: true),
		};

		// Act + Assert
		ContextMenuProvider.Build(entries, new TextViewContext()).Should().BeNull();
	}

	[AvaloniaTest]
	public void Build_Inserts_Separators_Between_Categories()
	{
		// Arrange — two entries in category A, one in category B.
		var entries = new IContextMenuEntryExport[] {
			Stub("A1", category: "A", order: 0, visible: true, enabled: true),
			Stub("A2", category: "A", order: 1, visible: true, enabled: true),
			Stub("B1", category: "B", order: 2, visible: true, enabled: true),
		};

		// Act
		var menu = ContextMenuProvider.Build(entries, new TextViewContext())!;

		// Assert — order is A1, A2, separator, B1.
		var children = menu.Items.ToList();
		children.Should().HaveCount(4);
		children[0].Should().BeOfType<MenuItem>().Which.Header.Should().Be("A1");
		children[1].Should().BeOfType<MenuItem>().Which.Header.Should().Be("A2");
		children[2].Should().BeOfType<Separator>();
		children[3].Should().BeOfType<MenuItem>().Which.Header.Should().Be("B1");
	}

	[AvaloniaTest]
	public void Click_On_Menu_Item_Invokes_The_Entry_Execute_With_The_Context()
	{
		// Arrange — record the context the entry receives at Execute time.
		TextViewContext? received = null;
		var entry = new RecordingEntry(c => received = c);
		var export = new StubExport(
			entry,
			new ContextMenuEntryMetadata { Header = "Run me", Order = 0 });
		var context = new TextViewContext();

		// Act — build the menu, click the item.
		var menu = ContextMenuProvider.Build(new[] { export }, context)!;
		var item = menu.Items.OfType<MenuItem>().Single();
		item.RaiseEvent(new global::Avalonia.Interactivity.RoutedEventArgs(MenuItem.ClickEvent));

		// Assert — Execute fired with the supplied context.
		ReferenceEquals(received, context).Should().BeTrue();
	}

	static IContextMenuEntryExport Stub(string header, string? category, double order, bool visible, bool enabled)
		=> new StubExport(
			new StubEntry(visible, enabled),
			new ContextMenuEntryMetadata { Header = header, Category = category, Order = order });

	sealed class StubEntry(bool visible, bool enabled) : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => visible;
		public bool IsEnabled(TextViewContext context) => enabled;
		public void Execute(TextViewContext context) { }
	}

	sealed class RecordingEntry(System.Action<TextViewContext> onExecute) : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => true;
		public bool IsEnabled(TextViewContext context) => true;
		public void Execute(TextViewContext context) => onExecute(context);
	}

	sealed class StubExport(IContextMenuEntry entry, ContextMenuEntryMetadata metadata) : IContextMenuEntryExport
	{
		public IContextMenuEntry Value { get; } = entry;
		public ContextMenuEntryMetadata Metadata { get; } = metadata;
	}
}
