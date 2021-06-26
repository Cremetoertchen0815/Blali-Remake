Imports Nez.Textures

Namespace Games.Blali_1.Objects
    Public Class RedCoin
        Inherits GameObject
        Public Sub New(spawn As Vector2)
            Me.Spawn = spawn
        End Sub

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(-16, -16, 32, 32)))
            Ränder = Entity.AddComponent(New Sprites.SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/coin_red"))))
            Entity.SetPosition(Spawn - New Vector2(0, 16))
        End Sub

        Public Overrides Sub Update()
            If Player.Collider.CollidesWith(Collider, Nothing) Then
                Entity.Destroy()
                ScoreIncrease(5)
                GameScene.SFX.PlayCue("red_coin")
            End If
        End Sub

    End Class
End Namespace