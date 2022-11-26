Imports System
Imports System.IO
Imports System.Threading.Tasks

Module AsyncProgram
    ' Sample taken verbatim from https://www.dotnetperls.com/async-vbnet
    Sub Main(args As String())
        Dim task = New Task(AddressOf ProcessDataAsync)
        ' Start and wait for task to end.
        task.Start()
        task.Wait()
        Console.ReadLine()
    End Sub

    Async Sub ProcessDataAsync()
        ' Create a task Of Integer.
        ' ... Use HandleFileAsync method with a large file.
        Dim task As Task(Of Integer) = HandleFileAsync("C:\enable1.txt")
        ' This statement runs while HandleFileAsync executes.
        Console.WriteLine("Please wait, processing")
        ' Use await to wait for task to complete.
        Dim result As Integer = Await task
        Console.WriteLine("Count: " + result.ToString())
    End Sub

    Async Function HandleFileAsync(ByVal file As String) As Task(Of Integer)
        Console.WriteLine("HandleFile enter")

        ' Open the file.
        Dim count As Integer = 0
        Using reader As StreamReader = New StreamReader(file)
            Dim value As String = Await reader.ReadToEndAsync()
            count += value.Length

            ' Do a slow computation on the file.
            For i = 0 To 10000 Step 1
                Dim x = value.GetHashCode()
                If x = 0 Then
                    count -= 1
                End If

            Next
        End Using

        Console.WriteLine("HandleFile exit")
        Return count
    End Function
End Module
