Imports System.Collections.Generic
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Level

    <TestState(TestState.NearCompletion)>
    Public Class TileSet
        Inherits Dictionary(Of Vector2, Integer)

        'The current usable tileset layers
        Public Property FrontLayer As New Dictionary(Of Vector2, Integer)
        Public Property BackLayer As New Dictionary(Of Vector2, Integer)

        Public Property Spawn As Vector2 'The grid position at where the player is spawned
        Public Property Size As Vector2 'The size of the tile grid
        Public Property StdCamPosition As Vector2
        Public Property TileSpriteSheet As Texture2D
        Public Property TileSpriteSheetPath As String
        Public Property Tiles As New Dictionary(Of UShort, Tile) 'A list containing all TileSprites
        Public Property TileGroups As New List(Of TileGroup) 'A list containing all TileSprites

        Public Function Clone() As TileSet
            Return Me.MemberwiseClone()
        End Function

        Public Function GetGroupByID(ID As Integer) As TileGroup
            If ID >= 0 And ID < TileGroups.Count Then Return TileGroups(ID)
            Return Nothing
        End Function

    End Class

End Namespace