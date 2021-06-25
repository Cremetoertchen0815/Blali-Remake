Imports Nez.Tiled

Namespace Games.Blali_1
    Public MustInherit Class IVik
        Inherits Component
        Implements IUpdatable


        Private Property Enable As Boolean Implements IUpdatable.Enabled
            Get
                Return Enabled
            End Get
            Set(value As Boolean)
                Enabled = value
            End Set
        End Property
        Private ReadOnly Property IUpdatable_UpdateOrder As Integer Implements IUpdatable.UpdateOrder
            Get
                Return 0
            End Get
        End Property
        Public MustOverride Sub Update() Implements IUpdatable.Update


        Public Map As TmxMap
        Public Collider As BoxCollider
        Protected LevelScore As Integer = 0
        Protected NextID As Integer

        Public Sub Die()
            If Not Enabled Then Return

            Time.TimeScale = 0
            Enabled = False
            LevelScore = 0
            GameScene.SFX.PlayCue("death")

            Dim newsc As New GameScene(GameScene.Current)
            Core.Schedule(0.5, Sub()
                                   Core.Scene = newsc
                                   Time.TimeScale = 1
                               End Sub)
        End Sub

        Public Sub FinishStage()
            If Not Enabled Then Return

            Time.TimeScale = 0
            GameScene.Score += LevelScore
            Enabled = False
            GameScene.SFX.PlayCue("finish")

            Core.Schedule(0.5, Sub() Core.StartSceneTransition(New TransformTransition(TransformTransition.TransformTransitionType.SlideRight) With {.OnScreenObscured = Sub()
                                                                                                                                                                             Core.Scene = New GameScene(NextID)
                                                                                                                                                                             Time.TimeScale = 1
                                                                                                                                                                         End Sub, .TransitionEaseType = Tweens.EaseType.Linear, .Duration = 2}))

        End Sub
    End Class
End Namespace