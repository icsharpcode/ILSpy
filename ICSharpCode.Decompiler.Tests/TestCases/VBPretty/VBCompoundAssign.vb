Imports System
Imports Microsoft.VisualBasic

Module VBCompoundAssign
    Function Sum3(v As Int32()) As Double()
        Dim arr(3) As Double
        For i = 0 To UBound(v) Step 3
            arr(0) += v(i)
            arr(1) += v(i + 1)
            arr(2) += v(i + 2)
        Next
        Return arr
    End Function
End Module
