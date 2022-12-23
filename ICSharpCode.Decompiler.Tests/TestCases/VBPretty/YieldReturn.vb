Imports System
Imports System.Collections.Generic

Namespace ICSharpCode.Decompiler.Tests.TestCases.VBPretty
	Friend Structure StructWithYieldReturn
		Private val As Integer

		Public Iterator Function Count() As IEnumerable(Of Integer)
			Yield val
			Yield val
		End Function
	End Structure

	Public Class YieldReturnPrettyTest
		Private fieldOnThis As Integer

		Public Shared ReadOnly Iterator Property YieldChars As IEnumerable(Of Char)
			Get
				Yield "a"c
				Yield "b"c
				Yield "c"c
			End Get
		End Property

		Friend Shared Sub Print(Of T)(ByVal name As String, ByVal enumerator As IEnumerator(Of T))
			Console.WriteLine(name + ": Test start")
			While enumerator.MoveNext()
				Console.WriteLine(name + ": " + enumerator.Current.ToString())
			End While
		End Sub

		Public Shared Iterator Function SimpleYieldReturn() As IEnumerable(Of String)
			Yield "A"
			Yield "B"
			Yield "C"
		End Function

		Public Shared Iterator Function SimpleYieldReturnEnumerator() As IEnumerator(Of String)
			Yield "A"
			Yield "B"
			Yield "C"
		End Function

		Public Iterator Function YieldReturnParameters(ByVal p As Integer) As IEnumerable(Of Integer)
			Yield p
			Yield fieldOnThis
		End Function

		Public Iterator Function YieldReturnParametersEnumerator(ByVal p As Integer) As IEnumerator(Of Integer)
			Yield p
			Yield fieldOnThis
		End Function

		Public Shared Iterator Function YieldReturnInLoop() As IEnumerable(Of Integer)
			For i = 0 To 99
				Yield i
			Next
		End Function

		Public Shared Iterator Function YieldReturnWithTryFinally() As IEnumerable(Of Integer)
			Yield 0
			Try
				Yield 1
			Finally
				Console.WriteLine("Finally!")
			End Try
			Yield 2
		End Function

		Public Shared Iterator Function YieldReturnWithNestedTryFinally(ByVal breakInMiddle As Boolean) As IEnumerable(Of String)
	        Console.WriteLine("Start of method - 1")
	        Yield "Start of method"
	        Console.WriteLine("Start of method - 2")
	        Try
	            Console.WriteLine("Within outer try - 1")
	            Yield "Within outer try"
	            Console.WriteLine("Within outer try - 2")
	            Try
	                Console.WriteLine("Within inner try - 1")
	                Yield "Within inner try"
	                Console.WriteLine("Within inner try - 2")
	                If breakInMiddle Then
	                    Console.WriteLine("Breaking...")
	                    Return
	                End If
	                Console.WriteLine("End of inner try - 1")
	                Yield "End of inner try"
	                Console.WriteLine("End of inner try - 2")
	            Finally
	                Console.WriteLine("Inner Finally")
	            End Try
	            Console.WriteLine("End of outer try - 1")
	            Yield "End of outer try"
	            Console.WriteLine("End of outer try - 2")
	        Finally
	            Console.WriteLine("Outer Finally")
	        End Try
	        Console.WriteLine("End of method - 1")
	        Yield "End of method"
	        Console.WriteLine("End of method - 2")
	    End Function

	    Public Shared Iterator Function YieldReturnWithTwoNonNestedFinallyBlocks(ByVal input As IEnumerable(Of String)) As IEnumerable(Of String)
	        ' outer try-finally block
	        For Each line In input
	            ' nested try-finally block
	            Try
	                Yield line
	            Finally
	                Console.WriteLine("Processed " & line)
	            End Try
	        Next
	        Yield "A"
	        Yield "B"
	        Yield "C"
	        Yield "D"
	        Yield "E"
	        Yield "F"
	        ' outer try-finally block
	        For Each item In input
	            Yield item.ToUpper()
	        Next
	    End Function

	    Public Shared Iterator Function GetEvenNumbers(ByVal n As Integer) As IEnumerable(Of Integer)
	        For i = 0 To n - 1
	            If i Mod 2 = 0 Then
	                Yield i
	            End If
	        Next
	    End Function

	    Public Shared Iterator Function ExceptionHandling() As IEnumerable(Of Char)
	        Yield "a"c
	        Try
	            Console.WriteLine("1 - try")
	        Catch __unusedException1__ As Exception
	            Console.WriteLine("1 - catch")
	        End Try
	        Yield "b"c
	        Try
	            Try
	                Console.WriteLine("2 - try")
	            Finally
	                Console.WriteLine("2 - finally")
	            End Try
	            Yield "c"c
	        Finally
	            Console.WriteLine("outer finally")
	        End Try
	    End Function

	    Public Shared Iterator Function YieldBreakInCatch() As IEnumerable(Of Integer)
	        Yield 0
	        Try
	            Console.WriteLine("In Try")
	        Catch
	            ' yield return is not allowed in catch, but yield break is
	            Return
	        End Try
	        Yield 1
	    End Function

	    Public Shared Iterator Function YieldBreakInCatchInTryFinally() As IEnumerable(Of Integer)
	        Try
	            Yield 0
	            Try
	                Console.WriteLine("In Try")
	            Catch
	                ' yield return is not allowed in catch, but yield break is
	                ' Note that pre-roslyn, this code triggers a compiler bug:
	                ' If the finally block throws an exception, it ends up getting
	                ' called a second time.
	                Return
	            End Try
	            Yield 1
	        Finally
	            Console.WriteLine("Finally")
	        End Try
	    End Function

	    Public Shared Iterator Function YieldBreakInTryCatchInTryFinally() As IEnumerable(Of Integer)
	        Try
	            Yield 0
	            Try
	                Console.WriteLine("In Try")
	                ' same compiler bug as in YieldBreakInCatchInTryFinally
	                Return
	            Catch
	                Console.WriteLine("Catch")
	            End Try
	            Yield 1
	        Finally
	            Console.WriteLine("Finally")
	        End Try
	    End Function

	    Public Shared Iterator Function YieldBreakInTryFinallyInTryFinally(ByVal b As Boolean) As IEnumerable(Of Integer)
	        Try
	            Yield 0
	            Try
	                Console.WriteLine("In Try")
	                If b Then
	                    ' same compiler bug as in YieldBreakInCatchInTryFinally
	                    Return
	                End If

	            Finally
	                Console.WriteLine("Inner Finally")
	            End Try
	            Yield 1
	        Finally
	            Console.WriteLine("Finally")
	        End Try
	    End Function

	    Public Shared Iterator Function YieldBreakOnly() As IEnumerable(Of Integer)
	        Return
	    End Function

	    Public Shared Iterator Function UnconditionalThrowInTryFinally() As IEnumerable(Of Integer)
	        ' Here, MoveNext() doesn't call the finally methods at all
	        ' (only indirectly via Dispose())
	        Try
	            Yield 0
	            Throw New NotImplementedException()
	        Finally
	            Console.WriteLine("Finally")
	        End Try
	    End Function

	    Public Shared Iterator Function NestedTryFinallyStartingOnSamePosition() As IEnumerable(Of Integer)
	        ' The first user IL instruction is already in 2 nested try blocks.
	        Try
	            Try
	                Yield 0
	            Finally
	                Console.WriteLine("Inner Finally")
	            End Try

	        Finally
	            Console.WriteLine("Outer Finally")
	        End Try
	    End Function

#If ROSLYN Then
	    Public Shared Iterator Function LocalInFinally(Of T As IDisposable)(ByVal a As T) As IEnumerable(Of Integer)
	        Yield 1
	        Try
	            Yield 2
	        Finally
	            Dim val = a
	            val.Dispose()
	            val.Dispose()
	        End Try
	        Yield 3
	    End Function
#End If

	    Public Shared Iterator Function GenericYield(Of T As New)() As IEnumerable(Of T)
	        Dim val As T = New T()
	        For i = 0 To 2
	            Yield val
	        Next
	    End Function

	    Public Shared Iterator Function MultipleYieldBreakInTryFinally(ByVal i As Integer) As IEnumerable(Of Integer)
	        Try
	            If i = 2 Then
	                Return
	            End If

	            While i < 40
	                If i Mod 2 = 0 Then
	                    Return
	                End If
	                i += 1

	                Yield i
	            End While

	        Finally
	            Console.WriteLine("finally")
	        End Try
	        Console.WriteLine("normal exit")
	    End Function

		Friend Iterator Function ForLoopWithYieldReturn(ByVal [end] As Integer, ByVal evil As Integer) As IEnumerable(Of Integer)
			' This loop needs to pick the implicit "yield break;" as exit point
			' in order to produce pretty code; not the "throw" which would
			' be a less-pretty option.
			For i = 0 To [end] - 1
				If i = evil Then
					Throw New InvalidOperationException("Found evil number")
				End If
				Yield i
			Next
		End Function
	End Class
End Namespace
