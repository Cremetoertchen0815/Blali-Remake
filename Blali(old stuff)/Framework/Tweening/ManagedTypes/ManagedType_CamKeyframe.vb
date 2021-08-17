Imports Emmond.Framework.Cutscenes.TypeA

Namespace Framework.Tweening.ManagedTypes

    <TestState(TestState.Finalized)>
    Public Class ManagedType_CamKeyframe
        Implements IManagedType

        Public Function getManagedType() As Type Implements IManagedType.getManagedType
            Return GetType(CamKeyframe)
        End Function

        Public Function copy(ByVal o As Object) As Object Implements IManagedType.copy
            Dim c As CamKeyframe = CType(o, CamKeyframe)
            Return New CamKeyframe(c.X, c.Y, c.Z, c.Yaw, c.Pitch, c.Roll)
        End Function

        Public Function getIntermediateValue(ByVal start As Object, ByVal [end] As Object, ByVal dPercentage As Double) As Object Implements IManagedType.getIntermediateValue
            Dim startVector As CamKeyframe = CType(start, CamKeyframe)
            Dim endVector As CamKeyframe = CType([end], CamKeyframe)
            Dim iStart_X As Single = startVector.X
            Dim iStart_Y As Single = startVector.Y
            Dim iStart_Z As Single = startVector.Z
            Dim iStart_Yaw As Single = startVector.Yaw
            Dim iStart_Pitch As Single = startVector.Pitch
            Dim iStart_Roll As Single = startVector.Roll
            Dim iEnd_X As Single = endVector.X
            Dim iEnd_Y As Single = endVector.Y
            Dim iEnd_Z As Single = endVector.Z
            Dim iEnd_Yaw As Single = endVector.Yaw
            Dim iEnd_Pitch As Single = endVector.Pitch
            Dim iEnd_Roll As Single = endVector.Roll
            Dim new_X As Single = interpolate(iStart_X, iEnd_X, dPercentage)
            Dim new_Y As Single = interpolate(iStart_Y, iEnd_Y, dPercentage)
            Dim new_Z As Single = interpolate(iStart_Z, iEnd_Z, dPercentage)
            Dim new_Yaw As Single = interpolate(iStart_Yaw, iEnd_Yaw, dPercentage)
            Dim new_Pitch As Single = interpolate(iStart_Pitch, iEnd_Pitch, dPercentage)
            Dim new_Roll As Single = interpolate(iStart_Roll, iEnd_Roll, dPercentage)
            Return New CamKeyframe(new_X, new_Y, new_Z, new_Yaw, new_Pitch, new_Roll)
        End Function
    End Class
End Namespace