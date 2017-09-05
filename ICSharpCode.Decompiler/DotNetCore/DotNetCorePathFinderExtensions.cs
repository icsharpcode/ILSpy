using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using Mono.Cecil;
using Newtonsoft.Json.Linq;

namespace ICSharpCode.Decompiler
{

	public static class DotNetCorePathFinderExtensions
	{
		public static string DetectTargetFrameworkId(this AssemblyDefinition assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			const string TargetFrameworkAttributeName = "System.Runtime.Versioning.TargetFrameworkAttribute";

			foreach (var attribute in assembly.CustomAttributes) {
				if (attribute.AttributeType.FullName != TargetFrameworkAttributeName)
					continue;
				var blobReader = new BlobReader(attribute.GetBlob(), null);
				if (blobReader.ReadUInt16() == 0x0001) {
					return blobReader.ReadSerString();
				}
			}

			return string.Empty;
		}
	}
}
