Public Class VBAutomaticEvents
	Event EventWithParameter(ByVal EventNumber As Integer)
	Event EventWithoutParameter()

	Sub RaiseEvents()
		RaiseEvent EventWithParameter(1)
		RaiseEvent EventWithoutParameter()
	End Sub
End Class
