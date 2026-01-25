// Copyright (c) 2026 Siegfried Pammer
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

using ICSharpCode.Decompiler;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.ILSpy.TreeNodes
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	using ICSharpCode.Decompiler.Output;
	using ICSharpCode.Decompiler.TypeSystem;

	public sealed class ExtensionTreeNode : ILSpyTreeNode
	{
		public ExtensionTreeNode(ITypeDefinition typeDefinition, (IMethod Marker, IReadOnlyList<ITypeParameter> TypeParameters) extensionGroup, AssemblyTreeNode parentAssemblyNode)
		{
			this.ParentAssemblyNode = parentAssemblyNode ?? throw new ArgumentNullException(nameof(parentAssemblyNode));
			this.ContainerTypeDefinition = typeDefinition ?? throw new ArgumentNullException(nameof(typeDefinition));
			this.MarkerMethod = extensionGroup.Marker ?? throw new ArgumentNullException(nameof(extensionGroup.Marker));
			this.TypeParameters = extensionGroup.TypeParameters ?? throw new ArgumentNullException(nameof(extensionGroup.TypeParameters));
			this.LazyLoading = true;
		}

		public ITypeDefinition ContainerTypeDefinition { get; }

		public IMethod MarkerMethod { get; }

		public IReadOnlyList<ITypeParameter> TypeParameters { get; }

		public AssemblyTreeNode ParentAssemblyNode { get; }

		public override object Icon {
			get {
				var type = GetTypeDefinition();
				return Images.GetIcon(TypeTreeNode.GetTypeIcon(type, out bool isStatic), TypeTreeNode.GetOverlayIcon(type), isStatic, true);
			}
		}

		public override object Text => this.Language.TypeToString(GetTypeDefinition(), ConversionFlags.SupportExtensionDeclarations);

		public override object NavigationText => this.Language.TypeToString(GetTypeDefinition(), ConversionFlags.UseFullyQualifiedTypeNames | ConversionFlags.UseFullyQualifiedEntityNames | ConversionFlags.SupportExtensionDeclarations);

		private ITypeDefinition GetTypeDefinition()
		{
			return ((MetadataModule)ParentAssemblyNode.LoadedAssembly
				.GetMetadataFileOrNull()
				?.GetTypeSystemWithCurrentOptionsOrNull(SettingsService, AssemblyTreeModel.CurrentLanguageVersion)
				?.MainModule)?.GetDefinition((SRM.TypeDefinitionHandle)MarkerMethod.DeclaringTypeDefinition.MetadataToken);
		}

		protected override void LoadChildren()
		{
			var extensionInfo = ContainerTypeDefinition.ExtensionInfo;

			foreach (var field in extensionInfo.GetMembersOfGroup(MarkerMethod).OfType<IField>().OrderBy(f => f.Name, NaturalStringComparer.Instance))
			{
				this.Children.Add(new FieldTreeNode(field));
			}
			foreach (var property in extensionInfo.GetMembersOfGroup(MarkerMethod).OfType<IProperty>().OrderBy(p => p.Name, NaturalStringComparer.Instance))
			{
				this.Children.Add(new PropertyTreeNode(property));
			}
			foreach (var ev in extensionInfo.GetMembersOfGroup(MarkerMethod).OfType<IEvent>().OrderBy(e => e.Name, NaturalStringComparer.Instance))
			{
				this.Children.Add(new EventTreeNode(ev));
			}
			foreach (var method in extensionInfo.GetMembersOfGroup(MarkerMethod).OfType<IMethod>().OrderBy(m => m.Name, NaturalStringComparer.Instance))
			{
				if (method.MetadataToken.IsNil)
					continue;
				this.Children.Add(new MethodTreeNode(method));
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(language is CSharpLanguage);
			((CSharpLanguage)language).DecompileExtension(GetTypeDefinition(), output, options);
		}
	}
}
