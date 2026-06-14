#nullable enable

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// A role identifying a C# token in the output. Tokens are no longer AST nodes, so this role
	/// never holds a child; it only carries the token text for the output visitor.
	/// </summary>
	public sealed class TokenRole : Role
	{
		/// <summary>
		/// Gets the token as string. Note that the token Name and Token value may differ.
		/// </summary>
		public string Token { get; }

		/// <summary>
		/// Gets the char length of the token.
		/// </summary>
		public int Length { get; }

		public TokenRole(string token)
		{
			this.Token = token;
			this.Length = token.Length;
		}

		public override bool IsValid(object node) => false;

		internal override object? NullObjectUntyped => null;
	}
}
