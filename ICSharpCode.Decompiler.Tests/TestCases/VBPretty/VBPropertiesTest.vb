Imports System
Public Class VBPropertiesTest
	Private _fullProperty As Integer

	Property FullProperty As Integer
		Get
			Return _fullProperty
		End Get
		Set(value As Integer)
			_fullProperty = value
		End Set
	End Property

	Property AutoProperty As Integer

#If ROSLYN Then
	ReadOnly Property ReadOnlyAutoProperty As Integer

	Sub New()
		Me.ReadOnlyAutoProperty = 32
	End Sub
#End If

	Sub TestMethod()
		Me.FullProperty = 42
		Me._fullProperty = 24
		Me.AutoProperty = 4711

		Console.WriteLine(Me.AutoProperty)
		Console.WriteLine(Me._fullProperty)
		Console.WriteLine(Me.FullProperty)

#If ROSLYN Then
		Console.WriteLine(Me.ReadOnlyAutoProperty)
#End If
	End Sub
End Class