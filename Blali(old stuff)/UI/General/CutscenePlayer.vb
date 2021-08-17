Imports Emmond.Framework.Cutscenes.TypeA
Imports Emmond.Framework.Input
Imports Emmond.Framework.SceneManager
Imports Emmond.Framework.Tweening
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Media

Namespace UI.General
    Public Class CutscenePlayer
        Inherits Scene

        Private Cutscene As CutsceneTypeA
        Private activeframe As Integer
        Private launched As Boolean = False
        Private cammov As Transition(Of CamKeyframe)
        Public Property FinalAction As SceneSwitch = SceneSwitch.None

        Public Overrides Sub Initialize()
            Config = New SceneConfig With {.ID = 6, .ReloadOnSelection = True, .AutoLoadSoundBank = False, .Descriptor = "cut", .ShowLoadingScreen = False, .ShowLSCustom = False}
        End Sub

        Public Overrides Sub LoadContent(Optional parameter As Integer = 0)
            Cutscene = CutsceneTypeA.Load(ContentMan.RootDirectory & "\mps\" & parameter & ".cut")
            Cutscene.Init()

        End Sub

        Public Overrides Sub UnloadContent()

        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)
            Cutscene.Start(False)
            Cutscene.Draw(gameTime)
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
            End If

            Cutscene.CamPos = cammov.Value
            Cutscene.Update(gameTime)
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
                    Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Deceleration(saas.FadeTime), 0, 100, Cutscene.Effect, 0, Sub()
                                                                                                                                                       Automator.Add(New Transition(Of Single)(New TransitionTypes.TransitionType_Linear(saas.WaitTime - (2 * saas.FadeTime)), 0, 100,
                                                                                                                                                     Sub()
                                                                                                                                                         Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Deceleration(saas.FadeTime), 100, 0, Cutscene.Effect, 0, Nothing))
                                                                                                                                                     End Sub))
                                                                                                                                                   End Sub))
                Else
                    Cutscene.Effect.Parameters.Item(0).SetValue(100.0F)
                End If
                Cutscene.CamPos = saas.StartPos
                Automator.Add(cammov)
            Else
                StopCutscene()
            End If
        End Sub

        Private Sub StopCutscene()
            Cutscene.Effect.Parameters.Item(0).SetValue(100.0F)
            Automator.Clear()
            MediaPlayer.Stop()
            SceneMan.ChangeToScene(FinalAction)
        End Sub
    End Class

End Namespace