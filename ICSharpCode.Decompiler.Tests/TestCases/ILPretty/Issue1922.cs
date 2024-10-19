namespace Issue1922
{
	public class Program
	{
		public static long fnWorks(int x, int y)
		{
			return (long)((((ulong)y & 0xFFFFFFuL) << 24) | ((ulong)x & 0xFFFFFFuL));
		}

		public static long fnFails(int x, int y)
		{
			return ((y & 0xFFFFFFL) << 24) | (x & 0xFFFFFFL);
		}
	}
}
