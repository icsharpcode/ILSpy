using System;

namespace Issue1157
{
	internal abstract class BaseClass
	{
		public abstract event EventHandler IDontKnowMeHereOut;
	}

	internal class OtherClass : BaseClass
	{
		public override event EventHandler IDontKnowMeHereOut;
	}
}