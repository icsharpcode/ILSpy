Imports System

Module Program
	Sub SelectOnString()
		Select Case Environment.CommandLine
			Case "123"
				Console.WriteLine("a")
			Case "444"
				Console.WriteLine("b")
			Case "222"
				Console.WriteLine("c")
			Case "11"
				Console.WriteLine("d")
			Case "dd"
				Console.WriteLine("e")
			Case "sss"
				Console.WriteLine("f")
			Case "aa"
				Console.WriteLine("g")
			Case ""
				Console.WriteLine("empty")
		End Select
	End Sub
End Module
