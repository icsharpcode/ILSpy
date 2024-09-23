using System;
using System.Collections.Specialized;
using System.ComponentModel;

using TomsToolbox.Essentials;

namespace ICSharpCode.ILSpy.Util
{
	public static class MessageBus
	{
		public static void Send<T>(object sender, T e)
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

		public static void Send(object sender, T e)
		{
			subscriptions.Raise(sender, e);
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

	public class AssemblyTreeSelectionChangedEventArgs() : EventArgs;
}