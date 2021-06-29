Imports Nez.Sprites
Imports Nez.Textures
Imports Nez.Tiled

Namespace Games.Blali_1.Objects
    Public Class Canon
        Inherits GameObject

        Private BulletCollider As BoxCollider
        Private BulletRenderer As SpriteRenderer
        Private dir As Vector2
        Private Lifetime As Single
        Private Shot As Boolean = False

        Public Sub New(spawn As Vector2, map As TmxMap)
            Me.Spawn = spawn
            Me.Map = map
        End Sub

        Public Overrides Sub OnAddedToEntity()
            MyBase.OnAddedToEntity()

            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 67, 65)) With {.LocalOffset = Spawn})
            Ränder = Entity.AddComponent(New SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/portal"))) With {.LocalOffset = Spawn})
            BulletCollider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 9, 9)) With {.Enabled = False, .LocalOffset = Spawn})
            BulletRenderer = Entity.AddComponent(New SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/bullet"))) With {.LocalOffset = BulletCollider.Bounds.Size / 2 + Spawn, .RenderLayer = -2, .Enabled = False})
            Entity.LocalPosition = Vector2.Zero

        End Sub

        Public Overrides Sub Update()
            Lifetime += Time.DeltaTime

            'Move bullet
            If Shot Then
                BulletCollider.LocalOffset = BulletCollider.LocalOffset + dir * 218 * Time.DeltaTime
                BulletRenderer.LocalOffset = BulletRenderer.LocalOffset + dir * 218 * Time.DeltaTime
            End If

            'Spawn bullet
            If Lifetime > 0.66 And Not Shot Then
                Shot = True
                BulletCollider.Enabled = True
                BulletRenderer.Enabled = True
                dir = Vector2.Normalize(Player.Entity.LocalPosition - Spawn)
                Lifetime = 0
            End If

            'Kill bullet
            If Lifetime > 2.33 And Shot Then
                Shot = False
                Reset()
                Lifetime = 0
            End If

            Dim rct = BulletCollider.Bounds.GetRectangle
            For Each element In Map.TileLayers(0).GetCollisionRectangles()
                If element.Intersects(rct) Then Reset() : Exit For
            Next

            'Interact with bullets
            If Player.CheckBulletCollision(Collider) Then Entity.Destroy() : GameScene.SFX.PlayCue("hit")

            'Kill player if touching bullet
            If BulletCollider.CollidesWith(Player.Collider, Nothing) Then Player.Die()
        End Sub

        Private Sub Reset()
            BulletCollider.Enabled = False
            BulletCollider.LocalOffset = Spawn
            BulletRenderer.Enabled = False
            BulletRenderer.LocalOffset = Spawn
        End Sub
    End Class
End Namespace