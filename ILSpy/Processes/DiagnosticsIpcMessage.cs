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

using System;
using System.Buffers.Binary;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.Processes
{
	/// <summary>
	/// Wire format of the CoreCLR diagnostics IPC protocol (dotnet/diagnostics,
	/// documentation/design-docs/ipc-protocol.md): a 20-byte header - 14-byte null-terminated
	/// ASCII magic "DOTNET_IPC_V1", u16 total message size, u8 command set, u8 command id,
	/// u16 reserved - followed by a command-specific payload. Strings are UTF-16LE with a u32
	/// char-count prefix that includes the null terminator. All integers are little endian.
	/// </summary>
	internal static class DiagnosticsIpcMessage
	{
		public const int HeaderSize = 20;

		const byte ServerCommandSet = 0xFF;
		const byte ServerOkCommandId = 0x00;

		// A string longer than this is not a plausible command line or version string; treat
		// it as a corrupt response rather than allocating unbounded memory.
		const int MaxStringLength = 1024 * 1024;

		static ReadOnlySpan<byte> Magic => "DOTNET_IPC_V1\0"u8;

		public static byte[] EncodeRequest(byte commandSet, byte commandId, ReadOnlySpan<byte> payload)
		{
			var message = new byte[HeaderSize + payload.Length];
			Magic.CopyTo(message);
			BinaryPrimitives.WriteUInt16LittleEndian(message.AsSpan(14), checked((ushort)(HeaderSize + payload.Length)));
			message[16] = commandSet;
			message[17] = commandId;
			// Bytes 18-19 are the reserved field and stay zero.
			payload.CopyTo(message.AsSpan(HeaderSize));
			return message;
		}

		public static void WriteString(BinaryWriter writer, string? value)
		{
			if (value == null)
			{
				writer.Write(0u);
				return;
			}
			writer.Write((uint)(value.Length + 1));
			foreach (char c in value)
				writer.Write((ushort)c);
			writer.Write((ushort)0);
		}

		public static string? ReadString(BinaryReader reader)
		{
			uint length = reader.ReadUInt32();
			if (length == 0)
				return null;
			if (length > MaxStringLength)
				throw new IOException($"Diagnostics IPC string length {length} exceeds the sanity limit.");
			var chars = new char[length];
			for (int i = 0; i < chars.Length; i++)
				chars[i] = (char)reader.ReadUInt16();
			int end = chars.Length;
			if (end > 0 && chars[end - 1] == '\0')
				end--;
			return new string(chars, 0, end);
		}

		/// <summary>
		/// Reads one response message and returns its payload. A success response's payload is
		/// command-specific; an error response carries an HRESULT and is surfaced as an
		/// <see cref="IOException"/>. For streaming commands (EventPipe), any data following
		/// the sized message remains in the stream for the caller.
		/// </summary>
		public static async Task<byte[]> ReadResponseAsync(Stream stream, CancellationToken cancellationToken)
		{
			var header = new byte[HeaderSize];
			await stream.ReadExactlyAsync(header, cancellationToken).ConfigureAwait(false);
			if (!header.AsSpan(0, Magic.Length).SequenceEqual(Magic))
				throw new IOException("Diagnostics IPC response does not start with the DOTNET_IPC_V1 magic.");

			ushort size = BinaryPrimitives.ReadUInt16LittleEndian(header.AsSpan(14));
			if (size < HeaderSize)
				throw new IOException($"Diagnostics IPC response declares an impossible size of {size} bytes.");
			var payload = new byte[size - HeaderSize];
			await stream.ReadExactlyAsync(payload, cancellationToken).ConfigureAwait(false);

			if (header[16] != ServerCommandSet || header[17] != ServerOkCommandId)
			{
				int hresult = payload.Length >= 4 ? BinaryPrimitives.ReadInt32LittleEndian(payload) : 0;
				throw new IOException($"The target runtime rejected the command (error 0x{hresult:X8}).");
			}
			return payload;
		}
	}
}
