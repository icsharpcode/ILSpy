// Copyright (c) 2026 Dr. Masroor Ehsan

using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpyX.Tests.AI
{
	[TestFixture]
	public class EmbeddingStoreTests
	{
		[Test]
		public void Search_RanksMatchingTokensFirst()
		{
			var store = new EmbeddingStore();
			store.Add("database", "database query connection");
			store.Add("ui", "button window layout");
			var result = store.Search("database query", 2);
			Assert.That(result[0].Key, Is.EqualTo("database"));
			Assert.That(result[0].Score, Is.GreaterThan(0));
		}
	}
}
