using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.Decompiler.CSharp.Transforms;

namespace ICSharpCode.Decompiler.Tests.Helpers
{
	class RemoveCompilerAttribute : DepthFirstAstVisitor<object, object>, IAstTransform
	{
		public override object VisitAttribute(NRefactory.CSharp.Attribute attribute, object data)
		{
			var section = (AttributeSection)attribute.Parent;
			SimpleType type = attribute.Type as SimpleType;
			if (section.AttributeTarget == "assembly" &&
				(type.Identifier == "CompilationRelaxations" || type.Identifier == "RuntimeCompatibility" || type.Identifier == "SecurityPermission" || type.Identifier == "AssemblyVersion" || type.Identifier == "Debuggable"))
			{
				attribute.Remove();
				if (section.Attributes.Count == 0)
					section.Remove();
			}
			if (section.AttributeTarget == "module" && type.Identifier == "UnverifiableCode")
			{
				attribute.Remove();
				if (section.Attributes.Count == 0)
					section.Remove();
			}
			return null;
		}

		public void Run(AstNode rootNode, TransformContext context)
		{
			rootNode.AcceptVisitor(this, null);
		}
	}
	
	class EscapeGeneratedIdentifiers : DepthFirstAstVisitor
	{
		bool IsValid(char ch)
		{
			if (char.IsLetterOrDigit(ch))
				return true;
			if (ch == '_')
				return true;
			return false;
		}
		
		string ReplaceInvalid(string s)
		{
			return string.Concat(s.Select(ch => IsValid(ch) ? ch.ToString() : string.Format("_{0:0000X}", (int)ch)));
		}
		
		public override void VisitIdentifier(Identifier identifier)
		{
			identifier.Name = ReplaceInvalid(identifier.Name);
		}

		public void Run(AstNode node)
		{
			node.AcceptVisitor(this);
		}
	}
}
