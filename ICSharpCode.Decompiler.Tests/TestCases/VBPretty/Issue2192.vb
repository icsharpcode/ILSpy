Imports System
Imports System.Linq
Public Class Issue2192
	Public Shared Sub M()
		Dim words As String() = {"abc", "defgh", "ijklm"}
		Dim word As String = "test"
		Console.WriteLine(words.Count(Function(w) w.Length > word.Length))
	End Sub
End Class