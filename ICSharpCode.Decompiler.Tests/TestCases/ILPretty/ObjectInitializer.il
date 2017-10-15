.assembly ILSpyBugs
{
	
	.hash algorithm 0x00008004 // SHA1
	.ver 1:0:0:0
}

.module ILSpyBugs.exe
// MVID: {BAB92FF9-8191-4D51-9A72-CD3EE83B18CD}
.corflags 0x00020003 // ILOnly, Required32Bit, Preferred32Bit


.class private auto ansi beforefieldinit ILSpyBugs.Program
	extends [mscorlib]System.Object
{

.method private hidebysig static 
	void Main (
		string[] args
	) cil managed 
{
	// Method begins at RVA 0x2050
	// Code size 8 (0x8)
	.maxstack 8
	.entrypoint
	IL_0000: nop
	IL_0007: ret
}

.method private hidebysig static 
	class [System.Net.Http]System.Net.Http.HttpResponseMessage Aaa(
		class [System.Drawing]System.Drawing.Image image,
		string fileName
	) cil managed 
{
	// Method begins at RVA 0x49c4
	// Code size 155 (0x9b)
	.maxstack 3
	.locals init (
		[0] class [mscorlib]System.IO.MemoryStream,
		[1] class [mscorlib]System.IO.MemoryStream
	)

	IL_0000: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor()
	IL_0005: stloc.0
	IL_0006: ldarg.0
	IL_0007: ldloc.0
	IL_0008: call class [System.Drawing]System.Drawing.Imaging.ImageFormat [System.Drawing]System.Drawing.Imaging.ImageFormat::get_Jpeg()
	IL_000d: callvirt instance void [System.Drawing]System.Drawing.Image::Save(class [mscorlib]System.IO.Stream, class [System.Drawing]System.Drawing.Imaging.ImageFormat)
	IL_0012: ldloc.0
	IL_0013: callvirt instance void [mscorlib]System.IO.Stream::Close()
	IL_0018: ldloc.0
	IL_0019: callvirt instance uint8[] [mscorlib]System.IO.MemoryStream::ToArray()
	IL_001e: newobj instance void [mscorlib]System.IO.MemoryStream::.ctor(uint8[])
	IL_0023: stloc.1
	IL_0024: newobj instance void [System.Net.Http]System.Net.Http.HttpResponseMessage::.ctor()
	IL_0029: dup
	IL_002a: ldloc.1
	IL_002b: newobj instance void [System.Net.Http]System.Net.Http.StreamContent::.ctor(class [mscorlib]System.IO.Stream)
	IL_0030: callvirt instance void [System.Net.Http]System.Net.Http.HttpResponseMessage::set_Content(class [System.Net.Http]System.Net.Http.HttpContent)
	IL_0035: dup
	IL_0036: callvirt instance class [System.Net.Http]System.Net.Http.HttpContent [System.Net.Http]System.Net.Http.HttpResponseMessage::get_Content()
	IL_003b: callvirt instance class [System.Net.Http]System.Net.Http.Headers.HttpContentHeaders [System.Net.Http]System.Net.Http.HttpContent::get_Headers()
	IL_0040: ldstr "aaa"
	IL_0045: newobj instance void [System.Net.Http]System.Net.Http.Headers.ContentDispositionHeaderValue::.ctor(string)
	IL_004a: callvirt instance void [System.Net.Http]System.Net.Http.Headers.HttpContentHeaders::set_ContentDisposition(class [System.Net.Http]System.Net.Http.Headers.ContentDispositionHeaderValue)
	IL_004f: dup
	IL_0050: callvirt instance class [System.Net.Http]System.Net.Http.HttpContent [System.Net.Http]System.Net.Http.HttpResponseMessage::get_Content()
	IL_0055: callvirt instance class [System.Net.Http]System.Net.Http.Headers.HttpContentHeaders [System.Net.Http]System.Net.Http.HttpContent::get_Headers()
	IL_005a: callvirt instance class [System.Net.Http]System.Net.Http.Headers.ContentDispositionHeaderValue [System.Net.Http]System.Net.Http.Headers.HttpContentHeaders::get_ContentDisposition()
	IL_005f: ldarg.1
	IL_0060: callvirt instance void [System.Net.Http]System.Net.Http.Headers.ContentDispositionHeaderValue::set_FileName(string)
	IL_0065: dup
	IL_0066: callvirt instance class [System.Net.Http]System.Net.Http.HttpContent [System.Net.Http]System.Net.Http.HttpResponseMessage::get_Content()
	IL_006b: callvirt instance class [System.Net.Http]System.Net.Http.Headers.HttpContentHeaders [System.Net.Http]System.Net.Http.HttpContent::get_Headers()
	IL_0070: ldstr "bbb"
	IL_0075: newobj instance void [System.Net.Http]System.Net.Http.Headers.MediaTypeHeaderValue::.ctor(string)
	IL_007a: callvirt instance void [System.Net.Http]System.Net.Http.Headers.HttpContentHeaders::set_ContentType(class [System.Net.Http]System.Net.Http.Headers.MediaTypeHeaderValue)
	IL_007f: dup
	IL_0080: callvirt instance class [System.Net.Http]System.Net.Http.HttpContent [System.Net.Http]System.Net.Http.HttpResponseMessage::get_Content()
	IL_0085: callvirt instance class [System.Net.Http]System.Net.Http.Headers.HttpContentHeaders [System.Net.Http]System.Net.Http.HttpContent::get_Headers()
	IL_008a: ldloc.1
	IL_008b: callvirt instance int64 [mscorlib]System.IO.Stream::get_Length()
	IL_0090: newobj instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
	IL_0095: callvirt instance void [System.Net.Http]System.Net.Http.Headers.HttpContentHeaders::set_ContentLength(valuetype [mscorlib]System.Nullable`1<int64>)
	IL_009a: ret
}


.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x20d5
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: nop
		IL_0007: ret
	}

}
