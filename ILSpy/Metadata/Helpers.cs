using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.ILSpy.Controls;

namespace ICSharpCode.ILSpy.Metadata
{
	static class Helpers
	{
		public static ListView CreateListView(string name)
		{
			MetadataTableViews dict = new MetadataTableViews();
			var view = new ListView {
				View = (GridView)dict[name],
				ItemContainerStyle = (Style)dict["ItemContainerStyle"]
			};
			ContextMenuProvider.Add(view);
			FilterableGridViewColumn.SetSortMode(view, ListViewSortMode.Automatic);
			foreach (var column in ((GridView)view.View).Columns) {
				FilterableGridViewColumn.SetParentView(column, view);
			}

			return view;
		}

		public static string AttributesToString(TypeAttributes attributes)
		{
			const TypeAttributes allMasks = TypeAttributes.ClassSemanticsMask | TypeAttributes.CustomFormatMask | TypeAttributes.LayoutMask | TypeAttributes.ReservedMask | TypeAttributes.StringFormatMask | TypeAttributes.VisibilityMask;
			StringBuilder sb = new StringBuilder();
			var visibility = attributes & TypeAttributes.VisibilityMask;
			sb.AppendLine("Visibility: " + (visibility == 0 ? "NotPublic" : typeof(TypeAttributes).GetEnumName(visibility)));
			var layout = attributes & TypeAttributes.LayoutMask;
			sb.AppendLine("Class layout: " + (layout == 0 ? "AutoLayout" : typeof(TypeAttributes).GetEnumName(layout)));
			var semantics = attributes & TypeAttributes.ClassSemanticsMask;
			sb.AppendLine("Class semantics: " + (semantics == 0 ? "Class" : typeof(TypeAttributes).GetEnumName(semantics)));
			var stringFormat = attributes & TypeAttributes.StringFormatMask;
			sb.AppendLine("String format: " + (stringFormat == 0 ? "AnsiClass" : typeof(TypeAttributes).GetEnumName(stringFormat)));
			var customStringFormat = attributes & TypeAttributes.CustomFormatMask;
			sb.AppendLine("Custom string format: 0x" + customStringFormat.ToString("x"));
			var reserved = attributes & TypeAttributes.ReservedMask;
			sb.AppendLine("Reserved attributes: " + (reserved == 0 ? "" : reserved.ToString()));
			var additional = attributes & ~allMasks;
			sb.Append("Additional attributes: ");
			AdditionalAttributes(sb, additional);
			if (sb.Length == 0)
				return null;
			return sb.ToString();
		}

		static void AdditionalAttributes(StringBuilder sb, TypeAttributes attributes)
		{
			bool first = true;
			for (int bit = 0; bit < 32; bit++) {
				var value = (TypeAttributes)(1 << bit);
				if ((attributes & value) != 0) {
					if (!first)
						sb.Append(", ");
					first = false;
					sb.Append(typeof(TypeAttributes).GetEnumName(value));
				}
			}
		}
	}
}
