using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.Decompiler.Util;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.Decompiler.Metadata;
using System.Reflection;
using ICSharpCode.Decompiler.Disassembler;
using SRM = System.Reflection.Metadata;
using ILOpCode = System.Reflection.Metadata.ILOpCode;
using ICSharpCode.Decompiler;

using static System.Reflection.Metadata.PEReaderExtensions;

namespace ICSharpCode.ILSpy
{
	abstract class AbstractSearchStrategy
	{
		protected string[] searchTerm;
		protected Regex regex;
		protected bool fullNameSearch;

		protected AbstractSearchStrategy(params string[] terms)
		{
			if (terms.Length == 1 && terms[0].Length > 2) {
				var search = terms[0];
				if (search.StartsWith("/", StringComparison.Ordinal) && search.Length > 4) {
					var regexString = search.Substring(1, search.Length - 1);
					fullNameSearch = search.Contains("\\.");
					if (regexString.EndsWith("/", StringComparison.Ordinal))
						regexString = regexString.Substring(0, regexString.Length - 1);
					regex = SafeNewRegex(regexString);
				} else {
					fullNameSearch = search.Contains(".");
				}

				terms[0] = search;
			}

			searchTerm = terms;
		}

		protected float CalculateFitness(IMetadataEntity member)
		{
			var metadata = member.Module.GetMetadataReader();
			string text;
			switch (member) {
				case TypeDefinition td:
					text = metadata.GetString(metadata.GetTypeDefinition(td.Handle).Name);
					break;
				case FieldDefinition fd:
					text = metadata.GetString(metadata.GetFieldDefinition(fd.Handle).Name);
					break;
				case MethodDefinition md:
					text = metadata.GetString(metadata.GetMethodDefinition(md.Handle).Name);
					break;
				case PropertyDefinition pd:
					text = metadata.GetString(metadata.GetPropertyDefinition(pd.Handle).Name);
					break;
				case EventDefinition ed:
					text = metadata.GetString(metadata.GetEventDefinition(ed.Handle).Name);
					break;
				default:
					throw new NotSupportedException();
			}

			// Probably compiler generated types without meaningful names, show them last
			if (text.StartsWith("<")) {
				return 0;
			}

			// Constructors always have the same name in IL:
			// Use type name instead
			if (text == ".cctor" || text == ".ctor") {
				//text = member.DeclaringType.Name;
			}

			// Ignore generic arguments, it not possible to search based on them either
			text = Decompiler.TypeSystem.ReflectionHelper.SplitTypeParameterCountFromReflectionName(text);

			return 1.0f / text.Length;
		}

		protected virtual bool IsMatch(FieldDefinition field, Language language)
		{
			return false;
		}

		protected virtual bool IsMatch(PropertyDefinition property, Language language)
		{
			return false;
		}

		protected virtual bool IsMatch(EventDefinition ev, Language language)
		{
			return false;
		}

		protected virtual bool IsMatch(MethodDefinition m, Language language)
		{
			return false;
		}

		protected virtual bool MatchName(IMetadataEntity m, Language language)
		{
			return IsMatch(t => GetLanguageSpecificName(language, m, regex != null ? fullNameSearch : t.Contains(".")));
		}

		protected virtual bool IsMatch(Func<string, string> getText)
		{
			if (regex != null) {
				return regex.IsMatch(getText(""));
			}

			for (int i = 0; i < searchTerm.Length; ++i) {
				// How to handle overlapping matches?
				var term = searchTerm[i];
				if (string.IsNullOrEmpty(term)) continue;
				string text = getText(term);
				switch (term[0]) {
					case '+': // must contain
						term = term.Substring(1);
						goto default;
					case '-': // should not contain
						if (term.Length > 1 && text.IndexOf(term.Substring(1), StringComparison.OrdinalIgnoreCase) >= 0)
							return false;
						break;
					case '=': // exact match
						{
							var equalCompareLength = text.IndexOf('`');
							if (equalCompareLength == -1)
								equalCompareLength = text.Length;

							if (term.Length > 1 && String.Compare(term, 1, text, 0, Math.Max(term.Length, equalCompareLength), StringComparison.OrdinalIgnoreCase) != 0)
								return false;
						}
						break;
					default:
						if (text.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0)
							return false;
						break;
				}
			}
			return true;
		}

		string GetLanguageSpecificName(Language language, IMetadataEntity member, bool fullName = false)
		{
			var metadata = member.Module.GetMetadataReader();
			switch (member) {
				case TypeDefinition t:
					return language.TypeDefinitionToString(t, fullName);
				case FieldDefinition f:
					return language.FieldToString(f, fullName, fullName);
				case PropertyDefinition p:
					return language.PropertyToString(p, fullName, fullName, p.Handle.HasMatchingDefaultMemberAttribute(member.Module, out _));
				case MethodDefinition m:
					return language.MethodToString(m, fullName, fullName);
				case EventDefinition e:
					return language.EventToString(e, fullName, fullName);
				default:
					throw new NotSupportedException(member?.GetType() + " not supported!");
			}
		}

		void Add<T>(Func<IEnumerable<T>> itemsGetter, TypeDefinition type, Language language, Action<SearchResult> addResult, Func<T, Language, bool> matcher, Func<T, ImageSource> image) where T : IMetadataEntity
		{
			IEnumerable<T> items = Enumerable.Empty<T>();
			try {
				items = itemsGetter();
			} catch (Exception ex) {
				System.Diagnostics.Debug.Print(ex.ToString());
			}
			foreach (var item in items) {
				if (matcher(item, language)) {
					addResult(new SearchResult {
						Member = item,
						Fitness = CalculateFitness(item),
						Image = image(item),
						Name = GetLanguageSpecificName(language, item),
						LocationImage = TypeTreeNode.GetIcon(type),
						Location = language.TypeDefinitionToString(type, includeNamespace: true)
					});
				}
			}
		}

		public virtual void Search(TypeDefinition type, Language language, Action<SearchResult> addResult)
		{
			var td = type.This();
			var metadata = type.Module.GetMetadataReader();
			Add(() => td.GetFields().Select(f => new FieldDefinition(type.Module, f)), type, language, addResult, IsMatch, FieldTreeNode.GetIcon);
			Add(() => td.GetProperties().Select(p => new PropertyDefinition(type.Module, p)), type, language, addResult, IsMatch, p => PropertyTreeNode.GetIcon(p));
			Add(() => td.GetEvents().Select(e => new EventDefinition(type.Module, e)), type, language, addResult, IsMatch, EventTreeNode.GetIcon);
			Add(() => td.GetMethods().Where(m => NotSpecialMethod(m, metadata)).Select(m => new MethodDefinition(type.Module, m)), type, language, addResult, IsMatch, MethodTreeNode.GetIcon);

			foreach (var nestedType in td.GetNestedTypes()) {
				Search(new TypeDefinition(type.Module, nestedType), language, addResult);
			}
		}

		bool NotSpecialMethod(SRM.MethodDefinitionHandle method, SRM.MetadataReader metadata)
		{
			return (method.GetMethodSemanticsAttributes(metadata) & (
				MethodSemanticsAttributes.Setter
				| MethodSemanticsAttributes.Getter
				| MethodSemanticsAttributes.Adder
				| MethodSemanticsAttributes.Remover
				| MethodSemanticsAttributes.Raiser)) == 0;
		}

		Regex SafeNewRegex(string unsafePattern)
		{
			try {
				return new Regex(unsafePattern, RegexOptions.Compiled);
			} catch (ArgumentException) {
				return null;
			}
		}
	}

	class MetadataTokenSearchStrategy : TypeAndMemberSearchStrategy
	{
		readonly int searchTermToken;

		public MetadataTokenSearchStrategy(params string[] terms)
			: base(terms)
		{
			if (searchTerm.Length == 1) {
				int.TryParse(searchTerm[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out searchTermToken);
			}
		}

		protected override bool MatchName(IMetadataEntity m, Language language)
		{
			return SRM.Ecma335.MetadataTokens.GetToken(m.Handle) == searchTermToken;
		}
	}

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

		protected override bool IsMatch(FieldDefinition field, Language language)
		{
			return IsLiteralMatch(field.DecodeConstant());
		}

		protected override bool IsMatch(PropertyDefinition property, Language language)
		{
			var accessors = property.This().GetAccessors();
			return MethodIsLiteralMatch(accessors.Getter, property.Module) || MethodIsLiteralMatch(accessors.Setter, property.Module);
		}

		protected override bool IsMatch(EventDefinition ev, Language language)
		{
			var accessors = ev.This().GetAccessors();
			return MethodIsLiteralMatch(accessors.Adder, ev.Module) || MethodIsLiteralMatch(accessors.Remover, ev.Module) || MethodIsLiteralMatch(accessors.Raiser, ev.Module);
		}

		protected override bool IsMatch(MethodDefinition m, Language language)
		{
			return MethodIsLiteralMatch(m.Handle, m.Module);
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

		bool MethodIsLiteralMatch(SRM.MethodDefinitionHandle m, PEFile module)
		{
			if (module == null)
				return false;
			var metadata = module.GetMetadataReader();
			if (m.IsNil || !m.HasBody(metadata))
				return false;
			var methodDefinition = metadata.GetMethodDefinition(m);
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
							if ((string)searchTermLiteralValue == ILParser.DecodeUserString(ref blob, metadata))
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
					if (IsMatch(t => ILParser.DecodeUserString(ref blob, metadata)))
						return true;
				}
			}
			return false;
		}
	}

	enum MemberSearchKind
	{
		All,
		Field,
		Property,
		Event,
		Method
	}

	class MemberSearchStrategy : AbstractSearchStrategy
	{
		MemberSearchKind searchKind;

		public MemberSearchStrategy(string term, MemberSearchKind searchKind = MemberSearchKind.All)
			: this(new[] { term }, searchKind)
		{
		}

		public MemberSearchStrategy(string[] terms, MemberSearchKind searchKind = MemberSearchKind.All)
			: base(terms)
		{
			this.searchKind = searchKind;
		}

		protected override bool IsMatch(FieldDefinition field, Language language)
		{
			return (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Field) && MatchName(field, language);
		}

		protected override bool IsMatch(PropertyDefinition property, Language language)
		{
			return (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Property) && MatchName(property, language);
		}

		protected override bool IsMatch(EventDefinition ev, Language language)
		{
			return (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Event) && MatchName(ev, language);
		}

		protected override bool IsMatch(MethodDefinition m, Language language)
		{
			return (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Method) && MatchName(m, language);
		}
	}

	class TypeSearchStrategy : AbstractSearchStrategy
	{
		public TypeSearchStrategy(params string[] terms)
			: base(terms)
		{
		}

		public override void Search(TypeDefinition type, Language language, Action<SearchResult> addResult)
		{
			if (MatchName(type, language)) {
				string name = language.TypeDefinitionToString(type, includeNamespace: false);
				var metadata = type.Module.GetMetadataReader();
				var declaringType = type.This().GetDeclaringType();
				addResult(new SearchResult {
					Member = type,
					Fitness = CalculateFitness(type),
					Image = TypeTreeNode.GetIcon(type),
					Name = name,
					LocationImage = !declaringType.IsNil ? TypeTreeNode.GetIcon(new TypeDefinition(type.Module, declaringType)) : Images.Namespace,
					Location = !declaringType.IsNil ? language.TypeDefinitionToString(new TypeDefinition(type.Module, declaringType), includeNamespace: true) : type.Handle.GetFullTypeName(metadata).TopLevelTypeName.Namespace
				});
			}

			foreach (var nestedType in type.This().GetNestedTypes()) {
				Search(new TypeDefinition(type.Module, nestedType), language, addResult);
			}
		}
	}

	class TypeAndMemberSearchStrategy : AbstractSearchStrategy
	{
		public TypeAndMemberSearchStrategy(params string[] terms)
			: base(terms)
		{
		}

		public override void Search(TypeDefinition type, Language language, Action<SearchResult> addResult)
		{
			if (MatchName(type, language))
			{
				string name = language.TypeDefinitionToString(type, includeNamespace: false);
				var metadata = type.Module.GetMetadataReader();
				var declaringType = type.This().GetDeclaringType();
				addResult(new SearchResult {
					Member = type,
					Image = TypeTreeNode.GetIcon(type),
					Fitness = CalculateFitness(type),
					Name = name,
					LocationImage = !declaringType.IsNil ? TypeTreeNode.GetIcon(new TypeDefinition(type.Module, declaringType)) : Images.Namespace,
					Location = !declaringType.IsNil ? language.TypeDefinitionToString(new TypeDefinition(type.Module, declaringType), includeNamespace: true) : type.Handle.GetFullTypeName(metadata).TopLevelTypeName.Namespace
				});
			}

			base.Search(type, language, addResult);
		}

		protected override bool IsMatch(FieldDefinition field, Language language)
		{
			return MatchName(field, language);
		}

		protected override bool IsMatch(PropertyDefinition property, Language language)
		{
			return MatchName(property, language);
		}

		protected override bool IsMatch(EventDefinition ev, Language language)
		{
			return MatchName(ev, language);
		}

		protected override bool IsMatch(MethodDefinition m, Language language)
		{
			return MatchName(m, language);
		}
	}
}
