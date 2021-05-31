Namespace Games.Blali_1
    Public Class GameScene
        Inherits Scene

        Public PlayerComponent As PlayerMover
        Public Map As Tiled.TmxMap
        Public MapRenderer As TiledMapRenderer
        Public CamMover As FollowCamera


        Dim BtnCamera As VirtualJoystick
        Public Overrides Sub Initialize()
            MyBase.Initialize()

            AddRenderer(New DefaultRenderer)

            Map = Content.LoadTiledMap("levels\Blali_1\0.tmx")
            MapRenderer = CreateEntity("map").AddComponent(New TiledMapRenderer(Map, "Collision"))
            PlayerComponent = CreateEntity("player").AddComponent(New PlayerMover(Map))

            'Create camera
            Dim saas As New ScreenSpaceCamera
            CamMover = Camera.AddComponent(New FollowCamera(PlayerComponent.Entity, FollowCamera.CameraStyle.CameraWindow) With {.FollowLerp = 0.3, .MapLockEnabled = True, .MapSize = New Vector2(Map.Width * 16, Map.Height * 16)})
            Camera.Position = New Vector2(CInt(Map.Properties("camX")) * Map.TileWidth, CInt(Map.Properties("camY")) * Map.TileHeight)

            'Create Camera Button
            BtnCamera = New VirtualJoystick(True, New VirtualJoystick.GamePadRightStick, New VirtualJoystick.KeyboardKeys(VirtualJoystick.OverlapBehavior.CancelOut, Keys.Left, Keys.Right, Keys.Up, Keys.Down))
        End Sub

        Public Overrides Sub Update()
            MyBase.Update()

            'Move camera
            'CamMover.DesiredPositionDelta += BtnCamera.Value
        End Sub
    End Class
End Namespace