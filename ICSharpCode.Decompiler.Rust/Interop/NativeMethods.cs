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
using System.Runtime.InteropServices;
using System.Text;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// The C ABI exported by rust/ilspy-decompiler-ffi. Keep in sync with its src/lib.rs.
	/// </summary>
	internal static class NativeMethods
	{
		const string Library = "ilspy_decompiler_ffi";

		[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr ilspy_version();

		[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
		internal static extern RustSession ilspy_session_new(IntPtr ctx);

		[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ilspy_session_free(IntPtr session);

		[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr ilspy_last_error();

		/// <summary>Decodes a NUL-terminated UTF-8 string owned by the engine.</summary>
		internal static unsafe string? FromUtf8(IntPtr p)
		{
			if (p == IntPtr.Zero)
				return null;
			byte* bytes = (byte*)p;
			int length = 0;
			while (bytes[length] != 0)
				length++;
			return Encoding.UTF8.GetString(bytes, length);
		}

		/// <summary>The engine version, as declared in rust/Cargo.toml.</summary>
		internal static string Version => FromUtf8(ilspy_version())!;

		/// <summary>The last error the engine reported on the calling thread.</summary>
		internal static string? LastError => FromUtf8(ilspy_last_error());
	}
}
