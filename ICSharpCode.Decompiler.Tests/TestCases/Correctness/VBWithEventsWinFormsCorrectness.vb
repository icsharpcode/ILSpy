Imports System

Namespace System.Windows.Forms
	Public Class Control
		Public Event Load As EventHandler

		Public Property Text As String

		Public Sub RaiseLoad()
			RaiseEvent Load(Me, EventArgs.Empty)
		End Sub
	End Class

	Public Class Button
		Inherits Control

		Public Event Click As EventHandler

		Public Sub RaiseClick()
			RaiseEvent Click(Me, EventArgs.Empty)
		End Sub
	End Class
End Namespace

Public Class VBWithEventsWinFormsCorrectness
	Inherits System.Windows.Forms.Control

	Private WithEvents Button1 As System.Windows.Forms.Button
	Private log As String = ""

	Public Sub New()
		InitializeComponent()
	End Sub

	Private Sub InitializeComponent()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.Button1.Text = "Click"
	End Sub

	Public Function ReplaceButton(button As System.Windows.Forms.Button) As System.Windows.Forms.Button
		Dim oldButton = Button1
		Button1 = button
		Return oldButton
	End Function

	Public Function TakeLog() As String
		Dim result = log
		log = ""
		Return result
	End Function

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		log += "Click;"
	End Sub

	Public Shared Sub Main()
		Dim form = New VBWithEventsWinFormsCorrectness()
		Dim original = form.ReplaceButton(New System.Windows.Forms.Button())
		original.RaiseClick()
		Console.WriteLine("old=" + form.TakeLog())

		Dim current = form.ReplaceButton(New System.Windows.Forms.Button())
		current.RaiseClick()
		Console.WriteLine("current=" + form.TakeLog())

		Dim replacement = New System.Windows.Forms.Button()
		Dim previous = form.ReplaceButton(replacement)
		previous.RaiseClick()
		Console.WriteLine("previous=" + form.TakeLog())

		replacement.RaiseClick()
		Console.WriteLine("replacement=" + form.TakeLog())
	End Sub
End Class
