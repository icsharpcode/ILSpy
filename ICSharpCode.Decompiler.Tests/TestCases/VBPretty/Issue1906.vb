Imports System
Public Class Issue1906
	Public Sub M()
		Console.WriteLine(Math.Min(Math.Max(Int64.MinValue, New Long), Int64.MaxValue))
	End Sub
End Class