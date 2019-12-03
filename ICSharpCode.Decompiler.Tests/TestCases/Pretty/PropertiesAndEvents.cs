// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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
using System.Text;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class PropertiesAndEvents
	{
		private interface IBase
		{
			int GetterOnly {
				get;
			}

			int SetterOnly {
				set;
			}

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

		private class ExplicitImpl : IBase
		{
			int IBase.Test {
				get {
					throw new NotImplementedException();
				}
				set {
				}
			}

			int IBase.GetterOnly {
				get {
					throw new NotImplementedException();
				}
			}

			int IBase.SetterOnly {
				set {
					throw new NotImplementedException();
				}
			}

			event Action IBase.Event {
				add {
				}
				remove {
				}
			}
		}

		private class Impl : IBase
		{
			public int GetterOnly {
				get {
					throw new NotImplementedException();
				}
			}

			public int SetterOnly {
				set {
					throw new NotImplementedException();
				}
			}

			public int Test {
				get {
					throw new NotImplementedException();
				}
				set {
					throw new NotImplementedException();
				}
			}

			public event Action Event;
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

		public int AutomaticProperty {
			get;
			set;
		}

		public int CustomProperty {
			get {
				return AutomaticProperty;
			}
			set {
				AutomaticProperty = value;
			}
		}

		private object Issue1221 {
			set {
				issue1221 = value;
			}
		}

		public object Item {
			get {
				return null;
			}
			set {

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

#if ROSLYN
		// Legacy csc has a bug where EventHandler<dynamic> is only used for the backing field
		public event EventHandler<dynamic> DynamicAutoEvent;
		public event EventHandler<(int A, string B)> AutoEventWithTuple;
#endif

		public event EventHandler CustomEvent {
			add {
				AutomaticEvent += value;
			}
			remove {
				AutomaticEvent -= value;
			}
		}

		public int Getter(StringBuilder b)
		{
			return b.Length;
		}

		public void Setter(StringBuilder b)
		{
			b.Capacity = 100;
		}

		public char IndexerGetter(StringBuilder b)
		{
			return b[50];
		}

		public void IndexerSetter(StringBuilder b)
		{
			b[42] = 'b';
		}
	}
}
