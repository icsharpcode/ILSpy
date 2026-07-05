using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class CustomEventImplementations
	{
		private Action CustomEvent;
		public event Action Custom {
			add {
				CustomEvent = (Action)Delegate.Combine(CustomEvent, value);
				Console.WriteLine("add");
			}
			remove {
				CustomEvent = (Action)Delegate.Remove(CustomEvent, value);
			}
		}
	}
	public class GenericAutomaticEvents<T> where T : EventArgs
	{
		public event EventHandler<T> GenericEvent;
	}
	public class InstanceAutomaticEvents
	{
		public event Action InstanceEvent;
		public event EventHandler<EventArgs> InstanceEventWithArgs;
	}
	public static class StaticAutomaticEvents
	{
		public static event Action StaticEvent;
		public static event EventHandler StaticEventWithHandler;
		public static void RaiseStaticEvent()
		{
			StaticAutomaticEvents.StaticEvent();
		}
	}
}
