#if !NETCORE

#nullable enable

namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true)]
	internal sealed class NotNullIfNotNullAttribute : Attribute
	{
		public string ParameterName { get; }

		public NotNullIfNotNullAttribute(string parameterName)
		{
			ParameterName = parameterName;
		}
	}

	[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
	internal sealed class NotNullWhenAttribute : Attribute
	{
		public NotNullWhenAttribute(bool returnValue)
		{
			ReturnValue = returnValue;
		}

		public bool ReturnValue { get; }
	}
}
#endif
