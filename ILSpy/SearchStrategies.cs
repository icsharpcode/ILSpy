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
using ICSharpCode.Decompiler.TypeSystem;

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

		protected float CalculateFitness(IEntity member)
		{
			string text = member.Name;

			// Probably compiler generated types without meaningful names, show them last
			if (text.StartsWith("<")) {
				return 0;
			}

			// Constructors always have the same name in IL:
			// Use type name instead
			if (text == ".cctor" || text == ".ctor") {
				text = member.DeclaringType.Name;
			}

			// Ignore generic arguments, it not possible to search based on them either
			text = ReflectionHelper.SplitTypeParameterCountFromReflectionName(text);

			return 1.0f / text.Length;
		}

		protected virtual bool IsMatch(IField field, Language language)
		{
			return false;
		}

		protected virtual bool IsMatch(IProperty property, Language language)
		{
			return false;
		}

		protected virtual bool IsMatch(IEvent ev, Language language)
		{
			return false;
		}

		protected virtual bool IsMatch(IMethod m, Language language)
		{
			return false;
		}

		protected virtual bool MatchName(IEntity m, Language language)
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
					case '~':
						if (term.Length > 1 && !IsNoncontiguousMatch(text.ToLower(), term.Substring(1).ToLower()))
							return false;
						break;
					default:
						if (text.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0)
							return false;
						break;
				}
			}
			return true;
		}

		bool IsNoncontiguousMatch(string text, string searchTerm)
		{
			if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(searchTerm)) {
				return false;
			}
			var textLength = text.Length;
			if (searchTerm.Length > textLength) {
				return false;
			}
			var i = 0;
			for (int searchIndex = 0; searchIndex < searchTerm.Length;) {
				while (i != textLength) {
					if (text[i] == searchTerm[searchIndex]) {
						// Check if all characters in searchTerm have been matched
						if (searchTerm.Length == ++searchIndex)
							return true;
						i++;
						break;
					}
					i++;
				}
				if (i == textLength)
					return false;
			}
			return false;
		}

		string GetLanguageSpecificName(Language language, IEntity member, bool fullName = false)
		{
			switch (member) {
				case ITypeDefinition t:
					return language.TypeToString(Language.MakeParameterizedType(t), includeNamespace: fullName);
				case IField f:
					return language.FieldToString(f, fullName, fullName);
				case IProperty p:
					return language.PropertyToString(p, fullName, fullName, p.IsIndexer);
				case IMethod m:
					return language.MethodToString(m, fullName, fullName);
				case IEvent e:
					return language.EventToString(e, fullName, fullName);
				default:
					throw new NotSupportedException(member?.GetType() + " not supported!");
			}
		}

		void Add<T>(Func<IEnumerable<T>> itemsGetter, ITypeDefinition type, Language language, Action<SearchResult> addResult, Func<T, Language, bool> matcher, Func<T, ImageSource> image) where T : IEntity
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
						Location = language.TypeToString(Language.MakeParameterizedType(type), includeNamespace: true)
					});
				}
			}
		}

		public virtual void Search(ITypeDefinition type, Language language, Action<SearchResult> addResult)
		{
			Add(() => type.Fields, type, language, addResult, IsMatch, FieldTreeNode.GetIcon);
			Add(() => type.Properties, type, language, addResult, IsMatch, p => PropertyTreeNode.GetIcon(p));
			Add(() => type.Events, type, language, addResult, IsMatch, EventTreeNode.GetIcon);
			Add(() => type.Methods.Where(m => !m.IsAccessor), type, language, addResult, IsMatch, MethodTreeNode.GetIcon);

			foreach (var nestedType in type.NestedTypes) {
				Search(nestedType, language, addResult);
			}
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

		protected override bool MatchName(IEntity m, Language language)
		{
			return SRM.Ecma335.MetadataTokens.GetToken(m.MetadataToken) == searchTermToken;
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
			var module = ((MetadataAssembly)method.ParentAssembly).PEFile;
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

		protected override bool IsMatch(IField field, Language language)
		{
			return (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Field) && MatchName(field, language);
		}

		protected override bool IsMatch(IProperty property, Language language)
		{
			return (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Property) && MatchName(property, language);
		}

		protected override bool IsMatch(IEvent ev, Language language)
		{
			return (searchKind == MemberSearchKind.All || searchKind == MemberSearchKind.Event) && MatchName(ev, language);
		}

		protected override bool IsMatch(IMethod m, Language language)
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

		public override void Search(ITypeDefinition type, Language language, Action<SearchResult> addResult)
		{
			if (MatchName(type, language)) {
				string name = language.TypeToString(Language.MakeParameterizedType(type), includeNamespace: false);
				var declaringType = type.DeclaringTypeDefinition;
				addResult(new SearchResult {
					Member = type,
					Fitness = CalculateFitness(type),
					Image = TypeTreeNode.GetIcon(type),
					Name = name,
					LocationImage = declaringType != null ? TypeTreeNode.GetIcon(declaringType) : Images.Namespace,
					Location = declaringType != null ? language.TypeToString(Language.MakeParameterizedType(declaringType), includeNamespace: true) : type.Namespace
				});
			}

			foreach (var nestedType in type.NestedTypes) {
				Search(nestedType, language, addResult);
			}
		}
	}

	class TypeAndMemberSearchStrategy : AbstractSearchStrategy
	{
		public TypeAndMemberSearchStrategy(params string[] terms)
			: base(terms)
		{
		}

		public override void Search(ITypeDefinition type, Language language, Action<SearchResult> addResult)
		{
			if (MatchName(type, language))
			{
				string name = language.TypeToString(Language.MakeParameterizedType(type), includeNamespace: false);
				var declaringType = type.DeclaringTypeDefinition;
				addResult(new SearchResult {
					Member = type,
					Image = TypeTreeNode.GetIcon(type),
					Fitness = CalculateFitness(type),
					Name = name,
					LocationImage = declaringType != null ? TypeTreeNode.GetIcon(declaringType) : Images.Namespace,
					Location = declaringType != null ? language.TypeToString(Language.MakeParameterizedType(declaringType), includeNamespace: true) : type.Namespace
				});
			}

			base.Search(type, language, addResult);
		}

		protected override bool IsMatch(IField field, Language language)
		{
			return MatchName(field, language);
		}

		protected override bool IsMatch(IProperty property, Language language)
		{
			return MatchName(property, language);
		}

		protected override bool IsMatch(IEvent ev, Language language)
		{
			return MatchName(ev, language);
		}

		protected override bool IsMatch(IMethod m, Language language)
		{
			return MatchName(m, language);
		}
	}
}
