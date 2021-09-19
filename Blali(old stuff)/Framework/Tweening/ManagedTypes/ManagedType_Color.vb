Imports Microsoft.Xna.Framework

Namespace Framework.Tweening.ManagedTypes

    <TestState(TestState.Finalized)>
    Friend Class ManagedType_Color
        Implements IManagedType

        Public Function getManagedType() As Type Implements IManagedType.getManagedType
            Return GetType(Color)
        End Function

        Public Function copy(ByVal o As Object) As Object Implements IManagedType.copy
            Dim c As Color = CType(o, Color)
            Dim result As Color = New Color(c.R, c.G, c.B, c.A)
            Return result
        End Function

        Public Function getIntermediateValue(ByVal start As Object, ByVal [end] As Object, ByVal dPercentage As Double) As Object Implements IManagedType.getIntermediateValue
            Dim startColor As Color = CType(start, Color)
            Dim endColor As Color = CType([end], Color)
            Dim iStart_R As Integer = startColor.R
            Dim iStart_G As Integer = startColor.G
            Dim iStart_B As Integer = startColor.B
            Dim iStart_A As Integer = startColor.A
            Dim iEnd_R As Integer = endColor.R
            Dim iEnd_G As Integer = endColor.G
            Dim iEnd_B As Integer = endColor.B
            Dim iEnd_A As Integer = endColor.A
            Dim new_R As Integer = interpolate(iStart_R, iEnd_R, dPercentage)
            Dim new_G As Integer = interpolate(iStart_G, iEnd_G, dPercentage)
            Dim new_B As Integer = interpolate(iStart_B, iEnd_B, dPercentage)
            Dim new_A As Integer = interpolate(iStart_A, iEnd_A, dPercentage)
            Return New Color(new_R, new_G, new_B, new_A)
        End Function
    End Class
End Namespace
