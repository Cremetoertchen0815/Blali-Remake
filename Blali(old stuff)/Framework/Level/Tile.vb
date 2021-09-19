Imports Microsoft.Xna.Framework

Namespace Framework.Level

    <TestState(TestState.NearCompletion)>
    Public Class Tile
        Public Property TileGroupPart As Integer = -1 'Describes whether tile is part of a tile group
        Public Property Collision As TileCollisionType  'Describes the type of collision(0 = No Collision, 1 = Collision as passable floor(droppable), 2 = Complete Collision, 3 = Collision as passable floor(non-droppable))
        Friend _OriRect As Rectangle
        Public Property OriginRectangle As Rectangle 'Represents the source rectangle of the texture
            Get
                Return _OriRect
            End Get
            Set(value As Rectangle)
                _OriRect = value
            End Set
        End Property
        Public Property HeightMap As Byte() = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        Public Property IsSlope As Boolean = False
        Public Property DisableWallJump As Boolean = False


        Function Clone() As Tile
            Return MyBase.MemberwiseClone
        End Function
    End Class
End Namespace