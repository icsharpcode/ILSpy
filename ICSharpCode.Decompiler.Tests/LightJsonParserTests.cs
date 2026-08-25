// Copyright (c) 2026 Christoph Wille
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

using System.Text;

using LightJson.Serialization;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture]
	public class LightJsonParserTests
	{
		[Test]
		public void DeeplyNestedArrays_ThrowCatchableException_InsteadOfOverflowingTheStack()
		{
			// A crafted .deps.json (see DotNetCorePathFinder) nested far beyond any real
			// dependency graph must fail with a catchable parse exception, not an
			// uncatchable StackOverflowException that terminates the process.
			const int depth = 500;
			var json = new string('[', depth) + new string(']', depth);

			Assert.Throws<JsonParseException>(() => JsonReader.Parse(json));
		}

		[Test]
		public void DeeplyNestedObjects_ThrowCatchableException_InsteadOfOverflowingTheStack()
		{
			const int depth = 500;
			var builder = new StringBuilder();
			for (int i = 0; i < depth; i++)
				builder.Append("{\"a\":");
			builder.Append("1");
			builder.Append('}', depth);

			Assert.Throws<JsonParseException>(() => JsonReader.Parse(builder.ToString()));
		}

		[Test]
		public void ModeratelyNestedJson_ParsesSuccessfully()
		{
			// Depth well within the limit must still round-trip; the guard must not
			// reject legitimately structured documents.
			const int depth = 32;
			var json = new string('[', depth) + "42" + new string(']', depth);

			var value = JsonReader.Parse(json);

			var current = value;
			for (int i = 0; i < depth; i++)
				current = current[0];
			Assert.That((int)current, Is.EqualTo(42));
		}
	}
}
