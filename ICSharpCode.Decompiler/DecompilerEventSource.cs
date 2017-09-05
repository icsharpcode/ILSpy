using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using Mono.Cecil;

namespace ICSharpCode.Decompiler
{
	[EventSource(Name = "DecompilerEventSource")]
	public sealed class DecompilerEventSource : EventSource
	{
		public static DecompilerEventSource Log = new DecompilerEventSource();

		[Event(1, Level = EventLevel.Informational)]
		public void Info(string text)
		{
			WriteEvent(1, text);
		}
	}
}
