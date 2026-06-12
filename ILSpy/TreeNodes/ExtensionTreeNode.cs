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
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Tree-node container for a single C# 14 explicit-extension declaration block
	/// — the new <c>extension&lt;T&gt;(ReceiverType source) { ... }</c> syntax. Each
	/// block surfaces under its containing static class as a sibling of the regular
	/// methods/properties; the block's own members (methods + properties) appear as
	/// children of this node when expanded.
	/// </summary>
	public sealed class ExtensionTreeNode : ILSpyTreeNode
	{
		public ExtensionTreeNode(
			ITypeDefinition typeDefinition,
			(IMethod Marker, IReadOnlyList<ITypeParameter> TypeParameters) extensionGroup,
			AssemblyTreeNode parentAssemblyNode)
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

		public override object Icon => Images.GetIcon(Images.Class, AccessOverlayIcon.Public, isStatic: false, isExtension: true);

		public override object Text
			=> Language.TypeToString(GetTypeDefinition(), ConversionFlags.SupportExtensionDeclarations);

		public override object NavigationText
			=> Language.TypeToString(GetTypeDefinition(),
				ConversionFlags.UseFullyQualifiedTypeNames
				| ConversionFlags.UseFullyQualifiedEntityNames
				| ConversionFlags.SupportExtensionDeclarations);

		// Uses the constructor-time marker reference directly. The marker comes from the
		// parent TypeTreeNode, which resolves through GetTypeSystemWithCurrentOptionsOrNull,
		// so the chain is consistent with the current settings; per-access re-resolution (so
		// language-version flips refresh the display string without rebuilding the node, as
		// WPF does) remains a follow-up (see `extension-methods-tree` in the tracker).
		ITypeDefinition GetTypeDefinition() => MarkerMethod.DeclaringTypeDefinition!;

		protected override void LoadChildren()
		{
			var extensionInfo = ContainerTypeDefinition.ExtensionInfo!;
			var members = extensionInfo.GetMembersOfGroup(MarkerMethod).ToList();

			foreach (var property in members.OfType<IProperty>().OrderBy(p => p.Name, NaturalStringComparer.Instance))
				this.Children.Add(new PropertyTreeNode(property));
			foreach (var method in members.OfType<IMethod>().OrderBy(m => m.Name, NaturalStringComparer.Instance))
			{
				if (method.MetadataToken.IsNil)
					continue;
				this.Children.Add(new MethodTreeNode(method));
			}
		}

		public override FilterResult Filter(LanguageSettings settings)
		{
			if (Language is not CSharpLanguage)
				return FilterResult.Hidden;

			var decompilerSettings = TryGetDecompilerSettings();
			if (decompilerSettings != null && !decompilerSettings.ExtensionMembers)
				return FilterResult.Hidden;

			return base.Filter(settings);
		}

		static ICSharpCode.Decompiler.DecompilerSettings? TryGetDecompilerSettings()
		{
			try
			{
				return AppComposition.Current.GetExport<ICSharpCode.ILSpy.SettingsService>().DecompilerSettings.Clone();
			}
			catch
			{
				return null;
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			Debug.Assert(language is CSharpLanguage);
			((CSharpLanguage)language).DecompileExtension(GetTypeDefinition(), output, options);
		}
	}
}
