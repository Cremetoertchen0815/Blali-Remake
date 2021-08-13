﻿Imports Nez.Tiled

Namespace Games.Blali_1
    Public MustInherit Class IVik
        Inherits Component
        Implements IUpdatable

        'Properties(IUpdatable implementation)
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
        Friend MustOverride Function CheckBulletCollision(co As Collider) As Boolean

        'Fields
        Public Shared PlayerStartSFX As Boolean = False
        Public Map As TmxMap
        Public Collider As BoxCollider
        Public BulletCount As Integer = 0
        Protected LevelScore As Integer = 0
        Protected NextID As Integer

        Public Sub Die()
            If Not Enabled Then Return

            Time.TimeScale = 0
            Enabled = False
            LevelScore = 0
            GameScene.SFX.PlayCue("death")

            Core.Schedule(0.5, Sub() Core.StartSceneTransition(New CrossFadeTransition(Function() New GameScene(GameScene.Current)) With {.OnScreenObscured = Sub() Time.TimeScale = 1, .FadeDuration = 0}))
            'Core.Schedule(0.5, Sub()
            '                       Core.Scene = newsc
            '                       Time.TimeScale = 1
            '                   End Sub)
        End Sub

        Public Sub FinishStage()
            If Not Enabled Then Return

            Time.TimeScale = 0
            GameScene.Score += LevelScore
            Enabled = False
            GameScene.SFX.PlayCue("finish")

            Core.Schedule(0.5, Sub() Core.StartSceneTransition(New TransformTransition(Function() New GameScene(NextID), TransformTransition.TransformTransitionType.SlideRight) With {.OnScreenObscured = Sub() Time.TimeScale = 1, .TransitionEaseType = Tweens.EaseType.Linear, .Duration = 2}))

        End Sub
    End Class
End Namespace