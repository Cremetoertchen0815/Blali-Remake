
Imports Emmond.Framework.Camera

Imports Emmond.Framework.SceneManager
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace UI.General

    Public Class ErrorHandler
        Inherits Scene

        'Dim bgm As Media.Song
        'Dim pic As Texture2D
        Dim cam As Camera
        Dim r As Random

        Sub New()
            Config = New SceneConfig With {.ID = 9, .ReloadOnSelection = False, .AutoLoadSoundBank = False, .Descriptor = "err2", .ShowLoadingScreen = False}
        End Sub

        Public Overrides Sub Initialize()
            Loaded = False
            Media.MediaPlayer.Stop()

            cam = New Camera(False)
            cam.DebugCam = True
            cam.AllowHorizontal = True
            cam.AllowVertical = True



            r = New Random
        End Sub

        Public Overrides Sub LoadContent(Optional parameter As Integer = 0)
        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)
            SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, Nothing, MainScalingMatrix * cam.GetMatrix)
            Dim nx As Integer = r.Next(0, 500)
            'If nx = 50 Then SpriteBat.Draw(pic, New Rectangle(Point.Zero, GameSize.ToPoint), Color.White)

            Dim depth As Integer
            Dim textwrapC As String = StaticFunctions.WrapText(DebugFont, ErrorHandlerMessages(2), GameSize.X / 6 * 5, depth)
            Dim textD As String = "Please report to your mom. Or one of the developers. Or one of your non-existing friends."

            Dim yoffset As Integer = (GameSize.Y / 4)
            SpriteBat.DrawString(DebugFont, "You crashed the game. Great job.", New Vector2(((GameSize / 2) - (DebugFont.MeasureString("You crashed the game. Great job.") / 2)).X, yoffset), Color.White)
            yoffset += 50
            SpriteBat.DrawString(DebugFont, ErrorHandlerMessages(0), New Vector2(((GameSize / 2) - (DebugFont.MeasureString(ErrorHandlerMessages(0)) / 2)).X, yoffset), Color.White)
            yoffset += (DebugFont.MeasureString(ErrorHandlerMessages(0)).Y) + 10
            SpriteBat.DrawString(DebugFont, ErrorHandlerMessages(1), New Vector2(((GameSize / 2) - (DebugFont.MeasureString(ErrorHandlerMessages(1)) / 2)).X, yoffset), Color.White)
            yoffset += (DebugFont.MeasureString(ErrorHandlerMessages(1)).Y) + 10
            SpriteBat.DrawString(DebugFont, textwrapC, New Vector2(((GameSize / 2) - (DebugFont.MeasureString(textwrapC) / 2)).X, yoffset), Color.White)
            yoffset += (DebugFont.MeasureString(textwrapC).Y) + 50
            SpriteBat.DrawString(DebugFont, textD, New Vector2(((GameSize / 2) - (DebugFont.MeasureString(textD) / 2)).X, yoffset), Color.White)
            SpriteBat.End()
        End Sub

        Friend firsttick As Boolean = False
        Public Overrides Sub Update(gameTime As GameTime)
            cam.UpdateCamera(gameTime)

            If Not firsttick Then
                firsttick = True
                Automator.Clear()
                Media.MediaPlayer.Stop()
            End If
        End Sub

        Public Overrides Sub UnloadContent()

        End Sub

        Public Overrides Sub DrawDebug(gameTime As GameTime)

        End Sub
    End Class

End Namespace