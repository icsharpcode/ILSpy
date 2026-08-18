// Copyright (c) 2026 Masroor

using System.Linq;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.Analyzers.Builtin;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.Analyzers
{
	[TestFixture]
	public class AISecurityAnalyzerTests
	{
		[Test]
		public void ParseFindings_HandlesFencedJsonAndResolvesMethodTargets()
		{
			using var module = new PEFile(typeof(SecuritySample).Assembly.Location);
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			ITypeDefinition type = decompiler.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(typeof(SecuritySample).FullName!));

			const string response = "```json\n[{\"type\":\"SQL injection\",\"method\":\"Execute\",\"issue\":\"user input reaches a query\",\"severity\":\"high\",\"line\":12},{\"issue\":\"  \"}]\n```";

			var findings = AISecurityAnalyzer.ParseFindings(response, type);

			findings.Should().HaveCount(1);
			findings[0].Target.Should().BeSameAs(type.Methods.Single(method => method.Name == nameof(SecuritySample.Execute)));
			findings[0].Type.Should().Be("SQL injection");
			findings[0].Issue.Should().Be("user input reaches a query");
			findings[0].Severity.Should().Be("High");
			findings[0].Line.Should().Be(12);
		}

		[Test]
		public void ParseFindings_UsesSafeDefaultsAndClampsNegativeLines()
		{
			using var module = new PEFile(typeof(SecuritySample).Assembly.Location);
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			ITypeDefinition type = decompiler.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(typeof(SecuritySample).FullName!));

			var findings = AISecurityAnalyzer.ParseFindings("[{\"issue\":\"hardcoded secret\",\"severity\":\"unknown\",\"line\":-4}]", type);

			findings.Should().HaveCount(1);
			findings[0].Target.Should().BeSameAs(type);
			findings[0].Type.Should().Be("Security risk");
			findings[0].Severity.Should().Be("Medium");
			findings[0].Line.Should().Be(0);
		}
	}

	sealed class SecuritySample
	{
		public void Execute()
		{
		}
	}
}
