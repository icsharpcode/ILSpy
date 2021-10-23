// Copyright (c) 2021 Christoph Wille
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

using System.Diagnostics.Tracing;

namespace ICSharpCode.Decompiler.Instrumentation
{
	[EventSource(Name = "ICSharpCode.Decompiler")]
	public sealed class DecompilerEventSource : EventSource
	{
		[Event(1, Level = EventLevel.Informational)]
		public void DoDecompileEvent(string eventName, long elapsedMilliseconds)
		{
			WriteEvent(1, eventName, elapsedMilliseconds);
		}

		[Event(2, Level = EventLevel.Informational)]
		public void DoDecompileProperty(string propertyName, long elapsedMilliseconds)
		{
			WriteEvent(2, propertyName, elapsedMilliseconds);
		}

		[Event(3, Level = EventLevel.Informational)]
		public void DoDecompileField(string fieldName, long elapsedMilliseconds)
		{
			WriteEvent(3, fieldName, elapsedMilliseconds);
		}

		[Event(4, Level = EventLevel.Informational)]
		public void DoDecompileTypeDefinition(string typeDefName, long elapsedMilliseconds)
		{
			WriteEvent(4, typeDefName, elapsedMilliseconds);
		}

		[Event(5, Level = EventLevel.Informational)]
		public void DoDecompileMethod(string methodName, long elapsedMilliseconds)
		{
			WriteEvent(5, methodName, elapsedMilliseconds);
		}

		public static DecompilerEventSource Log = new DecompilerEventSource();
	}

}
