using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using Mono.Cecil;

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

	public class ReferenceLoadInfo
	{
		readonly Dictionary<string, UnresolvedAssemblyNameReference> loadedAssemblyReferences = new Dictionary<string, UnresolvedAssemblyNameReference>();

		public void AddMessage(string fullName, MessageKind kind, string message)
		{
			lock (loadedAssemblyReferences) {
				if (!loadedAssemblyReferences.TryGetValue(fullName, out var referenceInfo)) {
					referenceInfo = new UnresolvedAssemblyNameReference(fullName);
					loadedAssemblyReferences.Add(fullName, referenceInfo);
				}
				referenceInfo.Messages.Add((kind, message));
			}
		}

		public void AddMessageOnce(string fullName, MessageKind kind, string message)
		{
			lock (loadedAssemblyReferences) {
				if (!loadedAssemblyReferences.TryGetValue(fullName, out var referenceInfo)) {
					referenceInfo = new UnresolvedAssemblyNameReference(fullName);
					loadedAssemblyReferences.Add(fullName, referenceInfo);
					referenceInfo.Messages.Add((kind, message));
				} else {
					var lastMsg = referenceInfo.Messages.LastOrDefault();
					if (kind != lastMsg.Item1 && message != lastMsg.Item2)
						referenceInfo.Messages.Add((kind, message));
				}
			}
		}

		public bool TryGetInfo(string fullName, out UnresolvedAssemblyNameReference info)
		{
			lock (loadedAssemblyReferences) {
				return loadedAssemblyReferences.TryGetValue(fullName, out info);
			}
		}

		public bool HasErrors {
			get {
				lock (loadedAssemblyReferences) {
					return loadedAssemblyReferences.Any(i => i.Value.HasErrors);
				}
			}
		}
	}
}
