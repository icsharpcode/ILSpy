// Copyright (c) 2014 Daniel Grunwald
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

using System.Collections.Generic;
using System.Reflection.Metadata;

namespace ICSharpCode.Decompiler.Metadata
{
	public enum OperandType
	{
		BrTarget,
		Field,
		I,
		I8,
		Method,
		None,
		R = 7,
		Sig = 9,
		String,
		Switch,
		Tok,
		Type,
		Variable,
		ShortBrTarget,
		ShortI,
		ShortR,
		ShortVariable
	}

	public static partial class ILOpCodeExtensions
	{
		public static OperandType GetOperandType(this ILOpCode opCode)
		{
			ushort index = (ushort)((((int)opCode & 0x200) >> 1) | ((int)opCode & 0xff));
			if (index >= operandTypes.Length)
				return (OperandType)255;
			return (OperandType)operandTypes[index];
		}

		public static string GetDisplayName(this ILOpCode opCode)
		{
			ushort index = (ushort)((((int)opCode & 0x200) >> 1) | ((int)opCode & 0xff));
			if (index >= operandNames.Length)
				return "";
			return operandNames[index];
		}

		public static bool IsDefined(this ILOpCode opCode)
		{
			return !string.IsNullOrEmpty(GetDisplayName(opCode));
		}

		static ILOpCodeExtensions()
		{
			ILKeywords = BuildKeywordList(
				"abstract", "algorithm", "alignment", "ansi", "any", "arglist",
				"array", "as", "assembly", "assert", "at", "auto", "autochar", "beforefieldinit",
				"blob", "blob_object", "bool", "brnull", "brnull.s", "brzero", "brzero.s", "bstr",
				"bytearray", "byvalstr", "callmostderived", "carray", "catch", "cdecl", "cf",
				"char", "cil", "class", "clsid", "const", "currency", "custom", "date", "decimal",
				"default", "demand", "deny", "endmac", "enum", "error", "explicit", "extends", "extern",
				"false", "famandassem", "family", "famorassem", "fastcall", "fault", "field", "filetime",
				"filter", "final", "finally", "fixed", "float", "float32", "float64", "forwardref",
				"fromunmanaged", "handler", "hidebysig", "hresult", "idispatch", "il", "illegal",
				"implements", "implicitcom", "implicitres", "import", "in", "inheritcheck", "init",
				"initonly", "instance", "int", "int16", "int32", "int64", "int8", "interface", "internalcall",
				"iunknown", "lasterr", "lcid", "linkcheck", "literal", "localloc", "lpstr", "lpstruct", "lptstr",
				"lpvoid", "lpwstr", "managed", "marshal", "method", "modopt", "modreq", "native", "nested",
				"newslot", "noappdomain", "noinlining", "nomachine", "nomangle", "nometadata", "noncasdemand",
				"noncasinheritance", "noncaslinkdemand", "noprocess", "not", "not_in_gc_heap", "notremotable",
				"notserialized", "null", "nullref", "object", "objectref", "opt", "optil", "out",
				"permitonly", "pinned", "pinvokeimpl", "prefix1", "prefix2", "prefix3", "prefix4", "prefix5", "prefix6",
				"prefix7", "prefixref", "prejitdeny", "prejitgrant", "preservesig", "private", "privatescope", "protected",
				"public", "record", "refany", "reqmin", "reqopt", "reqrefuse", "reqsecobj", "request", "retval",
				"rtspecialname", "runtime", "safearray", "sealed", "sequential", "serializable", "special", "specialname",
				"static", "stdcall", "storage", "stored_object", "stream", "streamed_object", "string", "struct",
				"synchronized", "syschar", "sysstring", "tbstr", "thiscall", "tls", "to", "true", "typedref",
				"unicode", "unmanaged", "unmanagedexp", "unsigned", "unused", "userdefined", "value", "valuetype",
				"vararg", "variant", "vector", "virtual", "void", "wchar", "winapi", "with", "wrapper",

				// These are not listed as keywords in spec, but ILAsm treats them as such
				"property", "type", "flags", "callconv", "strict",
				// ILDasm uses these keywords for unsigned integers
				"uint8", "uint16", "uint32", "uint64"
			);
		}

		public static readonly HashSet<string> ILKeywords;

		static HashSet<string> BuildKeywordList(params string[] keywords)
		{
			HashSet<string> s = new HashSet<string>(keywords);
			foreach (var inst in operandNames) {
				if (string.IsNullOrEmpty(inst))
					continue;
				s.Add(inst);
			}
			return s;
		}
	}
}
