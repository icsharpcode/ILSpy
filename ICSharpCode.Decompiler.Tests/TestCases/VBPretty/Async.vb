Imports System
Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports System.Runtime.CompilerServices

Namespace ICSharpCode.Decompiler.Tests.TestCases.VBPretty
	Public Class Async
		Private memberField As Integer

		Private Shared Function [True]() As Boolean
	        Return True
	    End Function

	    Public Async Sub SimpleVoidMethod()
	        Console.WriteLine("Before")
	        Await Task.Delay(TimeSpan.FromSeconds(1.0))
	        Console.WriteLine("After")
	    End Sub

	    Public Async Sub VoidMethodWithoutAwait()
	        Console.WriteLine("No Await")
	    End Sub

	    Public Async Sub EmptyVoidMethod()
	    End Sub

	    Public Async Sub AwaitYield()
	        Await Task.Yield()
	    End Sub

	    Public Async Sub AwaitDefaultYieldAwaitable()
	        Await CType(Nothing, YieldAwaitable)
	    End Sub

		Public Async Sub AwaitDefaultHopToThreadPool()
			' unlike YieldAwaitable which implements ICriticalNotifyCompletion,
			' the HopToThreadPoolAwaitable struct only implements
			' INotifyCompletion, so this results in different codegen
			Await CType(Nothing, HopToThreadPoolAwaitable)
		End Sub

	    Public Async Function SimpleVoidTaskMethod() As Task
	        Console.WriteLine("Before")
	        Await Task.Delay(TimeSpan.FromSeconds(1.0))
	        Console.WriteLine("After")
	    End Function

	    Public Async Function TaskMethodWithoutAwait() As Task
	        Console.WriteLine("No Await")
	    End Function

	    Public Async Function CapturingThis() As Task
	        Await Task.Delay(memberField)
	    End Function

	    Public Async Function CapturingThisWithoutAwait() As Task
	        Console.WriteLine(memberField)
	    End Function

	    Public Async Function SimpleBoolTaskMethod() As Task(Of Boolean)
	        Console.WriteLine("Before")
	        Await Task.Delay(TimeSpan.FromSeconds(1.0))
	        Console.WriteLine("After")
	        Return True
	    End Function

	    Public Async Sub TwoAwaitsWithDifferentAwaiterTypes()
	        Console.WriteLine("Before")
	        If Await SimpleBoolTaskMethod() Then
	            Await Task.Delay(TimeSpan.FromSeconds(1.0))
	        End If
	        Console.WriteLine("After")
	    End Sub

	    Public Async Sub AwaitInLoopCondition()
	        While Await SimpleBoolTaskMethod()
	            Console.WriteLine("Body")
	        End While
	    End Sub

		Public Async Function Issue2366a() As Task
			While True
				Try
					Await Task.CompletedTask
				Catch
				End Try
			End While
		End Function

	    Public Shared Async Function GetIntegerSumAsync(ByVal items As IEnumerable(Of Integer)) As Task(Of Integer)
	        Await Task.Delay(100)
	        Dim num = 0
	        For Each item In items
	            num += item
	        Next
	        Return num
	    End Function

		 Public Async Function AsyncCatch(ByVal b As Boolean, ByVal task1 As Task(Of Integer), ByVal task2 As Task(Of Integer)) As Task
			 Try
				 Console.WriteLine("Start try")
				 Await task1
				 Console.WriteLine("End try")
			 Catch ex As Exception
				 Console.WriteLine("No await")
			 End Try
		 End Function

		 Public Async Function AsyncCatchThrow(ByVal b As Boolean, ByVal task1 As Task(Of Integer), ByVal task2 As Task(Of Integer)) As Task
			 Try
				 Console.WriteLine("Start try")
				 Await task1
				 Console.WriteLine("End try")
			 Catch ex As Exception
				 Console.WriteLine("No await")
				 Throw
			 End Try
		 End Function

		 Public Async Function AsyncFinally(ByVal b As Boolean, ByVal task1 As Task(Of Integer), ByVal task2 As Task(Of Integer)) As Task
			 Try
				 Console.WriteLine("Start try")
				 Await task1
				 Console.WriteLine("End try")
			 Finally
				 Console.WriteLine("No await")
			 End Try
		 End Function

	    Public Shared Async Function AlwaysThrow() As Task
	    Throw New Exception
	    End Function

	    Public Shared Async Function InfiniteLoop() As Task
	        While True
	        End While
	    End Function

	    Public Shared Async Function InfiniteLoopWithAwait() As Task
	        While True
	            Await Task.Delay(10)
	        End While
	    End Function

		Public Async Function AsyncWithLocalVar() As Task
	        Dim a As Object = New Object()
	        Await UseObj(a)
	        Await UseObj(a)
	    End Function

	    Public Shared Async Function UseObj(ByVal a As Object) As Task
	    End Function
	End Class

	Public Structure AsyncInStruct
		Private i As Integer

		Public Async Function Test(ByVal xx As AsyncInStruct) As Task(Of Integer)
			xx.i += 1
			i += 1
			Await Task.Yield()
			Return i + xx.i
		End Function
	End Structure

	Public Structure HopToThreadPoolAwaitable
		Implements INotifyCompletion
		Public Property IsCompleted As Boolean

		Public Function GetAwaiter() As HopToThreadPoolAwaitable
			Return Me
		End Function

		Public Sub OnCompleted(ByVal continuation As Action) Implements INotifyCompletion.OnCompleted
			Task.Run(continuation)
		End Sub

		Public Sub GetResult()
		End Sub
	End Structure
End Namespace
