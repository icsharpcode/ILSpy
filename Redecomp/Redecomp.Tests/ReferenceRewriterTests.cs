// Copyright (c) 2026 Franck
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

using System.Xml.Linq;

using Redecomp.Bench;
using Redecomp.Core.Reconstruction;

namespace Redecomp.Tests;

public static class ReferenceRewriterTests
{
	[Test]
	public static void TurnsSharedFrameworkReferencesIntoFrameworkReferences()
	{
		string dotnetRoot = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location)!, "..", "..", ".."));
		string aspNet = Path.Combine(dotnetRoot, "shared", "Microsoft.AspNetCore.App", "10.0.0", "Microsoft.AspNetCore.Http.Abstractions.dll");
		string core = Path.Combine(dotnetRoot, "shared", "Microsoft.NETCore.App", "10.0.0", "System.Linq.dll");
		string projectFile = Path.Combine(ProjectBuilder.NewTempDirectory(), "Web.csproj");
		File.WriteAllText(projectFile, $"""
			<Project Sdk="Microsoft.NET.Sdk">
			  <ItemGroup>
			    <Reference Include="Microsoft.AspNetCore.Http.Abstractions"><HintPath>{aspNet}</HintPath></Reference>
			    <Reference Include="System.Linq"><HintPath>{core}</HintPath></Reference>
			  </ItemGroup>
			</Project>
			""");

		ReferenceRewriter.Apply(projectFile, null);

		var doc = XDocument.Load(projectFile);
		Assert.AreEqual(0, doc.Descendants("Reference").Count(), "assembly references left");
		var frameworks = doc.Descendants("FrameworkReference").Select(f => f.Attribute("Include")!.Value).ToList();
		Assert.AreEqual("Microsoft.AspNetCore.App", string.Join(",", frameworks), "framework references");
	}
}
