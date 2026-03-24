namespace CrossAssemblyDep
{
	public class BaseClass
	{
		private string name = "BaseClass";

		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}

		public T Convert<T>(T input) { return input; }
	}
}
