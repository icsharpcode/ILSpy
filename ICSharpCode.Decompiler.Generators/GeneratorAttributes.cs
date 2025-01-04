using System;

namespace ICSharpCode.Decompiler.CSharp.Syntax;

public class DecompilerAstNodeAttribute(bool hasNullNode) : Attribute
{
	public bool HasNullNode { get; } = hasNullNode;
}