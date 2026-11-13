// Copyright (c) 2026 Christoph Wille
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

#nullable enable

using System;
using System.Linq;
using System.Reflection;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The <see cref="DecompilerSettings"/> the engine reads, as the bit set of
	/// <c>ilspy_session_read_function</c>. The engine names the properties
	/// (<c>ilspy_setting_names</c>); a name without a boolean property is an error, so a
	/// renamed setting cannot be silently ignored.
	/// </summary>
	internal static class RustSettings
	{
		static readonly PropertyInfo[] properties = (NativeMethods.FromUtf8(NativeMethods.ilspy_setting_names()) ?? "")
			.Split(['\n'], StringSplitOptions.RemoveEmptyEntries)
			.Select(name => typeof(DecompilerSettings).GetProperty(name) is { PropertyType: var t } p && t == typeof(bool)
				? p
				: throw new InvalidOperationException($"The engine reads the setting '{name}', which is no boolean DecompilerSettings property"))
			.ToArray();

		/// <summary>The setting names, in bit order.</summary>
		public static string[] Names => properties.Select(p => p.Name).ToArray();

		/// <summary>The settings as two words: bits 0 to 63, and the following ones.</summary>
		public static (ulong Low, ulong High) Bits(DecompilerSettings settings)
		{
			if (properties.Length > 128)
				throw new InvalidOperationException("The engine reads more settings than two words hold");
			ulong low = 0, high = 0;
			for (int i = 0; i < properties.Length; i++)
			{
				if (!(bool)properties[i].GetValue(settings)!)
					continue;
				if (i < 64)
					low |= 1UL << i;
				else
					high |= 1UL << (i - 64);
			}
			return (low, high);
		}
	}
}
