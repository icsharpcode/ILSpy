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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ICSharpCode.ILSpy.Options;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// ExtensionMethods used in ILSpy.
	/// </summary>
	public static class ExtensionMethods
	{
		public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> items)
		{
			foreach (T item in items)
				if (!list.Contains(item))
					list.Add(item);
		}

		public static T PeekOrDefault<T>(this Stack<T> stack)
		{
			if (stack.Count == 0)
				return default(T);
			return stack.Peek();
		}

		public static int BinarySearch<T>(this IList<T> list, T item, int start, int count, IComparer<T> comparer)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));
			if (start < 0 || start >= list.Count)
				throw new ArgumentOutOfRangeException(nameof(start), start, "Value must be between 0 and " + (list.Count - 1));
			if (count < 0 || count > list.Count - start)
				throw new ArgumentOutOfRangeException(nameof(count), count, "Value must be between 0 and " + (list.Count - start));
			int end = start + count - 1;
			while (start <= end) {
				int pivot = (start + end) / 2;
				int result = comparer.Compare(item, list[pivot]);
				if (result == 0)
					return pivot;
				if (result < 0)
					end = pivot - 1;
				else
					start = pivot + 1;
			}
			return ~start;
		}

		public static int BinarySearch<T, TKey>(this IList<T> instance, TKey itemKey, Func<T, TKey> keySelector)
			where TKey : IComparable<TKey>, IComparable
		{
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));
			if (keySelector == null)
				throw new ArgumentNullException(nameof(keySelector));

			int start = 0;
			int end = instance.Count - 1;

			while (start <= end) {
				int m = (start + end) / 2;
				TKey key = keySelector(instance[m]);
				int result = key.CompareTo(itemKey);
				if (result == 0)
					return m;
				if (result < 0)
					start = m + 1;
				else
					end = m - 1;
			}
			return ~start;
		}
		/*
		public static bool IsCustomAttribute(this TypeDefinition type)
		{
			while (type.FullName != "System.Object") {
				var resolvedBaseType = type.BaseType.Resolve();
				if (resolvedBaseType == null)
					return false;
				if (resolvedBaseType.FullName == "System.Attribute")
					return true;
				type = resolvedBaseType;
			}
			return false;
		}
		*/
		public static string ToSuffixString(this System.Reflection.Metadata.EntityHandle handle)
		{
			if (!DisplaySettingsPanel.CurrentDisplaySettings.ShowMetadataTokens)
				return string.Empty;

			int token = System.Reflection.Metadata.Ecma335.MetadataTokens.GetToken(handle);
			if (DisplaySettingsPanel.CurrentDisplaySettings.ShowMetadataTokensInBase10)
				return " @" + token;
			return " @" + token.ToString("x8");
		}

		public static string ToSuffixString(this System.Reflection.Metadata.MethodDefinitionHandle handle)
		{
			return ToSuffixString((System.Reflection.Metadata.EntityHandle)handle);
		}

		public static string ToSuffixString(this System.Reflection.Metadata.PropertyDefinitionHandle handle)
		{
			return ToSuffixString((System.Reflection.Metadata.EntityHandle)handle);
		}

		public static string ToSuffixString(this System.Reflection.Metadata.EventDefinitionHandle handle)
		{
			return ToSuffixString((System.Reflection.Metadata.EntityHandle)handle);
		}

		public static string ToSuffixString(this System.Reflection.Metadata.FieldDefinitionHandle handle)
		{
			return ToSuffixString((System.Reflection.Metadata.EntityHandle)handle);
		}

		public static string ToSuffixString(this System.Reflection.Metadata.TypeDefinitionHandle handle)
		{
			return ToSuffixString((System.Reflection.Metadata.EntityHandle)handle);
		}

		/// <summary>
		/// Takes at most <paramref name="length" /> first characters from string, and appends '...' if string is longer.
		/// String can be null.
		/// </summary>
		public static string TakeStartEllipsis(this string s, int length)
		{
			if (string.IsNullOrEmpty(s) || length >= s.Length)
				return s;
			return s.Substring(0, length) + "...";
		}

		/// <summary>
		/// Equivalent to <code>collection.Select(func).ToArray()</code>, but more efficient as it makes
		/// use of the input collection's known size.
		/// </summary>
		public static U[] SelectArray<T, U>(this ICollection<T> collection, Func<T, U> func)
		{
			U[] result = new U[collection.Count];
			int index = 0;
			foreach (var element in collection) {
				result[index++] = func(element);
			}
			return result;
		}

		#region DPI independence
		public static Rect TransformToDevice(this Rect rect, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return Rect.Transform(rect, matrix);
		}

		public static Rect TransformFromDevice(this Rect rect, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
			return Rect.Transform(rect, matrix);
		}

		public static Size TransformToDevice(this Size size, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
		}

		public static Size TransformFromDevice(this Size size, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
			return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
		}

		public static Point TransformToDevice(this Point point, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
			return matrix.Transform(point);
		}

		public static Point TransformFromDevice(this Point point, Visual visual)
		{
			Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
			return matrix.Transform(point);
		}
		#endregion

		public static T FindVisualChild<T>(this DependencyObject depObj) where T : DependencyObject
		{
			if (depObj != null) {
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
					DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
					if (child != null && child is T) {
						return (T)child;
					}

					T childItem = FindVisualChild<T>(child);
					if (childItem != null) return childItem;
				}
			}
			return null;
		}

		public static T GetParent<T>(this DependencyObject depObj) where T : DependencyObject
		{
			if (depObj == null)
				return null;
			while (!(depObj is T)) {
				var parent = VisualTreeHelper.GetParent(depObj);
				if (parent == null)
					return null;
				depObj = parent;
			}
			return (T)depObj;
		}

		public static void SelectItem(this DataGrid view, object item)
		{
			var container = (DataGridRow)view.ItemContainerGenerator.ContainerFromItem(item);
			if (container != null)
				container.IsSelected = true;
			view.Focus();
		}
	}
}
