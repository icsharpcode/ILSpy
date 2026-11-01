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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.ILSpyX.Settings;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Cross-cutting application-level toggles that don't fit Decompiler or Display.
	/// Persisted under the <c>&lt;MiscSettings/&gt;</c> XML section; XML attribute names
	/// and defaults match the WPF host so saved settings round-trip across platforms.
	/// </summary>
	public sealed partial class MiscSettings : ObservableObject, ISettingsSection
	{
		[ObservableProperty]
		bool allowMultipleInstances;

		[ObservableProperty]
		bool loadPreviousAssemblies = true;

		readonly HashSet<string> allowedDocumentationLinkOrigins = new(StringComparer.OrdinalIgnoreCase);

		public XName SectionName => "MiscSettings";

		public bool IsDocumentationLinkOriginAllowed(Uri uri)
		{
			return TryGetDocumentationLinkOrigin(uri) is { } origin
				&& allowedDocumentationLinkOrigins.Contains(origin);
		}

		public void AllowDocumentationLinkOrigin(Uri uri)
		{
			if (TryGetDocumentationLinkOrigin(uri) is { } origin)
				allowedDocumentationLinkOrigins.Add(origin);
		}

		public void ClearDocumentationLinkOrigins()
		{
			allowedDocumentationLinkOrigins.Clear();
		}

		static string? TryGetDocumentationLinkOrigin(Uri uri)
		{
			if (!uri.IsAbsoluteUri || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
				return null;

			int defaultPort = uri.Scheme == Uri.UriSchemeHttp ? 80 : 443;
			string host = uri.IdnHost.ToLowerInvariant();
			if (host.Length == 0)
				return null;
			if (host.Contains(':'))
				host = "[" + host + "]";
			return uri.Scheme.ToLowerInvariant() + "://" + host
				+ (uri.Port == defaultPort ? string.Empty : ":" + uri.Port);
		}

		public void LoadFromXml(XElement e)
		{
			AllowMultipleInstances = (bool?)e.Attribute(nameof(AllowMultipleInstances)) ?? false;
			LoadPreviousAssemblies = (bool?)e.Attribute(nameof(LoadPreviousAssemblies)) ?? true;
			allowedDocumentationLinkOrigins.Clear();
			foreach (var origin in e.Elements("DocumentationLinkOrigin").Select(e => (string?)e.Attribute("Value")))
			{
				if (Uri.TryCreate(origin, UriKind.Absolute, out var uri) && TryGetDocumentationLinkOrigin(uri) is { } normalized)
					allowedDocumentationLinkOrigins.Add(normalized);
			}
		}

		public XElement SaveToXml()
		{
			var section = new XElement(SectionName);
			section.SetAttributeValue(nameof(AllowMultipleInstances), AllowMultipleInstances);
			section.SetAttributeValue(nameof(LoadPreviousAssemblies), LoadPreviousAssemblies);
			foreach (var origin in allowedDocumentationLinkOrigins.OrderBy(origin => origin, StringComparer.OrdinalIgnoreCase))
				section.Add(new XElement("DocumentationLinkOrigin", new XAttribute("Value", origin)));
			return section;
		}
	}
}
