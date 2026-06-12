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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ICSharpCode.ILSpy.Metadata.Filters
{
	public enum TriState { DontCare, Required, Excluded }

	public enum MatchMode { All, Any }

	/// <summary>
	/// Mutable per-column user state for the schema-driven flag filter. The popup UI
	/// pokes at this; <see cref="CompiledFilter.Compile"/> snapshots it into an
	/// evaluation-friendly form. <c>null</c> in <see cref="MutexSelections"/> means the
	/// user picked "any" for that group — every row passes that axis.
	/// </summary>
	public sealed partial class FilterState : ObservableObject
	{
		public FlagsSchema Schema { get; }

		public Dictionary<string, ImmutableHashSet<uint>?> MutexSelections { get; } = new();

		public Dictionary<string, TriState> FlagStates { get; } = new();

		[ObservableProperty]
		private MatchMode independentMode = MatchMode.All;

		public FilterState(FlagsSchema schema)
		{
			Schema = schema ?? throw new ArgumentNullException(nameof(schema));
			foreach (var group in schema.MutexGroups)
				MutexSelections[group.Name] = null;
			foreach (var flag in schema.IndependentFlags)
				FlagStates[flag.Name] = TriState.DontCare;
		}

		public void SetMutexSelection(string groupName, ImmutableHashSet<uint>? values)
		{
			MutexSelections[groupName] = values;
			OnPropertyChanged(nameof(MutexSelections));
		}

		public void SetFlagState(string flagName, TriState state)
		{
			FlagStates[flagName] = state;
			OnPropertyChanged(nameof(FlagStates));
		}

		/// <summary>Resets every group to "any" and every flag to don't-care.</summary>
		public void Clear()
		{
			foreach (var key in MutexSelections.Keys.ToArray())
				MutexSelections[key] = null;
			foreach (var key in FlagStates.Keys.ToArray())
				FlagStates[key] = TriState.DontCare;
			IndependentMode = MatchMode.All;
			OnPropertyChanged(nameof(MutexSelections));
			OnPropertyChanged(nameof(FlagStates));
		}

		/// <summary>True when no constraints are active — the predicate degenerates to "match all".</summary>
		public bool IsEmpty {
			get {
				foreach (var sel in MutexSelections.Values)
					if (sel is not null)
						return false;
				foreach (var s in FlagStates.Values)
					if (s != TriState.DontCare)
						return false;
				return true;
			}
		}
	}
}
