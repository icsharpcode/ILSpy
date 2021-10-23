// Dummy types so that we can use compile some ICS.Decompiler classes in the AddIn context
// without depending on SRM etc.

using System;
using System.Collections.Generic;
using System.Text;

namespace ICSharpCode.Decompiler
{
	public class ReferenceLoadInfo
	{
		public void AddMessage(params object[] args) { }
	}

	enum MessageKind { Warning }

	public static class MetadataExtensions
	{
		public static string ToHexString(this IEnumerable<byte> bytes, int estimatedLength)
		{
			if (bytes == null)
				throw new ArgumentNullException(nameof(bytes));

			StringBuilder sb = new StringBuilder(estimatedLength * 2);
			foreach (var b in bytes)
				sb.AppendFormat("{0:x2}", b);
			return sb.ToString();
		}
	}
}
