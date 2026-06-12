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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Recycling;
using Avalonia.Controls.Recycling.Model;
using Avalonia.VisualTree;

namespace ICSharpCode.ILSpy.Docking
{
	/// <summary>
	/// View resolver for the dock chrome. The dock must render one view per dockable identity
	/// (not one per slot) or two dockables sharing a slot render each other's content. Rather
	/// than pinning views in an app-lifetime, never-evicting global cache (a leak for transient
	/// document tabs), every dockable owns its view via <see cref="IDockableViewOwner"/>: this
	/// resolver builds the view once through the <see cref="ViewLocator"/> on first resolution,
	/// stores it on the dockable, and hands the same instance back on every later resolution
	/// (tab switch, drag to a new slot). The view then lives and dies with the dockable.
	///
	/// <para>
	/// Because the owned view is returned directly, the dock never re-resolves it through the
	/// data-template machinery on a cache hit, so the recursive template re-entry that the stock
	/// <see cref="ControlRecycling"/> is prone to cannot occur. Non-owner dockables (if any) fall
	/// through to a stock <see cref="ControlRecycling"/>.
	/// </para>
	/// </summary>
	public sealed class DockableViewRecycling : IControlRecycling
	{
		readonly ControlRecycling fallback = new();
		readonly ViewLocator viewLocator = new();

		// Exposed for tests: the stock recycler used only for non-owner dockables. Owners must
		// never end up here -- that global, never-evicting cache is the leak the owned-view model
		// replaces.
		internal ControlRecycling FallbackCache => fallback;

		public bool TryToUseIdAsKey {
			get => fallback.TryToUseIdAsKey;
			set => fallback.TryToUseIdAsKey = value;
		}

		public bool TryGetValue(object? data, out object? control)
		{
			if (data is IDockableViewOwner { OwnedView: { } view })
			{
				control = view;
				return true;
			}
			return fallback.TryGetValue(data, out control);
		}

		public void Add(object data, object control)
		{
			if (data is IDockableViewOwner owner && control is Control view)
				owner.OwnedView = view;
			else
				fallback.Add(data, control);
		}

		public void Clear() => fallback.Clear();

		public object? Build(object? data, object? existing, object? parent)
		{
			if (data is not IDockableViewOwner owner)
				return fallback.Build(data, existing, parent);

			var view = owner.OwnedView ??= viewLocator.Build(data);
			if (view is null)
				return null;
			if (ReferenceEquals(existing, view) || TryDetachFromParent(view))
				return view;
			// Detach failed: the owned view is still attached to its previous slot -- e.g. mid-drag
			// the dock shows it in the source slot while this resolution targets the new slot, and
			// the source's DeferredContentPresenter rebuilds its child synchronously so it won't
			// release. Adopt a fresh view as the owned instance (the stuck one is discarded when its
			// slot tears down) so the dockable actually moves. Mirrors stock ControlRecycling
			// caching its BuildFallback result -- without re-adopting, OwnedView stays pinned to the
			// stuck view and the dockable never relocates.
			var fresh = viewLocator.Build(data);
			owner.OwnedView = fresh;
			return fresh;
		}

		// Faithful port of the stock ControlRecycling's TryDetachFromParent: detach the control
		// from whatever container currently holds it so it can be re-parented into a new dock
		// slot. Returns false when the control could not be released (its visual parent is still
		// set afterwards) -- the caller then builds a fresh view instead of moving this one. The
		// logical Parent is null when the control sits inside a templated host (Dock's
		// DeferredContentPresenter is a ContentPresenter), so fall back to the visual parent.
		static bool TryDetachFromParent(Control control)
		{
			var parent = control.Parent ?? control.GetVisualParent() as StyledElement;
			switch (parent)
			{
				case null:
					return true;
				case Panel panel:
					return panel.Children.Remove(control);
				case ContentPresenter presenter:
					if (!ReferenceEquals(presenter.Child, control))
						return false;
					presenter.SetCurrentValue(ContentPresenter.ContentProperty, null);
					presenter.UpdateChild();
					return control.GetVisualParent() is null;
				case ContentControl contentControl:
					if (!ReferenceEquals(contentControl.Content, control))
						return false;
					contentControl.SetCurrentValue(ContentControl.ContentProperty, null);
					return true;
				case Decorator decorator:
					if (!ReferenceEquals(decorator.Child, control))
						return false;
					decorator.Child = null;
					return true;
				default:
					return false;
			}
		}
	}
}
