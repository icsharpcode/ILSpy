
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly '42zjmgbc'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module '42zjmgbc.dll'
// MVID: {9BD10752-1FE2-4FB9-B5D1-63D7002B8838}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01610000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions
       extends [mscorlib]System.Object
{
  .class auto ansi nested public beforefieldinit Customer
         extends [mscorlib]System.Object
  {
    .field public int32 CustomerID
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> Orders
    .field public string Name
    .field public string Country
    .field public string City
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Customer::.ctor

  } // end of class Customer

  .class auto ansi nested public beforefieldinit Order
         extends [mscorlib]System.Object
  {
    .field public int32 OrderID
    .field public valuetype [mscorlib]System.DateTime OrderDate
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer Customer
    .field public int32 CustomerID
    .field public valuetype [mscorlib]System.Decimal Total
    .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> Details
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Order::.ctor

  } // end of class Order

  .class auto ansi nested public beforefieldinit OrderDetail
         extends [mscorlib]System.Object
  {
    .field public valuetype [mscorlib]System.Decimal UnitPrice
    .field public int32 Quantity
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method OrderDetail::.ctor

  } // end of class OrderDetail

  .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> customers
  .field public class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> orders
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> 'CS$<>9__CachedAnonymousMethodDelegate2'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> 'CS$<>9__CachedAnonymousMethodDelegate3'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> 'CS$<>9__CachedAnonymousMethodDelegate7'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> 'CS$<>9__CachedAnonymousMethodDelegate8'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> 'CS$<>9__CachedAnonymousMethodDelegatee'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> 'CS$<>9__CachedAnonymousMethodDelegatef'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,valuetype [mscorlib]System.Decimal> 'CS$<>9__CachedAnonymousMethodDelegate10'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> 'CS$<>9__CachedAnonymousMethodDelegate11'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> 'CS$<>9__CachedAnonymousMethodDelegate18'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> 'CS$<>9__CachedAnonymousMethodDelegate19'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> 'CS$<>9__CachedAnonymousMethodDelegate1a'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType2`3'<string,int32,int32>> 'CS$<>9__CachedAnonymousMethodDelegate1b'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> 'CS$<>9__CachedAnonymousMethodDelegate25'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> 'CS$<>9__CachedAnonymousMethodDelegate26'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> 'CS$<>9__CachedAnonymousMethodDelegate27'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> 'CS$<>9__CachedAnonymousMethodDelegate28'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>> 'CS$<>9__CachedAnonymousMethodDelegate29'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>> 'CS$<>9__CachedAnonymousMethodDelegate2a'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>> 'CS$<>9__CachedAnonymousMethodDelegate30'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,bool> 'CS$<>9__CachedAnonymousMethodDelegate31'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>> 'CS$<>9__CachedAnonymousMethodDelegate32'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,valuetype [mscorlib]System.Decimal> 'CS$<>9__CachedAnonymousMethodDelegate33'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>> 'CS$<>9__CachedAnonymousMethodDelegate39'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>> 'CS$<>9__CachedAnonymousMethodDelegate3a'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>,string> 'CS$<>9__CachedAnonymousMethodDelegate3b'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> 'CS$<>9__CachedAnonymousMethodDelegate40'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> 'CS$<>9__CachedAnonymousMethodDelegate41'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousTypea`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>> 'CS$<>9__CachedAnonymousMethodDelegate42'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> 'CS$<>9__CachedAnonymousMethodDelegate4b'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> 'CS$<>9__CachedAnonymousMethodDelegate4c'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>> 'CS$<>9__CachedAnonymousMethodDelegate4d'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>> 'CS$<>9__CachedAnonymousMethodDelegate4e'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,bool> 'CS$<>9__CachedAnonymousMethodDelegate4f'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,class '<>f__AnonymousTyped`2'<string,int32>> 'CS$<>9__CachedAnonymousMethodDelegate50'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,string> 'CS$<>9__CachedAnonymousMethodDelegate53'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> 'CS$<>9__CachedAnonymousMethodDelegate54'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> 'CS$<>9__CachedAnonymousMethodDelegate57'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> 'CS$<>9__CachedAnonymousMethodDelegate58'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> 'CS$<>9__CachedAnonymousMethodDelegate5a'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> 'CS$<>9__CachedAnonymousMethodDelegate5d'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>,class '<>f__AnonymousTypee`2'<string,int32>> 'CS$<>9__CachedAnonymousMethodDelegate5e'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig instance object 
          MultipleWhere() cil managed
  {
    // Code size       75 (0x4b)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate2'
    IL_000b:  brtrue.s   IL_001e

    IL_000d:  ldnull
    IL_000e:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleWhere>b__0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool>::.ctor(object,
                                                                                                                                                                native int)
    IL_0019:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate2'
    IL_001e:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate2'
    IL_0023:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                 class [mscorlib]System.Func`2<!!0,bool>)
    IL_0028:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate3'
    IL_002d:  brtrue.s   IL_0040

    IL_002f:  ldnull
    IL_0030:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleWhere>b__1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0036:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool>::.ctor(object,
                                                                                                                                                                native int)
    IL_003b:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate3'
    IL_0040:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate3'
    IL_0045:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                 class [mscorlib]System.Func`2<!!0,bool>)
    IL_004a:  ret
  } // end of method QueryExpressions::MultipleWhere

  .method public hidebysig instance object 
          SelectManyFollowedBySelect() cil managed
  {
    // Code size       70 (0x46)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_000b:  brtrue.s   IL_001e

    IL_000d:  ldnull
    IL_000e:  ldftn      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<SelectManyFollowedBySelect>b__5'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                native int)
    IL_0019:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_001e:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_0023:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate8'
    IL_0028:  brtrue.s   IL_003b

    IL_002a:  ldnull
    IL_002b:  ldftn      class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<SelectManyFollowedBySelect>b__6'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,
                                                                                                                                                                                                           class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0031:  newobj     instance void class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                     native int)
    IL_0036:  stsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate8'
    IL_003b:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate8'
    IL_0040:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [System.Core]System.Linq.Enumerable::SelectMany<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                class [mscorlib]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>,
                                                                                                                                                                                                                                                                                                                                                                                class [mscorlib]System.Func`3<!!0,!!1,!!2>)
    IL_0045:  ret
  } // end of method QueryExpressions::SelectManyFollowedBySelect

  .method public hidebysig instance object 
          SelectManyFollowedByOrderBy() cil managed
  {
    // Code size       138 (0x8a)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegatee'
    IL_000b:  brtrue.s   IL_001e

    IL_000d:  ldnull
    IL_000e:  ldftn      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<SelectManyFollowedByOrderBy>b__a'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                native int)
    IL_0019:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegatee'
    IL_001e:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegatee'
    IL_0023:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_0028:  brtrue.s   IL_003b

    IL_002a:  ldnull
    IL_002b:  ldftn      class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<SelectManyFollowedByOrderBy>b__b'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,
                                                                                                                                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0031:  newobj     instance void class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                              native int)
    IL_0036:  stsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_003b:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_0040:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [System.Core]System.Linq.Enumerable::SelectMany<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`3<!!0,!!1,!!2>)
    IL_0045:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate10'
    IL_004a:  brtrue.s   IL_005d

    IL_004c:  ldnull
    IL_004d:  ldftn      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<SelectManyFollowedByOrderBy>b__c'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>)
    IL_0053:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,valuetype [mscorlib]System.Decimal>::.ctor(object,
                                                                                                                                                                                                                                                                                                        native int)
    IL_0058:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate10'
    IL_005d:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate10'
    IL_0062:  call       class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::OrderByDescending<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,valuetype [mscorlib]System.Decimal>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                     class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0067:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate11'
    IL_006c:  brtrue.s   IL_007f

    IL_006e:  ldnull
    IL_006f:  ldftn      class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<SelectManyFollowedByOrderBy>b__d'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>)
    IL_0075:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                    native int)
    IL_007a:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate11'
    IL_007f:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate11'
    IL_0084:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                           class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0089:  ret
  } // end of method QueryExpressions::SelectManyFollowedByOrderBy

  .method public hidebysig instance object 
          MultipleSelectManyFollowedBySelect() cil managed
  {
    // Code size       133 (0x85)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate18'
    IL_000b:  brtrue.s   IL_001e

    IL_000d:  ldnull
    IL_000e:  ldftn      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleSelectManyFollowedBySelect>b__14'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                native int)
    IL_0019:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate18'
    IL_001e:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate18'
    IL_0023:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate19'
    IL_0028:  brtrue.s   IL_003b

    IL_002a:  ldnull
    IL_002b:  ldftn      class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleSelectManyFollowedBySelect>b__15'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,
                                                                                                                                                                                                                                                                                                                             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0031:  newobj     instance void class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                              native int)
    IL_0036:  stsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate19'
    IL_003b:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate19'
    IL_0040:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [System.Core]System.Linq.Enumerable::SelectMany<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`3<!!0,!!1,!!2>)
    IL_0045:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate1a'
    IL_004a:  brtrue.s   IL_005d

    IL_004c:  ldnull
    IL_004d:  ldftn      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleSelectManyFollowedBySelect>b__16'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>)
    IL_0053:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                native int)
    IL_0058:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate1a'
    IL_005d:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate1a'
    IL_0062:  ldsfld     class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType2`3'<string,int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate1b'
    IL_0067:  brtrue.s   IL_007a

    IL_0069:  ldnull
    IL_006a:  ldftn      class '<>f__AnonymousType2`3'<string,int32,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleSelectManyFollowedBySelect>b__17'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,
                                                                                                                                                                                       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail)
    IL_0070:  newobj     instance void class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType2`3'<string,int32,int32>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                        native int)
    IL_0075:  stsfld     class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType2`3'<string,int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate1b'
    IL_007a:  ldsfld     class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType2`3'<string,int32,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate1b'
    IL_007f:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [System.Core]System.Linq.Enumerable::SelectMany<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType2`3'<string,int32,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                   class [mscorlib]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                   class [mscorlib]System.Func`3<!!0,!!1,!!2>)
    IL_0084:  ret
  } // end of method QueryExpressions::MultipleSelectManyFollowedBySelect

  .method public hidebysig instance object 
          MultipleSelectManyFollowedByLet() cil managed
  {
    // Code size       201 (0xc9)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate25'
    IL_000b:  brtrue.s   IL_001e

    IL_000d:  ldnull
    IL_000e:  ldftn      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleSelectManyFollowedByLet>b__1f'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                native int)
    IL_0019:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate25'
    IL_001e:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate25'
    IL_0023:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate26'
    IL_0028:  brtrue.s   IL_003b

    IL_002a:  ldnull
    IL_002b:  ldftn      class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleSelectManyFollowedByLet>b__20'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,
                                                                                                                                                                                                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0031:  newobj     instance void class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                              native int)
    IL_0036:  stsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate26'
    IL_003b:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate26'
    IL_0040:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [System.Core]System.Linq.Enumerable::SelectMany<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`3<!!0,!!1,!!2>)
    IL_0045:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate27'
    IL_004a:  brtrue.s   IL_005d

    IL_004c:  ldnull
    IL_004d:  ldftn      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleSelectManyFollowedByLet>b__21'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>)
    IL_0053:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                native int)
    IL_0058:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate27'
    IL_005d:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate27'
    IL_0062:  ldsfld     class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate28'
    IL_0067:  brtrue.s   IL_007a

    IL_0069:  ldnull
    IL_006a:  ldftn      class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleSelectManyFollowedByLet>b__22'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,
                                                                                                                                                                                                                                                                                                                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail)
    IL_0070:  newobj     instance void class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              native int)
    IL_0075:  stsfld     class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate28'
    IL_007a:  ldsfld     class [mscorlib]System.Func`3<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate28'
    IL_007f:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!2> [System.Core]System.Linq.Enumerable::SelectMany<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`3<!!0,!!1,!!2>)
    IL_0084:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate29'
    IL_0089:  brtrue.s   IL_009c

    IL_008b:  ldnull
    IL_008c:  ldftn      class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleSelectManyFollowedByLet>b__23'(class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>)
    IL_0092:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               native int)
    IL_0097:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate29'
    IL_009c:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate29'
    IL_00a1:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      class [mscorlib]System.Func`2<!!0,!!1>)
    IL_00a6:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate2a'
    IL_00ab:  brtrue.s   IL_00be

    IL_00ad:  ldnull
    IL_00ae:  ldftn      class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleSelectManyFollowedByLet>b__24'(class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>)
    IL_00b4:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      native int)
    IL_00b9:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate2a'
    IL_00be:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate2a'
    IL_00c3:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             class [mscorlib]System.Func`2<!!0,!!1>)
    IL_00c8:  ret
  } // end of method QueryExpressions::MultipleSelectManyFollowedByLet

  .method public hidebysig instance object 
          FromLetWhereSelect() cil managed
  {
    // Code size       109 (0x6d)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::orders
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate30'
    IL_000b:  brtrue.s   IL_001e

    IL_000d:  ldnull
    IL_000e:  ldftn      class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<FromLetWhereSelect>b__2c'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                     native int)
    IL_0019:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate30'
    IL_001e:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate30'
    IL_0023:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                            class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0028:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate31'
    IL_002d:  brtrue.s   IL_0040

    IL_002f:  ldnull
    IL_0030:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<FromLetWhereSelect>b__2e'(class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>)
    IL_0036:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,bool>::.ctor(object,
                                                                                                                                                                                                                               native int)
    IL_003b:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate31'
    IL_0040:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate31'
    IL_0045:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                class [mscorlib]System.Func`2<!!0,bool>)
    IL_004a:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate32'
    IL_004f:  brtrue.s   IL_0062

    IL_0051:  ldnull
    IL_0052:  ldftn      class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<FromLetWhereSelect>b__2f'(class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>)
    IL_0058:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                  native int)
    IL_005d:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate32'
    IL_0062:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate32'
    IL_0067:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>,class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,!!1>)
    IL_006c:  ret
  } // end of method QueryExpressions::FromLetWhereSelect

  .method public hidebysig instance object 
          MultipleLet() cil managed
  {
    // Code size       109 (0x6d)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate39'
    IL_000b:  brtrue.s   IL_001e

    IL_000d:  ldnull
    IL_000e:  ldftn      class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleLet>b__36'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>>::.ctor(object,
                                                                                                                                                                                                                                                                               native int)
    IL_0019:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate39'
    IL_001e:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate39'
    IL_0023:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                      class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0028:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate3a'
    IL_002d:  brtrue.s   IL_0040

    IL_002f:  ldnull
    IL_0030:  ldftn      class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleLet>b__37'(class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>)
    IL_0036:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                           native int)
    IL_003b:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate3a'
    IL_0040:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate3a'
    IL_0045:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                  class [mscorlib]System.Func`2<!!0,!!1>)
    IL_004a:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate3b'
    IL_004f:  brtrue.s   IL_0062

    IL_0051:  ldnull
    IL_0052:  ldftn      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<MultipleLet>b__38'(class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>)
    IL_0058:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>,string>::.ctor(object,
                                                                                                                                                                                                                                              native int)
    IL_005d:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate3b'
    IL_0062:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate3b'
    IL_0067:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                     class [mscorlib]System.Func`2<!!0,!!1>)
    IL_006c:  ret
  } // end of method QueryExpressions::MultipleLet

  .method public hidebysig instance object 
          Join() cil managed
  {
    // Code size       105 (0x69)
    .maxstack  6
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldarg.0
    IL_0007:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::orders
    IL_000c:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate40'
    IL_0011:  brtrue.s   IL_0024

    IL_0013:  ldnull
    IL_0014:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<Join>b__3d'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32>::.ctor(object,
                                                                                                                                                                 native int)
    IL_001f:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate40'
    IL_0024:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate40'
    IL_0029:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate41'
    IL_002e:  brtrue.s   IL_0041

    IL_0030:  ldnull
    IL_0031:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<Join>b__3e'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0037:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32>::.ctor(object,
                                                                                                                                                              native int)
    IL_003c:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate41'
    IL_0041:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate41'
    IL_0046:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousTypea`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate42'
    IL_004b:  brtrue.s   IL_005e

    IL_004d:  ldnull
    IL_004e:  ldftn      class '<>f__AnonymousTypea`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<Join>b__3f'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,
                                                                                                                                                                                                                    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0054:  newobj     instance void class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousTypea`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                   native int)
    IL_0059:  stsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousTypea`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate42'
    IL_005e:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,class '<>f__AnonymousTypea`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate42'
    IL_0063:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!3> [System.Core]System.Linq.Enumerable::Join<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32,class '<>f__AnonymousTypea`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                              class [mscorlib]System.Func`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                                                                                              class [mscorlib]System.Func`2<!!1,!!2>,
                                                                                                                                                                                                                                                                                                                                                                                                              class [mscorlib]System.Func`3<!!0,!!1,!!3>)
    IL_0068:  ret
  } // end of method QueryExpressions::Join

  .method public hidebysig instance object 
          JoinInto() cil managed
  {
    // Code size       207 (0xcf)
    .maxstack  6
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldarg.0
    IL_0007:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::orders
    IL_000c:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4b'
    IL_0011:  brtrue.s   IL_0024

    IL_0013:  ldnull
    IL_0014:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<JoinInto>b__45'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_001a:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32>::.ctor(object,
                                                                                                                                                                 native int)
    IL_001f:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4b'
    IL_0024:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4b'
    IL_0029:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4c'
    IL_002e:  brtrue.s   IL_0041

    IL_0030:  ldnull
    IL_0031:  ldftn      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<JoinInto>b__46'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0037:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32>::.ctor(object,
                                                                                                                                                              native int)
    IL_003c:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4c'
    IL_0041:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4c'
    IL_0046:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4d'
    IL_004b:  brtrue.s   IL_005e

    IL_004d:  ldnull
    IL_004e:  ldftn      class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<JoinInto>b__47'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,
                                                                                                                                                                                                                                                                                                                                                             class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>)
    IL_0054:  newobj     instance void class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  native int)
    IL_0059:  stsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4d'
    IL_005e:  ldsfld     class [mscorlib]System.Func`3<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4d'
    IL_0063:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!3> [System.Core]System.Linq.Enumerable::GroupJoin<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,int32,class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        class [mscorlib]System.Func`2<!!0,!!2>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        class [mscorlib]System.Func`2<!!1,!!2>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        class [mscorlib]System.Func`3<!!0,class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1>,!!3>)
    IL_0068:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4e'
    IL_006d:  brtrue.s   IL_0080

    IL_006f:  ldnull
    IL_0070:  ldftn      class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<JoinInto>b__48'(class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>)
    IL_0076:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      native int)
    IL_007b:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4e'
    IL_0080:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4e'
    IL_0085:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             class [mscorlib]System.Func`2<!!0,!!1>)
    IL_008a:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4f'
    IL_008f:  brtrue.s   IL_00a2

    IL_0091:  ldnull
    IL_0092:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<JoinInto>b__49'(class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>)
    IL_0098:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,bool>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                         native int)
    IL_009d:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4f'
    IL_00a2:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate4f'
    IL_00a7:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                          class [mscorlib]System.Func`2<!!0,bool>)
    IL_00ac:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,class '<>f__AnonymousTyped`2'<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate50'
    IL_00b1:  brtrue.s   IL_00c4

    IL_00b3:  ldnull
    IL_00b4:  ldftn      class '<>f__AnonymousTyped`2'<string,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<JoinInto>b__4a'(class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>)
    IL_00ba:  newobj     instance void class [mscorlib]System.Func`2<class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,class '<>f__AnonymousTyped`2'<string,int32>>::.ctor(object,
                                                                                                                                                                                                                                                                                                                                                                                                                native int)
    IL_00bf:  stsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,class '<>f__AnonymousTyped`2'<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate50'
    IL_00c4:  ldsfld     class [mscorlib]System.Func`2<class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,class '<>f__AnonymousTyped`2'<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate50'
    IL_00c9:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>,class '<>f__AnonymousTyped`2'<string,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                                                                                                                                                                       class [mscorlib]System.Func`2<!!0,!!1>)
    IL_00ce:  ret
  } // end of method QueryExpressions::JoinInto

  .method public hidebysig instance object 
          OrderBy() cil managed
  {
    // Code size       75 (0x4b)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::orders
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate53'
    IL_000b:  brtrue.s   IL_001e

    IL_000d:  ldnull
    IL_000e:  ldftn      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<OrderBy>b__51'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,string>::.ctor(object,
                                                                                                                                                               native int)
    IL_0019:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate53'
    IL_001e:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate53'
    IL_0023:  call       class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::OrderBy<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                  class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0028:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate54'
    IL_002d:  brtrue.s   IL_0040

    IL_002f:  ldnull
    IL_0030:  ldftn      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<OrderBy>b__52'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order)
    IL_0036:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>::.ctor(object,
                                                                                                                                                                                           native int)
    IL_003b:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate54'
    IL_0040:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate54'
    IL_0045:  call       class [System.Core]System.Linq.IOrderedEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::ThenByDescending<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>(class [System.Core]System.Linq.IOrderedEnumerable`1<!!0>,
                                                                                                                                                                                                                                                       class [mscorlib]System.Func`2<!!0,!!1>)
    IL_004a:  ret
  } // end of method QueryExpressions::OrderBy

  .method public hidebysig instance object 
          GroupBy() cil managed
  {
    // Code size       70 (0x46)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate57'
    IL_000b:  brtrue.s   IL_001e

    IL_000d:  ldnull
    IL_000e:  ldftn      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<GroupBy>b__55'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>::.ctor(object,
                                                                                                                                                                  native int)
    IL_0019:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate57'
    IL_001e:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate57'
    IL_0023:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate58'
    IL_0028:  brtrue.s   IL_003b

    IL_002a:  ldnull
    IL_002b:  ldftn      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<GroupBy>b__56'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0031:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>::.ctor(object,
                                                                                                                                                                  native int)
    IL_0036:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate58'
    IL_003b:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate58'
    IL_0040:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.IGrouping`2<!!1,!!2>> [System.Core]System.Linq.Enumerable::GroupBy<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                 class [mscorlib]System.Func`2<!!0,!!1>,
                                                                                                                                                                                                                                                                                 class [mscorlib]System.Func`2<!!0,!!2>)
    IL_0045:  ret
  } // end of method QueryExpressions::GroupBy

  .method public hidebysig instance object 
          ExplicitType() cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Cast<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>(class [mscorlib]System.Collections.IEnumerable)
    IL_000b:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate5a'
    IL_0010:  brtrue.s   IL_0023

    IL_0012:  ldnull
    IL_0013:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<ExplicitType>b__59'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0019:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool>::.ctor(object,
                                                                                                                                                                native int)
    IL_001e:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate5a'
    IL_0023:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate5a'
    IL_0028:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                 class [mscorlib]System.Func`2<!!0,bool>)
    IL_002d:  ret
  } // end of method QueryExpressions::ExplicitType

  .method public hidebysig instance object 
          QueryContinuation() cil managed
  {
    // Code size       75 (0x4b)
    .maxstack  3
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::customers
    IL_0006:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate5d'
    IL_000b:  brtrue.s   IL_001e

    IL_000d:  ldnull
    IL_000e:  ldftn      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<QueryContinuation>b__5b'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer)
    IL_0014:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>::.ctor(object,
                                                                                                                                                                  native int)
    IL_0019:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate5d'
    IL_001e:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate5d'
    IL_0023:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.IGrouping`2<!!1,!!0>> [System.Core]System.Linq.Enumerable::GroupBy<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                          class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0028:  ldsfld     class [mscorlib]System.Func`2<class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>,class '<>f__AnonymousTypee`2'<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate5e'
    IL_002d:  brtrue.s   IL_0040

    IL_002f:  ldnull
    IL_0030:  ldftn      class '<>f__AnonymousTypee`2'<string,int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<QueryContinuation>b__5c'(class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>)
    IL_0036:  newobj     instance void class [mscorlib]System.Func`2<class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>,class '<>f__AnonymousTypee`2'<string,int32>>::.ctor(object,
                                                                                                                                                                                                                                                          native int)
    IL_003b:  stsfld     class [mscorlib]System.Func`2<class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>,class '<>f__AnonymousTypee`2'<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate5e'
    IL_0040:  ldsfld     class [mscorlib]System.Func`2<class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>,class '<>f__AnonymousTypee`2'<string,int32>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate5e'
    IL_0045:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>,class '<>f__AnonymousTypee`2'<string,int32>>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                                                                                                                                                 class [mscorlib]System.Func`2<!!0,!!1>)
    IL_004a:  ret
  } // end of method QueryExpressions::QueryContinuation

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method QueryExpressions::.ctor

  .method private hidebysig static bool  '<MultipleWhere>b__0'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       16 (0x10)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Orders
    IL_0006:  call       int32 [System.Core]System.Linq.Enumerable::Count<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  ldc.i4.s   10
    IL_000d:  cgt
    IL_000f:  ret
  } // end of method QueryExpressions::'<MultipleWhere>b__0'

  .method private hidebysig static bool  '<MultipleWhere>b__1'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Country
    IL_0006:  ldstr      "DE"
    IL_000b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0010:  ret
  } // end of method QueryExpressions::'<MultipleWhere>b__1'

  .method private hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
          '<SelectManyFollowedBySelect>b__5'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Orders
    IL_0006:  ret
  } // end of method QueryExpressions::'<SelectManyFollowedBySelect>b__5'

  .method private hidebysig static class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal> 
          '<SelectManyFollowedBySelect>b__6'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c,
                                             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
    IL_0006:  ldarg.1
    IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::OrderID
    IL_000c:  ldarg.1
    IL_000d:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Total
    IL_0012:  newobj     instance void class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                             !1,
                                                                                                                             !2)
    IL_0017:  ret
  } // end of method QueryExpressions::'<SelectManyFollowedBySelect>b__6'

  .method private hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
          '<SelectManyFollowedByOrderBy>b__a'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Orders
    IL_0006:  ret
  } // end of method QueryExpressions::'<SelectManyFollowedByOrderBy>b__a'

  .method private hidebysig static class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
          '<SelectManyFollowedByOrderBy>b__b'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c,
                                              class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::.ctor(!0,
                                                                                                                                                                                                                                      !1)
    IL_0007:  ret
  } // end of method QueryExpressions::'<SelectManyFollowedByOrderBy>b__b'

  .method private hidebysig static valuetype [mscorlib]System.Decimal 
          '<SelectManyFollowedByOrderBy>b__c'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> '<>h__TransparentIdentifier9') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
    IL_0006:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Total
    IL_000b:  ret
  } // end of method QueryExpressions::'<SelectManyFollowedByOrderBy>b__c'

  .method private hidebysig static class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal> 
          '<SelectManyFollowedByOrderBy>b__d'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> '<>h__TransparentIdentifier9') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_c()
    IL_0006:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
    IL_000b:  ldarg.0
    IL_000c:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
    IL_0011:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::OrderID
    IL_0016:  ldarg.0
    IL_0017:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
    IL_001c:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Total
    IL_0021:  newobj     instance void class '<>f__AnonymousType0`3'<string,int32,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                             !1,
                                                                                                                             !2)
    IL_0026:  ret
  } // end of method QueryExpressions::'<SelectManyFollowedByOrderBy>b__d'

  .method private hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
          '<MultipleSelectManyFollowedBySelect>b__14'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Orders
    IL_0006:  ret
  } // end of method QueryExpressions::'<MultipleSelectManyFollowedBySelect>b__14'

  .method private hidebysig static class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
          '<MultipleSelectManyFollowedBySelect>b__15'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c,
                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::.ctor(!0,
                                                                                                                                                                                                                                      !1)
    IL_0007:  ret
  } // end of method QueryExpressions::'<MultipleSelectManyFollowedBySelect>b__15'

  .method private hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> 
          '<MultipleSelectManyFollowedBySelect>b__16'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> '<>h__TransparentIdentifier12') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
    IL_0006:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Details
    IL_000b:  ret
  } // end of method QueryExpressions::'<MultipleSelectManyFollowedBySelect>b__16'

  .method private hidebysig static class '<>f__AnonymousType2`3'<string,int32,int32> 
          '<MultipleSelectManyFollowedBySelect>b__17'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> '<>h__TransparentIdentifier12',
                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail d) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       34 (0x22)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_c()
    IL_0006:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
    IL_000b:  ldarg.0
    IL_000c:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
    IL_0011:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::OrderID
    IL_0016:  ldarg.1
    IL_0017:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail::Quantity
    IL_001c:  newobj     instance void class '<>f__AnonymousType2`3'<string,int32,int32>::.ctor(!0,
                                                                                                !1,
                                                                                                !2)
    IL_0021:  ret
  } // end of method QueryExpressions::'<MultipleSelectManyFollowedBySelect>b__17'

  .method private hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
          '<MultipleSelectManyFollowedByLet>b__1f'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Orders
    IL_0006:  ret
  } // end of method QueryExpressions::'<MultipleSelectManyFollowedByLet>b__1f'

  .method private hidebysig static class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> 
          '<MultipleSelectManyFollowedByLet>b__20'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c,
                                                   class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::.ctor(!0,
                                                                                                                                                                                                                                      !1)
    IL_0007:  ret
  } // end of method QueryExpressions::'<MultipleSelectManyFollowedByLet>b__20'

  .method private hidebysig static class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> 
          '<MultipleSelectManyFollowedByLet>b__21'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> '<>h__TransparentIdentifier1c') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
    IL_0006:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Details
    IL_000b:  ret
  } // end of method QueryExpressions::'<MultipleSelectManyFollowedByLet>b__21'

  .method private hidebysig static class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> 
          '<MultipleSelectManyFollowedByLet>b__22'(class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> '<>h__TransparentIdentifier1c',
                                                   class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail d) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>::.ctor(!0,
                                                                                                                                                                                                                                                                                                                                                      !1)
    IL_0007:  ret
  } // end of method QueryExpressions::'<MultipleSelectManyFollowedByLet>b__22'

  .method private hidebysig static class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal> 
          '<MultipleSelectManyFollowedByLet>b__23'(class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> '<>h__TransparentIdentifier1d') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance !1 class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>::get_d()
    IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail::Quantity
    IL_000c:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_0011:  ldarg.0
    IL_0012:  callvirt   instance !1 class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>::get_d()
    IL_0017:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail::UnitPrice
    IL_001c:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0021:  newobj     instance void class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                                                                                                                                                                                                                                                                                        !1)
    IL_0026:  ret
  } // end of method QueryExpressions::'<MultipleSelectManyFollowedByLet>b__23'

  .method private hidebysig static class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal> 
          '<MultipleSelectManyFollowedByLet>b__24'(class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal> '<>h__TransparentIdentifier1e') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       54 (0x36)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>::'get_<>h__TransparentIdentifier1d'()
    IL_0006:  callvirt   instance !0 class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>::'get_<>h__TransparentIdentifier1c'()
    IL_000b:  callvirt   instance !0 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_c()
    IL_0010:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
    IL_0015:  ldarg.0
    IL_0016:  callvirt   instance !0 class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>::'get_<>h__TransparentIdentifier1d'()
    IL_001b:  callvirt   instance !0 class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>::'get_<>h__TransparentIdentifier1c'()
    IL_0020:  callvirt   instance !1 class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>::get_o()
    IL_0025:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::OrderID
    IL_002a:  ldarg.0
    IL_002b:  callvirt   instance !1 class '<>f__AnonymousType4`2'<class '<>f__AnonymousType3`2'<class '<>f__AnonymousType1`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>,valuetype [mscorlib]System.Decimal>::get_x()
    IL_0030:  newobj     instance void class '<>f__AnonymousType5`3'<string,int32,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                             !1,
                                                                                                                             !2)
    IL_0035:  ret
  } // end of method QueryExpressions::'<MultipleSelectManyFollowedByLet>b__24'

  .method private hidebysig static class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> 
          '<FromLetWhereSelect>b__2c'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       47 (0x2f)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldfld      class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Details
    IL_0007:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate33'
    IL_000c:  brtrue.s   IL_001f

    IL_000e:  ldnull
    IL_000f:  ldftn      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'<FromLetWhereSelect>b__2d'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,valuetype [mscorlib]System.Decimal>::.ctor(object,
                                                                                                                                                                                                 native int)
    IL_001a:  stsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate33'
    IL_001f:  ldsfld     class [mscorlib]System.Func`2<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail,valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions::'CS$<>9__CachedAnonymousMethodDelegate33'
    IL_0024:  call       valuetype [mscorlib]System.Decimal [System.Core]System.Linq.Enumerable::Sum<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                       class [mscorlib]System.Func`2<!!0,valuetype [mscorlib]System.Decimal>)
    IL_0029:  newobj     instance void class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                                                           !1)
    IL_002e:  ret
  } // end of method QueryExpressions::'<FromLetWhereSelect>b__2c'

  .method private hidebysig static bool  '<FromLetWhereSelect>b__2e'(class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> '<>h__TransparentIdentifier2b') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !1 class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>::get_t()
    IL_0006:  ldc.i4     0x3e8
    IL_000b:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0010:  call       bool [mscorlib]System.Decimal::op_GreaterThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                              valuetype [mscorlib]System.Decimal)
    IL_0015:  ret
  } // end of method QueryExpressions::'<FromLetWhereSelect>b__2e'

  .method private hidebysig static class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal> 
          '<FromLetWhereSelect>b__2f'(class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal> '<>h__TransparentIdentifier2b') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       23 (0x17)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>::get_o()
    IL_0006:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::OrderID
    IL_000b:  ldarg.0
    IL_000c:  callvirt   instance !1 class '<>f__AnonymousType6`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order,valuetype [mscorlib]System.Decimal>::get_t()
    IL_0011:  newobj     instance void class '<>f__AnonymousType7`2'<int32,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                      !1)
    IL_0016:  ret
  } // end of method QueryExpressions::'<FromLetWhereSelect>b__2f'

  .method private hidebysig static valuetype [mscorlib]System.Decimal 
          '<FromLetWhereSelect>b__2d'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail d) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       23 (0x17)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail::UnitPrice
    IL_0006:  ldarg.0
    IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/OrderDetail::Quantity
    IL_000c:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_0011:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0016:  ret
  } // end of method QueryExpressions::'<FromLetWhereSelect>b__2d'

  .method private hidebysig static class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> 
          '<MultipleLet>b__36'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer a) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Country
    IL_0007:  newobj     instance void class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>::.ctor(!0,
                                                                                                                                                                  !1)
    IL_000c:  ret
  } // end of method QueryExpressions::'<MultipleLet>b__36'

  .method private hidebysig static class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string> 
          '<MultipleLet>b__37'(class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string> '<>h__TransparentIdentifier34') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance !0 class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>::get_a()
    IL_0007:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
    IL_000c:  newobj     instance void class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>::.ctor(!0,
                                                                                                                                                                                                        !1)
    IL_0011:  ret
  } // end of method QueryExpressions::'<MultipleLet>b__37'

  .method private hidebysig static string 
          '<MultipleLet>b__38'(class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string> '<>h__TransparentIdentifier35') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       23 (0x17)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>::'get_<>h__TransparentIdentifier34'()
    IL_0006:  callvirt   instance !1 class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>::get_b()
    IL_000b:  ldarg.0
    IL_000c:  callvirt   instance !1 class '<>f__AnonymousType9`2'<class '<>f__AnonymousType8`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,string>,string>::get_c()
    IL_0011:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0016:  ret
  } // end of method QueryExpressions::'<MultipleLet>b__38'

  .method private hidebysig static int32 
          '<Join>b__3d'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::CustomerID
    IL_0006:  ret
  } // end of method QueryExpressions::'<Join>b__3d'

  .method private hidebysig static int32 
          '<Join>b__3e'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::CustomerID
    IL_0006:  ret
  } // end of method QueryExpressions::'<Join>b__3e'

  .method private hidebysig static class '<>f__AnonymousTypea`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal> 
          '<Join>b__3f'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c,
                        class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
    IL_0006:  ldarg.1
    IL_0007:  ldfld      valuetype [mscorlib]System.DateTime ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::OrderDate
    IL_000c:  ldarg.1
    IL_000d:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Total
    IL_0012:  newobj     instance void class '<>f__AnonymousTypea`3'<string,valuetype [mscorlib]System.DateTime,valuetype [mscorlib]System.Decimal>::.ctor(!0,
                                                                                                                                                           !1,
                                                                                                                                                           !2)
    IL_0017:  ret
  } // end of method QueryExpressions::'<Join>b__3f'

  .method private hidebysig static int32 
          '<JoinInto>b__45'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::CustomerID
    IL_0006:  ret
  } // end of method QueryExpressions::'<JoinInto>b__45'

  .method private hidebysig static int32 
          '<JoinInto>b__46'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::CustomerID
    IL_0006:  ret
  } // end of method QueryExpressions::'<JoinInto>b__46'

  .method private hidebysig static class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> 
          '<JoinInto>b__47'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c,
                            class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order> co) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::.ctor(!0,
                                                                                                                                                                                                                                                                                                !1)
    IL_0007:  ret
  } // end of method QueryExpressions::'<JoinInto>b__47'

  .method private hidebysig static class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32> 
          '<JoinInto>b__48'(class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>> '<>h__TransparentIdentifier43') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance !1 class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::get_co()
    IL_0007:  call       int32 [System.Core]System.Linq.Enumerable::Count<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  newobj     instance void class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>::.ctor(!0,
                                                                                                                                                                                                                                                                                                                                     !1)
    IL_0011:  ret
  } // end of method QueryExpressions::'<JoinInto>b__48'

  .method private hidebysig static bool  '<JoinInto>b__49'(class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32> '<>h__TransparentIdentifier44') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !1 class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>::get_n()
    IL_0006:  ldc.i4.s   10
    IL_0008:  clt
    IL_000a:  ldc.i4.0
    IL_000b:  ceq
    IL_000d:  ret
  } // end of method QueryExpressions::'<JoinInto>b__49'

  .method private hidebysig static class '<>f__AnonymousTyped`2'<string,int32> 
          '<JoinInto>b__4a'(class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32> '<>h__TransparentIdentifier44') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>::'get_<>h__TransparentIdentifier43'()
    IL_0006:  callvirt   instance !0 class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>::get_c()
    IL_000b:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
    IL_0010:  ldarg.0
    IL_0011:  callvirt   instance !1 class '<>f__AnonymousTypec`2'<class '<>f__AnonymousTypeb`2'<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer,class [mscorlib]System.Collections.Generic.IEnumerable`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order>>,int32>::get_n()
    IL_0016:  newobj     instance void class '<>f__AnonymousTyped`2'<string,int32>::.ctor(!0,
                                                                                          !1)
    IL_001b:  ret
  } // end of method QueryExpressions::'<JoinInto>b__4a'

  .method private hidebysig static string 
          '<OrderBy>b__51'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Customer
    IL_0006:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
    IL_000b:  ret
  } // end of method QueryExpressions::'<OrderBy>b__51'

  .method private hidebysig static valuetype [mscorlib]System.Decimal 
          '<OrderBy>b__52'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order o) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Order::Total
    IL_0006:  ret
  } // end of method QueryExpressions::'<OrderBy>b__52'

  .method private hidebysig static string 
          '<GroupBy>b__55'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Country
    IL_0006:  ret
  } // end of method QueryExpressions::'<GroupBy>b__55'

  .method private hidebysig static string 
          '<GroupBy>b__56'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Name
    IL_0006:  ret
  } // end of method QueryExpressions::'<GroupBy>b__56'

  .method private hidebysig static bool  '<ExplicitType>b__59'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::City
    IL_0006:  ldstr      "London"
    IL_000b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0010:  ret
  } // end of method QueryExpressions::'<ExplicitType>b__59'

  .method private hidebysig static string 
          '<QueryContinuation>b__5b'(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer c) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer::Country
    IL_0006:  ret
  } // end of method QueryExpressions::'<QueryContinuation>b__5b'

  .method private hidebysig static class '<>f__AnonymousTypee`2'<string,int32> 
          '<QueryContinuation>b__5c'(class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer> g) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class [System.Core]System.Linq.IGrouping`2<string,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>::get_Key()
    IL_0006:  ldarg.0
    IL_0007:  call       int32 [System.Core]System.Linq.Enumerable::Count<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions/Customer>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  newobj     instance void class '<>f__AnonymousTypee`2'<string,int32>::.ctor(!0,
                                                                                          !1)
    IL_0011:  ret
  } // end of method QueryExpressions::'<QueryContinuation>b__5c'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.QueryExpressions

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType0`3'<'<Name>j__TPar','<OrderID>j__TPar','<Total>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<Name>j__TPar' '<Name>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<OrderID>j__TPar' '<OrderID>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Total>j__TPar' '<Total>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<Name>j__TPar' Name,
                               !'<OrderID>j__TPar' OrderID,
                               !'<Total>j__TPar' Total) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0014:  ldarg.0
    IL_0015:  ldarg.3
    IL_0016:  stfld      !2 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_001b:  ret
  } // end of method '<>f__AnonymousType0`3'::.ctor

  .method public hidebysig specialname instance !'<Name>j__TPar' 
          get_Name() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`3'::get_Name

  .method public hidebysig specialname instance !'<OrderID>j__TPar' 
          get_OrderID() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`3'::get_OrderID

  .method public hidebysig specialname instance !'<Total>j__TPar' 
          get_Total() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !2 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`3'::get_Total

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       115 (0x73)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ Name = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0019:  box        !'<Name>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", OrderID = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0037:  box        !'<OrderID>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      ", Total = "
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  ldarg.0
    IL_0050:  ldfld      !2 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0055:  box        !'<Total>j__TPar'
    IL_005a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_005f:  pop
    IL_0060:  ldloc.0
    IL_0061:  ldstr      " }"
    IL_0066:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_006b:  pop
    IL_006c:  ldloc.0
    IL_006d:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0072:  ret
  } // end of method '<>f__AnonymousType0`3'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       83 (0x53)
    .maxstack  3
    .locals init (class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0051

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::Equals(!0,
                                                                                                                               !0)
    IL_0020:  brfalse.s  IL_0051

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::Equals(!0,
                                                                                                                                  !0)
    IL_0038:  brfalse.s  IL_0051

    IL_003a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::get_Default()
    IL_003f:  ldarg.0
    IL_0040:  ldfld      !2 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0045:  ldloc.0
    IL_0046:  ldfld      !2 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_004b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::Equals(!0,
                                                                                                                                !0)
    IL_0050:  ret

    IL_0051:  ldc.i4.0
    IL_0052:  ret
  } // end of method '<>f__AnonymousType0`3'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       83 (0x53)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0x6ad4015d
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldc.i4     0xa5555529
    IL_003d:  ldloc.0
    IL_003e:  mul
    IL_003f:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::get_Default()
    IL_0044:  ldarg.0
    IL_0045:  ldfld      !2 class '<>f__AnonymousType0`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_004a:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::GetHashCode(!0)
    IL_004f:  add
    IL_0050:  stloc.0
    IL_0051:  ldloc.0
    IL_0052:  ret
  } // end of method '<>f__AnonymousType0`3'::GetHashCode

  .property instance !'<Name>j__TPar' Name()
  {
    .get instance !'<Name>j__TPar' '<>f__AnonymousType0`3'::get_Name()
  } // end of property '<>f__AnonymousType0`3'::Name
  .property instance !'<OrderID>j__TPar' OrderID()
  {
    .get instance !'<OrderID>j__TPar' '<>f__AnonymousType0`3'::get_OrderID()
  } // end of property '<>f__AnonymousType0`3'::OrderID
  .property instance !'<Total>j__TPar' Total()
  {
    .get instance !'<Total>j__TPar' '<>f__AnonymousType0`3'::get_Total()
  } // end of property '<>f__AnonymousType0`3'::Total
} // end of class '<>f__AnonymousType0`3'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType1`2'<'<c>j__TPar','<o>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<c>j__TPar' '<c>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<o>j__TPar' '<o>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<c>j__TPar' c,
                               !'<o>j__TPar' o) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<c>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<o>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType1`2'::.ctor

  .method public hidebysig specialname instance !'<c>j__TPar' 
          get_c() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<c>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1`2'::get_c

  .method public hidebysig specialname instance !'<o>j__TPar' 
          get_o() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<o>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1`2'::get_o

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ c = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<c>i__Field'
    IL_0019:  box        !'<c>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", o = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<o>i__Field'
    IL_0037:  box        !'<o>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType1`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<c>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<c>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<o>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<o>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType1`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0x31b06f1c
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<c>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<c>j__TPar',!'<o>j__TPar'>::'<o>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType1`2'::GetHashCode

  .property instance !'<c>j__TPar' c()
  {
    .get instance !'<c>j__TPar' '<>f__AnonymousType1`2'::get_c()
  } // end of property '<>f__AnonymousType1`2'::c
  .property instance !'<o>j__TPar' o()
  {
    .get instance !'<o>j__TPar' '<>f__AnonymousType1`2'::get_o()
  } // end of property '<>f__AnonymousType1`2'::o
} // end of class '<>f__AnonymousType1`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType2`3'<'<Name>j__TPar','<OrderID>j__TPar','<Quantity>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<Name>j__TPar' '<Name>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<OrderID>j__TPar' '<OrderID>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Quantity>j__TPar' '<Quantity>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<Name>j__TPar' Name,
                               !'<OrderID>j__TPar' OrderID,
                               !'<Quantity>j__TPar' Quantity) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Name>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<OrderID>i__Field'
    IL_0014:  ldarg.0
    IL_0015:  ldarg.3
    IL_0016:  stfld      !2 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Quantity>i__Field'
    IL_001b:  ret
  } // end of method '<>f__AnonymousType2`3'::.ctor

  .method public hidebysig specialname instance !'<Name>j__TPar' 
          get_Name() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Name>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType2`3'::get_Name

  .method public hidebysig specialname instance !'<OrderID>j__TPar' 
          get_OrderID() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<OrderID>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType2`3'::get_OrderID

  .method public hidebysig specialname instance !'<Quantity>j__TPar' 
          get_Quantity() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !2 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Quantity>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType2`3'::get_Quantity

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       115 (0x73)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ Name = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Name>i__Field'
    IL_0019:  box        !'<Name>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", OrderID = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<OrderID>i__Field'
    IL_0037:  box        !'<OrderID>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      ", Quantity = "
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  ldarg.0
    IL_0050:  ldfld      !2 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Quantity>i__Field'
    IL_0055:  box        !'<Quantity>j__TPar'
    IL_005a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_005f:  pop
    IL_0060:  ldloc.0
    IL_0061:  ldstr      " }"
    IL_0066:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_006b:  pop
    IL_006c:  ldloc.0
    IL_006d:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0072:  ret
  } // end of method '<>f__AnonymousType2`3'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       83 (0x53)
    .maxstack  3
    .locals init (class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0051

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Name>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Name>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::Equals(!0,
                                                                                                                               !0)
    IL_0020:  brfalse.s  IL_0051

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<OrderID>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<OrderID>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::Equals(!0,
                                                                                                                                  !0)
    IL_0038:  brfalse.s  IL_0051

    IL_003a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Quantity>j__TPar'>::get_Default()
    IL_003f:  ldarg.0
    IL_0040:  ldfld      !2 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Quantity>i__Field'
    IL_0045:  ldloc.0
    IL_0046:  ldfld      !2 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Quantity>i__Field'
    IL_004b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Quantity>j__TPar'>::Equals(!0,
                                                                                                                                   !0)
    IL_0050:  ret

    IL_0051:  ldc.i4.0
    IL_0052:  ret
  } // end of method '<>f__AnonymousType2`3'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       83 (0x53)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0xe53298dd
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Name>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<OrderID>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldc.i4     0xa5555529
    IL_003d:  ldloc.0
    IL_003e:  mul
    IL_003f:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Quantity>j__TPar'>::get_Default()
    IL_0044:  ldarg.0
    IL_0045:  ldfld      !2 class '<>f__AnonymousType2`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<Quantity>j__TPar'>::'<Quantity>i__Field'
    IL_004a:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Quantity>j__TPar'>::GetHashCode(!0)
    IL_004f:  add
    IL_0050:  stloc.0
    IL_0051:  ldloc.0
    IL_0052:  ret
  } // end of method '<>f__AnonymousType2`3'::GetHashCode

  .property instance !'<Name>j__TPar' Name()
  {
    .get instance !'<Name>j__TPar' '<>f__AnonymousType2`3'::get_Name()
  } // end of property '<>f__AnonymousType2`3'::Name
  .property instance !'<OrderID>j__TPar' OrderID()
  {
    .get instance !'<OrderID>j__TPar' '<>f__AnonymousType2`3'::get_OrderID()
  } // end of property '<>f__AnonymousType2`3'::OrderID
  .property instance !'<Quantity>j__TPar'
          Quantity()
  {
    .get instance !'<Quantity>j__TPar' '<>f__AnonymousType2`3'::get_Quantity()
  } // end of property '<>f__AnonymousType2`3'::Quantity
} // end of class '<>f__AnonymousType2`3'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType3`2'<'<<>h__TransparentIdentifier1c>j__TPar','<d>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<<>h__TransparentIdentifier1c>j__TPar' '<<>h__TransparentIdentifier1c>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<d>j__TPar' '<d>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<<>h__TransparentIdentifier1c>j__TPar' '<>h__TransparentIdentifier1c',
                               !'<d>j__TPar' d) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>::'<<>h__TransparentIdentifier1c>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>::'<d>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType3`2'::.ctor

  .method public hidebysig specialname instance !'<<>h__TransparentIdentifier1c>j__TPar' 
          'get_<>h__TransparentIdentifier1c'() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>::'<<>h__TransparentIdentifier1c>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType3`2'::'get_<>h__TransparentIdentifier1c'

  .method public hidebysig specialname instance !'<d>j__TPar' 
          get_d() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>::'<d>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType3`2'::get_d

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ <>h__TransparentIdentifier1c = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>::'<<>h__TransparentIdentifier1c>i__Field'
    IL_0019:  box        !'<<>h__TransparentIdentifier1c>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", d = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>::'<d>i__Field'
    IL_0037:  box        !'<d>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType3`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier1c>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>::'<<>h__TransparentIdentifier1c>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>::'<<>h__TransparentIdentifier1c>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier1c>j__TPar'>::Equals(!0,
                                                                                                                                                       !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<d>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>::'<d>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>::'<d>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<d>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType3`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0x528c7730
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier1c>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>::'<<>h__TransparentIdentifier1c>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier1c>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<d>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType3`2'<!'<<>h__TransparentIdentifier1c>j__TPar',!'<d>j__TPar'>::'<d>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<d>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType3`2'::GetHashCode

  .property instance !'<<>h__TransparentIdentifier1c>j__TPar'
          '<>h__TransparentIdentifier1c'()
  {
    .get instance !'<<>h__TransparentIdentifier1c>j__TPar' '<>f__AnonymousType3`2'::'get_<>h__TransparentIdentifier1c'()
  } // end of property '<>f__AnonymousType3`2'::'<>h__TransparentIdentifier1c'
  .property instance !'<d>j__TPar' d()
  {
    .get instance !'<d>j__TPar' '<>f__AnonymousType3`2'::get_d()
  } // end of property '<>f__AnonymousType3`2'::d
} // end of class '<>f__AnonymousType3`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType4`2'<'<<>h__TransparentIdentifier1d>j__TPar','<x>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<<>h__TransparentIdentifier1d>j__TPar' '<<>h__TransparentIdentifier1d>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<x>j__TPar' '<x>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<<>h__TransparentIdentifier1d>j__TPar' '<>h__TransparentIdentifier1d',
                               !'<x>j__TPar' x) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>::'<<>h__TransparentIdentifier1d>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType4`2'::.ctor

  .method public hidebysig specialname instance !'<<>h__TransparentIdentifier1d>j__TPar' 
          'get_<>h__TransparentIdentifier1d'() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>::'<<>h__TransparentIdentifier1d>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType4`2'::'get_<>h__TransparentIdentifier1d'

  .method public hidebysig specialname instance !'<x>j__TPar' 
          get_x() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType4`2'::get_x

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ <>h__TransparentIdentifier1d = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>::'<<>h__TransparentIdentifier1d>i__Field'
    IL_0019:  box        !'<<>h__TransparentIdentifier1d>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", x = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0037:  box        !'<x>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType4`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier1d>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>::'<<>h__TransparentIdentifier1d>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>::'<<>h__TransparentIdentifier1d>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier1d>j__TPar'>::Equals(!0,
                                                                                                                                                       !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType4`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0x3b8afcfe
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier1d>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>::'<<>h__TransparentIdentifier1d>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier1d>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType4`2'<!'<<>h__TransparentIdentifier1d>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType4`2'::GetHashCode

  .property instance !'<<>h__TransparentIdentifier1d>j__TPar'
          '<>h__TransparentIdentifier1d'()
  {
    .get instance !'<<>h__TransparentIdentifier1d>j__TPar' '<>f__AnonymousType4`2'::'get_<>h__TransparentIdentifier1d'()
  } // end of property '<>f__AnonymousType4`2'::'<>h__TransparentIdentifier1d'
  .property instance !'<x>j__TPar' x()
  {
    .get instance !'<x>j__TPar' '<>f__AnonymousType4`2'::get_x()
  } // end of property '<>f__AnonymousType4`2'::x
} // end of class '<>f__AnonymousType4`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType5`3'<'<Name>j__TPar','<OrderID>j__TPar','<x>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<Name>j__TPar' '<Name>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<OrderID>j__TPar' '<OrderID>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<x>j__TPar' '<x>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<Name>j__TPar' Name,
                               !'<OrderID>j__TPar' OrderID,
                               !'<x>j__TPar' x) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<Name>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<OrderID>i__Field'
    IL_0014:  ldarg.0
    IL_0015:  ldarg.3
    IL_0016:  stfld      !2 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_001b:  ret
  } // end of method '<>f__AnonymousType5`3'::.ctor

  .method public hidebysig specialname instance !'<Name>j__TPar' 
          get_Name() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<Name>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType5`3'::get_Name

  .method public hidebysig specialname instance !'<OrderID>j__TPar' 
          get_OrderID() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<OrderID>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType5`3'::get_OrderID

  .method public hidebysig specialname instance !'<x>j__TPar' 
          get_x() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !2 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType5`3'::get_x

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       115 (0x73)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ Name = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<Name>i__Field'
    IL_0019:  box        !'<Name>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", OrderID = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<OrderID>i__Field'
    IL_0037:  box        !'<OrderID>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      ", x = "
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  ldarg.0
    IL_0050:  ldfld      !2 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0055:  box        !'<x>j__TPar'
    IL_005a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_005f:  pop
    IL_0060:  ldloc.0
    IL_0061:  ldstr      " }"
    IL_0066:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_006b:  pop
    IL_006c:  ldloc.0
    IL_006d:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0072:  ret
  } // end of method '<>f__AnonymousType5`3'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       83 (0x53)
    .maxstack  3
    .locals init (class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0051

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<Name>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<Name>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::Equals(!0,
                                                                                                                               !0)
    IL_0020:  brfalse.s  IL_0051

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<OrderID>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<OrderID>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::Equals(!0,
                                                                                                                                  !0)
    IL_0038:  brfalse.s  IL_0051

    IL_003a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::get_Default()
    IL_003f:  ldarg.0
    IL_0040:  ldfld      !2 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_0045:  ldloc.0
    IL_0046:  ldfld      !2 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_004b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0050:  ret

    IL_0051:  ldc.i4.0
    IL_0052:  ret
  } // end of method '<>f__AnonymousType5`3'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       83 (0x53)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0x394329c9
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<Name>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<OrderID>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldc.i4     0xa5555529
    IL_003d:  ldloc.0
    IL_003e:  mul
    IL_003f:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::get_Default()
    IL_0044:  ldarg.0
    IL_0045:  ldfld      !2 class '<>f__AnonymousType5`3'<!'<Name>j__TPar',!'<OrderID>j__TPar',!'<x>j__TPar'>::'<x>i__Field'
    IL_004a:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<x>j__TPar'>::GetHashCode(!0)
    IL_004f:  add
    IL_0050:  stloc.0
    IL_0051:  ldloc.0
    IL_0052:  ret
  } // end of method '<>f__AnonymousType5`3'::GetHashCode

  .property instance !'<Name>j__TPar' Name()
  {
    .get instance !'<Name>j__TPar' '<>f__AnonymousType5`3'::get_Name()
  } // end of property '<>f__AnonymousType5`3'::Name
  .property instance !'<OrderID>j__TPar' OrderID()
  {
    .get instance !'<OrderID>j__TPar' '<>f__AnonymousType5`3'::get_OrderID()
  } // end of property '<>f__AnonymousType5`3'::OrderID
  .property instance !'<x>j__TPar' x()
  {
    .get instance !'<x>j__TPar' '<>f__AnonymousType5`3'::get_x()
  } // end of property '<>f__AnonymousType5`3'::x
} // end of class '<>f__AnonymousType5`3'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType6`2'<'<o>j__TPar','<t>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<o>j__TPar' '<o>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<t>j__TPar' '<t>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<o>j__TPar' o,
                               !'<t>j__TPar' t) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<o>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<t>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType6`2'::.ctor

  .method public hidebysig specialname instance !'<o>j__TPar' 
          get_o() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<o>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType6`2'::get_o

  .method public hidebysig specialname instance !'<t>j__TPar' 
          get_t() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<t>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType6`2'::get_t

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ o = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<o>i__Field'
    IL_0019:  box        !'<o>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", t = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<t>i__Field'
    IL_0037:  box        !'<t>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType6`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<o>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<o>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<t>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<t>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<t>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<t>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType6`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0xadfdc98c
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<o>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<o>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<t>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType6`2'<!'<o>j__TPar',!'<t>j__TPar'>::'<t>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<t>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType6`2'::GetHashCode

  .property instance !'<o>j__TPar' o()
  {
    .get instance !'<o>j__TPar' '<>f__AnonymousType6`2'::get_o()
  } // end of property '<>f__AnonymousType6`2'::o
  .property instance !'<t>j__TPar' t()
  {
    .get instance !'<t>j__TPar' '<>f__AnonymousType6`2'::get_t()
  } // end of property '<>f__AnonymousType6`2'::t
} // end of class '<>f__AnonymousType6`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType7`2'<'<OrderID>j__TPar','<Total>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<OrderID>j__TPar' '<OrderID>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Total>j__TPar' '<Total>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<OrderID>j__TPar' OrderID,
                               !'<Total>j__TPar' Total) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType7`2'::.ctor

  .method public hidebysig specialname instance !'<OrderID>j__TPar' 
          get_OrderID() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType7`2'::get_OrderID

  .method public hidebysig specialname instance !'<Total>j__TPar' 
          get_Total() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType7`2'::get_Total

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ OrderID = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0019:  box        !'<OrderID>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", Total = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0037:  box        !'<Total>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType7`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::Equals(!0,
                                                                                                                                  !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::Equals(!0,
                                                                                                                                !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType7`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0xc461253a
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<OrderID>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderID>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType7`2'<!'<OrderID>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType7`2'::GetHashCode

  .property instance !'<OrderID>j__TPar' OrderID()
  {
    .get instance !'<OrderID>j__TPar' '<>f__AnonymousType7`2'::get_OrderID()
  } // end of property '<>f__AnonymousType7`2'::OrderID
  .property instance !'<Total>j__TPar' Total()
  {
    .get instance !'<Total>j__TPar' '<>f__AnonymousType7`2'::get_Total()
  } // end of property '<>f__AnonymousType7`2'::Total
} // end of class '<>f__AnonymousType7`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType8`2'<'<a>j__TPar','<b>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<a>j__TPar' '<a>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<b>j__TPar' '<b>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<a>j__TPar' a,
                               !'<b>j__TPar' b) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<a>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<b>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType8`2'::.ctor

  .method public hidebysig specialname instance !'<a>j__TPar' 
          get_a() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<a>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType8`2'::get_a

  .method public hidebysig specialname instance !'<b>j__TPar' 
          get_b() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<b>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType8`2'::get_b

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ a = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<a>i__Field'
    IL_0019:  box        !'<a>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", b = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<b>i__Field'
    IL_0037:  box        !'<b>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType8`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<a>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<a>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<a>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<a>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<b>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<b>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<b>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<b>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType8`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0xd40b4140
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<a>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<a>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<a>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<b>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType8`2'<!'<a>j__TPar',!'<b>j__TPar'>::'<b>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<b>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType8`2'::GetHashCode

  .property instance !'<a>j__TPar' a()
  {
    .get instance !'<a>j__TPar' '<>f__AnonymousType8`2'::get_a()
  } // end of property '<>f__AnonymousType8`2'::a
  .property instance !'<b>j__TPar' b()
  {
    .get instance !'<b>j__TPar' '<>f__AnonymousType8`2'::get_b()
  } // end of property '<>f__AnonymousType8`2'::b
} // end of class '<>f__AnonymousType8`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType9`2'<'<<>h__TransparentIdentifier34>j__TPar','<c>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<<>h__TransparentIdentifier34>j__TPar' '<<>h__TransparentIdentifier34>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<c>j__TPar' '<c>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<<>h__TransparentIdentifier34>j__TPar' '<>h__TransparentIdentifier34',
                               !'<c>j__TPar' c) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>::'<<>h__TransparentIdentifier34>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>::'<c>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType9`2'::.ctor

  .method public hidebysig specialname instance !'<<>h__TransparentIdentifier34>j__TPar' 
          'get_<>h__TransparentIdentifier34'() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>::'<<>h__TransparentIdentifier34>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType9`2'::'get_<>h__TransparentIdentifier34'

  .method public hidebysig specialname instance !'<c>j__TPar' 
          get_c() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>::'<c>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType9`2'::get_c

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ <>h__TransparentIdentifier34 = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>::'<<>h__TransparentIdentifier34>i__Field'
    IL_0019:  box        !'<<>h__TransparentIdentifier34>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", c = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>::'<c>i__Field'
    IL_0037:  box        !'<c>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType9`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier34>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>::'<<>h__TransparentIdentifier34>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>::'<<>h__TransparentIdentifier34>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier34>j__TPar'>::Equals(!0,
                                                                                                                                                       !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>::'<c>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>::'<c>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType9`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0x19e5d042
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier34>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>::'<<>h__TransparentIdentifier34>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier34>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType9`2'<!'<<>h__TransparentIdentifier34>j__TPar',!'<c>j__TPar'>::'<c>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType9`2'::GetHashCode

  .property instance !'<<>h__TransparentIdentifier34>j__TPar'
          '<>h__TransparentIdentifier34'()
  {
    .get instance !'<<>h__TransparentIdentifier34>j__TPar' '<>f__AnonymousType9`2'::'get_<>h__TransparentIdentifier34'()
  } // end of property '<>f__AnonymousType9`2'::'<>h__TransparentIdentifier34'
  .property instance !'<c>j__TPar' c()
  {
    .get instance !'<c>j__TPar' '<>f__AnonymousType9`2'::get_c()
  } // end of property '<>f__AnonymousType9`2'::c
} // end of class '<>f__AnonymousType9`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousTypea`3'<'<Name>j__TPar','<OrderDate>j__TPar','<Total>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<Name>j__TPar' '<Name>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<OrderDate>j__TPar' '<OrderDate>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Total>j__TPar' '<Total>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<Name>j__TPar' Name,
                               !'<OrderDate>j__TPar' OrderDate,
                               !'<Total>j__TPar' Total) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<OrderDate>i__Field'
    IL_0014:  ldarg.0
    IL_0015:  ldarg.3
    IL_0016:  stfld      !2 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_001b:  ret
  } // end of method '<>f__AnonymousTypea`3'::.ctor

  .method public hidebysig specialname instance !'<Name>j__TPar' 
          get_Name() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousTypea`3'::get_Name

  .method public hidebysig specialname instance !'<OrderDate>j__TPar' 
          get_OrderDate() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<OrderDate>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousTypea`3'::get_OrderDate

  .method public hidebysig specialname instance !'<Total>j__TPar' 
          get_Total() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !2 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousTypea`3'::get_Total

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       115 (0x73)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ Name = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0019:  box        !'<Name>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", OrderDate = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<OrderDate>i__Field'
    IL_0037:  box        !'<OrderDate>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      ", Total = "
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  ldarg.0
    IL_0050:  ldfld      !2 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0055:  box        !'<Total>j__TPar'
    IL_005a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_005f:  pop
    IL_0060:  ldloc.0
    IL_0061:  ldstr      " }"
    IL_0066:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_006b:  pop
    IL_006c:  ldloc.0
    IL_006d:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0072:  ret
  } // end of method '<>f__AnonymousTypea`3'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       83 (0x53)
    .maxstack  3
    .locals init (class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0051

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::Equals(!0,
                                                                                                                               !0)
    IL_0020:  brfalse.s  IL_0051

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderDate>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<OrderDate>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<OrderDate>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderDate>j__TPar'>::Equals(!0,
                                                                                                                                    !0)
    IL_0038:  brfalse.s  IL_0051

    IL_003a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::get_Default()
    IL_003f:  ldarg.0
    IL_0040:  ldfld      !2 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_0045:  ldloc.0
    IL_0046:  ldfld      !2 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_004b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::Equals(!0,
                                                                                                                                !0)
    IL_0050:  ret

    IL_0051:  ldc.i4.0
    IL_0052:  ret
  } // end of method '<>f__AnonymousTypea`3'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       83 (0x53)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0x87627f5a
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Name>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderDate>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<OrderDate>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderDate>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldc.i4     0xa5555529
    IL_003d:  ldloc.0
    IL_003e:  mul
    IL_003f:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::get_Default()
    IL_0044:  ldarg.0
    IL_0045:  ldfld      !2 class '<>f__AnonymousTypea`3'<!'<Name>j__TPar',!'<OrderDate>j__TPar',!'<Total>j__TPar'>::'<Total>i__Field'
    IL_004a:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Total>j__TPar'>::GetHashCode(!0)
    IL_004f:  add
    IL_0050:  stloc.0
    IL_0051:  ldloc.0
    IL_0052:  ret
  } // end of method '<>f__AnonymousTypea`3'::GetHashCode

  .property instance !'<Name>j__TPar' Name()
  {
    .get instance !'<Name>j__TPar' '<>f__AnonymousTypea`3'::get_Name()
  } // end of property '<>f__AnonymousTypea`3'::Name
  .property instance !'<OrderDate>j__TPar'
          OrderDate()
  {
    .get instance !'<OrderDate>j__TPar' '<>f__AnonymousTypea`3'::get_OrderDate()
  } // end of property '<>f__AnonymousTypea`3'::OrderDate
  .property instance !'<Total>j__TPar' Total()
  {
    .get instance !'<Total>j__TPar' '<>f__AnonymousTypea`3'::get_Total()
  } // end of property '<>f__AnonymousTypea`3'::Total
} // end of class '<>f__AnonymousTypea`3'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousTypeb`2'<'<c>j__TPar','<co>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<c>j__TPar' '<c>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<co>j__TPar' '<co>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<c>j__TPar' c,
                               !'<co>j__TPar' co) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<c>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<co>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousTypeb`2'::.ctor

  .method public hidebysig specialname instance !'<c>j__TPar' 
          get_c() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<c>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousTypeb`2'::get_c

  .method public hidebysig specialname instance !'<co>j__TPar' 
          get_co() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<co>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousTypeb`2'::get_co

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ c = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<c>i__Field'
    IL_0019:  box        !'<c>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", co = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<co>i__Field'
    IL_0037:  box        !'<co>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousTypeb`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<c>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<c>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<co>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<co>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<co>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<co>j__TPar'>::Equals(!0,
                                                                                                                             !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousTypeb`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0xd95164b8
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<c>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<c>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<co>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousTypeb`2'<!'<c>j__TPar',!'<co>j__TPar'>::'<co>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<co>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousTypeb`2'::GetHashCode

  .property instance !'<c>j__TPar' c()
  {
    .get instance !'<c>j__TPar' '<>f__AnonymousTypeb`2'::get_c()
  } // end of property '<>f__AnonymousTypeb`2'::c
  .property instance !'<co>j__TPar' co()
  {
    .get instance !'<co>j__TPar' '<>f__AnonymousTypeb`2'::get_co()
  } // end of property '<>f__AnonymousTypeb`2'::co
} // end of class '<>f__AnonymousTypeb`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousTypec`2'<'<<>h__TransparentIdentifier43>j__TPar','<n>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<<>h__TransparentIdentifier43>j__TPar' '<<>h__TransparentIdentifier43>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<n>j__TPar' '<n>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<<>h__TransparentIdentifier43>j__TPar' '<>h__TransparentIdentifier43',
                               !'<n>j__TPar' n) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>::'<<>h__TransparentIdentifier43>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>::'<n>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousTypec`2'::.ctor

  .method public hidebysig specialname instance !'<<>h__TransparentIdentifier43>j__TPar' 
          'get_<>h__TransparentIdentifier43'() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>::'<<>h__TransparentIdentifier43>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousTypec`2'::'get_<>h__TransparentIdentifier43'

  .method public hidebysig specialname instance !'<n>j__TPar' 
          get_n() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>::'<n>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousTypec`2'::get_n

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ <>h__TransparentIdentifier43 = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>::'<<>h__TransparentIdentifier43>i__Field'
    IL_0019:  box        !'<<>h__TransparentIdentifier43>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", n = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>::'<n>i__Field'
    IL_0037:  box        !'<n>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousTypec`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier43>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>::'<<>h__TransparentIdentifier43>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>::'<<>h__TransparentIdentifier43>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier43>j__TPar'>::Equals(!0,
                                                                                                                                                       !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<n>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>::'<n>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>::'<n>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<n>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousTypec`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0xc3dcc655
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier43>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>::'<<>h__TransparentIdentifier43>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<<>h__TransparentIdentifier43>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<n>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousTypec`2'<!'<<>h__TransparentIdentifier43>j__TPar',!'<n>j__TPar'>::'<n>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<n>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousTypec`2'::GetHashCode

  .property instance !'<<>h__TransparentIdentifier43>j__TPar'
          '<>h__TransparentIdentifier43'()
  {
    .get instance !'<<>h__TransparentIdentifier43>j__TPar' '<>f__AnonymousTypec`2'::'get_<>h__TransparentIdentifier43'()
  } // end of property '<>f__AnonymousTypec`2'::'<>h__TransparentIdentifier43'
  .property instance !'<n>j__TPar' n()
  {
    .get instance !'<n>j__TPar' '<>f__AnonymousTypec`2'::get_n()
  } // end of property '<>f__AnonymousTypec`2'::n
} // end of class '<>f__AnonymousTypec`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousTyped`2'<'<Name>j__TPar','<OrderCount>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<Name>j__TPar' '<Name>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<OrderCount>j__TPar' '<OrderCount>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<Name>j__TPar' Name,
                               !'<OrderCount>j__TPar' OrderCount) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<Name>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<OrderCount>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousTyped`2'::.ctor

  .method public hidebysig specialname instance !'<Name>j__TPar' 
          get_Name() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<Name>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousTyped`2'::get_Name

  .method public hidebysig specialname instance !'<OrderCount>j__TPar' 
          get_OrderCount() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<OrderCount>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousTyped`2'::get_OrderCount

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ Name = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<Name>i__Field'
    IL_0019:  box        !'<Name>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", OrderCount = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<OrderCount>i__Field'
    IL_0037:  box        !'<OrderCount>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousTyped`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<Name>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<Name>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::Equals(!0,
                                                                                                                               !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderCount>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<OrderCount>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<OrderCount>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderCount>j__TPar'>::Equals(!0,
                                                                                                                                     !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousTyped`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0x38951b52
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<Name>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Name>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderCount>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousTyped`2'<!'<Name>j__TPar',!'<OrderCount>j__TPar'>::'<OrderCount>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<OrderCount>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousTyped`2'::GetHashCode

  .property instance !'<Name>j__TPar' Name()
  {
    .get instance !'<Name>j__TPar' '<>f__AnonymousTyped`2'::get_Name()
  } // end of property '<>f__AnonymousTyped`2'::Name
  .property instance !'<OrderCount>j__TPar'
          OrderCount()
  {
    .get instance !'<OrderCount>j__TPar' '<>f__AnonymousTyped`2'::get_OrderCount()
  } // end of property '<>f__AnonymousTyped`2'::OrderCount
} // end of class '<>f__AnonymousTyped`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousTypee`2'<'<Country>j__TPar','<CustCount>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<Country>j__TPar' '<Country>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<CustCount>j__TPar' '<CustCount>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<Country>j__TPar' Country,
                               !'<CustCount>j__TPar' CustCount) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<Country>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<CustCount>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousTypee`2'::.ctor

  .method public hidebysig specialname instance !'<Country>j__TPar' 
          get_Country() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<Country>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousTypee`2'::get_Country

  .method public hidebysig specialname instance !'<CustCount>j__TPar' 
          get_CustCount() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<CustCount>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousTypee`2'::get_CustCount

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ Country = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<Country>i__Field'
    IL_0019:  box        !'<Country>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", CustCount = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<CustCount>i__Field'
    IL_0037:  box        !'<CustCount>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousTypee`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Country>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<Country>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<Country>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Country>j__TPar'>::Equals(!0,
                                                                                                                                  !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<CustCount>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<CustCount>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<CustCount>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<CustCount>j__TPar'>::Equals(!0,
                                                                                                                                    !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousTypee`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0x8cdb705a
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Country>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<Country>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Country>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<CustCount>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousTypee`2'<!'<Country>j__TPar',!'<CustCount>j__TPar'>::'<CustCount>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<CustCount>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousTypee`2'::GetHashCode

  .property instance !'<Country>j__TPar' Country()
  {
    .get instance !'<Country>j__TPar' '<>f__AnonymousTypee`2'::get_Country()
  } // end of property '<>f__AnonymousTypee`2'::Country
  .property instance !'<CustCount>j__TPar'
          CustCount()
  {
    .get instance !'<CustCount>j__TPar' '<>f__AnonymousTypee`2'::get_CustCount()
  } // end of property '<>f__AnonymousTypee`2'::CustCount
} // end of class '<>f__AnonymousTypee`2'


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../../ICSharpCode.Decompiler.Tests/TestCases/Pretty\QueryExpressions.opt.res
