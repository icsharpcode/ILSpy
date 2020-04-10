// Copyright (c) 2020 AlphaSierraPapa for the SharpDevelop Team
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
using System.Windows.Media;
using System.Windows;
using System.Collections;

namespace ICSharpCode.TreeView
{
	static class ExtensionMethods
	{
		public static T FindAncestor<T>(this DependencyObject d) where T : class
		{
			return AncestorsAndSelf(d).OfType<T>().FirstOrDefault();
		}

		public static IEnumerable<DependencyObject> AncestorsAndSelf(this DependencyObject d)
		{
			while (d != null) {
				yield return d;
				d = VisualTreeHelper.GetParent(d);
			}
		}

		public static void AddOnce(this IList list, object item)
		{
			if (!list.Contains(item)) {
				list.Add(item);
			}
		}
	}
}
