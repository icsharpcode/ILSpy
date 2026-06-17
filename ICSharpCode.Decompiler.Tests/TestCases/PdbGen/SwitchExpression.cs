internal class SwitchExpression
{
	public static string Classify(int n)
	{
		return n switch
		{
			0 => "zero",
			1 => "one",
			_ => "many",
		};
	}
}
