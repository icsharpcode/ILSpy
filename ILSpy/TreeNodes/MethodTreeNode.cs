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
using System.Reflection.Metadata;
using System.Windows.Media;

using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpy.TreeNodes
{
	using ICSharpCode.Decompiler.Output;
	using ICSharpCode.Decompiler.TypeSystem;
	using ICSharpCode.ILSpyX;

	/// <summary>
	/// Tree Node representing a field, method, property, or event.
	/// </summary>
	public sealed class MethodTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		public IMethod MethodDefinition { get; }

		public MethodTreeNode(IMethod method)
		{
			this.MethodDefinition = method ?? throw new ArgumentNullException(nameof(method));
		}

		public override object Text => GetText(GetMethodDefinition(), Language) + GetSuffixString(MethodDefinition);

		public override object NavigationText => GetText(GetMethodDefinition(), Language, includeDeclaringTypeName: true);

		private IMethod GetMethodDefinition()
		{
			var m = ((MetadataModule)MethodDefinition.ParentModule?.MetadataFile
				?.GetTypeSystemWithCurrentOptionsOrNull(SettingsService, AssemblyTreeModel.CurrentLanguageVersion)
				?.MainModule)?.GetDefinition((MethodDefinitionHandle)MethodDefinition.MetadataToken);
			return m?.Specialize(MethodDefinition.Substitution) ?? MethodDefinition;
		}

		public static object GetText(IMethod method, Language language, bool includeDeclaringTypeName = false)
		{
			return language.EntityToString(method, includeDeclaringTypeName ? ConversionFlags.ShowDeclaringType : ConversionFlags.None);
		}

		public override object Icon => GetIcon(GetMethodDefinition());

		public static ImageSource GetIcon(IMethod method)
		{
			bool isExtensionMethod = method.ResolveExtensionInfo()?.InfoOfExtensionMember((IMethod)method.MemberDefinition).HasValue == true
				|| method.IsExtensionMethod;

			if (method.IsOperator)
				return Images.GetIcon(MemberIcon.Operator, Images.GetOverlayIcon(method.Accessibility), false, isExtensionMethod);

			if (isExtensionMethod)
				return Images.GetIcon(MemberIcon.Method, Images.GetOverlayIcon(method.Accessibility), false, true);

			if (method.IsConstructor)
				return Images.GetIcon(MemberIcon.Constructor, Images.GetOverlayIcon(method.Accessibility), method.IsStatic, false);

			if (!method.HasBody && method.HasAttribute(KnownAttribute.DllImport))
				return Images.GetIcon(MemberIcon.PInvokeMethod, Images.GetOverlayIcon(method.Accessibility), true, false);

			return Images.GetIcon(method.IsVirtual ? MemberIcon.VirtualMethod : MemberIcon.Method,
				Images.GetOverlayIcon(method.Accessibility), method.IsStatic, false);
		}

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			if (Parent is ExtensionTreeNode && language is CSharpLanguage cs)
				cs.DecompileExtension(MethodDefinition, output, options);
			else
				language.DecompileMethod(MethodDefinition, output, options);
		}

		public override FilterResult Filter(LanguageSettings settings)
		{
			if (settings.ShowApiLevel == ApiVisibility.PublicOnly && !IsPublicAPI)
				return FilterResult.Hidden;
			// hide implementation methods of extension blocks in the tree view
			if (Language is CSharpLanguage && MethodDefinition.DeclaringTypeDefinition?.ExtensionInfo is { } extInfo)
			{
				var decompilerSettings = SettingsService.DecompilerSettings.Clone();
				if (!Enum.TryParse(LanguageService.LanguageVersion?.Version, out Decompiler.CSharp.LanguageVersion csharpLanguageVersion))
					csharpLanguageVersion = Decompiler.CSharp.LanguageVersion.Latest;
				decompilerSettings.SetLanguageVersion(csharpLanguageVersion);
				if (decompilerSettings.ExtensionMembers && extInfo.InfoOfImplementationMember((IMethod)MethodDefinition.MemberDefinition).HasValue)
					return FilterResult.Hidden;
			}
			if (settings.SearchTermMatches(MethodDefinition.Name) && (settings.ShowApiLevel == ApiVisibility.All || LanguageService.Language.ShowMember(MethodDefinition)))
				return FilterResult.Match;
			else
				return FilterResult.Hidden;
		}

		public override bool IsPublicAPI {
			get {
				switch (GetMethodDefinition().Accessibility)
				{
					case Accessibility.Public:
					case Accessibility.Protected:
					case Accessibility.ProtectedOrInternal:
						return true;
					default:
						return false;
				}
			}
		}

		IEntity IMemberTreeNode.Member => MethodDefinition;

		public override string ToString()
		{
			return "Method " + LanguageService.ILLanguage.EntityToString(MethodDefinition, ConversionFlags.None);
		}
	}
}
