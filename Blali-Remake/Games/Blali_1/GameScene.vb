Namespace Games.Blali_1
    Public Class GameScene
        Inherits Scene

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            AddRenderer(New DefaultRenderer)

            Dim map As Tiled.TmxMap = Content.LoadTiledMap("levels\Blali_1\0.tmx")
            CreateEntity("map").AddComponent(New TiledMapRenderer(map, "Collision"))
            CreateEntity("player").AddComponent(New PlayerMover(map))

            Camera.Position = New Vector2(CInt(map.Properties("camX")) * map.TileWidth, CInt(map.Properties("camY")) * map.TileHeight)
        End Sub
    End Class
End Namespace