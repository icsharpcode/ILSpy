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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class RefStructInterfaces
	{
		public interface ICounter
		{
			int Value { get; }

			void Increment();
		}

		public interface IWithDefaultMethod
		{
			void Required();

			void Optional()
			{
				Console.WriteLine("default");
			}
		}

		public interface IStaticFactory<T> where T : allows ref struct
		{
			static abstract T Create(int size);
		}

		public interface IProcessor<T> where T : allows ref struct
		{
			void Process(scoped T item);
		}

		public interface IGenericMethods
		{
			void Run<T>(scoped T value) where T : allows ref struct;
		}

		public ref struct RefCounter : ICounter
		{
			private int count;

			public int Value => count;

			public void Increment()
			{
				count++;
			}
		}

		public ref struct ExplicitCounter : ICounter
		{
			private int count;

			int ICounter.Value => count;

			void ICounter.Increment()
			{
				count++;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public ref struct DefaultMethodImpl : IWithDefaultMethod
		{
			public void Required()
			{
			}

			public void Optional()
			{
				Console.WriteLine("overridden");
			}
		}

		public ref struct SpanFactory : IStaticFactory<SpanFactory>
		{
			public Span<byte> Buffer;

			public static SpanFactory Create(int size)
			{
				return default(SpanFactory);
			}
		}

		public ref struct Processor<T> : IProcessor<T> where T : allows ref struct
		{
			public T Item;

			public void Process(scoped T item)
			{
			}
		}

		public abstract class BaseWithVirtual
		{
			public abstract void Process<T>(T value) where T : allows ref struct;

			public virtual T Passthrough<T>(T value) where T : IDisposable, allows ref struct
			{
				return value;
			}
		}

		public class DerivedOverride : BaseWithVirtual
		{
			public override void Process<T>(T value)
			{
			}

			public override T Passthrough<T>(T value)
			{
				return value;
			}
		}

		public class ExplicitGenericImpl : IGenericMethods
		{
			void IGenericMethods.Run<T>(scoped T value)
			{
			}
		}

		public class GenericHolder<T> where T : allows ref struct
		{
			public void M<U>() where U : allows ref struct
			{
			}
		}

		public delegate void RefStructAction<T>(T arg) where T : allows ref struct;

		public static void PlainAllows<T>() where T : allows ref struct
		{
		}

		public static void UseCounter<T>(T counter) where T : ICounter, allows ref struct
		{
			counter.Increment();
			Console.WriteLine(counter.Value);
		}

		public static void UseDefaultMethod<T>(T value) where T : IWithDefaultMethod, allows ref struct
		{
			value.Required();
			value.Optional();
		}

		public static void UsingOnT<T>(T disposable) where T : IDisposable, allows ref struct
		{
			using (disposable)
			{
				Console.WriteLine("in using");
			}
		}

		public static T CreateViaFactory<T>(int size) where T : IStaticFactory<T>, allows ref struct
		{
			return T.Create(size);
		}

		public static T CreateDefault<T>() where T : allows ref struct
		{
			return default(T);
		}

		public static void CombinedUnmanaged<T>() where T : unmanaged, allows ref struct
		{
		}

		public static void CombinedStruct<T>() where T : struct, allows ref struct
		{
		}

		public static void CombinedNew<T>() where T : new(), allows ref struct
		{
		}

		public static void ScopedAndRef<T>(scoped T value, ref T byRef, in T input, out T output) where T : allows ref struct
		{
			output = byRef;
		}

		public static void InvokeDelegate(RefStructAction<Span<int>> action)
		{
			action(default(Span<int>));
		}

		public static void LocalFunctionAllows()
		{
			Local<Span<byte>>();
			static void Local<T>() where T : allows ref struct
			{
			}
		}

		public static void CapturingLocalFunction<T>(int seed) where T : allows ref struct
		{
			Nested(default(T));
			Console.WriteLine(seed);
			void Nested(scoped T value)
			{
				seed++;
			}
		}

		public static IEnumerable<int> Iterator<T>() where T : allows ref struct
		{
			yield return 1;
			yield return 2;
		}

		public static async Task<int> AsyncMethod<T>() where T : allows ref struct
		{
			await Task.Delay(1);
			return 42;
		}

		public static void CallSites()
		{
			RefStructInterfaces.PlainAllows<Span<int>>();
			RefStructInterfaces.PlainAllows<ReadOnlySpan<char>>();
			PlainAllows<int>();
			RefStructInterfaces.UseCounter<RefCounter>(default(RefCounter));
			RefStructInterfaces.UseCounter<ExplicitCounter>(default(ExplicitCounter));
			RefStructInterfaces.UseDefaultMethod<DefaultMethodImpl>(default(DefaultMethodImpl));
			new GenericHolder<Span<byte>>().M<ReadOnlySpan<char>>();
		}

		public static SpanFactory CallFactory()
		{
			return RefStructInterfaces.CreateViaFactory<SpanFactory>(16);
		}
	}
}
