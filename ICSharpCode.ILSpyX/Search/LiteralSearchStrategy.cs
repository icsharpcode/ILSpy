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
using System.Collections.Concurrent;
using System.Reflection.Metadata;
using System.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpyX.Abstractions;

using static System.Reflection.Metadata.PEReaderExtensions;

using ILOpCode = System.Reflection.Metadata.ILOpCode;

namespace ICSharpCode.ILSpyX.Search
{
	public class LiteralSearchStrategy : AbstractEntitySearchStrategy
	{
		readonly TypeCode searchTermLiteralType;
		readonly object searchTermLiteralValue;

		public LiteralSearchStrategy(ILanguage language, ApiVisibility apiVisibility, SearchRequest request,
			IProducerConsumerCollection<SearchResult> resultQueue)
			: base(language, apiVisibility, request, resultQueue)
		{
			var terms = request.Keywords;
			if (terms.Length == 1)
			{
				var lexer = new Lexer(new LATextReader(new System.IO.StringReader(terms[0])));
				var value = lexer.NextToken();

				if (value != null && value.LiteralValue != null)
				{
					TypeCode valueType = Type.GetTypeCode(value.LiteralValue.GetType());
					switch (valueType)
					{
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

		public override void Search(PEFile module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			var metadata = module.Metadata;
			var typeSystem = module.GetTypeSystemWithDecompilerSettingsOrNull(searchRequest.DecompilerSettings);
			if (typeSystem == null)
				return;

			foreach (var handle in metadata.MethodDefinitions)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var md = metadata.GetMethodDefinition(handle);
				if (!md.HasBody() || !MethodIsLiteralMatch(module, md))
					continue;
				var method = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
				if (!CheckVisibility(method) || !IsInNamespaceOrAssembly(method))
					continue;
				OnFoundResult(method);
			}

			foreach (var handle in metadata.FieldDefinitions)
			{
				cancellationToken.ThrowIfCancellationRequested();
				var fd = metadata.GetFieldDefinition(handle);
				if (!fd.HasFlag(System.Reflection.FieldAttributes.Literal))
					continue;
				var constantHandle = fd.GetDefaultValue();
				if (constantHandle.IsNil)
					continue;
				var constant = metadata.GetConstant(constantHandle);
				var blob = metadata.GetBlobReader(constant.Value);
				if (!IsLiteralMatch(metadata, blob.ReadConstant(constant.TypeCode)))
					continue;
				IField field = ((MetadataModule)typeSystem.MainModule).GetDefinition(handle);
				if (!CheckVisibility(field) || !IsInNamespaceOrAssembly(field))
					continue;
				OnFoundResult(field);
			}
		}

		bool IsLiteralMatch(MetadataReader metadata, object? val)
		{
			if (val == null)
				return false;
			switch (searchTermLiteralType)
			{
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
					return IsMatch(val.ToString());
			}
		}

		bool MethodIsLiteralMatch(PEFile module, MethodDefinition methodDefinition)
		{
			var blob = module.Reader.GetMethodBody(methodDefinition.RelativeVirtualAddress).GetILReader();
			if (searchTermLiteralType == TypeCode.Int64)
			{
				long val = (long)searchTermLiteralValue;
				while (blob.RemainingBytes > 0)
				{
					ILOpCode code;
					switch (code = ILParser.DecodeOpCode(ref blob))
					{
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
			}
			else if (searchTermLiteralType != TypeCode.Empty)
			{
				ILOpCode expectedCode;
				switch (searchTermLiteralType)
				{
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
				while (blob.RemainingBytes > 0)
				{
					var code = ILParser.DecodeOpCode(ref blob);
					if (code != expectedCode)
					{
						ILParser.SkipOperand(ref blob, code);
						continue;
					}
					switch (code)
					{
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
			}
			else
			{
				while (blob.RemainingBytes > 0)
				{
					var code = ILParser.DecodeOpCode(ref blob);
					if (code != ILOpCode.Ldstr)
					{
						ILParser.SkipOperand(ref blob, code);
						continue;
					}
					if (IsMatch(ILParser.DecodeUserString(ref blob, module.Metadata)))
						return true;
				}
			}
			return false;
		}
	}
}
