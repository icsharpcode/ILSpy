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

#if DEBUG

using System;
using System.Collections.Generic;
using System.Globalization;

using Avalonia.Data.Converters;

using ICSharpCode.Decompiler.DebugSteps;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// Decides whether a Debug Steps tree row stays visible under the pane's filter box. A step is
	/// shown when the filter is empty, or when its description -- or that of any descendant --
	/// contains the filter text, so the path to every match is preserved. Bound per row against
	/// [ the step, the filter text ].
	/// </summary>
	public sealed class DebugStepFilterConverter : IMultiValueConverter
	{
		public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
		{
			if (values.Count < 2 || values[1] is not string filter || string.IsNullOrWhiteSpace(filter))
				return true;
			return values[0] is Stepper.Node node && Matches(node, filter.Trim());
		}

		static bool Matches(Stepper.Node node, string filter)
		{
			if (node.Description.Contains(filter, StringComparison.OrdinalIgnoreCase))
				return true;
			foreach (var child in node.Children)
			{
				if (Matches(child, filter))
					return true;
			}
			return false;
		}
	}
}

#endif
