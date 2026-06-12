// Copyright (c) 2026 Siegfried Pammer
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
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Util
{
	[TestFixture]
	public class Sha1ForNonSecretPurposesTests
	{
		static string OneShotHash(byte[] input)
		{
			byte[] output = new byte[20];
			Sha1ForNonSecretPurposes.HashData(input, output);
			return ToHex(output);
		}

		static string IncrementalHash(byte[] input)
		{
			var sha1 = default(Sha1ForNonSecretPurposes);
			sha1.Start();
			sha1.Append(input);
			byte[] output = new byte[20];
			sha1.Finish(output);
			return ToHex(output);
		}

		static string ToHex(byte[] bytes)
		{
			return string.Concat(bytes.Select(b => b.ToString("x2")));
		}

		// FIPS 180 test vectors
		[TestCase("", "da39a3ee5e6b4b0d3255bfef95601890afd80709")]
		[TestCase("abc", "a9993e364706816aba3e25717850c26c9cd0d89d")]
		[TestCase("abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq", "84983e441c3bd26ebaae4aa1f95129e5e54670f1")]
		public void KnownVectors(string input, string expectedHex)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(input);
			Assert.That(OneShotHash(bytes), Is.EqualTo(expectedHex));
			Assert.That(IncrementalHash(bytes), Is.EqualTo(expectedHex));
		}

		[Test]
		public void MatchesPlatformSha1AroundBlockBoundaries()
		{
			// Cover all interesting positions relative to the 64-byte block size and the
			// 8-byte length suffix: 0, 1, 55, 56, 57, 63, 64, 65, 119, 120, 127, 128, ...
			using var platformSha1 = System.Security.Cryptography.SHA1.Create();
			for (int length = 0; length <= 130; length++)
			{
				byte[] input = new byte[length];
				for (int i = 0; i < length; i++)
				{
					input[i] = unchecked((byte)(i * 131 + 7));
				}
				string expected = ToHex(platformSha1.ComputeHash(input));
				Assert.That(OneShotHash(input), Is.EqualTo(expected), $"one-shot, length {length}");
				Assert.That(IncrementalHash(input), Is.EqualTo(expected), $"incremental, length {length}");
			}
		}

		[Test]
		public void StartResetsTheIncrementalState()
		{
			var sha1 = default(Sha1ForNonSecretPurposes);
			sha1.Start();
			sha1.Append(Encoding.ASCII.GetBytes("garbage that must not leak into the second hash"));
			byte[] output = new byte[20];
			sha1.Finish(output);

			sha1.Start();
			sha1.Append(Encoding.ASCII.GetBytes("abc"));
			sha1.Finish(output);
			Assert.That(ToHex(output), Is.EqualTo("a9993e364706816aba3e25717850c26c9cd0d89d"));
		}

		[Test]
		public void EcmaStandardPublicKeyYieldsKnownToken()
		{
			// The strong-name public key token is defined as the last 8 bytes of the
			// SHA-1 of the public key, in reversed order. The ECMA standard public key
			// must produce the well-known token b77a5c561934e089 (mscorlib et al.).
			byte[] ecmaKey = new byte[16];
			ecmaKey[8] = 0x04;
			byte[] hash = new byte[20];
			Sha1ForNonSecretPurposes.HashData(ecmaKey, hash);
			string token = ToHex(hash.Skip(12).Reverse().ToArray());
			Assert.That(token, Is.EqualTo("b77a5c561934e089"));
		}
	}
}
