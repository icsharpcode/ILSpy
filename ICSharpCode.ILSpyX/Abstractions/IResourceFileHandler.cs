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
using System.IO;
using System.Threading;

using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpyX.Abstractions
{
	/// <summary>
	/// Plug-in contract for converting a resource entry from its raw byte form into a
	/// language-specific source file during project export. Implementations are MEF-discovered
	/// via <c>[Export(typeof(IResourceFileHandler))]</c> and consulted by the project-export
	/// path; the first one to <see cref="CanHandle"/> the resource wins. The handler returns
	/// the on-disk file name it emitted so the project file picks the right MSBuild item group
	/// + Generator / SubType properties.
	/// </summary>
	public interface IResourceFileHandler
	{
		/// <summary>
		/// MSBuild item type for the produced project entry — <c>"Page"</c> for BAML→XAML,
		/// <c>"EmbeddedResource"</c> for the default raw fallback, etc.
		/// </summary>
		string EntryType { get; }

		/// <summary>Returns true when this handler can process the named resource.</summary>
		bool CanHandle(string name, ResourceFileHandlerContext context);

		/// <summary>
		/// Writes the decoded resource into the project directory (taken from
		/// <see cref="ResourceFileHandlerContext.SaveAsProjectDirectory"/>) and returns the
		/// resulting file name (relative to that directory). May add partial-type info to
		/// <paramref name="context"/> for downstream WPF page x:Class binding.
		/// </summary>
		string WriteResourceToFile(LoadedAssembly assembly, string fileName, Stream stream, ResourceFileHandlerContext context);
	}

	/// <summary>
	/// Per-project-export context shared across every <see cref="IResourceFileHandler"/>
	/// invocation in a single export run. Carries the three host-supplied parameters the
	/// handler actually needs (settings, cancellation, output directory) and collects
	/// partial-type info + additional MSBuild properties for the project file emitter to
	/// stitch into the produced .csproj. Lives in ILSpyX rather than in the host so plugin
	/// authors and headless consumers like ilspycmd can implement and host handlers
	/// without depending on the GUI app.
	/// </summary>
	public class ResourceFileHandlerContext
	{
		readonly List<PartialTypeInfo> partialTypes = new();
		public List<PartialTypeInfo> PartialTypes => partialTypes;

		readonly Dictionary<string, string> additionalProperties = new();
		public Dictionary<string, string> AdditionalProperties => additionalProperties;

		public DecompilerSettings DecompilerSettings { get; }
		public CancellationToken CancellationToken { get; }
		public string? SaveAsProjectDirectory { get; }

		public ResourceFileHandlerContext(DecompilerSettings decompilerSettings, CancellationToken cancellationToken, string? saveAsProjectDirectory)
		{
			this.DecompilerSettings = decompilerSettings;
			this.CancellationToken = cancellationToken;
			this.SaveAsProjectDirectory = saveAsProjectDirectory;
		}

		public void AddPartialTypeInfo(PartialTypeInfo info)
		{
			this.PartialTypes.Add(info);
		}
	}
}
