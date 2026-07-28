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

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Metadata.CorTables;
using ICSharpCode.ILSpy.ViewModels;

using Avalonia.Headless.NUnit;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

/// <summary>
/// A type that owns no rows in the Field or MethodDef tables: its stored FieldList and
/// MethodList column values are empty-list start positions, not references to own
/// members. An interface cannot acquire an implicit constructor, so memberlessness is
/// structurally guaranteed.
/// </summary>
public interface IMemberlessSampleType
{
}

[TestFixture]
public class TypeDefMemberListColumnTests
{
	static PEFile? testAssembly;
	static List<TypeDefTableTreeNode.TypeDefEntry>? entries;

	static PEFile LoadAssembly()
	{
		return testAssembly ??= new PEFile(typeof(TypeDefMemberListColumnTests).Assembly.Location);
	}

	static List<TypeDefTableTreeNode.TypeDefEntry> LoadEntries()
	{
		if (entries != null)
			return entries;
		var page = (MetadataTablePageModel)new TypeDefTableTreeNode(LoadAssembly()).CreateTab();
		return entries = page.Items.Cast<TypeDefTableTreeNode.TypeDefEntry>().ToList();
	}

	[OneTimeTearDown]
	public void Unload()
	{
		testAssembly?.Dispose();
	}

	static int Row(int token) => token & 0x00ffffff;

	[AvaloniaTest]
	public void MemberlessTypeShowsStoredListValues()
	{
		var entries = LoadEntries();
		var entry = entries.Single(e => e.Name == nameof(IMemberlessSampleType));

		// The stored FieldList/MethodList of a memberless type is the running list
		// position (the next type's first member row, or one past the table end) -
		// never row 0.
		Assert.That(Row(entry.FieldList), Is.Not.Zero);
		Assert.That(Row(entry.MethodList), Is.Not.Zero);
	}

	[AvaloniaTest]
	public void ParameterlessMethodShowsStoredParamListValue()
	{
		var page = (MetadataTablePageModel)new MethodTableTreeNode(LoadAssembly()).CreateTab();
		var methods = page.Items.Cast<MethodTableTreeNode.MethodDefEntry>().ToList();
		int paramRows = testAssembly!.Metadata.GetTableRowCount(TableIndex.Param);

		var parameterless = methods.Single(m => m.Name == nameof(ParameterlessMethodShowsStoredParamListValue));
		Assert.That(Row(parameterless.ParamList), Is.Not.Zero);

		for (int i = 0; i < methods.Count; i++)
		{
			Assert.That(Row(methods[i].ParamList), Is.InRange(1, paramRows + 1), $"ParamList of row {i + 1}");
			if (i > 0)
			{
				Assert.That(Row(methods[i].ParamList), Is.GreaterThanOrEqualTo(Row(methods[i - 1].ParamList)), $"ParamList of row {i + 1}");
			}
		}
	}

	[AvaloniaTest]
	public void ListColumnsAreMonotonicallyNonDecreasing()
	{
		var entries = LoadEntries();
		int fieldRows = testAssembly!.Metadata.GetTableRowCount(TableIndex.Field);
		int methodRows = testAssembly.Metadata.GetTableRowCount(TableIndex.MethodDef);

		for (int i = 0; i < entries.Count; i++)
		{
			Assert.That(Row(entries[i].FieldList), Is.InRange(1, fieldRows + 1), $"FieldList of row {i + 1}");
			Assert.That(Row(entries[i].MethodList), Is.InRange(1, methodRows + 1), $"MethodList of row {i + 1}");
			if (i > 0)
			{
				Assert.That(Row(entries[i].FieldList), Is.GreaterThanOrEqualTo(Row(entries[i - 1].FieldList)), $"FieldList of row {i + 1}");
				Assert.That(Row(entries[i].MethodList), Is.GreaterThanOrEqualTo(Row(entries[i - 1].MethodList)), $"MethodList of row {i + 1}");
			}
		}
	}
}
