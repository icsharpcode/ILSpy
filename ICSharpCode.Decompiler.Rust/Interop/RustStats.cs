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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ICSharpCode.Decompiler.Rust.Interop
{
	/// <summary>Mirrors <c>Stats</c> in rust/ilspy-decompiler-ffi/src/stats.rs.</summary>
	[StructLayout(LayoutKind.Sequential)]
	internal struct RustNativeStats
	{
		public ulong AllocatedBytes;
		public ulong Allocations;
		public ulong LiveBytes;
		public ulong PeakLiveBytes;
		public ulong RustElapsedNanoseconds;
		public ulong CallbackNanoseconds;
		public ulong Calls;
	}

	/// <summary>
	/// Process-wide performance counters of the engine, for measurement tools: the Rust heap as
	/// the engine's counting allocator sees it, the time spent inside the engine, and the time
	/// and bytes of the shim's requests to it. All values only grow, except the live and peak
	/// bytes; take differences of two snapshots to measure a stretch of work.
	/// </summary>
	public static class RustStats
	{
		static long requests, requestNanoseconds, requestBytes, responseBytes;

		/// <summary>The counters now. <paramref name="resetPeak"/> then restarts the peak of
		/// live Rust bytes at the bytes live now.</summary>
		public static RustStatsSnapshot Read(bool resetPeak = false)
		{
			NativeMethods.ilspy_stats(out var native, resetPeak ? (byte)1 : (byte)0);
			return new RustStatsSnapshot(native, Interlocked.Read(ref requests), Interlocked.Read(ref requestNanoseconds),
				Interlocked.Read(ref requestBytes), Interlocked.Read(ref responseBytes));
		}

		/// <summary>
		/// Times one request until disposed: everything the shim does for it, from encoding the
		/// request through the engine call to materializing the answer.
		/// </summary>
		internal static RequestTimer Time() => new RequestTimer(Stopwatch.GetTimestamp());

		internal static void Sent(long bytes) => Interlocked.Add(ref requestBytes, bytes);

		internal static void Received(long bytes) => Interlocked.Add(ref responseBytes, bytes);

		internal readonly struct RequestTimer(long start) : IDisposable
		{
			public void Dispose()
			{
				long ticks = Stopwatch.GetTimestamp() - start;
				Interlocked.Increment(ref requests);
				Interlocked.Add(ref requestNanoseconds, (long)(ticks * (1e9 / Stopwatch.Frequency)));
			}
		}
	}

	/// <summary>One reading of <see cref="RustStats"/>.</summary>
	public sealed class RustStatsSnapshot
	{
		internal RustStatsSnapshot(RustNativeStats native, long requests, long requestNanoseconds, long requestBytes, long responseBytes)
		{
			RustAllocatedBytes = (long)native.AllocatedBytes;
			RustAllocations = (long)native.Allocations;
			RustLiveBytes = (long)native.LiveBytes;
			RustPeakLiveBytes = (long)native.PeakLiveBytes;
			EngineNanoseconds = (long)(native.RustElapsedNanoseconds - native.CallbackNanoseconds);
			CallbackNanoseconds = (long)native.CallbackNanoseconds;
			EngineCalls = (long)native.Calls;
			Requests = requests;
			RequestNanoseconds = requestNanoseconds;
			RequestBytes = requestBytes;
			ResponseBytes = responseBytes;
		}

		/// <summary>Bytes the engine allocated.</summary>
		public long RustAllocatedBytes { get; }
		/// <summary>Allocations the engine made (a reallocation counts once).</summary>
		public long RustAllocations { get; }
		/// <summary>Bytes the engine holds now.</summary>
		public long RustLiveBytes { get; }
		/// <summary>The most bytes the engine held at once since the process started or the last peak reset.</summary>
		public long RustPeakLiveBytes { get; }
		/// <summary>Wall time inside the engine, its callbacks into the host excluded.</summary>
		public long EngineNanoseconds { get; }
		/// <summary>Wall time of the engine's callbacks into the host.</summary>
		public long CallbackNanoseconds { get; }
		/// <summary>Calls into the engine.</summary>
		public long EngineCalls { get; }
		/// <summary>Requests the shim made.</summary>
		public long Requests { get; }
		/// <summary>Wall time of those requests in the shim, engine and callbacks included.</summary>
		public long RequestNanoseconds { get; }
		/// <summary>Bytes the shim sent with its requests (syntax trees; the other requests send scalars).</summary>
		public long RequestBytes { get; }
		/// <summary>Bytes the engine answered with.</summary>
		public long ResponseBytes { get; }
		/// <summary>Request time spent neither in the engine nor in its callbacks: marshaling and materialization.</summary>
		public long MarshalingNanoseconds => RequestNanoseconds - EngineNanoseconds - CallbackNanoseconds;
	}
}
