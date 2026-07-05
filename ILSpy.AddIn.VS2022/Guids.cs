// Copyright (c) 2014 Siegfried Pammer
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

// Guids.cs
// MUST match guids.h
using System;

namespace ICSharpCode.ILSpy.AddIn
{
	static class GuidList
	{
#if VS2022
		public const string guidILSpyAddInPkgString = "ebf12ca7-a1fd-4aee-a894-4a0c5682fc2f";
#else
		public const string guidILSpyAddInPkgString = "a9120dbe-164a-4891-842f-fb7829273838";
#endif
		public const string guidILSpyAddInCmdSetString = "85ddb8ca-a842-4b1c-ba1a-94141fdf19d0";

		public static readonly Guid guidILSpyAddInCmdSet = new Guid(guidILSpyAddInCmdSetString);
	};
}