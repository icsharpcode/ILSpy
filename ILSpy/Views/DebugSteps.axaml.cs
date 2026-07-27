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

#if DEBUG

using System;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// Thin renderer over <see cref="DebugStepsPaneModel"/>. All cross-language /
	/// cross-decompile state lives on the ViewModel, so the View holds no state of its own —
	/// it just binds. The pointer / keyboard handlers below translate user gestures into
	/// ViewModel command invocations, and the only ViewModel event the View listens to
	/// (bounded by attach/detach, so a discarded view never leaks through the long-lived
	/// ViewModel) is the purely visual "center the selection" request after filter changes.
	/// </summary>
	public partial class DebugSteps : UserControl
	{
		DebugStepsPaneModel? attachedModel;

		public DebugSteps()
		{
			InitializeComponent();
			// TreeViewItem.OnKeyDown consumes Enter/Return to expand or collapse the focused row
			// (marking the event handled) before it bubbles, so a bubble-phase handler never sees
			// Enter on a group row. Intercept in the tunnel phase, ahead of the item, so Enter and
			// Shift+Enter drive the show-state commands for both leaf and group steps.
			StepsTree.AddHandler(InputElement.KeyDownEvent, OnTreeKeyDown, RoutingStrategies.Tunnel, handledEventsToo: true);
			// DataContext usually arrives before the control enters the logical tree; defer to
			// OnAttachedToLogicalTree in that case (and after a detach), so the subscription
			// stays strictly within the attach/detach bracket and a view that never attaches
			// cannot leak through the long-lived pane model.
			DataContextChanged += (_, _) => {
				if (((ILogical)this).IsAttachedToLogicalTree)
					AttachModel();
			};
		}

		protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
		{
			base.OnAttachedToLogicalTree(e);
			AttachModel();
		}

		protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
		{
			base.OnDetachedFromLogicalTree(e);
			DetachModel();
		}

		void AttachModel()
		{
			var model = DataContext as DebugStepsPaneModel;
			if (ReferenceEquals(attachedModel, model))
				return;
			DetachModel();
			attachedModel = model;
			if (model != null)
				model.SelectionRevealRequested += OnSelectionRevealRequested;
		}

		void DetachModel()
		{
			if (attachedModel != null)
			{
				attachedModel.SelectionRevealRequested -= OnSelectionRevealRequested;
				attachedModel = null;
			}
		}

		void OnSelectionRevealRequested(object? sender, EventArgs e)
		{
			// Containers for rows the filter just expanded materialise in the next layout pass;
			// Loaded priority runs after it, so the container geometry is valid when we measure.
			Dispatcher.UIThread.Post(CenterSelectedStep, DispatcherPriority.Loaded);
		}

		void CenterSelectedStep()
		{
			if (attachedModel?.SelectedStep is not { } selected)
				return;
			if (StepsTree.TreeContainerFromItem(selected) is not Control container)
			{
				StepsTree.UpdateLayout();
				if (StepsTree.TreeContainerFromItem(selected) is not Control lateContainer)
					return;
				container = lateContainer;
			}
			var scrollViewer = StepsTree.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();
			if (scrollViewer == null)
				return;
			// An expanded group's container spans its whole subtree; center on the header row.
			var header = container.GetVisualDescendants().OfType<Control>()
				.FirstOrDefault(c => c.Name == "PART_Header") ?? container;
			if (header.TranslatePoint(new Point(0, header.Bounds.Height / 2), scrollViewer) is not { } rowCenter)
				return;
			double delta = rowCenter.Y - scrollViewer.Viewport.Height / 2;
			double maxOffset = Math.Max(0, scrollViewer.Extent.Height - scrollViewer.Viewport.Height);
			scrollViewer.Offset = new Vector(
				scrollViewer.Offset.X,
				Math.Clamp(scrollViewer.Offset.Y + delta, 0, maxOffset));
		}

		void OnTreeDoubleTapped(object? sender, TappedEventArgs e)
			=> (DataContext as DebugStepsPaneModel)?.ShowStateAfterCommand.Execute(null);

		void OnTreeKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key != Key.Enter && e.Key != Key.Return)
				return;
			if (DataContext is not DebugStepsPaneModel vm)
				return;
			if ((e.KeyModifiers & KeyModifiers.Shift) != 0)
				vm.ShowStateBeforeCommand.Execute(null);
			else
				vm.ShowStateAfterCommand.Execute(null);
			e.Handled = true;
		}
	}
}

#endif
