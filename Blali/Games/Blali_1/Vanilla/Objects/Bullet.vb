Imports System
Imports System.Collections.Generic
Imports Nez.Textures

Namespace Games.Blali_1.Vanilla.Objects
    Public Class Bullet
        Inherits GameObject

        Private dir As Vector2
        Private Lifetime As Single
        Private pos As Vector2
        Private BulletList As List(Of Bullet)
        Friend Suicidesquad As Boolean = False

        Public Sub New(location As Vector2, xSpeed As Vector2, adder As List(Of Bullet))
            pos = location
            dir = xSpeed
            BulletList = adder
        End Sub

        Public Overrides Sub OnAddedToEntity()
            MyBase.OnAddedToEntity()

            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 9, 9)))
            Ränder = Entity.AddComponent(New Sprites.SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/bullet"))) With {.RenderLayer = -2})
            Ränder.LocalOffset = Collider.Bounds.Size / 2
            Ränder.RenderLayer = -2
            Entity.LocalPosition = pos
            BulletList.Add(Me)
        End Sub

        Public Overrides Sub Update()
            Entity.LocalPosition = Entity.LocalPosition + dir * 480 * Time.DeltaTime
            Lifetime += Time.DeltaTime

            If Lifetime > 4 Or Suicidesquad Then
                BulletList.Remove(Me)
                Entity.Destroy()
            End If
        End Sub
    End Class
End Namespace