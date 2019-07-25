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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler
{
	/// <summary>
	/// Description of DecompilerException.
	/// </summary>
	public class DecompilerException : Exception, ISerializable
	{
		public string AssemblyName => File.Name;

		public string FileName => File.FileName;

		public IEntity DecompiledEntity { get; }
		public IModule Module { get; }
		public PEFile File { get; }

		public DecompilerException(MetadataModule module, IEntity decompiledEntity, Exception innerException, string message = null)
			: base(message ?? GetDefaultMessage(decompiledEntity), innerException)
		{
			this.File = module.PEFile;
			this.Module = module;
			this.DecompiledEntity = decompiledEntity;
		}

		public DecompilerException(PEFile file, string message, Exception innerException)
			: base(message, innerException)
		{
			this.File = file;
		}

		static string GetDefaultMessage(IEntity entity)
		{
			if (entity == null)
				return "Error decompiling";
			return $"Error decompiling @{MetadataTokens.GetToken(entity.MetadataToken):X8} {entity.FullName}";
		}

		// This constructor is needed for serialization.
		protected DecompilerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override string StackTrace => GetStackTrace(this);

		public override string ToString() => ToString(this);

		string ToString(Exception exception)
		{
			if (exception == null)
				throw new ArgumentNullException("exception");
			string exceptionType = GetTypeName(exception);
			string stacktrace = GetStackTrace(exception);
			while (exception.InnerException != null) {
				exception = exception.InnerException;

				stacktrace = GetStackTrace(exception) + Environment.NewLine
					+ "-- continuing with outer exception (" + exceptionType + ") --" + Environment.NewLine
					+ stacktrace;
				exceptionType = GetTypeName(exception);
			}
			return this.Message + Environment.NewLine
				+ $"in assembly \"{this.FileName}\"" + Environment.NewLine
				+ " ---> " + exceptionType + ": " + exception.Message + Environment.NewLine
				+ stacktrace;
		}

		static string GetTypeName(Exception exception)
		{
			string type = exception.GetType().FullName;
			if (exception is ExternalException || exception is IOException)
				return type + " (" + Marshal.GetHRForException(exception).ToString("x8") + ")";
			else
				return type;
		}

		static string GetStackTrace(Exception exception)
		{
			// Output stacktrace in custom format (very similar to Exception.StackTrace property on English systems).
			// Include filenames where available, but no paths.
			StackTrace stackTrace = new StackTrace(exception, true);
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < stackTrace.FrameCount; i++) {
				StackFrame frame = stackTrace.GetFrame(i);
				MethodBase method = frame.GetMethod();
				if (method == null)
					continue;

				if (b.Length > 0)
					b.AppendLine();

				b.Append("   at ");
				Type declaringType = method.DeclaringType;
				if (declaringType != null) {
					b.Append(declaringType.FullName.Replace('+', '.'));
					b.Append('.');
				}
				b.Append(method.Name);
				// output type parameters, if any
				if ((method is MethodInfo) && ((MethodInfo)method).IsGenericMethod) {
					Type[] genericArguments = ((MethodInfo)method).GetGenericArguments();
					b.Append('[');
					for (int j = 0; j < genericArguments.Length; j++) {
						if (j > 0)
							b.Append(',');
						b.Append(genericArguments[j].Name);
					}
					b.Append(']');
				}

				// output parameters, if any
				b.Append('(');
				ParameterInfo[] parameters = method.GetParameters();
				for (int j = 0; j < parameters.Length; j++) {
					if (j > 0)
						b.Append(", ");
					if (parameters[j].ParameterType != null) {
						b.Append(parameters[j].ParameterType.Name);
					} else {
						b.Append('?');
					}
					if (!string.IsNullOrEmpty(parameters[j].Name)) {
						b.Append(' ');
						b.Append(parameters[j].Name);
					}
				}
				b.Append(')');

				// source location
				if (frame.GetILOffset() >= 0) {
					string filename = null;
					try {
						string fullpath = frame.GetFileName();
						if (fullpath != null)
							filename = Path.GetFileName(fullpath);
					} catch (SecurityException) {
						// StackFrame.GetFileName requires PathDiscovery permission
					} catch (ArgumentException) {
						// Path.GetFileName might throw on paths with invalid chars
					}
					b.Append(" in ");
					if (filename != null) {
						b.Append(filename);
						b.Append(":line ");
						b.Append(frame.GetFileLineNumber());
					} else {
						b.Append("offset ");
						b.Append(frame.GetILOffset());
					}
				}
			}

			return b.ToString();
		}
	}
}