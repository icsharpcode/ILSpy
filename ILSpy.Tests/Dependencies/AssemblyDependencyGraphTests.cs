// Copyright (c) 2026 Piero Viano
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
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Dependencies;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Dependencies;

[TestFixture]
public class AssemblyDependencyGraphTests
{
	static async Task<LoadedAssembly> OpenAsync(AssemblyList list, string path)
	{
		var loaded = list.OpenAssembly(path);
		await loaded.GetLoadResultAsync();
		return loaded;
	}

	[Test]
	public async Task Graph_Contains_The_Root_Its_Library_And_The_Edge_Between_Them()
	{
		var (_, consumerPath, libraryName, consumerName) = DependencyFixtures.EmitPair("Graph");
		var list = new AssemblyList();
		var consumer = await OpenAsync(list, consumerPath);

		var graph = AssemblyDependencyGraph.Build([consumer]);

		var root = graph.Nodes[0];
		root.Name.Should().Be(consumerName);
		root.IsRoot.Should().BeTrue();
		root.IsResolved.Should().BeTrue();
		var library = graph.Nodes.Single(n => n.Name == libraryName);
		library.IsResolved.Should().BeTrue("the library sits next to the consumer");
		library.IsRoot.Should().BeFalse();
		graph.Edges.Should().Contain(e => e.From == root.Index && e.To == library.Index && !e.IsVersionMismatch);
		graph.Nodes.Should().Contain(n => n.Name == TreeNavigation.CoreLibName, "the emitted code references the core library");
	}

	[Test]
	public async Task Transitive_Build_Follows_The_Library_References_While_Direct_Build_Stops()
	{
		var (_, consumerPath, libraryName, _) = DependencyFixtures.EmitPair("Graph");
		var list = new AssemblyList();
		var consumer = await OpenAsync(list, consumerPath);

		var direct = AssemblyDependencyGraph.Build([consumer], transitive: false);
		var transitive = AssemblyDependencyGraph.Build([consumer], transitive: true);

		direct.Edges.Should().OnlyContain(e => e.From == 0, "only the root's references are followed");
		var library = transitive.Nodes.Single(n => n.Name == libraryName);
		transitive.Edges.Should().Contain(e => e.From == library.Index, "the library's own references are followed");
	}

	[Test]
	public async Task Unresolved_Reference_Becomes_An_Unresolved_Node()
	{
		var (consumerPath, missingName) = DependencyFixtures.EmitConsumerWithMissingReference("Graph");
		var list = new AssemblyList();
		var consumer = await OpenAsync(list, consumerPath);

		var graph = AssemblyDependencyGraph.Build([consumer]);

		var missing = graph.Nodes.Single(n => n.Name == missingName);
		missing.IsResolved.Should().BeFalse();
		missing.FileName.Should().BeNull();
		missing.FullName.Should().StartWith(missingName + ",");
		graph.Edges.Should().Contain(e => e.From == 0 && e.To == missing.Index);
	}

	[Test]
	public async Task Version_Mismatch_Is_Flagged_On_The_Edge()
	{
		var dir = DependencyFixtures.NewDirectory();
		var libraryName = DependencyFixtures.UniqueName("GraphLib");
		var libraryPath = DependencyFixtures.EmitLibrary(dir, libraryName, new Version(1, 0, 0, 0));
		var consumerPath = DependencyFixtures.EmitConsumer(dir, DependencyFixtures.UniqueName("GraphApp"), libraryPath);
		DependencyFixtures.EmitLibrary(dir, libraryName, new Version(2, 0, 0, 0));
		var list = new AssemblyList();
		var consumer = await OpenAsync(list, consumerPath);

		var graph = AssemblyDependencyGraph.Build([consumer]);

		var library = graph.Nodes.Single(n => n.Name == libraryName);
		library.Version.Should().Be(new Version(2, 0, 0, 0));
		var edge = graph.Edges.Single(e => e.To == library.Index);
		edge.IsVersionMismatch.Should().BeTrue();
		edge.ReferencedVersion.Should().Be(new Version(1, 0, 0, 0));
	}

	[Test]
	public async Task Build_Is_Deterministic_And_Cancellable()
	{
		var (_, consumerPath, _, _) = DependencyFixtures.EmitPair("Graph");
		var list = new AssemblyList();
		var consumer = await OpenAsync(list, consumerPath);

		var first = AssemblyDependencyGraph.Build([consumer]);
		var second = AssemblyDependencyGraph.Build([consumer]);
		second.Nodes.Select(n => n.FullName).Should().Equal(first.Nodes.Select(n => n.FullName));
		second.Edges.Select(e => (e.From, e.To)).Should().Equal(first.Edges.Select(e => (e.From, e.To)));

		using var cts = new CancellationTokenSource();
		cts.Cancel();
		var act = () => AssemblyDependencyGraph.Build([consumer], cancellationToken: cts.Token);
		act.Should().Throw<OperationCanceledException>();
	}
}
