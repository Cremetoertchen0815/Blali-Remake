Namespace Framework.Tweening.ManagedTypes

    <TestState(TestState.Finalized)>
    Friend Class ManagedType_String
        Implements IManagedType

        Public Function getManagedType() As Type Implements IManagedType.getManagedType
            Return GetType(String)
        End Function

        Public Function copy(ByVal o As Object) As Object Implements IManagedType.copy
            Dim s As String = CStr(o)
            Return New String(s.ToCharArray())
        End Function

        Public Function getIntermediateValue(ByVal start As Object, ByVal [end] As Object, ByVal dPercentage As Double) As Object Implements IManagedType.getIntermediateValue
            Dim strStart As String = CStr(start)
            Dim strEnd As String = CStr([end])
            Dim iStartLength As Integer = strStart.Length
            Dim iEndLength As Integer = strEnd.Length
            Dim iLength As Integer = interpolate(iStartLength, iEndLength, dPercentage)
            Dim result As Char() = New Char(iLength - 1) {}

            For i As Integer = 0 To iLength - 1
                Dim cStart As Char = "a"c

                If i < iStartLength Then
                    cStart = strStart(i)
                End If

                Dim cEnd As Char = "a"c

                If i < iEndLength Then
                    cEnd = strEnd(i)
                End If

                Dim cInterpolated As Char

                If cEnd = " "c Then
                    cInterpolated = " "c
                Else
                    Dim iStart As Integer = Convert.ToInt32(cStart)
                    Dim iEnd As Integer = Convert.ToInt32(cEnd)
                    Dim iInterpolated As Integer = interpolate(iStart, iEnd, dPercentage)
                    cInterpolated = Convert.ToChar(iInterpolated)
                End If

                result(i) = cInterpolated
            Next

            Return New String(result)
        End Function
    End Class
End Namespace
