Imports Microsoft.Xna.Framework.Audio
Imports Microsoft.Xna.Framework.Media
Imports Blali.Games.Blali_1.Collectibles
Imports Blali.Games.Blali_1.Mobs

Namespace Games.Blali_1
    Public Class GameScene
        Inherits Scene

        Public Shared Score As Integer = 0

        Public PlayerComponent As PlayerMover
        Public Map As Tiled.TmxMap
        Public MapRenderer As TiledMapRenderer
        Public Shared SFX As SoundBank
        Public Shared Current As Integer

        Public Sub New(lvl_ID As Integer)
            Map = Content.LoadTiledMap("levels\Blali_1\" & lvl_ID.ToString & ".tmx")
            Current = lvl_ID

            MapRenderer = CreateEntity("map").AddComponent(New TiledMapRenderer(Map, "Collision"))
            PlayerComponent = CreateEntity("player").AddComponent(New PlayerMover(Map))

            'Load BG
            Dim bgprp = Map.GetObjectGroup("BG").Properties
            CreateEntity("BG").SetScale(CSng(bgprp("scale"))).SetLocalPosition(New Vector2(CSng(bgprp("posX")), CSng(bgprp("posY")))).AddComponent(New Sprites.SpriteRenderer(Content.LoadTexture("game/Blali_1/" & bgprp("tex")))).SetLayerDepth(2)

            'Load sound bank
            Dim xact_prj As New AudioEngine("assets\game\Blali_1\sfx\win\sfx.xgs")
            Dim wave_bank As New WaveBank(xact_prj, "assets\game\Blali_1\sfx\win\classic_1.xwb")
            SFX = New SoundBank(xact_prj, "assets\game\Blali_1\sfx\win\classic_1.xsb")

            'Play music
            MediaPlayer.Play(Content.Load(Of Song)("game/Blali_1/bgm/" & lvl_ID))
            MediaPlayer.Volume = 0.2

            'Get mobs from level
            For Each element In Map.GetObjectGroup("Objects").Objects
                Select Case element.Type
                    Case "mob_spike"
                        CreateEntity(element.Name).AddComponent(New Spike(New Vector2(element.X, element.Y), Map, PlayerComponent))
                    Case "mob_flight"
                        CreateEntity(element.Name).AddComponent(New Fliegviech(New Vector2(element.X, element.Y), Map, PlayerComponent))
                    Case "mob_floor"
                        CreateEntity(element.Name).AddComponent(New Bodenviech(New Vector2(element.X, element.Y), Map, PlayerComponent))
                End Select
            Next

            'Get collectibles
            For Each element In Map.GetObjectGroup("Collectibles").Objects
                Select Case element.Type
                    Case "yellow_coin"
                        CreateEntity("coin_yellow_" & element.Id).AddComponent(New YellowCoin(New Vector2(element.X, element.Y), PlayerComponent))
                    Case "red_coin"
                        CreateEntity("coin_red_" & element.Id).AddComponent(New RedCoin(New Vector2(element.X, element.Y), PlayerComponent))
                End Select
            Next

            'Create camera
            Dim saas = Camera.AddComponent(New FollowCamera(PlayerComponent.Entity, FollowCamera.CameraStyle.CameraWindow) With {.FollowLerp = 0.3, .MapLockEnabled = True, .MapSize = New Vector2(Map.Width * 16, Map.Height * 16)})
            saas.FocusOffset = New Vector2(270, 100)
            Camera.Position = New Vector2(CInt(Map.Properties("camX")) * Map.TileWidth, CInt(Map.Properties("camY")) * Map.TileHeight)
            Camera.Zoom = 0.2
        End Sub

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            AddRenderer(New DefaultRenderer)
        End Sub

        Public Overrides Sub Update()
            MyBase.Update()

            'Move camera
        End Sub
    End Class
End Namespace