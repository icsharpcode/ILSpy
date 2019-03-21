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
			this.Children.Add(new ExportedTypeTableTreeNode(module));
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
