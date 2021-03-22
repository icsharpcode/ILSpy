using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.Decompiler.DebugInfo
{
	public static class KnownGuids
	{
		public static readonly Guid CSharpLanguageGuid = new Guid("3f5162f8-07c6-11d3-9053-00c04fa302a1");
		public static readonly Guid VBLanguageGuid = new Guid("3a12d0b8-c26c-11d0-b442-00a0244a1dd2");
		public static readonly Guid FSharpLanguageGuid = new Guid("ab4f38c9-b6e6-43ba-be3b-58080b2ccce3");

		public static readonly Guid StateMachineHoistedLocalScopes = new Guid("6DA9A61E-F8C7-4874-BE62-68BC5630DF71");
		public static readonly Guid DynamicLocalVariables = new Guid("83C563C4-B4F3-47D5-B824-BA5441477EA8");
		public static readonly Guid DefaultNamespaces = new Guid("58b2eab6-209f-4e4e-a22c-b2d0f910c782");
		public static readonly Guid EditAndContinueLocalSlotMap = new Guid("755F52A8-91C5-45BE-B4B8-209571E552BD");
		public static readonly Guid EditAndContinueLambdaAndClosureMap = new Guid("A643004C-0240-496F-A783-30D64F4979DE");
		public static readonly Guid EmbeddedSource = new Guid("0e8a571b-6926-466e-b4ad-8ab04611f5fe");
		public static readonly Guid SourceLink = new Guid("CC110556-A091-4D38-9FEC-25AB9A351A6A");
		public static readonly Guid MethodSteppingInformation = new Guid("54FD2AC5-E925-401A-9C2A-F94F171072F8");
		public static readonly Guid CompilationOptions = new Guid("B5FEEC05-8CD0-4A83-96DA-466284BB4BD8");
		public static readonly Guid CompilationMetadataReferences = new Guid("7E4D4708-096E-4C5C-AEDA-CB10BA6A740D");
		public static readonly Guid TupleElementNames = new Guid("ED9FDF71-8879-4747-8ED3-FE5EDE3CE710");

		public static readonly Guid HashAlgorithmSHA1 = new Guid("ff1816ec-aa5e-4d10-87f7-6f4963833460");
		public static readonly Guid HashAlgorithmSHA256 = new Guid("8829d00f-11b8-4213-878b-770e8597ac16");
	}
}
