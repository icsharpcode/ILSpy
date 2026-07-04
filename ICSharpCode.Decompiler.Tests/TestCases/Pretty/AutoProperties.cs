using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class AsymmetricAccessibility
	{
		public int PrivateSetter { get; private set; }

		public int ProtectedSetter { get; protected set; }
	}
	internal class AutoProperties
	{
#if CS110
		public required int RequiredField;
#endif
		public int A { get; } = 1;

		public int B { get; set; } = 2;

		public static int C { get; } = 3;

		public static int D { get; set; } = 4;

		public string value { get; set; }

		[Obsolete("Property")]
#if CS70
		[field: Obsolete("Field")]
#endif
		public int PropertyWithAttributeOnBackingField { get; set; }

		public int issue1319 { get; }

#if CS110
		public required int RequiredProperty { get; set; }
#endif

		public AutoProperties(int issue1319)
		{
			this.issue1319 = issue1319;
#if CS110
			RequiredProperty = 42;
			RequiredField = 42;
#endif
		}
	}
	internal class AutoPropertiesBase
	{
		public virtual int VirtualProperty { get; set; }

		public virtual int VirtualGetterOnly { get; }
	}
	internal class AutoPropertiesDerived : AutoPropertiesBase
	{
		public override int VirtualProperty { get; set; }

		public sealed override int VirtualGetterOnly { get; }
	}
	internal class AutoPropertyExplicitImpl : IAutoProperty
	{
		int IAutoProperty.Property { get; set; }
	}
	internal class AutoPropertyImplicitImpl : IAutoProperty
	{
		public int Property { get; set; }
	}
	internal interface IAutoProperty
	{
		int Property { get; set; }
	}
#if !NET70
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	internal sealed class RequiredMemberAttribute : Attribute
	{
	}
#endif
	internal struct StructAutoProperties
	{
		public int Property { get; set; }

		public int GetterOnly { get; }
	}
}
