namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class LateBaseConstructorCall
	{
		private static void Initialize()
		{
		}

		public LateBaseConstructorCall()
		{
			Initialize();
		}
	}

	public class SpilledArgumentSource
	{
		public string Content;
		public string Refusal;
		public object Output;
		public string Function;
	}

	public class SpilledConstructorInitializer
	{
		private SpilledConstructorInitializer(SpilledRole role, string content, in SpilledPatch patch, string refusal, string participantName, object output, object tools, string function)
		{
		}

		public SpilledConstructorInitializer(SpilledArgumentSource source)
			: this(content: source?.Content, patch: default, refusal: source?.Refusal, participantName: null, function: source?.Function, role: SpilledRole.Assistant, output: (source?.Output != null) ? new object() : null, tools: null)
		{
		}
	}

	public struct SpilledPatch
	{
		public int Value;
	}

	public enum SpilledRole
	{
		Assistant = 2
	}
}
