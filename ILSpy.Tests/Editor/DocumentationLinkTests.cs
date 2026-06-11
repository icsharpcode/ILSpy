// Copyright (c) 2026 Siegfried Pammer
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
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.TypeSystem;

using ILSpy.AssemblyTree;
using ILSpy.TextView;
using ILSpy.TreeNodes;
using ILSpy.Util;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// Pins the hyperlink behaviour of documentation tooltips: a resolvable
/// <c>&lt;see cref="..."/&gt;</c> must render as a clickable link that requests
/// navigation to the referenced entity (via <see cref="MessageBus{T}"/> with
/// <see cref="NavigateToReferenceEventArgs"/>, the same channel the analyzer uses) and
/// must notify the hosting popup through <see cref="DocumentationRenderer.LinkClicked"/>
/// so it can close.
/// </summary>
[TestFixture]
public class DocumentationLinkTests
{
	[AvaloniaTest]
	public async Task SeeCref_Renders_As_A_Link_That_Navigates_On_Click()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var stringType = (IEntity)vm.AssemblyTreeModel
			.FindNode<TypeTreeNode>(coreLibName, "System", "System.String").Member!;

		var renderer = new DocumentationRenderer(
			new CSharpAmbience(),
			new FontFamily("Consolas, Menlo, Monospace"),
			12);
		renderer.AddXmlDocumentation(
			"""<summary>Implemented by <see cref="T:System.String"/> instances.</summary>""",
			declaringEntity: null,
			resolver: id => id == "T:System.String" ? stringType : null);

		var window = new Window { Content = renderer.CreateView() };
		window.Show();
		AvaloniaHeadlessPlatform.ForceRenderTimerTick();
		Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		window.UpdateLayout();

		var link = window.GetVisualDescendants().OfType<TextBlock>()
			.FirstOrDefault(tb => tb.Classes.Contains("doc-link"));
		link.Should().NotBeNull("a resolvable cref must render as a clickable link");
		link!.Cursor.Should().NotBeNull("links show the hand cursor as a click affordance");

		object? navigated = null;
		var linkClickedRaised = false;
		renderer.LinkClicked += (_, _) => linkClickedRaised = true;
		EventHandler<NavigateToReferenceEventArgs> capture = (_, e) => navigated = e.Reference;
		MessageBus<NavigateToReferenceEventArgs>.Subscribers += capture;
		try
		{
			var centre = link.TranslatePoint(
				new Point(link.Bounds.Width / 2, link.Bounds.Height / 2), window)!.Value;
			window.MouseDown(centre, MouseButton.Left);
			window.MouseUp(centre, MouseButton.Left);
		}
		finally
		{
			MessageBus<NavigateToReferenceEventArgs>.Subscribers -= capture;
		}

		navigated.Should().BeSameAs(stringType,
			"clicking the link must request navigation to the referenced entity");
		linkClickedRaised.Should().BeTrue(
			"the hosting popup closes itself when a link is followed");
	}

	[AvaloniaTest]
	public void Unresolvable_Cref_Falls_Back_To_Plain_Text()
	{
		var renderer = new DocumentationRenderer(
			new CSharpAmbience(),
			new FontFamily("Consolas, Menlo, Monospace"),
			12);
		renderer.AddXmlDocumentation(
			"""<summary>See <see cref="T:Does.Not.Exist"/>.</summary>""",
			declaringEntity: null,
			resolver: _ => null);

		var window = new Window { Content = renderer.CreateView() };
		window.Show();
		window.UpdateLayout();

		window.GetVisualDescendants().OfType<TextBlock>()
			.Should().NotContain(tb => tb.Classes.Contains("doc-link"),
				"an unresolvable cref renders as plain text, not a dead link");
	}

	[AvaloniaTest]
	public async Task FindEntityInRelevantAssemblies_Resolves_A_Type_IdString()
	{
		var (_, vm) = await TestHarness.BootAsync();
		var list = vm.AssemblyTreeModel.AssemblyList!;

		var entity = AssemblyTreeModel.FindEntityInRelevantAssemblies(
			"T:System.String", list.GetAssemblies());

		entity.Should().NotBeNull("the doc-comment cref resolver feeds on id strings");
		entity!.FullName.Should().Be("System.String");
	}
}
