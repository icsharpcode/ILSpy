Imports System
Imports Microsoft.VisualBasic

Module VBOnErrorCorrectness
	Sub Main()
		ResumeNext()
		GoToHandler()
		GoToHandlerResumeNext()
		Console.WriteLine(GoToHandlerResume(3))
		GoToHandlerResumeLabel()
		Try
			GoToZero()
		Catch ex As InvalidOperationException
			Console.WriteLine("outer: " & ex.Message)
		End Try
		Console.WriteLine(GoToHandlerWithResult())
	End Sub

	Sub FailWhilePositive(retries As Integer)
		If retries > 0 Then Fail("retry " & retries)
	End Sub

	Sub Fail(message As String)
		Throw New InvalidOperationException(message)
	End Sub

	Sub ResumeNext()
		On Error Resume Next
		Fail("a")
		Console.WriteLine("after a: " & Err.Number & " " & Err.Description)
		Err.Clear()
		Console.WriteLine("cleared: " & Err.Number)
	End Sub

	Sub GoToHandler()
		On Error GoTo Handler
		Fail("b")
		Console.WriteLine("unreachable")
		Exit Sub
Handler:
		Console.WriteLine("handler: " & Err.Description)
	End Sub

	Sub GoToHandlerResumeNext()
		On Error GoTo Handler
		Fail("c1")
		Console.WriteLine("between")
		Fail("c2")
		Console.WriteLine("done")
		Exit Sub
Handler:
		Console.WriteLine("handler: " & Err.Description)
		Resume Next
	End Sub

	Function GoToHandlerResume(retries As Integer) As Integer
		On Error GoTo Handler
		FailWhilePositive(retries)
		Return retries
Handler:
		retries -= 1
		Resume
	End Function

	Sub GoToHandlerResumeLabel()
		On Error GoTo Handler
		Fail("d")
Done:
		Console.WriteLine("done")
		Exit Sub
Handler:
		Console.WriteLine("handler: " & Err.Description)
		Resume Done
	End Sub

	Sub GoToZero()
		On Error Resume Next
		Fail("e1")
		Console.WriteLine("swallowed")
		On Error GoTo 0
		Fail("e2")
		Console.WriteLine("unreachable")
	End Sub

	Function GoToHandlerWithResult() As Integer
		On Error GoTo Handler
		Return Integer.Parse("x")
Handler:
		Return -1
	End Function
End Module
