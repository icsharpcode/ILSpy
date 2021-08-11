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

using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Analyzers.TreeNodes
{
	using ICSharpCode.Decompiler.TypeSystem;

	internal sealed class AnalyzedEventTreeNode : AnalyzerEntityTreeNode
	{
		readonly IEvent analyzedEvent;
		readonly string prefix;

		public AnalyzedEventTreeNode(IEvent analyzedEvent, string prefix = "")
		{
			this.analyzedEvent = analyzedEvent ?? throw new ArgumentNullException(nameof(analyzedEvent));
			this.prefix = prefix;
			this.LazyLoading = true;
		}

		public override IEntity Member => analyzedEvent;

		public override object Icon => EventTreeNode.GetIcon(analyzedEvent);

		// TODO: This way of formatting is not suitable for events which explicitly implement interfaces.
		public override object Text => prefix + Language.EventToString(analyzedEvent, includeDeclaringTypeName: true, includeNamespace: false, includeNamespaceOfDeclaringTypeName: true);

		protected override void LoadChildren()
		{
			if (analyzedEvent.CanAdd)
				this.Children.Add(new AnalyzedAccessorTreeNode(analyzedEvent.AddAccessor, "add"));
			if (analyzedEvent.CanRemove)
				this.Children.Add(new AnalyzedAccessorTreeNode(analyzedEvent.RemoveAccessor, "remove"));
			if (TryFindBackingField(analyzedEvent, out var backingField))
				this.Children.Add(new AnalyzedFieldTreeNode(backingField));

			//foreach (var accessor in analyzedEvent.OtherMethods)
			//	this.Children.Add(new AnalyzedAccessorTreeNode(accessor, null));

			var analyzers = App.ExportProvider.GetExports<IAnalyzer, IAnalyzerMetadata>("Analyzer");
			foreach (var lazy in analyzers.OrderBy(item => item.Metadata.Order))
			{
				var analyzer = lazy.Value;
				if (analyzer.Show(analyzedEvent))
				{
					this.Children.Add(new AnalyzerSearchTreeNode(analyzedEvent, analyzer, lazy.Metadata.Header));
				}
			}
		}

		bool TryFindBackingField(IEvent analyzedEvent, out IField backingField)
		{
			backingField = null;
			foreach (var field in analyzedEvent.DeclaringTypeDefinition.GetFields(options: GetMemberOptions.IgnoreInheritedMembers))
			{
				if (field.Name == analyzedEvent.Name && field.Accessibility == Accessibility.Private)
				{
					backingField = field;
					return true;
				}
			}
			return false;
		}
	}
}
