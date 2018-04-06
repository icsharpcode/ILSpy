using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace ICSharpCode.Decompiler.Metadata
{
	public static class DotNetCorePathFinderExtensions
	{
		public static string DetectTargetFrameworkId(this PEReader assembly)
		{
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));

			const string TargetFrameworkAttributeName = "System.Runtime.Versioning.TargetFrameworkAttribute";
			var reader = assembly.GetMetadataReader();

			foreach (var h in reader.GetCustomAttributes(Handle.AssemblyDefinition)) {
				var attribute = reader.GetCustomAttribute(h);
				if (attribute.GetAttributeType(reader).GetFullTypeName(reader).ToString() != TargetFrameworkAttributeName)
					continue;
				var blobReader = reader.GetBlobReader(attribute.Value);
				if (blobReader.ReadUInt16() == 0x0001) {
					return blobReader.ReadSerializedString();
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
