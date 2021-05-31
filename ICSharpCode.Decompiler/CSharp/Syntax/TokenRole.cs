namespace ICSharpCode.Decompiler.CSharp.Syntax
{
	/// <summary>
	/// A specific role only used for C# tokens
	/// </summary>
	public sealed class TokenRole : Role<CSharpTokenNode>
	{
		/// <summary>
		/// Gets the token as string. Note that the token Name and Token value may differ.
		/// </summary>
		public string Token { get; }

		/// <summary>
		/// Gets the char length of the token.
		/// </summary>
		public int Length { get; }

		public TokenRole(string token) : base(token, CSharpTokenNode.Null)
		{
			this.Token = token;
			this.Length = token.Length;
		}
	}
}
