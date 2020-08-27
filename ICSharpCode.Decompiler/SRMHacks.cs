using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.Decompiler
{
	public static partial class SRMExtensions
	{
		public static ImmutableArray<MethodImplementationHandle> GetMethodImplementations(
			this MethodDefinitionHandle handle, MetadataReader reader)
		{
			var resultBuilder = ImmutableArray.CreateBuilder<MethodImplementationHandle>();
			var typeDefinition = reader.GetTypeDefinition(reader.GetMethodDefinition(handle)
				.GetDeclaringType());

			foreach (var methodImplementationHandle in typeDefinition.GetMethodImplementations())
			{
				var methodImplementation = reader.GetMethodImplementation(methodImplementationHandle);
				if (methodImplementation.MethodBody == handle)
				{
					resultBuilder.Add(methodImplementationHandle);
				}
			}

			return resultBuilder.ToImmutable();
		}
	}
}
