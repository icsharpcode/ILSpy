// Copyright (c) 2026 Christoph Wille
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Processes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Processes;

/// <summary>
/// Covers the loaded-assembly half of the process explorer: an EventPipe rundown session
/// against the test host itself, and the scoped nettrace reader that turns the resulting
/// stream into module entries. The test host is the one process guaranteed to be running
/// a known set of assemblies on every OS, so it doubles as the fixture.
/// </summary>
[TestFixture]
public class NettraceRundownReaderTests
{
	static MemoryStream? rundown;

	// Kept alive for the fixture's lifetime: an assembly the runtime has collected is no
	// longer in the rundown.
	static Assembly? inMemoryFixture;

	const string InMemoryFixtureName = "ILSpy.Tests.InMemoryRundownFixture";

	/// <summary>
	/// Collecting a rundown takes a moment, so every test in this fixture shares one. The
	/// in-memory assembly is emitted first, so the one rundown covers both the ordinary
	/// file-backed modules and an assembly that has no file anywhere.
	/// </summary>
	[OneTimeSetUp]
	public async Task CollectRundownOfTheTestHost()
	{
		inMemoryFixture = EmitAssemblyWithNoFile();
		using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
		rundown = await DiagnosticsIpcClient.CollectModuleRundownAsync(Environment.ProcessId, cts.Token);
	}

	/// <summary>
	/// An assembly that exists only in this process's memory - the case a user hits with
	/// <c>Assembly.Load(byte[])</c>, a dynamic proxy, or a source generator's scratch
	/// assembly. It is what the reader must classify as unopenable.
	/// </summary>
	static Assembly EmitAssemblyWithNoFile()
	{
		var name = new AssemblyName(InMemoryFixtureName);
		var builder = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
		builder.DefineDynamicModule(InMemoryFixtureName);
		return builder;
	}

	[OneTimeTearDown]
	public void DisposeRundown() => rundown?.Dispose();

	static MemoryStream Rundown()
	{
		rundown.Should().NotBeNull("the rundown collected in OneTimeSetUp is the fixture");
		return new MemoryStream(rundown!.ToArray(), writable: false);
	}

	[Test]
	public void The_Collected_Stream_Is_A_Nettrace_Stream()
	{
		var buffer = new byte[8];
		Rundown().ReadExactly(buffer);

		Encoding.ASCII.GetString(buffer).Should().Be("Nettrace",
			"CollectTracing2 with format=NetTrace must produce a nettrace container");
	}

	[Test]
	public void Rundown_Lists_CoreLib_With_A_Real_Path()
	{
		var modules = NettraceRundownReader.ReadModules(Rundown());

		var coreLib = modules.Should().ContainSingle(
			m => string.Equals(m.Name, "System.Private.CoreLib.dll", StringComparison.OrdinalIgnoreCase),
			"every CoreCLR process has exactly one CoreLib loaded").Subject;
		coreLib.IsInMemory.Should().BeFalse();
		File.Exists(coreLib.Path).Should().BeTrue("the rundown reports the module's real IL path");
	}

	[Test]
	public void Rundown_Lists_The_Entry_Assembly_Of_The_Process()
	{
		var modules = NettraceRundownReader.ReadModules(Rundown());

		string entryAssembly = Assembly.GetEntryAssembly()!.GetName().Name!;
		modules.Should().Contain(
			m => string.Equals(Path.GetFileNameWithoutExtension(m.Name), entryAssembly, StringComparison.OrdinalIgnoreCase),
			"the entry assembly - the dll behind the apphost - is the whole point of the feature");
	}

	[Test]
	public void Rundown_Lists_Managed_Assemblies_Only()
	{
		var modules = NettraceRundownReader.ReadModules(Rundown());

		// The native side of the same process is dominated by libraries the decompiler has no
		// use for - the runtime host itself among them, loaded by every CoreCLR process.
		// Asking the runtime yields exactly the managed set, which is why the feature reads
		// the rundown rather than enumerating native modules.
		File.Exists(NativeRuntimeHost.FullPath).Should().BeTrue(
			"the runtime host of the current process is the native module to look for");
		modules.Should().NotContain(m => string.Equals(m.Path, NativeRuntimeHost.FullPath, StringComparison.OrdinalIgnoreCase));

		// Stated as the property itself rather than as a list of names to keep out: any native
		// module the reader starts admitting fails this, not just the ones thought of here.
		foreach (var module in modules.Where(m => !m.IsInMemory))
		{
			ProcessExplorer.IsManagedAssembly(module.Path!).Should().BeTrue(
				$"'{module.Name}' was listed as an assembly loaded in the process");
		}
	}

	[Test]
	public void An_Assembly_With_No_File_Is_Listed_But_Marked_Unopenable()
	{
		var modules = NettraceRundownReader.ReadModules(Rundown());

		inMemoryFixture.Should().NotBeNull("the fixture assembly is emitted before the rundown is collected");
		var emitted = modules.Should().ContainSingle(m => m.Name.StartsWith(InMemoryFixtureName, StringComparison.Ordinal),
			"an assembly the runtime holds only in memory is still worth showing").Subject;
		emitted.IsInMemory.Should().BeTrue();
		emitted.Path.Should().BeNull("there is no file to open, which is what the dialog tells the user");
	}

	[Test]
	public void A_Payload_That_Ends_Inside_A_String_Is_Rejected()
	{
		// "ab", filling the payload exactly, with no terminator - what a payload whose layout
		// does not match what its event id promised looks like. The reader must stop at the
		// payload's end rather than hunt for a zero word through the rest of the stream.
		var unterminated = new byte[] { 0x61, 0, 0x62, 0 };
		using var reader = new BinaryReader(new MemoryStream(unterminated));

		var read = () => NettraceRundownReader.ReadUtf16NullTerminated(reader, unterminated.Length);

		read.Should().Throw<InvalidDataException>().WithMessage("*unterminated*");
	}

	[Test]
	public void A_Terminated_String_Inside_Its_Payload_Reads_Back()
	{
		var terminated = new byte[] { 0x61, 0, 0x62, 0, 0, 0, 0xFF, 0xFF };
		using var reader = new BinaryReader(new MemoryStream(terminated));

		NettraceRundownReader.ReadUtf16NullTerminated(reader, terminated.Length).Should().Be("ab");
		reader.BaseStream.Position.Should().Be(6, "the terminator is consumed and the rest is left alone");
	}

	[Test]
	public void Modules_Are_Reported_Once_Each()
	{
		var modules = NettraceRundownReader.ReadModules(Rundown());

		modules.Where(m => !m.IsInMemory).Select(m => m.Path)
			.Should().OnlyHaveUniqueItems("a module loaded once must not be listed twice");
	}

	[Test]
	public void A_Stream_That_Is_Not_Nettrace_Is_Rejected_Clearly()
	{
		using var garbage = new MemoryStream(Encoding.ASCII.GetBytes("this is not a trace"));

		var read = () => NettraceRundownReader.ReadModules(garbage);

		read.Should().Throw<InvalidDataException>().WithMessage("*Nettrace*");
	}
}
