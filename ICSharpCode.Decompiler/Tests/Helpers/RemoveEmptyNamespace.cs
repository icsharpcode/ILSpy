using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.NRefactory.CSharp;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	class RemoveEmptyNamespace : DepthFirstAstVisitor<object, object>
	{
		public override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data)
		{
			if (namespaceDeclaration.FullName.Length == 0) {
				namespaceDeclaration.Remove();
				return null;
			}
			return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
		}

		public void Run(AstNode node)
		{
			node.AcceptVisitor(this, null);
		}
	}
}
