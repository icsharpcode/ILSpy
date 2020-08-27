using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

[StandardModule]
internal sealed class VBCompoundAssign
{
	public static double[] Sum3(int[] v)
	{
		double[] array = new double[4];
		int num = Information.UBound(v);
		checked
		{
			for (int i = 0; i <= num; i += 3)
			{
				array[0] += v[i];
				array[1] += v[i + 1];
				array[2] += v[i + 2];
			}
			return array;
		}
	}
}
