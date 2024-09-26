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
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class CustomDebugInformationTableTreeNode : DebugMetadataTableTreeNode
	{
		public CustomDebugInformationTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.CustomDebugInformation, metadataFile)
		{
		}

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			var view = Helpers.PrepareDataGrid(tabPage, this);

			view.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
			view.RowDetailsTemplateSelector = new CustomDebugInformationDetailsTemplateSelector();

			var list = new List<CustomDebugInformationEntry>();
			CustomDebugInformationEntry scrollTargetEntry = default;

			foreach (var row in metadataFile.Metadata.CustomDebugInformation)
			{
				CustomDebugInformationEntry entry = new CustomDebugInformationEntry(metadataFile, row);
				if (entry.RID == scrollTarget)
				{
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry?.RID > 0)
			{
				ScrollItemIntoView(view, scrollTargetEntry);
			}

			return true;
		}

		class CustomDebugInformationDetailsTemplateSelector : DataTemplateSelector
		{
			public override DataTemplate SelectTemplate(object item, DependencyObject container)
			{
				var entry = (CustomDebugInformationEntry)item;
				switch (entry.kind)
				{
					case CustomDebugInformationEntry.CustomDebugInformationKind.StateMachineHoistedLocalScopes:
					case CustomDebugInformationEntry.CustomDebugInformationKind.CompilationMetadataReferences:
					case CustomDebugInformationEntry.CustomDebugInformationKind.CompilationOptions:
					case CustomDebugInformationEntry.CustomDebugInformationKind.TupleElementNames:
						return (DataTemplate)MetadataTableViews.Instance["CustomDebugInformationDetailsDataGrid"];
					default:
						return (DataTemplate)MetadataTableViews.Instance["CustomDebugInformationDetailsTextBlob"];
				}
			}
		}

		class CustomDebugInformationEntry
		{
			readonly int? offset;
			readonly MetadataFile metadataFile;
			readonly CustomDebugInformationHandle handle;
			readonly CustomDebugInformation debugInfo;
			internal readonly CustomDebugInformationKind kind;

			internal enum CustomDebugInformationKind
			{
				None,
				Unknown,
				StateMachineHoistedLocalScopes,
				DynamicLocalVariables,
				DefaultNamespaces,
				EditAndContinueLocalSlotMap,
				EditAndContinueLambdaAndClosureMap,
				EncStateMachineStateMap,
				EmbeddedSource,
				SourceLink,
				MethodSteppingInformation,
				CompilationOptions,
				CompilationMetadataReferences,
				TupleElementNames,
				TypeDefinitionDocuments
			}

			static CustomDebugInformationKind GetKind(MetadataReader metadata, GuidHandle h)
			{
				if (h.IsNil)
					return CustomDebugInformationKind.None;
				var guid = metadata.GetGuid(h);
				if (KnownGuids.StateMachineHoistedLocalScopes == guid)
				{
					return CustomDebugInformationKind.StateMachineHoistedLocalScopes;
				}
				if (KnownGuids.DynamicLocalVariables == guid)
				{
					return CustomDebugInformationKind.DynamicLocalVariables;
				}
				if (KnownGuids.DefaultNamespaces == guid)
				{
					return CustomDebugInformationKind.DefaultNamespaces;
				}
				if (KnownGuids.EditAndContinueLocalSlotMap == guid)
				{
					return CustomDebugInformationKind.EditAndContinueLocalSlotMap;
				}
				if (KnownGuids.EditAndContinueLambdaAndClosureMap == guid)
				{
					return CustomDebugInformationKind.EditAndContinueLambdaAndClosureMap;
				}
				if (KnownGuids.EncStateMachineStateMap == guid)
				{
					return CustomDebugInformationKind.EncStateMachineStateMap;
				}
				if (KnownGuids.EmbeddedSource == guid)
				{
					return CustomDebugInformationKind.EmbeddedSource;
				}
				if (KnownGuids.SourceLink == guid)
				{
					return CustomDebugInformationKind.SourceLink;
				}
				if (KnownGuids.MethodSteppingInformation == guid)
				{
					return CustomDebugInformationKind.MethodSteppingInformation;
				}
				if (KnownGuids.CompilationOptions == guid)
				{
					return CustomDebugInformationKind.CompilationOptions;
				}
				if (KnownGuids.CompilationMetadataReferences == guid)
				{
					return CustomDebugInformationKind.CompilationMetadataReferences;
				}
				if (KnownGuids.TupleElementNames == guid)
				{
					return CustomDebugInformationKind.TupleElementNames;
				}
				if (KnownGuids.TypeDefinitionDocuments == guid)
				{
					return CustomDebugInformationKind.TypeDefinitionDocuments;
				}

				return CustomDebugInformationKind.Unknown;
			}

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public object Offset => offset == null ? "n/a" : (object)offset;

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(debugInfo.Parent);

			public void OnParentClick()
			{
				MessageBus.Send(this, new NavigateToReferenceEventArgs(new EntityReference(metadataFile, debugInfo.Parent, protocol: "metadata")));
			}

			string parentTooltip;
			public string ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, debugInfo.Parent);

			string kindString;
			public string Kind {
				get {
					if (kindString != null)
						return kindString;

					Guid guid;
					if (kind != CustomDebugInformationKind.None)
					{
						guid = metadataFile.Metadata.GetGuid(debugInfo.Kind);
					}
					else
					{
						guid = Guid.Empty;
					}
					kindString = kind switch {
						CustomDebugInformationKind.None => "",
						CustomDebugInformationKind.StateMachineHoistedLocalScopes => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - State Machine Hoisted Local Scopes (C# / VB) [{guid}]",
						CustomDebugInformationKind.DynamicLocalVariables => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Dynamic Local Variables (C#) [{guid}]",
						CustomDebugInformationKind.DefaultNamespaces => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Default Namespaces (VB) [{guid}]",
						CustomDebugInformationKind.EditAndContinueLocalSlotMap => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Edit And Continue Local Slot Map (C# / VB) [{guid}]",
						CustomDebugInformationKind.EditAndContinueLambdaAndClosureMap => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Edit And Continue Lambda And Closure Map (C# / VB) [{guid}]",
						CustomDebugInformationKind.EncStateMachineStateMap => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Edit And Continue State Machine State Map (C# / VB) [{guid}]",
						CustomDebugInformationKind.EmbeddedSource => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Embedded Source (C# / VB) [{guid}]",
						CustomDebugInformationKind.SourceLink => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Source Link (C# / VB) [{guid}]",
						CustomDebugInformationKind.MethodSteppingInformation => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Method Stepping Information (C# / VB) [{guid}]",
						CustomDebugInformationKind.CompilationOptions => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Compilation Options (C# / VB) [{guid}]",
						CustomDebugInformationKind.CompilationMetadataReferences => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Compilation Metadata References (C# / VB) [{guid}]",
						CustomDebugInformationKind.TupleElementNames => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Tuple Element Names (C#) [{guid}]",
						CustomDebugInformationKind.TypeDefinitionDocuments => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Type Definition Documents (C# / VB) [{guid}]",
						_ => $"{MetadataTokens.GetHeapOffset(debugInfo.Kind):X8} - Unknown [{guid}]",
					};
					return kindString;
				}
			}

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Value => MetadataTokens.GetHeapOffset(debugInfo.Value);

			public string ValueTooltip {
				get {
					if (debugInfo.Value.IsNil)
						return "<nil>";
					return metadataFile.Metadata.GetBlobReader(debugInfo.Value).ToHexString();
				}
			}

			object rowDetails;

			public object RowDetails {
				get {
					if (rowDetails != null)
						return rowDetails;

					if (debugInfo.Value.IsNil)
						return null;

					var reader = metadataFile.Metadata.GetBlobReader(debugInfo.Value);
					ArrayList list;

					switch (kind)
					{
						case CustomDebugInformationKind.None:
							return null;
						case CustomDebugInformationKind.StateMachineHoistedLocalScopes:
							list = new ArrayList();

							while (reader.RemainingBytes > 0)
							{
								uint offset = reader.ReadUInt32();
								uint length = reader.ReadUInt32();
								list.Add(new { StartOffset = offset, Length = length });
							}

							return rowDetails = list;
						case CustomDebugInformationKind.SourceLink:
							return reader.ReadUTF8(reader.RemainingBytes);
						case CustomDebugInformationKind.CompilationOptions:
							list = new ArrayList();

							while (reader.RemainingBytes > 0)
							{
								string name = reader.ReadUTF8StringNullTerminated();
								string value = reader.ReadUTF8StringNullTerminated();
								list.Add(new { Name = name, Value = value });
							}

							return rowDetails = list;
						case CustomDebugInformationKind.CompilationMetadataReferences:
							list = new ArrayList();

							while (reader.RemainingBytes > 0)
							{
								string fileName = reader.ReadUTF8StringNullTerminated();
								string aliases = reader.ReadUTF8StringNullTerminated();
								byte flags = reader.ReadByte();
								uint timestamp = reader.ReadUInt32();
								uint fileSize = reader.ReadUInt32();
								Guid guid = reader.ReadGuid();
								list.Add(new { FileName = fileName, Aliases = aliases, Flags = flags, Timestamp = timestamp, FileSize = fileSize, Guid = guid });
							}

							return rowDetails = list;
						case CustomDebugInformationKind.TupleElementNames:
							list = new ArrayList();
							while (reader.RemainingBytes > 0)
							{
								list.Add(new { ElementName = reader.ReadUTF8StringNullTerminated() });
							}
							return rowDetails = list;
						default:
							return reader.ToHexString();
					}
				}
			}

			public CustomDebugInformationEntry(MetadataFile metadataFile, CustomDebugInformationHandle handle)
			{
				this.metadataFile = metadataFile;
				this.offset = metadataFile.IsEmbedded ? null : (int?)metadataFile.Metadata.GetTableMetadataOffset(TableIndex.CustomDebugInformation)
					+ metadataFile.Metadata.GetTableRowSize(TableIndex.CustomDebugInformation) * (MetadataTokens.GetRowNumber(handle) - 1);
				this.handle = handle;
				this.debugInfo = metadataFile.Metadata.GetCustomDebugInformation(handle);
				this.kind = GetKind(metadataFile.Metadata, debugInfo.Kind);
			}
		}
	}
}
