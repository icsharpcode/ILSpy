namespace Humanizer.Inflections;

using CharSpan = System.ReadOnlySpan<System.Char>;

/// <summary>
/// Contains extension methods for humanizing string values.
/// </summary>
internal static class StringHumanizeExtensions
{
	internal static unsafe string Concat(CharSpan left, CharSpan right)
	{
		var result = new string('\0', left.Length + right.Length);
		fixed (char* pResult = result)
		{
			left.CopyTo(new(pResult, left.Length));
			right.CopyTo(new(pResult + left.Length, right.Length));
		}
		return result;
	}

	internal static unsafe string Concat(char left, CharSpan right) =>
		Concat(new CharSpan(&left, 1), right);
}