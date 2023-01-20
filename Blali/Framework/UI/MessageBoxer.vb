Imports System
Imports System.Collections.Generic
Imports Blali.Framework.Misc
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input

Namespace Framework.UI
    Public Class MessageBoxer
        Inherits GlobalManager
        Implements IFinalRenderDelegate

        'Buttons
        Private btnConfirm As VirtualButton
        Private btnEscape As VirtualButton
        Private focusBtn As Integer = 0

        'Rendering & assets
        Private batcher As Batcher
        Private ButtonBase As Texture2D
        Private DispFont As NezSpriteFont
        Private Material As New Material() With {.SamplerState = SamplerState.AnisotropicClamp}

        'Layout & data
        Private MsgBoxArea As New Rectangle(710, 440, 500, 170)
        Private CurrentMessage As Message
        Private ShowMessageBox As Boolean = False
        Private WasEnabled As Boolean
        Private MessageStack As New List(Of Message)
        Private InputBoxData As String

        Sub New()
            batcher = New Batcher(Core.GraphicsDevice)
            ButtonBase = Core.Content.LoadTexture("btn_base_blali")
            DispFont = New NezSpriteFont(Core.Content.Load(Of SpriteFont)("font/MsgText"))

            btnConfirm = New VirtualButton(New VirtualButton.KeyboardKey(Keys.Space), New VirtualButton.KeyboardKey(Keys.Enter), New VirtualButton.GamePadButton(0, Buttons.A))
            btnEscape = New VirtualButton(New VirtualButton.KeyboardKey(Keys.Escape), New VirtualButton.KeyboardKey(Keys.Back), New VirtualButton.GamePadButton(0, Buttons.B))

            Core.Emitter.AddObserver(CoreEvents.SceneChanged, AddressOf OnSceneChanged)
        End Sub

        Dim lastmstate As MouseState
        Dim lastkstate As KeyboardState
        Public Overrides Sub Update()
            If MessageStack.Count > 0 And Not ShowMessageBox Then
                WasEnabled = Core.Scene.Enabled
                Core.Scene.Enabled = False
                ShowMessageBox = True
                CurrentMessage = MessageStack(0)
                If CurrentMessage.IsInputbox Then InputBoxData = CurrentMessage.DefaultResponse
                MessageStack.RemoveAt(0)
                focusBtn = 0
            End If

            If Not ShowMessageBox Then Return

            Dim mstate As MouseState = Mouse.GetState()
            Dim kstate = Keyboard.GetState()
            Dim mpos As Vector2 = Vector2.Transform(mstate.Position.ToVector2, Matrix.Invert(ScaleMatrix))

            If CurrentMessage.IsInputbox Then

                If (mstate.LeftButton = ButtonState.Pressed And lastmstate.LeftButton = ButtonState.Released AndAlso New Rectangle(MsgBoxArea.Right - 100, MsgBoxArea.Top + 10, 80, 30).Contains(mpos)) Or btnConfirm.IsPressed Then
                    If CurrentMessage.FinalActionInputBox IsNot Nothing Then CurrentMessage.FinalActionInputBox(InputBoxData, 0) : CloseMsgBox() Else CloseMsgBox()
                End If
                If (mstate.LeftButton = ButtonState.Pressed And lastmstate.LeftButton = ButtonState.Released And New Rectangle(MsgBoxArea.Right - 100, MsgBoxArea.Top + 50, 80, 30).Contains(mpos)) Or btnEscape.IsPressed Then
                    If CurrentMessage.FinalActionInputBox IsNot Nothing Then CurrentMessage.FinalActionInputBox(CurrentMessage.DefaultResponse, 1) : CloseMsgBox() Else CloseMsgBox()
                End If

                Dim UpperCase As Boolean = kstate.IsKeyDown(Keys.LeftShift) Or kstate.IsKeyDown(Keys.RightShift) Or kstate.IsKeyDown(Keys.CapsLock)
                For Each key In kstate.GetPressedKeys
                    If lastkstate.IsKeyDown(key) Then Continue For
                    If key >= Keys.A And key <= Keys.Z Then InputBoxData &= If(UpperCase, key.ToString.ToUpper, key.ToString.ToLower)
                    If key >= Keys.D0 And key <= Keys.D9 And Not UpperCase Then InputBoxData &= (key - 48).ToString
                    If key >= Keys.NumPad0 And key <= Keys.NumPad9 Then InputBoxData &= (key - 96).ToString
                    If key = Keys.Back And InputBoxData.Length > 0 Then InputBoxData = InputBoxData.Substring(0, InputBoxData.Length - 1)

                    Select Case key
                        Case Keys.Space
                            InputBoxData &= " "
                        Case Keys.OemPeriod
                            InputBoxData &= If(UpperCase, ":", ".")
                        Case Keys.OemComma
                            InputBoxData &= If(UpperCase, ";", ",")
                        Case Keys.OemMinus
                            InputBoxData &= If(UpperCase, "_", "-")
                        Case Keys.D1
                            If UpperCase Then InputBoxData &= "!"
                        Case Keys.D2
                            If UpperCase Then InputBoxData &= """"
                        Case Keys.D3
                            If UpperCase Then InputBoxData &= "§"
                        Case Keys.D4
                            If UpperCase Then InputBoxData &= "$"
                        Case Keys.D5
                            If UpperCase Then InputBoxData &= "%"
                        Case Keys.D6
                            If UpperCase Then InputBoxData &= "&"
                        Case Keys.D7
                            If UpperCase Then InputBoxData &= "/"
                        Case Keys.D8
                            If UpperCase Then InputBoxData &= "("
                        Case Keys.D9
                            If UpperCase Then InputBoxData &= ")"
                        Case Keys.D9
                            If UpperCase Then InputBoxData &= "="
                    End Select
                Next
            Else
                Dim tot_len As Single = 0F
                Dim padding As Single = 25.0F
                Dim margin As Single = 30.0F
                For i As Integer = 0 To CurrentMessage.Buttons.Length - 1
                    tot_len += DispFont.MeasureString(CurrentMessage.Buttons(i)).X + padding 'width + padding
                Next
                tot_len += (CurrentMessage.Buttons.Length - 1) * margin 'Add margin

                'Draw buttons
                Dim x_offset As Single = 0F
                For i As Integer = 0 To CurrentMessage.Buttons.Length - 1
                    Dim txt_width As Single = DispFont.MeasureString(CurrentMessage.Buttons(i)).X + padding
                    If mstate.LeftButton = ButtonState.Pressed And lastmstate.LeftButton = ButtonState.Released AndAlso New Rectangle(MsgBoxArea.Center.X - tot_len / 2 + x_offset, MsgBoxArea.Y + MsgBoxArea.Height * 0.7, txt_width, 30).Contains(mpos) Or (btnConfirm.IsPressed And i = focusBtn) Then
                        If CurrentMessage.FinalActionMsgBox IsNot Nothing Then CurrentMessage.FinalActionMsgBox(i)
                        CloseMsgBox()
                    End If
                    'Update offset
                    x_offset += txt_width + margin
                Next
            End If

            lastmstate = mstate
            lastkstate = kstate
        End Sub

        Public Sub EnqueueMsgbox(Prompt As String, finalaction As FinalMsgAction, buttons As String())
            MessageStack.Add(New Message With {.Message = Prompt, .IsInputbox = False, .Buttons = buttons, .FinalActionMsgBox = finalaction})
        End Sub
        Public Sub EnqueueMsgbox(Prompt As String)
            MessageStack.Add(New Message With {.Message = Prompt, .IsInputbox = False, .Buttons = {"OK"}, .FinalActionMsgBox = Nothing})
        End Sub

        Public Function OpenMsgbox(Prompt As String, finalaction As FinalMsgAction, buttons As String()) As Boolean
            If ShowMessageBox Then Return False
            MessageStack.Add(New Message With {.Message = Prompt, .IsInputbox = False, .Buttons = buttons, .FinalActionMsgBox = finalaction})
            Return True
        End Function

        Public Sub EnqueueInputbox(Prompt As String, finalaction As FinalInputAction, Optional def As String = "")
            MessageStack.Add(New Message With {.Message = Prompt, .IsInputbox = True, .Buttons = {"OK", "Cancel"}, .FinalActionInputBox = finalaction, .DefaultResponse = def})
        End Sub

        Public Function OpenInputbox(Prompt As String, finalaction As FinalInputAction, Optional def As String = "") As Boolean
            If ShowMessageBox Then Return False
            MessageStack.Add(New Message With {.Message = Prompt, .IsInputbox = True, .Buttons = {"OK", "Cancel"}, .FinalActionInputBox = finalaction, .DefaultResponse = def})
            Return True
        End Function


        Private Sub CloseMsgBox()
            If MessageStack.Count > 0 Then
                CurrentMessage = MessageStack(0)
                If CurrentMessage.IsInputbox Then InputBoxData = CurrentMessage.DefaultResponse
                MessageStack.RemoveAt(0)
                focusBtn = 0
            Else
                Core.Scene.Enabled = WasEnabled
                ShowMessageBox = False
            End If
        End Sub

        Public Sub OnAddedToScene(scene As Scene) Implements IFinalRenderDelegate.OnAddedToScene

        End Sub

        Public Sub OnSceneBackBufferSizeChanged(newWidth As Integer, newHeight As Integer) Implements IFinalRenderDelegate.OnSceneBackBufferSizeChanged

        End Sub

        Public Sub OnSceneChanged()
            Core.Scene.FinalRenderDelegate = Me
        End Sub

        Public Sub HandleFinalRender(finalRenderTarget As RenderTarget2D, letterboxColor As Color, source As RenderTarget2D, finalRenderDestinationRect As Rectangle, samplerState As SamplerState) Implements IFinalRenderDelegate.HandleFinalRender
            Core.GraphicsDevice.SetRenderTarget(finalRenderTarget)
            Core.GraphicsDevice.Clear(letterboxColor)

            batcher.Begin(BlendState.Opaque, samplerState, Nothing, Nothing)
            batcher.Draw(source, finalRenderDestinationRect, Color.White)
            batcher.End()

            If Not ShowMessageBox Then Return

            batcher.Begin(Material, ScaleMatrix)

            'Draw base
            batcher.DrawRect(MsgBoxArea, New Color(90, 90, 90))
            batcher.DrawLine(New Vector2(MsgBoxArea.Left, MsgBoxArea.Top), New Vector2(MsgBoxArea.Left, MsgBoxArea.Bottom), New Color(20, 20, 20), 4)
            batcher.DrawLine(New Vector2(MsgBoxArea.Left, MsgBoxArea.Top), New Vector2(MsgBoxArea.Right, MsgBoxArea.Top), New Color(20, 20, 20), 3)
            batcher.DrawLine(New Vector2(MsgBoxArea.Right, MsgBoxArea.Top), New Vector2(MsgBoxArea.Right, MsgBoxArea.Bottom), New Color(180, 180, 180), 4)
            batcher.DrawLine(New Vector2(MsgBoxArea.Left, MsgBoxArea.Bottom), New Vector2(MsgBoxArea.Right, MsgBoxArea.Bottom), New Color(180, 180, 180), 3)

            If CurrentMessage.IsInputbox Then
                'Draw text
                batcher.DrawString(DispFont, WrapTextDifferently(CurrentMessage.Message, 38, False), MsgBoxArea.Location.ToVector2 + New Vector2(30, 10), Color.White)

                'Draw buttons
                batcher.Draw(ButtonBase, New Rectangle(MsgBoxArea.Right - 100, MsgBoxArea.Top + 10, 80, 30), Color.White)
                batcher.Draw(ButtonBase, New Rectangle(MsgBoxArea.Right - 100, MsgBoxArea.Top + 50, 80, 30), Color.White)
                batcher.DrawString(DispFont, "OK", New Vector2(MsgBoxArea.Right - 70, MsgBoxArea.Top + 15), Color.White)
                batcher.DrawString(DispFont, "Back", New Vector2(MsgBoxArea.Right - 76, MsgBoxArea.Top + 55), Color.White)

                'Draw text box line
                batcher.DrawRect(New Rectangle(MsgBoxArea.Left + 20, MsgBoxArea.Bottom - 40, MsgBoxArea.Width - 40, 25), New Color(130, 130, 130))
                If InputBoxData <> "" Then batcher.DrawString(DispFont, InputBoxData, New Vector2(MsgBoxArea.Left + 22, MsgBoxArea.Bottom - 38), Color.White)
                Dim start As New Vector2(MsgBoxArea.Left + 22 + DispFont.MeasureString(InputBoxData).X, MsgBoxArea.Bottom - 38)
                batcher.DrawLine(start, start + New Vector2(0, 20), Color.Red, 2)
            Else
                'Draw text
                batcher.DrawString(DispFont, WrapTextDifferently(CurrentMessage.Message, 49, False), MsgBoxArea.Location.ToVector2 + New Vector2(30, 10), Color.White)

                'Get total button length
                Dim tot_len As Single = 0F
                Dim padding As Single = 25.0F
                Dim margin As Single = 30.0F
                For i As Integer = 0 To CurrentMessage.Buttons.Length - 1
                    tot_len += DispFont.MeasureString(CurrentMessage.Buttons(i)).X + padding 'width + padding
                Next
                tot_len += (CurrentMessage.Buttons.Length - 1) * margin 'Add margin

                'Draw buttons
                Dim x_offset As Single = 0F
                For i As Integer = 0 To CurrentMessage.Buttons.Length - 1
                    Dim txt_width As Single = DispFont.MeasureString(CurrentMessage.Buttons(i)).X + padding
                    Dim rect As New Rectangle(MsgBoxArea.Center.X - tot_len / 2 + x_offset, MsgBoxArea.Y + MsgBoxArea.Height * 0.7, txt_width, 30)
                    batcher.Draw(ButtonBase, rect, Color.White)
                    batcher.DrawString(DispFont, CurrentMessage.Buttons(i), rect.Location.ToVector2 + New Vector2(padding / 2, 5), Color.White)

                    'Update offset
                    x_offset += txt_width + margin
                Next
            End If
            batcher.End()
        End Sub

        Public Sub Unload() Implements IFinalRenderDelegate.Unload

        End Sub

        Public Delegate Sub FinalMsgAction(ByVal button As Integer)
        Public Delegate Sub FinalInputAction(ByVal text As String, ByVal button As Integer)

        Private Structure Message
            Public Message As String
            Public Buttons As String()
            Public FinalActionMsgBox As FinalMsgAction
            Public FinalActionInputBox As FinalInputAction
            Public IsInputbox As Boolean
            Public DefaultResponse As String
        End Structure
    End Class
End Namespace