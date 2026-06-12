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

using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Languages;

/// <summary>
/// The Export Project/Solution dialog's preview computation (extracted as a pure static so it is
/// testable without the window): per-assembly project name, target subdirectory, and the
/// invalid / duplicate-name / PDB-eligible badges.
/// </summary>
[TestFixture]
public class ExportPreviewRowTests
{
	[AvaloniaTest]
	public async Task Solution_Mode_Uses_ShortName_Subdirectories_And_Flags_No_Duplicates()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var assemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Where(a => a.IsLoadedAsValidAssembly && a.ShortName != TreeNavigation.CoreLibName)
			.GroupBy(a => a.ShortName).Select(g => g.First())
			.Take(2).ToList();
		assemblies.Should().HaveCount(2);

		var rows = ExportProjectDialog.BuildPreviewRows(assemblies, solutionMode: true);

		rows.Should().HaveCount(2);
		rows.Should().OnlyContain(r => r.IsValidAssembly);
		rows.Should().OnlyContain(r => !r.HasDuplicateShortName);
		rows.Select(r => r.TargetSubdirectory).Should().BeEquivalentTo(assemblies.Select(a => a.ShortName));
		rows.Select(r => r.ProjectName).Should().BeEquivalentTo(assemblies.Select(a => a.ShortName + ".csproj"));
	}

	[AvaloniaTest]
	public async Task Project_Mode_Writes_Into_The_Chosen_Folder_Directly()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var assembly = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.First(a => a.IsLoadedAsValidAssembly && a.ShortName != TreeNavigation.CoreLibName);

		var rows = ExportProjectDialog.BuildPreviewRows(new[] { assembly }, solutionMode: false);

		rows.Single().TargetSubdirectory.Should().Be(".",
			"project mode writes the single project into the chosen folder, not a subdirectory");
	}

	[AvaloniaTest]
	public async Task Duplicate_Short_Names_Are_Flagged_On_Every_Affected_Row()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var assembly = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.First(a => a.IsLoadedAsValidAssembly && a.ShortName != TreeNavigation.CoreLibName);

		var rows = ExportProjectDialog.BuildPreviewRows(new[] { assembly, assembly }, solutionMode: true);

		rows.Should().HaveCount(2);
		rows.Should().OnlyContain(r => r.HasDuplicateShortName);
		rows.Should().OnlyContain(r => r.BadgeText.Contains("duplicate name"));
	}
}
