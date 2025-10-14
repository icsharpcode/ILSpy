using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal static class Issue3576
	{
		public static Issue3576_Camera GetOrCreate(long key, int frameCount, Dictionary<long, (Issue3576_Camera, int)> cache)
		{
			if (!cache.TryGetValue(key, out var value))
			{
				Issue3576_GameObject issue3576_GameObject = new Issue3576_GameObject();
				value = (issue3576_GameObject.AddComponent<Issue3576_Camera>(), frameCount);
				value.Item1.Property = 1;
				issue3576_GameObject.SetActive(value: false);
				cache[key] = value;
			}
			else
			{
				value.Item2 = frameCount;
				cache[key] = value;
			}
			return value.Item1;
		}
	}
	internal sealed class Issue3576_Camera
	{
		public int Property { get; set; }
	}
	internal sealed class Issue3576_GameObject
	{
		public T AddComponent<T>()
		{
			throw null;
		}
		public void SetActive(bool value)
		{
		}
	}
}
