Imports System
Imports Nez.Tiled

Namespace Games.Blali_1
    Public MustInherit Class GameObject
        Inherits Component
        Implements IUpdatable

        'Shared fields
        Public Shared ScoreIncrease As Action(Of Integer)
        Friend Shared Player As IVik

        'Object fields
        Protected Ränder As Sprites.SpriteRenderer
        Protected Collider As BoxCollider
        Protected Map As TmxMap
        Protected Spawn As Vector2

        Private Property IUpdatable_Enabled As Boolean Implements IUpdatable.Enabled
            Get
                Return Enabled
            End Get
            Set(value As Boolean)
                Enabled = value
            End Set
        End Property
        Private ReadOnly Property IUpdatable_UpdateOrder As Integer = 0 Implements IUpdatable.UpdateOrder
        Public MustOverride Sub Update() Implements IUpdatable.Update
    End Class
End Namespace