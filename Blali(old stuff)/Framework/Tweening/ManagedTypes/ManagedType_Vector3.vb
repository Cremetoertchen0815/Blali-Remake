Imports Microsoft.Xna.Framework

Namespace Framework.Tweening.ManagedTypes

    <TestState(TestState.Finalized)>
    Public Class ManagedType_Vector3
    Implements IManagedType

    Public Function getManagedType() As Type Implements IManagedType.getManagedType
        Return GetType(Vector3)
    End Function

    Public Function copy(ByVal o As Object) As Object Implements IManagedType.copy
        Dim c As Vector3 = CType(o, Vector3)
        Return New Vector3(c.X, c.Y, c.Z)
    End Function

    Public Function getIntermediateValue(ByVal start As Object, ByVal [end] As Object, ByVal dPercentage As Double) As Object Implements IManagedType.getIntermediateValue
        Dim startVector As Vector3 = CType(start, Vector3)
        Dim endVector As Vector3 = CType([end], Vector3)
        Dim iStart_X As Single = startVector.X
        Dim iStart_Y As Single = startVector.Y
        Dim iStart_Z As Single = startVector.Z
        Dim iEnd_X As Single = endVector.X
        Dim iEnd_Y As Single = endVector.Y
        Dim iEnd_Z As Single = endVector.Z
        Dim new_X As Single = interpolate(iStart_X, iEnd_X, dPercentage)
        Dim new_Y As Single = interpolate(iStart_Y, iEnd_Y, dPercentage)
        Dim new_Z As Single = interpolate(iStart_Z, iEnd_Z, dPercentage)
        Return New Vector3(new_X, new_Y, new_Z)
    End Function
End Class

End Namespace