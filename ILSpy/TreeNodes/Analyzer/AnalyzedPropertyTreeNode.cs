// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Reflection.Metadata;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	sealed class AnalyzedPropertyTreeNode : AnalyzerEntityTreeNode
	{
		readonly Decompiler.Metadata.PEFile module;
		readonly PropertyDefinitionHandle analyzedProperty;
		readonly bool isIndexer;
		readonly string prefix;

		public AnalyzedPropertyTreeNode(Decompiler.Metadata.PEFile module, PropertyDefinitionHandle analyzedProperty, string prefix = "")
		{
			if (analyzedProperty == null)
				throw new ArgumentNullException(nameof(analyzedProperty));
			using (LoadedAssembly.DisableAssemblyLoad()) {
				this.isIndexer = analyzedProperty.HasMatchingDefaultMemberAttribute(module, out _);
			}
			this.module = module;
			this.analyzedProperty = analyzedProperty;
			this.prefix = prefix;
			this.LazyLoading = true;
		}

		public override object Icon => PropertyTreeNode.GetIcon(new Decompiler.Metadata.PropertyDefinition(module, analyzedProperty), isIndexer);

		// TODO: This way of formatting is not suitable for properties which explicitly implement interfaces.
		public override object Text => prefix + Language.PropertyToString(new Decompiler.Metadata.PropertyDefinition(module, analyzedProperty), includeNamespace: true, includeTypeName: true, isIndexer: isIndexer);

		protected override void LoadChildren()
		{
			var accessors = module.Metadata.GetPropertyDefinition(analyzedProperty).GetAccessors();
			if (!accessors.Getter.IsNil)
				this.Children.Add(new AnalyzedPropertyAccessorTreeNode(module, accessors.Getter, "get"));
			if (!accessors.Setter.IsNil)
				this.Children.Add(new AnalyzedPropertyAccessorTreeNode(module, accessors.Setter, "set"));
			//foreach (var accessor in analyzedProperty.OtherMethods)
			//	this.Children.Add(new AnalyzedPropertyAccessorTreeNode(accessor, null));

			/*if (AnalyzedPropertyOverridesTreeNode.CanShow(analyzedProperty))
				this.Children.Add(new AnalyzedPropertyOverridesTreeNode(analyzedProperty));
			if (AnalyzedInterfacePropertyImplementedByTreeNode.CanShow(analyzedProperty))
				this.Children.Add(new AnalyzedInterfacePropertyImplementedByTreeNode(analyzedProperty));*/
		}

		/*public static AnalyzerTreeNode TryCreateAnalyzer(IMemberReference member)
		{
			if (CanShow(member))
				return new AnalyzedPropertyTreeNode((PropertyDefinition)member);
			else
				return null;
		}

		public static bool CanShow(IMemberReference member)
		{
			if (!(member is PropertyDefinition property))
				return false;

			return !MainWindow.Instance.CurrentLanguage.ShowMember(property.GetMethod.IsNil ? property.SetMethod : property.GetMethod)
			    || AnalyzedPropertyOverridesTreeNode.CanShow(property);
		}*/

		public override Decompiler.Metadata.IMetadataEntity Member => new Decompiler.Metadata.PropertyDefinition(module, analyzedProperty);
	}
}
