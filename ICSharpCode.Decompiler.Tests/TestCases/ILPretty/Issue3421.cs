internal class Issue3421
{
	private string name;
	private object value;

	public virtual void SetValue(object value)
	{
		if (name == null)
		{
			return;
		}
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
		}
		if (this.value == null)
		{
			this.value = "";
		}
	}
}
