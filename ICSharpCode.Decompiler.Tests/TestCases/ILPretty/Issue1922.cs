namespace Issue1922
{
	public class Program
	{
		public static long fnWorks(int x, int y)
		{
			return (((long)y & 0xFFFFFFL) << 24) | ((long)x & 0xFFFFFFL);
		}

		public static long fnFails(int x, int y)
		{
			return ((y & 0xFFFFFFL) << 24) | (x & 0xFFFFFFL);
		}
	}
}
