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

Public Class VBWithEventsWinForms
	Inherits System.Windows.Forms.Control

	Friend WithEvents Button1 As System.Windows.Forms.Button
	Private log As String

	Public Sub New()
		InitializeComponent()
	End Sub

	Private Sub InitializeComponent()
		Me.Button1 = New System.Windows.Forms.Button()
		Me.Button1.Text = "Click"
	End Sub

	Private Sub VBWithEventsWinForms_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		log &= "Load;"
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		log &= "Click;"
	End Sub

	Public Sub ReplaceButton(button As System.Windows.Forms.Button)
		Me.Button1 = button
	End Sub
End Class
