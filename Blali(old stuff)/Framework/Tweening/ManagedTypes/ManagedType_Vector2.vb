Imports Microsoft.Xna.Framework

Namespace Framework.Tweening.ManagedTypes

    <TestState(TestState.Finalized)>
    Public Class ManagedType_Vector2
    Implements IManagedType

    Public Function getManagedType() As Type Implements IManagedType.getManagedType
        Return GetType(Vector2)
    End Function

    Public Function copy(ByVal o As Object) As Object Implements IManagedType.copy
        Dim c As Vector2 = CType(o, Vector2)
        Return New Vector2(c.X, c.Y)
    End Function

    Public Function getIntermediateValue(ByVal start As Object, ByVal [end] As Object, ByVal dPercentage As Double) As Object Implements IManagedType.getIntermediateValue
        Dim startVector As Vector2 = CType(start, Vector2)
        Dim endVector As Vector2 = CType([end], Vector2)
        Dim iStart_X As Single = startVector.X
        Dim iStart_Y As Single = startVector.Y
        Dim iEnd_X As Single = endVector.X
        Dim iEnd_Y As Single = endVector.Y
        Dim new_X As Single = interpolate(iStart_X, iEnd_X, dPercentage)
        Dim new_Y As Single = interpolate(iStart_Y, iEnd_Y, dPercentage)
        Return New Vector2(new_X, new_Y)
    End Function
End Class

End Namespace