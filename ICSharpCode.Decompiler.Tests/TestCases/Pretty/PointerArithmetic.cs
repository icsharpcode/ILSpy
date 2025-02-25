using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class PointerArithmetic
	{
		public unsafe static void AssignmentVoidPointerToIntPointer(void* ptr)
		{
			((int*)ptr)[2] = 1;
		}

		public unsafe static int AccessVoidPointerToIntPointer(void* ptr)
		{
			return ((int*)ptr)[2];
		}

		public unsafe static void AssignmentLongPointerToIntPointer_2(long* ptr)
		{
			((int*)ptr)[2] = 1;
		}

		public unsafe static int AccessLongPointerToIntPointer_2(long* ptr)
		{
			return ((int*)ptr)[2];
		}

		public unsafe static void AssignmentLongPointerToIntPointer_3(long* ptr)
		{
			((int*)ptr)[3] = 1;
		}

		public unsafe static int AccessLongPointerToIntPointer_3(long* ptr)
		{
			return ((int*)ptr)[3];
		}

		public unsafe static void AssignmentGuidPointerToIntPointer(Guid* ptr)
		{
			((int*)ptr)[2] = 1;
		}

		public unsafe static int AccessGuidPointerToIntPointer(Guid* ptr)
		{
			return ((int*)ptr)[2];
		}

		public unsafe static void AssignmentIntPointer(int* ptr)
		{
			ptr[2] = 1;
		}

		public unsafe static int AccessIntPointer(int* ptr)
		{
			return ptr[2];
		}

		public unsafe static void AssignmentGuidPointer(Guid* ptr)
		{
			ptr[2] = Guid.NewGuid();
		}

		public unsafe static Guid AccessGuidPointer(Guid* ptr)
		{
			return ptr[2];
		}

		public unsafe static void AssignmentVoidPointerToGuidPointer(void* ptr)
		{
			((Guid*)ptr)[2] = Guid.NewGuid();
		}

		public unsafe static Guid AccessVoidPointerToGuidPointer(void* ptr)
		{
			return ((Guid*)ptr)[2];
		}

		public unsafe static void AssignmentIntPointerToGuidPointer(int* ptr)
		{
			((Guid*)ptr)[2] = Guid.NewGuid();
		}

		public unsafe static Guid AccessIntPointerToGuidPointer(int* ptr)
		{
			return ((Guid*)ptr)[2];
		}
	}
}