Imports System

Public Class VBDelegateCache
	Public Function MakeAction() As Action
		Return Sub() Console.WriteLine("fixed")
	End Function
End Class
