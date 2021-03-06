Imports Blali.Framework.UI
Imports Blali.Games.Blali_1.Remastered.Mobs
Imports Blali.Games.Blali_1.Remastered.Objects
Imports Blali.Games.Blali_1.Remastered.Viks
Imports Microsoft.Xna.Framework.Audio
Imports Microsoft.Xna.Framework.Media

Namespace Games.Blali_1.Remastered
    Public Class GameScene
        Inherits Scene

        Public Shared Score As Integer = 0

        Public PlayerComponent As IVik
        Public Map As Tiled.TmxMap
        Public MapRenderer As TiledMapRenderer
        Private new_lvl As Boolean
        Private StartMessagesTriggered As Boolean
        Public Shared SFX As SoundBank
        Public Shared Current As Integer = -1

        'HUD
        Private WithEvents HUD As GuiSystem
        Private WithEvents HUD_ScoreLabel As Controls.Label

        Public Sub New(lvl_ID As Integer)
            Map = Content.LoadTiledMap("levels\Blali_1\" & lvl_ID.ToString & ".tmx")
            new_lvl = lvl_ID <> Current
            StartMessagesTriggered = False
            Current = lvl_ID

            'Load map renderer
            MapRenderer = CreateEntity("map").AddComponent(New TiledMapRenderer(Map, "Collision"))
            MapRenderer.SetRenderLayer(3)

            'Test bg
            Dim bgprp = Map.GetObjectGroup("BG").Properties
            CreateEntity("BG").AddComponent(New Framework.Graphics.BackgroundSprite(Content.LoadTexture("game/Blali_1/" & bgprp("tex")), New Rectangle(50, 50, 1920, 1080), Vector2.One * 0.1) With {.Origin = New Vector2(0, 2400), .LoopHorizontal = Framework.Graphics.BackgroundSprite.LoopMode.FillReverse, .LoopVertical = Framework.Graphics.BackgroundSprite.LoopMode.FillReverse}).SetRenderLayer(4)

            'Generate clouds
            Dim clouds = CreateEntity("clouds")
            For i As Integer = 0 To 40
                clouds.AddComponent(New Cloud(New Vector2(0, 2400)))
            Next

            'Load HUD
            HUD = New GuiSystem()
            HUD_ScoreLabel = New Controls.Label(Function() "Score: " & Score.ToString, New Vector2(50, 1005)) With {.Font = New NezSpriteFont(Content.Load(Of SpriteFont)("font/InstructionText")), .Color = Color.BlanchedAlmond} : HUD.Controls.Add(HUD_ScoreLabel)
            CreateEntity("HUD").AddComponent(HUD).SetRenderLayer(-3)
            HUD.Color = Color.White

            'Load sound bank
            Dim xact_prj As New AudioEngine("assets\game\Blali_1\sfx\win\sfx.xgs")
            Dim wave_bank As New WaveBank(xact_prj, "assets\game\Blali_1\sfx\win\classic_1.xwb")
            SFX = New SoundBank(xact_prj, "assets\game\Blali_1\sfx\win\classic_1.xsb")

            'Get mobs from level
            For Each element In Map.GetObjectGroup("Objects").Objects
                Select Case element.Type
                    Case "player"
                        Select Case element.Properties("type")
                            Case "ball"
                                PlayerComponent = CreateEntity("player").AddComponent(New BallVik(Map))
                            Case Else
                                PlayerComponent = CreateEntity("player").AddComponent(New VollVik(Map))

                        End Select
                        GameObject.Player = PlayerComponent
                    Case "jeff"
                        CreateEntity("jeff").AddComponent(New MynameisJeff(New Vector2(element.X, element.Y), Map))
                    Case "gun"
                        CreateEntity("gun_" & element.Id).AddComponent(New Gun(New Vector2(element.X, element.Y)))
                    Case "canon"
                        CreateEntity("canon_" & element.Id).AddComponent(New Canon(New Vector2(element.X, element.Y), Map))
                    Case "mob_spike"
                        CreateEntity(element.Name).AddComponent(New Spike(New Vector2(element.X, element.Y), Map))
                    Case "mob_flight"
                        CreateEntity(element.Name).AddComponent(New Fliegviech(New Vector2(element.X, element.Y), Map))
                    Case "mob_floor"
                        CreateEntity(element.Name).AddComponent(New Bodenviech(New Vector2(element.X, element.Y), Map))
                End Select
            Next

            'Play music
            Dim song As Song = If(PlayerComponent.NextID < 0, Core.Content.Load(Of Song)("game/Blali_1/bgm/" & lvl_ID), Content.Load(Of Song)("game/Blali_1/bgm/" & lvl_ID))
            MediaPlayer.Play(song)
            MediaPlayer.Volume = 0.4

            'Get collectibles
            For Each element In Map.GetObjectGroup("Collectibles").Objects
                Select Case element.Type
                    Case "yellow_coin"
                        CreateEntity("coin_yellow_" & element.Id).AddComponent(New YellowCoin(New Vector2(element.X, element.Y)))
                    Case "red_coin"
                        CreateEntity("coin_red_" & element.Id).AddComponent(New RedCoin(New Vector2(element.X, element.Y)))
                End Select
            Next

            'Init ppfx
            AddPostProcessor(New ColorGradePostProcessor(0) With {.LUT = Content.LoadTexture("game/Blali_1/lut/" & lvl_ID.ToString)})
            AddPostProcessor(New QualityBloomPostProcessor(1) With {.BloomPreset = QualityBloomPostProcessor.BloomPresets.SuperWide, .BloomStrengthMultiplier = 0.5F, .BloomThreshold = 0.7F})

            'Init misc things
            YellowCoin.CollectedCount = 0
            ClearColor = Color.Transparent
        End Sub

        Public Overrides Sub Initialize()
            MyBase.Initialize()

            AddRenderer(New DefaultRenderer)
        End Sub
    End Class
End Namespace