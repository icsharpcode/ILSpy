// Copyright (c) 2026 Dr. Masroor Ehsan

using System;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.AI.Tests.AI
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
