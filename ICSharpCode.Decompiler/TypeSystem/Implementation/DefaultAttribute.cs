// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// IAttribute implementation for already-resolved attributes.
	/// </summary>
	public class DefaultAttribute : IAttribute
	{
		volatile IMethod constructor;
		
		public DefaultAttribute(IType attributeType, IReadOnlyList<ResolveResult> positionalArguments = null,
								IReadOnlyList<KeyValuePair<IMember, ResolveResult>> namedArguments = null)
		{
			this.AttributeType = attributeType ?? throw new ArgumentNullException("attributeType");
			this.PositionalArguments = positionalArguments ?? EmptyList<ResolveResult>.Instance;
			this.NamedArguments = namedArguments ?? EmptyList<KeyValuePair<IMember, ResolveResult>>.Instance;
		}
		
		public DefaultAttribute(IMethod constructor, IReadOnlyList<ResolveResult> positionalArguments = null,
								IReadOnlyList<KeyValuePair<IMember, ResolveResult>> namedArguments = null)
		{
			this.constructor = constructor ?? throw new ArgumentNullException("constructor");
			this.AttributeType = constructor.DeclaringType ?? SpecialType.UnknownType;
			this.PositionalArguments = positionalArguments ?? EmptyList<ResolveResult>.Instance;
			this.NamedArguments = namedArguments ?? EmptyList<KeyValuePair<IMember, ResolveResult>>.Instance;
			if (this.PositionalArguments.Count != constructor.Parameters.Count) {
				throw new ArgumentException("Positional argument count must match the constructor's parameter count");
			}
		}
		
		public IType AttributeType { get; }

		public IMethod Constructor {
			get {
				var ctor = this.constructor;
				if (ctor == null) {
					foreach (var candidate in this.AttributeType.GetConstructors(m => m.Parameters.Count == PositionalArguments.Count)) {
						if (candidate.Parameters.Select(p => p.Type).SequenceEqual(this.PositionalArguments.Select(a => a.Type))) {
							ctor = candidate;
							break;
						}
					}
					this.constructor = ctor;
				}
				return ctor;
			}
		}
		
		public IReadOnlyList<ResolveResult> PositionalArguments { get; }

		public IReadOnlyList<KeyValuePair<IMember, ResolveResult>> NamedArguments { get; }
	}
}
