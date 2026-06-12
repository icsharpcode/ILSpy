// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Vendored from dotnet/runtime, src/libraries/Common/src/System/Sha1ForNonSecretPurposes.cs
// (commit 6e51f762bc4c98ea90ae6ca21c4e220b4b2e7a5c). The only changes are the "unchecked" blocks
// in Finish and Drain: upstream compiles without overflow checks, while this project sets
// CheckForOverflowUnderflow, and SHA-1 relies on wrapping 32-bit arithmetic.
//
// Strong-name public-key tokens are defined by ECMA-335 as a SHA-1 digest, so this managed
// implementation is used instead of System.Security.Cryptography.SHA1 to keep reading assembly
// metadata independent of the host crypto policy (FIPS-mode systems refuse to create platform
// SHA-1 instances).

using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;

namespace System
{
    /// <summary>
    /// Implements the SHA1 hashing algorithm. Note that
    /// implementation is for hashing public information. Do not
    /// use code to hash private data, as implementation does
    /// not take any steps to avoid information disclosure.
    /// </summary>
    internal struct Sha1ForNonSecretPurposes
    {
        private long _length; // Total message length in bits
        private uint[] _w; // Workspace
        private int _pos; // Length of current chunk in bytes

        /// <summary>
        /// Computes the SHA1 hash of the provided data.
        /// </summary>
        /// <param name="source">The data to hash.</param>
        /// <param name="destination">The buffer to receive the hash value.</param>
        public static unsafe void HashData(ReadOnlySpan<byte> source, Span<byte> destination)
        {
            Debug.Assert(destination.Length == 20);

            Span<uint> w = stackalloc uint[85];

            Start(w);

            int originalLength = source.Length;

            while (source.Length >= 64)
            {
                for (int i = 0; i < 16; i++)
                {
                    w[i] = BinaryPrimitives.ReadUInt32BigEndian(source);
                    source = source.Slice(4);
                }
                Drain(w);
            }

            Span<byte> tail = stackalloc byte[2 * 64];
            source.CopyTo(tail);
            int pos = source.Length;
            tail[pos++] = 0x80;
            while ((pos & 63) != 56)
            {
                tail[pos++] = 0x00;
            }
            BinaryPrimitives.WriteUInt64BigEndian(tail.Slice(pos), (ulong)originalLength * 8);
            tail = tail.Slice(0, pos + 8);

            while (tail.Length > 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    w[i] = BinaryPrimitives.ReadUInt32BigEndian(tail);
                    tail = tail.Slice(4);
                }
                Drain(w);
            }

            for (int i = 80; i < w.Length; i++)
            {
                BinaryPrimitives.WriteUInt32BigEndian(destination, w[i]);
                destination = destination.Slice(4);
            }
        }

        /// <summary>
        /// Call Start() to initialize the hash object.
        /// </summary>
        public void Start()
        {
            Start(_w ??= new uint[85]);

            _length = 0;
            _pos = 0;
        }

        private static void Start(Span<uint> w)
        {
            w[80] = 0x67452301;
            w[81] = 0xEFCDAB89;
            w[82] = 0x98BADCFE;
            w[83] = 0x10325476;
            w[84] = 0xC3D2E1F0;
        }

        /// <summary>
        /// Adds an input byte to the hash.
        /// </summary>
        /// <param name="input">Data to include in the hash.</param>
        public void Append(byte input)
        {
            int idx = _pos >> 2;
            _w[idx] = (_w[idx] << 8) | input;
            if (64 == ++_pos)
            {
                Drain();
            }
        }

        /// <summary>
        /// Adds input bytes to the hash.
        /// </summary>
        /// <param name="input">
        /// Data to include in the hash. Must not be null.
        /// </param>
        public void Append(ReadOnlySpan<byte> input)
        {
            foreach (byte b in input)
            {
                Append(b);
            }
        }

        /// <summary>
        /// Retrieves the hash value.
        /// Note that after calling function, the hash object should
        /// be considered uninitialized. Subsequent calls to Append or
        /// Finish will produce useless results. Call Start() to
        /// reinitialize.
        /// </summary>
        /// <param name="output">
        /// Buffer to receive the hash value. Must not be null.
        /// Up to 20 bytes of hash will be written to the output buffer.
        /// If the buffer is smaller than 20 bytes, the remaining hash
        /// bytes will be lost. If the buffer is larger than 20 bytes, the
        /// rest of the buffer is left unmodified.
        /// </param>
        public void Finish(Span<byte> output)
        {
            Debug.Assert(output.Length == 20);

            long l = _length + 8 * _pos;
            Append(0x80);
            while (_pos != 56)
            {
                Append(0x00);
            }

            unchecked
            {
                Append((byte)(l >> 56));
                Append((byte)(l >> 48));
                Append((byte)(l >> 40));
                Append((byte)(l >> 32));
                Append((byte)(l >> 24));
                Append((byte)(l >> 16));
                Append((byte)(l >> 8));
                Append((byte)l);
            }

            for (int i = 80; i < _w.Length; i++)
            {
                BinaryPrimitives.WriteUInt32BigEndian(output, _w[i]);
                output = output.Slice(4);
            }
        }

        /// <summary>
        /// Called when pos reaches 64.
        /// </summary>
        private void Drain()
        {
            Drain(_w);
            _length += 512; // 64 bytes == 512 bits
            _pos = 0;
        }

        private static void Drain(Span<uint> w)
        {
            unchecked
            {
                var _ = w[84]; // Hint to eliminate bounds checks

                for (int i = 16; i < 80; i++)
                {
                    w[i] = BitOperations.RotateLeft(w[i - 3] ^ w[i - 8] ^ w[i - 14] ^ w[i - 16], 1);
                }

                uint a = w[80];
                uint b = w[81];
                uint c = w[82];
                uint d = w[83];
                uint e = w[84];

                for (int i = 0; i < 20; i++)
                {
                    const uint k = 0x5A827999;
                    uint f = (b & c) | ((~b) & d);
                    uint temp = BitOperations.RotateLeft(a, 5) + f + e + k + w[i]; e = d; d = c; c = BitOperations.RotateLeft(b, 30); b = a; a = temp;
                }

                for (int i = 20; i < 40; i++)
                {
                    uint f = b ^ c ^ d;
                    const uint k = 0x6ED9EBA1;
                    uint temp = BitOperations.RotateLeft(a, 5) + f + e + k + w[i]; e = d; d = c; c = BitOperations.RotateLeft(b, 30); b = a; a = temp;
                }

                for (int i = 40; i < 60; i++)
                {
                    uint f = (b & c) | (b & d) | (c & d);
                    const uint k = 0x8F1BBCDC;
                    uint temp = BitOperations.RotateLeft(a, 5) + f + e + k + w[i]; e = d; d = c; c = BitOperations.RotateLeft(b, 30); b = a; a = temp;
                }

                for (int i = 60; i < 80; i++)
                {
                    uint f = b ^ c ^ d;
                    const uint k = 0xCA62C1D6;
                    uint temp = BitOperations.RotateLeft(a, 5) + f + e + k + w[i]; e = d; d = c; c = BitOperations.RotateLeft(b, 30); b = a; a = temp;
                }

                w[80] += a;
                w[81] += b;
                w[82] += c;
                w[83] += d;
                w[84] += e;
            }
        }
    }
}
