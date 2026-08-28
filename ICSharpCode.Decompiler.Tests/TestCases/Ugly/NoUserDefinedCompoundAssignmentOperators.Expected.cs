using System.Runtime.CompilerServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly;

internal class NoUserDefinedCompoundAssignmentOperators
{
	public class Compound
	{
		public int Value;

		[SpecialName]
		[CompilerFeatureRequired("UserDefinedCompoundAssignmentOperators")]
		public void op_AdditionAssignment(int rhs)
		{
			Value += rhs;
		}

		[SpecialName]
		[CompilerFeatureRequired("UserDefinedCompoundAssignmentOperators")]
		public void op_SubtractionAssignment(int rhs)
		{
			Value -= rhs;
		}

		[SpecialName]
		[CompilerFeatureRequired("UserDefinedCompoundAssignmentOperators")]
		public void op_CheckedSubtractionAssignment(int rhs)
		{
			checked
			{
				Value -= rhs;
			}
		}

		[SpecialName]
		[CompilerFeatureRequired("UserDefinedCompoundAssignmentOperators")]
		public void op_IncrementAssignment()
		{
			Value++;
		}
	}

	public static void Use(Compound c, int n)
	{
		c.op_AdditionAssignment(n);
		c.op_CheckedSubtractionAssignment(n);
		c.op_IncrementAssignment();
	}
}
