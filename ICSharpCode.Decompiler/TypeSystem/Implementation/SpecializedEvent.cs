// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Decompiler.TypeSystem.Implementation
{
	/// <summary>
	/// Represents a specialized IEvent (event after type substitution).
	/// </summary>
	public class SpecializedEvent : SpecializedMember, IEvent
	{
		readonly IEvent eventDefinition;
		
		public SpecializedEvent(IEvent eventDefinition, TypeParameterSubstitution substitution)
			: base(eventDefinition)
		{
			this.eventDefinition = eventDefinition;
			AddSubstitution(substitution);
		}
		
		public bool CanAdd => eventDefinition.CanAdd;

		public bool CanRemove => eventDefinition.CanRemove;

		public bool CanInvoke => eventDefinition.CanInvoke;

		IMethod addAccessor, removeAccessor, invokeAccessor;
		
		public IMethod AddAccessor => WrapAccessor(ref this.addAccessor, eventDefinition.AddAccessor);

		public IMethod RemoveAccessor => WrapAccessor(ref this.removeAccessor, eventDefinition.RemoveAccessor);

		public IMethod InvokeAccessor => WrapAccessor(ref this.invokeAccessor, eventDefinition.InvokeAccessor);
	}
}
