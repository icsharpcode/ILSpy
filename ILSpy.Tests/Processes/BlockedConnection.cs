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
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.Tests.Processes;

/// <summary>
/// Stands in for the connection of an EventPipe session that has been granted but never
/// delivers: a read of it completes for one reason only, the stream being torn down, and then
/// it fails the way a transport whose far end is gone does. It is the shape of connection a
/// collection is left holding when the target dies before the session can be stopped.
/// </summary>
/// <remarks>
/// Only the async read path is implemented, since that is all a copy out of this stream uses;
/// the synchronous one would have to block a thread to behave the same way and no caller needs
/// it.
/// </remarks>
sealed class BlockedConnection : Stream
{
	readonly TaskCompletionSource tornDown = new(TaskCreationOptions.RunContinuationsAsynchronously);

	public override bool CanRead => true;
	public override bool CanSeek => false;
	public override bool CanWrite => false;
	public override long Length => throw new NotSupportedException();
	public override long Position {
		get => throw new NotSupportedException();
		set => throw new NotSupportedException();
	}

	public override async ValueTask<int> ReadAsync(
		Memory<byte> buffer, CancellationToken cancellationToken = default)
	{
		await tornDown.Task.WaitAsync(cancellationToken).ConfigureAwait(false);
		throw new IOException("The connection was torn down under a pending read.");
	}

	public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
	public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
	public override void Flush() { }
	public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
	public override void SetLength(long value) => throw new NotSupportedException();

	protected override void Dispose(bool disposing)
	{
		tornDown.TrySetResult();
		base.Dispose(disposing);
	}
}
