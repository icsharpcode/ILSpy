// Copyright (c) 2018 Daniel Grunwald
//   This file is based on the Mono implementation of ResXResourceWriter.
//   It is modified to add support for "ResourceSerializedObject" values.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// Copyright (c) 2004-2005 Novell, Inc.
//
// Authors:
//	Duncan Mak		duncan@ximian.com
//	Gonzalo Paniagua Javier	gonzalo@ximian.com
//	Peter Bartok		pbartok@novell.com
//	Gary Barnett		gary.barnett.mono@gmail.com
//	includes code by Mike Krüger and Lluis Sanchez

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.Util
{
#if INSIDE_SYSTEM_WEB
	internal
#else
	public
#endif
	class ResXResourceWriter : IDisposable
	{
		private string filename;
		private Stream stream;
		private TextWriter textwriter;
		private XmlTextWriter writer;
		private bool written;
		private string base_path;

		public static readonly string BinSerializedObjectMimeType = "application/x-microsoft.net.object.binary.base64";
		public static readonly string ByteArraySerializedObjectMimeType = "application/x-microsoft.net.object.bytearray.base64";
		public static readonly string DefaultSerializedObjectMimeType = BinSerializedObjectMimeType;
		public static readonly string ResMimeType = "text/microsoft-resx";
		public static readonly string SoapSerializedObjectMimeType = "application/x-microsoft.net.object.soap.base64";
		public static readonly string Version = "2.0";

		public ResXResourceWriter(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			if (!stream.CanWrite)
				throw new ArgumentException("stream is not writable.", nameof(stream));

			this.stream = stream;
		}

		public ResXResourceWriter(TextWriter textWriter)
		{
			if (textWriter == null)
				throw new ArgumentNullException(nameof(textWriter));

			this.textwriter = textWriter;
		}

		public ResXResourceWriter(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException(nameof(fileName));

			this.filename = fileName;
		}

		~ResXResourceWriter()
		{
			Dispose(false);
		}

		const string WinFormsAssemblyName = ", System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
		const string MSCorLibAssemblyName = ", mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
		const string ResXNullRefTypeName = "System.Resources.ResXNullRef" + WinFormsAssemblyName;

		void InitWriter()
		{
			if (filename != null)
				stream = File.Open(filename, FileMode.Create);
			if (textwriter == null)
				textwriter = new StreamWriter(stream, Encoding.UTF8);

			writer = new XmlTextWriter(textwriter);
			writer.Formatting = Formatting.Indented;
			writer.WriteStartDocument();
			writer.WriteStartElement("root");
			writer.WriteRaw(schema);
			WriteHeader("resmimetype", "text/microsoft-resx");
			WriteHeader("version", "1.3");
			WriteHeader("reader", "System.Resources.ResXResourceReader" + WinFormsAssemblyName);
			WriteHeader("writer", "System.Resources.ResXResourceWriter" + WinFormsAssemblyName);
		}

		void WriteHeader(string name, string value)
		{
			writer.WriteStartElement("resheader");
			writer.WriteAttributeString("name", name);
			writer.WriteStartElement("value");
			writer.WriteString(value);
			writer.WriteEndElement();
			writer.WriteEndElement();
		}

		void WriteNiceBase64(byte[] value, int offset, int length)
		{
			string base64 = Convert.ToBase64String(
				value, offset, length,
				Base64FormattingOptions.InsertLineBreaks);
			writer.WriteString(base64);
		}

		void WriteBytes(string name, string type, byte[] value, int offset, int length, string comment)
		{
			writer.WriteStartElement("data");
			writer.WriteAttributeString("name", name);

			if (type != null)
			{
				writer.WriteAttributeString("type", type);
				// byte[] should never get a mimetype, otherwise MS.NET won't be able
				// to parse the data.
				if (type != "System.Byte[]" + MSCorLibAssemblyName)
					writer.WriteAttributeString("mimetype", ByteArraySerializedObjectMimeType);
				writer.WriteStartElement("value");
				WriteNiceBase64(value, offset, length);
			}
			else
			{
				writer.WriteAttributeString("mimetype", BinSerializedObjectMimeType);
				writer.WriteStartElement("value");
				WriteNiceBase64(value, offset, length);
			}

			writer.WriteEndElement();

			if (!string.IsNullOrEmpty(comment))
			{
				writer.WriteStartElement("comment");
				writer.WriteString(comment);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		void WriteString(string name, string value, string type, string comment)
		{
			writer.WriteStartElement("data");
			writer.WriteAttributeString("name", name);
			if (type != null)
			{
				writer.WriteAttributeString("type", type);
			}
			else
			{
				writer.WriteAttributeString("xml:space", "preserve");
			}
			writer.WriteStartElement("value");
			writer.WriteString(value);
			writer.WriteEndElement();
			if (!string.IsNullOrEmpty(comment))
			{
				writer.WriteStartElement("comment");
				writer.WriteString(comment);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
			writer.WriteWhitespace("\n  ");
		}

		public void AddResource(string name, byte[] value)
		{
			AddResource(name, value, string.Empty);
		}

		public void AddResource(string name, object value)
		{
			AddResource(name, value, string.Empty);
		}

		private void AddResource(string name, object value, string comment)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));

			if (written)
				throw new InvalidOperationException("The resource is already generated.");

			if (writer == null)
				InitWriter();

			switch (value)
			{
				case null:
					// nulls written as ResXNullRef
					WriteString(name, "", ResXNullRefTypeName, comment);
					break;
				case string s:
					WriteString(name, s, null, comment);
					break;
				case bool bo:
					WriteString(name, bo.ToString(CultureInfo.InvariantCulture), "System.Boolean" + MSCorLibAssemblyName, comment);
					break;
				case char ch:
					WriteString(name, ch.ToString(CultureInfo.InvariantCulture), "System.Char" + MSCorLibAssemblyName, comment);
					break;
				case sbyte sb:
					WriteString(name, sb.ToString(CultureInfo.InvariantCulture), "System.SByte" + MSCorLibAssemblyName, comment);
					break;
				case byte b:
					WriteString(name, b.ToString(CultureInfo.InvariantCulture), "System.Byte" + MSCorLibAssemblyName, comment);
					break;
				case short sh:
					WriteString(name, sh.ToString(CultureInfo.InvariantCulture), "System.Int16" + MSCorLibAssemblyName, comment);
					break;
				case ushort ush:
					WriteString(name, ush.ToString(CultureInfo.InvariantCulture), "System.UInt16" + MSCorLibAssemblyName, comment);
					break;
				case int i:
					WriteString(name, i.ToString(CultureInfo.InvariantCulture), "System.Int32" + MSCorLibAssemblyName, comment);
					break;
				case uint u:
					WriteString(name, u.ToString(CultureInfo.InvariantCulture), "System.UInt32" + MSCorLibAssemblyName, comment);
					break;
				case long l:
					WriteString(name, l.ToString(CultureInfo.InvariantCulture), "System.Int64" + MSCorLibAssemblyName, comment);
					break;
				case ulong ul:
					WriteString(name, ul.ToString(CultureInfo.InvariantCulture), "System.UInt64" + MSCorLibAssemblyName, comment);
					break;
				case float f:
					WriteString(name, f.ToString(CultureInfo.InvariantCulture), "System.Single" + MSCorLibAssemblyName, comment);
					break;
				case double d:
					WriteString(name, d.ToString(CultureInfo.InvariantCulture), "System.Double" + MSCorLibAssemblyName, comment);
					break;
				case decimal m:
					WriteString(name, m.ToString(CultureInfo.InvariantCulture), "System.Decimal" + MSCorLibAssemblyName, comment);
					break;
				case DateTime dt:
					WriteString(name, dt.ToString(CultureInfo.InvariantCulture), "System.DateTime" + MSCorLibAssemblyName, comment);
					break;
				case TimeSpan sp:
					WriteString(name, sp.ToString(), "System.TimeSpan" + MSCorLibAssemblyName, comment);
					break;
				case byte[] array:
					WriteBytes(name, "System.Byte[]" + MSCorLibAssemblyName, array, 0, array.Length, comment);
					break;
				case MemoryStream memoryStream:
					var arr = memoryStream.ToArray();
					WriteBytes(name, null, arr, 0, arr.Length, comment);
					break;
				case ResourceSerializedObject rso:
					var bytes = rso.GetBytes();
					WriteBytes(name, rso.TypeName, bytes, 0, bytes.Length, comment);
					break;
				default:
					throw new NotSupportedException($"Value '{value}' of type {value.GetType().FullName} is not supported by this version of ResXResourceWriter. Use byte arrays or streams instead.");
			}
		}

		public void AddResource(string name, string value)
		{
			AddResource(name, value, string.Empty);
		}

		public void Close()
		{
			if (writer != null)
			{
				if (!written)
				{
					Generate();
				}

				writer.Close();
				stream = null;
				filename = null;
				textwriter = null;
			}
		}

		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Generate()
		{
			if (written)
				throw new InvalidOperationException("The resource is already generated.");

			written = true;
			writer.WriteEndElement();
			writer.Flush();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				Close();
		}

		static readonly string schema = @"
	<xsd:schema id='root' xmlns='' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:msdata='urn:schemas-microsoft-com:xml-msdata'>
		<xsd:element name='root' msdata:IsDataSet='true'>
			<xsd:complexType>
				<xsd:choice maxOccurs='unbounded'>
					<xsd:element name='data'>
						<xsd:complexType>
							<xsd:sequence>
								<xsd:element name='value' type='xsd:string' minOccurs='0' msdata:Ordinal='1' />
								<xsd:element name='comment' type='xsd:string' minOccurs='0' msdata:Ordinal='2' />
							</xsd:sequence>
							<xsd:attribute name='name' type='xsd:string' msdata:Ordinal='1' />
							<xsd:attribute name='type' type='xsd:string' msdata:Ordinal='3' />
							<xsd:attribute name='mimetype' type='xsd:string' msdata:Ordinal='4' />
						</xsd:complexType>
					</xsd:element>
					<xsd:element name='resheader'>
						<xsd:complexType>
							<xsd:sequence>
								<xsd:element name='value' type='xsd:string' minOccurs='0' msdata:Ordinal='1' />
							</xsd:sequence>
							<xsd:attribute name='name' type='xsd:string' use='required' />
						</xsd:complexType>
					</xsd:element>
				</xsd:choice>
			</xsd:complexType>
		</xsd:element>
	</xsd:schema>
".Replace("'", "\"").Replace("\t", "  ");

		public string BasePath {
			get { return base_path; }
			set { base_path = value; }
		}
	}
}
