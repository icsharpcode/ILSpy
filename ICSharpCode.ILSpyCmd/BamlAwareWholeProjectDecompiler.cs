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
using System.IO;
using System.Threading;

using ICSharpCode.BamlDecompiler;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpyCmd
{
	sealed class BamlAwareWholeProjectDecompiler : WholeProjectDecompiler
	{
		readonly BamlDecompilerTypeSystem bamlTypeSystem;
		readonly BamlDecompilerSettings bamlSettings;

		public BamlAwareWholeProjectDecompiler(
			DecompilerSettings settings,
			IAssemblyResolver assemblyResolver,
			IAssemblyReferenceClassifier assemblyReferenceClassifier,
			IDebugInfoProvider debugInfoProvider,
			BamlDecompilerTypeSystem bamlTypeSystem,
			BamlDecompilerSettings bamlSettings)
			: base(settings, assemblyResolver, projectWriter: null, assemblyReferenceClassifier, debugInfoProvider)
		{
			this.bamlTypeSystem = bamlTypeSystem ?? throw new ArgumentNullException(nameof(bamlTypeSystem));
			this.bamlSettings = bamlSettings ?? throw new ArgumentNullException(nameof(bamlSettings));
		}

		public CancellationToken CancellationToken { get; set; }

		protected override IEnumerable<ProjectItemInfo> WriteResourceToFile(string fileName, string resourceName, Stream entryStream)
		{
			if (!fileName.EndsWith(".baml", StringComparison.OrdinalIgnoreCase))
				return base.WriteResourceToFile(fileName, resourceName, entryStream);

			var decompiler = new XamlDecompiler(bamlTypeSystem, bamlSettings) {
				CancellationToken = CancellationToken
			};
			var result = decompiler.Decompile(entryStream);

			string xamlFileName;
			PartialTypeInfo partialTypeInfo = null;

			var typeDefinition = result.TypeName.HasValue
				? bamlTypeSystem.MainModule.GetTypeDefinition(result.TypeName.Value.TopLevelTypeName)
				: null;
			if (typeDefinition != null)
			{
				xamlFileName = SanitizeFileName(typeDefinition.ReflectionName + ".xaml");
				partialTypeInfo = new PartialTypeInfo(typeDefinition);
				foreach (var member in result.GeneratedMembers)
					partialTypeInfo.AddDeclaredMember(member);
			}
			else
			{
				xamlFileName = Path.ChangeExtension(fileName, ".xaml");
			}

			string fullPath = Path.Combine(TargetDirectory, xamlFileName);
			string directory = Path.GetDirectoryName(fullPath);
			if (!string.IsNullOrEmpty(directory))
				Directory.CreateDirectory(directory);
			result.Xaml.Save(fullPath);

			var item = new ProjectItemInfo("Page", xamlFileName)
				.With("Generator", "MSBuild:Compile")
				.With("SubType", "Designer");
			if (partialTypeInfo != null)
				item.PartialTypes = new List<PartialTypeInfo> { partialTypeInfo };

			return new[] { item };
		}
	}
}
