Imports System.Collections.Generic
Imports Emmond.Framework.Entities
Imports Emmond.Framework.Graphics
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Penumbra

Namespace Framework.Level

    <TestState(TestState.WorkInProgress)>
    Public Class Level

        Public Property Header As LevelHeader
        Public Property MSFX As MSFXcontainer

        'Camera
        Public Property Camera As Camera.Camera
        Public Property CamShaker As New Camera.CameraShaker

        'BG
        Public Property Background As Background.BackgroundManager 'Contains all the information about the background layer

        'MAP/TLS
        Public Property Map As TileSet 'A list containing the tile grid
        Public Property MapCluster As Dictionary(Of Vector2, Boolean())
        Public Property MapRenderer As MapRenderer

        'TRG
        Public Property TriggerMan As Dictionary(Of UInteger, Trigger) 'Contains the list of all triggers(ID 0 is the finish line)

        'FX
        Public Property FXData As Graphics.FXData

        'OBJ
        Public Property Entities As List(Of Entity)
        Public Property ItemMan As ObjectManager.ObjectManager 'Settingsages all items

        'Flags
        Friend BufferedData As New IG.BufferedCheckpoint With {.Trigger = -1}

        Public Sub LoadContent(Optional fillempties As Boolean = False, Optional onlyfillempties As Boolean = False)
            'Only load if not yet loaded
            If Not Header.Loaded Or onlyfillempties Then

                'Set Flag
                Header.Loaded = True

                'Fill non-existent tiles within the level boundaries with blank ones
                If fillempties Then
                    For yy As UShort = 0 To Map.Size.Y
                        For xx As UShort = 0 To Map.Size.X
                            Dim key As Vector2 = New Vector2(xx, yy)
                            If Not Map.ContainsKey(key) Then Map.Add(key, 0)
                            If Not Map.BackLayer.ContainsKey(key) Then Map.BackLayer.Add(key, 0)
                            If Not Map.FrontLayer.ContainsKey(key) Then Map.FrontLayer.Add(key, 0)
                        Next
                    Next
                ElseIf Not fillempties And DebugMode Then
                    For yy As UShort = 0 To Map.Size.Y
                        For xx As UShort = 0 To Map.Size.X
                            Dim key As Vector2 = New Vector2(xx, yy)
                            If Map.ContainsKey(key) AndAlso Map(key) = 0 Then Map.Remove(key)
                            If Map.BackLayer.ContainsKey(key) AndAlso Map.BackLayer(key) = 0 Then Map.BackLayer.Remove(key)
                            If Map.FrontLayer.ContainsKey(key) AndAlso Map.FrontLayer(key) = 0 Then Map.FrontLayer.Remove(key)
                        Next
                    Next
                End If

                'Load and initialize entities(if level wasn't loaded before)
                For Each element In Entities
                    If element IsNot Nothing Then
                        element.Initialize()
                        element.LoadContent(1)
                    End If
                Next

                If onlyfillempties Then Background.Init(Me) : Exit Sub

                If Map.TileSpriteSheet Is Nothing Then Map.TileSpriteSheet = ContentMan.Load(Of Texture2D)(Map.TileSpriteSheetPath)

                'Position camera
                Camera.Location = Camera.DefaultLocation
                Camera.Zoom = 1
                Camera.Rotation = 0
                Camera.NoCameraPoints = New List(Of Vector2)
                For Each trgs In TriggerMan
                    trgs.Value.Init()
                    If trgs.Value.Type = TriggerType.CameraBarrier Then Camera.NoCameraPoints.Add(trgs.Value.GridLine(0)) : Camera.NoCameraPoints.Add(trgs.Value.GridLine(1))
                Next

                Lighting = New PenumbraComponent(EmmondInstance)
                Lighting.Initialize()
                Lighting.SpriteBatchTransformEnabled = True
                Lighting.Enabled = FXData.EnableLighting
                Lighting.Visible = FXData.EnableLighting
                Lighting.AmbientColor = FXData.AmbientColor
                Lighting.Hulls.Clear()
                For Each element In FXData.Hulls
                    Lighting.Hulls.Add(element)
                Next
                Lighting.Lights.Clear()
                For Each element In FXData.Lights
                    Lighting.Lights.Add(element)
                Next

                'Load assets for backgrounds
                If Background.skyboxPath <> "" And Background.skybox Is Nothing Then Background.skybox = ContentMan.Load(Of Texture2D)("lvl\" & Header.LoadedID & "\" & Background.skyboxPath)
                For Each element In Background
                    If element.Texture Is Nothing Then element.Texture = ContentMan.Load(Of Texture2D)("lvl\" & Header.LoadedID & "\media\" & element.Path)
                Next

                Background.Init(Me)

                If ItemMan IsNot Nothing Then ItemMan.Init()

                For Each element In ItemMan
                    element.Value.Init(Me)
                Next

                For i As Integer = 0 To Map.TileGroups.Count - 1
                    Map.TileGroups(i).GenerateTiles(Me, True, i, True)
                    Map.TileGroups(i).RemapTilesToTileGroup(Me, i)
                Next
                For Each element In Map.TileGroups
                Next
            End If


            MapCluster = New Dictionary(Of Vector2, Boolean())
            For x As Integer = 0 To Math.Ceiling(Map.Size.X / (GameSize.X / cTileSize)) - 1
                For y As Integer = 0 To Math.Ceiling(Map.Size.Y / (GameSize.Y / cTileSize)) - 1

                    MapCluster.Add(New Vector2(x, y), {False, False, False})
                Next
            Next
        End Sub

        Public Sub DrawDebug()
            For Each element In Entities
                If element.CulledIn Then
                    Dim centr As Vector2 = Camera.TranslateToCamera(New Vector2(element.mPosition.X, 1080 - element.mPosition.Y))
                    DrawRectangle(New Rectangle(centr.X - element.mAABB.halfSize.X, centr.Y - element.mAABB.halfSize.Y, element.mAABB.halfSize.X * 2, element.mAABB.halfSize.Y * 2), Color.Red, 2) 'Draw Rectangle
                    DrawRectangle(New Rectangle(centr.X, centr.Y, 1, 1), Color.Red, 5) 'Draw Center
                    Dim rect As Vector2 = Camera.TranslateToCamera(New Vector2(element.mAABB.GetRectangle.Right + 5, 1080 - element.mAABB.GetRectangle.Bottom))
                    SpriteBat.DrawString(DebugSmolFont, "Entity #" & element.mAttributes.UniqueID.ToString, rect, Color.White)
                    SpriteBat.DrawString(DebugSmolFont, "Position: " & element.mPosition.ToString, rect + New Vector2(0, 15), Color.White)
                    SpriteBat.DrawString(DebugSmolFont, "Speed: " & element.mSpeed.ToString, rect + New Vector2(0, 30), Color.White)
                    SpriteBat.DrawString(DebugSmolFont, "Health: " & element.mAttributes.Health.ToString, rect + New Vector2(0, 45), Color.White)
                    SpriteBat.DrawString(DebugSmolFont, "Health: " & element.mAttributes.Health.ToString, rect + New Vector2(0, 45), Color.White)
                End If
            Next
        End Sub


        'Unload Assets
        Public Sub UnloadContent()
            Header.Loaded = False
            'For Each element In Map
            '    element = Nothing
            'Next
            Map = Nothing
            For Each element In ItemMan
                'element.Texture.Dispose()
                element = Nothing
            Next

            'For Each element In Clusters
            '    element.Value.TextureA.Dispose()
            '    element.Value.TextureB.Dispose()
            'Next
        End Sub

        Public Function GetTileSetBoundaries() As Rectangle
            Dim max As Vector2 = Vector2.Zero
            For Each e In Map
                If e.Key.X > max.X Then max.X = e.Key.X
                If e.Key.Y > max.Y Then max.Y = e.Key.Y
            Next
            Dim con As Vector2 = GetMapTilePosition(max, Vector2.Zero)
            Return New Rectangle(0, 1080 - con.Y, con.X + cTileSize / 2, con.Y + cTileSize / 2)
        End Function


        Public Function GetMapTileAtPoint(ByVal point As Vector2) As Vector2
            Return New Vector2(CInt(Math.Truncate((point.X + cTileSize / 2.0F) / CSng(cTileSize))), CInt(Math.Truncate((point.Y + cTileSize / 2.0F) / CSng(cTileSize))))
        End Function
        Public Function GetMapTileAtPoint(ByVal point As Vector2, switch As Boolean) As Vector2
            Return New Vector2(CInt(Math.Truncate((point.X) / CSng(cTileSize))), CInt(Math.Truncate((point.Y + cTileSize) / CSng(cTileSize))))
        End Function
        Public Shared Function GetMapTileAtPoint(ByVal point As Vector2, Position As Vector2) As Vector2
            Return New Vector2(CInt(Math.Truncate((point.X - Position.X + cTileSize / 2.0F) / CSng(cTileSize))), CInt(Math.Truncate((point.Y - Position.Y + cTileSize / 2.0F) / CSng(cTileSize))))
        End Function

        Public Shared Function GetMapTilePosition(ByVal tileCoords As Vector2, Position As Vector2) As Vector2
            Return New Vector2(CSng(tileCoords.X * cTileSize) + Position.X, CSng(tileCoords.Y * cTileSize) + Position.Y)
        End Function

        Public Function GetMapTileYAtPoint(ByVal y As Single, Optional tru As Boolean = False) As Integer
            Return CInt(Math.Truncate((y + cTileSize / 2.0F) / CSng(cTileSize)))
        End Function

        Public Function GetMapTileXAtPoint(ByVal x As Single) As Integer
            Return CInt(Math.Truncate(x / CSng(cTileSize)))
        End Function

        Public Function GetMapTilePosition(ByVal tileIndexX As Integer, ByVal tileIndexY As Integer, Optional RightOrientation As Boolean = False) As Vector2
            If RightOrientation Then
                Return New Vector2(CSng(tileIndexX * cTileSize), 1080 - CSng(tileIndexY * cTileSize))
            Else
                Return New Vector2(CSng(tileIndexX * cTileSize), CSng(tileIndexY * cTileSize))
            End If
        End Function

        Public Function GetMapTilePosition(ByVal tileCoords As Vector2) As Vector2
            Return New Vector2(CSng(tileCoords.X * cTileSize), CSng(tileCoords.Y * cTileSize))
        End Function

        Public Function GetTile(ByVal x As Integer, ByVal y As Integer) As Tile
            If x < 0 OrElse x >= Map.Size.X OrElse y < 0 OrElse y >= Map.Size.Y Then
                Return Nothing
            End If

            If Map.ContainsKey(New Vector2(x, y)) AndAlso Map(New Vector2(x, y)) > 0 Then Return Map.Tiles(Map(New Vector2(x, y)))
            Return Nothing
        End Function

        Public Function GetTile(ByVal cor As Vector2) As Tile
            If cor.X < 0 OrElse cor.X >= Map.Size.X OrElse cor.Y < 0 OrElse cor.Y >= Map.Size.Y Then
                Return Nothing
            End If

            If Map.ContainsKey(New Vector2(cor.X, cor.Y)) AndAlso Map(New Vector2(cor.X, cor.Y)) > 0 Then Return Map.Tiles(Map(New Vector2(cor.X, cor.Y)))
            Return Nothing
        End Function

        Public Function IsObstacle(ByVal x As Integer, ByVal y As Integer) As Boolean
            Return Map.ContainsKey(New Vector2(x, y)) AndAlso Map(New Vector2(x, y)) > 0 AndAlso (Map.Tiles(Map(New Vector2(x, y))).Collision = TileCollisionType.Solid)
        End Function

        Public Function CanWallJump(ByVal vec As Vector2) As Boolean
            Return Map.ContainsKey(New Vector2(vec.X, vec.Y)) AndAlso Map(New Vector2(vec.X, vec.Y)) > 0 AndAlso (Map.Tiles(Map(New Vector2(vec.X, vec.Y))).Collision = TileCollisionType.Solid) AndAlso Not Map.Tiles(Map(vec)).DisableWallJump
        End Function

        Public Function IsObstacle(ByVal vec As Vector2) As Boolean
            Return Map.ContainsKey(New Vector2(vec.X, vec.Y)) AndAlso Map(New Vector2(vec.X, vec.Y)) > 0 AndAlso (Map.Tiles(Map(New Vector2(vec.X, vec.Y))).Collision = TileCollisionType.Solid)
        End Function

        Public Function IsGround(ByVal x As Integer, ByVal y As Integer) As Boolean
            Return (Map.ContainsKey(New Vector2(x, y)) AndAlso Map(New Vector2(x, y)) > 0 AndAlso (Map.Tiles(Map(New Vector2(x, y))).Collision = TileCollisionType.Solid Or Map.Tiles(Map(New Vector2(x, y))).Collision = TileCollisionType.TopSolid))
        End Function

        Public Function IsOneWayPlatform(ByVal x As Integer, ByVal y As Integer) As Boolean
            Return Map.ContainsKey(New Vector2(x, y)) AndAlso Map(New Vector2(x, y)) > 0 AndAlso (Map.Tiles(Map(New Vector2(x, y))).Collision = TileCollisionType.HalfSolidFloor)
        End Function

        Public Function IsSlope(ByVal x As Integer, ByVal y As Integer) As Boolean
            Return Map.ContainsKey(New Vector2(x, y)) AndAlso Map(New Vector2(x, y)) > 0 AndAlso Map.Tiles(Map(New Vector2(x, y))).IsSlope
        End Function

        Public Function IsEmpty(ByVal x As Integer, ByVal y As Integer) As Boolean
            If x < 0 OrElse x >= Map.Size.X OrElse y < 0 OrElse y >= Map.Size.Y Then
                Return False
            End If

            Return Map(New Vector2(x, y)) = 0 OrElse (Map.Tiles(Map(New Vector2(x, y))).Collision = TileCollisionType.None)
        End Function

        Public Function GetTileRectangle(tpos As Vector2) As Rectangle
            Dim tv As Vector2 = GetMapTilePosition(tpos.X, tpos.Y)
            Return New Rectangle(tv.X, 1080 - tv.Y, cTileSize, cTileSize)
        End Function

        Public Function GetTileRectangleB(tpos As Vector2) As Rectangle
            Dim tv As Vector2 = GetMapTilePosition(tpos.X, tpos.Y)
            Return New Rectangle(tv.X, tv.Y, cTileSize, cTileSize)
        End Function


        Public Function ContainsEntity(key As Integer) As Boolean
            For Each element In Entities
                If element IsNot Nothing AndAlso element.mAttributes.UniqueID = key Then Return True
            Next
            Return False
        End Function

    End Class

End Namespace