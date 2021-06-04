Imports Nez_template.Games.Blali_1.Mobs

Namespace Games.Blali_1
    Public Class GameScene
        Inherits Scene

        Public PlayerComponent As PlayerMover
        Public Map As Tiled.TmxMap
        Public MapRenderer As TiledMapRenderer

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            AddRenderer(New DefaultRenderer)

            Map = Content.LoadTiledMap("levels\Blali_1\0.tmx")
            MapRenderer = CreateEntity("map").AddComponent(New TiledMapRenderer(Map, "Collision"))
            PlayerComponent = CreateEntity("player").AddComponent(New PlayerMover(Map))

            'Get enemies from level
            For Each element In Map.GetObjectGroup("Objects").Objects
                Select Case element.Type
                    Case "mob_spike"
                        CreateEntity(element.Name).AddComponent(New Spike(New Vector2(element.X, element.Y), Map) With {.Player = PlayerComponent})
                    Case "mob_flight"
                        CreateEntity(element.Name).AddComponent(New Fliegviech(New Vector2(element.X, element.Y), Map) With {.Player = PlayerComponent})
                End Select
            Next

            'Create camera
            Dim saas As New ScreenSpaceCamera
            Camera.AddComponent(New FollowCamera(PlayerComponent.Entity, FollowCamera.CameraStyle.CameraWindow) With {.FollowLerp = 0.3, .MapLockEnabled = True, .MapSize = New Vector2(Map.Width * 16, Map.Height * 16)})
            Camera.Position = New Vector2(CInt(Map.Properties("camX")) * Map.TileWidth, CInt(Map.Properties("camY")) * Map.TileHeight)

            'Create Camera Button
            'BtnCamera = New VirtualJoystick(True, New VirtualJoystick.GamePadRightStick, New VirtualJoystick.KeyboardKeys(VirtualJoystick.OverlapBehavior.CancelOut, Keys.Left, Keys.Right, Keys.Up, Keys.Down))
        End Sub

        Public Overrides Sub Update()
            MyBase.Update()

            'Move camera
        End Sub
    End Class
End Namespace