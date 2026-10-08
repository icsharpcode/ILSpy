Imports System

Public Class Issue2199
	Public Event Click As EventHandler
	Public Event DoubleClick As EventHandler

	Private Class Item
		Public Text As String
	End Class

	Public Sub Register(value As String)
		Dim item As New Item()
		item.Text = value.Trim()
		AddHandler Click, Sub() Console.WriteLine(item.Text)
		AddHandler DoubleClick, Sub() Console.WriteLine(item.Text)
	End Sub
End Class
