using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.Decompiler.Metadata;
using System.Reflection;
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
					return language.TypeToString(t, includeNamespace: fullName);
				case IField f:
					return language.FieldToString(f, fullName, fullName);
				case IProperty p:
					return language.PropertyToString(p, fullName, fullName);
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
						Location = language.TypeToString(type, includeNamespace: true)
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
}
