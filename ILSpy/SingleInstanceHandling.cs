// Copyright (c) 2022 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ICSharpCode.ILSpy
{
	internal static class SingleInstanceHandling
	{
		internal static Mutex SingleInstanceMutex;

		internal static void ForceSingleInstance(IEnumerable<string> cmdArgs)
		{
			bool isFirst;
			try
			{
				SingleInstanceMutex = new Mutex(initiallyOwned: true, @"Local\ILSpyInstance", out isFirst);
			}
			catch (WaitHandleCannotBeOpenedException)
			{
				isFirst = true;
			}
			if (!isFirst)
			{
				try
				{
					SingleInstanceMutex.WaitOne(10000);
				}
				catch (AbandonedMutexException)
				{
					// continue, there is no concurrent start happening.
				}
			}
			cmdArgs = cmdArgs.Select(FullyQualifyPath);
			string message = string.Join(Environment.NewLine, cmdArgs);
			if (SendToPreviousInstance("ILSpy:\r\n" + message, !App.CommandLineArguments.NoActivate))
			{
				ReleaseSingleInstanceMutex();
				Environment.Exit(0);
			}
		}

		internal static string FullyQualifyPath(string argument)
		{
			// Fully qualify the paths before passing them to another process,
			// because that process might use a different current directory.
			if (string.IsNullOrEmpty(argument) || argument[0] == '/')
				return argument;
			try
			{
				return Path.Combine(Environment.CurrentDirectory, argument);
			}
			catch (ArgumentException)
			{
				return argument;
			}
		}

		internal static void ReleaseSingleInstanceMutex()
		{
			var mutex = SingleInstanceMutex;
			SingleInstanceMutex = null;
			if (mutex == null)
			{
				return;
			}
			using (mutex)
			{
				mutex.ReleaseMutex();
			}
		}

		#region Pass Command Line Arguments to previous instance
		internal static bool SendToPreviousInstance(string message, bool activate)
		{
			string ownProcessName;
			using (var ownProcess = Process.GetCurrentProcess())
			{
				ownProcessName = ownProcess.ProcessName;
			}

			bool success = false;
			NativeMethods.EnumWindows(
				(hWnd, lParam) => {
					string windowTitle = NativeMethods.GetWindowText(hWnd, 100);
					if (windowTitle.StartsWith("ILSpy", StringComparison.Ordinal))
					{
						string processName = NativeMethods.GetProcessNameFromWindow(hWnd);
						Debug.WriteLine("Found {0:x4}: '{1}' in '{2}'", hWnd, windowTitle, processName);
						if (string.Equals(processName, ownProcessName, StringComparison.OrdinalIgnoreCase))
						{
							IntPtr result = Send(hWnd, message);
							Debug.WriteLine("WM_COPYDATA result: {0:x8}", result);
							if (result == (IntPtr)1)
							{
								if (activate)
									NativeMethods.SetForegroundWindow(hWnd);
								success = true;
								return false; // stop enumeration
							}
						}
					}
					return true; // continue enumeration
				}, IntPtr.Zero);
			return success;
		}

		unsafe static IntPtr Send(IntPtr hWnd, string message)
		{
			const uint SMTO_NORMAL = 0;

			CopyDataStruct lParam;
			lParam.Padding = IntPtr.Zero;
			lParam.Size = message.Length * 2;
			fixed (char* buffer = message)
			{
				lParam.Buffer = (IntPtr)buffer;
				IntPtr result;
				// SendMessage with 3s timeout (e.g. when the target process is stopped in the debugger)
				if (NativeMethods.SendMessageTimeout(
					hWnd, NativeMethods.WM_COPYDATA, IntPtr.Zero, ref lParam,
					SMTO_NORMAL, 3000, out result) != IntPtr.Zero)
				{
					return result;
				}
				else
				{
					return IntPtr.Zero;
				}
			}
		}
		#endregion
	}
}
