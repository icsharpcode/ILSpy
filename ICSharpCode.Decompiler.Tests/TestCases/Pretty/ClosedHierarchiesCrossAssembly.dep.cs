namespace CrossAssemblyClosed
{
	public closed record Vehicle;
	public record Car(int Doors) : Vehicle;
	public sealed record Truck(double PayloadTons) : Vehicle;
}

// Public so that the main test assembly can use the 'closed' modifier without
// declaring its own copy of the attribute (the shape the BCL will provide once
// ClosedAttribute ships).
namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ClosedAttribute : Attribute
	{
	}
}
