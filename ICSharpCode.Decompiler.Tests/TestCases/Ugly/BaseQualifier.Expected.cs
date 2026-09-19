using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly;

public class BaseQualifierBase : BaseQualifierRoot
{
	public int NonVirtualProperty { get; set; }
	public virtual int VirtualProperty { get; set; }
	public override int AbstractProperty { get; set; }
	public override int OverriddenProperty { get; set; }
	public sealed override int SealedProperty { get; set; }

	public void NonVirtualMethod()
	{
	}

	public virtual void VirtualMethod()
	{
	}
}

public class BaseQualifierDerived : BaseQualifierBase
{
	public int ReadNonVirtualProperty()
	{
		return NonVirtualProperty;
	}

	public int ReadSealedProperty()
	{
		return SealedProperty;
	}

	public Action NonVirtualMethodGroup()
	{
		return NonVirtualMethod;
	}

	public int ReadVirtualProperty()
	{
		return base.VirtualProperty;
	}

	public int ReadAbstractProperty()
	{
		return base.AbstractProperty;
	}

	public int ReadOverriddenProperty()
	{
		return base.OverriddenProperty;
	}

	public Action VirtualMethodGroup()
	{
		return base.VirtualMethod;
	}
}

public abstract class BaseQualifierRoot
{
	public abstract int AbstractProperty { get; set; }
	public virtual int OverriddenProperty { get; set; }
	public virtual int SealedProperty { get; set; }
}
