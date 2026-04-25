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

using Avalonia.Media;
using Avalonia.Svg.Skia;

namespace ILSpy.Images
{
	public static class Images
	{
		static IImage Load(string name)
		{
			return new SvgImage {
				Source = SvgSource.Load($"avares://ILSpy/Assets/Icons/{name}.svg", null)
			};
		}

		public static readonly IImage Assembly = Load(nameof(Assembly));
		public static readonly IImage AssemblyWarning = Load(nameof(AssemblyWarning));
		public static readonly IImage Namespace = Load(nameof(Namespace));
		public static readonly IImage Class = Load(nameof(Class));
		public static readonly IImage Interface = Load(nameof(Interface));
		public static readonly IImage Struct = Load(nameof(Struct));
		public static readonly IImage Enum = Load(nameof(Enum));
		public static readonly IImage Delegate = Load(nameof(Delegate));
		public static readonly IImage Method = Load(nameof(Method));
		public static readonly IImage Constructor = Load(nameof(Constructor));
		public static readonly IImage Operator = Load(nameof(Operator));
		public static readonly IImage Field = Load(nameof(Field));
		public static readonly IImage Property = Load(nameof(Property));
		public static readonly IImage Event = Load(nameof(Event));
	}
}
