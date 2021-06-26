Imports Nez.Textures
Imports Nez.Tiled

Namespace Games.Blali_1.Mobs
    Public Class Spike
        Inherits GameObject

        Public Sub New(spawn As Vector2, map As TmxMap)
            Me.Spawn = spawn
            Me.Map = map
        End Sub

        Private Mover As TiledMapMover
        Private Velocity As Vector2

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 20, 100, 130)))
            Mover = Entity.AddComponent(New TiledMapMover(CType(Map.GetLayer("Collision"), TmxLayer)))
            Entity.AddComponent(New Sprites.SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/enemy_a")))).LocalOffset = Collider.Bounds.Center - Entity.LocalPosition

            Velocity = New Vector2(130, 0)
            Entity.LocalPosition = New Vector2(Spawn.X - 140.0F / 2, Spawn.Y - 150)
        End Sub

        Public Overrides Sub Update()
            'I like to move it, move it!
            Dim lol As New TiledMapMover.CollisionState
            Mover.Move(Velocity * Time.DeltaTime, Collider, lol)

            'Flip direction
            If lol.Left Or lol.Right Then Velocity *= -1

            'Interact with bullets
            If Player.CheckBulletCollision(Collider) Then Entity.Destroy() : GameScene.SFX.PlayCue("hit")

            'Interact with player
            If Player.Collider.CollidesWith(Collider, Nothing) Then Player.Die()
        End Sub
    End Class
End Namespace