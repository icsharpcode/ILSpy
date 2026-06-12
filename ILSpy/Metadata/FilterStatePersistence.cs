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
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace ICSharpCode.ILSpy.Metadata.Filters
{
	/// <summary>
	/// Round-trips a <see cref="FilterState"/> through an <see cref="XElement"/> so the
	/// schema-driven flag-filter dropdown's user-set state survives a session restart.
	/// Storage is keyed externally (by page-entry type + column name); this class only
	/// owns the per-state XML shape.
	/// </summary>
	/// <remarks>
	/// Wire shape (only non-default constraints are written):
	/// <code>
	///   &lt;FilterState&gt;
	///     &lt;Mutex group="VisibilityMask"&gt;
	///       &lt;Value&gt;2&lt;/Value&gt;
	///       &lt;Value&gt;4&lt;/Value&gt;
	///     &lt;/Mutex&gt;
	///     &lt;Flag name="Sealed" state="Required" /&gt;
	///     &lt;IndependentMode&gt;Any&lt;/IndependentMode&gt;
	///   &lt;/FilterState&gt;
	/// </code>
	/// Apply tolerates unknown group/flag names so an older saved XML can't break a newer
	/// schema (the offending entry is silently skipped, others still apply).
	/// </remarks>
	public static class FilterStatePersistence
	{
		const string Root = "FilterState";
		const string MutexElement = "Mutex";
		const string FlagElement = "Flag";
		const string ValueElement = "Value";
		const string IndependentModeElement = "IndependentMode";

		public static XElement ToXml(FilterState state)
		{
			ArgumentNullException.ThrowIfNull(state);
			var element = new XElement(Root);
			foreach (var (groupName, selection) in state.MutexSelections)
			{
				if (selection is null)
					continue;       // "any" — write nothing
				element.Add(new XElement(MutexElement,
					new XAttribute("group", groupName),
					selection.OrderBy(v => v).Select(v =>
						new XElement(ValueElement, v.ToString(CultureInfo.InvariantCulture)))));
			}
			foreach (var (flagName, triState) in state.FlagStates)
			{
				if (triState == TriState.DontCare)
					continue;       // default — write nothing
				element.Add(new XElement(FlagElement,
					new XAttribute("name", flagName),
					new XAttribute("state", triState.ToString())));
			}
			if (state.IndependentMode != MatchMode.All)
				element.Add(new XElement(IndependentModeElement, state.IndependentMode.ToString()));
			return element;
		}

		public static void ApplyXml(FilterState state, XElement xml)
		{
			ArgumentNullException.ThrowIfNull(state);
			ArgumentNullException.ThrowIfNull(xml);
			foreach (var mutex in xml.Elements(MutexElement))
			{
				var groupName = (string?)mutex.Attribute("group");
				if (groupName is null || !state.MutexSelections.ContainsKey(groupName))
					continue;       // schema drift: group no longer present
				var values = mutex.Elements(ValueElement)
					.Select(v => uint.TryParse(v.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) ? n : (uint?)null)
					.Where(v => v.HasValue)
					.Select(v => v!.Value)
					.ToImmutableHashSet();
				state.SetMutexSelection(groupName, values.IsEmpty ? null : values);
			}
			foreach (var flag in xml.Elements(FlagElement))
			{
				var flagName = (string?)flag.Attribute("name");
				if (flagName is null || !state.FlagStates.ContainsKey(flagName))
					continue;       // schema drift: flag no longer present
				if (Enum.TryParse<TriState>((string?)flag.Attribute("state"), out var tri))
					state.SetFlagState(flagName, tri);
			}
			if (Enum.TryParse<MatchMode>(xml.Element(IndependentModeElement)?.Value, out var mode))
				state.IndependentMode = mode;
		}
	}
}
