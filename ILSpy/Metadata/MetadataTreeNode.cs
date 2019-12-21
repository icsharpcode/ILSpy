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
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	class MetadataTreeNode : ILSpyTreeNode
	{
		private PEFile module;
		private AssemblyTreeNode assemblyTreeNode;

		public MetadataTreeNode(PEFile module, AssemblyTreeNode assemblyTreeNode)
		{
			this.module = module;
			this.assemblyTreeNode = assemblyTreeNode;
			this.LazyLoading = true;
		}

		public override object Text => "Metadata";

		public override object Icon => Images.Library;

		public override bool View(TabPageModel tabPage)
		{
			tabPage.Title = Text.ToString();
			tabPage.SupportsLanguageSwitching = false;

			return false;
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, "Metadata");
		}

		protected override void LoadChildren()
		{
			this.Children.Add(new DosHeaderTreeNode(module));
			this.Children.Add(new CoffHeaderTreeNode(module));
			this.Children.Add(new OptionalHeaderTreeNode(module));
			this.Children.Add(new DataDirectoriesTreeNode(module));
			this.Children.Add(new ModuleTableTreeNode(module));
			this.Children.Add(new TypeRefTableTreeNode(module));
			this.Children.Add(new TypeDefTableTreeNode(module));
			this.Children.Add(new FieldTableTreeNode(module));
			this.Children.Add(new MethodTableTreeNode(module));
			this.Children.Add(new ParamTableTreeNode(module));
			this.Children.Add(new InterfaceImplTableTreeNode(module));
			this.Children.Add(new MemberRefTableTreeNode(module));
			this.Children.Add(new ConstantTableTreeNode(module));
			this.Children.Add(new CustomAttributeTableTreeNode(module));
			this.Children.Add(new DeclSecurityTableTreeNode(module));
			this.Children.Add(new ClassLayoutTableTreeNode(module));
			this.Children.Add(new StandAloneSigTableTreeNode(module));
			this.Children.Add(new EventMapTableTreeNode(module));
			this.Children.Add(new EventTableTreeNode(module));
			this.Children.Add(new PropertyMapTableTreeNode(module));
			this.Children.Add(new PropertyTableTreeNode(module));
			this.Children.Add(new MethodSemanticsTableTreeNode(module));
			this.Children.Add(new MethodImplTableTreeNode(module));
			this.Children.Add(new ModuleRefTableTreeNode(module));
			this.Children.Add(new TypeSpecTableTreeNode(module));
			this.Children.Add(new AssemblyTableTreeNode(module));
			this.Children.Add(new AssemblyRefTableTreeNode(module));
			this.Children.Add(new FileTableTreeNode(module));
			this.Children.Add(new ExportedTypeTableTreeNode(module));
			this.Children.Add(new ManifestResourceTableTreeNode(module));
			this.Children.Add(new GenericParamTableTreeNode(module));
			this.Children.Add(new MethodSpecTableTreeNode(module));
			this.Children.Add(new GenericParamConstraintTableTreeNode(module));
		}
	}

	class Entry
	{
		public string Member { get; set; }
		public int Offset { get; set; }
		public int Size { get; set; }
		public object Value { get; set; }
		public string Meaning { get; set; }

		public Entry(int offset, object value, int size, string member, string meaning)
		{
			this.Member = member;
			this.Offset = offset;
			this.Size = size;
			this.Value = value;
			this.Meaning = meaning;
		}
	}

	class ByteWidthConverter : IValueConverter
	{
		public static readonly ByteWidthConverter Instance = new ByteWidthConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return string.Format("{0:X" + 2 * ((Entry)value).Size + "}", ((Entry)value).Value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
