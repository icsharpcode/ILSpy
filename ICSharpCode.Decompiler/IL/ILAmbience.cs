// Copyright (c) Siegfried Pammer
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

#nullable enable

namespace ICSharpCode.Decompiler.IL
{
	public class ILAmbience : IAmbience
	{
		public ConversionFlags ConversionFlags { get; set; }

		public string ConvertConstantValue(object constantValue)
		{
			throw new NotImplementedException();
		}

		public string ConvertSymbol(ISymbol symbol)
		{
			StringWriter sw = new StringWriter();

			ConvertSymbol(sw, symbol);

			return sw.ToString();
		}

		void ConvertSymbol(StringWriter writer, ISymbol symbol)
		{
			var metadata = (symbol as IEntity)?.ParentModule!.MetadataFile?.Metadata;
			var token = (symbol as IEntity)?.MetadataToken ?? default;

			var output = new PlainTextOutput(writer);

			switch (symbol)
			{
				case IField f:
					Debug.Assert(metadata != null);
					if (ConversionFlags.HasFlag(ConversionFlags.ShowDefinitionKeyword))
						writer.Write(".field ");
					var fd = metadata.GetFieldDefinition((FieldDefinitionHandle)token);
					if (ConversionFlags.HasFlag(ConversionFlags.ShowAccessibility))
						ReflectionDisassembler.WriteEnum(fd.Attributes & FieldAttributes.FieldAccessMask, ReflectionDisassembler.fieldVisibility, output);
					if (ConversionFlags.HasFlag(ConversionFlags.ShowModifiers))
					{
						const FieldAttributes hasXAttributes = FieldAttributes.HasDefault | FieldAttributes.HasFieldMarshal | FieldAttributes.HasFieldRVA;
						ReflectionDisassembler.WriteFlags(fd.Attributes & ~(FieldAttributes.FieldAccessMask | hasXAttributes), ReflectionDisassembler.fieldAttributes, output);
						if (!f.IsStatic)
						{
							writer.Write("instance ");
						}
					}
					break;
				case IMethod m:
					Debug.Assert(metadata != null);
					if (ConversionFlags.HasFlag(ConversionFlags.ShowDefinitionKeyword))
						writer.Write(".method ");
					var md = metadata.GetMethodDefinition((MethodDefinitionHandle)token);
					if (ConversionFlags.HasFlag(ConversionFlags.ShowAccessibility))
						ReflectionDisassembler.WriteEnum(md.Attributes & MethodAttributes.MemberAccessMask, ReflectionDisassembler.methodVisibility, output);
					if (ConversionFlags.HasFlag(ConversionFlags.ShowModifiers))
					{
						ReflectionDisassembler.WriteFlags(md.Attributes & ~MethodAttributes.MemberAccessMask, ReflectionDisassembler.methodAttributeFlags, output);
						if (!m.IsStatic)
						{
							writer.Write("instance ");
						}
					}
					break;
				case IProperty p:
					Debug.Assert(metadata != null);
					if (ConversionFlags.HasFlag(ConversionFlags.ShowDefinitionKeyword))
						writer.Write(".property ");
					var pd = metadata.GetPropertyDefinition((PropertyDefinitionHandle)token);
					if (ConversionFlags.HasFlag(ConversionFlags.ShowModifiers))
					{
						ReflectionDisassembler.WriteFlags(pd.Attributes, ReflectionDisassembler.propertyAttributes, output);
						if (!p.IsStatic)
						{
							writer.Write("instance ");
						}
					}
					break;
				case IEvent e:
					Debug.Assert(metadata != null);
					if (ConversionFlags.HasFlag(ConversionFlags.ShowDefinitionKeyword))
						writer.Write(".event ");
					var ed = metadata.GetEventDefinition((EventDefinitionHandle)token);
					if (ConversionFlags.HasFlag(ConversionFlags.ShowModifiers))
					{
						ReflectionDisassembler.WriteFlags(ed.Attributes, ReflectionDisassembler.eventAttributes, output);
						if (!e.IsStatic)
						{
							writer.Write("instance ");
						}
					}
					break;
				case ITypeDefinition:
					Debug.Assert(metadata != null);
					var td = metadata.GetTypeDefinition((TypeDefinitionHandle)token);
					if (ConversionFlags.HasFlag(ConversionFlags.ShowDefinitionKeyword))
					{
						writer.Write(".class ");
						if (td.Attributes.HasFlag(TypeAttributes.Interface))
							writer.Write("interface ");
					}
					if (ConversionFlags.HasFlag(ConversionFlags.ShowAccessibility))
						ReflectionDisassembler.WriteEnum(td.Attributes & TypeAttributes.VisibilityMask, ReflectionDisassembler.typeVisibility, output);
					const TypeAttributes masks = TypeAttributes.ClassSemanticsMask | TypeAttributes.VisibilityMask | TypeAttributes.LayoutMask | TypeAttributes.StringFormatMask;
					if (ConversionFlags.HasFlag(ConversionFlags.ShowModifiers))
						ReflectionDisassembler.WriteFlags(td.Attributes & ~masks, ReflectionDisassembler.typeAttributes, output);
					break;
			}

			bool showReturnTypeBefore = ConversionFlags.HasFlag(ConversionFlags.ShowReturnType)
				&& !ConversionFlags.HasFlag(ConversionFlags.PlaceReturnTypeAfterParameterList);
			bool showReturnTypeAfter = ConversionFlags.HasFlag(ConversionFlags.ShowReturnType)
				&& ConversionFlags.HasFlag(ConversionFlags.PlaceReturnTypeAfterParameterList);

			if (showReturnTypeBefore && symbol is IMember { SymbolKind: not SymbolKind.Constructor })
			{
				switch (symbol)
				{
					case IField f:
						writer.Write(ConvertType(f.ReturnType));
						break;
					case IMethod m:
						writer.Write(ConvertType(m.ReturnType));
						break;
					case IProperty p:
						writer.Write(ConvertType(p.ReturnType));
						break;
					case IEvent e:
						writer.Write(ConvertType(e.ReturnType));
						break;
				}

				writer.Write(' ');
			}

			void WriteTypeDefinition(ITypeDefinition typeDef)
			{
				if ((ConversionFlags.HasFlag(ConversionFlags.UseFullyQualifiedEntityNames)
					|| ConversionFlags.HasFlag(ConversionFlags.ShowDeclaringType))
					&& typeDef.DeclaringTypeDefinition != null)
				{
					WriteTypeDefinition(typeDef.DeclaringTypeDefinition);
					writer.Write('.');
				}
				else if (ConversionFlags.HasFlag(ConversionFlags.UseFullyQualifiedEntityNames)
					&& !string.IsNullOrEmpty(typeDef.Namespace))
				{
					writer.Write(typeDef.Namespace);
					writer.Write('.');
				}
				writer.Write(typeDef.Name);
				WriteTypeParameters(typeDef.TypeParameters, typeDef);
			}

			void WriteTypeParameters(IReadOnlyList<ITypeParameter> typeParameters, IEntity owner)
			{
				if (typeParameters.Count > 0)
				{
					int typeParameterCount = typeParameters.Count - (owner.DeclaringTypeDefinition?.TypeParameterCount ?? 0);
					if (typeParameterCount > 0)
					{
						switch (owner)
						{
							case IType:
								writer.Write("`");
								break;
							case IMethod _:
								writer.Write("``");
								break;
						}

						writer.Write(typeParameterCount);
					}

					if (ConversionFlags.HasFlag(ConversionFlags.ShowTypeParameterList))
					{
						int i = 0;
						writer.Write('<');
						foreach (var tp in typeParameters)
						{
							if (i > 0)
								writer.Write(",");
							if (ConversionFlags.HasFlag(ConversionFlags.ShowTypeParameterVarianceModifier))
							{
								switch (tp.Variance)
								{
									case VarianceModifier.Covariant:
										writer.Write('+');
										break;
									case VarianceModifier.Contravariant:
										writer.Write('-');
										break;
								}
							}
							writer.Write(tp.Name);
							i++;
						}
						writer.Write('>');
					}
				}
			}

			switch (symbol)
			{
				case ITypeDefinition definition:
					WriteTypeDefinition(definition);
					break;
				case IMember member:
					if ((ConversionFlags.HasFlag(ConversionFlags.UseFullyQualifiedTypeNames)
					|| ConversionFlags.HasFlag(ConversionFlags.ShowDeclaringType)) && member.DeclaringTypeDefinition != null)
					{
						WriteTypeDefinition(member.DeclaringTypeDefinition);
						writer.Write("::");
					}
					writer.Write(member.Name);
					if (member is IMethod method)
					{
						WriteTypeParameters(method.TypeParameters, member);
					}
					break;
			}

			if (ConversionFlags.HasFlag(ConversionFlags.ShowParameterList) && symbol is IParameterizedMember { SymbolKind: not SymbolKind.Property } pm)
			{
				writer.Write('(');
				int i = 0;
				foreach (var parameter in pm.Parameters)
				{
					if (i > 0)
						writer.Write(", ");
					writer.Write(ConvertType(parameter.Type));
					if (ConversionFlags.HasFlag(ConversionFlags.ShowParameterNames))
						writer.Write(" " + parameter.Name);
					i++;
				}
				writer.Write(')');
			}

			if (showReturnTypeAfter && symbol is IMember { SymbolKind: not SymbolKind.Constructor })
			{
				writer.Write(" : ");

				switch (symbol)
				{
					case IField f:
						writer.Write(ConvertType(f.ReturnType));
						break;
					case IMethod m:
						writer.Write(ConvertType(m.ReturnType));
						break;
					case IProperty p:
						writer.Write(ConvertType(p.ReturnType));
						break;
					case IEvent e:
						writer.Write(ConvertType(e.ReturnType));
						break;
				}
			}
		}

		public string ConvertType(IType type)
		{
			var visitor = new TypeToStringVisitor(ConversionFlags);
			type.AcceptVisitor(visitor);
			return visitor.ToString();
		}

		class TypeToStringVisitor : TypeVisitor
		{
			readonly ConversionFlags flags;
			readonly StringBuilder builder;

			public override string ToString()
			{
				return builder.ToString();
			}

			public TypeToStringVisitor(ConversionFlags flags)
			{
				this.flags = flags;
				this.builder = new StringBuilder();
			}

			public override IType VisitArrayType(ArrayType type)
			{
				base.VisitArrayType(type);
				builder.Append('[');
				builder.Append(',', type.Dimensions - 1);
				builder.Append(']');
				return type;
			}

			public override IType VisitByReferenceType(ByReferenceType type)
			{
				base.VisitByReferenceType(type);
				builder.Append('&');
				return type;
			}

			public override IType VisitModOpt(ModifiedType type)
			{
				type.ElementType.AcceptVisitor(this);
				builder.Append(" modopt(");
				type.Modifier.AcceptVisitor(this);
				builder.Append(")");
				return type;
			}

			public override IType VisitModReq(ModifiedType type)
			{
				type.ElementType.AcceptVisitor(this);
				builder.Append(" modreq(");
				type.Modifier.AcceptVisitor(this);
				builder.Append(")");
				return type;
			}

			public override IType VisitPointerType(PointerType type)
			{
				base.VisitPointerType(type);
				builder.Append('*');
				return type;
			}

			public override IType VisitTypeParameter(ITypeParameter type)
			{
				base.VisitTypeParameter(type);
				EscapeName(builder, type.Name);
				return type;
			}

			public override IType VisitParameterizedType(ParameterizedType type)
			{
				type.GenericType.AcceptVisitor(this);
				builder.Append('<');
				for (int i = 0; i < type.TypeArguments.Count; i++)
				{
					if (i > 0)
						builder.Append(',');
					type.TypeArguments[i].AcceptVisitor(this);
				}
				builder.Append('>');
				return type;
			}

			public override IType VisitTupleType(TupleType type)
			{
				type.UnderlyingType.AcceptVisitor(this);
				return type;
			}

			public override IType VisitFunctionPointerType(FunctionPointerType type)
			{
				builder.Append("method ");
				if (type.CallingConvention != SignatureCallingConvention.Default)
				{
					builder.Append(type.CallingConvention.ToILSyntax());
					builder.Append(' ');
				}
				type.ReturnType.AcceptVisitor(this);
				builder.Append(" *(");
				bool first = true;
				foreach (var p in type.ParameterTypes)
				{
					if (first)
						first = false;
					else
						builder.Append(", ");

					p.AcceptVisitor(this);
				}
				builder.Append(')');
				return type;
			}

			public override IType VisitOtherType(IType type)
			{
				WriteType(type);
				return type;
			}

			private void WriteType(IType type)
			{
				if (flags.HasFlag(ConversionFlags.UseFullyQualifiedTypeNames))
					EscapeName(builder, type.FullName);
				else
					EscapeName(builder, type.Name);
				if (type.TypeParameterCount > 0)
				{
					builder.Append('`');
					builder.Append(type.TypeParameterCount);
				}
			}

			public override IType VisitTypeDefinition(ITypeDefinition type)
			{
				switch (type.KnownTypeCode)
				{
					case KnownTypeCode.Object:
						builder.Append("object");
						break;
					case KnownTypeCode.Boolean:
						builder.Append("bool");
						break;
					case KnownTypeCode.Char:
						builder.Append("char");
						break;
					case KnownTypeCode.SByte:
						builder.Append("int8");
						break;
					case KnownTypeCode.Byte:
						builder.Append("uint8");
						break;
					case KnownTypeCode.Int16:
						builder.Append("int16");
						break;
					case KnownTypeCode.UInt16:
						builder.Append("uint16");
						break;
					case KnownTypeCode.Int32:
						builder.Append("int32");
						break;
					case KnownTypeCode.UInt32:
						builder.Append("uint32");
						break;
					case KnownTypeCode.Int64:
						builder.Append("int64");
						break;
					case KnownTypeCode.UInt64:
						builder.Append("uint64");
						break;
					case KnownTypeCode.Single:
						builder.Append("float32");
						break;
					case KnownTypeCode.Double:
						builder.Append("float64");
						break;
					case KnownTypeCode.String:
						builder.Append("string");
						break;
					case KnownTypeCode.Void:
						builder.Append("void");
						break;
					case KnownTypeCode.IntPtr:
						builder.Append("native int");
						break;
					case KnownTypeCode.UIntPtr:
						builder.Append("native uint");
						break;
					case KnownTypeCode.TypedReference:
						builder.Append("typedref");
						break;
					default:
						WriteType(type);
						break;
				}
				return type;
			}
		}

		public string WrapComment(string comment)
		{
			return "// " + comment;
		}

		/// <summary>
		/// Escape characters that cannot be displayed in the UI.
		/// </summary>
		public static StringBuilder EscapeName(StringBuilder sb, string name)
		{
			foreach (char ch in name)
			{
				if (char.IsWhiteSpace(ch) || char.IsControl(ch) || char.IsSurrogate(ch))
					sb.AppendFormat("\\u{0:x4}", (int)ch);
				else
					sb.Append(ch);
			}
			return sb;
		}

		/// <summary>
		/// Escape characters that cannot be displayed in the UI.
		/// </summary>
		public static string EscapeName(string name)
		{
			return EscapeName(new StringBuilder(name.Length), name).ToString();
		}
	}
}
