using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.AddIn
{
	class ILSpyParameters
	{
		public ILSpyParameters(IEnumerable<string> assemblyFileNames, params string[] arguments)
		{
			this.AssemblyFileNames = assemblyFileNames;
			this.Arguments = arguments;
		}

		public IEnumerable<string> AssemblyFileNames { get; private set; }
		public string[] Arguments { get; private set; }
	}

	class ILSpyInstance
	{
		readonly ILSpyParameters parameters;

		public ILSpyInstance(ILSpyParameters parameters = null)
		{
			this.parameters = parameters;
		}

		static string GetILSpyPath()
		{
			var basePath = Path.GetDirectoryName(typeof(ILSpyAddInPackage).Assembly.Location);
			return Path.Combine(basePath, "ILSpy", "ILSpy.exe");
		}

		public void Start()
		{
			var commandLineArguments = parameters?.AssemblyFileNames?.Concat(parameters.Arguments);

			var process = new Process() {
				StartInfo = new ProcessStartInfo() {
					FileName = GetILSpyPath(),
					UseShellExecute = false,
					Arguments = "/navigateTo:none"
				}
			};
			process.Start();

			if ((commandLineArguments != null) && commandLineArguments.Any()) {
				// Only need a message to started process if there are any parameters to pass
				SendMessage(process, "ILSpy:\r\n" + string.Join(Environment.NewLine, commandLineArguments), true);
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD110:Observe result of async calls", Justification = "<Pending>")]
		void SendMessage(Process ilspyProcess, string message, bool activate)
		{
			// We wait asynchronously until target window can be found and try to find it multiple times
			Task.Run(async () => {
				bool success = false;
				int remainingAttempts = 20;
				do {
					NativeMethods.EnumWindows(
						(hWnd, lParam) => {
							string windowTitle = NativeMethods.GetWindowText(hWnd, 100);
							if (windowTitle.StartsWith("ILSpy", StringComparison.Ordinal)) {
								Debug.WriteLine("Found {0:x4}: {1}", hWnd, windowTitle);
								IntPtr result = Send(hWnd, message);
								Debug.WriteLine("WM_COPYDATA result: {0:x8}", result);
								if (result == (IntPtr)1) {
									if (activate)
										NativeMethods.SetForegroundWindow(hWnd);
									success = true;
									return false; // stop enumeration
								}
							}
							return true; // continue enumeration
						}, IntPtr.Zero);

					// Wait some time before next attempt
					await Task.Delay(500);
					remainingAttempts--;
				} while (!success && (remainingAttempts > 0));
			});
		}

		unsafe static IntPtr Send(IntPtr hWnd, string message)
		{
			const uint SMTO_NORMAL = 0;

			CopyDataStruct lParam;
			lParam.Padding = IntPtr.Zero;
			lParam.Size = message.Length * 2;
			fixed (char* buffer = message) {
				lParam.Buffer = (IntPtr)buffer;
				IntPtr result;
				// SendMessage with 3s timeout (e.g. when the target process is stopped in the debugger)
				if (NativeMethods.SendMessageTimeout(
					hWnd, NativeMethods.WM_COPYDATA, IntPtr.Zero, ref lParam,
					SMTO_NORMAL, 3000, out result) != IntPtr.Zero) {
					return result;
				} else {
					return IntPtr.Zero;
				}
			}
		}
	}
}
