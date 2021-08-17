Imports Emmond.Framework.Entities
Imports Emmond.Framework.Graphics
Imports Emmond.Framework.Level
Imports Emmond.Framework.SceneManager
Imports Emmond.IG
Imports Emmond.IG.Entities
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace UI.Menu
    Public Class PlayRoom
        Inherits Scene

        Public lvl As Level 'Level instance
        Public pl As Player 'Player instance
        Dim trigs As Boolean() = {False, False, False, False}

        Sub New()
            Config = New SceneConfig With {.ID = 7, .ReloadOnSelection = False, .AutoLoadSoundBank = False, .Descriptor = "plr", .ShowLoadingScreen = True, .ShowLSCustom = False}
        End Sub

        Public Overrides Sub Initialize()
            Media.MediaPlayer.Stop()

        End Sub

        Public Overrides Sub LoadContent(Optional parameter As Integer = 0)

            'Load the level
            lvl = LevelParser.GenerateTestLevel
            lvl.Entities.Add(pl)
            lvl.LoadContent()

            'Load Sound
            lvl.MSFX.SFXSoundBank = New Audio.SoundBank(AudioEng, "Content\sound\tst_0.xsb")


            'Prepare the player
            pl.mSpawn = lvl.Map.Spawn
            pl.DontLeaveTheFrickingBoundariesYouIdiot = False
            pl.Initialize()
            pl.LoadContent(1)
        End Sub

        Public Overrides Sub UnloadContent()
            'Set no level to be active

            'Load the level
            lvl.UnloadContent()
            lvl = Nothing

            'Unload the player
            pl = Nothing

        End Sub

        Public Overrides Sub Draw(gameTime As GameTime)
            SpriteBat.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, Nothing, Nothing, Nothing, lvl.Camera.GetMatrix * MainScalingMatrix)

            Dim tsnew As Integer = cTileSize * 2
            For x As UShort = 0 To GameSize.X / cTileSize
                Dim xx As Double = Level.GetMapTilePosition(New Vector2(x * 2, 0), New Vector2(0, 0)).X + 1
                Primitives2D.DrawLine(New Vector2(xx, (tsnew - lvl.Map.Size.Y) * tsnew - 10), New Vector2(xx, 1080), New Color(40, 40, 40, 255), 2)
            Next
            For y As UShort = 0 To GameSize.Y / cTileSize
                Dim yy As Double = Level.GetMapTilePosition(New Vector2(0, y * 2), New Vector2(0, 0)).Y
                Primitives2D.DrawLine(New Vector2(0, 1080 - yy), New Vector2(GameSize.X, 1080 - yy), New Color(40, 40, 40, 255), 2)
            Next

            'Draw Player
            pl.Draw(gameTime)


            '-aiming arrow
            pl.DrawOverlay(gameTime, lvl)

            SpriteBat.End()
        End Sub

        Public Overrides Sub DrawDebug(gameTime As GameTime)
            pl.DrawDebug(gameTime, lvl)
        End Sub

        Public Overrides Sub Update(gameTime As GameTime)
            'Update Player
            pl.Update(gameTime, lvl, trigs)
            lvl.Camera.UpdateCamera(gameTime)
        End Sub
    End Class
End Namespace