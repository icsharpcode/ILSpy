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

using System;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.Options;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Plugins;

/// <summary>
/// The ported sample plugin (Test.Plugin.dll) exposes its extension points through the same MEF
/// contracts the app composes at startup. This loads the built plugin assembly (not referenced into
/// this test's output, to avoid polluting the headless app's *.Plugin.dll scan) and asserts its
/// exports resolve against the Avalonia app's extension-point types.
/// </summary>
[TestFixture]
public class TestPluginCompositionTests
{
	static Assembly LoadPlugin()
	{
		// AppContext.BaseDirectory is .../ILSpy.Tests/bin/<Config>/net11.0/. The plugin targets net10.0
		// (the host loads it as a library, so its TFM need not track the test project's), and builds to
		// the sibling TestPlugin/bin/<Config>/net10.0/ (the ProjectReference guarantees it's built first).
		var baseDir = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar);
		var config = Path.GetFileName(Path.GetDirectoryName(baseDir)!);
		var repo = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
		var dll = Path.Combine(repo, "TestPlugin", "bin", config, "net10.0", "Test.Plugin.dll");

		File.Exists(dll).Should().BeTrue($"the sample plugin must be built at {dll}");
		return Assembly.LoadFrom(dll);
	}

	[Test]
	public void Plugin_Contributes_A_Custom_Language()
	{
		using var container = new ContainerConfiguration().WithAssembly(LoadPlugin()).CreateContainer();

		container.GetExports<Language>().Should().Contain(l => l.Name == "Custom",
			"the plugin exports a Language named 'Custom'");
	}

	[Test]
	public void Plugin_Contributes_An_Options_Page()
	{
		using var container = new ContainerConfiguration().WithAssembly(LoadPlugin()).CreateContainer();

		// [ExportOptionPage] exports under the named "OptionPages" contract, not the default one.
		container.GetExports<IOptionPage>("OptionPages").Should().Contain(p => p.Title == "TestPlugin",
			"the plugin exports an [ExportOptionPage] titled 'TestPlugin'");
	}

	[Test]
	public void Plugin_Contributes_An_About_Page_Addition()
	{
		using var container = new ContainerConfiguration().WithAssembly(LoadPlugin()).CreateContainer();

		container.GetExports<IAboutPageAddition>().Should().NotBeEmpty(
			"the plugin exports an IAboutPageAddition");
	}

	[Test]
	public void Commands_With_A_DI_Constructor_Mark_It_Importing()
	{
		// System.Composition can only instantiate a parameterized command constructor when it is
		// marked [ImportingConstructor]. A command export whose ctor takes services but lacks the
		// attribute throws when the menu/toolbar builder materialises it -- which previously took the
		// whole menu bar down. Guard every exported command in the plugin.
		var commands = LoadPlugin().GetTypes()
			.Where(t => typeof(System.Windows.Input.ICommand).IsAssignableFrom(t) && !t.IsAbstract);

		foreach (var type in commands)
		{
			var ctors = type.GetConstructors();
			if (ctors.Any(c => c.GetParameters().Length == 0))
				continue; // a parameterless ctor is always fine
			ctors.Should().Contain(
				c => c.GetCustomAttribute<System.Composition.ImportingConstructorAttribute>() != null,
				$"{type.Name} has only parameterized constructors, so one must be [ImportingConstructor]");
		}
	}
}
