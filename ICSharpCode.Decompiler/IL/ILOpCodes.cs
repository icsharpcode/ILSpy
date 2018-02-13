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

using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
namespace ICSharpCode.Decompiler.IL
{
	/// <summary>Describes the operand type of Microsoft intermediate language (MSIL) instruction.</summary>
	public enum OperandType
	{
		/// <summary>The operand is a 32-bit integer branch target.</summary>
		BrTarget,
		/// <summary>The operand is a 32-bit metadata token.</summary>
		Field,
		/// <summary>The operand is a 32-bit integer.</summary>
		I,
		/// <summary>The operand is a 64-bit integer.</summary>
		I8,
		/// <summary>The operand is a 32-bit metadata token.</summary>
		Method,
		/// <summary>No operand.</summary>
		None,
		/// <summary>The operand is reserved and should not be used.</summary>
		[Obsolete("This API has been deprecated. http://go.microsoft.com/fwlink/?linkid=14202")]
		Phi,
		/// <summary>The operand is a 64-bit IEEE floating point number.</summary>
		R,
		/// <summary>The operand is a 32-bit metadata signature token.</summary>
		Sig = 9,
		/// <summary>The operand is a 32-bit metadata string token.</summary>
		String,
		/// <summary>The operand is the 32-bit integer argument to a switch instruction.</summary>
		Switch,
		/// <summary>The operand is a <see langword="FieldRef" />, <see langword="MethodRef" />, or <see langword="TypeRef" /> token.</summary>
		Tok,
		/// <summary>The operand is a 32-bit metadata token.</summary>
		Type,
		/// <summary>The operand is 16-bit integer containing the ordinal of a local variable or an argument.</summary>
		Variable,
		/// <summary>The operand is an 8-bit integer branch target.</summary>
		ShortBrTarget,
		/// <summary>The operand is an 8-bit integer.</summary>
		ShortI,
		/// <summary>The operand is a 32-bit IEEE floating point number.</summary>
		ShortR,
		/// <summary>The operand is an 8-bit integer containing the ordinal of a local variable or an argumenta.</summary>
		ShortVariable
	}


	public static class ILOpCodeExtensions
	{
		// We use a byte array instead of an enum array because it can be initialized more efficiently
		static readonly byte[] operandTypes = { (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.ShortVariable, (byte)OperandType.ShortVariable, (byte)OperandType.ShortVariable, (byte)OperandType.ShortVariable, (byte)OperandType.ShortVariable, (byte)OperandType.ShortVariable, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.ShortI, (byte)OperandType.I, (byte)OperandType.I8, (byte)OperandType.ShortR, (byte)OperandType.R, 255, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.Method, (byte)OperandType.Method, (byte)OperandType.Sig, (byte)OperandType.None, (byte)OperandType.ShortBrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.BrTarget, (byte)OperandType.BrTarget, (byte)OperandType.BrTarget, (byte)OperandType.BrTarget, (byte)OperandType.BrTarget, (byte)OperandType.BrTarget, (byte)OperandType.BrTarget, (byte)OperandType.BrTarget, (byte)OperandType.BrTarget, (byte)OperandType.BrTarget, (byte)OperandType.BrTarget, (byte)OperandType.BrTarget, (byte)OperandType.BrTarget, (byte)OperandType.Switch, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.Method, (byte)OperandType.Type, (byte)OperandType.Type, (byte)OperandType.String, (byte)OperandType.Method, (byte)OperandType.Type, (byte)OperandType.Type, (byte)OperandType.None, 255, 255, (byte)OperandType.Type, (byte)OperandType.None, (byte)OperandType.Field, (byte)OperandType.Field, (byte)OperandType.Field, (byte)OperandType.Field, (byte)OperandType.Field, (byte)OperandType.Field, (byte)OperandType.Type, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.Type, (byte)OperandType.Type, (byte)OperandType.None, (byte)OperandType.Type, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.Type, (byte)OperandType.Type, (byte)OperandType.Type, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, 255, 255, 255, 255, 255, 255, 255, (byte)OperandType.Type, (byte)OperandType.None, 255, 255, (byte)OperandType.Type, 255, 255, 255, 255, 255, 255, 255, 255, 255, (byte)OperandType.Tok, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.BrTarget, (byte)OperandType.ShortBrTarget, (byte)OperandType.None, (byte)OperandType.None, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.Method, (byte)OperandType.Method, 255, (byte)OperandType.Variable, (byte)OperandType.Variable, (byte)OperandType.Variable, (byte)OperandType.Variable, (byte)OperandType.Variable, (byte)OperandType.Variable, (byte)OperandType.None, 255, (byte)OperandType.None, (byte)OperandType.ShortI, (byte)OperandType.None, (byte)OperandType.None, (byte)OperandType.Type, (byte)OperandType.Type, (byte)OperandType.None, (byte)OperandType.None, 255, (byte)OperandType.None, 255, (byte)OperandType.Type, (byte)OperandType.None, (byte)OperandType.None,  };

		static readonly string[] operandNames = { "nop", "break", "ldarg.0", "ldarg.1", "ldarg.2", "ldarg.3", "ldloc.0", "ldloc.1", "ldloc.2", "ldloc.3", "stloc.0", "stloc.1", "stloc.2", "stloc.3", "ldarg.s", "ldarga.s", "starg.s", "ldloc.s", "ldloca.s", "stloc.s", "ldnull", "ldc.i4.m1", "ldc.i4.0", "ldc.i4.1", "ldc.i4.2", "ldc.i4.3", "ldc.i4.4", "ldc.i4.5", "ldc.i4.6", "ldc.i4.7", "ldc.i4.8", "ldc.i4.s", "ldc.i4", "ldc.i8", "ldc.r4", "ldc.r8", "", "dup", "pop", "jmp", "call", "calli", "ret", "br.s", "brfalse.s", "brtrue.s", "beq.s", "bge.s", "bgt.s", "ble.s", "blt.s", "bne.un.s", "bge.un.s", "bgt.un.s", "ble.un.s", "blt.un.s", "br", "brfalse", "brtrue", "beq", "bge", "bgt", "ble", "blt", "bne.un", "bge.un", "bgt.un", "ble.un", "blt.un", "switch", "ldind.i1", "ldind.u1", "ldind.i2", "ldind.u2", "ldind.i4", "ldind.u4", "ldind.i8", "ldind.i", "ldind.r4", "ldind.r8", "ldind.ref", "stind.ref", "stind.i1", "stind.i2", "stind.i4", "stind.i8", "stind.r4", "stind.r8", "add", "sub", "mul", "div", "div.un", "rem", "rem.un", "and", "or", "xor", "shl", "shr", "shr.un", "neg", "not", "conv.i1", "conv.i2", "conv.i4", "conv.i8", "conv.r4", "conv.r8", "conv.u4", "conv.u8", "callvirt", "cpobj", "ldobj", "ldstr", "newobj", "castclass", "isinst", "conv.r.un", "", "", "unbox", "throw", "ldfld", "ldflda", "stfld", "ldsfld", "ldsflda", "stsfld", "stobj", "conv.ovf.i1.un", "conv.ovf.i2.un", "conv.ovf.i4.un", "conv.ovf.i8.un", "conv.ovf.u1.un", "conv.ovf.u2.un", "conv.ovf.u4.un", "conv.ovf.u8.un", "conv.ovf.i.un", "conv.ovf.u.un", "box", "newarr", "ldlen", "ldelema", "ldelem.i1", "ldelem.u1", "ldelem.i2", "ldelem.u2", "ldelem.i4", "ldelem.u4", "ldelem.i8", "ldelem.i", "ldelem.r4", "ldelem.r8", "ldelem.ref", "stelem.i", "stelem.i1", "stelem.i2", "stelem.i4", "stelem.i8", "stelem.r4", "stelem.r8", "stelem.ref", "ldelem", "stelem", "unbox.any", "", "", "", "", "", "", "", "", "", "", "", "", "", "conv.ovf.i1", "conv.ovf.u1", "conv.ovf.i2", "conv.ovf.u2", "conv.ovf.i4", "conv.ovf.u4", "conv.ovf.i8", "conv.ovf.u8", "", "", "", "", "", "", "", "refanyval", "ckfinite", "", "", "mkrefany", "", "", "", "", "", "", "", "", "", "ldtoken", "conv.u2", "conv.u1", "conv.i", "conv.ovf.i", "conv.ovf.u", "add.ovf", "add.ovf.un", "mul.ovf", "mul.ovf.un", "sub.ovf", "sub.ovf.un", "endfinally", "leave", "leave.s", "stind.i", "conv.u", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "prefix7", "prefix6", "prefix5", "prefix4", "prefix3", "prefix2", "prefix1", "prefixref", "arglist", "ceq", "cgt", "cgt.un", "clt", "clt.un", "ldftn", "ldvirtftn", "", "ldarg", "ldarga", "starg", "ldloc", "ldloca", "stloc", "localloc", "", "endfilter", "unaligned.", "volatile.", "tail.", "initobj", "constrained.", "cpblk", "initblk", "", "rethrow", "", "sizeof", "refanytype", "readonly.",  };

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

		public static readonly HashSet<string> ILKeywords = BuildKeywordList(
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
			"property", "type", "flags", "callconv", "strict"
		);

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
