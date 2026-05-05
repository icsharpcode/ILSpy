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
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

using AvaloniaEdit.Rendering;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Properties;

using ILSpy.Docking;
using ILSpy.TextView;

namespace ILSpy.Commands
{
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Help), Header = nameof(Resources._About), MenuOrder = 99999)]
	[Shared]
	sealed class AboutCommand : SimpleCommand
	{
		// Phrases inside the About blurb that should render as clickable hyperlinks. The Uri
		// uses the custom "resource:" scheme; the click handler reads the matching name as an
		// embedded manifest resource on this assembly.
		static readonly (string Phrase, Uri Uri)[] Links = {
			("MIT License", new Uri("resource:license.txt")),
			("third-party notices", new Uri("resource:third-party-notices.txt")),
		};

		readonly DockWorkspace dockWorkspace;
		readonly IEnumerable<IAboutPageAddition> additions;

		[ImportingConstructor]
		public AboutCommand(DockWorkspace dockWorkspace, [ImportMany] IEnumerable<IAboutPageAddition> additions)
		{
			this.dockWorkspace = dockWorkspace;
			this.additions = additions;
		}

		public override void Execute(object? parameter)
		{
			var output = new AvaloniaEditTextOutput { Title = Resources.About };
			output.WriteLine(Resources.ILSpyVersion + DecompilerVersionInfo.FullVersionWithCommitHash);
			output.WriteLine(Resources.NETFrameworkVersion + GetDotnetProductVersion());
			output.WriteLine();

			foreach (var addition in additions)
				addition.Write(output);
			output.WriteLine();

			var assembly = typeof(AboutCommand).Assembly;
			using (var stream = assembly.GetManifestResourceStream("ILSpyAboutPage.txt"))
			{
				if (stream != null)
				{
					using var reader = new StreamReader(stream);
					while (reader.ReadLine() is { } line)
						output.WriteLine(line);
				}
			}

			foreach (var (phrase, uri) in Links)
				output.AddVisualLineElementGenerator(new ResourceLinkGenerator(phrase, uri));

			var tab = OpenInNewTab(Resources.About, output, ".txt");
			tab.IsStaticContent = true;
			dockWorkspace.RecordStaticPage(tab, new Uri("resource:aboutpage"));
		}

		DecompilerTabPageModel OpenInNewTab(string title, AvaloniaEditTextOutput output, string syntaxExtension)
		{
			var tab = new DecompilerTabPageModel {
				Title = title,
				SyntaxExtension = syntaxExtension,
				Text = output.GetText(),
				HighlightingModel = output.HighlightingModel,
				References = output.References,
				DefinitionLookup = output.DefinitionLookup,
				UIElements = output.UIElements,
				Foldings = output.Foldings,
				CustomElementGenerators = output.ElementGenerators.Count > 0
					? new List<VisualLineElementGenerator>(output.ElementGenerators)
					: null,
			};
			tab.OpenUriRequested += OnOpenUri;
			dockWorkspace.OpenNewTab(tab);
			return tab;
		}

		bool OnOpenUri(Uri uri)
		{
			if (!string.Equals(uri.Scheme, "resource", StringComparison.OrdinalIgnoreCase))
				return false;
			OpenEmbeddedResource(uri.AbsolutePath);
			return true;
		}

		void OpenEmbeddedResource(string resourceName)
		{
			var assembly = typeof(AboutCommand).Assembly;
			using var stream = assembly.GetManifestResourceStream(resourceName);
			if (stream == null)
				return;
			using var reader = new StreamReader(stream);
			var output = new AvaloniaEditTextOutput { Title = resourceName };
			output.Write(reader.ReadToEnd());
			var tab = OpenInNewTab(resourceName, output, ".txt");
			tab.IsStaticContent = true;
			dockWorkspace.RecordStaticPage(tab, new Uri("resource:" + resourceName));
		}

		static string GetDotnetProductVersion()
		{
			// AOT scenarios leave Assembly.Location empty — fall back to the runtime version.
			var location = typeof(Uri).Assembly.Location;
			if (!string.IsNullOrWhiteSpace(location))
				return FileVersionInfo.GetVersionInfo(location).ProductVersion ?? "UNKNOWN";
			return typeof(object).Assembly.GetName().Version?.ToString() ?? "UNKNOWN";
		}

	}

	/// <summary>
	/// Plug-in extension point — a MEF-exported implementation can append additional content
	/// (logos, version snippets, third-party credits) to the bottom of the About page.
	/// </summary>
	public interface IAboutPageAddition
	{
		void Write(ISmartTextOutput output);
	}
}
