Imports Nez.Textures
Imports Nez.Tiled

Namespace Games.Blali_1.Mobs
    Public Class Fliegviech
        Inherits GameObject

        Private Mover As TiledMapMover
        Private Velocity As Vector2
        Private bounds As Single()
        Private Const Weite As Integer = 200

        Public Sub New(spawn As Vector2, map As TmxMap)
            Me.Spawn = spawn
            Me.Map = map
        End Sub

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 60, 55)))
            Mover = Entity.AddComponent(New TiledMapMover(CType(Map.GetLayer("Collision"), TmxLayer)))
            Ränder = Entity.AddComponent(New Sprites.SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/enemy_b"))))
            Ränder.LocalOffset = Collider.Bounds.Size / 2

            Velocity = New Vector2(220, 0)
            Entity.LocalPosition = New Vector2(Spawn.X - 60.0F / 2, Spawn.Y - 55)
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
            If Player.Collider.CollidesWith(Collider, Nothing) Then Player.Die()
        End Sub
    End Class
End Namespace