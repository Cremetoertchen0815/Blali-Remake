Imports System.Collections.Generic
Imports Microsoft.Xna.Framework

Namespace Framework.Level

    <TestState(TestState.NearCompletion)>
    Public Class TileGroup
        Public Size As Vector2
        Public Property Origin As Rectangle
        Public OriginalTile As Integer

        Public Property Tiles As Dictionary(Of UShort, Tile)
        Public Property MapTiles As Dictionary(Of Vector2, UShort)

        Public Sub GenerateTiles(lvl As Level, AddTilesToLvl As Boolean, id As Integer, Optional init As Boolean = False)
            If Not (init And MapTiles IsNot Nothing AndAlso MapTiles.Count > 0) Then
                Dim oldmaptile = MapTiles
                Tiles = New Dictionary(Of UShort, Tile)
                MapTiles = New Dictionary(Of Vector2, UShort)

                If lvl.Map.Tiles.ContainsKey(OriginalTile) Then Origin = lvl.Map.Tiles(OriginalTile).OriginRectangle

                Dim countingID As Integer = 1
                Dim tmpTile As Tile

                For x As Integer = 0 To Size.X - 1
                    For y As Integer = 0 To Size.Y - 1
                        'Generate Tile
                        tmpTile = New Tile
                        tmpTile.Collision = lvl.Map.Tiles(OriginalTile).Collision
                        tmpTile.DisableWallJump = lvl.Map.Tiles(OriginalTile).DisableWallJump
                        tmpTile.TileGroupPart = id
                        tmpTile.OriginRectangle = New Rectangle(Origin.Left + ((Origin.Width / Size.X) * x), Origin.Top + ((Origin.Height / Size.Y) * (Size.Y - 1 - y)), Origin.Width / Size.X, Origin.Height / Size.Y)
                        Do While lvl.Map.Tiles.ContainsKey(countingID)
                            countingID += 1
                        Loop
                        If AddTilesToLvl Then lvl.Map.Tiles.Add(countingID, tmpTile) 'Add tile to level
                        Tiles.Add(countingID, tmpTile) 'Add tile t
                        MapTiles.Add(New Vector2(x, y), countingID) 'Add tile to tilegroup preset
                        countingID += 1
                    Next
                Next


                If oldmaptile IsNot Nothing Then
                    'Remap map values
                    Dim remap As New Dictionary(Of Integer, Integer) 'contains what tile numbers should be replaced with in the map
                    For Each a In MapTiles
                        For Each b In oldmaptile
                            If a.Key = b.Key Then remap.Add(b.Value, a.Value)
                        Next
                    Next
                    'Make a copy of the map
                    Dim clone As New Dictionary(Of Vector2, Integer)
                    For Each copy In lvl.Map
                        clone.Add(copy.Key, copy.Value)
                    Next
                    For Each tile In clone
                        If remap.ContainsKey(tile.Value) Then lvl.Map(tile.Key) = remap(tile.Value)
                    Next
                End If

            Else
                Tiles = New Dictionary(Of UShort, Tile)
                For Each element In MapTiles
                    Tiles.Add(element.Value, lvl.Map.Tiles(element.Value))
                Next
            End If
        End Sub

        Public Sub RemoveTilesFromLvl(lvl As Level)
            For Each element In MapTiles
                If lvl.Map.Tiles.ContainsKey(element.Value) Then lvl.Map.Tiles.Remove(element.Value)
            Next
        End Sub

        Public Sub RemapTilesToTileGroup(lvl As Level, groupid As Integer)
            For Each element In lvl.Map.Tiles
                If Tiles.ContainsKey(element.Key) Then
                    'The tile fits into this tile group
                    element.Value.TileGroupPart = groupid
                Else
                    'The tile doesn't belong to this group
                    If element.Value.TileGroupPart = groupid Then element.Value.TileGroupPart = groupid = -1
                End If
            Next
        End Sub

    End Class

End Namespace