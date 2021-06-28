Imports Nez.Textures

Namespace Games.Blali_1.Objects
    Public Class YellowCoin
        Inherits GameObject

        Public Shared CollectedCount As Integer

        Public Sub New(spawn As Vector2)
            Me.Spawn = spawn
        End Sub

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(-15, -15, 30, 30)))
            Ränder = Entity.AddComponent(New Sprites.SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/coin_yellow"))))
            Entity.SetPosition(Spawn - New Vector2(0, 16))
        End Sub

        Public Overrides Sub Update()
            If Player.Collider.CollidesWith(Collider, Nothing) Then
                Entity.Destroy()
                ScoreIncrease(1)
                CollectedCount += 1
                GameScene.SFX.PlayCue("yellow_coin")
            End If
        End Sub

    End Class
End Namespace