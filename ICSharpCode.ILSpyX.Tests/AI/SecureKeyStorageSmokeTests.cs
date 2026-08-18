// Copyright (c) 2026 Masroor
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
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class SecureKeyStorageSmokeTests
	{
		[Test]
		[Platform(Include = "MacOsX", Reason = "Exercises the native macOS Keychain backend on this host.")]
		public async Task RoundTrip_WorksOnMacOs()
		{
			await RoundTripAsync("phase0-smoke-macos");
		}

		[Test]
		[Platform(Include = "Win", Reason = "Exercises the native Windows DPAPI backend in the Windows CI leg.")]
		public async Task RoundTrip_WorksOnWindows()
		{
			await RoundTripAsync("phase0-smoke-windows");
		}

		[Test]
		[Platform(Include = "Linux", Reason = "Exercises the native Secret Service backend in the Linux CI leg.")]
		public async Task RoundTrip_WorksOnLinux()
		{
			await RoundTripAsync("phase0-smoke-linux");
		}

		private static async Task RoundTripAsync(string providerPrefix)
		{
			string provider = $"{providerPrefix}-{Guid.NewGuid():N}";
			string key = "sk-phase0-smoke";
			var storage = new SecureKeyStorage();
			bool saved = false;

			try
			{
				await storage.SaveKeyAsync(provider, key);
				saved = true;

				(await storage.LoadKeyAsync(provider)).Should().Be(key);
				var lookup = await storage.TryLoadKeyAsync(provider);
				lookup.Status.Should().Be(SecureKeyLookupStatus.Found);
				lookup.Value.Should().Be(key);
			}
			finally
			{
				if (saved)
					await storage.DeleteKeyAsync(provider);
			}
		}
	}
}
