
Imports Emmond.Framework.Camera
Imports Emmond.Framework.SceneManager
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace UI.General

    Public Class GeneralHandler
        Inherits Scene

        Dim bgm As Media.Song
        Dim pic As Texture2D
        Dim cam As Camera
        Dim r As Random

        Sub New()
            Config = New SceneConfig With {.ID = 666, .ReloadOnSelection = False, .AutoLoadSoundBank = False, .Descriptor = "gen", .ShowLoadingScreen = False, .ShowLSCustom = False}
        End Sub

        Public Overrides Sub Initialize()
            Loaded = False
            Media.MediaPlayer.Stop()
        End Sub

        Public Overrides Sub LoadContent(Optional parameter As Integer = 0)
            bgm = ContentMan.Load(Of Media.Song)("nel_spook1")
            Media.MediaPlayer.IsRepeating = True
            Media.MediaPlayer.Volume = 100
            Media.MediaPlayer.Play(bgm)

            pic = ContentMan.Load(Of Texture2D)("ge_errhnd")

            cam = New Camera(False)
            cam.AllowHorizontal = True
            cam.AllowVertical = True

            r = New Random
        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)
            Dim nx As Integer = r.Next(0, 800)

            Select Case nx
                Case 0
                    SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, Nothing, MainScalingMatrix)
                    SpriteBat.Draw(pic, New Rectangle(Point.Zero, GameSize.ToPoint), Color.White)
                    SpriteBat.End()
                Case 1
                    SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, Nothing, MainScalingMatrix)
                    SpriteBat.DrawString(DebugFont, "No", (GameSize / 2) - (DebugFont.MeasureString("No") / 2), Color.White, 0, DebugFont.MeasureString("No") / 2, Rand.NextDouble * 60, SpriteEffects.None, 0)
                    SpriteBat.End()
                Case 2
                    SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, Nothing, MainScalingMatrix)
                    SpriteBat.DrawString(DebugFont, "No", (GameSize / 2) - (DebugFont.MeasureString("No") / 2), Color.White, 0, DebugFont.MeasureString("No") / 2, Rand.NextDouble * 60, SpriteEffects.FlipHorizontally, 0)
                    SpriteBat.End()
                Case 3
                    SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, Nothing, MainScalingMatrix)
                    SpriteBat.DrawString(DebugFont, "No", (GameSize / 2) - (DebugFont.MeasureString("No") / 2), Color.White, 0, DebugFont.MeasureString("No") / 2, Rand.NextDouble * 60, SpriteEffects.FlipVertically, 0)
                    SpriteBat.End()
                Case Else
                    SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, Nothing, Nothing, Nothing, MainScalingMatrix * cam.GetMatrix)
                    SpriteBat.DrawString(DebugFont, "No", (GameSize / 2) - (DebugFont.MeasureString("No") / 2), Color.White)
                    SpriteBat.End()
            End Select

        End Sub
        Public Overrides Sub Update(gameTime As GameTime)
            cam.Zoom += gameTime.ElapsedGameTime.TotalSeconds / 20
            cam.UpdateCamera(gameTime)
        End Sub

        Public Overrides Sub UnloadContent()

        End Sub

        Public Overrides Sub DrawDebug(gameTime As GameTime)

        End Sub
    End Class

End Namespace