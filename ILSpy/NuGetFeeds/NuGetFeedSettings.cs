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

namespace ICSharpCode.ILSpy.NuGetFeeds
{
	/// <summary>
	/// Persisted state of the "Open from NuGet feed" dialog: the MRU list of feed URLs the
	/// user has searched (always including the official nuget.org V3 feed), the feed that
	/// was active when the dialog was last used, and the pre-release toggle. The selected
	/// feed is guaranteed to be one of <see cref="Feeds"/>, and the list is bounded by
	/// <see cref="MaxFeeds"/> with the default feed exempt from eviction.
	/// </summary>
	public sealed partial class NuGetFeedSettings : ObservableObject, ISettingsSection
	{
		public const string DefaultFeed = "https://api.nuget.org/v3/index.json";

		/// <summary>
		/// Upper bound of the feed MRU. Eviction drops the oldest custom feed first; the
		/// default feed never leaves the list.
		/// </summary>
		public const int MaxFeeds = 10;

		readonly List<string> feeds = new() { DefaultFeed };

		[ObservableProperty]
		string selectedFeed = DefaultFeed;

		[ObservableProperty]
		bool includePrerelease;

		public XName SectionName => "NuGetFeedSettings";

		public IReadOnlyList<string> Feeds => feeds;

		/// <summary>
		/// Adds <paramref name="feedUrl"/> to the feed MRU (most recent last). Blank input
		/// and case-insensitive duplicates are ignored; when the cap is hit, the oldest
		/// custom feed is evicted.
		/// </summary>
		public void RememberFeed(string? feedUrl)
		{
			feedUrl = feedUrl?.Trim();
			if (string.IsNullOrEmpty(feedUrl))
				return;
			if (feeds.Any(f => string.Equals(f, feedUrl, StringComparison.OrdinalIgnoreCase)))
				return;
			feeds.Add(feedUrl);
			while (feeds.Count > MaxFeeds)
			{
				int oldestCustom = feeds.FindIndex(f => !string.Equals(f, DefaultFeed, StringComparison.OrdinalIgnoreCase));
				if (oldestCustom < 0)
					break;
				feeds.RemoveAt(oldestCustom);
			}
			OnPropertyChanged(nameof(Feeds));
		}

		public void LoadFromXml(XElement section)
		{
			feeds.Clear();
			feeds.Add(DefaultFeed);
			foreach (var feed in section.Element("Feeds")?.Elements("Feed") ?? Enumerable.Empty<XElement>())
				RememberFeed(feed.Value);

			var selected = ((string?)section.Element("SelectedFeed"))?.Trim();
			SelectedFeed = feeds.FirstOrDefault(f => string.Equals(f, selected, StringComparison.OrdinalIgnoreCase))
				?? DefaultFeed;

			bool prerelease;
			try
			{
				prerelease = (bool?)section.Element("IncludePrerelease") ?? false;
			}
			catch (FormatException)
			{
				// Malformed boolean in a hand-edited settings file; fall back to stable-only.
				prerelease = false;
			}
			IncludePrerelease = prerelease;
		}

		public XElement SaveToXml()
		{
			return new XElement(SectionName,
				new XElement("Feeds", feeds.Select(f => new XElement("Feed", f))),
				new XElement("SelectedFeed", SelectedFeed),
				new XElement("IncludePrerelease", IncludePrerelease));
		}
	}
}
