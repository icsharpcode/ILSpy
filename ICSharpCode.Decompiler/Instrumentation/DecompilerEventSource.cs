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
