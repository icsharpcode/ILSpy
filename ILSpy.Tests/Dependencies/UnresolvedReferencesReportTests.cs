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

using System.Linq;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Dependencies;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Dependencies;

[TestFixture]
public class UnresolvedReferencesReportTests
{
	[Test]
	public async Task Missing_Reference_Is_Reported_Under_Its_Referencing_Assembly_With_The_Probe_Log()
	{
		var (consumerPath, missingName) = DependencyFixtures.EmitConsumerWithMissingReference("Report");
		var (libraryPath, resolvableConsumerPath, libraryName, _) = DependencyFixtures.EmitPair("Report");
		var list = new AssemblyList();
		var orphan = list.OpenAssembly(consumerPath);
		var healthy = list.OpenAssembly(resolvableConsumerPath);
		await orphan.GetLoadResultAsync();
		await healthy.GetLoadResultAsync();

		var groups = UnresolvedReferencesReport.Collect([orphan, healthy]);

		var group = groups.Single(g => g.Assembly == orphan);
		var missing = group.References.Single(r => r.Reference.Name == missingName);
		missing.Messages.Should().Contain(m => m.Kind == MessageKind.Error && m.Message.Contains(missingName),
			"the resolver's probe log explains why the reference failed");
		groups.SelectMany(g => g.References).Should().NotContain(r => r.Reference.Name == libraryName,
			"a reference that resolves is not reported");
	}

	[Test]
	public async Task Fully_Resolved_Assemblies_Produce_No_Group()
	{
		var (libraryPath, _, _, _) = DependencyFixtures.EmitPair("Report");
		var list = new AssemblyList();
		var library = list.OpenAssembly(libraryPath);
		await library.GetLoadResultAsync();

		// The fixture library only references the core library, which always resolves.
		UnresolvedReferencesReport.Collect([library]).Should().BeEmpty();
	}
}
