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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class CustomDebugInformationTableTreeNode : DebugMetadataTableTreeNode
	{
		private readonly bool isEmbedded;

		public CustomDebugInformationTableTreeNode(PEFile module, MetadataReader metadata, bool isEmbedded)
			: base(HandleKind.CustomDebugInformation, module, metadata)
		{
			this.isEmbedded = isEmbedded;
		}

		public override object Text => $"37 CustomDebugInformation ({metadata.GetTableRowCount(TableIndex.CustomDebugInformation)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);
			var list = new List<CustomDebugInformationEntry>();
			CustomDebugInformationEntry scrollTargetEntry = default;

			foreach (var row in metadata.CustomDebugInformation) {
				CustomDebugInformationEntry entry = new CustomDebugInformationEntry(module, metadata, isEmbedded, row);
				if (entry.RID == scrollTarget) {
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 1) {
				ScrollItemIntoView(view, scrollTargetEntry);
			}

			return true;
		}

		struct CustomDebugInformationEntry
		{
			readonly int? offset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly CustomDebugInformationHandle handle;
			readonly CustomDebugInformation debugInfo;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public object Offset => offset == null ? "n/a" : (object)offset;

			[StringFormat("X8")]
			public int Parent => MetadataTokens.GetToken(debugInfo.Parent);

			public string ParentTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new GenericContext(default(TypeDefinitionHandle), module);
					debugInfo.Parent.WriteTo(module, output, context);
					return output.ToString();
				}
			}

			[StringFormat("X8")]
			public int Kind => MetadataTokens.GetHeapOffset(debugInfo.Kind);

			public string KindTooltip {
				get {
					if (debugInfo.Kind.IsNil)
						return null;
					var guid = metadata.GetGuid(debugInfo.Kind);
					if (KnownGuids.StateMachineHoistedLocalScopes == guid) {
						return "State Machine Hoisted Local Scopes (C# / VB) [" + guid + "]";
					}
					if (KnownGuids.DynamicLocalVariables == guid) {
						return "Dynamic Local Variables (C#) [" + guid + "]";
					}
					if (KnownGuids.DefaultNamespaces == guid) {
						return "Default Namespaces (VB) [" + guid + "]";
					}
					if (KnownGuids.EditAndContinueLocalSlotMap == guid) {
						return "Edit And Continue Local Slot Map (C# / VB) [" + guid + "]";
					}
					if (KnownGuids.EditAndContinueLambdaAndClosureMap == guid) {
						return "Edit And Continue Lambda And Closure Map (C# / VB) [" + guid + "]";
					}
					if (KnownGuids.EmbeddedSource == guid) {
						return "Embedded Source (C# / VB) [" + guid + "]";
					}
					if (KnownGuids.SourceLink == guid) {
						return "Source Link (C# / VB) [" + guid + "]";
					}
					if (KnownGuids.MethodSteppingInformation == guid) {
						return "Method Stepping Information (C# / VB) [" + guid + "]";
					}

					return $"Unknown [" + guid + "]";
				}
			}

			[StringFormat("X")]
			public int Value => MetadataTokens.GetHeapOffset(debugInfo.Value);

			public string ValueTooltip {
				get {
					if (debugInfo.Value.IsNil)
						return "<nil>";
					return metadata.GetBlobReader(debugInfo.Value).ToHexString();
				}
			}

			public CustomDebugInformationEntry(PEFile module, MetadataReader metadata, bool isEmbedded, CustomDebugInformationHandle handle)
			{
				this.offset = isEmbedded ? null : (int?)metadata.GetTableMetadataOffset(TableIndex.CustomDebugInformation)
					+ metadata.GetTableRowSize(TableIndex.CustomDebugInformation) * (MetadataTokens.GetRowNumber(handle) - 1);
				this.module = module;
				this.metadata = metadata;
				this.handle = handle;
				this.debugInfo = metadata.GetCustomDebugInformation(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "CustomDebugInformation");
		}
	}
}