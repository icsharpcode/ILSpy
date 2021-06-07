// Copyright (c) 2014 Daniel Grunwald
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

#nullable enable

namespace ICSharpCode.Decompiler.IL
{
	/// <summary>
	/// Holds information about the role of an instruction within its parent instruction.
	/// </summary>
	public class SlotInfo
	{
		public static SlotInfo None = new SlotInfo("<no slot>");

		/// <summary>
		/// Gets the name of the slot.
		/// </summary>
		public readonly string Name;

		/// <summary>
		/// Gets whether it is possible to inline into this slot.
		/// </summary>
		public readonly bool CanInlineInto;

		/// <summary>
		/// Gets whether this slot belongs to a collection.
		/// </summary>
		public readonly bool IsCollection;

		public SlotInfo(string name, bool canInlineInto = false, bool isCollection = false)
		{
			this.IsCollection = isCollection;
			this.Name = name;
			this.CanInlineInto = canInlineInto;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
