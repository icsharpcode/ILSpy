/*
	Copyright (c) 2015 Ki

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace ILSpy.BamlDecompiler.Baml
{
	internal partial class KnownThings
	{
		readonly IDecompilerTypeSystem typeSystem;

		readonly Dictionary<int, IModule> assemblies;
		readonly Dictionary<KnownMembers, KnownMember> members;
		readonly Dictionary<KnownTypes, ITypeDefinition> types;
		readonly Dictionary<int, string> strings;
		readonly Dictionary<int, (string, string, string)> resources;

		public KnownThings(IDecompilerTypeSystem typeSystem)
		{
			this.typeSystem = typeSystem;

			assemblies = new Dictionary<int, IModule>();
			types = new Dictionary<KnownTypes, ITypeDefinition>();
			members = new Dictionary<KnownMembers, KnownMember>();
			strings = new Dictionary<int, string>();
			resources = new Dictionary<int, (string, string, string)>();

			try
			{
				InitAssemblies();
				InitTypes();
				InitMembers();
				InitStrings();
				InitResources();
			}
			catch (Exception ex)
			{
				throw new ICSharpCode.Decompiler.DecompilerException(typeSystem.MainModule.PEFile, ex.Message, ex);
			}
		}

		public Func<KnownTypes, ITypeDefinition> Types => id => types[id];
		public Func<KnownMembers, KnownMember> Members => id => members[id];
		public Func<short, string> Strings => id => strings[id];
		public Func<short, (string, string, string)> Resources => id => resources[id];
		public IModule FrameworkAssembly => assemblies[0];
		IModule ResolveAssembly(string name)
		{
			IModule module = typeSystem.Modules.FirstOrDefault(m => m.AssemblyName == name);
			if (module == null)
				throw new Exception("Could not resolve known assembly '" + name + "'!");
			return module;
		}

		ITypeDefinition InitType(IModule assembly, string ns, string name) => assembly.GetTypeDefinition(new TopLevelTypeName(ns, name));
		KnownMember InitMember(KnownTypes parent, string name, ITypeDefinition type) => new KnownMember(parent, types[parent], name, type);
	}

	internal class KnownMember
	{
		public KnownMember(KnownTypes parent, ITypeDefinition declType, string name, ITypeDefinition type)
		{
			Parent = parent;
			Property = declType.GetProperties(p => p.Name == name, GetMemberOptions.IgnoreInheritedMembers).SingleOrDefault();
			DeclaringType = declType;
			Name = name;
			Type = type;
		}

		public KnownTypes Parent { get; }
		public ITypeDefinition DeclaringType { get; }
		public IProperty Property { get; }
		public string Name { get; }
		public ITypeDefinition Type { get; }
	}
}