using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Humanizer.Inflections;

#nullable enable

/// <summary>
/// A container for exceptions to simple pluralization/singularization rules.
/// Vocabularies.Default contains an extensive list of rules for US English.
/// At this time, multiple vocabularies and removing existing rules are not supported.
/// </summary>
internal class Vocabulary
{
	internal Vocabulary()
	{
	}

	readonly List<Rule> plurals = [];
	readonly List<Rule> singulars = [];
	readonly HashSet<string> uncountables = new(StringComparer.CurrentCultureIgnoreCase);
	readonly Regex letterS = new("^([sS])[sS]*$");

	/// <summary>
	/// Adds a word to the vocabulary which cannot easily be pluralized/singularized by RegEx, e.g. "person" and "people".
	/// </summary>
	/// <param name="singular">The singular form of the irregular word, e.g. "person".</param>
	/// <param name="plural">The plural form of the irregular word, e.g. "people".</param>
	/// <param name="matchEnding">True to match these words on their own as well as at the end of longer words. False, otherwise.</param>
	public void AddIrregular(string singular, string plural, bool matchEnding = true)
	{
		if (matchEnding)
		{
			var singularSubstring = singular.Substring(1);
			var pluralSubString = plural.Substring(1);
			AddPlural($"({singular[0]}){singularSubstring}$", $"$1{pluralSubString}");
			AddSingular($"({plural[0]}){pluralSubString}$", $"$1{singularSubstring}");
		}
		else
		{
			AddPlural($"^{singular}$", plural);
			AddSingular($"^{plural}$", singular);
		}
	}

	/// <summary>
	/// Adds an uncountable word to the vocabulary, e.g. "fish".  Will be ignored when plurality is changed.
	/// </summary>
	/// <param name="word">Word to be added to the list of uncountables.</param>
	public void AddUncountable(string word) =>
		uncountables.Add(word);

	/// <summary>
	/// Adds a rule to the vocabulary that does not follow trivial rules for pluralization, e.g. "bus" -> "buses"
	/// </summary>
	/// <param name="rule">RegEx to be matched, case insensitive, e.g. "(bus)es$"</param>
	/// <param name="replacement">RegEx replacement  e.g. "$1"</param>
	public void AddPlural(string rule, string replacement) =>
		plurals.Add(new(rule, replacement));

	/// <summary>
	/// Adds a rule to the vocabulary that does not follow trivial rules for singularization, e.g. "vertices/indices -> "vertex/index"
	/// </summary>
	/// <param name="rule">RegEx to be matched, case insensitive, e.g. ""(vert|ind)ices$""</param>
	/// <param name="replacement">RegEx replacement  e.g. "$1ex"</param>
	public void AddSingular(string rule, string replacement) =>
		singulars.Add(new(rule, replacement));

	/// <summary>
	/// Pluralizes the provided input considering irregular words
	/// </summary>
	/// <param name="word">Word to be pluralized</param>
	/// <param name="inputIsKnownToBeSingular">Normally you call Pluralize on singular words; but if you're unsure call it with false</param>
	[return: NotNullIfNotNull(nameof(word))]
	public string? Pluralize(string? word, bool inputIsKnownToBeSingular = true)
	{
		if (word == null)
		{
			return null;
		}

		var s = LetterS(word);
		if (s != null)
		{
			return s + "s";
		}

		var result = ApplyRules(plurals, word, false);

		if (inputIsKnownToBeSingular)
		{
			return result ?? word;
		}

		var asSingular = ApplyRules(singulars, word, false);
		var asSingularAsPlural = ApplyRules(plurals, asSingular, false);
		if (asSingular != null &&
			asSingular != word &&
			asSingular + "s" != word &&
			asSingularAsPlural == word &&
			result != word)
		{
			return word;
		}

		return result!;
	}

	/// <summary>
	/// Singularizes the provided input considering irregular words
	/// </summary>
	/// <param name="word">Word to be singularized</param>
	/// <param name="inputIsKnownToBePlural">Normally you call Singularize on plural words; but if you're unsure call it with false</param>
	/// <param name="skipSimpleWords">Skip singularizing single words that have an 's' on the end</param>
	[return: NotNullIfNotNull(nameof(word))]
	public string? Singularize(string? word, bool inputIsKnownToBePlural = true, bool skipSimpleWords = false)
	{
		if (word == null)
		{
			return null;
		}
		var s = LetterS(word);
		if (s != null)
		{
			return s;
		}

		var result = ApplyRules(singulars, word, skipSimpleWords);

		if (inputIsKnownToBePlural)
		{
			return result ?? word;
		}

		// the Plurality is unknown so we should check all possibilities
		var asPlural = ApplyRules(plurals, word, false);
		if (asPlural == word ||
			word + "s" == asPlural)
		{
			return result ?? word;
		}

		var asPluralAsSingular = ApplyRules(singulars, asPlural, false);
		if (asPluralAsSingular != word ||
			result == word)
		{
			return result ?? word;
		}

		return word;
	}

	string? ApplyRules(IList<Rule> rules, string? word, bool skipFirstRule)
	{
		if (word == null)
		{
			return null;
		}

		if (word.Length < 1)
		{
			return word;
		}

		if (IsUncountable(word))
		{
			return word;
		}

		var result = word;
		var end = skipFirstRule ? 1 : 0;
		for (var i = rules.Count - 1; i >= end; i--)
		{
			if ((result = rules[i].Apply(word)) != null)
			{
				break;
			}
		}

		if (result == null)
		{
			return null;
		}

		return MatchUpperCase(word, result);
	}

	bool IsUncountable(string word) =>
		uncountables.Contains(word);

	static string MatchUpperCase(string word, string replacement) =>
		char.IsUpper(word[0]) &&
		char.IsLower(replacement[0]) ? StringHumanizeExtensions.Concat(char.ToUpper(replacement[0]), replacement.AsSpan(1)) : replacement;

	/// <summary>
	/// If the word is the letter s, singular or plural, return the letter s singular
	/// </summary>
	string? LetterS(string word)
	{
		var s = letterS.Match(word);
		return s.Groups.Count > 1 ? s.Groups[1].Value : null;
	}

	class Rule(string pattern, string replacement)
	{
		readonly Regex regex = new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public string? Apply(string word)
		{
			if (!regex.IsMatch(word))
			{
				return null;
			}

			return regex.Replace(word, replacement);
		}
	}
}