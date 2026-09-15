Imports System
Imports System.IO

Public Class VBTryCatchFinally
	Private Shared Function Condition() As Boolean
		Return True
	End Function

	Public Shared Sub TryFinally()
		Try
			Console.WriteLine("Try")
		Finally
			Console.WriteLine("Finally")
		End Try
	End Sub

	Public Shared Sub TryCatchBare()
		Try
			Console.WriteLine("Try")
		Catch
			Console.WriteLine("Catch")
		End Try
	End Sub

	Public Shared Sub TryCatchVariable()
		Try
			Console.WriteLine("Try")
		Catch ex As Exception
			Console.WriteLine(ex.Message)
		End Try
	End Sub

	Public Shared Sub TryCatchSpecificType()
		Try
			Console.WriteLine("Try")
		Catch ex As IOException
			Console.WriteLine(ex.Message)
		End Try
	End Sub

	Public Shared Sub TryCatchUnusedVariable()
		Try
			Console.WriteLine("Try")
		Catch ex As InvalidOperationException
			Console.WriteLine("Catch")
		End Try
	End Sub

	Public Shared Sub TryCatchWhen()
		Try
			Console.WriteLine("Try")
		Catch When Condition()
			Console.WriteLine("Catch")
		End Try
	End Sub

	Public Shared Sub TryCatchVariableWhen()
		Try
			Console.WriteLine("Try")
		Catch ex As Exception When ex.Message IsNot Nothing
			Console.WriteLine(ex.Message)
		End Try
	End Sub

	Public Shared Sub TryCatchSpecificTypeWhen()
		Try
			Console.WriteLine("Try")
		Catch ex As IOException When Condition()
			Console.WriteLine(ex.Message)
		End Try
	End Sub

	Public Shared Sub TryCatchExistingLocal()
		Dim ex As Exception = Nothing
		Try
			Console.WriteLine("Try")
		Catch ex
			Console.WriteLine("Catch")
		End Try
		Console.WriteLine(ex)
	End Sub

	Public Shared Sub TryCatchExistingLocalWhen()
		Dim ex As Exception = Nothing
		Try
			Console.WriteLine("Try")
		Catch ex When ex.Message IsNot Nothing
			Console.WriteLine("Catch")
		End Try
		Console.WriteLine(ex)
	End Sub

	Public Shared Sub TryCatchFinally()
		Try
			Console.WriteLine("Try")
		Catch ex As Exception
			Console.WriteLine(ex.Message)
		Finally
			Console.WriteLine("Finally")
		End Try
	End Sub

	Public Shared Sub TryCatchBareFinally()
		Try
			Console.WriteLine("Try")
		Catch
			Console.WriteLine("Catch")
		Finally
			Console.WriteLine("Finally")
		End Try
	End Sub

	Public Shared Sub TryCatchWhenFinally()
		Try
			Console.WriteLine("Try")
		Catch ex As Exception When Condition()
			Console.WriteLine(ex.Message)
		Finally
			Console.WriteLine("Finally")
		End Try
	End Sub

	Public Shared Sub TryMultipleCatch()
		Try
			Console.WriteLine("Try")
		Catch ex As FileNotFoundException
			Console.WriteLine(ex.FileName)
		Catch ex As IOException
			Console.WriteLine(ex.Message)
		Catch
			Console.WriteLine("Catch")
		End Try
	End Sub

	Public Shared Sub TryMultipleCatchFinally()
		Try
			Console.WriteLine("Try")
		Catch ex As FileNotFoundException
			Console.WriteLine(ex.FileName)
		Catch ex As IOException
			Console.WriteLine(ex.Message)
		Finally
			Console.WriteLine("Finally")
		End Try
	End Sub

	Public Shared Sub TryMultipleCatchWhen()
		Try
			Console.WriteLine("Try")
		Catch ex As IOException When Condition()
			Console.WriteLine(ex.Message)
		Catch When Condition()
			Console.WriteLine("Catch When")
		Catch ex As Exception
			Console.WriteLine(ex.Message)
		End Try
	End Sub

	Public Shared Sub EmptyTryFinally()
		Try
		Finally
			Console.WriteLine("Finally")
		End Try
	End Sub

	Public Shared Sub EmptyCatch()
		Try
			Console.WriteLine("Try")
		Catch
		End Try
	End Sub

	Public Shared Sub EmptyFinally()
		Try
			Console.WriteLine("Try")
		Catch ex As Exception
			Console.WriteLine(ex.Message)
		Finally
		End Try
	End Sub

	Public Shared Sub ExitTryInTry(b As Boolean)
		Try
			If b Then
				Exit Try
			End If
			Console.WriteLine("Try")
		Catch
			Console.WriteLine("Catch")
		End Try
		Console.WriteLine("End")
	End Sub

	Public Shared Sub ExitTryInCatch(b As Boolean)
		Try
			Console.WriteLine("Try")
		Catch
			If b Then
				Exit Try
			End If
			Console.WriteLine("Catch")
		End Try
		Console.WriteLine("End")
	End Sub

	Public Shared Sub ExitTryWithFinally(b As Boolean)
		Try
			If b Then
				Exit Try
			End If
			Console.WriteLine("Try")
		Finally
			Console.WriteLine("Finally")
		End Try
		Console.WriteLine("End")
	End Sub

	Public Shared Sub Rethrow()
		Try
			Console.WriteLine("Try")
		Catch ex As Exception
			Console.WriteLine(ex.Message)
			Throw
		End Try
	End Sub

	Public Shared Sub ThrowNew()
		Try
			Console.WriteLine("Try")
		Catch ex As Exception
			Throw New InvalidOperationException("Catch", ex)
		End Try
	End Sub

	Public Shared Function ReturnFromTry() As Integer
		Try
			Return 1
		Catch
			Return 2
		Finally
			Console.WriteLine("Finally")
		End Try
	End Function

	Public Shared Function ReturnAfterTry() As Integer
		Try
			Console.WriteLine("Try")
		Catch ex As Exception
			Console.WriteLine(ex.Message)
			Return -1
		End Try
		Return 0
	End Function

	Public Shared Sub NestedTryInTry()
		Try
			Try
				Console.WriteLine("Inner Try")
			Catch ex As IOException
				Console.WriteLine(ex.Message)
			End Try
		Catch ex As Exception
			Console.WriteLine(ex.Message)
		End Try
	End Sub

	Public Shared Sub NestedTryInCatch()
		Try
			Console.WriteLine("Try")
		Catch ex As Exception
			Try
				Console.WriteLine(ex.Message)
			Catch ex2 As Exception
				Console.WriteLine(ex2.Message)
			End Try
		End Try
	End Sub

	Public Shared Sub NestedTryInFinally()
		Try
			Console.WriteLine("Try")
		Finally
			Try
				Console.WriteLine("Inner Try")
			Catch
				Console.WriteLine("Inner Catch")
			End Try
		End Try
	End Sub

	Public Shared Sub TryInLoop()
		For i = 0 To 9
			Try
				If i = 5 Then
					Continue For
				End If
				If i = 8 Then
					Exit For
				End If
				Console.WriteLine(i)
			Catch
				Console.WriteLine("Catch")
			Finally
				Console.WriteLine("Finally")
			End Try
		Next
	End Sub
End Class
