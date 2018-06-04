// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class Generics
	{
		private class GenericClass<T>
		{
			public void M(out GenericClass<T> self)
			{
				self = this;
			}
		}

		public class BaseClass
		{
		}

		public class DerivedClass : BaseClass
		{
		}

		public T CastToTypeParameter<T>(DerivedClass d) where T : BaseClass
		{
			return (T)(BaseClass)d;
		}

		public TTarget GenericAsGeneric<TSource, TTarget>(TSource source) where TTarget : class
		{
			return source as TTarget;
		}

		public TTarget? GenericAsNullable<TSource, TTarget>(TSource source) where TTarget : struct
		{
			return source as TTarget?;
		}

		public TTarget ObjectAsGeneric<TTarget>(object source) where TTarget : class
		{
			return source as TTarget;
		}

		public TTarget? ObjectAsNullable<TTarget>(object source) where TTarget : struct
		{
			return source as TTarget?;
		}

		public TTarget IntAsGeneric<TTarget>(int source) where TTarget : class
		{
			return source as TTarget;
		}

		public TTarget? IntAsNullable<TTarget>(int source) where TTarget : struct
		{
			return source as TTarget?;
		}

		public T New<T>() where T : new()
		{
			return new T();
		}

		public T NotNew<T>()
		{
			return Activator.CreateInstance<T>();
		}

		public bool IsNull<T>(T t)
		{
			return t == null;
		}

		public T[] NewArray<T>(int size)
		{
			return new T[size];
		}

		public T[,] NewArray<T>(int size1, int size2)
		{
			return new T[size1, size2];
		}
	}
}
