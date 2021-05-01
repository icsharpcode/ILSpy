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
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;

using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Util;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.Disassembler
{
	public enum ILNameSyntax
	{
		/// <summary>
		/// class/valuetype + TypeName (built-in types use keyword syntax)
		/// </summary>
		Signature,
		/// <summary>
		/// Like signature, but always refers to type parameters using their position
		/// </summary>
		SignatureNoNamedTypeParameters,
		/// <summary>
		/// [assembly]Full.Type.Name (even for built-in types)
		/// </summary>
		TypeName,
		/// <summary>
		/// Name (but built-in types use keyword syntax)
		/// </summary>
		ShortTypeName
	}

	public static class DisassemblerHelpers
	{
		static readonly char[] _validNonLetterIdentifierCharacter = new char[] { '_', '$', '@', '?', '`', '.' };

		public static string OffsetToString(int offset)
		{
			return string.Format("IL_{0:x4}", offset);
		}

		public static string OffsetToString(long offset)
		{
			return string.Format("IL_{0:x4}", offset);
		}

		public static void WriteOffsetReference(ITextOutput writer, int? offset)
		{
			if (offset == null)
				writer.Write("null");
			else
				writer.WriteLocalReference(OffsetToString(offset.Value), offset);
		}

		public static void WriteTo(this SRM.ExceptionRegion exceptionHandler, Metadata.PEFile module, GenericContext context, ITextOutput writer)
		{
			writer.Write(".try ");
			WriteOffsetReference(writer, exceptionHandler.TryOffset);
			writer.Write('-');
			WriteOffsetReference(writer, exceptionHandler.TryOffset + exceptionHandler.TryLength);
			writer.Write(' ');
			writer.Write(exceptionHandler.Kind.ToString().ToLowerInvariant());
			if (exceptionHandler.FilterOffset != -1)
			{
				writer.Write(' ');
				WriteOffsetReference(writer, exceptionHandler.FilterOffset);
				writer.Write(" handler ");
			}
			if (!exceptionHandler.CatchType.IsNil)
			{
				writer.Write(' ');
				exceptionHandler.CatchType.WriteTo(module, writer, context);
			}
			writer.Write(' ');
			WriteOffsetReference(writer, exceptionHandler.HandlerOffset);
			writer.Write('-');
			WriteOffsetReference(writer, exceptionHandler.HandlerOffset + exceptionHandler.HandlerLength);
		}

		static string ToInvariantCultureString(object value)
		{
			IConvertible convertible = value as IConvertible;
			return (null != convertible)
				? convertible.ToString(System.Globalization.CultureInfo.InvariantCulture)
				: value.ToString();
		}

		static bool IsValidIdentifierCharacter(char c)
			=> char.IsLetterOrDigit(c) || _validNonLetterIdentifierCharacter.IndexOf(c) >= 0;

		static bool IsValidIdentifier(string identifier)
		{
			if (string.IsNullOrEmpty(identifier))
				return false;

			if (char.IsDigit(identifier[0]))
				return false;

			// As a special case, .ctor and .cctor are valid despite starting with a dot
			if (identifier[0] == '.')
				return identifier == ".ctor" || identifier == ".cctor";

			if (identifier.Contains(".."))
				return false;

			if (Metadata.ILOpCodeExtensions.ILKeywords.Contains(identifier))
				return false;

			return identifier.All(IsValidIdentifierCharacter);
		}

		public static string Escape(string identifier)
		{
			if (IsValidIdentifier(identifier))
			{
				return identifier;
			}

			// The ECMA specification says that ' inside SQString should be ecaped using an octal escape sequence,
			// but we follow Microsoft's ILDasm and use \'.
			return $"'{EscapeString(identifier).Replace("'", "\\'")}'";
		}

		public static void WriteParameterReference(ITextOutput writer, MetadataReader metadata, MethodDefinitionHandle handle, int sequence)
		{
			var methodDefinition = metadata.GetMethodDefinition(handle);
			var signature = methodDefinition.DecodeSignature(new FullTypeNameSignatureDecoder(metadata), default);
			var parameters = methodDefinition.GetParameters().Select(p => metadata.GetParameter(p)).ToArray();
			var signatureHeader = signature.Header;
			int index = sequence;
			if (signatureHeader.IsInstance && signature.ParameterTypes.Length == parameters.Length)
			{
				index--;
			}
			if (index < 0 || index >= parameters.Length)
			{
				writer.WriteLocalReference(sequence.ToString(), "param_" + index);
			}
			else
			{
				var param = parameters[index];
				if (param.Name.IsNil)
				{
					writer.WriteLocalReference(sequence.ToString(), "param_" + index);
				}
				else
				{
					writer.WriteLocalReference(Escape(metadata.GetString(param.Name)), "param_" + index);
				}
			}
		}

		public static void WriteVariableReference(ITextOutput writer, MetadataReader metadata, MethodDefinitionHandle handle, int index)
		{
			writer.WriteLocalReference(index.ToString(), "loc_" + index);
		}

		public static void WriteOperand(ITextOutput writer, object operand)
		{
			if (operand == null)
				throw new ArgumentNullException(nameof(operand));

			string s = operand as string;
			if (s != null)
			{
				WriteOperand(writer, s);
			}
			else if (operand is char)
			{
				writer.Write(((int)(char)operand).ToString());
			}
			else if (operand is float)
			{
				WriteOperand(writer, (float)operand);
			}
			else if (operand is double)
			{
				WriteOperand(writer, (double)operand);
			}
			else if (operand is bool)
			{
				writer.Write((bool)operand ? "true" : "false");
			}
			else
			{
				s = ToInvariantCultureString(operand);
				writer.Write(s);
			}
		}

		public static void WriteOperand(ITextOutput writer, long val)
		{
			writer.Write(ToInvariantCultureString(val));
		}

		public static void WriteOperand(ITextOutput writer, float val)
		{
			if (val == 0)
			{
				if (1 / val == float.NegativeInfinity)
				{
					// negative zero is a special case
					writer.Write('-');
				}
				writer.Write("0.0");
			}
			else if (float.IsInfinity(val) || float.IsNaN(val))
			{
				byte[] data = BitConverter.GetBytes(val);
				writer.Write('(');
				for (int i = 0; i < data.Length; i++)
				{
					if (i > 0)
						writer.Write(' ');
					writer.Write(data[i].ToString("X2"));
				}
				writer.Write(')');
			}
			else
			{
				writer.Write(val.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
			}
		}

		public static void WriteOperand(ITextOutput writer, double val)
		{
			if (val == 0)
			{
				if (1 / val == double.NegativeInfinity)
				{
					// negative zero is a special case
					writer.Write('-');
				}
				writer.Write("0.0");
			}
			else if (double.IsInfinity(val) || double.IsNaN(val))
			{
				byte[] data = BitConverter.GetBytes(val);
				writer.Write('(');
				for (int i = 0; i < data.Length; i++)
				{
					if (i > 0)
						writer.Write(' ');
					writer.Write(data[i].ToString("X2"));
				}
				writer.Write(')');
			}
			else
			{
				writer.Write(val.ToString("R", System.Globalization.CultureInfo.InvariantCulture));
			}
		}

		public static void WriteOperand(ITextOutput writer, string operand)
		{
			writer.Write('"');
			writer.Write(EscapeString(operand));
			writer.Write('"');
		}

		public static string EscapeString(string str)
		{
			var sb = new StringBuilder();
			foreach (char ch in str)
			{
				switch (ch)
				{
					case '"':
						sb.Append("\\\"");
						break;
					case '\\':
						sb.Append("\\\\");
						break;
					case '\0':
						sb.Append("\\0");
						break;
					case '\a':
						sb.Append("\\a");
						break;
					case '\b':
						sb.Append("\\b");
						break;
					case '\f':
						sb.Append("\\f");
						break;
					case '\n':
						sb.Append("\\n");
						break;
					case '\r':
						sb.Append("\\r");
						break;
					case '\t':
						sb.Append("\\t");
						break;
					case '\v':
						sb.Append("\\v");
						break;
					default:
						// print control characters and uncommon white spaces as numbers
						if (char.IsControl(ch) || char.IsSurrogate(ch) || (char.IsWhiteSpace(ch) && ch != ' '))
						{
							sb.AppendFormat("\\u{0:x4}", (int)ch);
						}
						else
						{
							sb.Append(ch);
						}
						break;
				}
			}
			return sb.ToString();
		}
		public static string PrimitiveTypeName(string fullName)
		{
			switch (fullName)
			{
				case "System.SByte":
					return "int8";
				case "System.Int16":
					return "int16";
				case "System.Int32":
					return "int32";
				case "System.Int64":
					return "int64";
				case "System.Byte":
					return "uint8";
				case "System.UInt16":
					return "uint16";
				case "System.UInt32":
					return "uint32";
				case "System.UInt64":
					return "uint64";
				case "System.Single":
					return "float32";
				case "System.Double":
					return "float64";
				case "System.Void":
					return "void";
				case "System.Boolean":
					return "bool";
				case "System.String":
					return "string";
				case "System.Char":
					return "char";
				case "System.Object":
					return "object";
				case "System.IntPtr":
					return "native int";
				default:
					return null;
			}
		}
	}
}
