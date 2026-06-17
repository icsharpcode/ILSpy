internal class PatternMatching
{
	public static string Describe(object value)
	{
		if (value is int num)
		{
			return "int " + num;
		}
		if (value is string text)
		{
			return "string " + text;
		}
		return "other";
	}
}
