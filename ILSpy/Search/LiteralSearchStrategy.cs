using System;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.Decompiler.Disassembler;
using SRM = System.Reflection.Metadata;
using ILOpCode = System.Reflection.Metadata.ILOpCode;
using ICSharpCode.Decompiler;

using static System.Reflection.Metadata.PEReaderExtensions;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy
{
	class LiteralSearchStrategy : AbstractSearchStrategy
	{
		readonly TypeCode searchTermLiteralType;
		readonly object searchTermLiteralValue;

		public LiteralSearchStrategy(params string[] terms)
			: base(terms)
		{
			if (searchTerm.Length == 1) {
				var lexer = new Lexer(new LATextReader(new System.IO.StringReader(searchTerm[0])));
				var value = lexer.NextToken();

				if (value != null && value.LiteralValue != null) {
					TypeCode valueType = Type.GetTypeCode(value.LiteralValue.GetType());
					switch (valueType) {
						case TypeCode.Byte:
						case TypeCode.SByte:
						case TypeCode.Int16:
						case TypeCode.UInt16:
						case TypeCode.Int32:
						case TypeCode.UInt32:
						case TypeCode.Int64:
						case TypeCode.UInt64:
							searchTermLiteralType = TypeCode.Int64;
							searchTermLiteralValue = CSharpPrimitiveCast.Cast(TypeCode.Int64, value.LiteralValue, false);
							break;
						case TypeCode.Single:
						case TypeCode.Double:
						case TypeCode.String:
							searchTermLiteralType = valueType;
							searchTermLiteralValue = value.LiteralValue;
							break;
					}
				}
			}
		}

		protected override bool IsMatch(IField field, Language language)
		{
			return IsLiteralMatch(field.ConstantValue);
		}

		protected override bool IsMatch(IProperty property, Language language)
		{
			return MethodIsLiteralMatch(property.Getter) || MethodIsLiteralMatch(property.Setter);
		}

		protected override bool IsMatch(IEvent ev, Language language)
		{
			return MethodIsLiteralMatch(ev.AddAccessor) || MethodIsLiteralMatch(ev.RemoveAccessor) || MethodIsLiteralMatch(ev.InvokeAccessor);
		}

		protected override bool IsMatch(IMethod m, Language language)
		{
			return MethodIsLiteralMatch(m);
		}

		bool IsLiteralMatch(object val)
		{
			if (val == null)
				return false;
			switch (searchTermLiteralType) {
				case TypeCode.Int64:
					TypeCode tc = Type.GetTypeCode(val.GetType());
					if (tc >= TypeCode.SByte && tc <= TypeCode.UInt64)
						return CSharpPrimitiveCast.Cast(TypeCode.Int64, val, false).Equals(searchTermLiteralValue);
					else
						return false;
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.String:
					return searchTermLiteralValue.Equals(val);
				default:
					// substring search with searchTerm
					return IsMatch(t => val.ToString());
			}
		}

		bool MethodIsLiteralMatch(IMethod method)
		{
			if (method == null)
				return false;
			var module = ((MetadataModule)method.ParentModule).PEFile;
			var m = (SRM.MethodDefinitionHandle)method.MetadataToken;
			if (m.IsNil)
				return false;
			var methodDefinition = module.Metadata.GetMethodDefinition(m);
			if (!methodDefinition.HasBody())
				return false;
			var blob = module.Reader.GetMethodBody(methodDefinition.RelativeVirtualAddress).GetILReader();
			if (searchTermLiteralType == TypeCode.Int64) {
				long val = (long)searchTermLiteralValue;
				while (blob.RemainingBytes > 0) {
					ILOpCode code;
					switch (code = ILParser.DecodeOpCode(ref blob)) {
						case ILOpCode.Ldc_i8:
							if (val == blob.ReadInt64())
								return true;
							break;
						case ILOpCode.Ldc_i4:
							if (val == blob.ReadInt32())
								return true;
							break;
						case ILOpCode.Ldc_i4_s:
							if (val == blob.ReadSByte())
								return true;
							break;
						case ILOpCode.Ldc_i4_m1:
							if (val == -1)
								return true;
							break;
						case ILOpCode.Ldc_i4_0:
							if (val == 0)
								return true;
							break;
						case ILOpCode.Ldc_i4_1:
							if (val == 1)
								return true;
							break;
						case ILOpCode.Ldc_i4_2:
							if (val == 2)
								return true;
							break;
						case ILOpCode.Ldc_i4_3:
							if (val == 3)
								return true;
							break;
						case ILOpCode.Ldc_i4_4:
							if (val == 4)
								return true;
							break;
						case ILOpCode.Ldc_i4_5:
							if (val == 5)
								return true;
							break;
						case ILOpCode.Ldc_i4_6:
							if (val == 6)
								return true;
							break;
						case ILOpCode.Ldc_i4_7:
							if (val == 7)
								return true;
							break;
						case ILOpCode.Ldc_i4_8:
							if (val == 8)
								return true;
							break;
						default:
							ILParser.SkipOperand(ref blob, code);
							break;
					}
				}
			} else if (searchTermLiteralType != TypeCode.Empty) {
				ILOpCode expectedCode;
				switch (searchTermLiteralType) {
					case TypeCode.Single:
						expectedCode = ILOpCode.Ldc_r4;
						break;
					case TypeCode.Double:
						expectedCode = ILOpCode.Ldc_r8;
						break;
					case TypeCode.String:
						expectedCode = ILOpCode.Ldstr;
						break;
					default:
						throw new InvalidOperationException();
				}
				while (blob.RemainingBytes > 0) {
					var code = ILParser.DecodeOpCode(ref blob);
					if (code != expectedCode) {
						ILParser.SkipOperand(ref blob, code);
						continue;
					}
					switch (code) {
						case ILOpCode.Ldc_r4:
							if ((float)searchTermLiteralValue == blob.ReadSingle())
								return true;
							break;
						case ILOpCode.Ldc_r8:
							if ((double)searchTermLiteralValue == blob.ReadDouble())
								return true;
							break;
						case ILOpCode.Ldstr:
							if ((string)searchTermLiteralValue == ILParser.DecodeUserString(ref blob, module.Metadata))
								return true;
							break;
					}
				}
			} else {
				while (blob.RemainingBytes > 0) {
					var code = ILParser.DecodeOpCode(ref blob);
					if (code != ILOpCode.Ldstr) {
						ILParser.SkipOperand(ref blob, code);
						continue;
					}
					if (base.IsMatch(t => ILParser.DecodeUserString(ref blob, module.Metadata)))
						return true;
				}
			}
			return false;
		}
	}
}
