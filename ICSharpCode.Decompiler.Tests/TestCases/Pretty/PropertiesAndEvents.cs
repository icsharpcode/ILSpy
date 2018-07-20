using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class PropertiesAndEvents
	{
		private interface IBase
		{
			int Test {
				get;
				set;
			}

			event Action Event;
		}

		private abstract class BaseClass
		{
			public abstract event EventHandler ThisIsAnAbstractEvent;
		}

		private class OtherClass : BaseClass
		{
			public override event EventHandler ThisIsAnAbstractEvent;
		}

		private class Impl : IBase
		{
			int IBase.Test {
				get {
					throw new NotImplementedException();
				}
				set {
				}
			}

			event Action IBase.Event {
				add {
				}
				remove {
				}
			}
		}

		private interface IChange
		{
			int Property {
				get;
				set;
			}

			event EventHandler Changed;
		}

		private class Change : IChange
		{
			private EventHandler Changed;

			int IChange.Property {
				get;
				set;
			}

			event EventHandler IChange.Changed {
				add {
					Changed = (EventHandler)Delegate.Combine(Changed, value);
				}
				remove {
					Changed = (EventHandler)Delegate.Remove(Changed, value);
				}
			}
		}

		[NonSerialized]
		private int someField;

		private object issue1221;

		public int Value {
			get;
			private set;
		}

		private object Issue1221 {
			set {
				issue1221 = value;
			}
		}

#if ROSLYN
		public int NotAnAutoProperty => someField;
#else
		public int NotAnAutoProperty {
			get {
				return someField;
			}
		}
#endif

		public event EventHandler AutomaticEvent;

		[field: NonSerialized]
		public event EventHandler AutomaticEventWithInitializer = delegate {
		};

		public event EventHandler CustomEvent {
			add {
				AutomaticEvent += value;
			}
			remove {
				AutomaticEvent -= value;
			}
		}
	}
}
