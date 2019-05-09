#nullable enable
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class NullableRefTypes
	{
		private string field_string;
		private string? field_nullable_string;
		private dynamic? field_nullable_dynamic;

		private Dictionary<string?, string> field_generic;
		private (string, string?, string) field_tuple;
		private string[]?[] field_array;
		private Dictionary<(string, string?), (int, string[]?, string?[])> field_complex;

		public int GetLength1(string[] arr)
		{
			return field_string.Length + arr.Length;
		}

		public int GetLength2(string[]? arr)
		{
			return field_nullable_string!.Length + arr!.Length;
		}

		public int? GetLength3(string[]? arr)
		{
			return field_nullable_string?.Length + arr?.Length;
		}
	}
}
