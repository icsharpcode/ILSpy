// Copyright (c) Cristian Civera (cristian@aspitalia.com)
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)


namespace Ricciolo.StylesExplorer.MarkupReflection
{
	/// <summary>
	/// Interface representing a DotNet type
	/// </summary>
	public interface IDotNetType
	{
		IDotNetType BaseType { get; }
		string AssemblyQualifiedName { get; }
		bool IsSubclassOf(IDotNetType type);
		bool Equals(IDotNetType type);
	}

	public class UnresolvableType : IDotNetType
	{
		public UnresolvableType(string assemblyQualifiedName)
		{
			this.AssemblyQualifiedName = assemblyQualifiedName;
		}

		public IDotNetType BaseType => null;

		public string AssemblyQualifiedName { get; }

		public bool IsSubclassOf(IDotNetType type)
		{
			return Equals(type);
		}

		public bool Equals(IDotNetType type)
		{
			return type is UnresolvableType && type.AssemblyQualifiedName == AssemblyQualifiedName;
		}
	}
}
