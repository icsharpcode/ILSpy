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
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.ILSpy.AppEnv
{
	public sealed record ExceptionData(Exception Exception)
	{
		public string? PluginName { get; init; }
	}

	public static class StartupExceptions
	{
		public static IList<ExceptionData> Items { get; } = new List<ExceptionData>();

		public static string Format()
		{
			var sb = new StringBuilder();
			foreach (var item in Items)
			{
				if (!string.IsNullOrEmpty(item.PluginName))
					sb.AppendLine($"[Plugin: {item.PluginName}]");
				sb.AppendLine(item.Exception.ToString());
				sb.AppendLine();
			}
			return sb.ToString();
		}
	}
}