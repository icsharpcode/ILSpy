// Copyright (c) 2021 Tom Englert
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

namespace ICSharpCode.ILSpy.Themes
{
	public static class ResourceKeys
	{
		public static ResourceKey TextBackgroundBrush = new ComponentResourceKey(typeof(ResourceKeys), nameof(TextBackgroundBrush));
		public static ResourceKey TextForegroundBrush = new ComponentResourceKey(typeof(ResourceKeys), nameof(TextForegroundBrush));
		public static ResourceKey TextMarkerBackgroundColor = new ComponentResourceKey(typeof(ResourceKeys), nameof(TextMarkerBackgroundColor));
		public static ResourceKey TextMarkerDefinitionBackgroundColor = new ComponentResourceKey(typeof(ResourceKeys), nameof(TextMarkerDefinitionBackgroundColor));
		public static ResourceKey SearchResultBackgroundBrush = new ComponentResourceKey(typeof(ResourceKeys), nameof(SearchResultBackgroundBrush));
		public static ResourceKey LinkTextForegroundBrush = new ComponentResourceKey(typeof(ResourceKeys), nameof(LinkTextForegroundBrush));
		public static ResourceKey BracketHighlightBackgroundBrush = new ComponentResourceKey(typeof(ResourceKeys), nameof(BracketHighlightBackgroundBrush));
		public static ResourceKey BracketHighlightBorderPen = new ComponentResourceKey(typeof(ResourceKeys), nameof(BracketHighlightBorderPen));
		public static ResourceKey ThemeAwareButtonEffect = new ComponentResourceKey(typeof(ResourceKeys), nameof(ThemeAwareButtonEffect));
	}
}
