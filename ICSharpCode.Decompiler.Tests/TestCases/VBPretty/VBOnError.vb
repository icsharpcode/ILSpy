Imports System
Imports Microsoft.VisualBasic

Public Class VBOnError
	Public Shared Sub ResumeNext()
		On Error Resume Next
		Console.WriteLine("A")
		Console.WriteLine("B")
	End Sub

	Public Shared Function ResumeNextWithResult() As Integer
		On Error Resume Next
		Dim x As Integer = 1
		x = x \ 0
		Return x
	End Function

	Public Shared Sub ResumeNextErrNumber()
		On Error Resume Next
		Console.WriteLine("A")
		If Err.Number <> 0 Then
			Console.WriteLine(Err.Description)
			Err.Clear()
		End If
	End Sub

	Public Shared Sub GoToHandler()
		On Error GoTo Handler
		Console.WriteLine("Body")
		Exit Sub
Handler:
		Console.WriteLine(Err.Description)
	End Sub

	Public Shared Sub GoToHandlerResumeNext()
		On Error GoTo Handler
		Console.WriteLine("A")
		Console.WriteLine("B")
		Exit Sub
Handler:
		Console.WriteLine(Err.Number)
		Resume Next
	End Sub

	Public Shared Sub GoToHandlerResume(retries As Integer)
		On Error GoTo Handler
		Console.WriteLine("Body")
		Exit Sub
Handler:
		retries -= 1
		If retries > 0 Then
			Resume
		End If
	End Sub

	Public Shared Sub GoToHandlerResumeLabel()
		On Error GoTo Handler
		Console.WriteLine("Body")
Done:
		Console.WriteLine("Done")
		Exit Sub
Handler:
		Console.WriteLine(Err.Description)
		Resume Done
	End Sub

	Public Shared Sub GoToZero()
		On Error Resume Next
		Console.WriteLine("A")
		On Error GoTo 0
		Console.WriteLine("B")
	End Sub

	Public Shared Sub GoToMinusOne()
		On Error GoTo Handler
		Console.WriteLine("A")
		Exit Sub
Handler:
		On Error GoTo -1
		On Error GoTo Handler2
		Console.WriteLine("B")
		Exit Sub
Handler2:
		Console.WriteLine("C")
	End Sub

	Public Shared Sub SwitchHandlers()
		On Error GoTo Handler1
		Console.WriteLine("A")
		On Error GoTo Handler2
		Console.WriteLine("B")
		Exit Sub
Handler1:
		Console.WriteLine("Handler1")
		Resume Next
Handler2:
		Console.WriteLine("Handler2")
		Resume Next
	End Sub

	Public Shared Function GoToHandlerWithResult() As Integer
		On Error GoTo Handler
		Return Integer.Parse("x")
Handler:
		Return -1
	End Function
End Class
