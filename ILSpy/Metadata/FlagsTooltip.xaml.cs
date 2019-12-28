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
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Interaction logic for FlagsTooltip.xaml
	/// </summary>
	public partial class FlagsTooltip : IEnumerable<FlagGroup>
	{
		public FlagsTooltip(int value = 0, Type flagsType = null)
		{
			InitializeComponent();
		}

		public void Add(FlagGroup group)
		{
			Groups.Add(group);
		}

		public IEnumerator<FlagGroup> GetEnumerator()
		{
			return Groups.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Groups.GetEnumerator();
		}

		public List<FlagGroup> Groups { get; } = new List<FlagGroup>();
	}

	class FlagActiveConverter : DependencyObject, IValueConverter
	{
		public int Value {
			get { return (int)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(int), typeof(FlagActiveConverter), new PropertyMetadata(0));

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Value == (int)value || ((int)value & Value) != 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public readonly struct Flag
	{
		public string Name { get; }
		public int Value { get; }
		public bool IsSelected { get; }

		public Flag(string name, int value, bool isSelected)
		{
			this.Name = name;
			this.Value = value;
			this.IsSelected = isSelected;
		}
	}

	public abstract class FlagGroup
	{
		public static MultipleChoiceGroup CreateMultipleChoiceGroup(Type flagsType, string header = null, int mask = -1, int selectedValue = 0, bool includeAll = true)
		{
			MultipleChoiceGroup group = new MultipleChoiceGroup(GetFlags(flagsType, mask, selectedValue, includeAll ? "<All>" : null));
			group.Header = header;
			group.SelectedFlags = selectedValue;
			return group;
		}

		public static SingleChoiceGroup CreateSingleChoiceGroup(Type flagsType, string header = null, int mask = -1, int selectedValue = 0, Flag defaultFlag = default, bool includeAny = true)
		{
			var group = new SingleChoiceGroup(GetFlags(flagsType, mask, selectedValue, includeAny ? "<Any>" : null));
			group.Header = header;
			group.SelectedFlag = group.Flags.SingleOrDefault(f => f.Value == selectedValue);
			if (group.SelectedFlag.Name == null) {
				group.SelectedFlag = defaultFlag;
			}
			return group;
		}

		public static IEnumerable<Flag> GetFlags(Type flagsType, int mask = -1, int selectedValues = 0, string neutralItem = null)
		{
			if (neutralItem != null)
				yield return new Flag(neutralItem, -1, false);

			foreach (var item in flagsType.GetFields(BindingFlags.Static | BindingFlags.Public)) {
				if (item.Name.EndsWith("Mask", StringComparison.Ordinal))
					continue;
				int value = (int)CSharpPrimitiveCast.Cast(TypeCode.Int32, item.GetRawConstantValue(), false);
				if ((value & mask) == 0)
					continue;
				yield return new Flag($"{item.Name} ({value:X4})", value, (selectedValues & value) != 0);
			}
		}

		public string Header { get; set; }

		public IList<Flag> Flags { get; protected set; }
	}

	public class MultipleChoiceGroup : FlagGroup
	{
		public MultipleChoiceGroup(IEnumerable<Flag> flags)
		{
			this.Flags = flags.ToList();
		}

		public int SelectedFlags { get; set; }
	}

	public class SingleChoiceGroup : FlagGroup
	{
		public SingleChoiceGroup(IEnumerable<Flag> flags)
		{
			this.Flags = flags.ToList();
		}

		public Flag SelectedFlag { get; set; }
	}

	class NullVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return Visibility.Collapsed;
			return Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
