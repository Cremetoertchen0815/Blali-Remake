Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input

Namespace Framework.Input
    <TestState(TestState.NearCompletion)>
    Public Class HintGenerator
        Public Shared Sub RenderHint(input As HintInput, mode As HintMode, location As Vector2, ButtonColor As Color, text As String, font As SpriteFont, Optional UseColorForBoth As Boolean = False)
            Dim co As Color = Color.White
            If UseColorForBoth Then co = ButtonColor
            Dim textheight As Integer = font.MeasureString("LOL!").Y + 10
            Select Case mode
                Case HintMode.Xbox
                    Dim rct As Rectangle = GetXboxSourceRect(input)
                    SpriteBat.Draw(XboxButtons, New Rectangle(location.X - (rct.Width - 33), location.Y, textheight, textheight), rct, ButtonColor)
                    SpriteBat.DrawString(font, text, New Vector2(location.X + textheight + 4, location.Y + 4), co)
                Case HintMode.Keyboard
                    GetKeyName(input, font, location, ButtonColor)
                    SpriteBat.DrawString(font, text, New Vector2(location.X + textheight + 4, location.Y + 4), co)
            End Select
        End Sub

        Public Shared Function GetHintSize(input As HintInput, mode As HintMode, text As String, font As SpriteFont) As Vector2
            Dim textheight As Integer = font.MeasureString("LOL!").Y + 10
            Select Case mode
                Case HintMode.Xbox
                    Dim rct As Rectangle = GetXboxSourceRect(input)
                    Return New Vector2(textheight + 4 + font.MeasureString(text).X + (rct.Width - 33), font.MeasureString(text).Y)
                Case HintMode.Keyboard
                    Dim sz As Vector2 = Vector2.Zero
                    GetKeySize(input, font, sz)
                    Return New Vector2(4 + font.MeasureString(text).X + sz.X, font.MeasureString(text).Y)
                Case Else
                    Return Vector2.Zero
            End Select
        End Function

        Public Shared Sub RenderHint(input As HintInput, mode As HintMode, location As Vector2, font As SpriteFont, ButtonColor As Color)
            Select Case mode
                Case HintMode.Xbox
                    Dim srcrect As Rectangle = GetXboxSourceRect(input)
                    SpriteBat.Draw(XboxButtons, New Rectangle(location.X, location.Y, srcrect.Width, srcrect.Height), srcrect, ButtonColor)
                Case HintMode.Keyboard
                    GetKeyName(input, font, location, ButtonColor)
            End Select
        End Sub

        Private Shared Function GetXboxSourceRect(input As HintInput) As Rectangle
            Select Case input
                Case HintInput.A
                    Return New Rectangle(0, 0, 33, 33)
                Case HintInput.B
                    Return New Rectangle(33, 0, 33, 33)
                Case HintInput.X
                    Return New Rectangle(66, 0, 33, 33)
                Case HintInput.Y
                    Return New Rectangle(99, 0, 33, 33)
                Case HintInput.Start
                    Return New Rectangle(165, 0, 33, 33)
                Case HintInput.LS
                    Return New Rectangle(198, 0, 33, 33)
                Case HintInput.RS
                    Return New Rectangle(231, 0, 33, 33)
                Case HintInput.DUp
                    Return New Rectangle(265, 0, 33, 33)
                Case HintInput.DDown
                    Return New Rectangle(297, 0, 33, 33)
                Case HintInput.DLeft
                    Return New Rectangle(265, 33, 33, 33)
                Case HintInput.DRight
                    Return New Rectangle(297, 33, 33, 33)
                Case HintInput.LB
                    Return New Rectangle(33, 33, 50, 33)
                Case HintInput.RB
                    Return New Rectangle(82, 33, 50, 33)
                Case HintInput.LT
                    Return New Rectangle(198, 33, 33, 33)
                Case HintInput.RT
                    Return New Rectangle(231, 33, 33, 33)
            End Select
        End Function

        Private Shared Sub GetKeyName(input As HintInput, font As SpriteFont, location As Vector2, ButtonColor As Color) '{-1 = Left MB, -2 = Middle MB, -3 = Right MB, -4 = XButton 1, -5 = XButton 2, -6 = Wheel Up, -7 = Wheel Down}
            Dim nr As Integer = -8
            Select Case input
                Case HintInput.A
                    nr = InputMan.KeyIM(2)
                Case HintInput.B
                    nr = InputMan.KeyIM(3)
                Case HintInput.X
                    nr = InputMan.KeyIM(4)
                Case HintInput.Y
                    nr = InputMan.KeyIM(5)
                Case HintInput.Start
                    nr = InputMan.KeyIM(0)
                Case HintInput.LB
                    nr = InputMan.KeyIM(6)
                Case HintInput.RB
                    nr = InputMan.KeyIM(7)
                Case HintInput.LT
                    nr = InputMan.KeyIM(8)
                Case HintInput.RT
                    nr = InputMan.KeyIM(9)
            End Select

            Select Case nr
                Case -1
                    Dim txt As String = "[LB]"
                    SpriteBat.DrawString(font, txt, New Vector2(location.X - (font.MeasureString(txt).X - 33), location.Y + 4), ButtonColor)
                Case -2
                    Dim txt As String = "[MB]"
                    SpriteBat.DrawString(font, txt, New Vector2(location.X - (font.MeasureString(txt).X - 33), location.Y + 4), ButtonColor)
                Case -3
                    Dim txt As String = "[RB]"
                    SpriteBat.DrawString(font, txt, New Vector2(location.X - (font.MeasureString(txt).X - 33), location.Y + 4), ButtonColor)
                Case -4
                    Dim txt As String = "[X1B]"
                    SpriteBat.DrawString(font, txt, New Vector2(location.X - (font.MeasureString(txt).X - 33), location.Y + 4), ButtonColor)
                Case -5
                    Dim txt As String = "[X2B]"
                    SpriteBat.DrawString(font, txt, New Vector2(location.X - (font.MeasureString(txt).X - 33), location.Y + 4), ButtonColor)
                Case -6
                    Dim txt As String = "[Scr. Up]"
                    SpriteBat.DrawString(font, txt, New Vector2(location.X - (font.MeasureString(txt).X - 33), location.Y + 4), ButtonColor)
                Case -7
                    Dim txt As String = "[Scr. Down]"
                    SpriteBat.DrawString(font, txt, New Vector2(location.X - (font.MeasureString(txt).X - 33), location.Y + 4), ButtonColor)
                Case > 0
                    Dim txt As String = "[" & DirectCast(nr, Keys).ToString & "]"
                    SpriteBat.DrawString(font, txt, New Vector2(location.X - (font.MeasureString(txt).X - 33), location.Y + 4), ButtonColor)
            End Select

        End Sub
        Private Shared Sub GetKeySize(input As HintInput, font As SpriteFont, ByRef size As Vector2) '{-1 = Left MB, -2 = Middle MB, -3 = Right MB, -4 = XButton 1, -5 = XButton 2, -6 = Wheel Up, -7 = Wheel Down}
            Dim nr As Integer = -8
            Select Case input
                Case HintInput.A
                    nr = InputMan.KeyIM(2)
                Case HintInput.B
                    nr = InputMan.KeyIM(3)
                Case HintInput.X
                    nr = InputMan.KeyIM(4)
                Case HintInput.Y
                    nr = InputMan.KeyIM(5)
                Case HintInput.Start
                    nr = InputMan.KeyIM(0)
                Case HintInput.LB
                    nr = InputMan.KeyIM(6)
                Case HintInput.RB
                    nr = InputMan.KeyIM(7)
                Case HintInput.LT
                    nr = InputMan.KeyIM(8)
                Case HintInput.RT
                    nr = InputMan.KeyIM(9)
            End Select
            size = Vector2.Zero

            'If nr < -7 Then Return "OwO"
            Select Case nr
                Case -1
                    size = font.MeasureString("[RB]")
                Case -2
                    size = font.MeasureString("[MB]")
                Case -3
                    size = font.MeasureString("[LB]")
                Case -4
                    size = font.MeasureString("[X1]")
                Case -5
                    size = font.MeasureString("[X2]")
                Case -6
                    size = font.MeasureString("[Scr. Up]")
                Case -7
                    size = font.MeasureString("[Scr. Down]")
                Case > 0
                    size = font.MeasureString("[" & DirectCast(nr, Keys).ToString & "]")
            End Select

        End Sub
    End Class
End Namespace