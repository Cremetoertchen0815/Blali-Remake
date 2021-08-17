Imports System.Collections.Generic
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Graphics

    <TestState(TestState.WorkInProgress)>
    Public Class MapRenderer
        Dim lvl As Level.Level

        Dim Cluster As List(Of ClusterData) 'Contains all screen boundary cluster(Key) and their respective tiles(Value = in tile format)

        Friend Transparency As Byte() = {255, 255, 255}

        Dim dev As GraphicsDevice

        Friend Sub Init(lvlS As Level.Level)
            lvl = lvlS
            dev = StandardAssets.Graphx.GraphicsDevice

            Cluster = New List(Of ClusterData)

            For Each clus In lvl.MapCluster
                Dim rect As New Rectangle(clus.Key.X * GameSize.X, (-clus.Key.Y * GameSize.Y), GameSize.X, GameSize.Y)
                Dim BottomLeftTile As Point = lvl.GetMapTileAtPoint(New Vector2(rect.Left, 1080 - rect.Bottom)).ToPoint
                Dim TopRightTile As Point = lvl.GetMapTileAtPoint(New Vector2(rect.Right, 1080 - rect.Top)).ToPoint

                'Calculate culling
                Dim cc As Boolean() = CalculateClusterCulling(clus.Key, lvlS)

                Dim clustr As New ClusterData With {
                .Screenbounds = rect,
                .Tilebounds = New Rectangle(BottomLeftTile, TopRightTile - BottomLeftTile),
                .TranslationMatrix = Matrix.CreateTranslation(-rect.X, -rect.Y, 0) * MainScalingMatrix,
                .ClusterKey = clus.Key,
                .EnableA = cc(0),
                .EnableB = cc(1),
                .EnableC = cc(2),
                .GeneratedA = False,
                .GeneratedB = False,
                .GeneratedC = False
                }
                If clustr.EnableA Then clustr.LayerA = ClusterData.GetRendertarget(dev)
                If clustr.EnableB Then clustr.LayerB = ClusterData.GetRendertarget(dev)
                If clustr.EnableC Then clustr.LayerC = ClusterData.GetRendertarget(dev)
                Cluster.Add(clustr)
            Next
        End Sub

        Friend Sub Update(gameTime As GameTime)
            For Each c In Cluster
                c.InBounds = lvl.Camera.Viewport.Intersects(c.Screenbounds)
            Next
        End Sub

        Friend Sub InvalidateVisible(layer As Integer, lvl As Level.Level)
            For Each element In Cluster
                If element.InBounds Then
                    Dim cc As Boolean() = CalculateClusterCulling(element.ClusterKey, lvl)
                    element.EnableA = cc(0)
                    element.EnableB = cc(1)
                    element.EnableC = cc(2)
                    If element.EnableA And layer = 0 Then element.GeneratedA = False
                    If element.EnableB And layer = 1 Then element.GeneratedB = False
                    If element.EnableC And layer = 2 Then element.GeneratedC = False
                End If
            Next
        End Sub

        Friend Sub Draw()
            For Each element In Cluster
                If element.InBounds Then
                    If element.EnableA Then SpriteBat.Draw(element.LayerA, element.Screenbounds, Nothing, Color.White * (Transparency(0) / 255), 0, Vector2.Zero, SpriteEffects.None, 0.0)
                    If element.EnableB Then SpriteBat.Draw(element.LayerB, element.Screenbounds, Nothing, Color.White * (Transparency(1) / 255), 0, Vector2.Zero, SpriteEffects.None, 0.01)
                    If element.EnableC Then SpriteBat.Draw(element.LayerC, element.Screenbounds, Nothing, Color.White * (Transparency(2) / 255), 0, Vector2.Zero, SpriteEffects.None, 0.81)
                End If
            Next
        End Sub

        Friend Sub DrawDebug()
            Dim BottomLeftTile As Point = lvl.GetMapTileAtPoint(New Vector2(lvl.Camera.Viewport.Left, 1080 - lvl.Camera.Viewport.Bottom)).ToPoint
            Dim TopRightTile As Point = lvl.GetMapTileAtPoint(New Vector2(lvl.Camera.Viewport.Right, 1080 - lvl.Camera.Viewport.Top)).ToPoint
            DrawMapLtd(New Rectangle(BottomLeftTile, TopRightTile - BottomLeftTile))
        End Sub

        Friend Sub PreDraw()
            'Draw Map to clusters
            For Each element In Cluster

                If element.EnableA And Not element.GeneratedA Then
                    dev.SetRenderTarget(element.LayerA)
                    dev.Clear(Color.Transparent)

                    SpriteBat.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, Nothing, Nothing, Nothing, element.TranslationMatrix)
                    DrawMap(lvl.Map.BackLayer)
                    SpriteBat.End()
                    element.GeneratedA = True
                End If


                If element.EnableB And Not element.GeneratedB Then
                    dev.SetRenderTarget(element.LayerB)
                    dev.Clear(Color.Transparent)

                    SpriteBat.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, Nothing, Nothing, Nothing, element.TranslationMatrix)
                    DrawMap(lvl.Map)
                    SpriteBat.End()
                    element.GeneratedB = True
                End If


                If element.EnableC And Not element.GeneratedC Then
                    dev.SetRenderTarget(element.LayerC)
                    dev.Clear(Color.Transparent)

                    SpriteBat.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, Nothing, Nothing, Nothing, element.TranslationMatrix)
                    DrawMap(lvl.Map.FrontLayer)
                    SpriteBat.End()
                    element.GeneratedC = True
                End If

            Next

        End Sub



        'Draws the TileSet on screen with positions relative to the camera
        Public Sub DrawMap(MapS As Dictionary(Of Vector2, Integer), Optional Depth As Double = 0)
            For Each element In MapS
                If element.Value > 0 Then
                    Dim location As Vector2 = lvl.GetMapTilePosition(element.Key.X, element.Key.Y)
                    SpriteBat.Draw(lvl.Map.TileSpriteSheet, New Rectangle(location.X, GameSize.Y - location.Y, cTileSize, cTileSize), lvl.Map.Tiles(element.Value).OriginRectangle, Color.White, 0, Vector2.Zero, SpriteEffects.None, Depth)
                    SpriteCount += CUInt(1)
                End If
            Next
        End Sub

        'Draws the TileSet on screen with positions relative to the camera
        Public Sub DrawMapLtd(area As Rectangle)
            For x As Integer = area.Left To area.Right
                For y As Integer = area.Top To area.Bottom
                    If lvl.Map.BackLayer.ContainsKey(New Vector2(x, y)) AndAlso lvl.Map.BackLayer(New Vector2(x, y)) > 0 Then
                        Dim val As Integer = lvl.Map.BackLayer(New Vector2(x, y))
                        Dim location As Vector2 = lvl.GetMapTilePosition(x, y)
                        SpriteBat.Draw(lvl.Map.TileSpriteSheet, New Rectangle(location.X, GameSize.Y - location.Y, cTileSize, cTileSize), lvl.Map.Tiles(val).OriginRectangle, Color.White * (Transparency(0) / 255), 0, Vector2.Zero, SpriteEffects.None, 0)
                        SpriteCount += CUInt(1)
                    End If

                    If lvl.Map.ContainsKey(New Vector2(x, y)) AndAlso lvl.Map(New Vector2(x, y)) > 0 Then
                        Dim val As Integer = lvl.Map(New Vector2(x, y))
                        Dim location As Vector2 = lvl.GetMapTilePosition(x, y)
                        SpriteBat.Draw(lvl.Map.TileSpriteSheet, New Rectangle(location.X, GameSize.Y - location.Y, cTileSize, cTileSize), lvl.Map.Tiles(val).OriginRectangle, Color.White * (Transparency(1) / 255), 0, Vector2.Zero, SpriteEffects.None, 0.8)
                        SpriteCount += CUInt(1)
                    End If

                    If lvl.Map.FrontLayer.ContainsKey(New Vector2(x, y)) AndAlso lvl.Map.FrontLayer(New Vector2(x, y)) > 0 Then
                        Dim val As Integer = lvl.Map.FrontLayer(New Vector2(x, y))
                        Dim location As Vector2 = lvl.GetMapTilePosition(x, y)
                        SpriteBat.Draw(lvl.Map.TileSpriteSheet, New Rectangle(location.X, GameSize.Y - location.Y, cTileSize, cTileSize), lvl.Map.Tiles(val).OriginRectangle, Color.White * (Transparency(2) / 255), 0, Vector2.Zero, SpriteEffects.None, 0.81)
                        SpriteCount += CUInt(1)
                    End If
                Next
            Next

        End Sub

        Friend Function CalculateClusterCulling(cluster As Vector2, lvl As Level.Level) As Boolean()
            Dim enabled As Boolean() = {False, False, False}

            Dim ex As Boolean = False
            For x As Integer = cluster.X * (GameSize.X / cTileSize) To (cluster.X + 1) * (GameSize.X / cTileSize) - 1
                For y As Integer = cluster.Y * (GameSize.Y / cTileSize) To (cluster.Y + 1) * (GameSize.Y / cTileSize) - 1

                    Dim ps As New Vector2(x, y)
                    If lvl.Map.BackLayer.ContainsKey(ps) AndAlso lvl.Map.BackLayer(ps) > 0 Then enabled(0) = True
                    If lvl.Map.ContainsKey(ps) AndAlso lvl.Map(ps) > 0 Then enabled(1) = True
                    If lvl.Map.FrontLayer.ContainsKey(ps) AndAlso lvl.Map.FrontLayer(ps) > 0 Then enabled(2) = True

                    If enabled(0) And enabled(1) And enabled(2) Then ex = True : Exit For
                Next
                If ex Then Exit For
            Next

            Return enabled
        End Function
    End Class

    Public Enum RenderingMode
        Live
        ToRenderTarget
    End Enum
End Namespace