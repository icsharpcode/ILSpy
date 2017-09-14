using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class PropertiesAndEvents
	{
		public int Value {
			get;
			private set;
		}

		public event EventHandler AutomaticEvent;

		[field: NonSerialized]
		public event EventHandler AutomaticEventWithInitializer = (EventHandler)delegate(object sender, EventArgs e) {
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
