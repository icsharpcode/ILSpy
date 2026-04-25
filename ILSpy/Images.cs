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

using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Svg.Skia;

namespace ILSpy.Images
{
	public static class Images
	{
		const string AssetBase = "avares://ILSpy/Assets/Icons/";

		static IImage LoadSvg(string name)
		{
			return new SvgImage {
				Source = SvgSource.Load(AssetBase + name + ".svg", null)
			};
		}

		static IImage LoadPng(string name)
		{
			using var stream = AssetLoader.Open(new Uri(AssetBase + name + ".png"));
			return new Bitmap(stream);
		}

		// Assemblies / packages
		public static readonly IImage Assembly = LoadSvg(nameof(Assembly));
		public static readonly IImage AssemblyLoading = LoadSvg(nameof(AssemblyLoading));
		public static readonly IImage AssemblyWarning = LoadSvg(nameof(AssemblyWarning));
		public static readonly IImage FindAssembly = LoadSvg(nameof(FindAssembly));
		public static readonly IImage Library = LoadSvg(nameof(Library));
		public static readonly IImage NuGet = LoadPng(nameof(NuGet));
		public static readonly IImage MetadataFile = LoadSvg(nameof(MetadataFile));
		public static readonly IImage WebAssemblyFile = LoadSvg("WebAssembly");
		public static readonly IImage ProgramDebugDatabase = LoadSvg(nameof(ProgramDebugDatabase));

		// Containers
		public static readonly IImage Namespace = LoadSvg(nameof(Namespace));

		// Types
		public static readonly IImage Class = LoadSvg(nameof(Class));
		public static readonly IImage Interface = LoadSvg(nameof(Interface));
		public static readonly IImage Struct = LoadSvg(nameof(Struct));
		public static readonly IImage Enum = LoadSvg(nameof(Enum));
		public static readonly IImage Delegate = LoadSvg(nameof(Delegate));

		// Methods
		public static readonly IImage Method = LoadSvg(nameof(Method));
		public static readonly IImage Constructor = LoadSvg(nameof(Constructor));
		public static readonly IImage Operator = LoadSvg(nameof(Operator));

		// Fields / properties / events
		public static readonly IImage Field = LoadSvg(nameof(Field));
		public static readonly IImage Property = LoadSvg(nameof(Property));
		public static readonly IImage Event = LoadSvg(nameof(Event));
	}
}
