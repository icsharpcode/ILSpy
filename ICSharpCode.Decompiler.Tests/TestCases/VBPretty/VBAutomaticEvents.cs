public class VBAutomaticEvents
{
	public delegate void EventWithParameterEventHandler(int EventNumber);
	public delegate void EventWithoutParameterEventHandler();

	public event EventWithParameterEventHandler EventWithParameter;
	public event EventWithoutParameterEventHandler EventWithoutParameter;

	public void RaiseEvents()
	{
		EventWithParameter?.Invoke(1);
		EventWithoutParameter?.Invoke();
	}
}
