// Copyright (c) Cristian Civera (cristian@aspitalia.com)
// This code is distributed under the MS-PL (for details please see \doc\MS-PL.txt)


namespace Ricciolo.StylesExplorer.MarkupReflection
{
	public interface IDotNetTypeResolver
	{
		ICSharpCode.Decompiler.Metadata.TargetRuntime RuntimeVersion { get; }
		bool IsLocalAssembly(string name);
		IDotNetType GetTypeByAssemblyQualifiedName(string name);
		IDependencyPropertyDescriptor GetDependencyPropertyDescriptor(string name, IDotNetType ownerType, IDotNetType targetType);
	}
}
