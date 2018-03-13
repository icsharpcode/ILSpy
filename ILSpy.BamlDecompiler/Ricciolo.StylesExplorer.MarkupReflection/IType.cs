// Copyright (c) Cristian Civera (cristian@aspitalia.com)
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)


namespace Ricciolo.StylesExplorer.MarkupReflection
{
	/// <summary>
	/// Interface representing a DotNet type
	/// </summary>
	public interface IType
	{
		IType BaseType { get; }
		string AssemblyQualifiedName { get; }
		bool IsSubclassOf(IType type);
		bool Equals(IType type);
	}
	
	public class UnresolvableType : IType
	{
		public UnresolvableType(string assemblyQualifiedName)
		{
			this.AssemblyQualifiedName = assemblyQualifiedName;
		}
		
		public IType BaseType => null;

		public string AssemblyQualifiedName { get; }

		public bool IsSubclassOf(IType type)
		{
			return Equals(type);
		}
		
		public bool Equals(IType type)
		{
			return type is UnresolvableType && type.AssemblyQualifiedName == AssemblyQualifiedName;
		}
	}
}
