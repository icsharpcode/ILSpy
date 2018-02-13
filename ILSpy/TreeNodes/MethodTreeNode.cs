// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Dom;
using SRM = System.Reflection.Metadata;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Tree Node representing a field, method, property, or event.
	/// </summary>
	public sealed class MethodTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		public MethodDefinition MethodDefinition { get; }

		public MethodTreeNode(MethodDefinition method)
		{
			if (method.IsNil)
				throw new ArgumentNullException(nameof(method));
			this.MethodDefinition = method;
		}

		public override object Text => GetText(MethodDefinition, Language) + MethodDefinition.Handle.ToSuffixString();

		public static object GetText(MethodDefinition method, Language language)
		{
			var b = new StringBuilder();
			var signatureProvider = language.CreateSignatureTypeProvider(false);
			var signature = method.DecodeSignature(signatureProvider, new GenericContext(method));

			b.Append('(');
			for (int i = 0; i < signature.ParameterTypes.Length; i++) {
				if (i > 0)
					b.Append(", ");
				b.Append(signature.ParameterTypes[i]);
			}
			if (signature.Header.CallingConvention == SRM.SignatureCallingConvention.VarArgs) {
				if (signature.ParameterTypes.Length > 0)
					b.Append(", ");
				b.Append("...");
			}
			if (method.IsConstructor) {
				b.Append(')');
			} else {
				b.Append(") : ");
				b.Append(signature.ReturnType);
			}
			return HighlightSearchMatch(language.FormatMethodName(method), b.ToString());
		}

		public override object Icon => GetIcon(MethodDefinition);

		public static ImageSource GetIcon(MethodDefinition method)
		{
			if (method.HasFlag(MethodAttributes.SpecialName) && method.Name.StartsWith("op_", StringComparison.Ordinal)) {
				return Images.GetIcon(MemberIcon.Operator, GetOverlayIcon(method.Attributes), false);
			}

			if (method.IsExtensionMethod) {
				return Images.GetIcon(MemberIcon.ExtensionMethod, GetOverlayIcon(method.Attributes), false);
			}

			if (method.HasFlag(MethodAttributes.SpecialName) &&
				(method.Name == ".ctor" || method.Name == ".cctor")) {
				return Images.GetIcon(MemberIcon.Constructor, GetOverlayIcon(method.Attributes), method.HasFlag(MethodAttributes.Static));
			}

			if (method.HasPInvokeInfo)
				return Images.GetIcon(MemberIcon.PInvokeMethod, GetOverlayIcon(method.Attributes), true);

			bool showAsVirtual = method.HasFlag(MethodAttributes.Virtual) && !(method.HasFlag(MethodAttributes.NewSlot) && method.HasFlag(MethodAttributes.Final)) && !method.DeclaringType.IsInterface;

			return Images.GetIcon(
				showAsVirtual ? MemberIcon.VirtualMethod : MemberIcon.Method,
				GetOverlayIcon(method.Attributes),
				method.HasFlag(MethodAttributes.Static));
		}

		private static AccessOverlayIcon GetOverlayIcon(MethodAttributes methodAttributes)
		{
			switch (methodAttributes & MethodAttributes.MemberAccessMask) {
				case MethodAttributes.Public:
					return AccessOverlayIcon.Public;
				case MethodAttributes.Assembly:
					return AccessOverlayIcon.Internal;
				case MethodAttributes.FamANDAssem:
					return AccessOverlayIcon.PrivateProtected;
				case MethodAttributes.Family:
					return AccessOverlayIcon.Protected;
				case MethodAttributes.FamORAssem:
					return AccessOverlayIcon.ProtectedInternal;
				case MethodAttributes.Private:
					return AccessOverlayIcon.Private;
				case 0:
					return AccessOverlayIcon.CompilerControlled;
				default:
					throw new NotSupportedException();
			}
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.DecompileMethod(MethodDefinition, output, options);
		}

		public override FilterResult Filter(FilterSettings settings)
		{
			if (!settings.ShowInternalApi && !IsPublicAPI)
				return FilterResult.Hidden;
			if (settings.SearchTermMatches(MethodDefinition.Name) && settings.Language.ShowMember(MethodDefinition))
				return FilterResult.Match;
			else
				return FilterResult.Hidden;
		}

		public override bool IsPublicAPI {
			get {
				switch (MethodDefinition.Attributes & MethodAttributes.MemberAccessMask) {
					case MethodAttributes.Public:
					case MethodAttributes.Family:
					case MethodAttributes.FamORAssem:
						return true;
					default:
						return false;
				}
			}
		}

		IMemberReference IMemberTreeNode.Member => MethodDefinition;
	}
}
