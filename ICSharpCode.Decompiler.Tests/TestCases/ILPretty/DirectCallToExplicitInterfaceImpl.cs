using System;

public sealed class TestClass : IDisposable
{
	void IDisposable.Dispose()
	{
	}

	public void Test(TestClass other)
	{
		((IDisposable)this).Dispose();
		((IDisposable)other).Dispose();
	}
}
