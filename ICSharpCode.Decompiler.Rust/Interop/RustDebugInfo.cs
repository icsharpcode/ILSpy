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
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;

using ICSharpCode.Decompiler.DebugInfo;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>
	/// Serves an <see cref="IDebugInfoProvider"/> to the engine through <c>DebugInfoCallback</c>
	/// (rust/ilspy-decompiler-ffi/src/lib.rs). An exception thrown by the provider cannot cross
	/// the native frames: it is captured and rethrown by <see cref="ThrowIfFailed"/> once the
	/// engine call has returned.
	/// </summary>
	internal sealed class RustDebugInfo
	{
		const uint KindName = 0;
		const uint KindExtraTypeInfo = 1;

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal unsafe delegate IntPtr Callback(IntPtr context, uint methodToken, uint kind, int index, byte* buffer, UIntPtr capacity);

		readonly IDebugInfoProvider provider;
		ExceptionDispatchInfo? failure;

		/// <summary>The delegate to pass to the engine; keep this object alive for the call.</summary>
		public Callback Delegate { get; }

		public unsafe RustDebugInfo(IDebugInfoProvider provider)
		{
			this.provider = provider;
			Delegate = Answer;
		}

		unsafe IntPtr Answer(IntPtr context, uint methodToken, uint kind, int index, byte* buffer, UIntPtr capacity)
		{
			try
			{
				var method = (MethodDefinitionHandle)MetadataTokens.EntityHandle((int)methodToken);
				byte[]? answer = kind switch {
					KindName => provider.TryGetName(method, index, out string name) && name != null ? Encoding.UTF8.GetBytes(name) : null,
					KindExtraTypeInfo => provider.TryGetExtraTypeInfo(method, index, out var info) ? Encode(info) : null,
					_ => null,
				};
				if (answer == null)
					return (IntPtr)(-1);
				if ((ulong)answer.Length <= (ulong)capacity)
					Marshal.Copy(answer, 0, (IntPtr)buffer, answer.Length);
				return (IntPtr)answer.Length;
			}
			catch (Exception ex)
			{
				failure ??= ExceptionDispatchInfo.Capture(ex);
				return (IntPtr)(-1);
			}
		}

		/// <summary>The format <c>decode_extra_type_info</c> reads.</summary>
		internal static byte[] Encode(PdbExtraTypeInfo info)
		{
			var stream = new MemoryStream();
			using (var writer = new BinaryWriter(stream))
			{
				writer.Write(info.DynamicFlags?.Length ?? -1);
				foreach (bool flag in info.DynamicFlags ?? [])
					writer.Write(flag);
				writer.Write(info.TupleElementNames?.Length ?? -1);
				foreach (string? name in info.TupleElementNames ?? [])
				{
					if (name == null)
					{
						writer.Write(-1);
						continue;
					}
					byte[] bytes = Encoding.UTF8.GetBytes(name);
					writer.Write(bytes.Length);
					writer.Write(bytes);
				}
			}
			return stream.ToArray();
		}

		/// <summary>Rethrows the first exception the provider threw during the engine call.</summary>
		public void ThrowIfFailed() => failure?.Throw();
	}
}
