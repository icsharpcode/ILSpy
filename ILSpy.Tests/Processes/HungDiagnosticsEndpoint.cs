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
using System.IO;
using System.IO.Pipes;
using System.Net.Sockets;

namespace ICSharpCode.ILSpy.Tests.Processes;

/// <summary>
/// A diagnostics endpoint that accepts a connection and then says nothing, standing at the
/// transport name a CoreCLR process of the given pid would use. It is how a suspended or
/// wedged runtime behaves, and the only way to make the client's command budget expire
/// without waiting out the production one.
/// </summary>
/// <remarks>
/// Both transports are created unconditionally rather than under <c>#if</c>, matching how the
/// production scanner keeps its Windows and unix halves in one always-compiled type; only the
/// one this OS uses is actually opened.
/// </remarks>
sealed class HungDiagnosticsEndpoint : IDisposable
{
	readonly NamedPipeServerStream? pipe;
	readonly Socket? socket;
	readonly string? socketPath;

	public HungDiagnosticsEndpoint(int pid)
	{
		if (OperatingSystem.IsWindows())
		{
			pipe = new NamedPipeServerStream("dotnet-diagnostic-" + pid, PipeDirection.InOut,
				maxNumberOfServerInstances: 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
			// Accepting is all it does: the connected client waits for a response that never
			// comes. The task is deliberately not awaited and needs no result.
			_ = pipe.WaitForConnectionAsync();
		}
		else
		{
			socketPath = Path.Combine(Path.GetTempPath(), $"dotnet-diagnostic-{pid}-1-socket");
			File.Delete(socketPath);
			socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.Unspecified);
			socket.Bind(new UnixDomainSocketEndPoint(socketPath));
			socket.Listen(1);
			_ = socket.AcceptAsync();
		}
	}

	public void Dispose()
	{
		pipe?.Dispose();
		socket?.Dispose();
		if (socketPath != null)
			File.Delete(socketPath);
	}
}
