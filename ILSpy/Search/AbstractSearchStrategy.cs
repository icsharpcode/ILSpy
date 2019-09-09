using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Media;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Search
{
	abstract class AbstractSearchStrategy
	{
		protected readonly string[] searchTerm;
		protected readonly Regex regex;
		protected readonly bool fullNameSearch;
		protected readonly bool omitGenerics;
		protected readonly Language language;
		protected readonly ApiVisibility apiVisibility;
		private readonly IProducerConsumerCollection<SearchResult> resultQueue;

		protected AbstractSearchStrategy(Language language, ApiVisibility apiVisibility, IProducerConsumerCollection<SearchResult> resultQueue, params string[] terms)
		{
			this.language = language;
			this.apiVisibility = apiVisibility;
			this.resultQueue = resultQueue;

			if (terms.Length == 1 && terms[0].Length > 2) {
				string search = terms[0];
				if (search.StartsWith("/", StringComparison.Ordinal) && search.Length > 4) {
					var regexString = search.Substring(1, search.Length - 1);
					fullNameSearch = search.Contains("\\.");
					omitGenerics = !search.Contains("<");
					if (regexString.EndsWith("/", StringComparison.Ordinal))
						regexString = regexString.Substring(0, regexString.Length - 1);
					regex = SafeNewRegex(regexString);
				} else {
					fullNameSearch = search.Contains(".");
					omitGenerics = !search.Contains("<");
				}
			}
			searchTerm = terms;
		}

		public abstract void Search(PEFile module, CancellationToken cancellationToken);

		protected virtual bool IsMatch(string entityName)
		{
			if (regex != null) {
				return regex.IsMatch(entityName);
			}

			for (int i = 0; i < searchTerm.Length; ++i) {
				// How to handle overlapping matches?
				var term = searchTerm[i];
				if (string.IsNullOrEmpty(term)) continue;
				string text = entityName;
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

							if (term.Length > 1 && String.Compare(term, 1, text, 0, Math.Max(term.Length, equalCompareLength),
								StringComparison.OrdinalIgnoreCase) != 0)
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

		protected bool CheckVisibility(IEntity entity)
		{
			if (apiVisibility == ApiVisibility.All)
				return true;

			do {
				if (apiVisibility == ApiVisibility.PublicOnly) {
					if (!(entity.Accessibility == Accessibility.Public ||
						entity.Accessibility == Accessibility.Protected ||
						entity.Accessibility == Accessibility.ProtectedOrInternal))
						return false;
				} else if (apiVisibility == ApiVisibility.PublicAndInternal) {
					if (!language.ShowMember(entity))
						return false;
				}
				entity = entity.DeclaringTypeDefinition;
			}
			while (entity != null);

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
		
		protected void OnFoundResult(IEntity entity)
		{
			var result = ResultFromEntity(entity);
			resultQueue.TryAdd(result);
		}

		Regex SafeNewRegex(string unsafePattern)
		{
			try {
				return new Regex(unsafePattern, RegexOptions.Compiled);
			} catch (ArgumentException) {
				return null;
			}
		}

		SearchResult ResultFromEntity(IEntity item)
		{
			var declaringType = item.DeclaringTypeDefinition;
			return new SearchResult {
				Member = item,
				Fitness = CalculateFitness(item),
				Image = GetIcon(item),
				Name = GetLanguageSpecificName(item),
				LocationImage = declaringType != null ? TypeTreeNode.GetIcon(declaringType) : Images.Namespace,
				Location = declaringType != null ? language.TypeToString(declaringType, includeNamespace: true) : item.Namespace,
				ToolTip = item.ParentModule.PEFile?.FileName
			};
		}

		float CalculateFitness(IEntity member)
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

		string GetLanguageSpecificName(IEntity member)
		{
			switch (member) {
				case ITypeDefinition t:
					return language.TypeToString(t, false);
				case IField f:
					return language.FieldToString(f, true, false, false);
				case IProperty p:
					return language.PropertyToString(p, true, false, false);
				case IMethod m:
					return language.MethodToString(m, true, false, false);
				case IEvent e:
					return language.EventToString(e, true, false, false);
				default:
					throw new NotSupportedException(member?.GetType() + " not supported!");
			}
		}

		object GetIcon(IEntity member)
		{
			switch (member) {
				case ITypeDefinition t:
					return TypeTreeNode.GetIcon(t);
				case IField f:
					return FieldTreeNode.GetIcon(f);
				case IProperty p:
					return PropertyTreeNode.GetIcon(p);
				case IMethod m:
					return MethodTreeNode.GetIcon(m);
				case IEvent e:
					return EventTreeNode.GetIcon(e);
				default:
					throw new NotSupportedException(member?.GetType() + " not supported!");
			}
		}
	}
}
