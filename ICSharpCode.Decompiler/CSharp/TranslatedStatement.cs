using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.IL;

namespace ICSharpCode.Decompiler.CSharp
{
	[DebuggerDisplay("{Statement}")]
	struct TranslatedStatement
	{
		public readonly Statement Statement;

		public IEnumerable<ILInstruction> ILInstructions {
			get { return Statement.Annotations.OfType<ILInstruction>(); }
		}

		internal TranslatedStatement(Statement statement)
		{
			Debug.Assert(statement != null);
			this.Statement = statement;
		}

		public static implicit operator Statement(TranslatedStatement statement)
		{
			return statement.Statement;
		}
	}
}
