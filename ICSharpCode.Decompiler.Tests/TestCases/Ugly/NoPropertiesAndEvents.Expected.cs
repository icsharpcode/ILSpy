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
#if ROSLYN
#if !OPT
using System.Diagnostics;
#endif
using System.Runtime.CompilerServices;
#endif
using System.Threading;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal class NoPropertiesAndEvents
	{
#if ROSLYN
		[CompilerGenerated]
#if !OPT
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
#endif
		private EventHandler m_MyEvent;

		public event EventHandler MyEvent {
#if ROSLYN
		[CompilerGenerated]
#endif
			add {
				EventHandler eventHandler = this.m_MyEvent;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler value2 = (EventHandler)Delegate.Combine(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange(ref this.m_MyEvent, value2, eventHandler2);
				} while ((object)eventHandler != eventHandler2);
			}
#if ROSLYN
		[CompilerGenerated]
#endif
			remove {
				EventHandler eventHandler = this.m_MyEvent;
				EventHandler eventHandler2;
				do
				{
					eventHandler2 = eventHandler;
					EventHandler value2 = (EventHandler)Delegate.Remove(eventHandler2, value);
					eventHandler = Interlocked.CompareExchange(ref this.m_MyEvent, value2, eventHandler2);
				} while ((object)eventHandler != eventHandler2);
			}
		}
	}
}
