using DataGridExtensions;

namespace ICSharpCode.ILSpy.Metadata
{
	public class FlagsContentFilter : IContentFilter
	{
		public int Mask { get; }

		public FlagsContentFilter(int mask)
		{
			this.Mask = mask;
		}

		public bool IsMatch(object value)
		{
			if (value == null)
				return true;

			return Mask == -1 || (Mask & (int)value) != 0;
		}
	}
}
