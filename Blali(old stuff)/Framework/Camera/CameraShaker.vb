
Imports Emmond.Framework.Tweening
Imports Microsoft.Xna.Framework

Namespace Framework.Camera
    <TestState(TestState.NearCompletion)>
    Public Class CameraShaker
        Friend ReadOnly Property Offset As Vector2
            Get
                Return New Vector2(moverX.Value, moverY.Value)
            End Get
        End Property

        Dim moverX As New Transition(Of Single) With {.Value = 0}
        Dim moverY As New Transition(Of Single) With {.Value = 0}
        Private Const Len As Integer = 150

        Friend Sub TriggerShaker(strength As Single)
            'TODO: Change random strength to random direction
            moverX = New Transition(Of Single)(New TransitionTypes.TransitionType_Swing(15, 100), 0, Math.Sign(Rand.NextDouble * 2 - 1) * strength, Nothing)
            moverY = New Transition(Of Single)(New TransitionTypes.TransitionType_Swing(15, 100), 0, Math.Sign(Rand.NextDouble * 2 - 1) * strength, Nothing)
            Automator.Add(moverX)
            Automator.Add(moverY)
        End Sub
    End Class
End Namespace