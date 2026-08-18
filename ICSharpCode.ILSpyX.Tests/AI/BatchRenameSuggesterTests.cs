// Copyright (c) 2026 Masroor

using System.Linq;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class BatchRenameSuggesterTests
	{
		[Test]
		public void OrderMembers_VisitsLocalDependenciesBeforeCallers()
		{
			using var module = new PEFile(typeof(BatchRenameSample).Assembly.Location);
			var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
			ITypeDefinition type = decompiler.TypeSystem.MainModule.GetTypeDefinition(new FullTypeName(typeof(BatchRenameSample).FullName!));

			var methods = BatchRenameSuggester.OrderMembers(type)
				.OfType<IMethod>()
				.Where(method => method.Name is nameof(BatchRenameSample.Entry) or nameof(BatchRenameSample.Middle) or nameof(BatchRenameSample.Leaf))
				.Select(method => method.Name)
				.ToArray();

			methods.Should().ContainInOrder(nameof(BatchRenameSample.Leaf), nameof(BatchRenameSample.Middle), nameof(BatchRenameSample.Entry));
		}
	}

	sealed class BatchRenameSample
	{
		public void Entry() => Middle();

		public void Middle() => Leaf();

		public void Leaf()
		{
		}
	}
}
