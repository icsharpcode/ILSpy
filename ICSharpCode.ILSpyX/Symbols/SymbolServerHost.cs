// Copyright (c) 2026 Piero Viano
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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace ICSharpCode.ILSpyX.Symbols
{
	/// <summary>Resolves a symbol-server key (<c>name/id/name</c>, lower-case) to a local file.</summary>
	public interface ISymbolFileSource
	{
		/// <returns>The path of the file to serve, or <c>null</c> for 404.</returns>
		Task<string?> GetFileAsync(string key, CancellationToken cancellationToken);
	}

	/// <summary>
	/// A minimal HTTP/1.1 symbol server in the SSQP / SymSrv layout (<c>GET /name/id/name</c>)
	/// bound to the loopback interface only, so debuggers on this machine can use it as a symbol
	/// source while nothing is exposed to the network.
	/// </summary>
	public sealed class SymbolServerHost : IDisposable
	{
		const int MaxHeaderBytes = 16 * 1024;

		readonly ISymbolFileSource source;
		readonly int requestedPort;
		TcpListener? listener;
		CancellationTokenSource? shutdown;
		Task? acceptLoop;

		/// <param name="port">The loopback port to listen on; 0 picks a free port.</param>
		public SymbolServerHost(ISymbolFileSource source, int port = 0)
		{
			this.source = source ?? throw new ArgumentNullException(nameof(source));
			if (port < 0 || port > IPEndPoint.MaxPort)
				throw new ArgumentOutOfRangeException(nameof(port));
			this.requestedPort = port;
		}

		public bool IsRunning => listener != null;

		/// <summary>The URL to configure as the debugger's symbol server; <c>null</c> when stopped.</summary>
		public Uri? BaseAddress { get; private set; }

		/// <summary>Raised after each request with the request path and the response status code.</summary>
		public event Action<string, int>? RequestServed;

		/// <exception cref="SocketException">The port is in use.</exception>
		public void Start()
		{
			if (listener != null)
				return;
			listener = new TcpListener(IPAddress.Loopback, requestedPort);
			try
			{
				listener.Start();
			}
			catch
			{
				listener.Dispose();
				listener = null;
				throw;
			}
			shutdown = new CancellationTokenSource();
			BaseAddress = new Uri($"http://localhost:{((IPEndPoint)listener.LocalEndpoint).Port}/");
			acceptLoop = AcceptLoopAsync(listener, shutdown.Token);
		}

		public async Task StopAsync()
		{
			var l = listener;
			if (l == null)
				return;
			listener = null;
			BaseAddress = null;
			shutdown!.Cancel();
			l.Dispose();
			try
			{
				await acceptLoop!.ConfigureAwait(false);
			}
			catch (Exception ex) when (ex is OperationCanceledException || ex is ObjectDisposedException || ex is SocketException)
			{
			}
			shutdown.Dispose();
		}

		public void Dispose()
		{
			StopAsync().GetAwaiter().GetResult();
			listener?.Dispose();
			shutdown?.Dispose();
		}

		async Task AcceptLoopAsync(TcpListener l, CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				TcpClient client;
				try
				{
					client = await l.AcceptTcpClientAsync(token).ConfigureAwait(false);
				}
				catch (Exception ex) when (ex is OperationCanceledException || ex is ObjectDisposedException || ex is SocketException)
				{
					return;
				}
				_ = Task.Run(() => HandleClientAsync(client, token), CancellationToken.None);
			}
		}

		async Task HandleClientAsync(TcpClient client, CancellationToken token)
		{
			using (client)
			{
				try
				{
					var stream = client.GetStream();
					string? requestLine = await ReadHeadAsync(stream, token).ConfigureAwait(false);
					if (requestLine == null)
						return;
					string[] parts = requestLine.Split(' ');
					if (parts.Length != 3 || !parts[2].StartsWith("HTTP/1.", StringComparison.Ordinal))
					{
						await WriteStatusAsync(stream, 400, "Bad Request", token).ConfigureAwait(false);
						return;
					}
					bool isHead = parts[0] == "HEAD";
					if (parts[0] != "GET" && !isHead)
					{
						await WriteStatusAsync(stream, 405, "Method Not Allowed", token).ConfigureAwait(false);
						return;
					}
					string? key = NormalizeKey(parts[1]);
					string? path = key == null ? null : await source.GetFileAsync(key, token).ConfigureAwait(false);
					if (path == null || !File.Exists(path))
					{
						await WriteStatusAsync(stream, 404, "Not Found", token).ConfigureAwait(false);
						RequestServed?.Invoke(parts[1], 404);
						return;
					}
					using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						string header = "HTTP/1.1 200 OK\r\nContent-Type: application/octet-stream\r\n"
							+ $"Content-Length: {file.Length}\r\nConnection: close\r\n\r\n";
						await stream.WriteAsync(Encoding.ASCII.GetBytes(header), token).ConfigureAwait(false);
						if (!isHead)
							await file.CopyToAsync(stream, token).ConfigureAwait(false);
					}
					RequestServed?.Invoke(parts[1], 200);
				}
				catch (Exception ex) when (ex is IOException || ex is OperationCanceledException || ex is SocketException || ex is ObjectDisposedException)
				{
					// The client went away or the server is stopping.
				}
			}
		}

		/// <summary>
		/// Maps a request target to a key: strips the query, URL-decodes, lower-cases, and accepts only
		/// <c>name/id/name</c> with plain segments (no traversal).
		/// </summary>
		internal static string? NormalizeKey(string target)
		{
			int query = target.IndexOfAny(new[] { '?', '#' });
			if (query >= 0)
				target = target.Substring(0, query);
			string decoded = Uri.UnescapeDataString(target).Trim('/').ToLowerInvariant();
			string[] segments = decoded.Split('/');
			if (segments.Length != 3)
				return null;
			foreach (var segment in segments)
			{
				if (segment.Length == 0 || segment == "." || segment == ".."
					|| segment.IndexOfAny(new[] { '\\', ':' }) >= 0 || segment.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
					return null;
			}
			return decoded;
		}

		/// <summary>Reads the request head and returns the request line; headers are not needed.</summary>
		static async Task<string?> ReadHeadAsync(NetworkStream stream, CancellationToken token)
		{
			var buffer = new byte[MaxHeaderBytes];
			int length = 0;
			while (length < buffer.Length)
			{
				int read = await stream.ReadAsync(buffer.AsMemory(length), token).ConfigureAwait(false);
				if (read == 0)
					return null;
				length += read;
				string text = Encoding.ASCII.GetString(buffer, 0, length);
				int end = text.IndexOf("\r\n\r\n", StringComparison.Ordinal);
				if (end >= 0)
				{
					int lineEnd = text.IndexOf("\r\n", StringComparison.Ordinal);
					return text.Substring(0, lineEnd);
				}
			}
			return null;
		}

		static Task WriteStatusAsync(NetworkStream stream, int code, string reason, CancellationToken token)
		{
			string response = $"HTTP/1.1 {code} {reason}\r\nContent-Length: 0\r\nConnection: close\r\n\r\n";
			return stream.WriteAsync(Encoding.ASCII.GetBytes(response), token).AsTask();
		}
	}
}
