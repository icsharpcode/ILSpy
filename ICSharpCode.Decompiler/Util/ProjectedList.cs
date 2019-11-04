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

using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Util
{
	public sealed class ProjectedList<TInput, TOutput> : IReadOnlyList<TOutput> where TOutput : class
	{
		readonly IList<TInput> input;
		readonly Func<TInput, TOutput> projection;
		readonly TOutput[] items;
		
		public ProjectedList(IList<TInput> input, Func<TInput, TOutput> projection)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));
			if (projection == null)
				throw new ArgumentNullException(nameof(projection));
			this.input = input;
			this.projection = projection;
			this.items = new TOutput[input.Count];
		}
		
		public TOutput this[int index] {
			get {
				TOutput output = LazyInit.VolatileRead(ref items[index]);
				if (output != null) {
					return output;
				}
				return LazyInit.GetOrSet(ref items[index], projection(input[index]));
			}
		}
		
		public int Count {
			get { return items.Length; }
		}
		
		public IEnumerator<TOutput> GetEnumerator()
		{
			for (int i = 0; i < this.Count; i++) {
				yield return this[i];
			}
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
	
	public sealed class ProjectedList<TContext, TInput, TOutput> : IReadOnlyList<TOutput> where TOutput : class
	{
		readonly IList<TInput> input;
		readonly TContext context;
		readonly Func<TContext, TInput, TOutput> projection;
		readonly TOutput[] items;
		
		public ProjectedList(TContext context, IList<TInput> input, Func<TContext, TInput, TOutput> projection)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));
			if (projection == null)
				throw new ArgumentNullException(nameof(projection));
			this.input = input;
			this.context = context;
			this.projection = projection;
			this.items = new TOutput[input.Count];
		}
		
		public TOutput this[int index] {
			get {
				TOutput output = LazyInit.VolatileRead(ref items[index]);
				if (output != null) {
					return output;
				}
				return LazyInit.GetOrSet(ref items[index], projection(context, input[index]));
			}
		}
		
		public int Count {
			get { return items.Length; }
		}
		
		public IEnumerator<TOutput> GetEnumerator()
		{
			for (int i = 0; i < this.Count; i++) {
				yield return this[i];
			}
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
