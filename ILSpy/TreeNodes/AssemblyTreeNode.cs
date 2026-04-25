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
using System.IO;
using System.Linq;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;

using ILSpy.Images;

namespace ILSpy.TreeNodes
{
	sealed class AssemblyTreeNode : ILSpyTreeNode
	{
		readonly LoadedAssembly assembly;
		bool loadFailed;
		string? loadError;

		public LoadedAssembly LoadedAssembly => assembly;

		public AssemblyTreeNode(LoadedAssembly assembly)
		{
			ArgumentNullException.ThrowIfNull(assembly);
			this.assembly = assembly;
			LazyLoading = true;
		}

		public override object Text => assembly.ShortName;

		public override object Icon => loadFailed ? Images.Images.AssemblyWarning : Images.Images.Assembly;

		public override object? ToolTip => loadFailed ? loadError : assembly.FileName;

		// When a load fails we still want the user to see the entry, but with the warning glyph
		// and no expander chevron (LoadChildren produced nothing, ShowExpander becomes false).
		public override bool ShowExpander => !loadFailed && base.ShowExpander;

		protected override void LoadChildren()
		{
			MetadataFile? module;
			try
			{
				module = assembly.GetMetadataFileOrNullAsync().Result;
			}
			catch (Exception ex)
			{
				module = null;
				loadError = $"Failed to load '{assembly.FileName}':\n{ex.GetBaseException().Message}";
			}

			if (module == null)
			{
				loadFailed = true;
				if (loadError == null)
				{
					loadError = File.Exists(assembly.FileName)
						? $"Failed to load '{assembly.FileName}'."
						: $"File not found:\n{assembly.FileName}";
				}
				RaisePropertyChanged(nameof(Icon));
				RaisePropertyChanged(nameof(ShowExpander));
				RaisePropertyChanged(nameof(ToolTip));
				return;
			}

			var metadata = module.Metadata;
			var namespaces = metadata.TypeDefinitions
				.Where(t => metadata.GetTypeDefinition(t).GetDeclaringType().IsNil)
				.Select(t => metadata.GetString(metadata.GetTypeDefinition(t).Namespace))
				.Where(ns => !string.IsNullOrEmpty(ns))
				.Distinct()
				.OrderBy(ns => ns);

			foreach (var ns in namespaces)
				Children.Add(new NamespaceTreeNode(ns, module));

			var globalTypes = metadata.TypeDefinitions
				.Where(t => {
					var td = metadata.GetTypeDefinition(t);
					return td.GetDeclaringType().IsNil
						&& string.IsNullOrEmpty(metadata.GetString(td.Namespace));
				});

			foreach (var t in globalTypes)
				Children.Add(new TypeTreeNode(t, module));
		}
	}
}
