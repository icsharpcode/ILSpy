// Copyright (c) 2026 Siegfried Pammer
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

#if DEBUG

using System.Collections.Generic;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.Decompiler.DebugSteps;

namespace ICSharpCode.ILSpy.ViewModels
{
	/// <summary>
	/// Per-row UI state for the Debug Steps tree. <see cref="Stepper.Node"/> is a decompiler-side
	/// record with no notion of expansion or visibility, and TreeViewItem containers cannot carry
	/// that state reliably either: every expander gesture writes IsExpanded via SetCurrentValue,
	/// which any style-driven binding overwrites the next time it produces a value. Keeping the
	/// state on a view-model wrapper makes it authoritative — the view binds each row's
	/// IsVisible/IsExpanded here, and the pane can snapshot and restore expansion around filter
	/// sessions instead of losing it to the containers.
	/// </summary>
	public sealed partial class StepNodeViewModel : ObservableObject
	{
		public Stepper.Node Step { get; }
		public StepNodeViewModel? Parent { get; }
		public string Description => Step.Description;

		IReadOnlyList<StepNodeViewModel>? children;

		/// <summary>
		/// Wrappers for this step's children, built on first access. A recorded type runs to tens of
		/// thousands of steps, so materialising the whole tree up front would put that many view-models
		/// on the UI thread for the handful of rows an expanded path actually shows.
		/// </summary>
		public IReadOnlyList<StepNodeViewModel> Children => children ??= Wrap(Step.Children, this);

		/// <summary>Two-way bound to the row's TreeViewItem.IsExpanded.</summary>
		[ObservableProperty]
		bool isExpanded;

		/// <summary>Bound to the row's TreeViewItem.IsVisible; false while the filter hides the row.</summary>
		[ObservableProperty]
		bool isVisible = true;

		/// <summary>
		/// Expansion state captured when a filter session starts, restored when it ends.
		/// Null outside filter sessions.
		/// </summary>
		internal bool? ExpansionBeforeFilter { get; set; }

		StepNodeViewModel(Stepper.Node step, StepNodeViewModel? parent)
		{
			Step = step;
			Parent = parent;
		}

		public static IReadOnlyList<StepNodeViewModel> Wrap(IList<Stepper.Node> steps) => Wrap(steps, null);

		static IReadOnlyList<StepNodeViewModel> Wrap(IList<Stepper.Node> steps, StepNodeViewModel? parent)
		{
			var wrapped = new List<StepNodeViewModel>(steps.Count);
			foreach (var step in steps)
			{
				wrapped.Add(new StepNodeViewModel(step, parent));
			}
			return wrapped;
		}
	}
}

#endif
