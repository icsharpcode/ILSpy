Imports System
Public Class Issue3659
	Public Shared Sub Func(ByRef obj As Issue3659, value As Object)
	End Sub

	Public Shared Function ShowMessage(a As String, b As Integer, c As String, d As Object, e As Integer) As Integer
		Return 0
	End Function

	Friend Sub VBFunction(value As Object)
		On Error GoTo Handler
		Func(Me, value)
		Exit Sub
Handler:
		ShowMessage("VBFunction", 0, "Exception", Nothing, 0)
	End Sub
End Class
