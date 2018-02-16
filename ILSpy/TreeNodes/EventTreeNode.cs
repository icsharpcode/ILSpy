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
using System.Linq;
using System.Reflection;
using SRM = System.Reflection.Metadata;
using System.Windows.Media;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

namespace ICSharpCode.ILSpy.TreeNodes
{
	/// <summary>
	/// Represents an event in the TreeView.
	/// </summary>
	public sealed class EventTreeNode : ILSpyTreeNode, IMemberTreeNode
	{
		public EventTreeNode(EventDefinition ev)
		{
			if (ev.IsNil)
				throw new ArgumentNullException(nameof(ev));
			this.EventDefinition = ev;
			var metadata = ev.Module.GetMetadataReader();
			var eventDefinition = metadata.GetEventDefinition(ev.Handle);
			var accessors = eventDefinition.GetAccessors();
			if (!accessors.Adder.IsNil)
				this.Children.Add(new MethodTreeNode(new MethodDefinition(ev.Module, accessors.Adder)));
			if (!accessors.Remover.IsNil)
				this.Children.Add(new MethodTreeNode(new MethodDefinition(ev.Module, accessors.Remover)));
			if (!accessors.Raiser.IsNil)
				this.Children.Add(new MethodTreeNode(new MethodDefinition(ev.Module, accessors.Raiser)));
			//foreach (var m in ev.OtherMethods)
			//	this.Children.Add(new MethodTreeNode(m));
		}

		public EventDefinition EventDefinition { get; }

		public override object Text => GetText(EventDefinition, this.Language) + EventDefinition.Handle.ToSuffixString();

		public static object GetText(EventDefinition ev, Language language)
		{
			var metadata = ev.Module.GetMetadataReader();
			var eventDefinition = metadata.GetEventDefinition(ev.Handle);
			var accessors = eventDefinition.GetAccessors();
			SRM.TypeDefinitionHandle declaringType;
			if (!accessors.Adder.IsNil) {
				declaringType = metadata.GetMethodDefinition(accessors.Adder).GetDeclaringType();
			} else if (!accessors.Remover.IsNil) {
				declaringType = metadata.GetMethodDefinition(accessors.Remover).GetDeclaringType();
			} else {
				declaringType = metadata.GetMethodDefinition(accessors.Raiser).GetDeclaringType();
			}
			var eventType = eventDefinition.DecodeSignature(metadata, language.CreateSignatureTypeProvider(false), new GenericContext(declaringType, ev.Module));
			return HighlightSearchMatch(metadata.GetString(eventDefinition.Name), " : " + eventType);
		}

		public override object Icon => GetIcon(EventDefinition);

		public static ImageSource GetIcon(EventDefinition @event)
		{
			var metadata = @event.Module.GetMetadataReader();
			var accessor = metadata.GetEventDefinition(@event.Handle).GetAccessors().GetAny();
			if (!accessor.IsNil) {
				var accessorMethod = metadata.GetMethodDefinition(accessor);
				return Images.GetIcon(MemberIcon.Event, GetOverlayIcon(accessorMethod.Attributes), accessorMethod.HasFlag(MethodAttributes.Static));
			}
			return Images.GetIcon(MemberIcon.Event, AccessOverlayIcon.Public, false);
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

		public override FilterResult Filter(FilterSettings settings)
		{
			if (!settings.ShowInternalApi && !IsPublicAPI)
				return FilterResult.Hidden;
			var metadata = EventDefinition.Module.GetMetadataReader();
			if (settings.SearchTermMatches(metadata.GetString(metadata.GetEventDefinition(EventDefinition.Handle).Name)) && settings.Language.ShowMember(EventDefinition))
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
				var metadata = EventDefinition.Module.GetMetadataReader();
				var accessor = metadata.GetEventDefinition(EventDefinition.Handle).GetAccessors().GetAny();
				if (accessor.IsNil) return false;
				var accessorMethod = metadata.GetMethodDefinition(accessor);
				switch (accessorMethod.Attributes & MethodAttributes.MemberAccessMask) {
					case MethodAttributes.Public:
					case MethodAttributes.FamORAssem:
					case MethodAttributes.Family:
						return true;
					default:
						return false;
				}
			}
		}

		IMetadataEntity IMemberTreeNode.Member => EventDefinition;
	}
}
