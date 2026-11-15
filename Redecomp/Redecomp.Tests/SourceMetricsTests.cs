// Copyright (c) 2026 Franck
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

using Redecomp.Bench;

namespace Redecomp.Tests;

public static class SourceMetricsTests
{
	const string Code = """
		public class C
		{
			private readonly decimal _rate;
			public int Count { get; set; }

			/// <summary>Computes.</summary>
			public decimal Compute(List<Line> lines, int factor = 2)
			{
				// sum first
				decimal totalHt = lines.Sum(line => line.Price);
				var remise = totalHt * 0.05m; // discount
				int num;
				foreach (Line item in lines)
					num = item.Quantity;
				for (int index = 0; index < factor; index++) { }
				using var stream = File.OpenRead("x");
				string url = "http://example.com"; // after the string
				return totalHt - remise;
			}
		}
		""";

	[Test]
	public static void FindsLocalsButNotFieldsPropertiesOrParameters()
	{
		var locals = SourceMetrics.DeclaredLocals(Code);
		string actual = string.Join(",", locals.Order());
		Assert.AreEqual("index,item,num,remise,stream,totalHt,url", actual, "declared locals");
	}

	[Test]
	public static void ExtractsCommentLinesIgnoringStringContents()
	{
		var comments = SourceMetrics.CommentLines(Code);
		Assert.AreEqual("<summary>Computes.</summary>|sum first|discount|after the string", string.Join("|", comments), "comments");
	}

	[Test]
	public static void CountsGenericLocalNames()
	{
		const string code = """
			int num = 1;
			int num2 = 2;
			bool flag = true;
			object result = null;
			string value = "";
			decimal totalTtc = 0m;
			int numeroLigne = 3;
			""";
		Assert.AreEqual(5, SourceMetrics.GenericLocalCount(code), "generic locals");
	}
}
