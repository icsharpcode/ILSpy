#define CORE_ASSEMBLY "System.Runtime"

.assembly extern CORE_ASSEMBLY
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}
.assembly extern UnknownAssembly
{
  .publickeytoken = (01 02 03 04 05 06 07 08 )
  .ver 1:0:0:0
}

.class private auto ansi beforefieldinit UnknownTypes
    extends [System.Private.CoreLib]System.Object
{
    .field private initonly class [UnknownAssembly]IInterface memberField

    // Methods
    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        // Method begins at RVA 0x206e
        // Code size 8 (0x8)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [CORE_ASSEMBLY]System.Object::.ctor()
        IL_0006: nop
        IL_0007: ret
    } // end of method UnknownTypes::.ctor

    .method public hidebysig virtual 
	    instance bool CanExecute (
		    class [UnknownAssembly]CallbackQuery message
	    ) cil managed 
    {
	    // Method begins at RVA 0x4dbc
	    // Code size 44 (0x2c)
	    .maxstack 8

	    IL_0000: ldarg.0
	    IL_0001: ldfld class [UnknownAssembly]IInterface UnknownTypes::memberField
	    IL_0006: newobj instance void [UnknownAssembly]SomeClass::.ctor()
	    IL_000b: dup
	    IL_000d: ldarg.1
	    IL_000e: call int64 [UnknownAssembly]StaticClass::GetChatId(class [UnknownAssembly]CallbackQuery)
	    IL_0013: stfld int64 [UnknownAssembly]SomeClass::ChatId
	    IL_0018: dup
	    IL_001a: ldarg.1
	    IL_001b: call int32 [UnknownAssembly]StaticClass::GetMessageId(class [UnknownAssembly]CallbackQuery)
	    IL_0020: conv.i8
	    IL_0021: stfld int64 [UnknownAssembly]SomeClass::MessageId
	    IL_0026: callvirt instance !1 class [UnknownAssembly]IInterface`2<class [UnknownAssembly]SomeClass, bool>::Execute(!0)
	    IL_002b: ret
    } // end of method CanExecute


} // end of class UnknownTypes
