using System.Runtime.CompilerServices;
public interface ICancellable
{
	bool IsCancellable { get; }
}
public class TestClass : ICancellable
{
	public bool IsCancellable => false;
	[SpecialName]
	bool ICancellable.get_IsCancellable()
	{
		_ = IsCancellable;
		/*Error: End of method reached without returning.*/;
	}
}
