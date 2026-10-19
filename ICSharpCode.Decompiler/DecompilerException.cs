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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
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
		public MetadataFile File { get; }

		public DecompilerException(MetadataModule module, IEntity decompiledEntity,
			Exception innerException, string message = null)
			: base(message ?? GetDefaultMessage(decompiledEntity), innerException)
		{
			this.File = module.MetadataFile;
			this.Module = module;
			this.DecompiledEntity = decompiledEntity;
		}

		public DecompilerException(MetadataFile file, string message, Exception innerException)
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
				throw new ArgumentNullException(nameof(exception));
			string exceptionType = GetTypeName(exception);
			string stacktrace = GetStackTrace(exception);
			while (exception.InnerException != null)
			{
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

		// StackFrame.GetMethod() is annotated RequiresUnreferencedCode because trimmed and Native AOT
		// apps may have no MethodBase for a frame; the null case is handled below, and nothing is
		// reflected on beyond what the runtime hands out for the frames it does describe.
		[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "A null MethodBase is handled by printing the frame through StackFrame.ToString().")]
		static string GetStackTrace(Exception exception)
		{
			// Output stacktrace in custom format (very similar to Exception.StackTrace
			// property on English systems).
			// Include filenames where available, but no paths.
			StackTrace stackTrace = new StackTrace(exception, true);
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < stackTrace.FrameCount; i++)
			{
				StackFrame frame = stackTrace.GetFrame(i);
				MethodBase method = frame.GetMethod();
				if (method == null)
				{
					// Frames without reflection metadata (Native AOT keeps none for most methods): the
					// runtime still names them through StackFrame.ToString(), e.g.
					// "Namespace.Type.Method() + 0x2b at offset 43 in file:line:column <filename unknown>:0:0".
					// Keep the name and drop the offset noise; a frame the runtime cannot name at all
					// ("<null>" on CoreCLR and .NET Framework) is skipped.
					string description = frame.ToString();
					int offsetStart = description.IndexOf(" at offset ", StringComparison.Ordinal);
					if (offsetStart > 0)
						description = description.Substring(0, offsetStart);
					description = description.Trim();
					if (description.Length == 0 || description == "<null>")
						continue;
					if (b.Length > 0)
						b.AppendLine();
					b.Append("   at ").Append(description);
					continue;
				}

				if (b.Length > 0)
					b.AppendLine();

				b.Append("   at ");
				Type declaringType = method.DeclaringType;
				if (declaringType != null)
				{
					b.Append(declaringType.FullName.Replace('+', '.'));
					b.Append('.');
				}
				b.Append(method.Name);
				// output type parameters, if any
				if ((method is MethodInfo) && ((MethodInfo)method).IsGenericMethod)
				{
					Type[] genericArguments = ((MethodInfo)method).GetGenericArguments();
					b.Append('[');
					for (int j = 0; j < genericArguments.Length; j++)
					{
						if (j > 0)
							b.Append(',');
						b.Append(genericArguments[j].Name);
					}
					b.Append(']');
				}

				// output parameters, if any
				b.Append('(');
				ParameterInfo[] parameters = method.GetParameters();
				for (int j = 0; j < parameters.Length; j++)
				{
					if (j > 0)
						b.Append(", ");
					if (parameters[j].ParameterType != null)
					{
						b.Append(parameters[j].ParameterType.Name);
					}
					else
					{
						b.Append('?');
					}
					if (!string.IsNullOrEmpty(parameters[j].Name))
					{
						b.Append(' ');
						b.Append(parameters[j].Name);
					}
				}
				b.Append(')');

				// source location
				if (frame.GetILOffset() >= 0)
				{
					string filename = null;
					try
					{
						string fullpath = frame.GetFileName();
						if (fullpath != null)
							filename = Path.GetFileName(fullpath);
					}
					catch (SecurityException)
					{
						// StackFrame.GetFileName requires PathDiscovery permission
					}
					catch (ArgumentException)
					{
						// Path.GetFileName might throw on paths with invalid chars
					}
					b.Append(" in ");
					if (filename != null)
					{
						b.Append(filename);
						b.Append(":line ");
						b.Append(frame.GetFileLineNumber());
					}
					else
					{
						b.Append("offset ");
						b.Append(frame.GetILOffset());
					}
				}
			}

			return b.ToString();
		}
	}
}