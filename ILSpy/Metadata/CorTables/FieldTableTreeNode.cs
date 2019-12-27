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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	internal class FieldTableTreeNode : MetadataTableTreeNode
	{
		public FieldTableTreeNode(PEFile module)
			: base(HandleKind.FieldDefinition, module)
		{
		}

		public override object Text => $"04 Field ({module.Metadata.GetTableRowCount(TableIndex.Field)})";

		public override object Icon => Images.Literal;

		public override bool View(ViewModels.TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;
			var view = Helpers.PrepareDataGrid(tabPage);
			var metadata = module.Metadata;

			var list = new List<FieldDefEntry>();

			FieldDefEntry scrollTargetEntry = default;

			foreach (var row in metadata.FieldDefinitions) {
				var entry = new FieldDefEntry(module, row);
				if (scrollTarget.Equals(row)) {
					scrollTargetEntry = entry;
				}
				list.Add(entry);
			}

			view.ItemsSource = list;

			tabPage.Content = view;

			if (scrollTargetEntry.RID > 0) {
				view.ScrollIntoView(scrollTargetEntry);
				this.scrollTarget = default;
			}

			return true;
		}

		struct FieldDefEntry : IMemberTreeNode
		{
			readonly int metadataOffset;
			readonly PEFile module;
			readonly MetadataReader metadata;
			readonly FieldDefinitionHandle handle;
			readonly FieldDefinition fieldDef;

			public int RID => MetadataTokens.GetRowNumber(handle);

			public int Token => MetadataTokens.GetToken(handle);

			public int Offset => metadataOffset
				+ metadata.GetTableMetadataOffset(TableIndex.Field)
				+ metadata.GetTableRowSize(TableIndex.Field) * (RID - 1);

			[StringFormat("X8")]
			public FieldAttributes Attributes => fieldDef.Attributes;

			const FieldAttributes otherFlagsMask = ~(FieldAttributes.FieldAccessMask);

			public object AttributesTooltip => new FlagsTooltip() {
				FlagGroup.CreateSingleChoiceGroup(typeof(FieldAttributes), "Field access: ", (int)FieldAttributes.FieldAccessMask, (int)(fieldDef.Attributes & FieldAttributes.FieldAccessMask), new Flag("CompilerControlled (0000)", 0, false), includeAny: false),
				FlagGroup.CreateMultipleChoiceGroup(typeof(FieldAttributes), "Flags:", (int)otherFlagsMask, (int)(fieldDef.Attributes & otherFlagsMask), includeAll: false),
			};

			public string Name => metadata.GetString(fieldDef.Name);

			public string NameTooltip => $"{MetadataTokens.GetHeapOffset(fieldDef.Name):X} \"{Name}\"";

			IEntity IMemberTreeNode.Member => ((MetadataModule)module.GetTypeSystemOrNull()?.MainModule).GetDefinition(handle);

			[StringFormat("X")]
			public int Signature => MetadataTokens.GetHeapOffset(fieldDef.Signature);

			public string SignatureTooltip {
				get {
					ITextOutput output = new PlainTextOutput();
					var context = new Decompiler.Metadata.GenericContext(default(TypeDefinitionHandle), module);
					((EntityHandle)handle).WriteTo(module, output, context);
					return output.ToString();
				}
			}

			public FieldDefEntry(PEFile module, FieldDefinitionHandle handle)
			{
				this.metadataOffset = module.Reader.PEHeaders.MetadataStartOffset;
				this.module = module;
				this.metadata = module.Metadata;
				this.handle = handle;
				this.fieldDef = metadata.GetFieldDefinition(handle);
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "FieldDefs");
		}
	}
}

class Time : IDisposable
{
	readonly System.Diagnostics.Stopwatch stopwatch;
	readonly string title;

	public Time(string title)
	{
		this.title = title;
		this.stopwatch = new System.Diagnostics.Stopwatch();
		stopwatch.Start();
	}

	public void Dispose()
	{
		stopwatch.Stop();
		System.Diagnostics.Debug.WriteLine(title + " took " + stopwatch.ElapsedMilliseconds + "ms");
	}
}