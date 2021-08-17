Imports Emmond.Framework.Entities
Imports Emmond.Framework.Tweening
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace Framework.Level.ObjectManager.Objects

    <TestState(TestState.WorkInProgress)>
    Public Class SublevelDoor
        Inherits LevelObject

        Friend Delegate Function SublevelLoader(lvlnr As Integer) As Level
        Friend Property SublvlLoad As SublevelLoader
        Public Property SublvlDestination As Integer = 0

        Dim lvl As Level
        Dim texture As Texture2D

        Public Overrides ReadOnly Property Type As Integer
            Get
                Return 3
            End Get
        End Property

        Public Overrides Sub Draw(gameTime As GameTime)
            SpriteBat.Draw(lvl.ItemMan.Textures(5), New Rectangle(CenterLocation.X - (Size.X / 2), CenterLocation.Y - (Size.Y / 2), Size.X, Size.Y), Nothing, New Color(255, 255, 255, Alpha), 0, Vector2.Zero, SpriteEffects.None, 0.1)
        End Sub

        Sub New(lvlS As Level, LocationS As Vector2)
            CenterLocation = New Vector2(LocationS.X, LocationS.Y)
            Size = New Vector2(64, 128)
        End Sub

        Public Overrides Sub Update(gameTime As GameTime, act As Entity)

        End Sub

        Public Overrides Sub Activate(MultiShader As Effect)
            'Save the player & the map renderer from destruction
            Dim pl As IG.Player = CType(lvl.Entities(0), IG.Player)
            Dim rend As Graphics.MapRenderer = lvl.MapRenderer
            pl.StopWorking = True

            MultiShader.Parameters.Item(3).SetValue(0.0F)
            Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Linear(750), 100.0F, 0F, MultiShader, 2, Sub()

                                                                                                                               'Load new sublevel
                                                                                                                               lvl = SublvlLoad.Invoke(SublvlDestination)
                                                                                                                               lvl.Entities(0) = pl
                                                                                                                               lvl.MapRenderer = rend

                                                                                                                               'Prepare player & Reset Map Renderer
                                                                                                                               pl.mSpawn = lvl.Map.Spawn
                                                                                                                               pl.mPosition = Level.GetMapTilePosition(pl.mSpawn, New Vector2(0, 0)) + New Vector2(0, cPlayerVerticalSpawnOffset)
                                                                                                                               pl.StopWorking = False
                                                                                                                               pl.PlayerState = IG.PlayerStatus.Idle
                                                                                                                               rend.Init(lvl)
                                                                                                                               MultiShader.Parameters.Item(5).SetValue(lvl.Background.skybox)


                                                                                                                               Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Linear(750), 0F, 100.0F, MultiShader, 2, Nothing))
                                                                                                                           End Sub))


            'lvl.MSFX.SFXSoundBank.PlayCue("switch")
        End Sub

        Public Overrides Sub Init(lvlS As Level)
            lvl = lvlS
            ActivationMode = ObjectActivationType.Interact
            ExecutionType = ExecType.OncePerTouch
            ReleasePoints = 0

            'Load texture
            If lvlS IsNot Nothing Then
                If Not lvlS.ItemMan.Textures.ContainsKey(5) Then lvlS.ItemMan.Textures.Add(5, ContentMan.Load(Of Texture2D)("obj\3\base"))
                texture = lvlS.ItemMan.Textures(5)
            Else
                texture = ContentMan.Load(Of Texture2D)("obj\3\base")
            End If
        End Sub
    End Class
End Namespace