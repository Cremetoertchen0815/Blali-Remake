Imports Nez.Textures
Imports Nez.Tiled

Namespace Games.Blali_1.Mobs
    Public Class Spike
        Inherits Component
        Implements IUpdatable

        Private Collider As BoxCollider
        Private Mover As TiledMapMover
        Private Map As TmxMap
        Private Velocity As Vector2
        Private Spawn As Vector2
        Friend Player As PlayerMover

        Public Sub New(spawn As Vector2, map As TmxMap)
            Me.Map = map
            Me.Spawn = spawn
        End Sub

        Private ReadOnly Property IUpdatable_Enabled As Boolean = True Implements IUpdatable.Enabled
        Private ReadOnly Property IUpdatable_UpdateOrder As Integer = 0 Implements IUpdatable.UpdateOrder

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 140, 150)))
            Mover = Entity.AddComponent(New TiledMapMover(CType(Map.GetLayer("Collision"), TmxLayer)))
            Entity.AddComponent(New Sprites.SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/enemy_a")))).LocalOffset = Collider.Bounds.Size / 2

            Velocity = New Vector2(200, 0)
            Entity.LocalPosition = New Vector2(Spawn.X - 140.0F / 2, Spawn.Y - 150)
        End Sub

        Public Sub Update() Implements IUpdatable.Update
            'I like to move it, move it!
            Dim lol As New TiledMapMover.CollisionState
            Mover.Move(Velocity * Time.DeltaTime, Collider, lol)

            'Flip direction
            If lol.Left Or lol.Right Then Velocity *= -1

            If Player.Collider.CollidesWith(Collider, Nothing) Then Player.Die()
        End Sub
    End Class
End Namespace