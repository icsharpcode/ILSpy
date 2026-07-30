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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Processes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Processes;

/// <summary>
/// Exercises the hand-rolled diagnostics IPC implementation on two levels: golden-byte
/// checks of the wire format (header and string encoding, per dotnet/diagnostics
/// ipc-protocol.md), and live end-to-end checks against the one .NET process guaranteed
/// to exist on every OS this suite runs on - the test host itself, which exposes a
/// diagnostics port like any other CoreCLR process.
/// </summary>
[TestFixture]
public class DiagnosticsIpcClientTests
{
	[Test]
	public void EncodeRequest_Produces_The_Documented_Header_Layout()
	{
		byte[] payload = { 1, 2, 3 };

		byte[] message = DiagnosticsIpcMessage.EncodeRequest(0x04, 0x04, payload);

		message.Should().HaveCount(23, "the header is 20 bytes, followed by the payload");
		Encoding.ASCII.GetString(message, 0, 13).Should().Be("DOTNET_IPC_V1");
		message[13].Should().Be(0, "the magic is null-terminated to 14 bytes");
		BitConverter.ToUInt16(message, 14).Should().Be(23, "the size field covers header plus payload");
		message[16].Should().Be(0x04, "command set");
		message[17].Should().Be(0x04, "command id");
		BitConverter.ToUInt16(message, 18).Should().Be(0, "the reserved field is zero");
		message.Skip(20).Should().Equal(payload);
	}

	[Test]
	public void Strings_Round_Trip_In_The_Documented_Wire_Format()
	{
		using var buffer = new MemoryStream();
		using (var writer = new BinaryWriter(buffer, Encoding.Unicode, leaveOpen: true))
		{
			DiagnosticsIpcMessage.WriteString(writer, "abc");
		}

		buffer.ToArray().Should().Equal(new byte[] {
			4, 0, 0, 0, // u32 char count, including the null terminator
			0x61, 0, 0x62, 0, 0x63, 0, // "abc" as UTF-16LE
			0, 0, // null terminator
		});

		buffer.Position = 0;
		using var reader = new BinaryReader(buffer, Encoding.Unicode, leaveOpen: true);
		DiagnosticsIpcMessage.ReadString(reader).Should().Be("abc");
	}

	[Test]
	public void A_Zero_Length_String_Reads_As_Null()
	{
		using var buffer = new MemoryStream(new byte[] { 0, 0, 0, 0 });
		using var reader = new BinaryReader(buffer, Encoding.Unicode);

		DiagnosticsIpcMessage.ReadString(reader).Should().BeNull();
	}

	[Test]
	public void The_Port_Scan_Finds_The_Current_Process()
	{
		DiagnosticsPortScanner.GetProcessIds().Should().Contain(Environment.ProcessId,
			"the test host is a CoreCLR process and must expose a diagnostics port");
	}

	[Test]
	public async Task ProcessInfo_Of_The_Current_Process_Reports_Its_Entry_Assembly()
	{
		var info = await DiagnosticsIpcClient.GetProcessInfoAsync(
			Environment.ProcessId, CancellationToken.None);

		info.Pid.Should().Be(Environment.ProcessId);
		info.RuntimeCookie.Should().NotBe(Guid.Empty);
		info.CommandLine.Should().NotBeNullOrWhiteSpace();
		info.EntryAssemblyName.Should().Be(Assembly.GetEntryAssembly()!.GetName().Name,
			"ProcessInfo2 reports the managed entry-point assembly, not the native host");
		info.ClrVersion.Should().NotBeNullOrWhiteSpace();
	}

	[Test]
	public async Task An_Endpoint_That_Accepts_But_Never_Answers_Is_Reported_As_A_Timeout()
	{
		// A suspended runtime behaves this way, and so does one whose diagnostics server is
		// wedged. The budget must expire into something that names the process, rather than
		// into a bare cancellation that reads like the caller changed its mind.
		using var endpoint = new HungDiagnosticsEndpoint(pid: HungEndpointPid);

		var query = async () => await DiagnosticsIpcClient.GetProcessInfoAsync(
			HungEndpointPid, TimeSpan.FromMilliseconds(250), CancellationToken.None);

		(await query.Should().ThrowAsync<TimeoutException>())
			.WithMessage($"*{HungEndpointPid}*", "the message must say which process went quiet");
	}

	[Test]
	public async Task Caller_Cancellation_Is_Not_Disguised_As_A_Timeout()
	{
		using var endpoint = new HungDiagnosticsEndpoint(pid: HungEndpointPid + 1);
		using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

		var query = async () => await DiagnosticsIpcClient.GetProcessInfoAsync(
			HungEndpointPid + 1, TimeSpan.FromMinutes(5), cts.Token);

		await query.Should().ThrowAsync<OperationCanceledException>(
			"the dialog closing is not the target's fault");
	}

	// No process has this id; the endpoint below is a fake one standing at its name.
	const int HungEndpointPid = 0x7FFF_0000;
}
