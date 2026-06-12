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
	public sealed class EventTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		public IEvent EventDefinition { get; }

		public IEntity? Member => EventDefinition;

		public EventTreeNode(IEvent ev)
		{
			EventDefinition = ev ?? throw new ArgumentNullException(nameof(ev));
			if (ev.CanAdd)
				Children.Add(new MethodTreeNode(ev.AddAccessor));
			if (ev.CanRemove)
				Children.Add(new MethodTreeNode(ev.RemoveAccessor));
			if (ev.CanInvoke)
				Children.Add(new MethodTreeNode(ev.InvokeAccessor));
		}

		public override object Text => Language.EntityToString(EventDefinition, ConversionFlags.None) + GetSuffixString(EventDefinition);

		public override object NavigationText => Language.EntityToString(EventDefinition, ConversionFlags.ShowDeclaringType);

		public override object Icon => Images.GetIcon(Images.Event,
			Images.GetOverlay(EventDefinition.Accessibility), EventDefinition.IsStatic);

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
			=> language.DecompileEvent(EventDefinition, output, options);

		public override bool IsPublicAPI => EventDefinition.Accessibility switch {
			Accessibility.Public or Accessibility.Protected or Accessibility.ProtectedOrInternal => true,
			_ => false,
		};

		public override FilterResult Filter(LanguageSettings settings)
		{
			if (settings.ShowApiLevel == ApiVisibility.PublicOnly && !IsPublicAPI)
				return FilterResult.Hidden;
			if (settings.SearchTermMatches(EventDefinition.Name) && (settings.ShowApiLevel == ApiVisibility.All || LanguageService.CurrentLanguage.ShowMember(EventDefinition)))
				return FilterResult.Match;
			else
				return FilterResult.Hidden;
		}

		public override string ToString() => "Event " + EventDefinition.Name;
	}
}
