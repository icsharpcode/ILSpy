Imports System
Imports System.IO

Module Program
    Sub Main(args As String())

    End Sub

    Sub TestCode(ByVal t As Test, ByVal i As Integer)
        Dim s As String = ""
        s += File.ReadAllText("Test.txt")
        s += "asdf"
        t.Parameterized(i) = s
        t.Unparameterized = s + "asdf"
    End Sub
End Module

Class Test
    Public Property Parameterized(ByVal i As Integer) As String
        Get
            Throw New NotImplementedException()
        End Get
        Set(value As String)
            Throw New NotImplementedException()
        End Set
    End Property

    Public Property Unparameterized As String
End Class