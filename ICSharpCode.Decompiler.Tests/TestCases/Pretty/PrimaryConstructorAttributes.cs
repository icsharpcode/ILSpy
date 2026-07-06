using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	[method: CtorMarker]
	[method: Obsolete("Use a different constructor.")]
	public class AttributedClassPrimaryCtor(int id)
	{
		public int Id { get; } = id;
	}

	[method: CtorMarker]
	public struct AttributedStructPrimaryCtor(int value)
	{
		public int Value = value;
	}

	[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = true)]
	public class CtorMarkerAttribute : Attribute
	{
	}
}
