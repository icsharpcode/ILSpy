.class /* 33554434 */ public auto ansi beforefieldinit ClassLibrary1.UnknownClassTest
	extends [netstandard]System.EventArgs
{
	// Methods
	.method /* 100663297 */ public hidebysig 
		instance void MethodUnknownClass () cil managed 
	{
		// Method begins at RVA 0x2050
		// Header size: 12
		// Code size: 325 (0x145)
		.maxstack 4
		.locals /* 285212673 */ init (
			[0] class [UnknownAssembly]UnknownNamespace.UnknownClass 'instance',
			[1] valuetype [netstandard]System.Nullable`1<int32> valueOfProp,
			[2] valuetype [netstandard]System.Nullable`1<int32> x,
			[3] class [netstandard]System.Collections.Generic.List`1<object> list,
			[4] string valueOfIndexer,
			[5] native int valueOfMultiIndexer,
			[6] valuetype [netstandard]System.Nullable`1<int32>,
			[7] valuetype [netstandard]System.Nullable`1<int64>
		)

		IL_0000: nop
		IL_0001: newobj instance void [UnknownAssembly]UnknownNamespace.UnknownClass::.ctor() /* 167772171 */
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: callvirt instance valuetype [netstandard]System.Nullable`1<int32> [UnknownAssembly]UnknownNamespace.UnknownClass::get_UnknownProperty() /* 167772172 */
		IL_000d: stloc.1
		IL_000e: ldloc.0
		IL_000f: ldloca.s 6
		IL_0011: ldloca.s 1
		IL_0013: call instance !0 valuetype [netstandard]System.Nullable`1<int32>::GetValueOrDefault() /* 167772173 */
		IL_0018: call instance void valuetype [netstandard]System.Nullable`1<int32>::.ctor(!0) /* 167772174 */
		IL_001d: ldloc.s 6
		IL_001f: callvirt instance void [UnknownAssembly]UnknownNamespace.UnknownClass::set_UnknownProperty(valuetype [netstandard]System.Nullable`1<int32>) /* 167772175 */
		IL_0024: nop
		IL_0025: ldloc.s 6
		IL_0027: stloc.2
		IL_0028: newobj instance void class [netstandard]System.Collections.Generic.List`1<object>::.ctor() /* 167772176 */
		IL_002d: dup
		IL_002e: ldloc.0
		IL_002f: ldloca.s 1
		IL_0031: call instance !0 valuetype [netstandard]System.Nullable`1<int32>::get_Value() /* 167772177 */
		IL_0036: callvirt instance string [UnknownAssembly]UnknownNamespace.UnknownClass::get_NotPropertyWithParameter(int32) /* 167772178 */
		IL_003b: dup
		IL_003c: brtrue.s IL_0044

		IL_003e: pop
		IL_003f: ldstr "" /* 1879048193 */

		IL_0044: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0049: nop
		IL_004a: dup
		IL_004b: ldloc.0
		IL_004c: callvirt instance string [UnknownAssembly]UnknownNamespace.UnknownClass::get_NotProperty() /* 167772180 */
		IL_0051: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0056: nop
		IL_0057: dup
		IL_0058: ldloc.0
		IL_0059: ldc.i4.s 42
		IL_005b: callvirt instance string [UnknownAssembly]UnknownNamespace.UnknownClass::get_NotPropertyWithGeneric<string>(int32) /* 721420289 */
		IL_0060: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0065: nop
		IL_0066: dup
		IL_0067: ldloc.0
		IL_0068: ldc.i4.s 42
		IL_006a: callvirt instance string [UnknownAssembly]UnknownNamespace.UnknownClass::get_NotPropertyWithParameter(int32) /* 167772178 */
		IL_006f: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0074: nop
		IL_0075: dup
		IL_0076: ldloc.0
		IL_0077: ldc.i4 -2147483648
		IL_007c: callvirt instance string [UnknownAssembly]UnknownNamespace.UnknownClass::get_NotPropertyWithParameterAndGeneric<object>(int32) /* 721420290 */
		IL_0081: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0086: nop
		IL_0087: dup
		IL_0088: ldloc.0
		IL_0089: callvirt instance string [UnknownAssembly]UnknownNamespace.UnknownClass::get_get_PropertyCalledGet() /* 167772183 */
		IL_008e: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0093: nop
		IL_0094: dup
		IL_0095: ldloc.0
		IL_0096: callvirt instance int32 [UnknownAssembly]UnknownNamespace.UnknownClass::set_HasReturnType() /* 167772184 */
		IL_009b: box [netstandard]System.Int32 /* 16777235 */
		IL_00a0: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_00a5: nop
		IL_00a6: dup
		IL_00a7: ldloc.0
		IL_00a8: ldstr "" /* 1879048193 */
		IL_00ad: callvirt instance int32 [UnknownAssembly]UnknownNamespace.UnknownClass::set_HasReturnType(string) /* 167772185 */
		IL_00b2: box [netstandard]System.Int32 /* 16777235 */
		IL_00b7: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_00bc: nop
		IL_00bd: stloc.3
		IL_00be: ldloc.0
		IL_00bf: callvirt instance void [UnknownAssembly]UnknownNamespace.UnknownClass::get_NoReturnType() /* 167772186 */
		IL_00c4: nop
		IL_00c5: ldloc.0
		IL_00c6: callvirt instance void [UnknownAssembly]UnknownNamespace.UnknownClass::set_NoValue() /* 167772187 */
		IL_00cb: nop
		IL_00cc: ldloc.0
		IL_00cd: ldarg.0
		IL_00ce: ldftn instance void ClassLibrary1.UnknownClassTest::Instance_OnEvent(object, class [netstandard]System.EventArgs) /* 100663301 */
		IL_00d4: newobj instance void [netstandard]System.EventHandler::.ctor(object, native int) /* 167772188 */
		IL_00d9: callvirt instance void [UnknownAssembly]UnknownNamespace.UnknownClass::add_OnEvent(class [netstandard]System.EventHandler) /* 167772189 */
		IL_00de: nop
		IL_00df: ldloc.0
		IL_00e0: ldarg.0
		IL_00e1: ldftn instance void ClassLibrary1.UnknownClassTest::Instance_OnEvent(object, class [netstandard]System.EventArgs) /* 100663301 */
		IL_00e7: newobj instance void [netstandard]System.EventHandler::.ctor(object, native int) /* 167772188 */
		IL_00ec: callvirt instance void [UnknownAssembly]UnknownNamespace.UnknownClass::remove_OnEvent(class [netstandard]System.EventHandler) /* 167772190 */
		IL_00f1: nop
		IL_00f2: ldloc.0
		IL_00f3: ldloca.s 7
		IL_00f5: initobj valuetype [netstandard]System.Nullable`1<int64> /* 452984835 */
		IL_00fb: ldloc.s 7
		IL_00fd: callvirt instance string [UnknownAssembly]UnknownNamespace.UnknownClass::get_Item(valuetype [netstandard]System.Nullable`1<int64>) /* 167772191 */
		IL_0102: stloc.s 4
		IL_0104: ldloc.0
		IL_0105: ldc.i8 9223372036854775807
		IL_010e: newobj instance void valuetype [netstandard]System.Nullable`1<int64>::.ctor(!0) /* 167772192 */
		IL_0113: ldloc.s 4
		IL_0115: callvirt instance void [UnknownAssembly]UnknownNamespace.UnknownClass::set_Item(valuetype [netstandard]System.Nullable`1<int64>, string) /* 167772193 */
		IL_011a: nop
		IL_011b: ldloc.0
		IL_011c: ldsfld native uint [netstandard]System.UIntPtr::Zero /* 167772194 */
		IL_0121: ldstr "Hello" /* 1879048195 */
		IL_0126: callvirt instance native int& [UnknownAssembly]UnknownNamespace.UnknownClass::get_Item(native uint, string) /* 167772195 */
		IL_012b: ldind.i
		IL_012c: stloc.s 5
		IL_012e: ldloc.0
		IL_012f: ldc.i4.s 32
		IL_0131: conv.i8
		IL_0132: call native uint [netstandard]System.UIntPtr::op_Explicit(uint64) /* 167772196 */
		IL_0137: ldstr "World" /* 1879048207 */
		IL_013c: callvirt instance native int& [UnknownAssembly]UnknownNamespace.UnknownClass::get_Item(native uint, string) /* 167772195 */
		IL_0141: ldloc.s 5
		IL_0143: stind.i
		IL_0144: ret
	} // end of method UnknownClassTest::MethodUnknownClass

	.method /* 100663298 */ public hidebysig 
		instance void MethodUnknownGenericClass () cil managed 
	{
		// Method begins at RVA 0x21a4
		// Header size: 12
		// Code size: 227 (0xe3)
		.maxstack 4
		.locals /* 285212674 */ init (
			[0] class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs> 'instance',
			[1] class [UnknownAssembly]UnknownNamespace.UnknownEventArgs valueOfProp,
			[2] class [netstandard]System.Collections.Generic.List`1<object> list,
			[3] class [UnknownAssembly]UnknownNamespace.UnknownEventArgs valueOfIndexer,
			[4] class [UnknownAssembly]UnknownNamespace.UnknownEventArgs valueOfMultiIndexer
		)

		IL_0000: nop
		IL_0001: newobj instance void class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::.ctor() /* 167772197 */
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: callvirt instance !0 class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::get_UnknownProperty() /* 167772198 */
		IL_000d: stloc.1
		IL_000e: ldloc.0
		IL_000f: ldloc.1
		IL_0010: callvirt instance void class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::set_UnknownProperty(!0) /* 167772199 */
		IL_0015: nop
		IL_0016: newobj instance void class [netstandard]System.Collections.Generic.List`1<object>::.ctor() /* 167772176 */
		IL_001b: dup
		IL_001c: ldloc.0
		IL_001d: ldloc.1
		IL_001e: callvirt instance int32 [netstandard]System.Object::GetHashCode() /* 167772200 */
		IL_0023: callvirt instance string class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::get_NotPropertyWithParameter(int32) /* 167772201 */
		IL_0028: dup
		IL_0029: brtrue.s IL_0031

		IL_002b: pop
		IL_002c: ldstr "" /* 1879048193 */

		IL_0031: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0036: nop
		IL_0037: dup
		IL_0038: ldloc.0
		IL_0039: callvirt instance string class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::get_NotProperty() /* 167772202 */
		IL_003e: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0043: nop
		IL_0044: dup
		IL_0045: ldloc.0
		IL_0046: ldc.i4.s 42
		IL_0048: callvirt instance string class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::get_NotPropertyWithGeneric<string>(int32) /* 721420291 */
		IL_004d: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0052: nop
		IL_0053: dup
		IL_0054: ldloc.0
		IL_0055: ldc.i4.s 42
		IL_0057: callvirt instance string class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::get_NotPropertyWithParameter(int32) /* 167772201 */
		IL_005c: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0061: nop
		IL_0062: dup
		IL_0063: ldloc.0
		IL_0064: ldc.i4 -2147483648
		IL_0069: callvirt instance string class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::get_NotPropertyWithParameterAndGeneric<object>(int32) /* 721420292 */
		IL_006e: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0073: nop
		IL_0074: dup
		IL_0075: ldloc.0
		IL_0076: callvirt instance string class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::get_get_PropertyCalledGet() /* 167772205 */
		IL_007b: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0080: nop
		IL_0081: stloc.2
		IL_0082: ldloc.0
		IL_0083: ldarg.0
		IL_0084: ldftn instance void ClassLibrary1.UnknownClassTest::Instance_OnEvent(object, class [UnknownAssembly]UnknownNamespace.UnknownEventArgs) /* 100663302 */
		IL_008a: newobj instance void class [netstandard]System.EventHandler`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::.ctor(object, native int) /* 167772206 */
		IL_008f: callvirt instance void class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::add_OnEvent(class [netstandard]System.EventHandler`1<!0>) /* 167772207 */
		IL_0094: nop
		IL_0095: ldloc.0
		IL_0096: ldarg.0
		IL_0097: ldftn instance void ClassLibrary1.UnknownClassTest::Instance_OnEvent(object, class [UnknownAssembly]UnknownNamespace.UnknownEventArgs) /* 100663302 */
		IL_009d: newobj instance void class [netstandard]System.EventHandler`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::.ctor(object, native int) /* 167772206 */
		IL_00a2: callvirt instance void class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::remove_OnEvent(class [netstandard]System.EventHandler`1<!0>) /* 167772208 */
		IL_00a7: nop
		IL_00a8: ldloc.0
		IL_00a9: ldnull
		IL_00aa: callvirt instance !0 class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::get_Item(!0) /* 167772209 */
		IL_00af: stloc.3
		IL_00b0: ldloc.0
		IL_00b1: newobj instance void [UnknownAssembly]UnknownNamespace.UnknownEventArgs::.ctor() /* 167772210 */
		IL_00b6: ldloc.3
		IL_00b7: callvirt instance void class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::set_Item(!0, !0) /* 167772211 */
		IL_00bc: nop
		IL_00bd: ldloc.0
		IL_00be: newobj instance void [UnknownAssembly]UnknownNamespace.UnknownEventArgs::.ctor() /* 167772210 */
		IL_00c3: newobj instance void [UnknownAssembly]UnknownNamespace.UnknownEventArgs::.ctor() /* 167772210 */
		IL_00c8: callvirt instance !0 class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::get_Item(!0, !0) /* 167772212 */
		IL_00cd: stloc.s 4
		IL_00cf: ldloc.0
		IL_00d0: newobj instance void [UnknownAssembly]UnknownNamespace.UnknownEventArgs::.ctor() /* 167772210 */
		IL_00d5: newobj instance void [UnknownAssembly]UnknownNamespace.UnknownEventArgs::.ctor() /* 167772210 */
		IL_00da: ldloc.s 4
		IL_00dc: callvirt instance void class [UnknownAssembly]UnknownNamespace.UnknownGenericClass`1<class [UnknownAssembly]UnknownNamespace.UnknownEventArgs>::set_Item(!0, !0, !0) /* 167772213 */
		IL_00e1: nop
		IL_00e2: ret
	} // end of method UnknownClassTest::MethodUnknownGenericClass

	.method /* 100663299 */ public hidebysig 
		instance void MethodUnknownStatic () cil managed 
	{
		// Method begins at RVA 0x2294
		// Header size: 12
		// Code size: 154 (0x9a)
		.maxstack 4
		.locals /* 285212675 */ init (
			[0] valuetype [netstandard]System.Nullable`1<int32> valueOfProp,
			[1] class [netstandard]System.Collections.Generic.List`1<object> list
		)

		IL_0000: nop
		IL_0001: call valuetype [netstandard]System.Nullable`1<int32> [UnknownAssembly]UnknownNamespace.UnknownStaticClass::get_UnknownProperty() /* 167772214 */
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: call void [UnknownAssembly]UnknownNamespace.UnknownStaticClass::set_UnknownProperty(valuetype [netstandard]System.Nullable`1<int32>) /* 167772215 */
		IL_000d: nop
		IL_000e: newobj instance void class [netstandard]System.Collections.Generic.List`1<object>::.ctor() /* 167772176 */
		IL_0013: dup
		IL_0014: ldloca.s 0
		IL_0016: call instance !0 valuetype [netstandard]System.Nullable`1<int32>::get_Value() /* 167772177 */
		IL_001b: call string [UnknownAssembly]UnknownNamespace.UnknownStaticClass::get_NotPropertyWithParameter(int32) /* 167772216 */
		IL_0020: dup
		IL_0021: brtrue.s IL_0029

		IL_0023: pop
		IL_0024: ldstr "" /* 1879048193 */

		IL_0029: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_002e: nop
		IL_002f: dup
		IL_0030: call string [UnknownAssembly]UnknownNamespace.UnknownStaticClass::get_NotProperty() /* 167772217 */
		IL_0035: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_003a: nop
		IL_003b: dup
		IL_003c: ldc.i4.s 42
		IL_003e: call string [UnknownAssembly]UnknownNamespace.UnknownStaticClass::get_NotPropertyWithGeneric<string>(int32) /* 721420293 */
		IL_0043: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0048: nop
		IL_0049: dup
		IL_004a: ldc.i4.s 42
		IL_004c: call string [UnknownAssembly]UnknownNamespace.UnknownStaticClass::get_NotPropertyWithParameter(int32) /* 167772216 */
		IL_0051: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0056: nop
		IL_0057: dup
		IL_0058: ldc.i4 -2147483648
		IL_005d: call string [UnknownAssembly]UnknownNamespace.UnknownStaticClass::get_NotPropertyWithParameterAndGeneric<object>(int32) /* 721420294 */
		IL_0062: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0067: nop
		IL_0068: dup
		IL_0069: call string [UnknownAssembly]UnknownNamespace.UnknownStaticClass::get_get_PropertyCalledGet() /* 167772220 */
		IL_006e: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0073: nop
		IL_0074: stloc.1
		IL_0075: ldarg.0
		IL_0076: ldftn instance void ClassLibrary1.UnknownClassTest::Instance_OnEvent(object, class [netstandard]System.EventArgs) /* 100663301 */
		IL_007c: newobj instance void [netstandard]System.EventHandler::.ctor(object, native int) /* 167772188 */
		IL_0081: call void [UnknownAssembly]UnknownNamespace.UnknownStaticClass::add_OnEvent(class [netstandard]System.EventHandler) /* 167772221 */
		IL_0086: nop
		IL_0087: ldarg.0
		IL_0088: ldftn instance void ClassLibrary1.UnknownClassTest::Instance_OnEvent(object, class [netstandard]System.EventArgs) /* 100663301 */
		IL_008e: newobj instance void [netstandard]System.EventHandler::.ctor(object, native int) /* 167772188 */
		IL_0093: call void [UnknownAssembly]UnknownNamespace.UnknownStaticClass::remove_OnEvent(class [netstandard]System.EventHandler) /* 167772222 */
		IL_0098: nop
		IL_0099: ret
	} // end of method UnknownClassTest::MethodUnknownStatic

	.method /* 100663300 */ public hidebysig 
		instance void MethodUnknownStaticGeneric () cil managed 
	{
		// Method begins at RVA 0x233c
		// Header size: 12
		// Code size: 153 (0x99)
		.maxstack 4
		.locals /* 285212676 */ init (
			[0] string valueOfProp,
			[1] class [netstandard]System.Collections.Generic.List`1<object> list
		)

		IL_0000: nop
		IL_0001: call !0 class [UnknownAssembly]UnknownNamespace.UnknownStaticGenericClass`1<string>::get_UnknownProperty() /* 167772223 */
		IL_0006: stloc.0
		IL_0007: ldloc.0
		IL_0008: call void class [UnknownAssembly]UnknownNamespace.UnknownStaticGenericClass`1<string>::set_UnknownProperty(!0) /* 167772224 */
		IL_000d: nop
		IL_000e: newobj instance void class [netstandard]System.Collections.Generic.List`1<object>::.ctor() /* 167772176 */
		IL_0013: dup
		IL_0014: ldloc.0
		IL_0015: callvirt instance int32 [netstandard]System.String::get_Length() /* 167772225 */
		IL_001a: call string class [UnknownAssembly]UnknownNamespace.UnknownStaticGenericClass`1<string>::get_NotPropertyWithParameter(int32) /* 167772226 */
		IL_001f: dup
		IL_0020: brtrue.s IL_0028

		IL_0022: pop
		IL_0023: ldstr "" /* 1879048193 */

		IL_0028: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_002d: nop
		IL_002e: dup
		IL_002f: call string class [UnknownAssembly]UnknownNamespace.UnknownStaticGenericClass`1<string>::get_NotProperty() /* 167772227 */
		IL_0034: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0039: nop
		IL_003a: dup
		IL_003b: ldc.i4.s 42
		IL_003d: call string class [UnknownAssembly]UnknownNamespace.UnknownStaticGenericClass`1<string>::get_NotPropertyWithGeneric<string>(int32) /* 721420295 */
		IL_0042: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0047: nop
		IL_0048: dup
		IL_0049: ldc.i4.s 42
		IL_004b: call string class [UnknownAssembly]UnknownNamespace.UnknownStaticGenericClass`1<string>::get_NotPropertyWithParameter(int32) /* 167772226 */
		IL_0050: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0055: nop
		IL_0056: dup
		IL_0057: ldc.i4 -2147483648
		IL_005c: call string class [UnknownAssembly]UnknownNamespace.UnknownStaticGenericClass`1<string>::get_NotPropertyWithParameterAndGeneric<object>(int32) /* 721420296 */
		IL_0061: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0066: nop
		IL_0067: dup
		IL_0068: call string class [UnknownAssembly]UnknownNamespace.UnknownStaticGenericClass`1<string>::get_get_PropertyCalledGet() /* 167772230 */
		IL_006d: callvirt instance void class [netstandard]System.Collections.Generic.List`1<object>::Add(!0) /* 167772179 */
		IL_0072: nop
		IL_0073: stloc.1
		IL_0074: ldarg.0
		IL_0075: ldftn instance void ClassLibrary1.UnknownClassTest::Instance_OnEvent(object, string) /* 100663303 */
		IL_007b: newobj instance void class [netstandard]System.EventHandler`1<string>::.ctor(object, native int) /* 167772231 */
		IL_0080: call void class [UnknownAssembly]UnknownNamespace.UnknownStaticGenericClass`1<string>::add_OnEvent(class [netstandard]System.EventHandler`1<!0>) /* 167772232 */
		IL_0085: nop
		IL_0086: ldarg.0
		IL_0087: ldftn instance void ClassLibrary1.UnknownClassTest::Instance_OnEvent(object, string) /* 100663303 */
		IL_008d: newobj instance void class [netstandard]System.EventHandler`1<string>::.ctor(object, native int) /* 167772231 */
		IL_0092: call void class [UnknownAssembly]UnknownNamespace.UnknownStaticGenericClass`1<string>::remove_OnEvent(class [netstandard]System.EventHandler`1<!0>) /* 167772233 */
		IL_0097: nop
		IL_0098: ret
	} // end of method UnknownClassTest::MethodUnknownStaticGeneric

	.method public hidebysig 
		instance void MethodUnknownIndexerInitializer () cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 32 (0x20)
		.maxstack 8

		IL_0000: nop
		IL_0001: newobj instance void [UnknownAssembly]UnknownNamespace.UnknownClass::.ctor()
		IL_0006: dup
		IL_0007: ldstr "a"
		IL_000c: ldc.i4.1
		IL_000d: callvirt instance void [UnknownAssembly]UnknownNamespace.UnknownClass::set_Item(string, int32)
		IL_0012: nop
		IL_0013: ldstr "b"
		IL_0018: ldc.i4.2
		IL_0019: callvirt instance void [UnknownAssembly]UnknownNamespace.UnknownClass::set_Item(string, int32)
		IL_001e: nop
		IL_001f: ret
	} // end of method C::MethodUnknownIndexerInitializer


	.method /* 100663301 */ private hidebysig 
		instance void Instance_OnEvent (
			object sender,
			class [netstandard]System.EventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x23e1
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: nop
		IL_0001: newobj instance void [netstandard]System.NotImplementedException::.ctor() /* 167772234 */
		IL_0006: throw
	} // end of method UnknownClassTest::Instance_OnEvent

	.method /* 100663302 */ private hidebysig 
		instance void Instance_OnEvent (
			object sender,
			class [UnknownAssembly]UnknownNamespace.UnknownEventArgs e
		) cil managed 
	{
		// Method begins at RVA 0x23e9
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: nop
		IL_0001: newobj instance void [netstandard]System.NotImplementedException::.ctor() /* 167772234 */
		IL_0006: throw
	} // end of method UnknownClassTest::Instance_OnEvent

	.method /* 100663303 */ private hidebysig 
		instance void Instance_OnEvent (
			object sender,
			string e
		) cil managed 
	{
		// Method begins at RVA 0x23f1
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: nop
		IL_0001: newobj instance void [netstandard]System.NotImplementedException::.ctor() /* 167772234 */
		IL_0006: throw
	} // end of method UnknownClassTest::Instance_OnEvent

	.method /* 100663304 */ private hidebysig static 
		void Instance_OnEvent (
			object sender,
			object e
		) cil managed 
	{
		// Method begins at RVA 0x23f9
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: nop
		IL_0001: newobj instance void [netstandard]System.NotImplementedException::.ctor() /* 167772234 */
		IL_0006: throw
	} // end of method UnknownClassTest::Instance_OnEvent

	.method /* 100663305 */ public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2401
		// Header size: 1
		// Code size: 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [netstandard]System.EventArgs::.ctor() /* 167772235 */
		IL_0006: nop
		IL_0007: ret
	} // end of method UnknownClassTest::.ctor

} // end of class ClassLibrary1.UnknownClassTest
