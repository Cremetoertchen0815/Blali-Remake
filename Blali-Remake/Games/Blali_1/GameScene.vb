Imports Microsoft.Xna.Framework.Audio
Imports Microsoft.Xna.Framework.Media
Imports Blali.Games.Blali_1.Collectibles
Imports Blali.Games.Blali_1.Mobs
Imports Blali.Games.Blali_1.Viks

Namespace Games.Blali_1
    Public Class GameScene
        Inherits Scene

        Public Shared Score As Integer = 0

        Public PlayerComponent As IVik
        Public Map As Tiled.TmxMap
        Public MapRenderer As TiledMapRenderer
        Public Shared SFX As SoundBank
        Public Shared Current As Integer

        Public Sub New(lvl_ID As Integer)
            Map = Content.LoadTiledMap("levels\Blali_1\" & lvl_ID.ToString & ".tmx")
            Current = lvl_ID

            MapRenderer = CreateEntity("map").AddComponent(New TiledMapRenderer(Map, "Collision"))

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
                    Case "player"
                        Select Case element.Properties("type")
                            Case "ball"
                                PlayerComponent = CreateEntity("player").AddComponent(New BallVik(Map))
                            Case Else
                                PlayerComponent = CreateEntity("player").AddComponent(New DefaultVik(Map))
                        End Select
                        GameObject.Player = PlayerComponent
                    Case "mob_spike"
                        CreateEntity(element.Name).AddComponent(New Spike(New Vector2(element.X, element.Y), Map))
                    Case "mob_flight"
                        CreateEntity(element.Name).AddComponent(New Fliegviech(New Vector2(element.X, element.Y), Map))
                    Case "mob_floor"
                        CreateEntity(element.Name).AddComponent(New Bodenviech(New Vector2(element.X, element.Y), Map))
                End Select
            Next

            'Get collectibles
            For Each element In Map.GetObjectGroup("Collectibles").Objects
                Select Case element.Type
                    Case "yellow_coin"
                        CreateEntity("coin_yellow_" & element.Id).AddComponent(New YellowCoin(New Vector2(element.X, element.Y)))
                    Case "red_coin"
                        CreateEntity("coin_red_" & element.Id).AddComponent(New RedCoin(New Vector2(element.X, element.Y)))
                End Select
            Next
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