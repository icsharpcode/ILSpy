// Copyright (c) 2026 Siegfried Pammer
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

#:package Microsoft.Diagnostics.Tracing.TraceEvent@3.1.21
#:property PublishAot=false

// allocview: turns a .nettrace of a decompiler run into a byte-weighted allocation flamegraph
// that speedscope.app can open, plus a text summary of where the bytes went.
//
// The trace itself needs no tool and no code in the product - the runtime writes one when these
// variables are set, for ilspycmd, the ILSpy UI, the test host, anything:
//
//   DOTNET_EnableEventPipe=1
//   DOTNET_EventPipeConfig=Microsoft-Windows-DotNETRuntime:0x1:5
//   DOTNET_EventPipeOutputPath=/path/to/trace.nettrace
//
// The numbers come from GCAllocationTick, which fires about once per 100 KB allocated per type,
// so this is a sampling profile: the proportions are meaningful, the absolute totals are an
// estimate, and a type that never accumulates 100 KB may not appear at all.
//
// usage: dotnet run allocview.cs -- <trace.nettrace> [-o <out.speedscope.json>] [--types <n>]

using System.Text;
using System.Text.Json;

using Microsoft.Diagnostics.Tracing.Etlx;

// Areas of interest, most specific prefix first: the first match wins, so
// ICSharpCode.Decompiler.CSharp.Syntax is claimed by the AST rather than by the C# layer.
var areas = new (string Prefix, string Area)[] {
	("ICSharpCode.Decompiler.CSharp.Syntax.", "C# AST"),
	("ICSharpCode.Decompiler.CSharp.OutputVisitor.", "C# output"),
	("ICSharpCode.Decompiler.CSharp.", "C# construction"),
	("ICSharpCode.Decompiler.IL.", "ILAst"),
	("ICSharpCode.Decompiler.FlowAnalysis.", "Flow analysis"),
	("ICSharpCode.Decompiler.TypeSystem.", "Type system"),
	("ICSharpCode.Decompiler.Semantics.", "Resolver results"),
	("ICSharpCode.Decompiler.Metadata.", "Metadata"),
	("ICSharpCode.Decompiler.Documentation.", "Documentation"),
	("ICSharpCode.Decompiler.Util.", "Util"),
	("ICSharpCode.Decompiler.", "Decompiler, other"),
	("System.Reflection.Metadata.", "Metadata reader"),
	("System.", "BCL"),
};

string? tracePath = null, outputPath = null;
int topTypes = 25;
for (int i = 0; i < args.Length; i++)
{
	switch (args[i])
	{
		case "-o" when i + 1 < args.Length:
			outputPath = args[++i];
			break;
		case "--types" when i + 1 < args.Length:
			topTypes = int.Parse(args[++i]);
			break;
		default:
			tracePath = args[i];
			break;
	}
}
if (tracePath == null || !File.Exists(tracePath))
{
	Console.Error.WriteLine("usage: allocview <trace.nettrace> [-o <out.speedscope.json>] [--types <n>]");
	Console.Error.WriteLine();
	Console.Error.WriteLine("collect a trace with:");
	Console.Error.WriteLine("  DOTNET_EnableEventPipe=1 \\");
	Console.Error.WriteLine("  DOTNET_EventPipeConfig=Microsoft-Windows-DotNETRuntime:0x1:5 \\");
	Console.Error.WriteLine("  DOTNET_EventPipeOutputPath=trace.nettrace \\");
	Console.Error.WriteLine("    ilspycmd <assembly> -o /tmp/out");
	return 1;
}
outputPath ??= Path.ChangeExtension(tracePath, ".speedscope.json");

// Resolving call stacks means converting to the indexed .etlx format first. It is kept beside
// the trace, because the conversion is the slow part and a rerun reuses it.
Console.WriteLine($"reading {Path.GetFileName(tracePath)} ...");
var etlxPath = Path.ChangeExtension(tracePath, ".etlx");
if (!File.Exists(etlxPath))
	TraceLog.CreateFromEventPipeDataFile(tracePath, etlxPath);
using var log = TraceLog.OpenOrConvert(etlxPath);

// Speedscope's "sampled" profile: every sample is a stack of indices into a shared frame table,
// ordered caller first, and carries its own weight - here the bytes the runtime attributed to
// the allocation that crossed the sampling threshold.
var frameIndices = new Dictionary<string, int>(StringComparer.Ordinal);
var frameNames = new List<string>();
var samples = new List<int[]>();
var weights = new List<long>();
var byType = new Dictionary<string, long>(StringComparer.Ordinal);
var areaByType = new Dictionary<string, string>(StringComparer.Ordinal);
long total = 0;
int events = 0, withoutStack = 0;

foreach (var traceEvent in log.Events)
{
	if (!traceEvent.EventName.Contains("AllocationTick", StringComparison.Ordinal))
		continue;
	events++;
	var typeName = traceEvent.PayloadByName("TypeName") as string;
	if (typeName == null)
		continue;
	long amount = Convert.ToInt64(traceEvent.PayloadByName("AllocationAmount64") ?? 0L);
	total += amount;
	byType[typeName] = byType.GetValueOrDefault(typeName) + amount;

	var stack = new List<int>();
	string? areaFromStack = null;
	for (var frame = traceEvent.CallStack(); frame != null; frame = frame.Caller)
	{
		var name = frame.CodeAddress.FullMethodName;
		if (string.IsNullOrEmpty(name))
			name = frame.CodeAddress.ModuleName ?? "?";
		// Walking outwards from the allocation, the first decompiler frame is the code the
		// allocation belongs to.
		areaFromStack ??= AreaOfPrefix(name);
		stack.Add(FrameIndex(name));
	}
	if (stack.Count == 0)
		withoutStack++;
	stack.Reverse();
	var area = AreaOf(typeName) ?? areaFromStack ?? "other";
	areaByType[typeName] = area;
	// The allocated type is the deepest frame, so a flamegraph reads "this code path allocated
	// this much of this type".
	stack.Add(FrameIndex($"{area}: {typeName}"));
	samples.Add(stack.ToArray());
	weights.Add(amount);
}

if (events == 0)
{
	Console.Error.WriteLine("no GCAllocationTick events in the trace - was it collected with "
		+ "Microsoft-Windows-DotNETRuntime:0x1:5 (GC keyword, verbose)?");
	return 1;
}

// A dictionary rather than an anonymous type, because the schema property is called '$schema'.
var profile = new Dictionary<string, object> {
	["$schema"] = "https://www.speedscope.app/file-format-schema.json",
	["exporter"] = "ILSpy allocview",
	["shared"] = new { frames = frameNames.Select(n => new { name = n }).ToArray() },
	["profiles"] = new[] {
		new {
			type = "sampled",
			name = "allocated bytes",
			unit = "bytes",
			startValue = 0L,
			endValue = total,
			samples = samples.ToArray(),
			weights = weights.ToArray(),
		}
	},
};
using (var stream = File.Create(outputPath))
	JsonSerializer.Serialize(stream, profile);

Console.WriteLine();
Console.WriteLine($"=== {events:N0} allocation samples, {Bytes(total)} sampled"
	+ (withoutStack > 0 ? $", {withoutStack:N0} without a stack" : "") + " ===");
Console.WriteLine();
Console.WriteLine("by area");
foreach (var (area, bytes) in byType.GroupBy(p => areaByType.GetValueOrDefault(p.Key, "other"))
	.Select(g => (Area: g.Key, Bytes: g.Sum(p => p.Value)))
	.OrderByDescending(a => a.Bytes))
{
	Console.WriteLine($"  {Percent(bytes, total)}  {Bytes(bytes),12}  {area}");
}
Console.WriteLine();
Console.WriteLine($"top {topTypes} types");
foreach (var (type, bytes) in byType.OrderByDescending(p => p.Value).Take(topTypes))
	Console.WriteLine($"  {Percent(bytes, total)}  {Bytes(bytes),12}  {type}");
Console.WriteLine();
Console.WriteLine($"speedscope: {Path.GetFullPath(outputPath)}");
Console.WriteLine("            open it at https://www.speedscope.app (the file never leaves the browser)");
return 0;

int FrameIndex(string name)
{
	if (frameIndices.TryGetValue(name, out int index))
		return index;
	index = frameNames.Count;
	frameIndices[name] = index;
	frameNames.Add(name);
	return index;
}

// An allocation is attributed to the decompiler area its own name mentions anywhere, so that a
// Stack<ILInstruction.ChildrenEnumerator> counts as ILAst work rather than as a collection: the
// generic argument is what the allocation exists for. Returns null when the name says nothing -
// the runtime reports nested and compiler-generated types without a namespace - and the caller
// then falls back to the innermost decompiler frame on the stack, which is a better answer
// anyway: it names the code that wanted the object rather than the object.
string? AreaOf(string typeName)
{
	foreach (var (prefix, area) in areas)
	{
		if (prefix.StartsWith("ICSharpCode.", StringComparison.Ordinal)
			&& typeName.Contains(prefix, StringComparison.Ordinal))
		{
			return area;
		}
	}
	return AreaOfPrefix(typeName);
}

string? AreaOfPrefix(string name)
{
	foreach (var (prefix, area) in areas)
	{
		if (name.StartsWith(prefix, StringComparison.Ordinal))
			return area;
	}
	return null;
}

static string Percent(long part, long total) => $"{100.0 * part / total,5:F1}%";

static string Bytes(long value) => value switch {
	>= 1L << 30 => $"{value / (double)(1L << 30):F1} GB",
	>= 1L << 20 => $"{value / (double)(1L << 20):F1} MB",
	>= 1L << 10 => $"{value / (double)(1L << 10):F1} KB",
	_ => $"{value} B"
};
