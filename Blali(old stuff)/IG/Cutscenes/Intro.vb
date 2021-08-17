Imports Emmond.Framework.Cutscenes.TypeA
Imports Emmond.Framework.Graphics
Imports Emmond.Framework.Input
Imports Emmond.Framework.SceneManager
Imports Emmond.Framework.Tweening
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Media

Namespace IG.Cutscenes

    Public Class LFIntro
        Inherits Scene

        Private bgm As Song
        Private Cutscene As CutsceneTypeA
        Private activeframe As Integer
        Private launched As Boolean = False
        Private cammov As Transition(Of CamKeyframe)
        Public Property FinalAction As SceneSwitch = SceneSwitch.None

        'Per cutscene flags
        Dim bgfader As Transition(Of Single)
        Dim bginit As Boolean = False
        Dim bloomfader As Transition(Of Single)
        Dim doon As Boolean = False

        Dim bright As Effect
        Public ffx As PostProcessing.BloomFilter
        Public TempTargetA As RenderTarget2D

        Public Overrides Sub Initialize()
            Config = New SceneConfig With {.ID = 1, .ReloadOnSelection = True, .AutoLoadSoundBank = False, .Descriptor = "lfi", .ShowLoadingScreen = False, .ShowLSCustom = False}
        End Sub

        Public Overrides Sub LoadContent(Optional parameter As Integer = 0)
            Cutscene = CutsceneTypeA.Load(ContentMan.RootDirectory & "\mps\lfi.cut")
            Cutscene.Init()

            bgfader = New Transition(Of Single) With {.Value = 1}
            bloomfader = New Transition(Of Single) With {.Value = 0}

            TempTargetA = New RenderTarget2D(
                    Graphx.GraphicsDevice,
                    BufferSize.X,
                    BufferSize.Y,
                    False,
                    Graphx.GraphicsDevice.PresentationParameters.BackBufferFormat,
                    DepthFormat.Depth24Stencil8)

            ffx = New PostProcessing.BloomFilter
            ffx.Load(Graphx.GraphicsDevice, ContentMan, GameSize.X, GameSize.Y)
            ffx.BloomPreset = PostProcessing.BloomFilter.BloomPresets.Wide
            ffx.BloomThreshold = 0
            ffx.BloomStrengthMultiplier = 1

            bright = ContentMan.Load(Of Effect)("fx\fx_bright")
            bright.Parameters.Item(1).SetValue(0F)
            bright.Parameters.Item(0).SetValue(100.0F)
        End Sub

        Public Overrides Sub UnloadContent()

        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)
            Graphx.GraphicsDevice.SetRenderTargets(TempTargetA)
            Graphx.GraphicsDevice.Clear(Color.Black)

            Cutscene.Start(False)
            Cutscene.Draw(gameTime)


            Graphx.GraphicsDevice.SetRenderTarget(Nothing)
            Dim txbloom As Texture2D = ffx.Draw(TempTargetA, GameSize.X, GameSize.Y)

            Graphx.GraphicsDevice.SetRenderTargets(Nothing)
            Graphx.GraphicsDevice.Clear(Color.Black)

            SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.Additive, Nothing, Nothing, Nothing, bright, MainScalingMatrix)
            SpriteBat.Draw(TempTargetA, New Rectangle(0, 0, GameSize.X, GameSize.Y), Color.White)
            SpriteBat.Draw(txbloom, New Rectangle(0, 0, GameSize.X, GameSize.Y), Color.White)
            SpriteBat.End()
        End Sub

        Public Overrides Sub DrawDebug(gameTime As GameTime)

        End Sub

        Dim skip As Boolean = False
        Public Overrides Sub Update(gameTime As GameTime)
            Dim cstate As MenuInputState = InputMan.GetMenuInput
            If (cstate.Confirm Or cstate.Start) And Not skip Then
                skip = True
                StopCutscene()
            End If

            If Not launched Then
                activeframe = -1
                If Cutscene.MusicPath <> "" Then
                    Cutscene.Song = ContentMan.Load(Of Song)(Cutscene.MusicPath)
                    MediaPlayer.Play(Cutscene.Song)
                End If
                PlayCutscene()
                launched = True

                Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_EaseInEaseOut(4000), 0F, 100.0F, Cutscene.Effect, 0, Nothing))
            End If

            Cutscene.CamPos = cammov.Value
            Cutscene.Update(gameTime)

            'Check keyframes
            If cammov.ElapsedTime > 5500 And Not bginit Then
                bgfader = New Transition(Of Single)(New TransitionTypes.TransitionType_EaseInEaseOut(5000), 1, 0, Nothing)
                Automator.Add(bgfader)

                bloomfader = New Transition(Of Single)(New TransitionTypes.TransitionType_CriticalDamping(8000), 0, 1, Nothing)
                Automator.Add(bloomfader)

                bginit = True
            End If

            If cammov.TransitionStater = TransitionState.Done And Not doon Then

                Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Acceleration(3000), 100.0F, 0F, bright, 0, Sub()
                                                                                                                                     StopCutscene()
                                                                                                                                 End Sub))
                doon = True
            End If

            Cutscene.FrameBuffer.Item(0).Item(0).Alpha = bgfader.Value
            Cutscene.FrameBuffer.Item(0).Item(1).Alpha = bgfader.Value
            ffx.BloomStrengthMultiplier = bloomfader.Value
        End Sub

        Private Sub PlayCutscene()
            If activeframe < Cutscene.FrameBuffer.Count - 1 Then
                activeframe += 1
                Cutscene.ActivateFrame(activeframe)

                Dim saas As Frame = Cutscene.FrameBuffer(activeframe)
                Automator.Clear()
                Dim metho As ITransitionType
                Select Case saas.MovementTransition
                    Case TransitionType.Acceleration
                        metho = New TransitionTypes.TransitionType_Acceleration(saas.WaitTime)
                    Case TransitionType.Bounce
                        metho = New TransitionTypes.TransitionType_Bounce(saas.WaitTime)
                    Case TransitionType.CriticalDamping
                        metho = New TransitionTypes.TransitionType_CriticalDamping(saas.WaitTime)
                    Case TransitionType.Deceleration
                        metho = New TransitionTypes.TransitionType_Deceleration(saas.WaitTime)
                    Case TransitionType.EaseInEaseOut
                        metho = New TransitionTypes.TransitionType_EaseInEaseOut(saas.WaitTime)
                    Case TransitionType.ThrowAndCatch
                        metho = New TransitionTypes.TransitionType_ThrowAndCatch(saas.WaitTime)
                    Case Else
                        metho = New TransitionTypes.TransitionType_Linear(saas.WaitTime)
                End Select

                cammov = New Transition(Of CamKeyframe)(metho, saas.StartPos, saas.EndPos, Sub()
                                                                                               PlayCutscene()
                                                                                           End Sub)
                If saas.Blackout Then
                    Cutscene.Effect.Parameters.Item(0).SetValue(0F)
                    Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Deceleration(saas.FadeTime), 0F, 100.0F, Cutscene.Effect, 0, Sub()
                                                                                                                                                           Automator.Add(New Transition(Of Single)(New TransitionTypes.TransitionType_Linear(saas.WaitTime - (2 * saas.FadeTime)), 0, 100,
                                                                                                                                                     Sub()
                                                                                                                                                         Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Deceleration(saas.FadeTime), 100.0F, 0F, Cutscene.Effect, 0, Nothing))
                                                                                                                                                     End Sub))
                                                                                                                                                       End Sub))
                Else
                    Cutscene.Effect.Parameters.Item(0).SetValue(100.0F)
                End If
                Cutscene.CamPos = saas.StartPos
                Automator.Add(cammov)
            End If
        End Sub

        Private Sub StopCutscene()
            Automator.Clear()
            MediaPlayer.Stop()
            SceneMan.ChangeToScene(New SceneSwitch(123))
        End Sub
    End Class
End Namespace
