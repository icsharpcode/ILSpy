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

		public int Value {
			get;
			private set;
		}

		public event EventHandler AutomaticEvent;

		[field: NonSerialized]
		public event EventHandler AutomaticEventWithInitializer = delegate {
		};

		public event EventHandler CustomEvent {
			add {
				this.AutomaticEvent += value;
			}
			remove {
				this.AutomaticEvent -= value;
			}
		}
	}
}
