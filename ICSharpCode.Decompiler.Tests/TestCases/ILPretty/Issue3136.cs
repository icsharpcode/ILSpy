using System.Runtime.CompilerServices;

public class MyClass
{
	private object _MyValue;

	protected virtual object MyValue {
		get {
			return _MyValue;
		}
		[MethodImpl(MethodImplOptions.Synchronized)]
		set {
			_MyValue = value;
		}
	}
}
