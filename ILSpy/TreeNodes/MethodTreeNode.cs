// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Output;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.TreeNodes
{
	public sealed class MethodTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		public IMethod MethodDefinition { get; }

		public IEntity? Member => MethodDefinition;

		public MethodTreeNode(IMethod method)
		{
			MethodDefinition = method ?? throw new ArgumentNullException(nameof(method));
		}

		public override object Text => Language.EntityToString(MethodDefinition, ConversionFlags.None) + GetSuffixString(MethodDefinition);

		public override object NavigationText => Language.EntityToString(MethodDefinition, ConversionFlags.ShowDeclaringType);

		public override object Icon => GetIcon(MethodDefinition);

		// Mirrors WPF's dispatch order: operator, extension method, constructor,
		// P/Invoke (DllImport without a body), virtual, plain method. Tested against
		// every kind in the analyzers fixtures.
		public static Avalonia.Media.IImage GetIcon(IMethod method)
		{
			bool isExtensionMethod = method.IsExtensionMethod
				|| (method.ResolveExtensionInfo()?.InfoOfExtensionMember((IMethod)method.MemberDefinition).HasValue == true);

			if (method.IsOperator)
				return Images.GetIcon(Images.Operator,
					Images.GetOverlay(method.Accessibility), false, isExtensionMethod);

			if (isExtensionMethod)
				return Images.GetIcon(Images.Method,
					Images.GetOverlay(method.Accessibility), false, true);

			if (method.IsConstructor)
				return Images.GetIcon(Images.Constructor,
					Images.GetOverlay(method.Accessibility), method.IsStatic);

			// P/Invoke: extern method tagged with [DllImport]. !HasBody filters out the
			// managed wrappers that also carry the attribute via attribute-forwarding.
			if (!method.HasBody && method.HasAttribute(KnownAttribute.DllImport))
				return Images.GetIcon(Images.PInvokeMethod,
					Images.GetOverlay(method.Accessibility), true);

			return Images.GetIcon(
				method.IsVirtual ? Images.VirtualMethod : Images.Method,
				Images.GetOverlay(method.Accessibility), method.IsStatic);
		}

		public override bool ShowExpander => false;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			// When the user clicks a method that lives under an ExtensionTreeNode (the C# 14
			// extension-block container), emit the whole extension declaration via the
			// dedicated DecompileExtension path so the output reads as source-faithful
			// `extension(T) { ... }` syntax, not the lowered static method we'd get from
			// DecompileMethod. Falls through to the regular method path for everything else.
			if (Parent is ExtensionTreeNode && language is CSharpLanguage cs)
				cs.DecompileExtension(MethodDefinition, output, options);
			else
				language.DecompileMethod(MethodDefinition, output, options);
		}

		public override bool IsPublicAPI => MethodDefinition.Accessibility switch {
			Accessibility.Public or Accessibility.Protected or Accessibility.ProtectedOrInternal => true,
			_ => false,
		};

		public override FilterResult Filter(LanguageSettings settings)
		{
			if (settings.ShowApiLevel == ApiVisibility.PublicOnly && !IsPublicAPI)
				return FilterResult.Hidden;
			// Hide implementation methods of C# 14 extension blocks when ExtensionMembers is on
			// — those land as ExtensionTreeNode children of the static container instead, so
			// surfacing them on the outer class would be duplicate UI. Pair to the
			// `extension-methods-tree` work in ea56a80cd.
			if (LanguageService.CurrentLanguage is Languages.CSharpLanguage
				&& MethodDefinition.DeclaringTypeDefinition?.ExtensionInfo is { } extInfo)
			{
				var decompilerSettings = TryGetDecompilerSettings();
				if (decompilerSettings?.ExtensionMembers == true
					&& extInfo.InfoOfImplementationMember((IMethod)MethodDefinition.MemberDefinition).HasValue)
				{
					return FilterResult.Hidden;
				}
			}
			if (settings.SearchTermMatches(MethodDefinition.Name) && (settings.ShowApiLevel == ApiVisibility.All || LanguageService.CurrentLanguage.ShowMember(MethodDefinition)))
				return FilterResult.Match;
			else
				return FilterResult.Hidden;
		}

		static ICSharpCode.Decompiler.DecompilerSettings? TryGetDecompilerSettings()
		{
			try
			{
				return AppEnv.AppComposition.Current.GetExport<SettingsService>().DecompilerSettings.Clone();
			}
			catch
			{
				return null;
			}
		}

		// Stable identity for SessionSettings.ActiveTreeViewPath; format must round-trip
		// across launches so the saved path can be restored.
		public override string ToString()
			=> "Method " + new ICSharpCode.Decompiler.IL.ILAmbience {
				ConversionFlags = ConversionFlags.ShowTypeParameterList
					| ConversionFlags.PlaceReturnTypeAfterParameterList
					| ConversionFlags.ShowReturnType
					| ConversionFlags.ShowParameterList
					| ConversionFlags.ShowParameterModifiers,
			}.ConvertSymbol(MethodDefinition);
	}
}
