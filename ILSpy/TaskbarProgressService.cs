// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Composition;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace ICSharpCode.ILSpy
{
	public enum TaskbarProgressState
	{
		None,
		Indeterminate,
		Normal,
	}

	/// <summary>
	/// Window-level taskbar progress indicator — currently only the
	/// <see cref="TaskbarProgressState.Indeterminate"/> / <c>None</c> cycle is wired up,
	/// driven by the decompile spinner. On non-Windows hosts the state is tracked but the
	/// OS hook is a no-op.
	/// </summary>
	[Export]
	[Shared]
	public class TaskbarProgressService
	{
		public TaskbarProgressState State { get; private set; }

		public event Action<TaskbarProgressState>? StateChanged;

		public void SetState(TaskbarProgressState state)
		{
			if (State == state)
				return;
			State = state;
			ApplyToOs(state);
			StateChanged?.Invoke(state);
		}

		void ApplyToOs(TaskbarProgressState state)
		{
			if (!OperatingSystem.IsWindows())
				return;
			var hwnd = TryGetMainWindowHandle();
			if (hwnd == IntPtr.Zero)
				return;
			ApplyToWindowsTaskbar(hwnd, state);
		}

		static IntPtr TryGetMainWindowHandle()
		{
			var window = AppEnv.UiContext.MainWindow;
			return window?.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
		}

		[SupportedOSPlatform("windows")]
		static void ApplyToWindowsTaskbar(IntPtr hwnd, TaskbarProgressState state)
		{
			var taskbar = WindowsTaskbarList.Instance;
			if (taskbar == null)
				return;
			taskbar.SetProgressState(hwnd, state switch {
				TaskbarProgressState.Indeterminate => TBPFLAG.TBPF_INDETERMINATE,
				TaskbarProgressState.Normal => TBPFLAG.TBPF_NORMAL,
				_ => TBPFLAG.TBPF_NOPROGRESS,
			});
		}
	}

	[SupportedOSPlatform("windows")]
	static class WindowsTaskbarList
	{
		static ITaskbarList3? instance;
		static bool initialised;

		public static ITaskbarList3? Instance {
			get {
				if (initialised)
					return instance;
				initialised = true;
				try
				{
					var t = Type.GetTypeFromCLSID(new Guid("56FDF344-FD6D-11d0-958A-006097C9A090"));
					if (t == null)
						return null;
					var obj = Activator.CreateInstance(t);
					if (obj is ITaskbarList3 list)
					{
						list.HrInit();
						instance = list;
					}
				}
				catch
				{
					// COM init failed — leave instance null and silently fall back.
				}
				return instance;
			}
		}
	}

	internal enum TBPFLAG
	{
		TBPF_NOPROGRESS = 0,
		TBPF_INDETERMINATE = 0x1,
		TBPF_NORMAL = 0x2,
	}

	[ComImport]
	[Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface ITaskbarList3
	{
		// ITaskbarList
		void HrInit();
		void AddTab(IntPtr hwnd);
		void DeleteTab(IntPtr hwnd);
		void ActivateTab(IntPtr hwnd);
		void SetActiveAlt(IntPtr hwnd);
		// ITaskbarList2
		void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fullscreen);
		// ITaskbarList3
		void SetProgressValue(IntPtr hwnd, ulong completed, ulong total);
		void SetProgressState(IntPtr hwnd, TBPFLAG flags);
	}
}
