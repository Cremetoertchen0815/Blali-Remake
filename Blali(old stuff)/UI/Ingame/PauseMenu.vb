Imports Emmond.Framework.Input
Imports Emmond.Framework.Level
Imports Emmond.Framework.SceneManager
Imports Emmond.Framework.Tweening
Imports Emmond.IG
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input
Imports Microsoft.Xna.Framework.Media

Namespace UI.Ingame
    Public Class PauseMenu
        Inherits Scene

        'Fade in/Fade out/Transitional helper flags
        Public vlass As LevelClass
        Public PauseMode As PauseMode
        Public Active As Boolean = False
        Public hasntstart As Boolean = False
        Public hasntended As Boolean = False

        'Assets
        Dim ZooSettingsi As Transition(Of Single)
        Dim font As SpriteFont
        Dim fontB As SpriteFont
        Dim Fader As Transition(Of Single)
        Dim bg As Texture2D
        Dim lastcstate As MenuInputState
        Dim lastmstate As MouseState
        Dim controlls As Boolean = False 'Flag that indicates, whether the menu is active

        'Root flags
        Dim hideroot As Boolean = False
        Dim selection As Integer = 0
        Dim texts As String() = {"Continue", "Restart", "Backpack", "Exit"}

        'Popup flags
        Public popupActive As Boolean = False 'If the popup is visible
        Public popupOpen As Boolean = False 'If the popup is loaded
        Dim popupFader As Transition(Of Single)
        Dim popup As Integer = 0 'The kind of popup
        Dim popupsize As Vector2 = Vector2.Zero 'The size of the popup instance
        Dim popupsel As Boolean = False 'A flag for two option popup selection
        Dim popuptexts As String() = {"Do you really wanna restart this stage?", "Restart", "Back"} 'The texts for a two option popup selection


        'fast move flags flags
        Dim UpCounter As UInteger = 0
        Dim DownCounter As UInteger = 0
        Dim SkipperVert As UInteger = 0

        Sub New()
            Config = New SceneConfig With {.ID = 37, .ReloadOnSelection = True, .AutoLoadSoundBank = True, .Descriptor = "pau", .ShowLoadingScreen = False, .ShowLSCustom = False}
        End Sub
        Public Overrides Sub Initialize()
            ZooSettingsi = New Transition(Of Single)(New TransitionTypes.TransitionType_EaseInEaseOut(300), 150, 50, Nothing) With {.Repeat = RepeatJob.Reverse}
            Automator.Add(ZooSettingsi)

            PauseMode = PauseMode.Play
            hasntstart = False
            hasntended = False
        End Sub

        Public Overrides Sub LoadContent(Optional parameter As Integer = 0)
            'Assets
            font = ContentMan.Load(Of SpriteFont)("font\fnt_pause")
            fontB = ContentMan.Load(Of SpriteFont)("font\fnt_pausepop")
            bg = ContentMan.Load(Of Texture2D)("ge_paral")

        End Sub

        Public Overrides Sub UnloadContent()

        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)
            SpriteBat.Begin(SpriteSortMode.Immediate, Nothing, SamplerState.AnisotropicClamp, Nothing, Nothing, Nothing, MainScalingMatrix)

            'Draw root
            If Not hideroot Then
                SpriteBat.Draw(bg, New Rectangle(600 - (MenuBorder / 2), (GameSize.Y / 2) - (Fader.Value * 2.5) - (MenuBorder / 2), GameSize.X - (600 * 2) + MenuBorder, (Fader.Value / 100 * (500 + MenuBorder))), Color.DarkMagenta * MenuBorderIntensity)
                SpriteBat.Draw(bg, New Rectangle(600, (GameSize.Y / 2) - (Fader.Value * 2.5), GameSize.X - (600 * 2), (Fader.Value * 5)), Color.DarkMagenta)
                If controlls Then
                    For i As Integer = 0 To texts.Length - 1
                        If selection <> i Then
                            SpriteBat.DrawString(font, texts(i), (GameSize / 2) - (font.MeasureString(texts(i)) / 2) + New Vector2(0, -180 + i * 120), Color.White)
                        Else
                            SpriteBat.DrawString(font, texts(i), (GameSize / 2) - (font.MeasureString(texts(i)) / 2) + New Vector2(0, -180 + i * 120), New Color(ZooSettingsi.Value / 255, ZooSettingsi.Value / 255, 1, 255))
                        End If
                    Next
                End If
            End If

            'Draw popup
            Dim baserect As Rectangle = New Rectangle((GameSize.X / 2) - (popupsize.X * popupFader.Value / 200), (GameSize.Y - popupsize.Y) / 2, popupFader.Value / 100 * popupsize.X, popupsize.Y)
                SpriteBat.Draw(ReferencePixel, New Rectangle(baserect.X - (MenuBorder / 2), baserect.Y - (MenuBorder / 2), (popupFader.Value / 100 * (popupsize.X + MenuBorder)), popupsize.Y + MenuBorder), Color.Blue * MenuBorderIntensity)
                SpriteBat.Draw(ReferencePixel, baserect, New Color(0, 0, 40, 255))
                If popupActive Then
                    'Draw heading
                    SpriteBat.DrawString(fontB, popuptexts(0), New Vector2((GameSize.X / 2) - (fontB.MeasureString(popuptexts(0)).X / 2), 330), Color.White)

                    'Draw selections
                    If popupsel Then
                        SpriteBat.DrawString(font, popuptexts(1), New Vector2((GameSize.X / 2) - 400, 670), New Color(255, ZooSettingsi.Value / 255, ZooSettingsi.Value / 255, 255))
                        SpriteBat.DrawString(font, popuptexts(2), New Vector2((GameSize.X / 2) + 360 - fontB.MeasureString(popuptexts(2)).X, 670), Color.White)
                    Else
                        SpriteBat.DrawString(font, popuptexts(1), New Vector2((GameSize.X / 2) - 400, 670), Color.White)
                        SpriteBat.DrawString(font, popuptexts(2), New Vector2((GameSize.X / 2) + 360 - fontB.MeasureString(popuptexts(2)).X, 670), New Color(255, ZooSettingsi.Value / 255, ZooSettingsi.Value / 255, 255))
                    End If
                End If
            SpriteBat.End()
        End Sub

        Public Overrides Sub DrawDebug(gameTime As GameTime)

        End Sub

        Public Overrides Sub Update(gameTime As GameTime)
            EmmondInstance.IsMouseVisible = True
            'Fade in animation / Open menu
            Dim cstate As MenuInputState = InputMan.GetMenuInput()
            If Not hasntstart And Active Then
                'Set menu texts
                If SettingsMan.ValueBool(144) Then
                    texts = {StandardAssets.Texts(104), StandardAssets.Texts(83), StandardAssets.Texts(33), StandardAssets.Texts(5)}
                Else
                    texts = {StandardAssets.Texts(104), StandardAssets.Texts(83), "-", StandardAssets.Texts(5)}
                End If
                hasntstart = True
                'Activate transitions
                Fader = New Transition(Of Single)(New TransitionTypes.TransitionType_CriticalDamping(250), 0, 100, Sub()
                                                                                                                       controlls = True
                                                                                                                   End Sub)
                Automator.Add(Fader)
            End If

            'Menu controls
            Dim mstate As MouseState = Mouse.GetState
            Dim deltaM As Vector2 = (mstate.Position - lastmstate.Position).ToVector2
            Dim MousePos As Vector2 = Vector2.Transform(mstate.Position.ToVector2, Matrix.Invert(MainScalingMatrix))
            Dim canclick As Boolean = False

            If controlls And Not popupOpen Then
                For i As Integer = 0 To texts.Length - 1
                    Dim r As New Rectangle(((GameSize / 2) - (font.MeasureString(texts(i)) / 2) + New Vector2(0, -180 + i * 120)).ToPoint, font.MeasureString(texts(i)).ToPoint)
                    If r.Contains(MousePos) Then
                        If deltaM <> Vector2.Zero Then selection = i
                        If mstate.LeftButton = ButtonState.Pressed Then canclick = True
                    End If
                Next
            ElseIf controlls And popupOpen And popup <> 1 Then
                Dim r1 As New Rectangle(New Point((GameSize.X / 2) - 400, 670), font.MeasureString(popuptexts(1)).ToPoint)
                Dim r2 As New Rectangle(New Point((GameSize.X / 2) + 360 - fontB.MeasureString(popuptexts(2)).X, 670), font.MeasureString(popuptexts(2)).ToPoint)
                If r1.Contains(MousePos) Then
                    If deltaM <> Vector2.Zero Then popupsel = True
                    If mstate.LeftButton = ButtonState.Pressed Then canclick = True
                End If
                If r2.Contains(MousePos) Then
                    If deltaM <> Vector2.Zero Then popupsel = False
                    If mstate.LeftButton = ButtonState.Pressed Then canclick = True
                End If
            End If

            Dim Confm As Boolean = cstate.Confirm Or (canclick And EmmondInstance.IsActive)
            If controlls And Not popupOpen Then
                Dim IsUp As Boolean = False
                Dim IsDown As Boolean = False

                'Check fast scrolling
                If cstate.Movement.Y > 0 Then 'Checks Movement To The Left
                    If UpCounter = 0 Then IsUp = True
                    If UpCounter < FastScrollThreshold Then
                        UpCounter += gameTime.ElapsedGameTime.TotalMilliseconds
                    Else
                        If SkipperVert >= SkippingThreshold Then
                            IsUp = True
                            SkipperVert = 0
                        Else
                            SkipperVert += gameTime.ElapsedGameTime.TotalMilliseconds
                        End If
                    End If

                ElseIf cstate.Movement.Y < 0 Then 'Checks Movement To The Right
                    If DownCounter = 0 Then IsDown = True
                    If DownCounter < FastScrollThreshold Then
                        DownCounter += gameTime.ElapsedGameTime.TotalMilliseconds
                    Else
                        If SkipperVert >= SkippingThreshold Then
                            IsDown = True
                            SkipperVert = 0
                        Else
                            SkipperVert += gameTime.ElapsedGameTime.TotalMilliseconds
                        End If

                    End If
                Else
                    DownCounter = 0
                    UpCounter = 0
                End If

                'Check menu movement
                If IsDown And selection < 3 Then
                    selection += 1
                ElseIf IsUp And selection > 0 Then
                    selection -= 1
                End If

                'Check menu point selection
                If Confm Then
                    Select Case selection
                        Case 0
                            ContinueWithGame()
                        Case 1
                            popup = 0
                            popuptexts = {StandardAssets.Texts(84), StandardAssets.Texts(83), StandardAssets.Texts(6)}
                            popupsize = New Vector2(1000, 500)
                            OpenPopup()
                        Case 2
                            If SettingsMan.ValueBool(144) Then
                                popup = 1
                                popupsize = New Vector2(1500, 900)
                                OpenPopup()
                            Else
                                'TODO: play disabled sound
                            End If
                        Case 3
                            popup = 2
                            popuptexts = {StandardAssets.Texts(85), StandardAssets.Texts(86), StandardAssets.Texts(6)}
                            popupsize = New Vector2(1100, 500)
                            OpenPopup()
                    End Select
                End If

                If cstate.Start Or cstate.Back Then
                    ContinueWithGame()
                End If
            End If

            If popupActive Then
                Select Case popup
                    Case 0
                        If Confm Then
                            If popupsel Then
                                vlass.Restart()
                            Else
                                ClosePopup()
                            End If
                        End If

                        If cstate.Movement.X > 0 Then
                            popupsel = False
                        ElseIf cstate.Movement.X < 0 Then
                            popupsel = True
                        End If
                    Case 1
                    Case 2
                        If Confm Then
                            If popupsel Then
                                SceneMan.ChangeToScene(2, 1)
                            Else
                                ClosePopup()
                            End If
                        End If

                        If cstate.Movement.X > 0 Then
                            popupsel = False
                        ElseIf cstate.Movement.X < 0 Then
                            popupsel = True
                        End If
                End Select
            End If

            lastcstate = cstate
            lastmstate = mstate
        End Sub

        Friend Sub Open()
            Active = True
            hideroot = False
        End Sub

        Private Sub ContinueWithGame()
            controlls = False
            Fader = New Transition(Of Single)(New TransitionTypes.TransitionType_Deceleration(250), 100, 0, Sub()
                                                                                                                hasntended = True
                                                                                                                MediaPlayer.Resume()
                                                                                                            End Sub)
            Automator.Add(Fader)
        End Sub

        Private Sub OpenPopup()
            If Not popupOpen Then
                popupOpen = True
                popupFader = New Transition(Of Single)(New TransitionTypes.TransitionType_CriticalDamping(250), 0, 100, Sub()
                                                                                                                            popupActive = True
                                                                                                                        End Sub)
                Automator.Add(popupFader)
            End If
        End Sub

        Private Sub ClosePopup()
            If popupActive Then
                popupActive = False
                popupFader = New Transition(Of Single)(New TransitionTypes.TransitionType_Deceleration(250), 100, 0, Sub()
                                                                                                                         popupOpen = False
                                                                                                                     End Sub)
                Automator.Add(popupFader)
            End If
        End Sub

        Friend Sub OpenBackpack()
            hideroot = True
            popup = 1
            popupsize = New Vector2(1500, 900)
            OpenPopup()
        End Sub

        Private Sub CloseBackpack()
            If popupActive Then
                popupActive = False
                popupFader = New Transition(Of Single)(New TransitionTypes.TransitionType_Deceleration(250), 100, 0, Sub()
                                                                                                                         popupOpen = False
                                                                                                                         ContinueWithGame()
                                                                                                                     End Sub)
                Automator.Add(popupFader)
            End If
        End Sub
    End Class
End Namespace
