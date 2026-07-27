Imports System

Public Interface IParameterized
	Property IndexedValue(index As Integer) As Integer
End Interface

Public Class ParameterizedProperties
	Implements IParameterized

	Private _field As Integer

	Public Shared Property SharedProp(index As Integer) As Integer
		Get
			Return index
		End Get
		Set(value As Integer)
		End Set
	End Property

	Public Property IndexedValue(index As Integer) As Integer Implements IParameterized.IndexedValue
		Get
			Return _field
		End Get
		Set(value As Integer)
			_field = value
		End Set
	End Property

	Public ReadOnly Property ReadOnlyProp(index As Integer) As Integer
		Get
			Return index
		End Get
	End Property

	<Obsolete("read-write parameterized property")>
	Public Property Attributed(index As Integer) As Integer
		Get
			Return index
		End Get
		Set(value As Integer)
		End Set
	End Property

	Public Sub Use()
		SharedProp(1) = 2
		_field = SharedProp(3)
		IndexedValue(4) = 5
		_field = IndexedValue(6)
		Dim p As IParameterized = Me
		p.IndexedValue(7) = 8
		_field = p.IndexedValue(9)
	End Sub
End Class
