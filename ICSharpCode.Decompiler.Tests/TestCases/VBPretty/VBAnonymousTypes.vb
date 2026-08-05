Imports System
Imports System.Collections.Generic
Imports System.Linq

Public Module VBAnonymousTypes
	Public Sub MutableAnonymousType()
		Dim value = New With {.Value = 1, .Name = "test"}
		Console.WriteLine(value.Value)
		Console.WriteLine(value.Name)
	End Sub

	Public Sub KeyAnonymousType()
		Dim value = New With {Key .Value = 1, Key .Name = "test"}
		Console.WriteLine(value.Value)
		Console.WriteLine(value.Name)
	End Sub

	Public Sub AnonymousTypeAsArgument()
		Console.WriteLine(New With {Key .Value = 1, Key .Name = "test"}.ToString())
	End Sub

	Public Sub SelectAnonymousType(items As IEnumerable(Of Integer))
		Dim query = From i In items
					Select New With {Key .Value = i, Key .Square = i * i}
		For Each item In query
			Console.WriteLine(item.Value)
			Console.WriteLine(item.Square)
		Next
	End Sub

	Public Sub LetWhereSelect(items As IEnumerable(Of Integer))
		Dim query = From i In items
					Let square = i * i
					Where square > 4
					Select i, square
		For Each item In query
			Console.WriteLine(item.i)
			Console.WriteLine(item.square)
		Next
	End Sub

	Public Sub JoinSelect(left As IEnumerable(Of Integer), right As IEnumerable(Of Integer))
		Dim query = From x In left
					Join y In right On x Equals y
					Select x, y
		For Each item In query
			Console.WriteLine(item.x)
			Console.WriteLine(item.y)
		Next
	End Sub

	Public Sub OrderBySelect(items As IEnumerable(Of Integer))
		Dim query = From i In items
					Let doubled = i * 2
					Order By doubled
					Select i, doubled
		For Each item In query
			Console.WriteLine(item.i)
			Console.WriteLine(item.doubled)
		Next
	End Sub
End Module
