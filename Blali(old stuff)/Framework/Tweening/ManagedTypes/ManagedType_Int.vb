Namespace Framework.Tweening.ManagedTypes

    <TestState(TestState.Finalized)>
    Friend Class ManagedType_Int
        Implements IManagedType

        Public Function getManagedType() As Type Implements IManagedType.getManagedType
            Return GetType(Integer)
        End Function

        Public Function copy(ByVal o As Object) As Object Implements IManagedType.copy
            Dim value As Integer = CInt(o)
            Return value
        End Function

        Public Function getIntermediateValue(ByVal start As Object, ByVal [end] As Object, ByVal dPercentage As Double) As Object Implements IManagedType.getIntermediateValue
            Dim iStart As Integer = CInt(start)
            Dim iEnd As Integer = CInt([end])
            Return interpolate(iStart, iEnd, dPercentage)
        End Function
    End Class
End Namespace
