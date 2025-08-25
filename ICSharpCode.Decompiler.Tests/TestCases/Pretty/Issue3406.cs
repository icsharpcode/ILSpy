internal class Issue3406
{
	private record struct S1(int Value);

	private record struct S2
	{
		public int Value;

		public S2(int value)
		{
			Value = value;
		}

		public S2(int a, int b)
		{
			Value = a + b;
		}
	}

	private record struct S3
	{
		public int Value;

		public S3(int value)
		{
			Value = value;
		}
	}

	// This also generates a hidden backing field
	private record struct S4(int value)
	{
		public int Value = value;
	}
}