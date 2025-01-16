using System;

using Shouldly;

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

			cmdLineArgs.AssembliesToLoad.ShouldBeEmpty();
			cmdLineArgs.SingleInstance.ShouldBeNull();
			cmdLineArgs.NavigateTo.ShouldBeNull();
			cmdLineArgs.Search.ShouldBeNull();
			cmdLineArgs.Language.ShouldBeNull();
			cmdLineArgs.NoActivate.ShouldBeFalse();
			cmdLineArgs.ConfigFile.ShouldBeNull();
		}

		[Test]
		public void VerifyHelpOption()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--help" });
			cmdLineArgs.ArgumentsParser.IsShowingInformation.ShouldBeTrue();
		}

		[Test]
		public void VerifyForceNewInstanceOption()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--newinstance" });
			cmdLineArgs.SingleInstance.ShouldNotBeNull();
			cmdLineArgs.SingleInstance.Value.ShouldBeFalse();
		}

		[Test]
		public void VerifyNavigateToOption()
		{
			const string navigateTo = "MyNamespace.MyClass";
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--navigateto", navigateTo });
			cmdLineArgs.NavigateTo.ShouldBe(navigateTo);
		}

		[Test]
		public void VerifyNavigateToOption_NoneTest_Matching_VSAddin()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--navigateto:none" });
			cmdLineArgs.NavigateTo.ShouldBe("none");
		}

		[Test]
		public void VerifyCaseSensitivityOfOptionsDoesntThrow()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--navigateTo:none" });

			cmdLineArgs.ArgumentsParser.RemainingArguments.Count.ShouldBe(1);
		}

		[Test]
		public void VerifySearchOption()
		{
			const string searchWord = "TestContainers";
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--search", searchWord });
			cmdLineArgs.Search.ShouldBe(searchWord);
		}

		[Test]
		public void VerifyLanguageOption()
		{
			const string language = "csharp";
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--language", language });
			cmdLineArgs.Language.ShouldBe(language);
		}

		[Test]
		public void VerifyConfigOption()
		{
			const string configFile = "myilspyoptions.xml";
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--config", configFile });
			cmdLineArgs.ConfigFile.ShouldBe(configFile);
		}

		[Test]
		public void VerifyNoActivateOption()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "--noactivate" });
			cmdLineArgs.NoActivate.ShouldBeTrue();
		}

		[Test]
		public void MultipleAssembliesAsArguments()
		{
			var cmdLineArgs = CommandLineArguments.Create(new string[] { "assembly1", "assembly2", "assembly3" });
			cmdLineArgs.AssembliesToLoad.Count.ShouldBe(3);
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

			cmdLineArgs.SingleInstance.ShouldNotBeNull();
			cmdLineArgs.SingleInstance.Value.ShouldBeFalse();
			cmdLineArgs.NoActivate.ShouldBeTrue();
			cmdLineArgs.AssembliesToLoad.Count.ShouldBe(3);
		}
	}
}
