// Copyright (c) 2026 Franck
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

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Redecomp.Core.Naming;

/// <summary>Pure naming rules that derive a variable name from the name of a method.</summary>
public static partial class MethodNameRules
{
	// Leading verbs that describe how a value is obtained rather than what it is, in English and
	// French (accents removed). The value maps each verb to its past participle.
	static readonly Dictionary<string, string> Verbs = new(StringComparer.OrdinalIgnoreCase) {
		// English
		["get"] = "got", ["compute"] = "computed", ["calculate"] = "calculated", ["calc"] = "calculated",
		["load"] = "loaded", ["read"] = "read", ["fetch"] = "fetched", ["find"] = "found",
		["create"] = "created", ["build"] = "built", ["generate"] = "generated", ["make"] = "made",
		["parse"] = "parsed", ["convert"] = "converted", ["to"] = "converted", ["retrieve"] = "retrieved",
		["count"] = "counted", ["determine"] = "determined", ["resolve"] = "resolved",
		["evaluate"] = "evaluated", ["format"] = "formatted", ["save"] = "saved", ["send"] = "sent",
		["try"] = "tried", ["run"] = "run", ["execute"] = "executed", ["process"] = "processed",
		["query"] = "queried", ["search"] = "searched", ["lookup"] = "looked", ["prepare"] = "prepared",
		["collect"] = "collected", ["extract"] = "extracted", ["produce"] = "produced",
		["render"] = "rendered", ["download"] = "downloaded", ["upload"] = "uploaded",
		["import"] = "imported", ["export"] = "exported", ["validate"] = "validated",
		["check"] = "checked", ["update"] = "updated", ["insert"] = "inserted", ["delete"] = "deleted",
		["remove"] = "removed", ["add"] = "added", ["apply"] = "applied", ["open"] = "opened",
		["write"] = "written", ["select"] = "selected", ["compose"] = "composed",
		// French
		["calculer"] = "calcule", ["compter"] = "compte", ["obtenir"] = "obtenu", ["lire"] = "lu",
		["charger"] = "charge", ["recuperer"] = "recupere", ["trouver"] = "trouve",
		["rechercher"] = "recherche", ["chercher"] = "cherche", ["generer"] = "genere",
		["creer"] = "cree", ["construire"] = "construit", ["convertir"] = "converti",
		["determiner"] = "determine", ["evaluer"] = "evalue", ["formater"] = "formate",
		["sauvegarder"] = "sauvegarde", ["envoyer"] = "envoye", ["executer"] = "execute",
		["traiter"] = "traite", ["extraire"] = "extrait", ["produire"] = "produit",
		["verifier"] = "verifie", ["valider"] = "valide", ["enregistrer"] = "enregistre",
		["importer"] = "importe", ["exporter"] = "exporte", ["telecharger"] = "telecharge",
		["preparer"] = "prepare", ["collecter"] = "collecte", ["selectionner"] = "selectionne",
		["ajouter"] = "ajoute", ["supprimer"] = "supprime", ["modifier"] = "modifie",
		["ouvrir"] = "ouvert", ["ecrire"] = "ecrit", ["lister"] = "liste", ["analyser"] = "analyse",
		["parser"] = "parse",
	};

	// Whole method names that carry no information about the value they return.
	static readonly HashSet<string> Uninformative = new(StringComparer.Ordinal) {
		"ToString", "Invoke", "Equals", "GetHashCode", "GetType", "Clone", "GetEnumerator",
		"MoveNext", "Main", "Create", "Get",
	};

	/// <summary>
	/// Name for a value produced by calling <paramref name="methodName"/>: the method name without
	/// its leading verb and Async suffix (<c>CalculerTotalTtc</c> gives <c>totalTtc</c>). A boolean
	/// result is named after the past participle of the verb, following its object when there is
	/// one (<c>Sauvegarder</c> gives <c>sauvegarde</c>, <c>RemoveItem</c> gives <c>itemRemoved</c>);
	/// any other result of a verb alone gets a <c>Result</c> suffix.
	/// Returns null when the method name says nothing about the value.
	/// </summary>
	public static string? NameForResultOf(string methodName, bool returnsBoolean)
	{
		if (Uninformative.Contains(methodName))
			return null;
		string name = methodName.EndsWith("Async", StringComparison.Ordinal) ? methodName[..^5] : methodName;
		var words = Words().Matches(name).Select(m => m.Value).ToList();
		if (words.Count == 0)
			return null;

		if (!Verbs.TryGetValue(RemoveDiacritics(words[0]), out string? participle))
			return CamelCase(words);
		var rest = words.Skip(1).ToList();
		if (returnsBoolean)
			return rest.Count == 0 ? participle : CamelCase(rest) + Capitalize(participle);
		return rest.Count == 0 ? words[0].ToLowerInvariant() + "Result" : CamelCase(rest);
	}

	[GeneratedRegex(@"[A-Z]+(?![a-z])|[A-Z]?[a-z\p{Ll}0-9]+")]
	private static partial Regex Words();

	static string CamelCase(List<string> words)
	{
		var sb = new StringBuilder(words[0].ToLowerInvariant());
		foreach (string word in words.Skip(1))
			sb.Append(Capitalize(word.ToLowerInvariant()));
		return sb.ToString();
	}

	static string Capitalize(string word) => word.Length == 0 ? word : char.ToUpperInvariant(word[0]) + word[1..];

	static string RemoveDiacritics(string text)
	{
		var sb = new StringBuilder(text.Length);
		foreach (char c in text.Normalize(NormalizationForm.FormD))
		{
			if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
				sb.Append(c);
		}
		return sb.ToString();
	}
}
