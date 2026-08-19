// Copyright (c) 2026 Dr. Masroor Ehsan

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

			const string response = "```json\n[{\"type\":\"SQL injection\",\"method\":\"Execute\",\"issue\":\"user input reaches a query\",\"severity\":\"high\",\"line\":12,\"confidence\":0.95},{\"issue\":\"  \"}]\n```";

			var findings = AISecurityAnalyzer.ParseFindings(response, type);

			findings.Should().HaveCount(1);
			findings[0].Target.Should().BeSameAs(type.Methods.Single(method => method.Name == nameof(SecuritySample.Execute)));
			findings[0].Type.Should().Be("SQL injection");
			findings[0].Issue.Should().Be("user input reaches a query");
			findings[0].Severity.Should().Be("High");
			findings[0].Line.Should().Be(12);
			findings[0].Confidence.Should().Be(0.95);
		}

		[Test]
		public void ParseFindings_UsesSafeDefaultsAndClampsNegativeLines()
		{
			using var module = new PEFile(typeof(SecuritySample).Assembly.Location);
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			ITypeDefinition type = decompiler.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(typeof(SecuritySample).FullName!));

			var findings = AISecurityAnalyzer.ParseFindings("[{\"issue\":\"hardcoded secret\",\"severity\":\"unknown\",\"line\":-4,\"confidence\":0.70}]", type);

			findings.Should().HaveCount(1);
			findings[0].Target.Should().BeSameAs(type);
			findings[0].Type.Should().Be("Security risk");
			findings[0].Severity.Should().Be("Medium");
			findings[0].Line.Should().Be(0);
		}

		[TestCase(0.69, false)]
		[TestCase(0.70, true)]
		[TestCase(1.0, true)]
		public void ParseFindings_FiltersByConfidence(double confidence, bool included)
		{
			using var module = new PEFile(typeof(SecuritySample).Assembly.Location);
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			ITypeDefinition type = decompiler.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(typeof(SecuritySample).FullName!));

			var findings = AISecurityAnalyzer.ParseFindings($"[{{\"issue\":\"issue\",\"confidence\":{confidence.ToString(System.Globalization.CultureInfo.InvariantCulture)}}}]", type);

			findings.Should().HaveCount(included ? 1 : 0);
		}

		[Test]
		public void ParseFindings_RejectsMissingAndOutOfRangeConfidence()
		{
			using var module = new PEFile(typeof(SecuritySample).Assembly.Location);
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			ITypeDefinition type = decompiler.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(typeof(SecuritySample).FullName!));

			var findings = AISecurityAnalyzer.ParseFindings("[{\"issue\":\"missing\"},{\"issue\":\"negative\",\"confidence\":-0.1},{\"issue\":\"high\",\"confidence\":1.1}]", type);

			findings.Should().BeEmpty();
		}

		[Test]
		public void BulkAuditPlanIsBoundedBeforeRequests()
		{
			using var module = new PEFile(typeof(SecuritySample).Assembly.Location);
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			ITypeDefinition type = decompiler.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(typeof(SecuritySample).FullName!));
			var service = new AISecurityAuditService();

			var plan = service.CreatePlan(new[] { type, type }, maximumTypes: 1);

			plan.TotalEligible.Should().Be(2);
			plan.IsOverLimit.Should().BeTrue();
			plan.Types.Should().HaveCount(2);
		}
	}

	sealed class SecuritySample
	{
		public void Execute()
		{
		}
	}
}
