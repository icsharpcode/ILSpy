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

using System.Composition;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Metadata;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click → "Show in metadata" on an entity hyperlink in the decompiler view.
	/// Visible whenever the click landed on a reference whose payload is an
	/// <see cref="IEntity"/> backed by a real metadata file; firing it routes through the
	/// dock workspace to open the matching CLI metadata table at the entity's row.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources.GoToToken), Category = "Navigation", Order = 170)]
	[Shared]
	public sealed class ShowInMetadataContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => Resolve(context) is not null;

		public bool IsEnabled(TextViewContext context) => true;

		public void Execute(TextViewContext context)
		{
			if (Resolve(context) is not { } target)
				return;
			AppComposition.Current.GetExport<DockWorkspace>().NavigateToToken(target);
		}

		static MetadataTokenReference? Resolve(TextViewContext context)
		{
			if (context.TextView is null)
				return null;
			if (context.Reference?.Reference is not IEntity entity)
				return null;
			if (entity.ParentModule?.MetadataFile is not { } module)
				return null;
			if (entity.MetadataToken.IsNil)
				return null;
			return new MetadataTokenReference(module, entity.MetadataToken);
		}
	}
}
