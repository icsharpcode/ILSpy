// Copyright (c) 2024 Holger Schmidt
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
using System.Linq;
using System.Text.RegularExpressions;

namespace ICSharpCode.ILSpyX.MermaidDiagrammer.Extensions
{
	internal static class StringExtensions
	{
		/// <summary>Replaces all consecutive horizontal white space characters in
		/// <paramref name="input"/> with <paramref name="normalizeTo"/> while leaving line breaks intact.</summary>
		internal static string NormalizeHorizontalWhiteSpace(this string input, string normalizeTo = " ")
			=> Regex.Replace(input, @"[ \t]+", normalizeTo);

		/// <summary>Replaces all occurrences of <paramref name="oldValues"/> in
		/// <paramref name="input"/> with <paramref name="newValue"/>.</summary>
		internal static string ReplaceAll(this string input, IEnumerable<string> oldValues, string? newValue)
			=> oldValues.Aggregate(input, (aggregate, oldValue) => aggregate.Replace(oldValue, newValue));

		/// <summary>Joins the specified <paramref name="strings"/> to a single one
		/// using the specified <paramref name="separator"/> as a delimiter.</summary>
		/// <param name="pad">Whether to pad the start and end of the string with the <paramref name="separator"/> as well.</param>
		internal static string Join(this IEnumerable<string?>? strings, string separator, bool pad = false)
		{
			if (strings == null)
				return string.Empty;

			var joined = string.Join(separator, strings);
			return pad ? string.Concat(separator, joined, separator) : joined;
		}

		/// <summary>Formats all items in <paramref name="collection"/> using the supplied <paramref name="format"/> strategy
		/// and returns a string collection - even if the incoming <paramref name="collection"/> is null.</summary>
		internal static IEnumerable<string> FormatAll<T>(this IEnumerable<T>? collection, Func<T, string> format)
			=> collection?.Select(format) ?? [];
	}
}