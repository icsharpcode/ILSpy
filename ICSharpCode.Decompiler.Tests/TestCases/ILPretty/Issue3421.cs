internal class Issue3421
{
	private string name;
	private object value;

	public virtual void SetValue(object value)
	{
		switch (name)
		{
			case "##Name##":
				return;
			case "##Value##":
				this.value = value;
				return;
			case "##InnerText##":
				this.value = value.ToString();
				return;
			case null:
				return;
		}
		if (this.value == null)
		{
			this.value = "";
		}
	}
}
