// Copyright (c) 2026 Siegfried Pammer
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

using ICSharpCode.ILSpy.Options;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

[TestFixture]
public sealed class DocumentationLinkPolicyTests
{
	[Test]
	public void AllowedDocumentationLinkOrigin_MatchesSchemeHostAndPort()
	{
		var settings = new MiscSettings();
		settings.AllowDocumentationLinkOrigin(new Uri("https://docs.example.com:8443/path"));

		Assert.That(settings.IsDocumentationLinkOriginAllowed(new Uri("https://DOCS.EXAMPLE.COM:8443/other")), Is.True);
		Assert.That(settings.IsDocumentationLinkOriginAllowed(new Uri("http://docs.example.com:8443/other")), Is.False);
		Assert.That(settings.IsDocumentationLinkOriginAllowed(new Uri("https://docs.example.com/other")), Is.False);
		Assert.That(settings.IsDocumentationLinkOriginAllowed(new Uri("https://sub.docs.example.com:8443/other")), Is.False);
		Assert.That(settings.IsDocumentationLinkOriginAllowed(new Uri("file:///tmp/documentation")), Is.False);

		var restored = new MiscSettings();
		restored.LoadFromXml(settings.SaveToXml());
		Assert.That(restored.IsDocumentationLinkOriginAllowed(new Uri("https://docs.example.com:8443/restored")), Is.True);
	}
}
