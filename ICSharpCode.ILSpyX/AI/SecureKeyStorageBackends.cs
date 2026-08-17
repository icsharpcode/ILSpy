// Copyright (c) 2026 Masroor
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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpyX.AI
{
	internal static class SecureKeyStorageBackendFactory
	{
		public static ISecureKeyStorageBackend CreateDefault()
		{
			if (OperatingSystem.IsWindows())
				return new WindowsDpapiKeyStorageBackend();
			if (OperatingSystem.IsMacOS())
				return new MacOsKeychainStorageBackend();
			if (OperatingSystem.IsLinux())
				return new LinuxSecretServiceStorageBackend();
			return new UnsupportedSecureKeyStorageBackend();
		}
	}

	internal sealed class UnsupportedSecureKeyStorageBackend : ISecureKeyStorageBackend
	{
		private static SecureKeyStorageUnavailableException CreateException()
			=> new("Secure key storage is unavailable on this platform.");

		public Task SaveAsync(string provider, string key, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			throw CreateException();
		}

		public Task<SecureKeyStorageBackendReadResult> LoadAsync(string provider, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			throw CreateException();
		}

		public Task DeleteAsync(string provider, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			throw CreateException();
		}
	}

	internal sealed class WindowsDpapiKeyStorageBackend : ISecureKeyStorageBackend
	{
		private const uint CryptProtectUiForbidden = 1;
		private readonly string directory;

		public WindowsDpapiKeyStorageBackend()
		{
			if (!OperatingSystem.IsWindows())
				throw new SecureKeyStorageUnavailableException("Windows DPAPI is only available on Windows.");

			string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			if (string.IsNullOrEmpty(localApplicationData))
				throw new SecureKeyStorageUnavailableException("The local application data directory is unavailable.");
			directory = Path.Combine(localApplicationData, "ILSpy", "AI", "Keys");
		}

		public async Task SaveAsync(string provider, string key, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			byte[] plaintext = Encoding.UTF8.GetBytes(key);
			byte[]? protectedData = null;
			string? temporaryPath = null;
			try
			{
				protectedData = Dpapi.Protect(plaintext);
				Directory.CreateDirectory(directory);
				string path = GetPath(provider);
				temporaryPath = path + "." + Path.GetRandomFileName() + ".tmp";
				await File.WriteAllBytesAsync(temporaryPath, protectedData, cancellationToken).ConfigureAwait(false);
				cancellationToken.ThrowIfCancellationRequested();
				File.Move(temporaryPath, path, overwrite: true);
				temporaryPath = null;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex) when (IsUnavailableException(ex))
			{
				throw new SecureKeyStorageUnavailableException("Windows secure key storage is unavailable.", ex);
			}
			finally
			{
				if (temporaryPath is not null)
					TryDelete(temporaryPath);
				CryptographicOperations.ZeroMemory(plaintext);
				if (protectedData is not null)
					CryptographicOperations.ZeroMemory(protectedData);
			}
		}

		public async Task<SecureKeyStorageBackendReadResult> LoadAsync(string provider, CancellationToken cancellationToken)
		{
			byte[]? protectedData = null;
			byte[]? plaintext = null;
			try
			{
				protectedData = await File.ReadAllBytesAsync(GetPath(provider), cancellationToken).ConfigureAwait(false);
				plaintext = Dpapi.Unprotect(protectedData);
				return SecureKeyStorageBackendReadResult.Found(Encoding.UTF8.GetString(plaintext));
			}
			catch (FileNotFoundException)
			{
				return SecureKeyStorageBackendReadResult.NotFound;
			}
			catch (DirectoryNotFoundException)
			{
				return SecureKeyStorageBackendReadResult.NotFound;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex) when (IsUnavailableException(ex))
			{
				throw new SecureKeyStorageUnavailableException("Windows secure key storage is unavailable.", ex);
			}
			finally
			{
				if (protectedData is not null)
					CryptographicOperations.ZeroMemory(protectedData);
				if (plaintext is not null)
					CryptographicOperations.ZeroMemory(plaintext);
			}
		}

		public Task DeleteAsync(string provider, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			try
			{
				File.Delete(GetPath(provider));
				return Task.CompletedTask;
			}
			catch (Exception ex) when (IsUnavailableException(ex))
			{
				throw new SecureKeyStorageUnavailableException("Windows secure key storage is unavailable.", ex);
			}
		}

		private string GetPath(string provider)
			=> Path.Combine(directory, provider + ".bin");

		private static void TryDelete(string path)
		{
			try
			{
				File.Delete(path);
			}
			catch (IOException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
		}

		private static bool IsUnavailableException(Exception exception)
		{
			return exception is CryptographicException
				or DllNotFoundException
				or EntryPointNotFoundException
				or BadImageFormatException
				or IOException
				or UnauthorizedAccessException
				or PlatformNotSupportedException
				or Win32Exception;
		}

		private static class Dpapi
		{
			public static byte[] Protect(byte[] data)
			{
				return Transform(data, CryptProtectUiForbidden, protect: true);
			}

			public static byte[] Unprotect(byte[] data)
			{
				return Transform(data, CryptProtectUiForbidden, protect: false);
			}

			private static byte[] Transform(byte[] data, uint flags, bool protect)
			{
				IntPtr inputMemory = Marshal.AllocHGlobal(data.Length);
				try
				{
					Marshal.Copy(data, 0, inputMemory, data.Length);
					var input = new NativeMethods.DataBlob { Size = data.Length, Data = inputMemory };
					NativeMethods.DataBlob output;
					bool success = protect
						? NativeMethods.CryptProtectData(ref input, null, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, flags, out output)
						: NativeMethods.CryptUnprotectData(ref input, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, flags, out output);
					if (!success)
						throw new CryptographicException(Marshal.GetLastWin32Error());

					try
					{
						byte[] result = new byte[output.Size];
						Marshal.Copy(output.Data, result, 0, output.Size);
						return result;
					}
					finally
					{
						NativeMethods.ZeroAndFree(output);
					}
				}
				finally
				{
					ZeroAndFreeHGlobal(inputMemory, data.Length);
				}
			}

			private static void ZeroAndFreeHGlobal(IntPtr memory, int length)
			{
				for (int i = 0; i < length; i++)
					Marshal.WriteByte(memory, i, 0);
				Marshal.FreeHGlobal(memory);
			}
		}

	}

	internal sealed class MacOsKeychainStorageBackend : ISecureKeyStorageBackend
	{
		private const string SecurityPath = "/usr/bin/security";
		private const string Service = "com.icsharpcode.ilspy.ai";

		public async Task SaveAsync(string provider, string key, CancellationToken cancellationToken)
		{
			ProcessResult result = await SecureKeyStorageProcess.RunAsync(
				SecurityPath,
				new[] { "add-generic-password", "-a", provider, "-s", Service, "-U", "-w" },
				key + "\n",
				cancellationToken).ConfigureAwait(false);
			if (result.ExitCode != 0)
				throw new SecureKeyStorageUnavailableException("macOS Keychain is unavailable.");
		}

		public async Task<SecureKeyStorageBackendReadResult> LoadAsync(string provider, CancellationToken cancellationToken)
		{
			ProcessResult result = await SecureKeyStorageProcess.RunAsync(
				SecurityPath,
				new[] { "find-generic-password", "-a", provider, "-s", Service, "-w" },
				null,
				cancellationToken).ConfigureAwait(false);
			if (result.ExitCode == 0)
				return SecureKeyStorageBackendReadResult.Found(result.Output.TrimEnd('\r', '\n'));
			if (IsNotFound(result))
				return SecureKeyStorageBackendReadResult.NotFound;
			throw new SecureKeyStorageUnavailableException("macOS Keychain is unavailable.");
		}

		public async Task DeleteAsync(string provider, CancellationToken cancellationToken)
		{
			ProcessResult result = await SecureKeyStorageProcess.RunAsync(
				SecurityPath,
				new[] { "delete-generic-password", "-a", provider, "-s", Service },
				null,
				cancellationToken).ConfigureAwait(false);
			if (result.ExitCode != 0 && !IsNotFound(result))
				throw new SecureKeyStorageUnavailableException("macOS Keychain is unavailable.");
		}

		private static bool IsNotFound(ProcessResult result)
		{
			return result.ExitCode == 44
				|| result.Error.Contains("could not be found", StringComparison.OrdinalIgnoreCase)
				|| result.Error.Contains("item not found", StringComparison.OrdinalIgnoreCase);
		}
	}

	internal sealed class LinuxSecretServiceStorageBackend : ISecureKeyStorageBackend
	{
		private const string SecretTool = "secret-tool";
		private const string Service = "com.icsharpcode.ilspy.ai";

		public async Task SaveAsync(string provider, string key, CancellationToken cancellationToken)
		{
			ProcessResult result = await SecureKeyStorageProcess.RunAsync(
				SecretTool,
				new[] { "store", "--label=ILSpy AI API key", "service", Service, "provider", provider },
				key + "\n",
				cancellationToken).ConfigureAwait(false);
			if (result.ExitCode != 0)
				throw new SecureKeyStorageUnavailableException("Linux Secret Service is unavailable.");
		}

		public async Task<SecureKeyStorageBackendReadResult> LoadAsync(string provider, CancellationToken cancellationToken)
		{
			ProcessResult result = await SecureKeyStorageProcess.RunAsync(
				SecretTool,
				new[] { "lookup", "service", Service, "provider", provider },
				null,
				cancellationToken).ConfigureAwait(false);
			if (result.ExitCode == 0)
				return SecureKeyStorageBackendReadResult.Found(result.Output.TrimEnd('\r', '\n'));
			if (result.ExitCode == 1)
				return SecureKeyStorageBackendReadResult.NotFound;
			throw new SecureKeyStorageUnavailableException("Linux Secret Service is unavailable.");
		}

		public async Task DeleteAsync(string provider, CancellationToken cancellationToken)
		{
			ProcessResult result = await SecureKeyStorageProcess.RunAsync(
				SecretTool,
				new[] { "clear", "service", Service, "provider", provider },
				null,
				cancellationToken).ConfigureAwait(false);
			if (result.ExitCode is not 0 and not 1)
				throw new SecureKeyStorageUnavailableException("Linux Secret Service is unavailable.");
		}
	}

	internal readonly record struct ProcessResult(int ExitCode, string Output, string Error);

	internal static class SecureKeyStorageProcess
	{
		public static async Task<ProcessResult> RunAsync(
			string fileName,
			string[] arguments,
			string? standardInput,
			CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var startInfo = new ProcessStartInfo
			{
				FileName = fileName,
				UseShellExecute = false,
				RedirectStandardInput = standardInput is not null,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};
			foreach (string argument in arguments)
				startInfo.ArgumentList.Add(argument);

			using var process = new Process { StartInfo = startInfo };
			try
			{
				if (!process.Start())
					throw new SecureKeyStorageUnavailableException("Secure key storage process could not be started.");
			}
			catch (SecureKeyStorageUnavailableException)
			{
				throw;
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or Win32Exception or InvalidOperationException)
			{
				throw new SecureKeyStorageUnavailableException("Secure key storage process could not be started.", ex);
			}

			using CancellationTokenRegistration cancellationRegistration = cancellationToken.Register(static state =>
			{
				var runningProcess = (Process)state!;
				try
				{
					if (!runningProcess.HasExited)
						runningProcess.Kill(entireProcessTree: true);
				}
				catch (InvalidOperationException)
				{
				}
				catch (Win32Exception)
				{
				}
			}, process);

			Task<string> outputTask = process.StandardOutput.ReadToEndAsync(cancellationToken);
			Task<string> errorTask = process.StandardError.ReadToEndAsync(cancellationToken);
			try
			{
				if (standardInput is not null)
				{
					await process.StandardInput.WriteAsync(standardInput.AsMemory(), cancellationToken).ConfigureAwait(false);
					process.StandardInput.Close();
				}
				await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
				return new ProcessResult(process.ExitCode, await outputTask.ConfigureAwait(false), await errorTask.ConfigureAwait(false));
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or Win32Exception or InvalidOperationException)
			{
				throw new SecureKeyStorageUnavailableException("Secure key storage process failed.", ex);
			}
			finally
			{
				if (standardInput is not null)
					process.StandardInput.Close();
			}
		}
	}

	internal static class NativeMethods
	{
		[StructLayout(LayoutKind.Sequential)]
		internal struct DataBlob
		{
			internal int Size;
			internal IntPtr Data;
		}

		[DllImport("Crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CryptProtectData(
			ref DataBlob dataIn,
			string? description,
			IntPtr optionalEntropy,
			IntPtr reserved,
			IntPtr promptStruct,
			uint flags,
			out DataBlob dataOut);

		[DllImport("Crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CryptUnprotectData(
			ref DataBlob dataIn,
			IntPtr description,
			IntPtr optionalEntropy,
			IntPtr reserved,
			IntPtr promptStruct,
			uint flags,
			out DataBlob dataOut);

		[DllImport("Kernel32.dll", EntryPoint = "LocalFree")]
		private static extern IntPtr LocalFreeNative(IntPtr handle);

		internal static void ZeroAndFree(DataBlob data)
		{
			ZeroAndFree(data.Data, data.Size);
		}

		internal static void ZeroAndFree(IntPtr data, int size)
		{
			if (data == IntPtr.Zero)
				return;
			for (int i = 0; i < size; i++)
				Marshal.WriteByte(data, i, 0);
			LocalFreeNative(data);
		}
	}
}
