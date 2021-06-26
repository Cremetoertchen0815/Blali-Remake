Imports Nez.Textures

Namespace Games.Blali_1
    Public Class Gun
        Inherits GameObject

        Public Sub New(spawn As Vector2)
            Me.Spawn = spawn
        End Sub

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 38, 30)))
            Ränder = Entity.AddComponent(New Sprites.SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/gun"))))
            Ränder.LocalOffset = Collider.Bounds.Size / 2

            Entity.LocalPosition = New Vector2(Spawn.X - 38.0F / 2, Spawn.Y - 30)
        End Sub

        Public Overrides Sub Update()
            If Player.Collider.CollidesWith(Collider, Nothing) Then
                Player.BulletCount += 10
                Entity.Destroy()
            End If
        End Sub
    End Class
End Namespace