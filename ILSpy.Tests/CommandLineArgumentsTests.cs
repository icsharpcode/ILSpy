using FluentAssertions;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests
{
	[TestFixture]
	public class CommandLineArgumentsTests
	{
		[Test]
		public void VerifyEmptyArgumentsArray()
		{
			var cmdLineArgs = new CommandLineArguments(new string[] { });

			cmdLineArgs.AssembliesToLoad.Should().BeEmpty();
			cmdLineArgs.SingleInstance.Should().BeNull();
			cmdLineArgs.NavigateTo.Should().BeNull();
			cmdLineArgs.Search.Should().BeNull();
			cmdLineArgs.Language.Should().BeNull();
			cmdLineArgs.NoActivate.Should().BeFalse();
			cmdLineArgs.ConfigFile.Should().BeNull();
		}

		[Test]
		public void VerifySeparateOption()
		{
			var cmdLineArgs = new CommandLineArguments(new string[] { "/separate" });
			cmdLineArgs.SingleInstance.Should().BeFalse();
		}

		[Test]
		public void VerifySingleInstanceOption()
		{
			var cmdLineArgs = new CommandLineArguments(new string[] { "/singleInstance" });
			cmdLineArgs.SingleInstance.Should().BeTrue();
		}

		[Test]
		public void VerifySeparateSingleInstanceOptionOrdering()
		{
			var cmdLineArgsCase1 = new CommandLineArguments(new string[] { "/singleInstance", "/separate" });
			cmdLineArgsCase1.SingleInstance.Should().BeFalse();

			var cmdLineArgsCase2 = new CommandLineArguments(new string[] { "/separate", "/singleInstance" });
			cmdLineArgsCase2.SingleInstance.Should().BeTrue();
		}
	}
}
