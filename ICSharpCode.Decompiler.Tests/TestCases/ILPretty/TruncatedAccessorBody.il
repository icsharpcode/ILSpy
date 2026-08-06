.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly TruncatedAccessorBody
{
  .ver 1:0:0:0
}
.module TruncatedAccessorBody.dll

.class interface public abstract auto ansi ICancellable
{
  .method public hidebysig newslot abstract virtual specialname
	instance bool get_IsCancellable () cil managed
  {
  }

  .property instance bool IsCancellable()
  {
    .get instance bool ICancellable::get_IsCancellable()
  }
}

.class public auto ansi beforefieldinit TestClass
	extends [mscorlib]System.Object
  implements ICancellable
{
  // An explicit interface accessor whose body is too short to be the
  // 'ldarg...; call; ret' forwarding stub. Reference assemblies produce such
  // bodies, and the accessor-forwarding matcher must reject them instead of
  // reading past the end of the body.
  .method private final hidebysig newslot virtual specialname
	instance bool ICancellable.get_IsCancellable () cil managed
  {
    .override method instance bool ICancellable::get_IsCancellable()
    .maxstack 8

    ldarg.0
    call instance bool TestClass::get_IsCancellable()
  }

  .method public hidebysig specialname
	instance bool get_IsCancellable () cil managed
  {
    .maxstack 8

    ldc.i4.0
    ret
  }

  .property instance bool IsCancellable()
  {
    .get instance bool TestClass::get_IsCancellable()
  }

  .method public hidebysig specialname rtspecialname
	instance void .ctor () cil managed
  {
    .maxstack 8

    ldarg.0
    call instance void [mscorlib]System.Object::.ctor()
    ret
  }

} // end of class TestClass
