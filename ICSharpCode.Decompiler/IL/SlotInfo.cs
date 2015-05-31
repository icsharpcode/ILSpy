/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 2015-05-31
 * Time: 14:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

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
