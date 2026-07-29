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
using System.IO.Pipes;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.Processes
{
	/// <summary>
	/// A CoreCLR runtime's answer to a diagnostics process-info query. Entry assembly and
	/// CLR version are only served by runtimes that understand ProcessInfo2 (.NET 6+); for
	/// older runtimes they are null and the rest comes from the ProcessInfo fallback.
	/// </summary>
	internal sealed record DotNetProcessInfo(
		long Pid,
		Guid RuntimeCookie,
		string? CommandLine,
		string? OperatingSystemName,
		string? Architecture,
		string? EntryAssemblyName,
		string? ClrVersion);

	/// <summary>
	/// Speaks the diagnostics IPC protocol to a single CoreCLR process, over the transport
	/// found by <see cref="DiagnosticsPortScanner"/>. Every call opens a fresh connection
	/// (the protocol is one-command-per-connection) and is bounded by a timeout so a hung
	/// target cannot stall the caller.
	/// </summary>
	internal static class DiagnosticsIpcClient
	{
		const byte ProcessCommandSet = 0x04;
		const byte ProcessInfoCommandId = 0x00;
		const byte ProcessInfo2CommandId = 0x04;

		const byte EventPipeCommandSet = 0x02;
		const byte StopTracingCommandId = 0x01;
		const byte CollectTracing2CommandId = 0x03;
		const byte CollectTracing4CommandId = 0x05;

		// Which rundown events the runtime emits when the session stops. The runtime's own
		// default (0x80020139) additionally asks for the JIT, NGen and IL-to-native-map
		// rundown, which in a process that has been running for a while outweighs the module
		// list by orders of magnitude - enough to overrun the session buffer and cost the
		// loader events this feature exists to read. These bits ask for the loader rundown
		// and the stop-time ("end") emission of it, and nothing else.
		const ulong LoaderRundownKeyword = 0x80000108;

		const string RuntimeProviderName = "Microsoft-Windows-DotNETRuntime";
		const ulong LoaderKeyword = 0x8;
		const uint InformationalLevel = 4;
		const uint NetTraceFormat = 1;
		// The session only ever carries loader events plus the rundown, so a small buffer is
		// ample; oversizing it would make the target runtime reserve memory for nothing.
		const uint CircularBufferMB = 16;

		static readonly TimeSpan CommandTimeout = TimeSpan.FromSeconds(2);
		// A rundown walks every loaded module and assembly, so it takes measurably longer
		// than a status query - and longer still on a large process.
		static readonly TimeSpan RundownTimeout = TimeSpan.FromSeconds(30);

		public static async Task<DotNetProcessInfo> GetProcessInfoAsync(int pid, CancellationToken cancellationToken)
		{
			try
			{
				return await QueryProcessInfoAsync(pid, ProcessInfo2CommandId, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception ex) when (ex is IOException or EndOfStreamException)
			{
				// ProcessInfo2 needs .NET 6+; a .NET Core 3.x/5 runtime answers with an
				// unknown-command error. The original ProcessInfo works from 3.0 on.
				return await QueryProcessInfoAsync(pid, ProcessInfoCommandId, cancellationToken).ConfigureAwait(false);
			}
		}

		static async Task<DotNetProcessInfo> QueryProcessInfoAsync(int pid, byte commandId, CancellationToken cancellationToken)
		{
			using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			timeout.CancelAfter(CommandTimeout);

			Stream stream = await ConnectAsync(pid, timeout.Token).ConfigureAwait(false);
			await using (stream.ConfigureAwait(false))
			{
				byte[] request = DiagnosticsIpcMessage.EncodeRequest(ProcessCommandSet, commandId, ReadOnlySpan<byte>.Empty);
				await stream.WriteAsync(request, timeout.Token).ConfigureAwait(false);
				byte[] payload = await DiagnosticsIpcMessage.ReadResponseAsync(stream, timeout.Token).ConfigureAwait(false);

				using var reader = new BinaryReader(new MemoryStream(payload));
				long reportedPid = reader.ReadInt64();
				var runtimeCookie = new Guid(reader.ReadBytes(16));
				string? commandLine = DiagnosticsIpcMessage.ReadString(reader);
				string? operatingSystem = DiagnosticsIpcMessage.ReadString(reader);
				string? architecture = DiagnosticsIpcMessage.ReadString(reader);
				string? entryAssembly = null;
				string? clrVersion = null;
				if (commandId == ProcessInfo2CommandId)
				{
					entryAssembly = DiagnosticsIpcMessage.ReadString(reader);
					clrVersion = DiagnosticsIpcMessage.ReadString(reader);
				}
				return new DotNetProcessInfo(reportedPid, runtimeCookie, commandLine,
					operatingSystem, architecture, entryAssembly, clrVersion);
			}
		}

		/// <summary>
		/// Runs a minimal EventPipe session against <paramref name="pid"/> purely to obtain
		/// its rundown: the runtime emits one event per loaded module and assembly when the
		/// session stops. The returned stream is the raw nettrace container, positioned at 0.
		/// </summary>
		public static async Task<MemoryStream> CollectModuleRundownAsync(int pid, CancellationToken cancellationToken)
		{
			try
			{
				return await CollectAsync(pid, CollectTracing4CommandId, cancellationToken).ConfigureAwait(false);
			}
			catch (Exception ex) when (ex is IOException or EndOfStreamException)
			{
				// Selecting the rundown events by keyword needs a runtime that knows
				// CollectTracing4; an older one answers with an unknown-command error and
				// only offers the all-or-nothing rundown.
				return await CollectAsync(pid, CollectTracing2CommandId, cancellationToken).ConfigureAwait(false);
			}
		}

		static async Task<MemoryStream> CollectAsync(int pid, byte commandId, CancellationToken cancellationToken)
		{
			using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
			timeout.CancelAfter(RundownTimeout);

			Stream session = await ConnectAsync(pid, timeout.Token).ConfigureAwait(false);
			await using (session.ConfigureAwait(false))
			{
				byte[] request = DiagnosticsIpcMessage.EncodeRequest(
					EventPipeCommandSet, commandId, BuildCollectTracingPayload(commandId));
				await session.WriteAsync(request, timeout.Token).ConfigureAwait(false);

				byte[] response = await DiagnosticsIpcMessage.ReadResponseAsync(session, timeout.Token).ConfigureAwait(false);
				if (response.Length < sizeof(ulong))
					throw new IOException("The runtime did not return an EventPipe session id.");
				ulong sessionId = BinaryPrimitives.ReadUInt64LittleEndian(response);

				// The nettrace stream must be drained while the session is stopped: the
				// runtime writes the rundown into the same connection, and a full buffer
				// would block the stop from completing.
				var trace = new MemoryStream();
				Task drain = session.CopyToAsync(trace, timeout.Token);
				try
				{
					await StopTracingAsync(pid, sessionId, timeout.Token).ConfigureAwait(false);
					await drain.ConfigureAwait(false);
				}
				catch
				{
					await trace.DisposeAsync().ConfigureAwait(false);
					throw;
				}
				trace.Position = 0;
				return trace;
			}
		}

		static async Task StopTracingAsync(int pid, ulong sessionId, CancellationToken cancellationToken)
		{
			Stream stream = await ConnectAsync(pid, cancellationToken).ConfigureAwait(false);
			await using (stream.ConfigureAwait(false))
			{
				var payload = new byte[sizeof(ulong)];
				BinaryPrimitives.WriteUInt64LittleEndian(payload, sessionId);
				byte[] request = DiagnosticsIpcMessage.EncodeRequest(EventPipeCommandSet, StopTracingCommandId, payload);
				await stream.WriteAsync(request, cancellationToken).ConfigureAwait(false);
				await DiagnosticsIpcMessage.ReadResponseAsync(stream, cancellationToken).ConfigureAwait(false);
			}
		}

		static byte[] BuildCollectTracingPayload(byte commandId)
		{
			using var buffer = new MemoryStream();
			using (var writer = new BinaryWriter(buffer, Encoding.Unicode, leaveOpen: true))
			{
				writer.Write(CircularBufferMB);
				writer.Write(NetTraceFormat);
				// Rundown is what makes this a snapshot of everything already loaded rather
				// than a recording of what loads from now on.
				if (commandId == CollectTracing4CommandId)
				{
					writer.Write(LoaderRundownKeyword);
					writer.Write(false); // no stack walks: the call stacks are pure overhead here
				}
				else
				{
					writer.Write(true);
				}
				writer.Write(1u); // one provider follows
				writer.Write(LoaderKeyword);
				writer.Write(InformationalLevel);
				DiagnosticsIpcMessage.WriteString(writer, RuntimeProviderName);
				DiagnosticsIpcMessage.WriteString(writer, null); // no filter data
			}
			return buffer.ToArray();
		}

		internal static async Task<Stream> ConnectAsync(int pid, CancellationToken cancellationToken)
		{
			if (OperatingSystem.IsWindows())
			{
				var pipe = new NamedPipeClientStream(".", "dotnet-diagnostic-" + pid,
					PipeDirection.InOut, PipeOptions.Asynchronous);
				try
				{
					await pipe.ConnectAsync(cancellationToken).ConfigureAwait(false);
					return pipe;
				}
				catch
				{
					await pipe.DisposeAsync().ConfigureAwait(false);
					throw;
				}
			}

			string socketPath = DiagnosticsPortScanner.GetUnixSocketPath(pid)
				?? throw new IOException($"Process {pid} exposes no diagnostics socket.");
			var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
			try
			{
				await socket.ConnectAsync(new UnixDomainSocketEndPoint(socketPath), cancellationToken).ConfigureAwait(false);
				return new NetworkStream(socket, ownsSocket: true);
			}
			catch
			{
				socket.Dispose();
				throw;
			}
		}
	}
}
