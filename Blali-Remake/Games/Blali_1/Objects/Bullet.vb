Imports System
Imports System.Collections.Generic
Imports Nez.Textures

Namespace Games.Blali_1.Objects
    Public Class Bullet
        Inherits GameObject

        Private dir As Integer
        Private Lifetime As Single
        Private pos As Vector2
        Private BulletList As List(Of Bullet)

        Public Sub New(location As Vector2, xSpeed As Integer, adder As List(Of Bullet))
            pos = location
            dir = xSpeed
            BulletList = adder
        End Sub

        Public Overrides Sub OnAddedToEntity()
            MyBase.OnAddedToEntity()

            Collider = Entity.AddComponent(New BoxCollider(New Rectangle(0, 0, 9, 9)))
            Ränder = Entity.AddComponent(New Sprites.SpriteRenderer(New Sprite(Entity.Scene.Content.LoadTexture("game/Blali_1/bullet"))))
            Ränder.LocalOffset = Collider.Bounds.Size / 2
            Ränder.RenderLayer = -1
            Entity.LocalPosition = pos
            BulletList.Add(Me)
        End Sub

        Public Overrides Sub Update()
            Entity.LocalPosition = Entity.LocalPosition + New Vector2(dir * 480, 0) * Time.DeltaTime
            Lifetime += Time.DeltaTime

            If Lifetime > 2 Then
                BulletList.Remove(Me)
                Entity.Destroy()
            End If
        End Sub
    End Class
End Namespace