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
using System.Windows.Media;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Represents an event in the TreeView.
	/// </summary>
	public sealed class EventTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		public EventTreeNode(IEvent @event)
		{
			this.EventDefinition = @event ?? throw new ArgumentNullException(nameof(@event));
			if (@event.CanAdd)
				this.Children.Add(new MethodTreeNode(@event.AddAccessor));
			if (@event.CanRemove)
				this.Children.Add(new MethodTreeNode(@event.RemoveAccessor));
			if (@event.CanInvoke)
				this.Children.Add(new MethodTreeNode(@event.InvokeAccessor));
			//foreach (var m in ev.OtherMethods)
			//	this.Children.Add(new MethodTreeNode(m));
		}

		public IEvent EventDefinition { get; }

		public override object Text => GetText(EventDefinition, this.Language) + EventDefinition.MetadataToken.ToSuffixString();

		public static object GetText(IEvent ev, Language language)
		{
			return language.EventToString(ev, false, false, false);
		}

		public override object Icon => GetIcon(EventDefinition);

		public static object GetIcon(IEvent @event)
		{
			return Images.GetIcon(MemberIcon.Event, MethodTreeNode.GetOverlayIcon(@event.Accessibility), @event.IsStatic);
		}

		public override FilterResult Filter(FilterSettings settings)
		{
			if (settings.ShowApiLevel == ApiVisibility.PublicOnly && !IsPublicAPI)
				return FilterResult.Hidden;
			if (settings.SearchTermMatches(EventDefinition.Name) && (settings.ShowApiLevel == ApiVisibility.All || settings.Language.ShowMember(EventDefinition)))
				return FilterResult.Match;
			else
				return FilterResult.Hidden;
		}
		
		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.DecompileEvent(EventDefinition, output, options);
		}
		
		public override bool IsPublicAPI {
			get {
				switch (EventDefinition.Accessibility) {
					case Accessibility.Public:
					case Accessibility.ProtectedOrInternal:
					case Accessibility.Protected:
						return true;
					default:
						return false;
				}
			}
		}

		IEntity IMemberTreeNode.Member => EventDefinition;
	}
}
