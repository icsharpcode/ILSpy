using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal abstract class Issue4000<T> : IEnumerable<T>, IEnumerable
	{
		public int Length;

		protected T[] results;

#if !ROSLYN4
		public T this[int i] {
			get {
				if (i >= Length || i < 0)
				{
					return default(T);
				}
				if (results[i] != null && results[i].Equals(default(T)))
				{
					results[i] = CreateIthElement(i);
				}
				return results[i];
			}
		}
#endif

		protected abstract T CreateIthElement(int i);

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < Length; i++)
			{
				if (results[i] != null && results[i].Equals(default(T)))
				{
					results[i] = CreateIthElement(i);
				}
				yield return results[i];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
