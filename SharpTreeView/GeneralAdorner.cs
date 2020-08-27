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

using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace ICSharpCode.TreeView
{
	public class GeneralAdorner : Adorner
	{
		public GeneralAdorner(UIElement target)
			: base(target)
		{
		}

		FrameworkElement child;

		public FrameworkElement Child {
			get {
				return child;
			}
			set {
				if (child != value)
				{
					RemoveVisualChild(child);
					RemoveLogicalChild(child);
					child = value;
					AddLogicalChild(value);
					AddVisualChild(value);
					InvalidateMeasure();
				}
			}
		}

		protected override int VisualChildrenCount {
			get { return child == null ? 0 : 1; }
		}

		protected override Visual GetVisualChild(int index)
		{
			return child;
		}

		protected override Size MeasureOverride(Size constraint)
		{
			if (child != null)
			{
				child.Measure(constraint);
				return child.DesiredSize;
			}
			return new Size();
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (child != null)
			{
				child.Arrange(new Rect(finalSize));
				return finalSize;
			}
			return new Size();
		}
	}
}
