// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Reflection;

using AwesomeAssertions;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests;

[TestFixture]
public sealed class AssemblySmokeTests
{
	[Test]
	public void ProductionAssemblyHasExpectedIdentity()
	{
		var assembly = Assembly.Load("ICSharpCode.ILSpyX");
		assembly.GetName().Name.Should().Be("ICSharpCode.ILSpyX");
	}
}
