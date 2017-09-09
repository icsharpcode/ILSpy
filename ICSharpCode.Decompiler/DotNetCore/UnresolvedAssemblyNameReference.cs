using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler
{
	public sealed class UnresolvedAssemblyNameReference
	{
		public string FullName { get; }

		public bool HasErrors => Messages.Any(m => m.Item1 == MessageKind.Error);

		public List<(MessageKind, string)> Messages { get; } = new List<(MessageKind, string)>();

		public UnresolvedAssemblyNameReference(string fullName)
		{
			this.FullName = fullName;
		}
	}

	public enum MessageKind { Error, Warning, Info }
}
