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
				blob.WriteCompressedInteger(MetadataTokens.GetToken(moveNext));
			}
			return blob;
		}
	}
}
