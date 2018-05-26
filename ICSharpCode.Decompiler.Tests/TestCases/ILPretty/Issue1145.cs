using System;

public sealed class EvType : MulticastDelegate
{

}

[Serializable]
public class OwningClass
{
	public event EvType EvName;
}
