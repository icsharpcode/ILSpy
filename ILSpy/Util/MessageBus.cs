// Copyright (c) 2024 Tom Englert for the SharpDevelop Team
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
using System.Windows.Navigation;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

using TomsToolbox.Essentials;

#nullable enable

namespace ICSharpCode.ILSpy.Util
{
	public static class MessageBus
	{
		public static void Send<T>(object? sender, T e)
			where T : EventArgs
		{
			MessageBus<T>.Send(sender, e);
		}
	}

	/// <summary>
	/// Simple, minimalistic message bus.
	/// </summary>
	/// <typeparam name="T">The type of the message event arguments</typeparam>
	public static class MessageBus<T>
		where T : EventArgs
	{
		private static readonly WeakEventSource<T> subscriptions = new();

		public static event EventHandler<T> Subscribers {
			add => subscriptions.Subscribe(value);
			remove => subscriptions.Unsubscribe(value);
		}

		public static void Send(object? sender, T e)
		{
			subscriptions.Raise(sender!, e);
		}
	}

	public abstract class WrappedEventArgs<T> : EventArgs
	{
		private readonly T inner;

		protected WrappedEventArgs(T inner)
		{
			this.inner = inner;
		}

		public static implicit operator T(WrappedEventArgs<T> outer)
		{
			return outer.inner;
		}
	}

	public class CurrentAssemblyListChangedEventArgs(NotifyCollectionChangedEventArgs e) : WrappedEventArgs<NotifyCollectionChangedEventArgs>(e);

	public class SettingsChangedEventArgs(PropertyChangedEventArgs e) : WrappedEventArgs<PropertyChangedEventArgs>(e);

	public class NavigateToReferenceEventArgs(object reference, bool inNewTabPage = false) : EventArgs
	{
		public object Reference { get; } = reference;

		public bool InNewTabPage { get; } = inNewTabPage;
	}

	public class NavigateToEventArgs(RequestNavigateEventArgs request, bool inNewTabPage = false) : EventArgs
	{
		public RequestNavigateEventArgs Request { get; } = request;

		public bool InNewTabPage { get; } = inNewTabPage;
	}

	public class AssemblyTreeSelectionChangedEventArgs() : EventArgs;

	public class ApplySessionSettingsEventArgs(SessionSettings sessionSettings) : EventArgs
	{
		public SessionSettings SessionSettings { get; } = sessionSettings;
	}

	public class MainWindowLoadedEventArgs() : EventArgs;

	public class ActiveTabPageChangedEventArgs(ViewState? viewState) : EventArgs
	{
		public ViewState? ViewState { get; } = viewState;
	}

	public class ResetLayoutEventArgs : EventArgs;

	public class ShowAboutPageEventArgs(TabPageModel tabPage) : EventArgs
	{
		public TabPageModel TabPage { get; } = tabPage;
	}

	public class ShowSearchPageEventArgs(string? searchTerm) : EventArgs
	{
		public string? SearchTerm { get; } = searchTerm;
	}

	public class CheckIfUpdateAvailableEventArgs(bool notify = false) : EventArgs
	{
		public bool Notify { get; } = notify;
	}
}