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
using System.Reflection.Metadata.Ecma335;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler.Disassembler
{
	public struct OpCodeInfo : IEquatable<OpCodeInfo>
	{
		public readonly ILOpCode Code;
		public readonly string Name;

		string encodedName;

		public OpCodeInfo(ILOpCode code, string name)
		{
			this.Code = code;
			this.Name = name ?? "";
			this.encodedName = null;
		}

		public bool Equals(OpCodeInfo other)
		{
			return other.Code == this.Code && other.Name == this.Name;
		}

		public static bool operator ==(OpCodeInfo lhs, OpCodeInfo rhs) => lhs.Equals(rhs);
		public static bool operator !=(OpCodeInfo lhs, OpCodeInfo rhs) => !(lhs == rhs);

		public override bool Equals(object obj)
		{
			if (obj is OpCodeInfo opCode)
				return Equals(opCode);
			return false;
		}

		public override int GetHashCode()
		{
			return unchecked(982451629 * Code.GetHashCode() + 982451653 * Name.GetHashCode());
		}

		public string Link => "https://docs.microsoft.com/dotnet/api/system.reflection.emit.opcodes." + EncodedName.ToLowerInvariant();
		public string EncodedName {
			get {
				if (encodedName != null)
					return encodedName;
				switch (Name) {
					case "constrained.":
						encodedName = "Constrained";
						return encodedName;
					case "no.":
						encodedName = "No";
						return encodedName;
					case "readonly.":
						encodedName = "Reaonly";
						return encodedName;
					case "tail.":
						encodedName = "Tailcall";
						return encodedName;
					case "unaligned.":
						encodedName = "Unaligned";
						return encodedName;
					case "volatile.":
						encodedName = "Volatile";
						return encodedName;
				}
				string text = "";
				bool toUpperCase = true;
				foreach (var ch in Name) {
					if (ch == '.') {
						text += '_';
						toUpperCase = true;
					} else if (toUpperCase) {
						text += char.ToUpperInvariant(ch);
						toUpperCase = false;
					} else {
						text += ch;
					}
				}
				encodedName = text;
				return encodedName;
			}
		}
	}
}
