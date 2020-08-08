// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;

namespace ICSharpCode.ILSpy
{
	static class NativeMethods
	{
		public const uint WM_COPYDATA = 0x4a;

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern unsafe int GetWindowThreadProcessId(IntPtr hWnd, int* lpdwProcessId);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder title, int size);

		public static string GetWindowText(IntPtr hWnd, int maxLength)
		{
			StringBuilder b = new StringBuilder(maxLength + 1);
			if (GetWindowText(hWnd, b, b.Capacity) != 0)
				return b.ToString();
			else
				return string.Empty;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		internal static extern IntPtr SendMessageTimeout(
			IntPtr hWnd, uint msg, IntPtr wParam, ref CopyDataStruct lParam,
			uint flags, uint timeout, out IntPtr result);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
		[DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		static extern unsafe char** CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass")]
		[DllImport("kernel32.dll")]
		static extern IntPtr LocalFree(IntPtr hMem);

		#region CommandLine <-> Argument Array
		/// <summary>
		/// Decodes a command line into an array of arguments according to the CommandLineToArgvW rules.
		/// </summary>
		/// <remarks>
		/// Command line parsing rules:
		/// - 2n backslashes followed by a quotation mark produce n backslashes, and the quotation mark is considered to be the end of the argument.
		/// - (2n) + 1 backslashes followed by a quotation mark again produce n backslashes followed by a quotation mark.
		/// - n backslashes not followed by a quotation mark simply produce n backslashes.
		/// </remarks>
		public static unsafe string[] CommandLineToArgumentArray(string commandLine)
		{
			if (string.IsNullOrEmpty(commandLine))
				return new string[0];
			int numberOfArgs;
			char** arr = CommandLineToArgvW(commandLine, out numberOfArgs);
			if (arr == null)
				throw new Win32Exception();
			try {
				string[] result = new string[numberOfArgs];
				for (int i = 0; i < numberOfArgs; i++) {
					result[i] = new string(arr[i]);
				}
				return result;
			} finally {
				// Free memory obtained by CommandLineToArgW.
				LocalFree(new IntPtr(arr));
			}
		}

		static readonly char[] charsNeedingQuoting = { ' ', '\t', '\n', '\v', '"' };

		/// <summary>
		/// Escapes a set of arguments according to the CommandLineToArgvW rules.
		/// </summary>
		/// <remarks>
		/// Command line parsing rules:
		/// - 2n backslashes followed by a quotation mark produce n backslashes, and the quotation mark is considered to be the end of the argument.
		/// - (2n) + 1 backslashes followed by a quotation mark again produce n backslashes followed by a quotation mark.
		/// - n backslashes not followed by a quotation mark simply produce n backslashes.
		/// </remarks>
		public static string ArgumentArrayToCommandLine(params string[] arguments)
		{
			if (arguments == null)
				return null;
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < arguments.Length; i++) {
				if (i > 0)
					b.Append(' ');
				AppendArgument(b, arguments[i]);
			}
			return b.ToString();
		}

		static void AppendArgument(StringBuilder b, string arg)
		{
			if (arg == null) {
				return;
			}

			if (arg.Length > 0 && arg.IndexOfAny(charsNeedingQuoting) < 0) {
				b.Append(arg);
			} else {
				b.Append('"');
				for (int j = 0; ; j++) {
					int backslashCount = 0;
					while (j < arg.Length && arg[j] == '\\') {
						backslashCount++;
						j++;
					}
					if (j == arg.Length) {
						b.Append('\\', backslashCount * 2);
						break;
					} else if (arg[j] == '"') {
						b.Append('\\', backslashCount * 2 + 1);
						b.Append('"');
					} else {
						b.Append('\\', backslashCount);
						b.Append(arg[j]);
					}
				}
				b.Append('"');
			}
		}
		#endregion

		public unsafe static string GetProcessNameFromWindow(IntPtr hWnd)
		{
			int processId;
			GetWindowThreadProcessId(hWnd, &processId);
			try {
				using (var p = Process.GetProcessById(processId)) {
					return p.ProcessName;
				}
			} catch (ArgumentException ex) {
				Debug.WriteLine(ex.Message);
				return null;
			} catch (InvalidOperationException ex) {
				Debug.WriteLine(ex.Message);
				return null;
			} catch (Win32Exception ex) {
				Debug.WriteLine(ex.Message);
				return null;
			}
		}
	}

	[return: MarshalAs(UnmanagedType.Bool)]
	delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

	[StructLayout(LayoutKind.Sequential)]
	struct CopyDataStruct
	{
		public IntPtr Padding;
		public int Size;
		public IntPtr Buffer;

		public CopyDataStruct(IntPtr padding, int size, IntPtr buffer)
		{
			this.Padding = padding;
			this.Size = size;
			this.Buffer = buffer;
		}
	}
}
