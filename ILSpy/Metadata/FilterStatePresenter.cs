// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;

namespace ICSharpCode.ILSpy.Metadata.Filters
{
	/// <summary>
	/// Renders a <see cref="FilterState"/> as a one-line predicate summary
	/// ("Visibility ∈ {Public, NestedPublic} ∧ Sealed ∧ ¬Abstract"). Lives next to
	/// <see cref="CompiledFilter"/> so the visible string and the actual matching rule
	/// stay in sync; tests assert on the string.
	/// </summary>
	public static class FilterStatePresenter
	{
		public static string Describe(FilterState state)
		{
			if (state.IsEmpty)
				return "(any)";
			var clauses = new System.Collections.Generic.List<string>();
			foreach (var group in state.Schema.MutexGroups)
			{
				if (!state.MutexSelections.TryGetValue(group.Name, out var sel) || sel is null)
					continue;
				if (sel.IsEmpty)
				{
					clauses.Add($"{group.Name} ∈ ∅");
					continue;
				}
				var labels = group.Values
					.Where(v => sel.Contains(v.Value))
					.Select(v => StripHexSuffix(v.Label))
					.ToList();
				clauses.Add(labels.Count == 1
					? $"{group.Name} = {labels[0]}"
					: $"{group.Name} ∈ {{{string.Join(", ", labels)}}}");
			}
			var requiredFlags = state.Schema.IndependentFlags
				.Where(f => state.FlagStates.TryGetValue(f.Name, out var t) && t == TriState.Required)
				.Select(f => f.Name)
				.ToList();
			var excludedFlags = state.Schema.IndependentFlags
				.Where(f => state.FlagStates.TryGetValue(f.Name, out var t) && t == TriState.Excluded)
				.Select(f => f.Name)
				.ToList();
			if (requiredFlags.Count > 0)
			{
				var joiner = state.IndependentMode == MatchMode.All ? " ∧ " : " ∨ ";
				clauses.Add(requiredFlags.Count == 1
					? requiredFlags[0]
					: $"({string.Join(joiner, requiredFlags)})");
			}
			foreach (var name in excludedFlags)
				clauses.Add($"¬{name}");
			return string.Join(" ∧ ", clauses);
		}

		static string StripHexSuffix(string label)
		{
			// MutexValue labels carry a "(XXXX)" hex suffix from the inferer for disambiguation
			// in the dropdown; the predicate summary reads better without it ("Public" vs
			// "Public (0001)"). Strip the trailing parenthesised group, keep the name.
			int idx = label.LastIndexOf(' ');
			if (idx <= 0)
				return label;
			var rest = label.AsSpan(idx + 1);
			return rest.Length > 2 && rest[0] == '(' && rest[^1] == ')'
				? label[..idx]
				: label;
		}
	}
}
