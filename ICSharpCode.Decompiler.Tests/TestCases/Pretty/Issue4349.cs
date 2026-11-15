using System;

internal class Issue4349
{
	private Action _func;

#if !LEGACY_CSC
	public Action Func => _func ?? (_func = DefFunc);
#else
	public Action Func {
		get {
			return _func ?? (_func = DefFunc);
		}
	}
#endif

	private void DefFunc()
	{
	}
}
