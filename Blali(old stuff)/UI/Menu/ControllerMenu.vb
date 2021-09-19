
Imports Emmond.Framework.Graphics
Imports Emmond.Framework.Input

Imports Emmond.Framework.SceneManager
Imports Emmond.Framework.Tweening
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input
Imports GamePadType = Emmond.Framework.Input.GamePadType

Namespace UI.Menu
    Public Class ControllerMenu
        Inherits Scene

        Public MenuFont As SpriteFont
        Public ControllerFont As SpriteFont
        Public BrightnessShader As Effect

        Const YSepOrientation As Integer = 180
        Const XSepOrientation As Integer = 545
        Const LeftOffset As Integer = 10
        Const LeftBOffset As Integer = 10
        Const TopOffset As Integer = 20
        Const BetweenOffset As Integer = 65
        Const BetweenBOffset As Integer = 75


        Dim IsUp As Boolean = False
        Dim IsDown As Boolean = False
        Dim IsLeft As Boolean = False
        Dim IsRight As Boolean = False

        Dim UpCounter As UInteger = 0
        Dim DownCounter As UInteger = 0
        Dim RightCounter As UInteger = 0
        Dim LeftCounter As UInteger = 0
        Dim SkipperHor As UInteger = 0
        Dim SkipperVert As UInteger = 0
        Dim UpdatePlayer As Boolean = False
        Dim IsLocked As Boolean = False
        Dim IsLegit As Boolean = True
        Dim DisableMoveFlag As Boolean = False
        Dim ignorenextframeinput As Boolean = False

        Dim sel As Vector2
        Dim rightcollumnmode As Integer = 0 '0 = Nothing, 1 = Map Gamepad, 2 = Map Keys
        Dim stat As GameInputState
        Dim ZooSettingsi As Transition(Of Single)
        Dim OldConnectionMethod As GamePadType = GamePadType.Undefined
        Dim TempConnectionMethod As GamePadType = GamePadType.Undefined
        Dim TempVal As Integer
        Dim PlayRoomInstance As PlayRoom


        Sub New()
            Config = New SceneConfig With {.ID = 6, .ReloadOnSelection = False, .AutoLoadSoundBank = False, .Descriptor = "con", .ShowLoadingScreen = True, .ShowLSCustom = False}
        End Sub

        Public Overrides Sub Initialize()
            EmmondInstance.IsMouseVisible = False
        End Sub

        Public Overrides Sub LoadContent(Optional parameter As Integer = 0)
            SoundBank = New Audio.SoundBank(AudioEng, "Content\sound\cont.xsb")
            MenuFont = ContentMan.Load(Of SpriteFont)("font\fnt_menpnt")
            ControllerFont = ContentMan.Load(Of SpriteFont)("font\fnt_contrl")
            BrightnessShader = ContentMan.Load(Of Effect)("fx\fx_bright")
            sel = New Vector2(0, 0)
            ZooSettingsi = New Transition(Of Single)(New TransitionTypes.TransitionType_Linear(500), 0.8, 1, Nothing) With {.Repeat = RepeatJob.Reverse}
            Automator.Add(ZooSettingsi)

            PlayRoomInstance = New PlayRoom
            PlayRoomInstance.Initialize()
            PlayRoomInstance.LoadContent()
            PlayRoomInstance.Update(New GameTime)

            Media.MediaPlayer.Play(ContentMan.Load(Of Media.Song)("sound\bgm\cont_full"))
            Media.MediaPlayer.IsRepeating = True

            BrightnessShader.Parameters.Item(1).SetValue(0F)
            BrightnessShader.Parameters.Item(0).SetValue(100.0F)
            Automator.Add(New ShaderTransition(New TransitionTypes.TransitionType_Acceleration(1500), 0F, 100.0F, BrightnessShader, 0, Nothing))  'Fade from normal to black and afterward activate Lvl 2
        End Sub

        Public Overrides Sub UnloadContent()
            SoundBank.Dispose()
        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)
            PlayRoomInstance.Draw(gameTime)

            SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, BrightnessShader, PlayRoomInstance.lvl.Camera.GetMatrix * MainScalingMatrix)

            'Draw Heading
            SpriteBat.DrawString(MenuFont, Texts(22), New Vector2(GameSize.X / 2 - MenuFont.MeasureString(Texts(22)).X / 2, YSepOrientation / 2 - MenuFont.MeasureString(Texts(22)).Y / 2), Color.White)
            Primitives2D.DrawLine(New Vector2(0, YSepOrientation), New Vector2(GameSize.X, YSepOrientation), Color.White, 5) 'Draw Horizontal Seperator
            Primitives2D.DrawLine(New Vector2(XSepOrientation, YSepOrientation), New Vector2(XSepOrientation, GameSize.Y), Color.White, 5) 'Draw Vertical Seperator

            Dim TileSizeMultip As Integer = cTileSize * 4
            Dim controllsoffset As Integer = 32

            'Analog Indicators
            Primitives2D.DrawRectangle(New Rectangle(0 * TileSizeMultip + controllsoffset, 1080 - 2 * TileSizeMultip, TileSizeMultip, TileSizeMultip), Color.Yellow, 5)
            Primitives2D.DrawCircle(New Vector2(0.5 * TileSizeMultip + controllsoffset + stat.Movement.X * TileSizeMultip * 0.5, 1080 - 1.5 * TileSizeMultip - stat.Movement.Y * TileSizeMultip * 0.5), 5, 2, Color.Red)
            Primitives2D.DrawRectangle(New Rectangle(2 * TileSizeMultip + controllsoffset, 1080 - 2 * TileSizeMultip, TileSizeMultip, TileSizeMultip), Color.Yellow, 5)
            Primitives2D.DrawCircle(New Vector2(2.5 * TileSizeMultip + controllsoffset + stat.Camera.X * TileSizeMultip * 0.5, 1080 - 1.5 * TileSizeMultip - stat.Camera.Y * TileSizeMultip * 0.5), 5, 2, Color.Red)

            'Draw 1st Button Cross buttons
            If stat.Jump Then Primitives2D.FillRectangle(New Rectangle(4 * TileSizeMultip + controllsoffset, 1080 - 2 * TileSizeMultip, TileSizeMultip / 2, TileSizeMultip / 2), Color.Red)
            If stat.UseItem Then Primitives2D.FillRectangle(New Rectangle(4.5 * TileSizeMultip + controllsoffset, 1080 - 2 * TileSizeMultip, TileSizeMultip / 2, TileSizeMultip / 2), Color.Red)
            If stat.Interact Then Primitives2D.FillRectangle(New Rectangle(4 * TileSizeMultip + controllsoffset, 1080 - 1.5 * TileSizeMultip, TileSizeMultip / 2, TileSizeMultip / 2), Color.Red)
            If stat.DropItem Then Primitives2D.FillRectangle(New Rectangle(4.5 * TileSizeMultip + controllsoffset, 1080 - 1.5 * TileSizeMultip, TileSizeMultip / 2, TileSizeMultip / 2), Color.Red)
            'Draw 1st Button Cross
            Primitives2D.DrawLine(New Vector2(4 * TileSizeMultip + controllsoffset, 1080 - 1.5 * TileSizeMultip), New Vector2(5 * TileSizeMultip + controllsoffset, 1080 - 1.5 * TileSizeMultip), Color.Yellow, 2)
            Primitives2D.DrawLine(New Vector2(4.5 * TileSizeMultip + controllsoffset, 1080 - 1 * TileSizeMultip), New Vector2(4.5 * TileSizeMultip + controllsoffset, 1080 - 2 * TileSizeMultip), Color.Yellow, 2)

            'Draw 2st Button Cross buttons
            If stat.Start Then Primitives2D.FillRectangle(New Rectangle(6 * TileSizeMultip + controllsoffset, 1080 - 2 * TileSizeMultip, TileSizeMultip / 2, TileSizeMultip / 2), Color.Red)
            If stat.Backpack Then Primitives2D.FillRectangle(New Rectangle(6.5 * TileSizeMultip + controllsoffset, 1080 - 2 * TileSizeMultip, TileSizeMultip / 2, TileSizeMultip / 2), Color.Red)
            'If stat.L Then Primitives2D.FillRectangle(New Rectangle(6 * TileSizeMultip, 1080 - 1.5 * TileSizeMultip, TileSizeMultip / 2, TileSizeMultip / 2), Color.Red)
            'If stat.RS Then Primitives2D.FillRectangle(New Rectangle(6.5 * TileSizeMultip, 1080 - 1.5 * TileSizeMultip, TileSizeMultip / 2, TileSizeMultip / 2), Color.Red)
            'Draw 2nd Button Cross
            Primitives2D.DrawLine(New Vector2(6 * TileSizeMultip + controllsoffset, 1080 - 1.5 * TileSizeMultip), New Vector2(7 * TileSizeMultip + controllsoffset, 1080 - 1.5 * TileSizeMultip), Color.Yellow, 2)
            Primitives2D.DrawLine(New Vector2(6.5 * TileSizeMultip + controllsoffset, 1080 - 1 * TileSizeMultip), New Vector2(6.5 * TileSizeMultip + controllsoffset, 1080 - 2 * TileSizeMultip), Color.Yellow, 2)

            'First Collumn
            Dim index As Integer = -1
            If sel.X = 0 Then index = sel.Y
            'Prepare Variables
            Dim tmpS As String = ""
            If InputMan.DpadMode = DpadMode.Movement Then tmpS = Texts(30) Else tmpS = Texts(31)
            Dim nr As Integer = 24 + InputMan.ConnectionMethod
            If nr = 26 Then nr = 71
            Dim textis As String() = {Texts(64) & ": " & Texts(24 + InputMan.DisplayHints), Texts(9) & ": " & InversionToString(InputMan.AxisMultip.X) & "X, " & InversionToString(InputMan.AxisMultip.Y) & "Y",
                Texts(58) & ": " & tmpS, Texts(59) & ": " & InputMan.MouseSense / 2 & "px/°", Texts(23) & ":  " & Texts(nr), Texts(80), Texts(81), Texts(27), Texts(40)}
            Dim posindices As Integer() = {0, 1, 2, 3, 5, 6, 7, 9, 10}

            If IsLocked And index = 4 Then
                nr = 24 + TempConnectionMethod
                If nr = 26 Then nr = 71
                textis(4) = Texts(23) & ": " & Texts(nr)
            End If
            DrawCollumnA(textis, posindices, index)

            Select Case rightcollumnmode
                Case 1
                    'Second Collumn
                    index = -1
                    If sel.X = 1 Then index = sel.Y
                    DrawCollumnB({Texts(30) & " " & Texts(28) & ": Ax" & InputMan.DxIM(0) + 1 & " (" & Texts(46) & ")",
                                 Texts(30) & " " & Texts(29) & ": Ax" & InputMan.DxIM(1) + 1 & " (" & Texts(46) & ")",
                                 Texts(66) & ": Ax" & InputMan.DxIM(2) + 1 & " (" & Texts(47) & ")"}, index)

                    'Third Collumn
                    index = -1
                    If sel.X = 2 Then index = sel.Y
                    DrawCollumnC({Texts(32) & ": B" & InputMan.DxIM(4) + 1 & " (Start)",
                                 Texts(33) & ": B" & InputMan.DxIM(5) + 1 & " (Back)",
                                 Texts(34) & ": B" & InputMan.DxIM(6) + 1 & " (A)",
                                 Texts(37) & ": B" & InputMan.DxIM(7) + 1 & " (B)",
                                 Texts(69) & ": B" & InputMan.DxIM(8) + 1 & " (X)",
                                 Texts(70) & ": B" & InputMan.DxIM(9) + 1 & " (Y)",
                                 Texts(67) & ": B" & InputMan.DxIM(10) + 1 & " (LB)",
                                 Texts(68) & ": B" & InputMan.DxIM(11) + 1 & " (RB)",
                                 Texts(36) & ": B" & InputMan.DxIM(12) + 1 & " (LT)",
                                 Texts(35) & ": B" & InputMan.DxIM(13) + 1 & " (RT)"}, index)
                Case 2
                    'Third Collumn
                    index = -1
                    If sel.X = 1 Then index = sel.Y
                    DrawCollumnB({Texts(32) & ": " & GetKeyName(InputMan.KeyIM(0)),
                                 Texts(33) & ": " & GetKeyName(InputMan.KeyIM(1)),
                                 Texts(34) & ": " & GetKeyName(InputMan.KeyIM(2)),
                                 Texts(37) & ": " & GetKeyName(InputMan.KeyIM(3)),
                                 Texts(69) & ": " & GetKeyName(InputMan.KeyIM(4)),
                                 Texts(70) & ": " & GetKeyName(InputMan.KeyIM(5))}, index)

                    index = -1
                    If sel.X = 2 Then index = sel.Y
                    DrawCollumnC({Texts(67) & ": " & GetKeyName(InputMan.KeyIM(6)),
                                 Texts(68) & ": " & GetKeyName(InputMan.KeyIM(7)),
                                 Texts(36) & ": " & GetKeyName(InputMan.KeyIM(8)),
                                 Texts(35) & ": " & GetKeyName(InputMan.KeyIM(9))}, index)
            End Select

            'Draw Hints
            If Not UpdatePlayer Then
                If IsLocked Then
                    Select Case sel.X
                        Case 0
                            SpriteBat.DrawString(ControllerFont, "A(Space): " & Texts(56), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 5), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                            SpriteBat.DrawString(ControllerFont, "B(Left Shift): " & Texts(6), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 4), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                        Case 1

                            If rightcollumnmode = 1 Then
                                SpriteBat.DrawString(ControllerFont, Texts(49), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 5), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                                SpriteBat.DrawString(ControllerFont, "A(Space): " & Texts(56), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 4), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                                SpriteBat.DrawString(ControllerFont, "B(Left Shift): " & Texts(6), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 3), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                            Else
                                SpriteBat.DrawString(ControllerFont, Texts(50), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 5), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                            End If
                        Case 2
                            SpriteBat.DrawString(ControllerFont, Texts(50), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 5), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                    End Select
                Else
                    Select Case sel.X
                        Case 0
                            SpriteBat.DrawString(ControllerFont, "A(Space): " & Texts(51), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 5), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                            SpriteBat.DrawString(ControllerFont, "B(Left Shift): " & Texts(40), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 4), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                        Case 1
                            SpriteBat.DrawString(ControllerFont, "A(Space): " & Texts(52), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 5), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                            SpriteBat.DrawString(ControllerFont, "B(Left Shift): " & Texts(48), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 4), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                            If rightcollumnmode = 1 Then
                                SpriteBat.DrawString(ControllerFont, "X(E): " & Texts(53), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 3), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                                SpriteBat.DrawString(ControllerFont, "Y(Q): " & Texts(54), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 2), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                            Else
                                SpriteBat.DrawString(ControllerFont, "Y(Q): " & Texts(54), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 3), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                            End If
                        Case 2
                            SpriteBat.DrawString(ControllerFont, "A(Space): " & Texts(52), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 5), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                            SpriteBat.DrawString(ControllerFont, "B(Left Shift): " & Texts(48), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 4), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                            SpriteBat.DrawString(ControllerFont, "Y(Q): " & Texts(54), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 3), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
                    End Select
                End If
            Else
                SpriteBat.DrawString(ControllerFont, "Start(Enter): " & Texts(48), New Vector2(XSepOrientation + LeftOffset, GameSize.Y - BetweenOffset / 2 * 5), Color.CadetBlue, 0, New Vector2(0, 0), 0.8, SpriteEffects.None, 0)
            End If
            SpriteBat.End()
        End Sub
        Private Sub DrawCollumnA(textis As String(), posindices As Integer(), zoomindex As Integer)
            For i As Integer = 0 To textis.Length - 1
                If i = zoomindex Then
                    If IsLocked And IsLegit Then
                        SpriteBat.DrawString(ControllerFont, textis(i), New Vector2(LeftOffset, YSepOrientation + TopOffset + BetweenOffset * posindices(i)) + ControllerFont.MeasureString(textis(i)) / 2,
                                               Color.Lime, 0, ControllerFont.MeasureString(textis(i)) / 2, ZooSettingsi.Value, SpriteEffects.None, 0)
                    ElseIf Not IsLegit Then
                        SpriteBat.DrawString(ControllerFont, textis(i), New Vector2(LeftOffset, YSepOrientation + TopOffset + BetweenOffset * posindices(i)) + ControllerFont.MeasureString(textis(i)) / 2,
                                               Color.Red, 0, ControllerFont.MeasureString(textis(i)) / 2, ZooSettingsi.Value, SpriteEffects.None, 0)
                    Else
                        SpriteBat.DrawString(ControllerFont, textis(i), New Vector2(LeftOffset, YSepOrientation + TopOffset + BetweenOffset * posindices(i)) + ControllerFont.MeasureString(textis(i)) / 2,
                                               Color.White, 0, ControllerFont.MeasureString(textis(i)) / 2, ZooSettingsi.Value, SpriteEffects.None, 0)
                    End If
                Else
                    SpriteBat.DrawString(ControllerFont, textis(i), New Vector2(LeftOffset, YSepOrientation + TopOffset + BetweenOffset * posindices(i)), Color.White)
                End If
            Next
        End Sub

        Private Sub DrawCollumnB(textis As String(), zoomindex As Integer)
            Dim vec As Vector2
            For i As Integer = 0 To textis.Length - 1
                Dim prefx As String = ""
                If (i = 0 And InputMan.Multip(0) < 0) Or (i = 1 And InputMan.Multip(1) < 0) Or (i = 2 And InputMan.Multip(2) < 0) Then prefx = "-"
                vec = New Vector2(LeftOffset + XSepOrientation, YSepOrientation + TopOffset + BetweenOffset * i)

                If i = zoomindex Then
                    If IsLocked And IsLegit Then
                        SpriteBat.DrawString(ControllerFont, "Ax" & (TempVal + 1).ToString, New Vector2(LeftOffset + XSepOrientation, YSepOrientation + TopOffset + BetweenBOffset * i) + ControllerFont.MeasureString(textis(i)) / 2,
                                                   Color.Lime, 0, ControllerFont.MeasureString(textis(i)) / 2, ZooSettingsi.Value, SpriteEffects.None, 0)
                    ElseIf IsLocked And Not IsLegit Then
                        SpriteBat.DrawString(ControllerFont, "Ax" & (TempVal + 1).ToString, New Vector2(LeftOffset + XSepOrientation, YSepOrientation + TopOffset + BetweenBOffset * i) + ControllerFont.MeasureString(textis(i)) / 2,
                                                   Color.Red, 0, ControllerFont.MeasureString(textis(i)) / 2, ZooSettingsi.Value, SpriteEffects.None, 0)
                    Else
                        SpriteBat.DrawString(ControllerFont, prefx & textis(i), New Vector2(LeftOffset + XSepOrientation, YSepOrientation + TopOffset + BetweenBOffset * i) + ControllerFont.MeasureString(prefx & textis(i)) / 2,
                                                   Color.White, 0, ControllerFont.MeasureString(textis(i)) / 2, ZooSettingsi.Value, SpriteEffects.None, 0)
                    End If
                Else
                    SpriteBat.DrawString(ControllerFont, prefx & textis(i), New Vector2(LeftOffset + XSepOrientation, YSepOrientation + TopOffset + BetweenBOffset * i), Color.White)
                End If
            Next
        End Sub

        Private Sub DrawCollumnC(textis As String(), zoomindex As Integer)
            For i As Integer = 0 To textis.Length - 1
                If i = zoomindex Then

                    If IsLocked And IsLegit Then
                        SpriteBat.DrawString(ControllerFont, "B" & (TempVal + 1).ToString, New Vector2(LeftBOffset + XSepOrientation + (GameSize.X - XSepOrientation) / 2, YSepOrientation + TopOffset + BetweenBOffset * i) + ControllerFont.MeasureString(textis(i)) / 2,
                                                   Color.Lime, 0, ControllerFont.MeasureString(textis(i)) / 2, ZooSettingsi.Value, SpriteEffects.None, 0)
                    ElseIf IsLocked And Not IsLegit Then
                        SpriteBat.DrawString(ControllerFont, "B" & (TempVal + 1).ToString, New Vector2(LeftBOffset + XSepOrientation + (GameSize.X - XSepOrientation) / 2, YSepOrientation + TopOffset + BetweenBOffset * i) + ControllerFont.MeasureString(textis(i)) / 2,
                                                   Color.Red, 0, ControllerFont.MeasureString(textis(i)) / 2, ZooSettingsi.Value, SpriteEffects.None, 0)
                    Else
                        SpriteBat.DrawString(ControllerFont, textis(i), New Vector2(LeftBOffset + XSepOrientation + (GameSize.X - XSepOrientation) / 2, YSepOrientation + TopOffset + BetweenBOffset * i) + ControllerFont.MeasureString(textis(i)) / 2,
                                                   Color.White, 0, ControllerFont.MeasureString(textis(i)) / 2, ZooSettingsi.Value, SpriteEffects.None, 0)
                    End If
                Else
                    SpriteBat.DrawString(ControllerFont, textis(i), New Vector2(LeftBOffset + XSepOrientation + (GameSize.X - XSepOrientation) / 2, YSepOrientation + TopOffset + BetweenBOffset * i), Color.White)
                End If
            Next
        End Sub
        Private Function GetKeyName(nr As Integer) As String '{-1 = Left MB, -2 = Middle MB, -3 = Right MB, -4 = XButton 1, -5 = XButton 2, -6 = Wheel Up, -7 = Wheel Down}
            If nr < -7 Then Return Texts(79)
            Select Case nr
                Case -1
                    Return Texts(72)
                Case -2
                    Return Texts(73)
                Case -3
                    Return Texts(74)
                Case -4
                    Return Texts(75)
                Case -5
                    Return Texts(76)
                Case -6
                    Return Texts(77)
                Case -7
                    Return Texts(78)
            End Select
            Return DirectCast(nr, Keys).ToString
        End Function
        Public Overrides Sub DrawDebug(gameTime As GameTime)
        End Sub

        Dim didlock As Boolean = False
        Dim stoplock As Boolean = False
        Public Overrides Sub Update(gameTime As GameTime)
            stat = InputMan.GetGameInput(True) 'Get Game Input
            Dim con As MenuInputState = InputMan.GetMenuInput
            If ignorenextframeinput Then con = New MenuInputState : ignorenextframeinput = False

            Dim state As GameInputState = InputMan.GetGameInput(True)

            'Check fast scroll
            Dim UpS As Boolean = False
            Dim DownS As Boolean = False
            Dim LeftS As Boolean = False
            Dim RightS As Boolean = False
            If con.Movement.Y < 0 Then 'Checks Movement To The Left
                If UpCounter = 0 Then UpS = True
                If UpCounter < FastScrollThreshold Then
                    UpCounter += gameTime.ElapsedGameTime.TotalMilliseconds
                Else
                    If SkipperVert >= SkippingThreshold Then
                        UpS = True
                        SkipperVert = 0
                    Else
                        SkipperVert += gameTime.ElapsedGameTime.TotalMilliseconds
                    End If
                End If

            ElseIf con.Movement.Y > 0 Then 'Checks Movement To The Right
                If DownCounter = 0 Then DownS = True
                If DownCounter < FastScrollThreshold Then
                    DownCounter += gameTime.ElapsedGameTime.TotalMilliseconds
                Else
                    If SkipperVert >= SkippingThreshold Then
                        DownS = True
                        SkipperVert = 0
                    Else
                        SkipperVert += gameTime.ElapsedGameTime.TotalMilliseconds
                    End If

                End If
            Else
                DownCounter = 0
                UpCounter = 0
            End If


            If con.Movement.X < 0 Then 'Checks Movement To The Left
                If LeftCounter = 0 Then LeftS = True
                If LeftCounter < FastScrollThreshold Then
                    LeftCounter += gameTime.ElapsedGameTime.TotalMilliseconds
                Else
                    If SkipperHor >= SkippingThreshold Then
                        LeftS = True
                        SkipperHor = 0
                    Else
                        SkipperHor += gameTime.ElapsedGameTime.TotalMilliseconds
                    End If
                End If

            ElseIf con.Movement.X > 0 Then 'Checks Movement To The Right
                If RightCounter = 0 Then RightS = True
                If RightCounter < FastScrollThreshold Then
                    RightCounter += gameTime.ElapsedGameTime.TotalMilliseconds
                Else
                    If SkipperHor >= SkippingThreshold Then
                        RightS = True
                        SkipperHor = 0
                    Else
                        SkipperHor += gameTime.ElapsedGameTime.TotalMilliseconds
                    End If

                End If
            Else
                RightCounter = 0
                LeftCounter = 0
            End If

            If Not UpdatePlayer Then
                'If Not InputMan.ConnectionMethod = GamePadType.Generic And Not OldConnectionMethod = InputMan.ConnectionMethod Then sel.X = 0 : sel.Y = 0
                OldConnectionMethod = InputMan.ConnectionMethod

                If didlock And IsLegit And (con.Confirm Or con.Back) And sel.Y < 4 And sel.X = 0 Then IsLocked = False : didlock = False : stoplock = True

                If IsLocked Then 'If locking mode is actived(user selection is expected)
                    didlock = True
                    Select Case sel.X
                        Case 0 'If setting the controll mode
                            Select Case sel.Y 'Display hints
                                Case 0
                                    If LeftS And Not IsLeft And InputMan.DisplayHints > 0 Then
                                        InputMan.DisplayHints -= 1 'Decrease sensitivity flag by 1
                                        SoundBank.PlayCue("Horizontal")
                                    ElseIf RightS And Not IsRight And InputMan.DisplayHints < 2 Then
                                        InputMan.DisplayHints += 1 'Increase sensitivity flag by 1
                                        SoundBank.PlayCue("Horizontal")
                                    End If
                                Case 1 'Invert axis
                                    If (RightS And Not IsRight) Or (LeftS And Not IsLeft) Then
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
                                            SoundBank.PlayCue("Horizontal")
                                        End With
                                    End If
                                Case 2 'D-Pad mode
                                    If (RightS And Not IsRight) Or (LeftS And Not IsLeft) Then
                                        If InputMan.DpadMode = 0 Then InputMan.DpadMode = 1 Else InputMan.DpadMode = 0
                                        SoundBank.PlayCue("Horizontal")
                                    End If
                                Case 3 'Mouse Sensitivity
                                    If (LeftS And Not IsLeft) And InputMan.MouseSense > 1 Then
                                        InputMan.MouseSense -= 1 'Decrease sensitivity flag by 1
                                        SoundBank.PlayCue("Horizontal")
                                    ElseIf (RightS And Not IsRight) And InputMan.MouseSense < 100 Then
                                        InputMan.MouseSense += 1 'Increase sensitivity flag by 1
                                        SoundBank.PlayCue("Horizontal")
                                    End If
                                Case 4
                                    If RightS And Not IsRight And TempConnectionMethod < 2 Then TempConnectionMethod += 1
                                    If LeftS And Not IsLeft And TempConnectionMethod > 0 Then TempConnectionMethod -= 1

                                    Select Case TempConnectionMethod
                                        Case GamePadType.Undefined
                                            IsLegit = True
                                        Case GamePadType.Xbox
                                            IsLegit = InputMan.IsXInputComp
                                        Case GamePadType.Generic
                                            IsLegit = InputMan.IsDirectInputComp
                                    End Select

                                    If IsLegit And con.Confirm Then
                                        InputMan.ConnectionMethod = TempConnectionMethod
                                        IsLocked = False
                                        If InputMan.ConnectionMethod = GamePadType.Generic Then InputMan.LoadGeneric()
                                    End If
                            End Select

                            Select Case TempConnectionMethod
                                Case GamePadType.Undefined
                                    IsLegit = True
                                Case GamePadType.Xbox
                                    IsLegit = InputMan.IsXInputComp
                                Case GamePadType.Generic
                                    IsLegit = InputMan.IsDirectInputComp
                            End Select

                        Case 1 'If setting up an axis
                            Select Case rightcollumnmode
                                Case 1
                                    DisableMoveFlag = True
                                    Dim ret As Integer = InputMan.GetPressedAxis()
                                    If ret > -1 Then
                                        TempVal = ret
                                        IsLegit = Not InputMan.DoesContain(True, ret)
                                    End If
                                    If IsLegit And con.Confirm Then InputMan.DxIM(sel.Y) = TempVal : IsLocked = False : InputMan.SaveControls()
                                    If con.Back Or state.Backpack Then IsLocked = False
                                Case 2
                                    DisableMoveFlag = True
                                    Dim ret As Integer = InputMan.GetPressedKey(stat.Jump)
                                    If ret > -8 Then
                                        TempVal = ret
                                        IsLegit = Not InputMan.DoesContainKey(ret)
                                        If IsLegit Or InputMan.KeyIM(sel.Y) = TempVal Then InputMan.KeyIM(sel.Y) = TempVal : IsLocked = False : InputMan.SaveControls() : ignorenextframeinput = True
                                    End If
                            End Select
                        Case 2 'If assigning a button
                            Select Case rightcollumnmode
                                Case 1
                                    DisableMoveFlag = True
                                    Dim ret As Integer = InputMan.GetPressedButton(con.Confirm)
                                    If ret > -1 Then
                                        TempVal = ret
                                        IsLegit = Not InputMan.DoesContain(False, ret)
                                        If IsLegit Or InputMan.DxIM(sel.Y + 4) = TempVal Then InputMan.DxIM(sel.Y + 4) = TempVal : IsLocked = False : InputMan.SaveControls()
                                    End If
                                Case 2
                                    DisableMoveFlag = True
                                    Dim ret As Integer = InputMan.GetPressedKey(stat.Jump)
                                    If ret > -8 Then
                                        TempVal = ret
                                        IsLegit = Not InputMan.DoesContainKey(ret)
                                        If IsLegit Or InputMan.KeyIM(sel.Y + 6) = TempVal Then InputMan.KeyIM(sel.Y + 6) = TempVal : IsLocked = False : InputMan.SaveControls() : ignorenextframeinput = True
                                    End If
                            End Select
                    End Select

                Else 'During normal menu movement
                    'Move cursor & clear options
                    Select Case sel.X
                        Case 0
                            If UpS And Not IsUp And sel.Y < 8 Then sel.Y += 1
                            If DownS And Not IsDown And sel.Y > 0 Then sel.Y -= 1

                            If sel.Y = 5 And InputMan.ConnectionMethod <> GamePadType.Generic Then IsLegit = False Else IsLegit = True
                        Case 1

                            If rightcollumnmode = 1 Then
                                If RightS And Not IsRight Then sel.X += 1
                                If UpS And Not IsUp And sel.Y < 2 Then sel.Y += 1
                                If DownS And Not IsDown And sel.Y > 0 Then sel.Y -= 1
                                If con.XtraY And Not DisableMoveFlag Then InputMan.DxIM(sel.Y) = -1
                            ElseIf rightcollumnmode = 2 Then
                                If RightS And Not IsRight Then
                                    If sel.Y > 3 Then sel.Y = 3
                                    sel.X += 1
                                End If
                                If UpS And Not IsUp And sel.Y < 5 Then sel.Y += 1
                                If DownS And Not IsDown And sel.Y > 0 Then sel.Y -= 1
                                If con.XtraY And Not DisableMoveFlag And sel.Y <> 2 Then InputMan.KeyIM(sel.Y) = -8
                            End If

                            'If DisableMoveFlag And Not con.XtraY Then DisableMoveFlag = False
                        Case 2

                            If rightcollumnmode = 1 Then
                                If LeftS And Not IsLeft Then
                                    If sel.Y > 2 Then sel.Y = 2
                                    sel.X -= 1
                                End If
                                If UpS And Not IsUp And sel.Y < 9 Then sel.Y += 1
                                If DownS And Not IsDown And sel.Y > 0 Then sel.Y -= 1
                            ElseIf rightcollumnmode = 2 Then
                                If LeftS And Not IsLeft Then sel.X -= 1
                                If UpS And Not IsUp And sel.Y < 3 Then sel.Y += 1
                                If DownS And Not IsDown And sel.Y > 0 Then sel.Y -= 1
                                If con.XtraY And Not DisableMoveFlag Then InputMan.KeyIM(sel.Y + 6) = -8
                            End If

                            'If DisableYFlag And Not state.DropItem Then DisableYFlag = False
                            If con.XtraY And Not DisableMoveFlag Then InputMan.DxIM(sel.Y + 4) = -1
                    End Select

                    'Confirm Menu selection
                    If con.Confirm And Not stoplock Then
                        Select Case sel.X
                            Case 0
                                Select Case sel.Y
                                    Case 0
                                        IsLocked = True
                                    Case 1
                                        IsLocked = True
                                    Case 2
                                        IsLocked = True
                                    Case 3
                                        IsLocked = True
                                    Case 4
                                        IsLocked = True
                                        TempConnectionMethod = InputMan.ConnectionMethod
                                    Case 5
                                        If IsLegit Then
                                            rightcollumnmode = 1
                                            sel.X = 1
                                            sel.Y = 0
                                        End If
                                    Case 6
                                        If IsLegit Then
                                            rightcollumnmode = 2
                                            sel.X = 1
                                            sel.Y = 0
                                        End If
                                    Case 7
                                        UpdatePlayer = True
                                    Case 8
                                        SceneMan.ChangeToScene(2, 2)
                                End Select
                            Case 1
                                If Not IsLocked Then IsLocked = True : InputMan.CheckFlag = False : TempVal = InputMan.DxIM(sel.Y)
                            Case 2
                                If Not IsLocked Then IsLocked = True : InputMan.CheckFlag = False : TempVal = InputMan.DxIM(sel.Y + 4)
                        End Select
                    End If

                    If con.Back And Not (stoplock Or rightcollumnmode > 0) Then SceneMan.ChangeToScene(2, 2)
                    If con.Back And rightcollumnmode > 0 And Not DisableMoveFlag Then rightcollumnmode = 0 : sel.X = 0 : sel.Y = 0
                    DisableMoveFlag = False

                    If con.XtraX And sel.X = 1 Then
                        Select Case sel.Y
                            Case 0
                                InputMan.Multip(0) = InputMan.Multip(0) * -1
                                InputMan.SaveControls()
                            Case 1
                                InputMan.Multip(1) = InputMan.Multip(1) * -1
                                InputMan.SaveControls()
                            Case 2
                                InputMan.Multip(2) = InputMan.Multip(2) * -1
                                InputMan.SaveControls()
                        End Select
                    End If
                End If

            Else
                PlayRoomInstance.Update(gameTime)
                If state.Start Then UpdatePlayer = False
            End If


            'If (cuA.IsStopped Or cuA.IsStopping) And Not cuB.IsPlaying Then cuB.Play()

            stoplock = False
            IsUp = UpS
            IsDown = DownS
            IsLeft = LeftS
            IsRight = RightS
        End Sub
    End Class
End Namespace
