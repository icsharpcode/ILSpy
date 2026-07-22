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

#nullable enable

using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.IL.Transforms;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Instrumentation
{
	/// <summary>
	/// Member kind reported in the DecompileMemberStart/Stop events.
	/// </summary>
	public enum DecompiledMemberKind
	{
		Method = 1,
		Field = 2,
		Property = 3,
		Event = 4,
	}

	/// <summary>
	/// Performance tracing for the decompilation pipeline.
	///
	/// The provider is consumable via ETW (PerfView) on Windows and via EventPipe
	/// (dotnet-trace) on all platforms. Start/Stop event pairs let trace viewers compute
	/// durations and nesting from the event timestamps; call sites therefore do not
	/// measure elapsed time themselves except for the high-volume single-shot events
	/// (per-transform), which carry an explicit elapsedMs payload.
	///
	/// Call sites use the strongly-typed [NonEvent] overloads below, which check
	/// IsEnabled() before computing any payload (FullName strings, IL body sizes) and
	/// before the params-array marshaling of the underlying WriteEvent calls. This keeps
	/// call sites free of guard clutter while tracing still costs only a branch when no
	/// listener is attached. The [Event] methods define the wire format and are public
	/// for tests.
	/// </summary>
	[EventSource(Name = "ICSharpCode.Decompiler")]
	public sealed class DecompilerEventSource : EventSource
	{
		public static class Keywords
		{
			/// <summary>Per-type and per-member decompilation events.</summary>
			public const EventKeywords Decompilation = (EventKeywords)0x1;
			/// <summary>Type system construction.</summary>
			public const EventKeywords TypeSystem = (EventKeywords)0x2;
			/// <summary>Assembly reference resolution (directory probing, disk I/O).</summary>
			public const EventKeywords AssemblyResolver = (EventKeywords)0x4;
			/// <summary>Whole-project decompilation (per-run and per-file).</summary>
			public const EventKeywords ProjectDecompiler = (EventKeywords)0x8;
			/// <summary>Per-transform timing of the IL and AST pipelines (Verbose, high volume).</summary>
			public const EventKeywords Transforms = (EventKeywords)0x10;
		}

		[Event(1, Level = EventLevel.Informational, Keywords = Keywords.Decompilation)]
		public void DecompileTypeStart(string fullName)
		{
			WriteEvent(1, fullName);
		}

		[Event(2, Level = EventLevel.Informational, Keywords = Keywords.Decompilation)]
		public void DecompileTypeStop(string fullName)
		{
			WriteEvent(2, fullName);
		}

		/// <param name="fullName">Full name of the decompiled member.</param>
		/// <param name="metadataToken">Metadata token of the member (int-encoded).</param>
		/// <param name="memberKind">One of the <see cref="DecompiledMemberKind"/> values.</param>
		/// <param name="ilBodySize">IL body size in bytes; 0 for members without a body.</param>
		[Event(3, Level = EventLevel.Informational, Keywords = Keywords.Decompilation)]
		public void DecompileMemberStart(string fullName, int metadataToken, int memberKind, int ilBodySize)
		{
			WriteEvent(3, fullName, metadataToken, memberKind, ilBodySize);
		}

		[Event(4, Level = EventLevel.Informational, Keywords = Keywords.Decompilation)]
		public void DecompileMemberStop(string fullName, int metadataToken, int memberKind)
		{
			WriteEvent(4, fullName, metadataToken, memberKind);
		}

		[Event(5, Level = EventLevel.Informational, Keywords = Keywords.TypeSystem)]
		public void TypeSystemInitStart(string moduleName)
		{
			WriteEvent(5, moduleName);
		}

		[Event(6, Level = EventLevel.Informational, Keywords = Keywords.TypeSystem)]
		public void TypeSystemInitStop(string moduleName, int referencedAssembliesResolved)
		{
			WriteEvent(6, moduleName, referencedAssembliesResolved);
		}

		[Event(7, Level = EventLevel.Informational, Keywords = Keywords.AssemblyResolver)]
		public void AssemblyResolveStart(string referenceName)
		{
			WriteEvent(7, referenceName);
		}

		[Event(8, Level = EventLevel.Informational, Keywords = Keywords.AssemblyResolver)]
		public void AssemblyResolveStop(string referenceName, string resolvedPath, bool success)
		{
			WriteEvent(8, referenceName, resolvedPath, success);
		}

		[Event(9, Level = EventLevel.Informational, Keywords = Keywords.ProjectDecompiler)]
		public void ProjectDecompilationStart(string moduleName)
		{
			WriteEvent(9, moduleName);
		}

		[Event(10, Level = EventLevel.Informational, Keywords = Keywords.ProjectDecompiler)]
		public void ProjectDecompilationStop(string moduleName, int codeFileCount, int resourceFileCount)
		{
			WriteEvent(10, moduleName, codeFileCount, resourceFileCount);
		}

		[Event(11, Level = EventLevel.Informational, Keywords = Keywords.ProjectDecompiler)]
		public void ProjectFileStart(string fileName, int typeCount)
		{
			WriteEvent(11, fileName, typeCount);
		}

		[Event(12, Level = EventLevel.Informational, Keywords = Keywords.ProjectDecompiler)]
		public void ProjectFileStop(string fileName)
		{
			WriteEvent(12, fileName);
		}

		[Event(13, Level = EventLevel.Verbose, Keywords = Keywords.Transforms)]
		public void ILTransformExecuted(string transformName, int methodToken, double elapsedMs)
		{
			WriteEvent(13, transformName, methodToken, elapsedMs);
		}

		[Event(14, Level = EventLevel.Verbose, Keywords = Keywords.Transforms)]
		public void AstTransformExecuted(string transformName, double elapsedMs)
		{
			WriteEvent(14, transformName, elapsedMs);
		}

		// Strongly-typed entry points for the instrumented code. Each one checks
		// IsEnabled() before extracting the payload, so a disabled provider costs a
		// single branch and zero allocations.

		[NonEvent]
		public void DecompileTypeStart(ITypeDefinition typeDef)
		{
			if (IsEnabled(EventLevel.Informational, Keywords.Decompilation))
				DecompileTypeStart(typeDef.FullName);
		}

		[NonEvent]
		public void DecompileTypeStop(ITypeDefinition typeDef)
		{
			if (IsEnabled(EventLevel.Informational, Keywords.Decompilation))
				DecompileTypeStop(typeDef.FullName);
		}

		[NonEvent]
		public void DecompileMemberStart(IEntity member, DecompiledMemberKind kind)
		{
			if (!IsEnabled(EventLevel.Informational, Keywords.Decompilation))
				return;
			int ilBodySize = 0;
			if (kind == DecompiledMemberKind.Method && !member.MetadataToken.IsNil
				&& member.ParentModule?.MetadataFile is { } file)
			{
				var methodDef = file.Metadata.GetMethodDefinition((MethodDefinitionHandle)member.MetadataToken);
				if (methodDef.RelativeVirtualAddress != 0)
				{
					try
					{
						ilBodySize = file.GetMethodBody(methodDef.RelativeVirtualAddress).GetILReader().Length;
					}
					catch (BadImageFormatException)
					{
						// A corrupted body must not break tracing; the decompilation itself
						// reports the problem when it reads the body.
					}
				}
			}
			DecompileMemberStart(member.FullName, MetadataTokens.GetToken(member.MetadataToken), (int)kind, ilBodySize);
		}

		[NonEvent]
		public void DecompileMemberStop(IEntity member, DecompiledMemberKind kind)
		{
			if (IsEnabled(EventLevel.Informational, Keywords.Decompilation))
				DecompileMemberStop(member.FullName, MetadataTokens.GetToken(member.MetadataToken), (int)kind);
		}

		[NonEvent]
		public void AssemblyResolveStart(IAssemblyReference reference)
		{
			if (IsEnabled(EventLevel.Informational, Keywords.AssemblyResolver))
				AssemblyResolveStart(reference.FullName);
		}

		[NonEvent]
		public void AssemblyResolveStop(IAssemblyReference reference, string? resolvedPath)
		{
			if (IsEnabled(EventLevel.Informational, Keywords.AssemblyResolver))
				AssemblyResolveStop(reference.FullName, resolvedPath ?? "", resolvedPath != null);
		}

		[NonEvent]
		public void ILTransformExecuted(IILTransform transform, ILFunction function, long startTimestamp)
		{
			if (!IsEnabled(EventLevel.Verbose, Keywords.Transforms))
				return;
			string transformName = transform is BlockILTransform blockTransform
				? blockTransform.ToString()
				: transform.GetType().Name;
			int methodToken = 0;
			if (function.Method != null && !function.Method.MetadataToken.IsNil)
				methodToken = MetadataTokens.GetToken(function.Method.MetadataToken);
			ILTransformExecuted(transformName, methodToken, ElapsedMilliseconds(startTimestamp));
		}

		[NonEvent]
		public void AstTransformExecuted(IAstTransform transform, long startTimestamp)
		{
			if (IsEnabled(EventLevel.Verbose, Keywords.Transforms))
				AstTransformExecuted(transform.GetType().Name, ElapsedMilliseconds(startTimestamp));
		}

		/// <summary>
		/// Fractional milliseconds elapsed since a <see cref="Stopwatch.GetTimestamp"/> value.
		/// </summary>
		[NonEvent]
		public static double ElapsedMilliseconds(long startTimestamp)
		{
			return (Stopwatch.GetTimestamp() - startTimestamp) * 1000.0 / Stopwatch.Frequency;
		}

		/// <summary>
		/// Gate for the per-transform timestamps in the IL/AST pipeline loops: the loops
		/// capture this once so they skip the Stopwatch.GetTimestamp() calls entirely (per
		/// transform, per method) when no Verbose trace session is attached.
		/// </summary>
		[NonEvent]
		public bool IsTransformTracingEnabled()
		{
			return IsEnabled(EventLevel.Verbose, Keywords.Transforms);
		}

		public static readonly DecompilerEventSource Log = new DecompilerEventSource();
	}
}
