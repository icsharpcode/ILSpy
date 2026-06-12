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

using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Windows.Input;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// MEF aggregator for [ExportMainMenuCommand]-tagged commands. We need this indirection
	/// because System.Composition only resolves [ImportMany] on constructor parameters of
	/// MEF-managed parts, but the menu UserControl is instantiated by AXAML's parameterless
	/// loader and can only retrieve parts via <c>GetExport&lt;T&gt;()</c>.
	/// </summary>
	[Export]
	[Shared]
	public sealed class MainMenuCommandRegistry
	{
		[ImportingConstructor]
		public MainMenuCommandRegistry(
			[ImportMany("MainMenuCommand")] IEnumerable<ExportFactory<ICommand, MainMenuCommandMetadata>> commands)
		{
			Commands = commands.ToArray();
		}

		public IReadOnlyList<ExportFactory<ICommand, MainMenuCommandMetadata>> Commands { get; }
	}
}
