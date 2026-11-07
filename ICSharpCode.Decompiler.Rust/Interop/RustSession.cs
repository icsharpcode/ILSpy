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

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// Owns one engine session; the handle is freed exactly once, also on finalization.
	/// </summary>
	internal sealed class RustSession : SafeHandle
	{
		RustSession() : base(IntPtr.Zero, ownsHandle: true)
		{
		}

		public override bool IsInvalid => handle == IntPtr.Zero;

		/// <summary>Creates a session or throws with the engine's error message.</summary>
		public static RustSession Create()
		{
			var session = NativeMethods.ilspy_session_new(IntPtr.Zero);
			if (session.IsInvalid)
				throw new InvalidOperationException("ilspy_session_new failed: " + (NativeMethods.LastError ?? "no error reported"));
			return session;
		}

		protected override bool ReleaseHandle()
		{
			NativeMethods.ilspy_session_free(handle);
			return true;
		}
	}
}
