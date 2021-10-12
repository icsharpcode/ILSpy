using System;
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

		public static DecompilerEventSource Log = new DecompilerEventSource();
	}

}
