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

using System.IO;
using System.Linq;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpyX;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.AssemblyTree;

/// <summary>
/// A member ID whose declaring type reaches the opened assembly only as a type forwarder:
/// the ID names System.String, but a facade like System.Runtime.dll carries no type rows at
/// all - the member lives in the assembly the forwarder points at, and the resolver has to
/// follow it there. This is what <c>--navigateto</c> hits on any modern framework assembly.
/// </summary>
[TestFixture]
public class NavigateToForwardedMemberTests
{
	static string FacadePath => Path.Combine(
		Path.GetDirectoryName(typeof(object).Assembly.Location)!, "System.Runtime.dll");

	[Test]
	public async Task NavigateTo_Follows_A_Type_Forwarder_Into_The_Assembly_Holding_The_Member()
	{
		string facadePath = FacadePath;
		File.Exists(facadePath).Should().BeTrue(
			"the running framework must ship the System.Runtime facade this test navigates through");

		var assemblyList = new AssemblyList();
		var facade = assemblyList.OpenAssembly(facadePath);
		var facadeFile = await facade.GetMetadataFileOrNullAsync();
		facadeFile.Should().NotBeNull();

		// Guard the premise: if the facade defined System.String itself, the plain lookup
		// would answer and the forwarder path under test would never run.
		facadeFile!.Metadata.TypeDefinitions
			.Select(h => facadeFile.Metadata.GetString(facadeFile.Metadata.GetTypeDefinition(h).Name))
			.Should().NotContain("String", "System.Runtime is a facade of forwarders, not definitions");
		facadeFile.Metadata.ExportedTypes.Should().NotBeEmpty("the facade forwards its types");

		var entity = AssemblyTreeModel.FindEntityInRelevantAssemblies(
			"M:System.String.Concat(System.String,System.String)", new[] { facade });

		entity.Should().NotBeNull("the ID must resolve through the forwarder");
		entity!.Name.Should().Be("Concat");
		entity.DeclaringType!.FullName.Should().Be("System.String");
		entity.ParentModule!.MetadataFile!.FileName.Should().NotBe(facadePath,
			"the member rows live in the forwarder's target assembly, not in the facade");
	}

	[Test]
	public async Task NavigateTo_Returns_Null_For_A_Member_That_No_Forwarder_Leads_To()
	{
		var assemblyList = new AssemblyList();
		var facade = assemblyList.OpenAssembly(FacadePath);
		await facade.GetMetadataFileOrNullAsync();

		var entity = AssemblyTreeModel.FindEntityInRelevantAssemblies(
			"M:System.String.ThisMethodDoesNotExist", new[] { facade });

		entity.Should().BeNull("an unresolvable member must not resolve to some other member");
	}
}
