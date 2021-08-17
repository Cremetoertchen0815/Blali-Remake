Imports Microsoft.Xna.Framework

Namespace Framework.Tweening.ManagedTypes

    <TestState(TestState.Finalized)>
    Public Class ManagedType_Vector4
        Implements IManagedType

        Public Function getManagedType() As Type Implements IManagedType.getManagedType
            Return GetType(Vector4)
        End Function

        Public Function copy(ByVal o As Object) As Object Implements IManagedType.copy
            Dim c As Vector4 = CType(o, Vector4)
            Return New Vector4(c.X, c.Y, c.Z, c.W)
        End Function

        Public Function getIntermediateValue(ByVal start As Object, ByVal [end] As Object, ByVal dPercentage As Double) As Object Implements IManagedType.getIntermediateValue
            Dim startVector As Vector4 = CType(start, Vector4)
            Dim endVector As Vector4 = CType([end], Vector4)
            Dim iStart_X As Single = startVector.X
            Dim iStart_Y As Single = startVector.Y
            Dim iStart_Z As Single = startVector.Z
            Dim iStart_W As Single = startVector.W
            Dim iEnd_X As Single = endVector.X
            Dim iEnd_Y As Single = endVector.Y
            Dim iEnd_Z As Single = endVector.Z
            Dim iEnd_W As Single = endVector.W
            Dim new_X As Single = interpolate(iStart_X, iEnd_X, dPercentage)
            Dim new_Y As Single = interpolate(iStart_Y, iEnd_Y, dPercentage)
            Dim new_Z As Single = interpolate(iStart_Z, iEnd_Z, dPercentage)
            Dim new_W As Single = interpolate(iStart_W, iEnd_W, dPercentage)
            Return New Vector4(new_X, new_Y, new_Z, new_W)
        End Function
    End Class
End Namespace