.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly DirectCallToExplicitInterfaceImpl
{
  .ver 1:0:0:0
}
.module DirectCallToExplicitInterfaceImpl.exe

.class public auto ansi sealed TestClass
	extends [mscorlib]System.Object
  implements [mscorlib]System.IDisposable
{
	// Methods
	
  .method private final hidebysig newslot virtual 
	instance void System.IDisposable.Dispose () cil managed 
  {
    .override method instance void [mscorlib]System.IDisposable::Dispose()
    ret
  }

  .method public hidebysig void Test (class TestClass other) cil managed 
  {
    ldarg.0
    call instance void TestClass::System.IDisposable.Dispose()

    ldarg.1
    call instance void TestClass::System.IDisposable.Dispose()

    ret
  }

} // end of class TestClass
