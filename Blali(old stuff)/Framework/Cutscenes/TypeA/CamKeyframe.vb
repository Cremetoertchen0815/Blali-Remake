Imports Microsoft.Xna.Framework

Namespace Framework.Cutscenes.TypeA

    <TestState(TestState.NearCompletion)>
    Public Structure CamKeyframe
        Public X As Single
        Public Y As Single
        Public Z As Single
        Public Yaw As Single
        Public Pitch As Single
        Public Roll As Single

        Public ReadOnly Property Location As Vector3
            Get
                Return New Vector3(X, Y, Z)
            End Get
        End Property

        Public Sub New(x As Single, y As Single, z As Single, w As Single, p As Single, r As Single)
            Me.X = x
            Me.Y = y
            Me.Z = z
            Yaw = w
            Pitch = p
            Roll = r
        End Sub

    End Structure

End Namespace