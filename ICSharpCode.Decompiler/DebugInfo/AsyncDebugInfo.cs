// Copyright (c) 2019 Daniel Grunwald
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
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace ICSharpCode.Decompiler.DebugInfo
{
	public readonly struct AsyncDebugInfo
	{
		public readonly int CatchHandlerOffset;
		public readonly ImmutableArray<Await> Awaits;

		public AsyncDebugInfo(int catchHandlerOffset, ImmutableArray<Await> awaits)
		{
			this.CatchHandlerOffset = catchHandlerOffset;
			this.Awaits = awaits;
		}

		public readonly struct Await
		{
			public readonly int YieldOffset;
			public readonly int ResumeOffset;

			public Await(int yieldOffset, int resumeOffset)
			{
				this.YieldOffset = yieldOffset;
				this.ResumeOffset = resumeOffset;
			}
		}

		public BlobBuilder BuildBlob(MethodDefinitionHandle moveNext)
		{
			BlobBuilder blob = new BlobBuilder();
			blob.WriteUInt32((uint)CatchHandlerOffset);
			foreach (var await in Awaits)
			{
				blob.WriteUInt32((uint)await.YieldOffset);
				blob.WriteUInt32((uint)await.ResumeOffset);
				blob.WriteCompressedInteger(MetadataTokens.GetRowNumber(moveNext));
			}
			return blob;
		}
	}
}
