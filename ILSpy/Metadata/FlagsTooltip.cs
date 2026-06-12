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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Rich cell tooltip for an enum-valued metadata column: it breaks a flags value down into
	/// per-bit groups, showing each named flag with a checkbox reflecting whether the bit is set
	/// (multiple-choice groups) or the single selected member of a mutually exclusive sub-range
	/// (single-choice groups). Entries build one with a collection initializer of
	/// <see cref="FlagGroup"/>s; <see cref="MetadataCellTooltip"/> renders it via <see cref="Build"/>.
	/// </summary>
	public sealed class FlagsTooltip : IEnumerable<FlagGroup>
	{
		// The (value, flagsType) ctor parameters are unused but kept so entry call sites read the
		// same as the value they describe; the groups carry the actual selection state.
		public FlagsTooltip(int value = 0, Type? flagsType = null)
		{
		}

		public List<FlagGroup> Groups { get; } = new List<FlagGroup>();

		public void Add(FlagGroup group) => Groups.Add(group);

		public IEnumerator<FlagGroup> GetEnumerator() => Groups.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => Groups.GetEnumerator();

		/// <summary>
		/// Materialises the tooltip into an Avalonia control: a vertical stack of group views.
		/// </summary>
		public Control Build()
		{
			var panel = new StackPanel { Orientation = Orientation.Vertical };
			foreach (var group in Groups)
				panel.Children.Add(group.Build());
			return panel;
		}

		/// <summary>
		/// Tooltip for a <see cref="TypeAttributes"/> column: one single-choice group per
		/// mutually exclusive sub-range (visibility, layout, class semantics, string format,
		/// custom format) plus a multiple-choice group for the remaining independent bits.
		/// Shared by every table whose rows carry type attributes (TypeDef, ExportedType).
		/// </summary>
		public static FlagsTooltip ForTypeAttributes(TypeAttributes attributes)
		{
			const TypeAttributes otherFlagsMask = ~(TypeAttributes.VisibilityMask | TypeAttributes.LayoutMask | TypeAttributes.ClassSemanticsMask | TypeAttributes.StringFormatMask | TypeAttributes.CustomFormatMask);
			// ECMA-335 II.23.1.15 Forwarder: set on ExportedType rows that forward the type to
			// another assembly. System.Reflection.TypeAttributes has no member for the bit, so
			// the enum-driven GetFlags enumeration cannot surface it; inject it explicitly.
			const TypeAttributes isTypeForwarder = (TypeAttributes)0x00200000;
			return new FlagsTooltip {
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "Visibility: ", (int)TypeAttributes.VisibilityMask, (int)(attributes & TypeAttributes.VisibilityMask), new Flag("NotPublic (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "Class layout: ", (int)TypeAttributes.LayoutMask, (int)(attributes & TypeAttributes.LayoutMask), new Flag("AutoLayout (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "Class semantics: ", (int)TypeAttributes.ClassSemanticsMask, (int)(attributes & TypeAttributes.ClassSemanticsMask), new Flag("Class (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "String format: ", (int)TypeAttributes.StringFormatMask, (int)(attributes & TypeAttributes.StringFormatMask), new Flag("AnsiClass (0000)", 0, false), includeAny: false),
				FlagGroup.CreateSingleChoiceGroup(typeof(TypeAttributes), "Custom format: ", (int)TypeAttributes.CustomFormatMask, (int)(attributes & TypeAttributes.CustomFormatMask), new Flag("Value0 (0000)", 0, false), includeAny: false),
				new MultipleChoiceGroup(
					FlagGroup.GetFlags(typeof(TypeAttributes), (int)otherFlagsMask, (int)(attributes & otherFlagsMask))
						.Append(new Flag($"IsTypeForwarder ({(int)isTypeForwarder:X4})", (int)isTypeForwarder, (attributes & isTypeForwarder) != 0))) {
					Header = "Flags:",
					SelectedFlags = (int)(attributes & otherFlagsMask),
				},
			};
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
		public static MultipleChoiceGroup CreateMultipleChoiceGroup(Type flagsType, string? header = null, int mask = -1, int selectedValue = 0, bool includeAll = true)
		{
			return new MultipleChoiceGroup(GetFlags(flagsType, mask, selectedValue, includeAll ? "<All>" : null)) {
				Header = header,
				SelectedFlags = selectedValue,
			};
		}

		public static SingleChoiceGroup CreateSingleChoiceGroup(Type flagsType, string? header = null, int mask = -1, int selectedValue = 0, Flag defaultFlag = default, bool includeAny = true)
		{
			var group = new SingleChoiceGroup(GetFlags(flagsType, mask, selectedValue, includeAny ? "<Any>" : null)) {
				Header = header,
			};
			group.SelectedFlag = group.Flags.SingleOrDefault(f => f.Value == selectedValue);
			if (group.SelectedFlag.Name == null)
				group.SelectedFlag = defaultFlag;
			return group;
		}

		public static IEnumerable<Flag> GetFlags(Type flagsType, int mask = -1, int selectedValues = 0, string? neutralItem = null)
		{
			if (neutralItem != null)
				yield return new Flag(neutralItem, -1, false);

			foreach (var item in flagsType.GetFields(BindingFlags.Static | BindingFlags.Public))
			{
				if (item.Name.EndsWith("Mask", StringComparison.Ordinal))
					continue;
				int value = (int)CSharpPrimitiveCast.Cast(TypeCode.Int32, item.GetRawConstantValue(), false);
				if ((value & mask) == 0)
					continue;
				yield return new Flag($"{item.Name} ({value:X4})", value, (selectedValues & value) != 0);
			}
		}

		public string? Header { get; set; }

		public IList<Flag> Flags { get; protected set; } = Array.Empty<Flag>();

		public abstract Control Build();

		private protected static TextBlock? BuildHeader(string? header)
		{
			if (string.IsNullOrEmpty(header))
				return null;
			return new TextBlock { Text = header, FontWeight = FontWeight.Bold };
		}
	}

	public sealed class MultipleChoiceGroup : FlagGroup
	{
		public MultipleChoiceGroup(IEnumerable<Flag> flags)
		{
			this.Flags = flags.ToList();
		}

		public int SelectedFlags { get; set; }

		public override Control Build()
		{
			var panel = new StackPanel { Orientation = Orientation.Vertical, Margin = new Thickness(3) };
			var header = BuildHeader(Header);
			if (header != null)
			{
				header.Margin = new Thickness(0, 0, 0, 3);
				panel.Children.Add(header);
			}
			foreach (var flag in Flags)
			{
				panel.Children.Add(new CheckBox {
					Content = flag.Name,
					IsChecked = flag.IsSelected,
					IsHitTestVisible = false,
					Margin = new Thickness(3, 1),
				});
			}
			return panel;
		}
	}

	public sealed class SingleChoiceGroup : FlagGroup
	{
		public SingleChoiceGroup(IEnumerable<Flag> flags)
		{
			this.Flags = flags.ToList();
		}

		public Flag SelectedFlag { get; set; }

		public override Control Build()
		{
			var panel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(3), Spacing = 3 };
			var header = BuildHeader(Header);
			if (header != null)
				panel.Children.Add(header);
			panel.Children.Add(new TextBlock { Text = SelectedFlag.Name });
			return panel;
		}
	}
}
