using CustomAttributeConflicts.NS1;
using CustomAttributeConflicts.NS2;
using CustomAttributeConflicts.NSWithConflictingTypes;
using CustomAttributeConflicts.NSWithConflictingTypes2;
using System;

namespace CustomAttributeConflicts
{
	internal class AttributeWithSameNameAsNormalType
	{
	}

	internal class TestClass
	{
		[Other]
		public void Test1()
		{
		}

		[CustomAttributeConflicts.NS1.Simple]
		public void Test2()
		{
		}

		[CustomAttributeConflicts.NS2.Simple]
		public void Test3()
		{
		}

		[CustomAttributeConflicts.NS1.AttributeWithSameNameAsNormalType]
		public void Test4()
		{
		}

		[@My]
		public void Test5()
		{
		}

		[@MyAttribute]
		public void Test6()
		{
		}

		[CustomAttributeConflicts.NSWithConflictingTypes2.@MyOther]
		public void Test7()
		{
		}

		[CustomAttributeConflicts.NSWithConflictingTypes2.@MyOtherAttribute]
		public void Test8()
		{
		}
	}
}
// The order of types in namespaces is completely different when compiling with the Roslyn compiler
#if ROSLYN
namespace CustomAttributeConflicts.NS1
{
	internal class OtherAttribute : Attribute
	{
	}
	internal class SimpleAttribute : Attribute
	{
	}
}
namespace CustomAttributeConflicts.NS2
{
	internal class SimpleAttribute : Attribute
	{
	}
}
namespace CustomAttributeConflicts.NS1
{
	internal class AttributeWithSameNameAsNormalType : Attribute
	{
	}
}
namespace CustomAttributeConflicts.NSWithConflictingTypes
{
	internal class My : Attribute
	{
	}
	internal class MyAttribute : Attribute
	{
	}
}
namespace CustomAttributeConflicts.NSWithConflictingTypes2
{

	internal class MyOther : Attribute
	{
	}
	internal class MyOtherAttribute : Attribute
	{
	}
	internal class MyOtherAttributeAttribute : Attribute
	{
	}
}
namespace CustomAttributeConflicts.NSWithConflictingTypes
{
	internal class MyAttributeAttribute : Attribute
	{
	}
	internal class MyOther : Attribute
	{
	}
	internal class MyOtherAttribute : Attribute
	{
	}
	internal class MyOtherAttributeAttribute : Attribute
	{
	}
}
#else
namespace CustomAttributeConflicts.NS1
{
	internal class OtherAttribute : Attribute
	{
	}

	internal class SimpleAttribute : Attribute
	{
	}
}

namespace CustomAttributeConflicts.NS2
{
	internal class SimpleAttribute : Attribute
	{
	}
}

namespace CustomAttributeConflicts.NS1
{
	internal class AttributeWithSameNameAsNormalType : Attribute
	{
	}
}
namespace CustomAttributeConflicts.NSWithConflictingTypes
{
	internal class My : Attribute
	{
	}
	internal class MyAttribute : Attribute
	{
	}
}
namespace CustomAttributeConflicts.NSWithConflictingTypes2
{
	internal class MyOther : Attribute
	{
	}
	internal class MyOtherAttribute : Attribute
	{
	}
}
namespace CustomAttributeConflicts.NSWithConflictingTypes
{
	internal class MyAttributeAttribute : Attribute
	{
	}

	internal class MyOther : Attribute
	{
	}

	internal class MyOtherAttribute : Attribute
	{
	}

	internal class MyOtherAttributeAttribute : Attribute
	{
	}
}

namespace CustomAttributeConflicts.NSWithConflictingTypes2
{
	internal class MyOtherAttributeAttribute : Attribute
	{
	}
}
#endif