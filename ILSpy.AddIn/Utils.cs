using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace ICSharpCode.ILSpy.AddIn
{
	public enum MessageButtonResult : int
	{
		IDOK = 1,
		IDCANCEL = 2,
		IDABORT = 3,
		IDRETRY = 4,
		IDIGNORE = 5,
		IDYES = 6,
		IDNO = 7,
		IDTRYAGAIN = 10,
		IDCONTINUE = 11,
	}

	static class Utils
	{

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

		public static byte[] HexStringToBytes(string hex)
		{
			if (hex == null)
				throw new ArgumentNullException(nameof(hex));
			var result = new byte[hex.Length / 2];
			for (int i = 0; i < hex.Length / 2; i++) {
				result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
			}
			return result;
		}

		public static bool TryGetProjectFileName(dynamic referenceObject, out string fileName)
		{
			try {
				fileName = referenceObject.Project.FileName;
				return true;
			} catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException) {
				fileName = null;
				return false;
			}
		}

		public static object[] GetProperties(EnvDTE.Properties properties, params string[] names)
		{
			var values = new object[names.Length];
			foreach (object p in properties) {
				try {
					if (p is Property property) {
						for (int i = 0; i < names.Length; i++) {
							if (names[i] == property.Name) {
								values[i] = property.Value;
								break;
							}
						}
					}
				} catch {
					continue;
				}
			}
			return values;
		}

		public static List<(string, object)> GetAllProperties(EnvDTE.Properties properties)
		{
			var result = new List<(string, object)>();
			for (int i = 0; i < properties.Count; i++) {
				try {
					if (properties.Item(i) is Property p) {
						result.Add((p.Name, p.Value));
					}
				} catch {
					continue;
				}
			}

			return result;
		}

		public static ITextSelection GetSelectionInCurrentView(IServiceProvider serviceProvider, Func<string, bool> predicate)
		{
			IWpfTextViewHost viewHost = GetCurrentViewHost(serviceProvider, predicate);
			if (viewHost == null)
				return null;

			return viewHost.TextView.Selection;
		}

		public static IWpfTextViewHost GetCurrentViewHost(IServiceProvider serviceProvider, Func<string, bool> predicate)
		{
			IWpfTextViewHost viewHost = GetCurrentViewHost(serviceProvider);
			if (viewHost == null)
				return null;

			ITextDocument textDocument = viewHost.GetTextDocument();
			if (textDocument == null || !predicate(textDocument.FilePath))
				return null;

			return viewHost;
		}

		public static IWpfTextViewHost GetCurrentViewHost(IServiceProvider serviceProvider)
		{
			IVsTextManager txtMgr = (IVsTextManager)serviceProvider.GetService(typeof(SVsTextManager));
			IVsTextView vTextView = null;
			int mustHaveFocus = 1;
			txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);
			IVsUserData userData = vTextView as IVsUserData;
			if (userData == null)
				return null;

			object holder;
			Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
			userData.GetData(ref guidViewHost, out holder);

			return holder as IWpfTextViewHost;
		}

		public static ITextDocument GetTextDocument(this IWpfTextViewHost viewHost)
		{
			ITextDocument textDocument = null;
			viewHost.TextView.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out textDocument);
			return textDocument;
		}

		public static VisualStudioWorkspace GetWorkspace(IServiceProvider serviceProvider)
		{
			return (VisualStudioWorkspace)serviceProvider.GetService(typeof(VisualStudioWorkspace));
		}

		public static string GetProjectOutputAssembly(Project project, Microsoft.CodeAnalysis.Project roslynProject)
		{
			string outputFileName = Path.GetFileName(roslynProject.OutputFilePath);

			// Get the directory path based on the project file.
			string projectPath = Path.GetDirectoryName(project.FullName);

			// Get the output path based on the active configuration
			string projectOutputPath = project.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();

			// Combine the project path and output path to get the bin path
			if ((projectPath != null) && (projectOutputPath != null) && (outputFileName != null))
				return Path.Combine(projectPath, projectOutputPath, outputFileName);

			return null;
		}
	}
}
