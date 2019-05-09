Imports System

Module UninitTest
    Sub Main()
        Dim i = 0
        Dim result As Integer = -5
        Dim num As Integer
        Do
            If num > result Then
                num += 5
                result += 5
            End If
            result += 1
            i += 1
            If i > 10 Then
                Exit Do
            End If
        Loop
        Console.WriteLine(result)
    End Sub
End Module