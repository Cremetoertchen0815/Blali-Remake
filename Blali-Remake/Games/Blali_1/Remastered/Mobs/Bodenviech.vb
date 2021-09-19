Imports Nez.Textures
Imports Nez.Tiled

Namespace Games.Blali_1.Remastered.Mobs
    Public Class Bodenviech
        Inherits GameObject

        Private Mover As TiledMapMover
        Private Velocity As Vector2
        Private bounds As Single()
        Private Const Weite As Integer = 180

        Public Sub New(spawn As Vector2, map As TmxMap)
            Me.Spawn = spawn
            Me.Map = map
        End Sub

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 35, 40)))
            Mover = Entity.AddComponent(New TiledMapMover(CType(Map.GetLayer("Collision"), TmxLayer)))
            Ränder = Entity.AddComponent(New Sprites.SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/enemy_c"))))
            Ränder.LocalOffset = Collider.Bounds.Size / 2 * 1.5
            Entity.LocalScale = New Vector2(1.5F)

            Velocity = New Vector2(270, 0)
            Entity.LocalPosition = New Vector2(Spawn.X - 35.0F / 2, Spawn.Y - 40)
            bounds = {Entity.LocalPosition.X - Weite, Entity.LocalPosition.X + Weite}
        End Sub

        Public Overrides Sub Update()
            'I like to move it, move it!
            Dim lol As New TiledMapMover.CollisionState
            Mover.Move(Velocity * Time.DeltaTime, Collider, lol)

            'Flip direction
            If Entity.LocalPosition.X < bounds(0) Then Velocity *= -1 : Entity.LocalPosition = New Vector2(bounds(0), Entity.LocalPosition.Y)
            If Entity.LocalPosition.X > bounds(1) Then Velocity *= -1 : Entity.LocalPosition = New Vector2(bounds(1), Entity.LocalPosition.Y)
            If lol.Left Or lol.Right Then Velocity *= -1

            'Flip texture
            Ränder.FlipX = Velocity.X > 0

            'Interact with bullets
            If Player.CheckBulletCollision(Collider) Then Entity.Destroy() : GameScene.SFX.PlayCue("hit")

            'Interact with player
            Dim col As New CollisionResult
            If Player.Collider.CollidesWith(Collider, col) Then
                If System.Math.Abs(col.MinimumTranslationVector.X) < System.Math.Abs(col.MinimumTranslationVector.Y) And col.MinimumTranslationVector.Y > 0 Then
                    Enabled = False
                    ScoreIncrease(20)
                    GameScene.SFX.PlayCue("stomp")
                    Ränder.LocalOffset = Collider.Bounds.Size * 0.75 + New Vector2(0, 15)
                    Entity.LocalScale = New Vector2(1.5, 0.2F)
                Else
                    Player.Die()
                End If
            End If
        End Sub
    End Class
End Namespace