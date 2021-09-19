
Imports Emmond.Framework.Data
Imports Emmond.Framework.Input
Imports Emmond.Framework.Level
Imports Emmond.Framework.SceneManager
Imports Emmond.IG.Entities
Imports Emmond.UI.Ingame
Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Media

Namespace IG.Levels
    Public Class Lvl00
        Inherits LevelClass

        Sub New()
            Config = New SceneConfig With {.ID = 14, .ReloadOnSelection = True, .AutoLoadSoundBank = False, .Descriptor = "tst_0", .ShowLoadingScreen = True, .ShowLSCustom = True, .LSCustomColor = Color.LimeGreen}
        End Sub

        Protected Overrides Sub LvlConfig()

        End Sub

        Protected Overrides Sub LvlLoad()
            lvl.MSFX.BGMusic = ContentMan.Load(Of Song)("sound\bgm\wod_1_full")
            lvl.MSFX.SFXSoundBank = SoundBank

            lvl.Background.Components.Add(New Background.Components.Clouds)

            pl = New Blali(0) With {.mSpawn = lvl.Map.Spawn, .lvlcl = Me}
            pl.Initialize()
        End Sub

        Protected Overrides Function SubLvlConfig(sublvlnr As Integer) As Level
            LevelParser.LoadSubLevel(sublvlnr, lvl, False)
            lvl.LoadContent()

            For Each item In lvl.ItemMan
                If item.Value.Type = 3 Then
                    CType(item.Value, ObjectManager.Objects.SublevelDoor).SublvlLoad = AddressOf SubLvlConfig
                End If
            Next

            Return lvl
        End Function

        Protected Overrides Sub AddDraw(gameTime As GameTime)

        End Sub

        Protected Overrides Sub LvlUpdate(gameTime As GameTime, cstate As GameInputState)

        End Sub

    End Class
End Namespace