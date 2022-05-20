internal class UnknownTypes
{
	private readonly IInterface memberField;

	public virtual bool CanExecute(CallbackQuery message)
	{
		return ((IInterface<SomeClass, bool>)(object)memberField).Execute(new SomeClass {
			ChatId = StaticClass.GetChatId(message),
			MessageId = StaticClass.GetMessageId(message)
		});
	}
}
