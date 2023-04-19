Imports System
Imports System.Collections

Public Class VBNonGenericForEach
	Public Shared Sub M()
		Dim collection = New ArrayList
		For Each element In collection
			Console.WriteLine(element)
		Next
	End Sub
End Class
