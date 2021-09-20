Imports System.Collections.Generic
Imports Blali.Games.Blali_1.Remastered.Objects
Imports Nez.Textures
Imports Nez.Tiled

Namespace Games.Blali_1.Remastered.Mobs
    Public Class MynameisJeff
        Inherits GameObject

        Private Velocity As Vector2
        Private Mover As TiledMapMover
        Private PengPeng As New List(Of Bullet)
        Private Destinyboard As Single = -1

        Public Sub New(spawn As Vector2, map As TmxMap)
            Me.Spawn = spawn
            Me.Map = map
        End Sub

        Public Overrides Sub Initialize()
            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 100, 150)))
            Mover = Entity.AddComponent(New TiledMapMover(CType(Map.GetLayer("Collision"), TmxLayer)))
            Ränder = Entity.AddComponent(New Sprites.SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/jeff"))) With {.LocalOffset = Collider.Bounds.Size})
            Ränder.RenderLayer = 2
            Entity.LocalScale = New Vector2(2)

            Velocity = New Vector2(350, 0)
            Entity.LocalPosition = Spawn
            MyBase.Initialize()
        End Sub

        'Creme: Ich kommentier das mal aus, weil Johannes es vergessen hat :P
        'TODO: ADD POOLING!!!!
        Public Overrides Sub Update()
            'I like to move it, move it!
            Dim lol As New TiledMapMover.CollisionState
            Dim WallCollider = Map.TileLayers(0).GetCollisionRectangles
            Mover.Move(Velocity * Time.DeltaTime, Collider, lol)

            'Flip direction
            If lol.Left Or lol.Right Then Velocity *= -1

            'Interact with bullets
            If Player.CheckBulletCollision(Collider) Then Entity.Destroy() : GameScene.SFX.PlayCue("hit") : Player.FinishStage()

            'Bullet spawn timer
            Destinyboard += Time.DeltaTime
            If Destinyboard >= 0.38 Then
                Velocity *= -1
                Destinyboard = 0
                Entity.Scene.CreateEntity("Bullet_jeff_" & PengPeng.Count.ToString).AddComponent(New Bullet(Entity.LocalPosition, Vector2.Normalize(Player.Entity.LocalPosition - Entity.LocalPosition) / 2, PengPeng))
            End If

            For Each element In PengPeng
                'Check if bullet hits player
                If element.Collider IsNot Nothing AndAlso Player.Collider.CollidesWith(element.Collider, Nothing) Then
                    Player.Die()
                End If
                'Check if bullet hit a wall
                For Each Tach3 In WallCollider
                    If element.Collider IsNot Nothing AndAlso element.Collider.Bounds.GetRectangle.Intersects(Tach3) Then
                        element.Suicidesquad = True
                        Exit For
                    End If
                Next

            Next

            Ränder.FlipX = Velocity.X < 0
        End Sub
    End Class
End Namespace