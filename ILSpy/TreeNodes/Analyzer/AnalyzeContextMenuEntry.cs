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
using System.Linq;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	[ExportContextMenuEntry(Header = "Analyze", Icon = "images/Search.png", Category = "Analyze", Order = 100)]
	internal sealed class AnalyzeContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.TreeView is AnalyzerTreeView && context.SelectedTreeNodes != null && context.SelectedTreeNodes.All(n => n.Parent.IsRoot))
				return false;
			if (context.SelectedTreeNodes == null)
				return context.Reference != null && IsValidReference(context.Reference.Reference);
			return context.SelectedTreeNodes.All(n => n is IMemberTreeNode);
		}

		public bool IsEnabled(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return context.Reference != null && context.Reference.Reference is IMetadataEntity;
			foreach (IMemberTreeNode node in context.SelectedTreeNodes) {
				if (!IsValidReference(node.Member))
					return false;
			}

			return true;
		}

		bool IsValidReference(object reference)
		{
			return reference is IMetadataEntity
				|| reference is MemberReference
				|| reference is MethodSpecification
				|| reference is TypeReference
				|| reference is TypeSpecification;
		}

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes != null) {
				foreach (IMemberTreeNode node in context.SelectedTreeNodes) {
					Analyze(node.Member);
				}
			} else if (context.Reference != null && IsValidReference(context.Reference.Reference)) {
				Analyze(context.Reference.Reference);
			}
		}

		public static void Analyze(object member)
		{
			switch (member) {
				case IMetadataEntity entity:
					switch (entity) {
						case TypeDefinition td:
							if (!td.IsNil)
								AnalyzerTreeView.Instance.ShowOrFocus(new AnalyzedTypeTreeNode(td));
							break;
						case FieldDefinition fd:
							//if (!fd.IsNil)
							//	AnalyzerTreeView.Instance.ShowOrFocus(new AnalyzedFieldTreeNode(fd));
							break;
						case MethodDefinition md:
							if (!md.IsNil)
								AnalyzerTreeView.Instance.ShowOrFocus(new AnalyzedMethodTreeNode(md.Module, md.Handle));
							break;
						case PropertyDefinition pd:
							//if (!pd.IsNil)
							//	AnalyzerTreeView.Instance.ShowOrFocus(new AnalyzedPropertyTreeNode(pd));
							break;
						case EventDefinition ed:
							//if (!ed.IsNil)
							//	AnalyzerTreeView.Instance.ShowOrFocus(new AnalyzedEventTreeNode(ed));
							break;
						default:
							throw new NotSupportedException();
					}
					break;
				case TypeReference tr:
					var resolved = tr.Handle.Resolve(new SimpleMetadataResolveContext(tr.Module));
					if (!resolved.IsNil)
						AnalyzerTreeView.Instance.ShowOrFocus(new AnalyzedTypeTreeNode(resolved));
					break;
				case TypeSpecification ts:
					resolved = ts.Handle.Resolve(new SimpleMetadataResolveContext(ts.Module));
					if (!resolved.IsNil)
						AnalyzerTreeView.Instance.ShowOrFocus(new AnalyzedTypeTreeNode(resolved));
					break;
				case MemberReference mr:
					var resolvedMember = mr.Handle.Resolve(new SimpleMetadataResolveContext(mr.Module));
					if (!resolvedMember.IsNil) {
						switch (resolvedMember) {
							case FieldDefinition fd:
								//AnalyzerTreeView.Instance.ShowOrFocus(new AnalyzedFieldTreeNode(fd));
								break;
							case MethodDefinition md:
								AnalyzerTreeView.Instance.ShowOrFocus(new AnalyzedMethodTreeNode(md.Module, md.Handle));
								break;
							default:
								throw new NotSupportedException();
						}
					}
					break;
				case MethodSpecification ms:
					resolvedMember = ms.Handle.Resolve(new SimpleMetadataResolveContext(ms.Module));
					if (!resolvedMember.IsNil) {
						switch (resolvedMember) {
							case MethodDefinition md:
								AnalyzerTreeView.Instance.ShowOrFocus(new AnalyzedMethodTreeNode(md.Module, md.Handle));
								break;
							default:
								throw new NotSupportedException();
						}
					}
					break;
			}
		}
	}
}
