using System;

public class Issue1906
{
	public void M()
	{
		Console.WriteLine(Math.Min(Math.Max(long.MinValue, default(long)), long.MaxValue));
	}
}
