#nullable enable

namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// Identifies a C# token in the output. Tokens are not AST nodes; this is a printer-side descriptor
	/// carrying the token text, and its instance identity lets the token writers single out specific
	/// tokens (e.g. structural braces, the constructor this/base keyword, the override modifier).
	/// </summary>
	public sealed class TokenRole
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
	}
}
