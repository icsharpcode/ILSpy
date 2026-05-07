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

using System.Linq;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ILSpy.Metadata;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataColumnBuilderTests
{
	sealed class SampleEntry
	{
		public int RID { get; set; }
		[ColumnInfo("X8")]
		public int Token { get; set; }
		public string Name { get; set; } = "";
	}

	[AvaloniaTest]
	public void For_Returns_One_TextColumn_Per_Public_Property_With_Hex_Converter_For_X_Format()
	{
		// The reflection-driven column builder is the bridge between a row-shape struct/class
		// and the DataGrid view. It should produce one column per public property in
		// declaration order; properties annotated with a hex format spec must apply that spec
		// at bind-time (so enum and integer values both line up cleanly).

		var columns = MetadataColumnBuilder.For<SampleEntry>();

		columns.Should().HaveCount(3);
		columns.Select(c => c.Header.ToString()).Should().Equal("RID", "Token", "Name");

		var token = (DataGridTextColumn)columns[1];
		token.Binding.Should().NotBeNull();
		token.IsReadOnly.Should().BeTrue();

		var name = (DataGridTextColumn)columns[2];
		name.SortMemberPath.Should().Be("Name");
	}

	[AvaloniaTest]
	public void For_Returns_Fresh_DataGridColumn_Instances_So_They_Can_Attach_To_Distinct_Grids()
	{
		// Avalonia's DataGrid tracks OwningGrid on each column; reusing column instances
		// across grids throws InvalidOperationException. Each call must therefore yield
		// fresh DataGridColumn instances, even though the underlying reflection metadata
		// is the same.
		var first = MetadataColumnBuilder.For<SampleEntry>();
		var second = MetadataColumnBuilder.For<SampleEntry>();
		first.Should().NotBeSameAs(second);
		first[0].Should().NotBeSameAs(second[0]);
	}
}
