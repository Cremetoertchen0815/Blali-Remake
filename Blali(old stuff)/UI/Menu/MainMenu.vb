
Imports Emmond.Framework.Graphics
Imports Emmond.Framework.Input

Imports Emmond.Framework.SceneManager
Imports Emmond.Framework.Tweening
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Audio
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input
Imports Microsoft.Xna.Framework.Media

Namespace UI.Menu

    Public Class MainMenu
        Inherits Scene

        'Menu
        Public MenuPointsSub0 As String() = {Texts(1), Texts(4), Texts(5)} 'Main Menu("Play", "Missions", "Options", "Credits", "Exit")
        Public MenuPointsSub1 As String() = {"Ey, you idiot!", "This is an abandoned menu.", "Leave.", "Or at least try to."} 'Savefile select(Slot A, Slot B, Slot C, "Back")
        Public MenuPointsSub2 As String() = {"Music Volume", "Sound FX Volume", Texts(6)} 'Audio("Music: xxx%", "Sound FX: xxx%", "Back")
        Public MenuPointsSub3 As String() = {Texts(7), Texts(8), Texts(22), Texts(10), Texts(6)} 'Options("Graphics", "Audio", "Controlls", "Language", "Back")
        Public MenuPointsSub4 As String() = {Texts(60), Texts(12), Texts(13), Texts(57), Texts(21), Texts(6)} 'Graphics("Window Mode", "Resolution", "FPS Counter", "Bloom", "V-Sync", "Back")
        Public MenuPointsSub5 As String() = {Texts(64), Texts(9), Texts(58), Texts(59), Texts(6)} 'Controlls("Display hints", "Invert Camera", "D-Pad Mode", "Mouse Sensivity", "Back")

        Public Const CreditText As String = "PLACE CREDITS HERE"

        'Splashscreen
        Public ShowSplashscreen As Boolean = True
        Dim SplashCount As Integer
        Dim SplashFadingOver As Boolean = True

        'General Flags
        Public Property BGFlag As Boolean = False
        Public Property SkipIntroAnimation As Boolean = False
        Dim SelectedFlag As Byte
        Dim CurrentMenuPointSub As String()
        Dim CurrentMenuPointIndex As UInteger
        Dim Enable As Boolean
        Dim IntroDone As Boolean
        Dim SecAniTriggered As Boolean

        'Assets
        Dim Icon As Texture2D
        Dim ArrowA As Texture2D
        Dim ArrowB As Texture2D
        Dim BrightnessShader As Effect
        Public MenuFont As SpriteFont
        Public MenuFontFull As SpriteFont

        'Effects & Animations
        Dim ZoomAni As Transition(Of Single) 'Animation for the text blinking
        Dim FadeoutAni As Transition(Of Single) 'Animation for the text fading invisible
        Dim MenuSizeAni As Transition(Of Single) 'Animation for the size of the menu
        Dim SplashScreenAnimation As ShaderTransition 'Animation for screen fading to black


        'Sideways flags
        Dim UpCounter As UInteger = 0
        Dim DownCounter As UInteger = 0
        Dim RightCounter As UInteger = 0
        Dim LeftCounter As UInteger = 0
        Dim SkipperHor As UInteger = 0
        Dim SkipperVert As UInteger = 0
        Dim SidewaysEnabled As Boolean = True
        Dim lastmstate As MouseState

        Sub New()
            Config = New SceneConfig With {.ID = 2, .ReloadOnSelection = True, .AutoLoadSoundBank = False, .Descriptor = "menu", .ShowLoadingScreen = True, .ShowLSCustom = False}
        End Sub

        Public Overrides Sub Initialize()
            'Set flags
            Media.MediaPlayer.Stop()
            ResetMenu()
            Enable = False
            SecAniTriggered = False
            IntroDone = False
            LoadMenuPoints()

            'Create Animations (ADD REVERSE!!!!)
            ZoomAni = New Transition(Of Single) With {.Value = 0}
            FadeoutAni = New Transition(Of Single) With {.Value = 1}
            MenuSizeAni = New Transition(Of Single) With {.Value = (MenuPaddingTop * 2) + (CurrentMenuPointSub.Length * MenuPaddingBetw)}

        End Sub

        Public Overrides Sub LoadContent(Optional parameter As Integer = 0)
            If parameter > 0 Then
                ShowSplashscreen = False

                'Set correct text animation version
                Automator.Remove(ZoomAni)
                ZoomAni = New Transition(Of Single)(New TransitionTypes.TransitionType_EaseInEaseOut(300), 1, 0, Nothing) With {.Repeat = RepeatJob.Reverse}
                Automator.Add(ZoomAni)
            End If

            SoundBank = New Audio.SoundBank(AudioEng, "Content\sound\menu_sfx.xsb")

            'Load Textures
            Icon = ContentMan.Load(Of Texture2D)("ge_prelog")
            XboxButtons = ContentMan.Load(Of Texture2D)("ge_buttons_xbox")
            ArrowA = ContentMan.Load(Of Texture2D)("ge_debarr1")
            ArrowB = ContentMan.Load(Of Texture2D)("ge_debarr2")

            'Load Fonts
            MenuFont = ContentMan.Load(Of SpriteFont)("font\fnt_menpnt")
            MenuFontFull = ContentMan.Load(Of SpriteFont)("font\fnt_menpntsel")

            'Load Shaders
            BrightnessShader = ContentMan.Load(Of Effect)("fx\fx_bright")
            BrightnessShader.Parameters.Item(1).SetValue(0F)
            BrightnessShader.Parameters.Item(0).SetValue(50.0F)

        End Sub

        Public Overrides Sub UnloadContent()
            'Unload Textures
            Icon = Nothing

            'Unload Fonts
            MenuFont = Nothing
            MenuFontFull = Nothing

            'Dispose Animations
            Automator.Remove(ZoomAni)
            ZoomAni = Nothing
        End Sub

        Sub LoadMenuPoints()
            MenuPointsSub0 = {Texts(1), Texts(4), Texts(5)} 'Main Menu("Play", "Missions", "Options", "Credits", "Exit")
            MenuPointsSub1 = {"Ey, you idiot!", "This is an abandoned menu.", "Leave.", "Or at least try to."} 'Savefile select(Slot A, Slot B, Slot C, "Back")
            MenuPointsSub2 = {Texts(65), Texts(82), Texts(6)} 'Audio("Music: xxx%", "Sound FX: xxx%", "Back")
            MenuPointsSub3 = {Texts(7), Texts(8), Texts(22), Texts(10), Texts(6)} 'Options("Graphics", "Audio", "Controlls", "Language", "Back")
            MenuPointsSub4 = {Texts(60), Texts(12), Texts(13), Texts(57), Texts(21), Texts(6)} 'Graphics("Window Mode", "Resolution", "FPS Counter", "Bloom", "V-Sync", "Back")
            MenuPointsSub5 = {Texts(64), Texts(9), Texts(58), Texts(59), Texts(6)} 'Controlls("Display hints", "Invert Camera", "D-Pad Mode", "Mouse Sensivity", "Back")
            If DebugMode Then MenuPointsSub0(0) = Texts(63)
        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)
            'Draw In-Fadable Part
            SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, Nothing, Nothing, BrightnessShader, MainScalingMatrix)
            If ShowSplashscreen Then
                SpriteBat.Draw(Icon, New Rectangle(GameSize.X / 2 - (Icon.Width * IconScale) / 2, GameSize.Y / 2 - (Icon.Height * IconScale) / 2 + IconOffset, (Icon.Width * IconScale), (Icon.Height * IconScale)), Color.White) : SpriteCount += CUInt(1) 'Draw icon
                SpriteBat.DrawString(MenuFont, Texts(87), GameSize / 2 - MenuFont.MeasureString(Texts(87)) / 2 + New Vector2(0, 300), Color.White * ZoomAni.Value)
            Else
                Dim rect As Rectangle = New Rectangle((GameSize.X / 2) - (MenuWidth / 2), (GameSize.Y / 2) - (MenuSizeAni.Value / 2), MenuWidth, MenuSizeAni.Value)
                Dim rectB As Rectangle = New Rectangle((GameSize.X - MenuWidth - MenuBorder) / 2, (GameSize.Y - MenuSizeAni.Value - MenuBorder) / 2, MenuWidth + MenuBorder, MenuSizeAni.Value + MenuBorder)
                FillRectangle(rectB, New Color(0, 0.5F * MenuBorderIntensity, 0, 255.0F))
                FillRectangle(rect, New Color(0, 150, 0, 255))
                Dim MenuPointText As String = ""
                If CurrentMenuPointIndex = 10 Then
                    SpriteBat.DrawString(MenuFont, CreditText, New Vector2(GameSize.X / 2 - (MenuFont.MeasureString(CreditText).X / 2), rect.Y + (MenuFont.MeasureString(MenuPointText).Y / 2) + MenuSizeAni.Value / 2), Color.White) : SpriteCount += CUInt(1)
                Else
                    For i As Integer = 0 To CurrentMenuPointSub.Length - 1
                        MenuPointText = CurrentMenuPointSub(i)
                        If i = SelectedFlag Then ' If the current menu point is selected
                            Dim DrawArrows As Boolean = False

                            If CurrentMenuPointIndex = 2 And i = 0 Then 'Music Volume
                                MenuPointText = MenuPointText & ": " & SettingsMan.Value(31).ToString & "%" : DrawArrows = True
                            ElseIf CurrentMenuPointIndex = 2 And i = 1 Then 'SFX Volume
                                MenuPointText = MenuPointText & ": " & SettingsMan.Value(32).ToString & "%" : DrawArrows = True
                            ElseIf CurrentMenuPointIndex = 3 And i = 3 Then 'Lang
                                MenuPointText = MenuPointText & ": " & Localisation.GetLangName(Texts, SettingsMan.Value(19)) : DrawArrows = True
                            ElseIf CurrentMenuPointIndex = 4 And i = 0 Then 'Window Mode
                                Select Case SettingsMan.Value(20)
                                    Case 0
                                        MenuPointText = MenuPointText & ": " & Texts(11)
                                    Case 1
                                        MenuPointText = MenuPointText & ": " & Texts(61)
                                    Case 2
                                        MenuPointText = MenuPointText & ": " & Texts(62)
                                End Select
                                DrawArrows = True
                            ElseIf CurrentMenuPointIndex = 4 And i = 1 Then 'Resolution
                                Dim mode As DisplayMode = ResolutionList(CInt(SettingsMan.Value(21)))
                                MenuPointText = MenuPointText & ": " & mode.Width.ToString & "x" & mode.Height.ToString : DrawArrows = True
                            ElseIf CurrentMenuPointIndex = 4 And i = 2 Then 'Frame Counter
                                MenuPointText = MenuPointText & ": " & Localisation.GetConditionName(Texts, SettingsMan.ValueBool(22)) : DrawArrows = True
                            ElseIf CurrentMenuPointIndex = 4 And i = 3 Then 'Bloom
                                MenuPointText = MenuPointText & ": " & Localisation.GetConditionName(Texts, SettingsMan.ValueBool(23)) : DrawArrows = True
                            ElseIf CurrentMenuPointIndex = 4 And i = 4 Then 'V-Sanic
                                MenuPointText = MenuPointText & ": " & Localisation.GetConditionName(Texts, SettingsMan.ValueBool(24)) : DrawArrows = True
                            ElseIf CurrentMenuPointIndex = 5 And i = 0 Then 'Display Hints
                                MenuPointText = MenuPointText & ": " & Texts(24 + InputMan.DisplayHints) : DrawArrows = True
                            ElseIf CurrentMenuPointIndex = 5 And i = 1 Then 'Invert Cam
                                MenuPointText = MenuPointText & ": " & InversionToString(InputMan.AxisMultip.X) & "X, " & InversionToString(InputMan.AxisMultip.Y) & "Y" : DrawArrows = True
                            ElseIf CurrentMenuPointIndex = 5 And i = 2 Then 'D-Pad Mode
                                Dim tmpS As String = ""
                                If InputMan.DpadMode = DpadMode.Movement Then tmpS = Texts(30) Else tmpS = Texts(31)
                                MenuPointText = MenuPointText & ": " & tmpS
                                DrawArrows = True
                            ElseIf CurrentMenuPointIndex = 5 And i = 3 Then 'Mouse Sensivity
                                MenuPointText = MenuPointText & ": " & InputMan.MouseSense / 2 & "px/°" : DrawArrows = True
                            End If
                            SpriteBat.DrawString(MenuFont, MenuPointText, New Vector2(GameSize.X / 2 - (MenuFont.MeasureString(MenuPointText).X / 2), rect.Y + (i * MenuPaddingBetw) + MenuPaddingTop), New Color(Color.Lerp(Color.White, Color.Red, ZoomAni.Value), FadeoutAni.Value)) : SpriteCount += CUInt(1)
                            If DrawArrows Then
                                SpriteBat.Draw(ArrowA, New Rectangle(rect.Left + 50, rect.Y + (i * MenuPaddingBetw) + MenuPaddingTop + 8, 64, 64), New Color(Color.White, FadeoutAni.Value))
                                SpriteBat.Draw(ArrowB, New Rectangle(rect.Right - 50 - 64, rect.Y + (i * MenuPaddingBetw) + MenuPaddingTop + 8, 64, 64), New Color(Color.White, FadeoutAni.Value))
                            End If
                        Else 'If the current menu point isn't selected
                            'SpriteBat.Draw(MenuText, New Rectangle((GameSize.X - MenuWidth) / 2, CurrentPaddingTop + i * (MenuPaddingBetw + MenuHeight), MenuWidth, MenuHeight), Color.White) : SpriteCount += CUInt(1) 'Draw menu blocks
                            SpriteBat.DrawString(MenuFontFull, MenuPointText, New Vector2(GameSize.X / 2 - (MenuFontFull.MeasureString(MenuPointText).X / 2), rect.Y + (i * MenuPaddingBetw) + MenuPaddingTop), New Color(Color.White, FadeoutAni.Value)) : SpriteCount += CUInt(1)
                        End If

                    Next
                End If
            End If
            'Draw Fia Text
            If BGFlag Then SpriteBat.DrawString(DebugFont, Framework.Entities.Entity.ConvertToString, New Vector2(0, 0), Color.Magenta)
            SpriteBat.End()
        End Sub


        Public Overrides Sub Update(gameTime As GameTime)
            EmmondInstance.IsMouseVisible = True

            'Trigger Second Animation
            If Not SecAniTriggered Then
                Enable = True
                SplashScreenAnimation = New ShaderTransition(New TransitionTypes.TransitionType_Acceleration(FadeTime), 0F, 100.0F, BrightnessShader, 0, Sub()
                                                                                                                                                             IntroDone = True
                                                                                                                                                             SplashFadingOver = False
                                                                                                                                                         End Sub)
                Automator.Add(SplashScreenAnimation)
                SecAniTriggered = True
            End If

            'Gamepad Functions
            Dim state As MenuInputState = InputMan.GetMenuInput()
            Dim mov As Vector2 = state.Movement
            Dim ls As Boolean = False
            Dim mstate As MouseState = Mouse.GetState
            Dim deltaM As Vector2 = (mstate.Position - lastmstate.Position).ToVector2
            If Not ShowSplashscreen Then

                'Mouse Controls
                Dim MousePos As Vector2 = Vector2.Transform(mstate.Position.ToVector2, Matrix.Invert(MainScalingMatrix))
                Dim rect As Rectangle = New Rectangle((GameSize.X / 2) - (MenuWidth / 2), (GameSize.Y / 2) - (MenuSizeAni.Value / 2), MenuWidth, MenuSizeAni.Value)
                For i As Integer = 0 To CurrentMenuPointSub.Length - 1
                    Dim r As New Rectangle(rect.X, rect.Y + (i * MenuPaddingBetw) + MenuPaddingTop, rect.Width, 65)
                    If r.Contains(MousePos) And Enable Then
                        If deltaM <> Vector2.Zero Then SelectedFlag = i
                        If mstate.LeftButton = ButtonState.Pressed Then ls = True
                    End If
                    If New Rectangle(rect.Left + 50, rect.Y + (i * MenuPaddingBetw) + MenuPaddingTop, 64, 64).Contains(MousePos) And mstate.LeftButton And EmmondInstance.IsActive And Enable Then mov.X = -1
                    If New Rectangle(rect.Right - 50 - 64, rect.Y + (i * MenuPaddingBetw) + MenuPaddingTop, 64, 64).Contains(MousePos) And mstate.LeftButton And EmmondInstance.IsActive And Enable Then mov.X = 1
                Next

                'Check auto scrolling
                Dim IsUp As Boolean = False
                Dim IsDown As Boolean = False
                Dim IsLeft As Boolean = False
                Dim IsRight As Boolean = False
                If Enable Then 'Vertical Movement
                    If mov.Y > 0 Then 'Checks Movement To The Left
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

                    ElseIf mov.Y < 0 Then 'Checks Movement To The Right
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
                End If
                If SidewaysEnabled And Enable Then 'Horizontal Movement
                    If mov.X < 0 Then 'Checks Movement To The Left
                        If LeftCounter = 0 Then IsLeft = True
                        If LeftCounter < FastScrollThreshold Then
                            LeftCounter += gameTime.ElapsedGameTime.TotalMilliseconds
                        Else
                            If SkipperHor >= SkippingThreshold Then
                                IsLeft = True
                                SkipperHor = 0
                            Else
                                SkipperHor += gameTime.ElapsedGameTime.TotalMilliseconds
                            End If
                        End If

                    ElseIf mov.X > 0 Then 'Checks Movement To The Right
                        If RightCounter = 0 Then IsRight = True
                        If RightCounter < FastScrollThreshold Then
                            RightCounter += gameTime.ElapsedGameTime.TotalMilliseconds
                        Else
                            If SkipperHor >= SkippingThreshold Then
                                IsRight = True
                                SkipperHor = 0
                            Else
                                SkipperHor += gameTime.ElapsedGameTime.TotalMilliseconds
                            End If

                        End If
                    Else
                        RightCounter = 0
                        LeftCounter = 0
                    End If
                End If


                If IsDown And SelectedFlag < CurrentMenuPointSub.Length - 1 Then 'Checks Movement Downwards
                    SelectedFlag += 1
                    SoundBank.PlayCue("Vertical")
                ElseIf IsUp And SelectedFlag > 0 Then 'Checks Movement Upwards
                    SelectedFlag -= 1
                    SoundBank.PlayCue("Vertical")
                End If


                Dim pressedok As Boolean = ls And EmmondInstance.IsActive
                If (state.Confirm Or pressedok) And Enable Then ' If Button A Was Pressed

                    Select Case CurrentMenuPointIndex 'Select Current Menu
                        Case 0 'Root Menu
                            Select Case SelectedFlag 'Select Menu Entry
                                Case 0 'Play/Play Demo
                                    If (IntroDone = True And Not DebugMode) Then ConfirmSceneChange(16, 0)
                                    If (IntroDone = True And DebugMode) Then ConfirmSceneChange(4, 0)
                                Case 1 'Options
                                    ConfirmSubmenuChange(3)
                                Case 2 'Exit
                                    EmmondInstance.RumbleFlag = 200
                                    EmmondInstance.Exit()
                            End Select
                        Case 1 'Normal Play
                            Select Case SelectedFlag 'Select Menu Entry
                                Case 0 'Slot A

                                Case 1 'Slot B
                                    ConfirmSubmenuChange(8)
                                Case 2 'Slot C

                                    'Case 3 'Back
                                    '    BackSubmenuChange(0)
                            End Select
                        Case 2 'Sound
                            Select Case SelectedFlag 'Select Menu Entry
                                Case 2 'Exit
                                    BackSubmenuChange(0)
                            End Select
                        Case 3 'Options
                            Select Case SelectedFlag 'Select Menu Entry
                                Case 0 'Graphics
                                    ConfirmSubmenuChange(4)
                                Case 1 'Sound
                                    ConfirmSubmenuChange(2)
                                Case 2 'Controls
                                    ConfirmSceneChange(6, 0)
                                Case 3 'Language

                                Case 4 'Back
                                    BackSubmenuChange(0)
                            End Select
                        Case 4 'Graphics/Options
                            Select Case SelectedFlag 'Select Menu Entry
                                Case 5 'Back
                                    BackSubmenuChange(3)
                            End Select
                        Case 5 'Controls
                            Select Case SelectedFlag
                                Case 1 'Invert axis
                                    EmmondInstance.RumbleFlag = 200
                                    With InputMan.AxisMultip
                                        If .X = 1 And .Y = 1 Then
                                            InputMan.AxisMultip = New Vector2(-1, 1)
                                        ElseIf .X = -1 And .Y = 1 Then
                                            InputMan.AxisMultip = New Vector2(1, -1)
                                        ElseIf .X = 1 And .Y = -1 Then
                                            InputMan.AxisMultip = New Vector2(-1, -1)
                                        ElseIf .X = -1 And .Y = -1 Then
                                            InputMan.AxisMultip = New Vector2(1, 1)
                                        End If
                                        SettingsMan.Save()
                                        SoundBank.PlayCue("Confirm")
                                    End With
                                Case 4 'Back
                                    BackSubmenuChange(3)
                            End Select
                        Case 10
                            BackSubmenuChange(0)
                    End Select
                End If

                If state.Back And Enable Then ' If Button B Was Pressed
                    Select Case CurrentMenuPointIndex 'Select Current Menu
                        Case 0 'Root Menu
                            ResetSplash()
                        Case 2, 3, 10
                            BackSubmenuChange(0)
                        Case 4, 5
                            BackSubmenuChange(3)
                    End Select

                End If

                SidewaysEnabled = False 'Disable Sideways Cursor Movement By Standard

                Select Case CurrentMenuPointIndex 'Sideways Movement
                    Case 2
                        Select Case SelectedFlag 'Select Menu Entry
                            Case 0
                                SidewaysEnabled = True
                                If IsLeft And SettingsMan.Value(31) > 0 Then
                                    SettingsMan.Value(31, True) -= 5 'Decrease volume flag by 1
                                    SoundBank.PlayCue("Horizontal")
                                ElseIf IsRight And SettingsMan.Value(31) < 100 Then
                                    SettingsMan.Value(31, True) += 5 'Increase volume flag by 1
                                    SoundBank.PlayCue("Horizontal")
                                End If
                                MediaPlayer.Volume = SettingsMan.Value(31) / 100
                            Case 1
                                SidewaysEnabled = True
                                If IsLeft And SettingsMan.Value(32) > 0 Then
                                    SettingsMan.Value(32, True) -= 5 'Decrease volume flag by 1
                                    SoundBank.PlayCue("Horizontal")
                                ElseIf IsRight And SettingsMan.Value(32) < 100 Then
                                    SettingsMan.Value(32, True) += 5 'Increase volume flag by 1
                                    SoundBank.PlayCue("Horizontal")
                                End If
                                AudioEng.GetCategory("Default").SetVolume(SettingsMan.Value(32) / 100)
                        End Select
                    Case 3
                        Select Case SelectedFlag 'Select Menu Entry
                            Case 3 'Change music volume
                                SidewaysEnabled = True
                                If IsLeft And SettingsMan.Value(19) > 0 Then
                                    SettingsMan.Value(19, True) -= 1 'Decrease Language flag by 1
                                    Texts = Localisation.GetLocalisationData([Enum].GetName(GetType(Localisation.Languages), SettingsMan.Value(19))) 'Load new Language file
                                    LoadMenuPoints() 'Reload Menu Texts
                                    SetCurrentMPS(True) 'Reload Current Menu texts
                                    SoundBank.PlayCue("Horizontal")
                                ElseIf IsRight And SettingsMan.Value(19) < 3 Then
                                    SettingsMan.Value(19, True) += 1 'Increase Language flag by 1
                                    Texts = Localisation.GetLocalisationData([Enum].GetName(GetType(Localisation.Languages), SettingsMan.Value(19))) 'Load new Language file
                                    LoadMenuPoints() 'Reload Menu Texts
                                    SetCurrentMPS(True) 'Reload Current Menu texts
                                    SoundBank.PlayCue("Horizontal")
                                End If
                        End Select
                    Case 4
                        Select Case SelectedFlag 'Select Menu Entry
                            Case 0 'Change fullscreen mode
                                SidewaysEnabled = True
                                If IsLeft Then
                                    If SettingsMan.Value(20) > 0 Then
                                        SettingsMan.Value(20, True) -= 1
                                        SoundBank.PlayCue("Horizontal")
                                    Else
                                        SettingsMan.Value(20, True) = 2
                                    End If
                                    EmmondInstance.UpdateGraphics()
                                ElseIf IsRight Then
                                    If SettingsMan.Value(20) < 2 Then
                                        SettingsMan.Value(20, True) += 1
                                        SoundBank.PlayCue("Horizontal")
                                    Else
                                        SettingsMan.Value(20, True) = 0
                                    End If
                                    EmmondInstance.UpdateGraphics()
                                End If
                            Case 1 'Change resolution
                                SidewaysEnabled = True
                                If IsLeft And SettingsMan.Value(21) > 0 Then
                                    SettingsMan.Value(21, True) -= 1 'Decrease resolution flag by 1
                                    EmmondInstance.UpdateGraphics()
                                    SoundBank.PlayCue("Horizontal")
                                ElseIf IsRight And SettingsMan.Value(21) < ResolutionList.Length - 1 Then
                                    SettingsMan.Value(21, True) += 1 'Increase resolution flag by 1
                                    EmmondInstance.UpdateGraphics()
                                    SoundBank.PlayCue("Horizontal")
                                End If
                            Case 2 'Enable/Disable FPS counter
                                SidewaysEnabled = True
                                If IsLeft And SettingsMan.ValueBool(2) Then
                                    SettingsMan.ValueBool(22, True) = False
                                    EmmondInstance.UpdateGraphics()
                                    SoundBank.PlayCue("Horizontal")
                                ElseIf IsRight And Not SettingsMan.ValueBool(22) Then
                                    SettingsMan.ValueBool(22, True) = True
                                    EmmondInstance.UpdateGraphics()
                                    SoundBank.PlayCue("Horizontal")
                                End If
                            Case 3 'Enable/Disable Bloom
                                SidewaysEnabled = True
                                If IsLeft And SettingsMan.ValueBool(23) Then
                                    SettingsMan.ValueBool(23, True) = False
                                    SoundBank.PlayCue("Horizontal")
                                ElseIf IsRight And Not SettingsMan.ValueBool(23) Then
                                    SettingsMan.ValueBool(23, True) = True
                                    SoundBank.PlayCue("Horizontal")
                                End If
                            Case 4 'Enable/Disable V-Sync
                                SidewaysEnabled = True
                                If IsLeft And SettingsMan.ValueBool(24) Then
                                    SettingsMan.ValueBool(24, True) = False
                                    EmmondInstance.UpdateGraphics()
                                    SoundBank.PlayCue("Horizontal")
                                ElseIf IsRight And Not SettingsMan.ValueBool(24) Then
                                    SettingsMan.Value(24, True) = True
                                    EmmondInstance.UpdateGraphics()
                                    SoundBank.PlayCue("Horizontal")
                                End If
                        End Select
                    Case 5
                        Select Case SelectedFlag
                            Case 0 'Display hints
                                SidewaysEnabled = True
                                If IsLeft And InputMan.DisplayHints > 0 Then
                                    InputMan.DisplayHints -= 1 'Decrease sensitivity flag by 1
                                    SoundBank.PlayCue("Horizontal")
                                ElseIf IsRight And InputMan.DisplayHints < 2 Then
                                    InputMan.DisplayHints += 1 'Increase sensitivity flag by 1
                                    SoundBank.PlayCue("Horizontal")
                                End If
                            Case 2 'D-Pad mode
                                SidewaysEnabled = True
                                If IsLeft Or IsRight Then
                                    If InputMan.DpadMode = 0 Then InputMan.DpadMode = 1 Else InputMan.DpadMode = 0
                                    SoundBank.PlayCue("Horizontal")
                                End If
                            Case 3
                                SidewaysEnabled = True
                                If IsLeft And InputMan.MouseSense > 1 Then
                                    InputMan.MouseSense -= 1 'Decrease sensitivity flag by 1
                                    SoundBank.PlayCue("Horizontal")
                                ElseIf IsRight And InputMan.MouseSense < 100 Then
                                    InputMan.MouseSense += 1 'Increase sensitivity flag by 1
                                    SoundBank.PlayCue("Horizontal")
                                End If
                        End Select
                End Select
            Else
                If SplashCount >= 0 Then SplashCount += gameTime.ElapsedGameTime.TotalMilliseconds

                If SplashCount > 1000 Then
                    ZoomAni = New Transition(Of Single)(New TransitionTypes.TransitionType_EaseInEaseOut(cLSFadeSpeed), 0, 1, Sub()
                                                                                                                                  ZoomAni = New Transition(Of Single)(New TransitionTypes.TransitionType_Linear(700), 1, 0.3, Nothing) With {.Repeat = RepeatJob.Reverse}
                                                                                                                                  Automator.Add(ZoomAni)
                                                                                                                              End Sub)
                    Automator.Add(ZoomAni)
                    SplashCount = -1
                End If

                If state.Start And Not pressedStart And Not SplashFadingOver And ShowSplashscreen Then
                    pressedStart = True
                    SplashFadingOver = True
                    Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_EaseInEaseOut(cLSFadeSpeed), 100.0F, 0F, BrightnessShader, 0, Sub()
                                                                                                                                                            pressedStart = False
                                                                                                                                                            ShowSplashscreen = False
                                                                                                                                                            'Change text fade transition
                                                                                                                                                            ZoomAni = New Transition(Of Single)(New TransitionTypes.TransitionType_EaseInEaseOut(300), 1, 0, Nothing) With {.Repeat = RepeatJob.Reverse}
                                                                                                                                                            Automator.Add(ZoomAni)
                                                                                                                                                            'Fade in screen
                                                                                                                                                            Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_EaseInEaseOut(cLSFadeSpeed), 0F, 100.0F, BrightnessShader, 0, Sub()
                                                                                                                                                                                                                                                                                                    SplashFadingOver = False
                                                                                                                                                                                                                                                                                                End Sub))
                                                                                                                                                        End Sub))
                End If
            End If

            lastmstate = mstate
        End Sub
        Dim pressedStart As Boolean = False

        Public Overrides Sub DrawDebug(gameTime As GameTime)

        End Sub

        Private Sub ResetSplash()
            If Not SplashFadingOver Then
                SplashFadingOver = True
                Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_EaseInEaseOut(cLSFadeSpeed), 100.0F, 0F, BrightnessShader, 0, Sub()
                                                                                                                                                        ShowSplashscreen = True
                                                                                                                                                        ZoomAni = New Transition(Of Single) With {.Value = 0}
                                                                                                                                                        SplashCount = 0
                                                                                                                                                        Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_EaseInEaseOut(cLSFadeSpeed), 0F, 100.0F, BrightnessShader, 0, Sub()
                                                                                                                                                                                                                                                                                                SplashFadingOver = False
                                                                                                                                                                                                                                                                                            End Sub))
                                                                                                                                                    End Sub))
            End If
        End Sub

        Private Sub ConfirmSceneChange(scenenr As Integer, parameter As Integer)
            EmmondInstance.RumbleFlag = 200
            Enable = False
            SoundBank.PlayCue("Go")

            Automator.Remove(ZoomAni)
            ZoomAni = New Transition(Of Single)(New TransitionTypes.TransitionType_EaseInEaseOut(80), 1, 0, Nothing) With {.Repeat = RepeatJob.Reverse}
            Automator.Add(ZoomAni)

            Automator.Add(New Transition(Of Single)(New TransitionTypes.TransitionType_Linear(1000), 100, 0, Sub()
                                                                                                                 SceneMan.ChangeToScene(scenenr, parameter)
                                                                                                             End Sub))
        End Sub

        Private Sub ConfirmSubmenuChange(men As Integer)
            EmmondInstance.RumbleFlag = 200
            Enable = False
            SoundBank.PlayCue("Confirm")

            Automator.Remove(ZoomAni)
            ZoomAni = New Transition(Of Single)(New TransitionTypes.TransitionType_EaseInEaseOut(80), 1, 0, Nothing) With {.Repeat = RepeatJob.Reverse}
            Automator.Add(ZoomAni)

            'Wait
            Automator.Add(New Transition(Of Single)(New TransitionTypes.TransitionType_Linear(MenuPressDelayTime), 100, 0, Sub()
                                                                                                                               ChangeOver(men)
                                                                                                                           End Sub))

            'Fade out text
            FadeoutAni = New Transition(Of Single)(New TransitionTypes.TransitionType_Linear(TextFadeoutTime), 1, 0, Nothing)
            Automator.Add(FadeoutAni)
        End Sub

        Private Sub BackSubmenuChange(men As Integer)
            EmmondInstance.RumbleFlag = 200
            Enable = False
            SoundBank.PlayCue("Back")

            'Wait
            Automator.Add(New Transition(Of Single)(New TransitionTypes.TransitionType_Linear(MenuPressDelayTime), 100, 0, Sub()
                                                                                                                               ChangeOver(men)
                                                                                                                           End Sub))

            'Fade out text
            FadeoutAni = New Transition(Of Single)(New TransitionTypes.TransitionType_Linear(TextFadeoutTime), 1, 0, Nothing)
            Automator.Add(FadeoutAni)
        End Sub

        Private Sub ChangeOver(men As Integer)
            'Reset text blinking
            Automator.Remove(ZoomAni)
            ZoomAni = New Transition(Of Single)(New TransitionTypes.TransitionType_EaseInEaseOut(300), 1, 0, Nothing) With {.Repeat = RepeatJob.Reverse}
            Automator.Add(ZoomAni)

            'Change Menu size
            CurrentMenuPointIndex = men
            SetCurrentMPS()
            MenuSizeAni = New Transition(Of Single)(New TransitionTypes.TransitionType_EaseInEaseOut(MenuMoveTime), MenuSizeAni.Value, (MenuPaddingTop * 2) + (CurrentMenuPointSub.Length * MenuPaddingBetw), Sub()
                                                                                                                                                                                                                  'Fade in text
                                                                                                                                                                                                                  FadeoutAni = New Transition(Of Single)(New TransitionTypes.TransitionType_Linear(TextFadeoutTime), 0, 1, Nothing)
                                                                                                                                                                                                                  Automator.Add(FadeoutAni)
                                                                                                                                                                                                                  Enable = True
                                                                                                                                                                                                              End Sub)
            Automator.Add(MenuSizeAni)
        End Sub

        Sub SetCurrentMPS(Optional NoReset As Boolean = False)
            SetRealMPS(NoReset)
        End Sub

        Private Sub SetRealMPS(NoReset As Boolean)
            Select Case CurrentMenuPointIndex
                Case 0
                    CurrentMenuPointSub = MenuPointsSub0
                Case 1
                    CurrentMenuPointSub = MenuPointsSub1
                Case 2
                    CurrentMenuPointSub = MenuPointsSub2
                Case 3
                    CurrentMenuPointSub = MenuPointsSub3
                Case 4
                    CurrentMenuPointSub = MenuPointsSub4
                Case 5
                    CurrentMenuPointSub = MenuPointsSub5
            End Select
            If Not NoReset Then SelectedFlag = 0
        End Sub

        Sub ResetMenu()
            CurrentMenuPointIndex = 0
            SelectedFlag = 0
            SetCurrentMPS()
        End Sub
    End Class

End Namespace