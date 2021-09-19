Imports Emmond.Framework.Level.ObjectManager.Objects
Imports Emmond.Framework.SceneManager
Imports Emmond.Framework.Tweening
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input

Namespace UI.General
    Public Class LoadingScreen
        Inherits Scene

        Public EnableExtendedDisplay As Boolean = False
        Public LoadTitle As String
        Public LoadDescription As String
        Public ParalHorOffset As Double = 0

        Public ShowScreen As Boolean = False
        Public ShowDirectScreen As Boolean = False
        Public Done As Boolean = False 'Indicates whether loading of scene was succesful
        Public Sleeper As Boolean = False 'Indicates whether the intro animation is finished and the scene can be loaded
        Dim PreSleeper As Boolean = False
        Public SleeperB As Boolean = False 'Indicates whether the outro animation has begun
        Public SleeperC As Boolean = False 'Indicates whether the outro animation is finished and the scene can be displayed
        Public PlayerColor As Color = Color.White
        Dim secondarycolor As Color
        Public Disable As Boolean = False 'Disables the effect(error handler)
        Dim max As Integer = GameSize.X + (GameSize.X * 1.4)
        Dim TextFade As Double = 1
        Friend changevolume As Boolean = False

        'Assets
        Dim BrightShader As Effect
        Dim efx As ShaderTransition
        Dim efxB As Transition(Of Single)
        Dim Parallel As Texture2D
        Dim titleFnt As SpriteFont
        Dim titlesubFnt As SpriteFont

        Sub New()
            Config = New SceneConfig With {.ID = 36, .ReloadOnSelection = False, .AutoLoadSoundBank = False, .Descriptor = "lda", .ShowLoadingScreen = False}
        End Sub

        Public Overrides Sub Initialize()
            If Not ShowDirectScreen Then

                SceneMan.NoInput = True
                Done = False
                Sleeper = False
                SleeperB = False
                SleeperC = False
                TextFade = 1
                If EnableExtendedDisplay Then
                    ShowScreen = True
                    efx = New ShaderTransition(New TransitionTypes.TransitionType_EaseInEaseOut(cLSFadeSpeed), 0F, 100.0F, BrightShader, 0, Nothing)
                    efxB = New Transition(Of Single)(New TransitionTypes.TransitionType_CriticalDamping(cLSParalSpeed), 0F, cLSParalPause, Sub()
                                                                                                                                               PreSleeper = True
                                                                                                                                           End Sub)
                    Automator.Add(efx)
                    Automator.Add(efxB)
                Else
                    ShowScreen = True
                    efx = New ShaderTransition(New TransitionTypes.TransitionType_EaseInEaseOut(cLSFadeSpeed), 0F, 100.0F, BrightShader, 0, Sub()
                                                                                                                                                PreSleeper = True
                                                                                                                                            End Sub)
                    Automator.Add(efx)
                    efxB = New Transition(Of Single)
                End If

            Else
                efxB = New Transition(Of Single) With {.Value = cLSParalPause}
                ShowScreen = True
            End If

            secondarycolor = New Color(PlayerColor * 0.35, 255)
        End Sub

        Public Overrides Sub LoadContent(Optional parameter As Integer = 0)
            BrightShader = ContentMan.Load(Of Effect)("fx\fx_alphablend")
            Parallel = ContentMan.Load(Of Texture2D)("ge_paral")
            titleFnt = ContentMan.Load(Of SpriteFont)("font\fnt_title")
            titlesubFnt = ContentMan.Load(Of SpriteFont)("font\fnt_titlesub")

            'LoadingIcon = {New EnergyParticle(GameSize - New Vector2(220, 200), True) With {.Color = Color.Lime},
            '                   New EnergyParticle(GameSize - New Vector2(220, 200), True) With {.Color = Color.Cyan},
            '                   New EnergyParticle(GameSize - New Vector2(220, 200), True) With {.Color = Color.Purple}}

            'For Each itm In LoadingIcon
            '    itm.Init(Nothing)
            'Next
        End Sub

        Public Overrides Sub UnloadContent()

        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)
            If ShowDirectScreen Then
                SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, Nothing, MainScalingMatrix)
                DrawBasicCore()
                If EnableExtendedDisplay Then DrawExtendedCore()
                SpriteBat.End()
            Else
                If Not Disable AndAlso efx IsNot Nothing And Not SleeperC Then
                    SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, BrightShader, MainScalingMatrix)
                    DrawBasicCore()
                    If Sleeper Then
                        SpriteBat.End()
                        SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, Nothing, MainScalingMatrix)
                    End If
                    If EnableExtendedDisplay Then DrawExtendedCore()
                    SpriteBat.End()
                End If
            End If
        End Sub

        Private Sub DrawBasicCore()
            SpriteBat.Draw(ReferencePixel, New Rectangle(Point.Zero, GameSize.ToPoint), New Color(0F, 0F, 0F, 1))
            If Not EnableExtendedDisplay Then SpriteBat.DrawString(titlesubFnt, "Loading...", GameSize - New Vector2(280, 120), Color.White)
        End Sub

        Private Sub DrawExtendedCore()
            SpriteBat.Draw(Parallel, New Rectangle((-GameSize.X * 1.2) + (max * efxB.Value / 1000), -GameSize.Y / 2 - 200, GameSize.X * 1.2, GameSize.Y), secondarycolor)
            SpriteBat.Draw(Parallel, New Rectangle(GameSize.X - (max * efxB.Value / 1000), GameSize.Y / 2 + 200, GameSize.X * 1.2, GameSize.Y), PlayerColor)
            SpriteBat.DrawString(titleFnt, LoadTitle, New Vector2((GameSize.X / 2) - (titleFnt.MeasureString(LoadTitle).X / 2), GameSize.Y * 3 / 9), Color.White * TextFade)
            SpriteBat.DrawString(titlesubFnt, LoadDescription, New Vector2((GameSize.X / 2) - (titlesubFnt.MeasureString(LoadDescription).X / 2), GameSize.Y * 5 / 10), Color.White * TextFade)
        End Sub

        Public Overrides Sub Update(gameTime As GameTime)
            If Done And Sleeper And Not SleeperB And Not ShowDirectScreen Then
                SleeperB = True
                SceneMan.NoInput = False
                efx = New ShaderTransition(New TransitionTypes.TransitionType_EaseInEaseOut(cLSFadeSpeed), 100.0F, 0F, BrightShader, 0, Sub()
                                                                                                                                            ShowScreen = False
                                                                                                                                            GenerateNewFade()
                                                                                                                                        End Sub)
                Automator.Add(efx)
            ElseIf ShowDirectScreen Then
                With Keyboard.GetState
                    If .IsKeyDown(Keys.NumPad1) Then
                        EnableExtendedDisplay = False
                    ElseIf .IsKeyDown(Keys.NumPad2) Then
                        EnableExtendedDisplay = True
                        LoadTitle = "He-.... hewwwo?"
                        LoadDescription = "OwO, what's this?"
                        EnableExtendedDisplay = True
                    End If
                End With
            End If



            If SleeperB Then
                TextFade = 1 - ((efxB.Value - cLSParalPause) / 400)
            End If
            ParalHorOffset += 0.05

            If changevolume Then
                Media.MediaPlayer.Volume = (SettingsMan.Value(31) / 100) * (1 - BrightShader.Parameters(0).GetValueSingle / 100)
                If BrightShader.Parameters(0).GetValueSingle >= 100 Then
                    'Set Media Player Volume
                    changevolume = False
                    Media.MediaPlayer.Stop()
                    Media.MediaPlayer.Volume = (SettingsMan.Value(31) / 100)
                End If
            End If

            If PreSleeper And Not changevolume Then
                Sleeper = True
            End If
        End Sub

        Sub GenerateNewFade()
            If EnableExtendedDisplay Then
                efxB = New Transition(Of Single)(New TransitionTypes.TransitionType_Acceleration(cLSParalSpeed), cLSParalPause, 1000.0F, Sub()
                                                                                                                                             SleeperC = True
                                                                                                                                         End Sub)
                Automator.Add(efxB)
            Else
                SleeperC = True
            End If
        End Sub

        Public Overrides Sub DrawDebug(gameTime As GameTime)
        End Sub
    End Class
End Namespace