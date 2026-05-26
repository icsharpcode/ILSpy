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
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class CustomDebugInformationTableTreeNode : DebugMetadataTableTreeNode<CustomDebugInformationTableTreeNode.CustomDebugInformationEntry>
	{
		public CustomDebugInformationTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.CustomDebugInformation, metadataFile)
		{
		}

		protected override IReadOnlyList<CustomDebugInformationEntry> LoadTable()
		{
			var list = new List<CustomDebugInformationEntry>();
			foreach (var row in metadataFile.Metadata.CustomDebugInformation)
			{
				list.Add(new CustomDebugInformationEntry(metadataFile, row));
			}
			return list;
		}

		protected override void ConfigureDataGrid(DataGrid view)
		{
			view.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
			view.RowDetailsTemplateSelector = new CustomDebugInformationDetailsTemplateSelector();
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

		internal struct CustomDebugInformationEntry
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

			[ColumnInfo("X2", Kind = ColumnKind.HeapOffset)]
			public int Kind => MetadataTokens.GetHeapOffset(debugInfo.Kind);

			[ColumnInfo("D")]
			public Guid KindGUID => metadataFile.Metadata.GetGuid(debugInfo.Kind);

			public string KindString => kind switch {
				CustomDebugInformationKind.None => "",
				CustomDebugInformationKind.StateMachineHoistedLocalScopes => "State Machine Hoisted Local Scopes (C# / VB)",
				CustomDebugInformationKind.DynamicLocalVariables => "Dynamic Local Variables (C#)",
				CustomDebugInformationKind.DefaultNamespaces => "Default Namespaces (VB)",
				CustomDebugInformationKind.EditAndContinueLocalSlotMap => "Edit And Continue Local Slot Map (C# / VB)",
				CustomDebugInformationKind.EditAndContinueLambdaAndClosureMap => "Edit And Continue Lambda And Closure Map (C# / VB)",
				CustomDebugInformationKind.EncStateMachineStateMap => "Edit And Continue State Machine State Map (C# / VB)",
				CustomDebugInformationKind.EmbeddedSource => "Embedded Source (C# / VB)",
				CustomDebugInformationKind.SourceLink => "Source Link (C# / VB)",
				CustomDebugInformationKind.MethodSteppingInformation => "Method Stepping Information (C# / VB)",
				CustomDebugInformationKind.CompilationOptions => "Compilation Options (C# / VB)",
				CustomDebugInformationKind.CompilationMetadataReferences => "Compilation Metadata References (C# / VB)",
				CustomDebugInformationKind.TupleElementNames => "Tuple Element Names (C#)",
				CustomDebugInformationKind.TypeDefinitionDocuments => "Type Definition Documents (C# / VB)",
				_ => "Unknown",
			};

			public string Info => kind switch {
				CustomDebugInformationKind.EmbeddedSource => GetEmbeddedSourceFormat(),
				_ => null,
			};

			string GetEmbeddedSourceFormat()
			{
				if (debugInfo.Value.IsNil)
					return "{nill blob}";

				var reader = metadataFile.Metadata.GetBlobReader(debugInfo.Value);

				if (reader.RemainingBytes < 4)
					return "{blob too short}";

				var format = reader.ReadInt32();

				return format switch {
					< 0 => $"Unknown format '{format}', {reader.Length} bytes",
					0 => $"Raw, {reader.Length} bytes",
					> 1 => $"DEFLATE, {reader.Length} bytes, {format} uncompressed",
				};
			}

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Value => MetadataTokens.GetHeapOffset(debugInfo.Value);

			[ColumnInfo("X8")]
			public int ValueLength => metadataFile.Metadata.GetBlobReader(debugInfo.Value).Length;

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

						case CustomDebugInformationKind.EmbeddedSource:
							var embeddedSourceFormat = reader.ReadInt32();

							if (embeddedSourceFormat < 0) // unknown format, show raw data as hex
								return reader.ToHexString();

							var embeddedSourceBytes = reader.ReadBytes(reader.RemainingBytes);
							Stream embeddedSourceByteStream = new MemoryStream(embeddedSourceBytes);

							if (embeddedSourceFormat >= 0) // positive length means the data is compressed using DEFLATE
								embeddedSourceByteStream = new System.IO.Compression.DeflateStream(embeddedSourceByteStream, System.IO.Compression.CompressionMode.Decompress);

							var textReader = new StreamReader(embeddedSourceByteStream, detectEncodingFromByteOrderMarks: true);
							return textReader.ReadToEnd();

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
