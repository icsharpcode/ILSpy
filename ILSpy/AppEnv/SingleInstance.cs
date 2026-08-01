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
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;

using ICSharpCode.ILSpy.Options;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.AppEnv
{
	/// <summary>
	/// Cross-platform single-instance coordination. The first launch for a user takes a named
	/// <see cref="Mutex"/> and listens on a named pipe; a later launch forwards its command-line
	/// arguments over that pipe and exits, so the running window handles them instead of a second
	/// window opening.
	///
	/// The mutex/pipe namespace is derived from machine + user only -- never the executable location
	/// -- so any launcher (Explorer "Open with", the CLI, a debug build at a different path) reuses
	/// the running instance. A launcher that needs a specific instance passes <c>--instanceid</c>;
	/// that value is a runtime reuse filter (see <see cref="ShouldReuse"/>), not part of the
	/// namespace, so it never re-introduces the location-partitioning it replaces.
	/// </summary>
	public static class SingleInstance
	{
		static readonly object gate = new();
		static Mutex? heldMutex;
		static string? ownLaunchId;
		static Action<string[]>? newInstanceHandler;
		static string[]? bufferedArgs;

		/// <summary>
		/// Raised (on a background thread) when another instance forwarded its arguments and the
		/// running instance accepted them. If no handler is attached yet -- the pipe server can
		/// outrace application startup -- the last payload is buffered and replayed on subscribe.
		/// </summary>
		public static event Action<string[]> NewInstanceDetected {
			add {
				string[]? replay = null;
				lock (gate)
				{
					newInstanceHandler += value;
					if (bufferedArgs is { } pending)
					{
						bufferedArgs = null;
						replay = pending;
					}
				}
				// Invoke the replay outside the lock: the handler must never run arbitrary code
				// (or re-enter SingleInstance) while gate is held.
				if (replay != null)
					value(replay);
			}
			remove {
				lock (gate)
				{
					newInstanceHandler -= value;
				}
			}
		}

		/// <summary>
		/// The mutex/pipe name shared by every launch of the current user, independent of the
		/// executable's location. Contains no path separators so it is a valid mutex/pipe identifier.
		/// </summary>
		public static string GetInstanceName()
		{
			var material = Encoding.UTF8.GetBytes(Environment.MachineName + "\n" + Environment.UserName);
			return "ILSpy." + Convert.ToHexString(SHA256.HashData(material));
		}

		/// <summary>
		/// The identity a launcher can target with <c>--instanceid</c>: the full path of the running
		/// executable. Uses <see cref="Environment.ProcessPath"/> (valid even in a single-file
		/// bundle, unlike <c>Assembly.Location</c>); empty when it cannot be determined.
		/// </summary>
		public static string SelfExecutableId()
		{
			var path = Environment.ProcessPath;
			if (string.IsNullOrWhiteSpace(path))
				return string.Empty;
			try
			{
				return Path.GetFullPath(path);
			}
			catch (Exception)
			{
				return path;
			}
		}

		/// <summary>
		/// Decides whether a launch requesting <paramref name="requestedId"/> may reuse a running
		/// instance whose own launch id is <paramref name="runningLaunchId"/> and whose executable
		/// identity is <paramref name="runningExecutableId"/>. Reuse when nothing specific was
		/// requested, or the request matches either the running instance's launch id or the
		/// executable it actually is.
		/// </summary>
		public static bool ShouldReuse(string? requestedId, string? runningLaunchId, string runningExecutableId)
		{
			if (string.IsNullOrEmpty(requestedId))
				return true;
			return IdentityEquals(requestedId, runningLaunchId)
				|| IdentityEquals(requestedId, runningExecutableId);
		}

		static bool IdentityEquals(string? a, string? b)
		{
			if (a is null || b is null)
				return false;
			// --instanceid is typically an executable path, so compare the way the platform compares
			// paths: case-insensitive on Windows, case-sensitive elsewhere.
			var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
			return string.Equals(a, b, comparison);
		}

		/// <summary>
		/// Resolves a forwarded command-line token to an absolute path when it names an existing file
		/// relative to the sending process's working directory; leaves option flags, non-file values,
		/// and already-rooted paths untouched. The running instance has a different working directory,
		/// so relative assembly paths must be qualified by the sender before forwarding.
		/// </summary>
		public static string FullyQualifyPath(string arg) => FullyQualifyPath(arg, Environment.CurrentDirectory);

		internal static string FullyQualifyPath(string arg, string baseDirectory)
		{
			if (string.IsNullOrEmpty(arg) || Path.IsPathRooted(arg))
				return arg;
			try
			{
				var full = Path.GetFullPath(arg, baseDirectory);
				if (File.Exists(full))
					return full;
			}
			catch (Exception)
			{
				// Not a usable path token (e.g. an option value with characters illegal in a path);
				// forward it verbatim so the receiver re-parses it exactly as typed.
			}
			return arg;
		}

		/// <summary>
		/// Coordinates single-instance startup. Returns <c>true</c> when this process should start its
		/// window (it is the first instance, single-instance is disabled, or the running instance was
		/// dead or declined the hand-off); returns <c>false</c> when the arguments were forwarded to a
		/// running instance and this process should exit.
		/// </summary>
		public static bool TrySignalFirstInstance(CommandLineArguments args)
		{
			bool forceSingleInstance = (args.SingleInstance ?? true) && !ReadAllowMultipleInstances();
			if (!forceSingleInstance)
				return true;

			try
			{
				string name = GetInstanceName();
				var mutex = new Mutex(initiallyOwned: false, @"Global\" + name);

				// Acquiring the mutex -- not merely observing that it exists -- is what makes this
				// process the owner. A crashed previous owner leaves it abandoned, so the next waiter
				// acquires it (AbandonedMutexException) and legitimately takes over. This is why
				// hand-off failure below never falls through to BecomeServer: only true ownership,
				// established here, starts a pipe server, so a startup race or a transient pipe
				// failure can never leave two processes both serving.
				bool owned;
				try
				{
					owned = mutex.WaitOne(0);
				}
				catch (AbandonedMutexException)
				{
					owned = true;
				}

				if (owned)
				{
					BecomeServer(mutex, name, args.InstanceId);
					return true;
				}

				// A live instance owns the mutex: hand off to it. If it accepts, exit; if it declines
				// (build-affinity mismatch) or its listener is transiently unreachable, start a normal
				// window -- without ever becoming a second server we do not own.
				bool accepted = ForwardToRunningInstance(name, args.InstanceId);
				mutex.Dispose();
				return !accepted;
			}
			catch (Exception ex)
			{
				Trace.TraceWarning("SingleInstance: falling back to a normal launch. " + ex);
				return true;
			}
		}

		static void BecomeServer(Mutex mutex, string name, string? launchId)
		{
			heldMutex = mutex;
			ownLaunchId = launchId;
			var thread = new Thread(() => ServerLoop(name)) {
				Name = "ILSpy single-instance listener",
				IsBackground = true
			};
			thread.Start();
		}

		static void ServerLoop(string name)
		{
			while (true)
			{
				try
				{
					using var server = new NamedPipeServerStream(name, PipeDirection.InOut, 1,
						PipeTransmissionMode.Byte, PipeOptions.CurrentUserOnly);
					server.WaitForConnection();
					HandleConnection(server);
				}
				catch (Exception ex)
				{
					Trace.TraceWarning("SingleInstance listener: " + ex);
					Thread.Sleep(100);
				}
			}
		}

		static void HandleConnection(NamedPipeServerStream server)
		{
			var request = JsonSerializer.Deserialize<ForwardRequest>(ReadMessage(server));
			bool accepted = request != null
				&& ShouldReuse(request.RequestedId, ownLaunchId, SelfExecutableId());
			WriteMessage(server, JsonSerializer.SerializeToUtf8Bytes(new ForwardResponse(accepted)));
			server.Flush();
			if (accepted && request != null)
				RaiseNewInstance(request.Args);
		}

		static void RaiseNewInstance(string[] args)
		{
			Action<string[]>? handler;
			lock (gate)
			{
				handler = newInstanceHandler;
				if (handler is null)
				{
					bufferedArgs = args;
					return;
				}
			}
			handler(args);
		}

		// Returns true if the running instance accepted the forwarded arguments (so this process
		// should exit); false if it declined or could not be reached (so this process starts a
		// normal window).
		static bool ForwardToRunningInstance(string name, string? requestedId)
		{
			try
			{
				using var client = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.CurrentUserOnly);
				client.Connect(2000);
				var forwarded = Environment.GetCommandLineArgs().Skip(1).Select(FullyQualifyPath).ToArray();
				WriteMessage(client, JsonSerializer.SerializeToUtf8Bytes(new ForwardRequest(requestedId, forwarded)));
				client.Flush();
				var response = JsonSerializer.Deserialize<ForwardResponse>(ReadMessage(client));
				return response?.Accepted == true;
			}
			catch (Exception ex)
			{
				Trace.TraceWarning("SingleInstance hand-off failed: " + ex);
				return false;
			}
		}

		static bool ReadAllowMultipleInstances()
		{
			try
			{
				var settings = new MiscSettings();
				settings.LoadFromXml(ILSpySettings.Load()["MiscSettings"]);
				return settings.AllowMultipleInstances;
			}
			catch (Exception)
			{
				// A missing or unreadable settings file must not force multiple instances.
				return false;
			}
		}

		static void WriteMessage(Stream stream, byte[] payload)
		{
			Span<byte> lengthPrefix = stackalloc byte[sizeof(int)];
			BinaryPrimitives.WriteInt32LittleEndian(lengthPrefix, payload.Length);
			stream.Write(lengthPrefix);
			stream.Write(payload);
		}

		static byte[] ReadMessage(Stream stream)
		{
			Span<byte> lengthPrefix = stackalloc byte[sizeof(int)];
			stream.ReadExactly(lengthPrefix);
			int length = BinaryPrimitives.ReadInt32LittleEndian(lengthPrefix);
			if (length < 0 || length > 16 * 1024 * 1024)
				throw new InvalidDataException("SingleInstance: message length out of range.");
			var payload = new byte[length];
			stream.ReadExactly(payload);
			return payload;
		}

		sealed record ForwardRequest(string? RequestedId, string[] Args);

		sealed record ForwardResponse(bool Accepted);
	}
}
