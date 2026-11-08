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
	/// <summary>Mirrors <c>Status</c> in rust/ilspy-decompiler-ffi/src/lib.rs.</summary>
	internal enum RustStatus
	{
		Ok = 0,
		Unsupported = 1,
		Cancelled = 2,
		Error = 3,
		Panic = 4,
	}

	/// <summary>Mirrors <c>Buf</c> in rust/ilspy-decompiler-ffi/src/lib.rs.</summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct RustBuffer
	{
		public IntPtr Pointer;
		public UIntPtr Length;
		public UIntPtr Capacity;
	}

	/// <summary>Mirrors <c>FfiModule</c> in rust/ilspy-decompiler-ffi/src/lib.rs.</summary>
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct RustModule
	{
		public byte* Image;
		public UIntPtr ImageLength;
		public byte* AssemblyName;
		public UIntPtr AssemblyNameLength;
		public int* AssemblyRefs;
		public UIntPtr AssemblyRefCount;
		public int* ModuleRefs;
		public UIntPtr ModuleRefCount;
	}

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

		[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
		internal static extern void ilspy_buf_free(RustBuffer buffer);

		[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
		static extern unsafe RustStatus ilspy_debug_list_members(byte* image, UIntPtr length, out RustBuffer listing);

		[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
		internal static extern unsafe RustStatus ilspy_debug_type_listing(RustModule* modules, UIntPtr count, uint options, out RustBuffer listing);

		[DllImport(Library, CallingConvention = CallingConvention.Cdecl)]
		internal static extern unsafe RustStatus ilspy_debug_read_il(RustModule* modules, UIntPtr count, uint options, uint readerFlags, out RustBuffer wire);

		/// <summary>Copies an engine buffer into managed memory and frees it.</summary>
		internal static unsafe byte[] TakeBuffer(RustBuffer buffer)
		{
			try
			{
				var bytes = new byte[checked((int)(ulong)buffer.Length)];
				if (bytes.Length > 0)
					Marshal.Copy(buffer.Pointer, bytes, 0, bytes.Length);
				return bytes;
			}
			finally
			{
				ilspy_buf_free(buffer);
			}
		}

		/// <summary>
		/// The engine's canonical metadata listing of a PE image (rust/ilspy-decompiler/src/metadata/listing.rs).
		/// Test support only.
		/// </summary>
		internal static unsafe string DebugListMembers(byte[] image)
		{
			RustStatus status;
			RustBuffer buffer;
			fixed (byte* p = image)
				status = ilspy_debug_list_members(p, (UIntPtr)image.Length, out buffer);
			byte[] bytes = TakeBuffer(buffer);
			if (status != RustStatus.Ok)
				throw new InvalidOperationException($"ilspy_debug_list_members: {status}: {LastError}");
			return Encoding.UTF8.GetString(bytes);
		}

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
