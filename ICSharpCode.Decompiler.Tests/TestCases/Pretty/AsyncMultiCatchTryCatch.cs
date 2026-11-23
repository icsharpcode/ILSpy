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
using System.Threading.Tasks;

public class AsyncMultiCatchTryCatch
{
#if EXPECTED_OUTPUT
	public async Task M()
	{
		int state = 1;
		while (true)
		{
			switch (state)
			{
			case 1:
				Console.WriteLine("first catch");
				try
				{
					Console.WriteLine("try");
				}
				catch (ArgumentException)
				{
					state = 1;
				}
				catch (InvalidOperationException)
				{
					state = 2;
				}
				continue;
			case 2:
				Console.WriteLine("second catch");
				break;
			}
			break;
		}
		await Task.Yield();
	}
#else
	public async Task M()
	{
		int state = 1;
		goto Test;
		Try:
		try
		{
			Console.WriteLine("try");
		}
		catch (ArgumentException)
		{
			state = 1;
		}
		catch (InvalidOperationException)
		{
			state = 2;
		}
		Test:
		if (state == 1)
		{
			Console.WriteLine("first catch");
			goto Try;
		}
		if (state == 2)
		{
			Console.WriteLine("second catch");
		}
		await Task.Yield();
	}
#endif
}
