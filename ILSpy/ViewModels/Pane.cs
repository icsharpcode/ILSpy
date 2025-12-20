// Copyright (c) 2019 AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.ILSpy.ViewModels
{
	public static class Pane
	{
		// Helper properties to enable binding state properties from the model to the view.

		public static readonly DependencyProperty IsActiveProperty = DependencyProperty.RegisterAttached(
			"IsActive", typeof(bool), typeof(Pane), new FrameworkPropertyMetadata(default(bool)));
		public static void SetIsActive(DependencyObject element, bool value)
		{
			element.SetValue(IsActiveProperty, value);
		}
		public static bool GetIsActive(DependencyObject element)
		{
			return (bool)element.GetValue(IsActiveProperty);
		}

		public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached(
			"IsVisible", typeof(bool), typeof(Pane), new FrameworkPropertyMetadata(default(bool)));
		public static void SetIsVisible(DependencyObject element, bool value)
		{
			element.SetValue(IsVisibleProperty, value);
		}
		public static bool GetIsVisible(DependencyObject element)
		{
			return (bool)element.GetValue(IsVisibleProperty);
		}
	}
}
