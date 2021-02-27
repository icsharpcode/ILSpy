// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using System.Linq;
using System.Threading;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;

using SRM = System.Reflection.Metadata;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Lists the sub types of a class.
	/// </summary>
	sealed class DerivedTypesTreeNode : ILSpyTreeNode
	{
		readonly AssemblyList list;
		readonly ITypeDefinition type;
		readonly ThreadingSupport threading;

		public DerivedTypesTreeNode(AssemblyList list, ITypeDefinition type)
		{
			this.list = list;
			this.type = type;
			this.LazyLoading = true;
			this.threading = new ThreadingSupport();
		}

		public override object Text => Resources.DerivedTypes;

		public override object Icon => Images.SubTypes;

		protected override void LoadChildren()
		{
			threading.LoadChildren(this, FetchChildren);
		}

		IEnumerable<ILSpyTreeNode> FetchChildren(CancellationToken cancellationToken)
		{
			// FetchChildren() runs on the main thread; but the enumerator will be consumed on a background thread
			return FindDerivedTypes(list, type, cancellationToken);
		}

		internal static IEnumerable<DerivedTypesEntryNode> FindDerivedTypes(AssemblyList list, ITypeDefinition type,
			CancellationToken cancellationToken)
		{
			var definitionMetadata = type.ParentModule.PEFile.Metadata;
			var metadataToken = (SRM.TypeDefinitionHandle)type.MetadataToken;
			var assemblies = list.GetAllAssemblies().GetAwaiter().GetResult()
				.Select(node => node.GetPEFileOrNull()).Where(asm => asm != null).ToArray();
			foreach (var module in assemblies)
			{
				var metadata = module.Metadata;
				var assembly = (MetadataModule)module.GetTypeSystemOrNull().MainModule;
				foreach (var h in metadata.TypeDefinitions)
				{
					cancellationToken.ThrowIfCancellationRequested();
					var td = metadata.GetTypeDefinition(h);
					foreach (var iface in td.GetInterfaceImplementations())
					{
						var ifaceImpl = metadata.GetInterfaceImplementation(iface);
						if (!ifaceImpl.Interface.IsNil && IsSameType(metadata, ifaceImpl.Interface, definitionMetadata, metadataToken))
							yield return new DerivedTypesEntryNode(list, assembly.GetDefinition(h));
					}
					SRM.EntityHandle baseType = td.GetBaseTypeOrNil();
					if (!baseType.IsNil && IsSameType(metadata, baseType, definitionMetadata, metadataToken))
					{
						yield return new DerivedTypesEntryNode(list, assembly.GetDefinition(h));
					}
				}
			}
			yield break;
		}

		static bool IsSameType(SRM.MetadataReader referenceMetadata, SRM.EntityHandle typeRef,
							   SRM.MetadataReader definitionMetadata, SRM.TypeDefinitionHandle typeDef)
		{
			// FullName contains only namespace, name and type parameter count, therefore this should suffice.
			return typeRef.GetFullTypeName(referenceMetadata) == typeDef.GetFullTypeName(definitionMetadata);
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			threading.Decompile(language, output, options, EnsureLazyChildren);
		}
	}
}