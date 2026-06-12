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
using System.Collections.Specialized;
using System.ComponentModel;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Util
{
	/// <summary>
	/// Non-generic convenience entry point. <c>MessageBus.Send(this, new XEventArgs(...))</c>
	/// is shorter than <c>MessageBus&lt;XEventArgs&gt;.Send(this, new XEventArgs(...))</c>
	/// and avoids the explicit type-argument repetition.
	/// </summary>
	public static class MessageBus
	{
		public static void Send<T>(object? sender, T e) where T : EventArgs
		{
			MessageBus<T>.Send(sender, e);
		}
	}

	/// <summary>
	/// Type-keyed pub-sub bus. One static dispatch table per message type <typeparamref name="T"/>;
	/// subscribers are stored weakly (see <see cref="WeakEventSource{T}"/>) so subscribers don't
	/// have to remember to unsubscribe on tear-down.
	/// </summary>
	public static class MessageBus<T> where T : EventArgs
	{
		static readonly WeakEventSource<T> subscriptions = new();

		public static event EventHandler<T> Subscribers {
			add => subscriptions.Subscribe(value);
			remove => subscriptions.Unsubscribe(value);
		}

		public static void Send(object? sender, T e)
		{
			subscriptions.Raise(sender!, e);
		}
	}

	/// <summary>
	/// Base for messages that wrap a framework-supplied <see cref="EventArgs"/> derivative.
	/// The inner value is exposed via the explicit <see cref="Inner"/> property — there is
	/// no implicit conversion operator, so the unwrap step is visible at every read site.
	/// </summary>
	public abstract class WrappedEventArgs<T>(T inner) : EventArgs where T : EventArgs
	{
		public T Inner { get; } = inner;
	}

	/// <summary>Raised when the active <c>AssemblyList</c>'s contents change.</summary>
	public class CurrentAssemblyListChangedEventArgs(NotifyCollectionChangedEventArgs inner)
		: WrappedEventArgs<NotifyCollectionChangedEventArgs>(inner);

	/// <summary>Raised when the open-tabs collection mutates.</summary>
	public class TabPagesCollectionChangedEventArgs(NotifyCollectionChangedEventArgs inner)
		: WrappedEventArgs<NotifyCollectionChangedEventArgs>(inner);

	/// <summary>Raised when any settings section's property changes.</summary>
	public class SettingsChangedEventArgs(PropertyChangedEventArgs inner)
		: WrappedEventArgs<PropertyChangedEventArgs>(inner);

	/// <summary>
	/// Request to navigate to an entity / metadata token / other reference. Decoupled
	/// from any framework navigation type so the message stays usable without WPF.
	/// </summary>
	public class NavigateToReferenceEventArgs(object reference, object? source = null, bool inNewTabPage = false) : EventArgs
	{
		public object Reference { get; } = reference;
		public object? Source { get; } = source;
		public bool InNewTabPage { get; } = inNewTabPage;
	}

	/// <summary>
	/// Request to navigate to a URI (e.g. from a clicked hyperlink in the decompiled
	/// output). The Avalonia variant carries the bare <see cref="Uri"/> plus optional
	/// source / new-tab hints — WPF's <c>RequestNavigateEventArgs</c> has no equivalent
	/// in Avalonia, so the fields that consumers actually read are lifted out instead.
	/// </summary>
	public class NavigateToEventArgs(Uri uri, object? source = null, bool inNewTabPage = false) : EventArgs
	{
		public Uri Uri { get; } = uri;
		public object? Source { get; } = source;
		public bool InNewTabPage { get; } = inNewTabPage;
	}

	/// <summary>Raised by the assembly-tree selection model when the user's selection moves.</summary>
	public class AssemblyTreeSelectionChangedEventArgs() : EventArgs;

	/// <summary>Raised once at app start so subscribers can apply persisted session settings.</summary>
	public class ApplySessionSettingsEventArgs(SessionSettings sessionSettings) : EventArgs
	{
		public SessionSettings SessionSettings { get; } = sessionSettings;
	}

	/// <summary>Raised after the main window's Loaded event has run.</summary>
	public class MainWindowLoadedEventArgs() : EventArgs;

	/// <summary>
	/// Raised when the active tab changes. <see cref="ViewState"/> may be null (e.g. when
	/// the new tab hasn't computed its decompiled state yet); subscribers must null-check.
	/// </summary>
	public class ActiveTabPageChangedEventArgs(ViewState? viewState) : EventArgs
	{
		public ViewState? ViewState { get; } = viewState;
	}

	/// <summary>Request that the dock layout be reset to its default arrangement.</summary>
	public class ResetLayoutEventArgs() : EventArgs;

	/// <summary>Request that an About-page tab be brought to the front (or created).</summary>
	public class ShowAboutPageEventArgs(TabPageModel tabPage) : EventArgs
	{
		public TabPageModel TabPage { get; } = tabPage;
	}

	/// <summary>
	/// Request that the search pane be brought to focus with an optional pre-filled term.
	/// The Avalonia pane already has <c>SearchPaneModel.RequestFocus()</c> + direct property
	/// access for term assignment; the message exists for symmetry and for use by code paths
	/// (like context-menu commands) that don't hold a reference to the pane.
	/// </summary>
	public class ShowSearchPageEventArgs(string? searchTerm) : EventArgs
	{
		public string? SearchTerm { get; } = searchTerm;
	}

	/// <summary>
	/// Request that the updater poll the release feed and optionally surface a banner if
	/// a newer build is available. <see cref="Notify"/> distinguishes silent startup checks
	/// from explicit user-initiated Help → Check For Updates.
	/// </summary>
	public class CheckIfUpdateAvailableEventArgs(bool notify = false) : EventArgs
	{
		public bool Notify { get; } = notify;
	}
}
