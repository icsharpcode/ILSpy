// Copyright (c) 2026 Masroor

using System;
using System.IO;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.Annotations;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.Annotations
{
	[TestFixture]
	public class RenameAnnotationManagerTests
	{
		[Test]
		public void LoadJson_RejectsAssemblyHashMismatch()
		{
			string path = CreateTempAssemblyFile();
			try
			{
				var manager = new RenameAnnotationManager(path);
				manager.LoadJson("{\"assemblyHash\":\"deadbeef\",\"renames\":[{\"token\":\"0x06000042\",\"newName\":\"ProcessPayment\"}]}");

				manager.GetRename("0x06000042").Should().BeNull();
			}
			finally
			{
				File.Delete(path);
				File.Delete(path + ".ilspy-annotations.json");
			}
		}

		[Test]
		public void SaveAndLoad_RoundTripsAnnotations()
		{
			string path = CreateTempAssemblyFile();
			try
			{
				var manager = new RenameAnnotationManager(path);
				manager.SetRename("0x06000042", "ProcessPayment");
				manager.SetRename("0x04000010", "paymentService");
				manager.Save();

				var reloaded = new RenameAnnotationManager(path);
				reloaded.Load();

				reloaded.GetRename("0x06000042").Should().Be("ProcessPayment");
				reloaded.GetRename("0x04000010").Should().Be("paymentService");
				reloaded.ToJson().Should().Contain(manager.AssemblyHash);
			}
			finally
			{
				File.Delete(path);
				File.Delete(path + ".ilspy-annotations.json");
			}
		}

		[Test]
		public void LoadJson_NormalizesTokensAndSkipsInvalidEntries()
		{
			string path = CreateTempAssemblyFile();
			try
			{
				var manager = new RenameAnnotationManager(path);
				string json = $"{{\"assemblyHash\":\"{manager.AssemblyHash}\",\"renames\":[{{\"token\":\" 0x42 \",\"newName\":\"PaymentService\"}},{{\"token\":\"not-a-token\",\"newName\":\"Ignored\"}},{{\"token\":\"0x43\",\"newName\":\"not valid\"}}]}}";

				manager.LoadJson(json);

				manager.GetRename("0x00000042").Should().Be("PaymentService");
				manager.GetRename("0x43").Should().BeNull();
				manager.Annotations.Should().HaveCount(1);
			}
			finally
			{
				File.Delete(path);
				File.Delete(path + ".ilspy-annotations.json");
			}
		}

		[Test]
		public void SetRename_RejectsKeywords()
		{
			string path = CreateTempAssemblyFile();
			try
			{
				var manager = new RenameAnnotationManager(path);
				Action action = () => manager.SetRename("0x06000042", "class");

				action.Should().Throw<ArgumentException>();
			}
			finally
			{
				File.Delete(path);
			}
		}

		static string CreateTempAssemblyFile()
		{
			string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".dll");
			File.WriteAllText(path, string.Empty);
			return path;
		}
	}
}
