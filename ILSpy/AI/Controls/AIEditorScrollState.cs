// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace ICSharpCode.ILSpy.AI.Controls
{
	internal readonly record struct AIEditorScrollSnapshot(double HorizontalOffset, double VerticalOffset, bool FollowTail);

	internal static class AIEditorScrollState
	{
		internal const double FollowTailThreshold = 24;

		internal static bool IsNearBottom(double extentHeight, double offsetY, double viewportHeight)
			=> Math.Max(0, extentHeight - (offsetY + viewportHeight)) <= FollowTailThreshold;

		internal static bool IsNearBottom(ScrollViewer? viewer)
			=> viewer is not null && IsNearBottom(viewer.Extent.Height, viewer.Offset.Y, viewer.Viewport.Height);

		internal static ScrollViewer? FindViewer(Control control)
			=> control.GetVisualDescendants().OfType<ScrollViewer>().FirstOrDefault();

		internal static AIEditorScrollSnapshot Capture(ScrollViewer? viewer, bool followTail)
			=> viewer is null
				? new(0, 0, followTail)
				: new(viewer.Offset.X, viewer.Offset.Y, followTail);

		internal static void Restore(ScrollViewer viewer, AIEditorScrollSnapshot snapshot)
		{
			double horizontal = Math.Max(0, snapshot.HorizontalOffset);
			double vertical = snapshot.FollowTail
				? Math.Max(0, viewer.Extent.Height - viewer.Viewport.Height)
				: Math.Max(0, snapshot.VerticalOffset);
			viewer.Offset = new Vector(horizontal, vertical);
		}
	}

	internal static class AIFollowTailPolicy
	{
		internal static bool ShouldFollowAfterAppend(bool followingTail) => followingTail;

		internal static bool ResetAfterLifecycle(bool nearBottom) => nearBottom;

		internal static bool ShouldForceScrollOnCompletion() => false;
	}

	internal sealed class AIFollowTailController : IDisposable
	{
		ScrollViewer? viewer;
		bool suppressOffsetTracking;
		int restoreVersion;

		internal bool IsFollowingTail { get; private set; }

		internal void Attach(ScrollViewer? target)
		{
			if (ReferenceEquals(viewer, target))
				return;
			Detach();
			viewer = target;
			if (viewer is null)
				return;
			viewer.ScrollChanged += OnScrollChanged;
			ResetFromViewport();
		}

		internal void Detach()
		{
			restoreVersion++;
			if (viewer is not null)
				viewer.ScrollChanged -= OnScrollChanged;
			viewer = null;
			IsFollowingTail = false;
		}

		internal void ResetFromViewport()
			=> IsFollowingTail = AIFollowTailPolicy.ResetAfterLifecycle(AIEditorScrollState.IsNearBottom(viewer));

		internal bool ShouldFollowAfterAppend()
			=> AIFollowTailPolicy.ShouldFollowAfterAppend(IsFollowingTail);

		internal AIEditorScrollSnapshot Capture()
			=> AIEditorScrollState.Capture(viewer, IsFollowingTail);

		internal void RestoreLater(AIEditorScrollSnapshot snapshot)
		{
			int version = ++restoreVersion;
			Dispatcher.UIThread.Post(() => {
				if (version != restoreVersion || viewer is null)
					return;
				IsFollowingTail = snapshot.FollowTail;
				suppressOffsetTracking = true;
				try
				{
					AIEditorScrollState.Restore(viewer, snapshot);
				}
				finally
				{
					suppressOffsetTracking = false;
				}
			}, DispatcherPriority.Loaded);
		}

		internal void SetFollowingTail(bool value) => IsFollowingTail = value;

		void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
		{
			if (!suppressOffsetTracking)
				IsFollowingTail = AIEditorScrollState.IsNearBottom(viewer);
		}

		public void Dispose() => Detach();
	}
}
