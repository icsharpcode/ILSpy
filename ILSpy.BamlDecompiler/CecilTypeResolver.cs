// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)

using System;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy;
using Ricciolo.StylesExplorer.MarkupReflection;

namespace ILSpy.BamlDecompiler
{
	/// <summary>
	/// Description of CecilTypeResolver.
	/// </summary>
	public class NRTypeResolver : IDotNetTypeResolver
	{
		readonly PEFile module;
		readonly DecompilerTypeSystem typeSystem;
	
		public NRTypeResolver(PEFile module, IAssemblyResolver resolver)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.typeSystem = new DecompilerTypeSystem(module, resolver);
		}
		
		public bool IsLocalAssembly(string name)
		{
			return MakeShort(name) == typeSystem.MainModule.AssemblyName;
		}
		
		string MakeShort(string name)
		{
			int endOffset = name.IndexOf(',');
			if (endOffset == -1)
				return name;
			
			return name.Substring(0, endOffset);
		}
		
		public IDotNetType GetTypeByAssemblyQualifiedName(string name)
		{
			int bracket = name.LastIndexOf(']');
			int comma = bracket > -1 ? name.IndexOf(',', bracket) : name.IndexOf(',');
			
			if (comma == -1)
				throw new ArgumentException("invalid name");

			string fullName = bracket > -1 ? name.Substring(0, name.IndexOf('[')) : name.Substring(0, comma);
			string assemblyName = name.Substring(comma + 1).Trim();

			var type = typeSystem.FindType(new FullTypeName(fullName)).GetDefinition();
			
			if (type == null)
				return new UnresolvableType(name);
			
			return new NRType(type);
		}
		
		public IDependencyPropertyDescriptor GetDependencyPropertyDescriptor(string name, IDotNetType ownerType, IDotNetType targetType)
		{
			if (ownerType == null)
				throw new ArgumentNullException("ownerType");
			
			if (ownerType is NRType)
				return new NRTypeDependencyPropertyDescriptor(((NRType)ownerType).Type, name);
			if (ownerType is UnresolvableType)
				return new UnresolvableDependencyPropertyDescriptor();
			
			throw new ArgumentException("Invalid IType: " + ownerType.GetType());
		}
		
		public TargetRuntime RuntimeVersion {
			get {
				return module.GetRuntime();
			}
		}
	}
}
