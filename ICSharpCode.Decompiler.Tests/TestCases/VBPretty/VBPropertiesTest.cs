using System;

public class VBPropertiesTest
{
	private int _fullProperty;

	public int FullProperty {
		get {
			return _fullProperty;
		}
		set {
			_fullProperty = value;
		}
	}

	public int AutoProperty { get; set; }

#if ROSLYN
	public int ReadOnlyAutoProperty { get; }
	public VBPropertiesTest()
	{
		ReadOnlyAutoProperty = 32;
	}
#endif

	public void TestMethod()
	{
		FullProperty = 42;
		_fullProperty = 24;
		AutoProperty = 4711;

		Console.WriteLine(AutoProperty);
		Console.WriteLine(_fullProperty);
		Console.WriteLine(FullProperty);
#if ROSLYN
		Console.WriteLine(ReadOnlyAutoProperty);
#endif
	}
}
