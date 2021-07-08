
Imports Blali.Games.Blali_1.Objects
Imports Nez.Textures
Imports Nez.Tiled
Imports System.Collections.Generic

Namespace Games.Blali_1.Mobs
    Public Class MynameisJeff
        Inherits GameObject

        Private Velocity As Vector2
        Private Mover As TiledMapMover
        Private PengPeng As New List(Of Bullet)
        Private Destinyboard As Single

        Public Sub New(spawn As Vector2, map As TmxMap)
            Me.Spawn = spawn
            Me.Map = map
        End Sub

        Public Overrides Sub Initialize()
            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 100, 150)))
            Mover = Entity.AddComponent(New TiledMapMover(CType(Map.GetLayer("Collision"), TmxLayer)))
            Entity.AddComponent(New Sprites.SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/jeff")))).LocalOffset = Collider.Bounds.Size
            Entity.LocalScale = New Vector2(2)

            Velocity = New Vector2(120, 0)
            Entity.LocalPosition = Spawn
            MyBase.Initialize()
        End Sub
        Public Overrides Sub Update()
            'I like to move it, move it!
            Dim lol As New TiledMapMover.CollisionState
            Dim WallCollider = Map.TileLayers(0).GetCollisionRectangles
            Mover.Move(Velocity * Time.DeltaTime, Collider, lol)

            'Flip direction
            If lol.Left Or lol.Right Then Velocity *= -1

            'Interact with bullets
            If Player.CheckBulletCollision(Collider) Then Entity.Destroy() : GameScene.SFX.PlayCue("hit")

            Destinyboard += Time.DeltaTime
            If Destinyboard >= 0.05 * 15 Then
                Destinyboard = 0
                PengPeng.Add(Entity.Scene.CreateEntity("Bullet_jeff_" & PengPeng.Count.ToString).AddComponent(New Bullet(Entity.LocalPosition, Vector2.Normalize(Player.Entity.LocalPosition - Entity.LocalPosition) / 2, PengPeng)))
            End If
            For Each element In PengPeng
                If element.Collider IsNot Nothing AndAlso Player.Collider.CollidesWith(element.Collider, Nothing) Then
                    Player.Die()
                End If
                For Each Tach3 In WallCollider


                    If element.Collider IsNot Nothing AndAlso element.Collider.Bounds.GetRectangle.Intersects(Tach3) Then
                        element.Suicidesquad = True
                        Exit For
                    End If
                Next

            Next
        End Sub
    End Class
End Namespace