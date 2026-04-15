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
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

using Avalonia.Threading;

namespace ILSpy.AppEnv
{
	public static class GlobalExceptionHandler
	{
		[ThreadStatic]
		static bool showingError;

		public static void Install()
		{
			if (Debugger.IsAttached)
				return;

			AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandled;
			TaskScheduler.UnobservedTaskException += OnUnobservedTask;
			Dispatcher.UIThread.UnhandledException += OnDispatcherUnhandled;
		}

		static void OnAppDomainUnhandled(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception ex)
				Report(ex);
		}

		static void OnUnobservedTask(object? sender, UnobservedTaskExceptionEventArgs e)
		{
			e.SetObserved();
			Report(e.Exception);
		}

		static void OnDispatcherUnhandled(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Report(e.Exception);
			e.Handled = true;
		}

		static void Report(Exception exception)
		{
			Debug.WriteLine(exception.ToString());
			for (var ex = exception; ex != null; ex = ex.InnerException)
			{
				if (ex is ReflectionTypeLoadException rtle && rtle.LoaderExceptions.Length > 0 && rtle.LoaderExceptions[0] is { } loader)
				{
					exception = loader;
					Debug.WriteLine(exception.ToString());
					break;
				}
			}

			if (showingError)
				return;
			showingError = true;
			try
			{
				// TODO: surface via CustomDialog once ported.
				Debug.WriteLine("[Unhandled] " + exception);
			}
			finally
			{
				showingError = false;
			}
		}
	}
}