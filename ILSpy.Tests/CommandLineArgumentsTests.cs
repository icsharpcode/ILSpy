using System;

using FluentAssertions;

using ICSharpCode.ILSpy.AppEnv;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests
{
	[TestFixture]
	public class CommandLineArgumentsTests
	{
		[Test]
		public void VerifyEmptyArgumentsArray()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { });

			cmdLineArgs.AssembliesToLoad.Should().BeEmpty();
			cmdLineArgs.SingleInstance.Should().BeNull();
			cmdLineArgs.NavigateTo.Should().BeNull();
			cmdLineArgs.Search.Should().BeNull();
			cmdLineArgs.Language.Should().BeNull();
			cmdLineArgs.NoActivate.Should().BeFalse();
			cmdLineArgs.ConfigFile.Should().BeNull();
		}

		[Test]
		public void VerifyHelpOption()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--help" });
			cmdLineArgs.ArgumentsParser.IsShowingInformation.Should().BeTrue();
		}

		[Test]
		public void VerifyForceNewInstanceOption()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--newinstance" });
			cmdLineArgs.SingleInstance.Should().BeFalse();
		}

		[Test]
		public void VerifyNavigateToOption()
		{
			const string navigateTo = "MyNamespace.MyClass";
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--navigateto", navigateTo });
			cmdLineArgs.NavigateTo.Should().BeEquivalentTo(navigateTo);
		}

		[Test]
		public void VerifyNavigateToOption_NoneTest_Matching_VSAddin()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--navigateto:none" });
			cmdLineArgs.NavigateTo.Should().BeEquivalentTo("none");
		}

		[Test]
		public void VerifyCaseSensitivityOfOptionsDoesntThrow()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--navigateTo:none" });

			cmdLineArgs.ArgumentsParser.RemainingArguments.Should().HaveCount(1);
		}

		[Test]
		public void VerifySearchOption()
		{
			const string searchWord = "TestContainers";
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--search", searchWord });
			cmdLineArgs.Search.Should().BeEquivalentTo(searchWord);
		}

		[Test]
		public void VerifyLanguageOption()
		{
			const string language = "csharp";
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--language", language });
			cmdLineArgs.Language.Should().BeEquivalentTo(language);
		}

		[Test]
		public void VerifyConfigOption()
		{
			const string configFile = "myilspyoptions.xml";
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--config", configFile });
			cmdLineArgs.ConfigFile.Should().BeEquivalentTo(configFile);
		}

		[Test]
		public void VerifyNoActivateOption()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--noactivate" });
			cmdLineArgs.NoActivate.Should().BeTrue();
		}

		[Test]
		public void MultipleAssembliesAsArguments()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "assembly1", "assembly2", "assembly3" });
			cmdLineArgs.AssembliesToLoad.Should().HaveCount(3);
		}

		[Test]
		public void PassAtFileArguments()
		{
			string filepath = System.IO.Path.GetTempFileName();

			System.IO.File.WriteAllText(filepath, "assembly1\r\nassembly2\r\nassembly3\r\n--newinstance\r\n--noactivate");

			var cmdLineArgs = CommandLineArguments.Create(new string[] { $"@{filepath}" });

			try
			{
				System.IO.File.Delete(filepath);
			}
			catch (Exception)
			{
			}

			cmdLineArgs.SingleInstance.Should().BeFalse();
			cmdLineArgs.NoActivate.Should().BeTrue();
			cmdLineArgs.AssembliesToLoad.Should().HaveCount(3);
		}
	}
}
